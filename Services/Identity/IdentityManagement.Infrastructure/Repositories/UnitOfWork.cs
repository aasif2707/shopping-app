using System.Collections.Concurrent;
using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Domain.Entities;
using IdentityManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityManagement.Infrastructure.Repositories;

/// <summary>
/// Unit of Work coordinating repositories and a single SaveChanges/transaction over the DbContext.
/// </summary>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public IGenericRepository<RefreshToken> RefreshTokens => Repository<RefreshToken>();
    public IGenericRepository<UserSession> Sessions => Repository<UserSession>();
    public IGenericRepository<UserDevice> Devices => Repository<UserDevice>();
    public IGenericRepository<Permission> Permissions => Repository<Permission>();
    public IGenericRepository<AuditLog> AuditLogs => Repository<AuditLog>();
    public IGenericRepository<Notification> Notifications => Repository<Notification>();

    public IGenericRepository<T> Repository<T>() where T : class
        => (IGenericRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new GenericRepository<T>(context));

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;
        await context.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
