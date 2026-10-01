using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Authentication.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Authentication.Commands.Register;

public record RegisterCommand(RegisterRequestDto Request, string? IpAddress) : IRequest<Result<AuthResponseDto>>;

public class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => identityService.RegisterAsync(request.Request, request.IpAddress, cancellationToken);
}
