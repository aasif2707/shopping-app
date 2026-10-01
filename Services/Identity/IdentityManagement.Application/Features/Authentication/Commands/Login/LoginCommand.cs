using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Authentication.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Authentication.Commands.Login;

public record LoginCommand(LoginRequestDto Request, string? IpAddress) : IRequest<Result<AuthResponseDto>>;

public class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        => identityService.LoginAsync(request.Request, request.IpAddress, cancellationToken);
}
