namespace IdentityManagement.Application.Common.Interfaces;

/// <summary>
/// Provides access to the current authenticated user context.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? IpAddress { get; }
    bool IsAuthenticated { get; }
}
