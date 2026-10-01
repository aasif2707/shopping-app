using IdentityManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace IdentityManagement.Domain.Entities;

/// <summary>
/// Application user extending the ASP.NET Core Identity user with profile,
/// verification, and lifecycle metadata.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    // Profile
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.Unspecified;
    public string? ProfileImageUrl { get; set; }
    public string? Address { get; set; }

    // Email confirmation (custom token workflow in addition to Identity's EmailConfirmed)
    public string? ConfirmationToken { get; set; }
    public DateTimeOffset? ConfirmationTokenExpiry { get; set; }

    // Phone confirmation via OTP (custom, in addition to Identity's PhoneNumberConfirmed)
    public bool PhoneConfirmed { get; set; }
    public string? Otp { get; set; }
    public DateTimeOffset? OtpExpiry { get; set; }

    // Lifecycle / account state
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public ICollection<PasswordHistory> PasswordHistories { get; set; } = new List<PasswordHistory>();
    public UserTwoFactorSettings? TwoFactorSettings { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}
