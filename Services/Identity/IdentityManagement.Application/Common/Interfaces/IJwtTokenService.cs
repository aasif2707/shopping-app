using System.Security.Claims;
using IdentityManagement.Domain.Entities;

namespace IdentityManagement.Application.Common.Interfaces;

/// <summary>
/// Result of generating an access token pair.
/// </summary>
public record TokenResult(string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);

/// <summary>
/// Abstraction over JWT access-token and refresh-token generation/validation.
/// </summary>
public interface IJwtTokenService
{
    Task<TokenResult> GenerateTokensAsync(ApplicationUser user, bool rememberMe, string? ipAddress, CancellationToken cancellationToken = default);

    string GenerateRefreshToken();

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
