using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Authentication.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(RefreshTokenRequestDto Request, string? IpAddress) : IRequest<Result<AuthResponseDto>>;

public class RefreshTokenCommandHandler(IIdentityService identityService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    public Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => identityService.RefreshTokenAsync(request.Request, request.IpAddress, cancellationToken);
}
