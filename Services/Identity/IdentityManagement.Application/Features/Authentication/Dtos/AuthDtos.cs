using IdentityManagement.Domain.Enums;

namespace IdentityManagement.Application.Features.Authentication.Dtos;

public record RegisterRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber,
    DateOnly? DateOfBirth,
    Gender Gender,
    string? Address);

public record LoginRequestDto(
    string UserNameOrEmail,
    string Password,
    bool RememberMe);

public record RefreshTokenRequestDto(
    string AccessToken,
    string RefreshToken);

public record LogoutRequestDto(
    string RefreshToken,
    bool AllDevices);

public record AuthResponseDto(
    Guid UserId,
    string UserName,
    string Email,
    string FullName,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    IReadOnlyList<string> Roles);

public record UserProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string? PhoneNumber,
    DateOnly? DateOfBirth,
    Gender Gender,
    string? Address,
    string? ProfileImageUrl,
    bool EmailConfirmed,
    bool PhoneConfirmed);

public record ConfirmEmailRequestDto(
    string Email,
    string Token);

public record ResendEmailConfirmationRequestDto(
    string Email);

public record SendPhoneOtpRequestDto(
    string PhoneNumber);

public record ConfirmPhoneOtpRequestDto(
    string PhoneNumber,
    string Otp);

public record UpdateProfileRequestDto(
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth,
    Gender Gender,
    string? Address);

public record SetProfileImageRequestDto(
    string ProfileImageUrl);

public record ChangeEmailRequestDto(
    string NewEmail,
    string CurrentPassword);

public record ChangePhoneNumberRequestDto(
    string NewPhoneNumber,
    string Otp);

public record ForgotPasswordRequestDto(
    string Email);

public record ResetPasswordRequestDto(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword);

public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword);
