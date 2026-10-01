using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Authentication.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Authentication.Commands.Logout;

public record LogoutCommand(Guid UserId, LogoutRequestDto Request, string? IpAddress) : IRequest<Result>;

public class LogoutCommandHandler(IIdentityService identityService)
    : IRequestHandler<LogoutCommand, Result>
{
    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        => identityService.LogoutAsync(request.UserId, request.Request, request.IpAddress, cancellationToken);
}
