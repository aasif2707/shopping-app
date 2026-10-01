using System.Security.Claims;
using Asp.Versioning;
using IdentityManagement.Application.Features.Authentication.Commands.Account;
using IdentityManagement.Application.Features.Authentication.Commands.Login;
using IdentityManagement.Application.Features.Authentication.Commands.Logout;
using IdentityManagement.Application.Features.Authentication.Commands.RefreshToken;
using IdentityManagement.Application.Features.Authentication.Commands.Register;
using IdentityManagement.Application.Features.Authentication.Dtos;
using IdentityManagement.Application.Features.Authentication.Queries.GetProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers;

/// <summary>
/// Authentication endpoints: registration, login, token refresh, logout and current profile.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ApiControllerBase
{
    /// <summary>Registers a new user and returns an access/refresh token pair.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        => HandleResult(await Mediator.Send(new RegisterCommand(request, IpAddress)));

    /// <summary>Authenticates a user via username or email and returns tokens.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        => HandleResult(await Mediator.Send(new LoginCommand(request, IpAddress)));

    /// <summary>Exchanges an expired access token and a valid refresh token for a new token pair.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        => HandleResult(await Mediator.Send(new RefreshTokenCommand(request, IpAddress)));

    /// <summary>Revokes the supplied refresh token (or all of the user's tokens).</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new LogoutCommand(userId.Value, request, IpAddress)));
    }

    /// <summary>Returns the current authenticated user's profile.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new GetProfileQuery(userId.Value)));
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto request)
        => HandleResult(await Mediator.Send(new ConfirmEmailCommand(request)));

    [HttpPost("resend-email-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationRequestDto request)
        => HandleResult(await Mediator.Send(new ResendEmailConfirmationCommand(request)));

    [HttpPost("send-phone-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendPhoneOtp([FromBody] SendPhoneOtpRequestDto request)
        => HandleResult(await Mediator.Send(new SendPhoneOtpCommand(request)));

    [HttpPost("confirm-phone-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPhoneOtp([FromBody] ConfirmPhoneOtpRequestDto request)
        => HandleResult(await Mediator.Send(new ConfirmPhoneOtpCommand(request)));

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        => HandleResult(await Mediator.Send(new ForgotPasswordCommand(request)));

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        => HandleResult(await Mediator.Send(new ResetPasswordCommand(request)));

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new ChangePasswordCommand(userId.Value, request)));
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new UpdateProfileCommand(userId.Value, request)));
    }

    [HttpPut("profile/image")]
    [Authorize]
    public async Task<IActionResult> SetProfileImage([FromBody] SetProfileImageRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new SetProfileImageCommand(userId.Value, request)));
    }

    [HttpDelete("profile/image")]
    [Authorize]
    public async Task<IActionResult> DeleteProfileImage()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new DeleteProfileImageCommand(userId.Value)));
    }

    [HttpPost("change-email")]
    [Authorize]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new ChangeEmailCommand(userId.Value, request)));
    }

    [HttpPost("change-phone")]
    [Authorize]
    public async Task<IActionResult> ChangePhone([FromBody] ChangePhoneNumberRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return HandleResult(await Mediator.Send(new ChangePhoneNumberCommand(userId.Value, request)));
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
