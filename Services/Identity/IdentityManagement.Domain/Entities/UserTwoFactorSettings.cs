using IdentityManagement.Domain.Enums;

namespace IdentityManagement.Domain.Entities;

/// <summary>
/// Per-user two-factor authentication configuration.
/// </summary>
public class UserTwoFactorSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public bool IsEnabled { get; set; }
    public TwoFactorType PreferredType { get; set; } = TwoFactorType.None;

    public string? AuthenticatorKey { get; set; }
    public string? RecoveryCodes { get; set; }

    public DateTimeOffset? EnabledAt { get; set; }
}
