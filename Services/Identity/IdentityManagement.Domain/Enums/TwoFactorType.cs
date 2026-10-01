namespace IdentityManagement.Domain.Enums;

/// <summary>
/// Supported two-factor authentication delivery mechanisms.
/// </summary>
public enum TwoFactorType
{
    None = 0,
    Email = 1,
    Sms = 2,
    AuthenticatorApp = 3
}
