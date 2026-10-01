using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Administration.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Administration.Queries;

public record GetUsersQuery(UserSearchRequestDto Request) : IRequest<Result<IReadOnlyList<UserListItemDto>>>;
public record GetUserRolesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<string>>>;

public class GetUsersQueryHandler(IIdentityService identityService) : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserListItemDto>>>
{
    public Task<Result<IReadOnlyList<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        => identityService.GetUsersAsync(request.Request, cancellationToken);
}

public class GetUserRolesQueryHandler(IIdentityService identityService) : IRequestHandler<GetUserRolesQuery, Result<IReadOnlyList<string>>>
{
    public Task<Result<IReadOnlyList<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        => identityService.GetUserRolesAsync(request.UserId, cancellationToken);
}
