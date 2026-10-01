using IdentityManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace IdentityManagement.Infrastructure.Services;

/// <summary>
/// Development email service that logs messages. Replace with a real SMTP/SendGrid provider in production.
/// </summary>
public class LoggingEmailService(ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("EMAIL -> To: {To} | Subject: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Development SMS service that logs messages. Replace with Twilio/etc. in production.
/// </summary>
public class LoggingSmsService(ILogger<LoggingSmsService> logger) : ISmsService
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("SMS -> To: {Phone} | Message: {Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
}
