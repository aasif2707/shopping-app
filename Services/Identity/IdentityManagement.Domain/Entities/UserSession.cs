namespace IdentityManagement.Domain.Entities;

/// <summary>
/// Tracks an authenticated user session for auditing and remote sign-out.
/// </summary>
public class UserSession
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string? Device { get; set; }
    public string? Browser { get; set; }
    public string? IpAddress { get; set; }

    public DateTimeOffset LoginDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LogoutDate { get; set; }
    public bool IsActive { get; set; } = true;
}
