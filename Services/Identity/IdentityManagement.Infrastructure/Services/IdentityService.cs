using AutoMapper;
using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Administration.Dtos;
using IdentityManagement.Application.Features.Authentication.Dtos;
using IdentityManagement.Domain.Entities;
using IdentityManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityManagement.Infrastructure.Services;

/// <summary>
/// Implements registration, login, refresh and logout by orchestrating Identity,
/// the JWT token service and the refresh-token store.
/// </summary>
public class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenService jwtTokenService,
    AppDbContext dbContext,
    IMapper mapper,
    IEmailService emailService,
    ISmsService smsService,
    ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result<AuthResponseDto>.Failure("Email is already registered.");

        if (await userManager.FindByNameAsync(request.UserName) is not null)
            return Result<AuthResponseDto>.Failure("Username is already taken.");

        var user = mapper.Map<ApplicationUser>(request);
        user.CreatedAt = DateTimeOffset.UtcNow;
        user.IsActive = true;

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<AuthResponseDto>.Failure(createResult.Errors.Select(e => e.Description));

        await userManager.AddToRoleAsync(user, Domain.Constants.RoleConstants.User);

        // Generate email confirmation token (custom + Identity flow).
        user.ConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        user.ConfirmationTokenExpiry = DateTimeOffset.UtcNow.AddHours(24);
        await userManager.UpdateAsync(user);

        var response = await BuildAuthResponseAsync(user, rememberMe: false, ipAddress, cancellationToken);
        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = request.UserNameOrEmail.Contains('@')
            ? await userManager.FindByEmailAsync(request.UserNameOrEmail)
            : await userManager.FindByNameAsync(request.UserNameOrEmail);

        if (user is null || user.IsDeleted || !user.IsActive)
            return Result<AuthResponseDto>.Failure("Invalid credentials.");

        var signIn = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (signIn.IsLockedOut)
            return Result<AuthResponseDto>.Failure("Account is locked due to multiple failed attempts. Try again later.");

        if (!signIn.Succeeded)
            return Result<AuthResponseDto>.Failure("Invalid credentials.");

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await userManager.UpdateAsync(user);

        var response = await BuildAuthResponseAsync(user, request.RememberMe, ipAddress, cancellationToken);
        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var principal = jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            return Result<AuthResponseDto>.Failure("Invalid access token.");

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Result<AuthResponseDto>.Failure("Invalid access token.");

        var stored = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId == userId, cancellationToken);

        if (stored is null || !stored.IsActive)
            return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.");

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result<AuthResponseDto>.Failure("User is no longer active.");

        // Rotate the refresh token.
        stored.RevokedAt = DateTimeOffset.UtcNow;
        stored.RevokedByIp = ipAddress;
        stored.ReasonRevoked = "Rotated on refresh";

        var response = await BuildAuthResponseAsync(user, stored.IsRememberMe, ipAddress, cancellationToken, replacing: stored);
        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result> LogoutAsync(Guid userId, LogoutRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var query = dbContext.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null);

        if (!request.AllDevices)
            query = query.Where(t => t.Token == request.RefreshToken);

        var tokens = await query.ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = request.AllDevices ? "Logout all devices" : "Logout";
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result<UserProfileDto>.Failure("User not found.");

        return Result<UserProfileDto>.Success(mapper.Map<UserProfileDto>(user));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted)
            return Result.Failure("Invalid confirmation request.");

        if (user.EmailConfirmed)
            return Result.Success();

        if (user.ConfirmationToken != request.Token || user.ConfirmationTokenExpiry is null || user.ConfirmationTokenExpiry < DateTimeOffset.UtcNow)
            return Result.Failure("Invalid or expired confirmation token.");

        var confirm = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!confirm.Succeeded)
            return Result.Failure(confirm.Errors.Select(e => e.Description));

        user.ConfirmationToken = null;
        user.ConfirmationTokenExpiry = null;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> ResendEmailConfirmationAsync(ResendEmailConfirmationRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted || user.EmailConfirmed)
            return Result.Success();

        user.ConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        user.ConfirmationTokenExpiry = DateTimeOffset.UtcNow.AddHours(24);
        await userManager.UpdateAsync(user);

        await emailService.SendAsync(user.Email!, "Confirm your email",
            $"Use this token to confirm your email: {user.ConfirmationToken}", cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SendPhoneOtpAsync(SendPhoneOtpRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber && !x.IsDeleted, cancellationToken);
        if (user is null)
            return Result.Failure("User not found for this phone number.");

        var otp = Random.Shared.Next(100000, 999999).ToString();
        user.Otp = otp;
        user.OtpExpiry = DateTimeOffset.UtcNow.AddMinutes(10);
        user.PhoneNumber = request.PhoneNumber;
        await userManager.UpdateAsync(user);

        await smsService.SendAsync(request.PhoneNumber, $"Your verification code is {otp}", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ConfirmPhoneOtpAsync(ConfirmPhoneOtpRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber && !x.IsDeleted, cancellationToken);
        if (user is null)
            return Result.Failure("User not found for this phone number.");

        if (user.Otp != request.Otp || user.OtpExpiry is null || user.OtpExpiry < DateTimeOffset.UtcNow)
            return Result.Failure("Invalid or expired OTP.");

        user.PhoneConfirmed = true;
        user.PhoneNumberConfirmed = true;
        user.Otp = null;
        user.OtpExpiry = null;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted)
            return Result.Success();

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        await emailService.SendAsync(request.Email, "Reset your password", $"Password reset token: {token}", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted)
            return Result.Failure("Invalid reset request.");

        var reset = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!reset.Succeeded)
            return Result.Failure(reset.Errors.Select(e => e.Description));

        await SavePasswordHistoryAsync(user, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        var change = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!change.Succeeded)
            return Result.Failure(change.Errors.Select(e => e.Description));

        await SavePasswordHistoryAsync(user, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result<UserProfileDto>.Failure("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DateOfBirth = request.DateOfBirth;
        user.Gender = request.Gender;
        user.Address = request.Address;

        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded)
            return Result<UserProfileDto>.Failure(update.Errors.Select(e => e.Description));

        return Result<UserProfileDto>.Success(mapper.Map<UserProfileDto>(user));
    }

    public async Task<Result> ChangeEmailAsync(Guid userId, ChangeEmailRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        var verifyPassword = await signInManager.CheckPasswordSignInAsync(user, request.CurrentPassword, false);
        if (!verifyPassword.Succeeded)
            return Result.Failure("Current password is invalid.");

        var token = await userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
        var result = await userManager.ChangeEmailAsync(user, request.NewEmail, token);
        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        user.EmailConfirmed = false;
        user.ConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        user.ConfirmationTokenExpiry = DateTimeOffset.UtcNow.AddHours(24);

        var usernameSet = await userManager.SetUserNameAsync(user, request.NewEmail);
        if (!usernameSet.Succeeded)
            logger.LogWarning("Failed to align username with email for user {UserId}", user.Id);

        await userManager.UpdateAsync(user);
        await emailService.SendAsync(request.NewEmail, "Confirm your new email", $"Verification token: {user.ConfirmationToken}", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangePhoneNumberAsync(Guid userId, ChangePhoneNumberRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        if (user.Otp != request.Otp || user.OtpExpiry is null || user.OtpExpiry < DateTimeOffset.UtcNow)
            return Result.Failure("Invalid or expired OTP.");

        user.PhoneNumber = request.NewPhoneNumber;
        user.PhoneConfirmed = true;
        user.PhoneNumberConfirmed = true;
        user.Otp = null;
        user.OtpExpiry = null;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result<IReadOnlyList<UserListItemDto>>> GetUsersAsync(UserSearchRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = userManager.Users.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                (x.UserName != null && x.UserName.Contains(search)) ||
                (x.Email != null && x.Email.Contains(search)) ||
                x.FirstName.Contains(search) ||
                x.LastName.Contains(search));
        }

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (request.EmailConfirmed.HasValue)
            query = query.Where(x => x.EmailConfirmed == request.EmailConfirmed.Value);

        var page = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > 200 ? 20 : request.PageSize;

        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new UserListItemDto(
                x.Id,
                x.UserName ?? string.Empty,
                x.Email ?? string.Empty,
                x.FullName,
                x.PhoneNumber,
                x.IsActive,
                x.EmailConfirmed,
                x.PhoneConfirmed,
                x.CreatedAt,
                x.LastLoginAt))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<UserListItemDto>>.Success(users);
    }

    public async Task<Result> SetUserActivationAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        user.IsActive = isActive;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure("User not found.");

        user.IsDeleted = true;
        user.IsActive = false;
        user.DeletedAt = DateTimeOffset.UtcNow;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> HardDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> UnlockUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        var resetAccess = await userManager.ResetAccessFailedCountAsync(user);
        var unlock = await userManager.SetLockoutEndDateAsync(user, null);

        if (!resetAccess.Succeeded)
            return Result.Failure(resetAccess.Errors.Select(e => e.Description));

        return unlock.Succeeded ? Result.Success() : Result.Failure(unlock.Errors.Select(e => e.Description));
    }

    public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        if (await roleManager.RoleExistsAsync(request.Name))
            return Result<RoleDto>.Failure("Role already exists.");

        var role = new ApplicationRole(request.Name)
        {
            Description = request.Description,
            IsSystemRole = false
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return Result<RoleDto>.Failure(result.Errors.Select(e => e.Description));

        return Result<RoleDto>.Success(new RoleDto(role.Id, role.Name ?? string.Empty, role.Description, role.IsSystemRole));
    }

    public async Task<Result<RoleDto>> UpdateRoleAsync(Guid roleId, UpdateRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role is null)
            return Result<RoleDto>.Failure("Role not found.");

        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpperInvariant();
        role.Description = request.Description;

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return Result<RoleDto>.Failure(result.Errors.Select(e => e.Description));

        return Result<RoleDto>.Success(new RoleDto(role.Id, role.Name ?? string.Empty, role.Description, role.IsSystemRole));
    }

    public async Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role is null)
            return Result.Failure("Role not found.");

        if (role.IsSystemRole)
            return Result.Failure("System roles cannot be deleted.");

        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        if (!await roleManager.RoleExistsAsync(roleName))
            return Result.Failure("Role not found.");

        if (await userManager.IsInRoleAsync(user, roleName))
            return Result.Success();

        var result = await userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> RemoveRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        if (!await userManager.IsInRoleAsync(user, roleName))
            return Result.Success();

        var result = await userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result<IReadOnlyList<string>>.Failure("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        return Result<IReadOnlyList<string>>.Success(roles.ToList());
    }

    public async Task<Result> SetProfileImageAsync(Guid userId, SetProfileImageRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        user.ProfileImageUrl = request.ProfileImageUrl;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> DeleteProfileImageAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result.Failure("User not found.");

        user.ProfileImageUrl = null;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    private async Task SavePasswordHistoryAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            return;

        await dbContext.PasswordHistory.AddAsync(new PasswordHistory
        {
            UserId = user.Id,
            PasswordHash = user.PasswordHash,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(
        ApplicationUser user, bool rememberMe, string? ipAddress, CancellationToken cancellationToken, RefreshToken? replacing = null)
    {
        var tokens = await jwtTokenService.GenerateTokensAsync(user, rememberMe, ipAddress, cancellationToken);

        var refresh = new RefreshToken
        {
            UserId = user.Id,
            Token = tokens.RefreshToken,
            ExpiresAt = tokens.RefreshTokenExpiresAt,
            CreatedByIp = ipAddress,
            IsRememberMe = rememberMe
        };

        if (replacing is not null)
            replacing.ReplacedByToken = refresh.Token;

        await dbContext.RefreshTokens.AddAsync(refresh, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var roles = await userManager.GetRolesAsync(user);

        return new AuthResponseDto(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.FullName,
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAt,
            roles.ToList());
    }
}
