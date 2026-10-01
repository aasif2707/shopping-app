using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Authentication.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Authentication.Queries.GetProfile;

public record GetProfileQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;

public class GetProfileQueryHandler(IIdentityService identityService)
    : IRequestHandler<GetProfileQuery, Result<UserProfileDto>>
{
    public Task<Result<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        => identityService.GetProfileAsync(request.UserId, cancellationToken);
}
