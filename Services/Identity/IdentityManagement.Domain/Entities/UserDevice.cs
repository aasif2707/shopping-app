namespace IdentityManagement.Domain.Entities;

/// <summary>
/// A remembered device/browser for a user (trusted-device workflows).
/// </summary>
public class UserDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string DeviceId { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public string? IpAddress { get; set; }

    public bool IsTrusted { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastSeenAt { get; set; } = DateTimeOffset.UtcNow;
}
