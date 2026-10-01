using System.Text.Json;
using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityManagement.Infrastructure.Persistence.Interceptors;

/// <summary>
/// SaveChanges interceptor that records create/update/delete operations into the AuditLogs table.
/// </summary>
public class AuditSaveChangesInterceptor(ICurrentUserService currentUser) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is not null)
            WriteAuditEntries(context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            WriteAuditEntries(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    private void WriteAuditEntries(DbContext context)
    {
        var audits = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Never audit the audit log itself.
            if (entry.Entity is AuditLog)
                continue;

            if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                audits.Add(new AuditLog
                {
                    Action = entry.State.ToString(),
                    Entity = entry.Entity.GetType().Name,
                    EntityId = TryGetKey(entry),
                    OldValues = entry.State == EntityState.Added ? null : Serialize(entry, original: true),
                    NewValues = entry.State == EntityState.Deleted ? null : Serialize(entry, original: false),
                    UserId = currentUser.UserId,
                    IpAddress = currentUser.IpAddress,
                    Timestamp = DateTimeOffset.UtcNow
                });
            }
        }

        if (audits.Count > 0)
            context.Set<AuditLog>().AddRange(audits);
    }

    private static string? TryGetKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key is null) return null;
        var values = key.Properties.Select(p => entry.Property(p.Name).CurrentValue?.ToString());
        return string.Join(",", values);
    }

    private static string Serialize(EntityEntry entry, bool original)
    {
        var dict = entry.Properties.ToDictionary(
            p => p.Metadata.Name,
            p => original ? p.OriginalValue : p.CurrentValue);
        return JsonSerializer.Serialize(dict);
    }
}
