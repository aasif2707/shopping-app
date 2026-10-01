using IdentityManagement.Domain.Entities;

namespace IdentityManagement.Application.Common.Interfaces;

/// <summary>
/// Unit of Work aggregating repositories and coordinating a single transaction/commit.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<RefreshToken> RefreshTokens { get; }
    IGenericRepository<UserSession> Sessions { get; }
    IGenericRepository<UserDevice> Devices { get; }
    IGenericRepository<Permission> Permissions { get; }
    IGenericRepository<AuditLog> AuditLogs { get; }
    IGenericRepository<Notification> Notifications { get; }

    IGenericRepository<T> Repository<T>() where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
