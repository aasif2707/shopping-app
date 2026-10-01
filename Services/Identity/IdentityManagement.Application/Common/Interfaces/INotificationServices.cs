namespace IdentityManagement.Application.Common.Interfaces;

/// <summary>
/// Abstraction for sending transactional emails (confirmation, password reset, OTP).
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstraction for sending SMS messages (OTP, 2FA).
/// </summary>
public interface ISmsService
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
