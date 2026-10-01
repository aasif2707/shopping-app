using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Administration.Dtos;
using IdentityManagement.Application.Features.Authentication.Dtos;

namespace IdentityManagement.Application.Common.Interfaces;

/// <summary>
/// Application-facing abstraction over ASP.NET Core Identity operations,
/// keeping the Application layer free of direct Identity/EF dependencies.
/// </summary>
public interface IIdentityService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(Guid userId, LogoutRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> ConfirmEmailAsync(ConfirmEmailRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> ResendEmailConfirmationAsync(ResendEmailConfirmationRequestDto request, CancellationToken cancellationToken = default);

    Task<Result> SendPhoneOtpAsync(SendPhoneOtpRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmPhoneOtpAsync(ConfirmPhoneOtpRequestDto request, CancellationToken cancellationToken = default);

    Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default);

    Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> ChangeEmailAsync(Guid userId, ChangeEmailRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> ChangePhoneNumberAsync(Guid userId, ChangePhoneNumberRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> SetProfileImageAsync(Guid userId, SetProfileImageRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> DeleteProfileImageAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<UserListItemDto>>> GetUsersAsync(UserSearchRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> SetUserActivationAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);
    Task<Result> SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> HardDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UnlockUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<RoleDto>> UpdateRoleAsync(Guid roleId, UpdateRoleRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
    Task<Result> RemoveRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
