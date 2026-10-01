namespace IdentityManagement.Application.Common.Settings;

/// <summary>
/// Strongly-typed JWT configuration bound from appsettings ("JwtSettings" section).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 7;
    public int RememberMeRefreshTokenExpiryDays { get; set; } = 30;
}
