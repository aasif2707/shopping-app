using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Administration.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Administration.Commands;

public record ActivateUserCommand(Guid UserId) : IRequest<Result>;
public record DeactivateUserCommand(Guid UserId) : IRequest<Result>;
public record SoftDeleteUserCommand(Guid UserId) : IRequest<Result>;
public record HardDeleteUserCommand(Guid UserId) : IRequest<Result>;
public record UnlockUserCommand(Guid UserId) : IRequest<Result>;

public record CreateRoleCommand(CreateRoleRequestDto Request) : IRequest<Result<RoleDto>>;
public record UpdateRoleCommand(Guid RoleId, UpdateRoleRequestDto Request) : IRequest<Result<RoleDto>>;
public record DeleteRoleCommand(Guid RoleId) : IRequest<Result>;
public record AssignRoleCommand(SetUserRoleRequestDto Request) : IRequest<Result>;
public record RemoveRoleCommand(SetUserRoleRequestDto Request) : IRequest<Result>;

public class ActivateUserCommandHandler(IIdentityService identityService) : IRequestHandler<ActivateUserCommand, Result>
{
    public Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        => identityService.SetUserActivationAsync(request.UserId, true, cancellationToken);
}

public class DeactivateUserCommandHandler(IIdentityService identityService) : IRequestHandler<DeactivateUserCommand, Result>
{
    public Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        => identityService.SetUserActivationAsync(request.UserId, false, cancellationToken);
}

public class SoftDeleteUserCommandHandler(IIdentityService identityService) : IRequestHandler<SoftDeleteUserCommand, Result>
{
    public Task<Result> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
        => identityService.SoftDeleteUserAsync(request.UserId, cancellationToken);
}

public class HardDeleteUserCommandHandler(IIdentityService identityService) : IRequestHandler<HardDeleteUserCommand, Result>
{
    public Task<Result> Handle(HardDeleteUserCommand request, CancellationToken cancellationToken)
        => identityService.HardDeleteUserAsync(request.UserId, cancellationToken);
}

public class UnlockUserCommandHandler(IIdentityService identityService) : IRequestHandler<UnlockUserCommand, Result>
{
    public Task<Result> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        => identityService.UnlockUserAsync(request.UserId, cancellationToken);
}

public class CreateRoleCommandHandler(IIdentityService identityService) : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    public Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        => identityService.CreateRoleAsync(request.Request, cancellationToken);
}

public class UpdateRoleCommandHandler(IIdentityService identityService) : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    public Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        => identityService.UpdateRoleAsync(request.RoleId, request.Request, cancellationToken);
}

public class DeleteRoleCommandHandler(IIdentityService identityService) : IRequestHandler<DeleteRoleCommand, Result>
{
    public Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        => identityService.DeleteRoleAsync(request.RoleId, cancellationToken);
}

public class AssignRoleCommandHandler(IIdentityService identityService) : IRequestHandler<AssignRoleCommand, Result>
{
    public Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        => identityService.AssignRoleAsync(request.Request.UserId, request.Request.RoleName, cancellationToken);
}

public class RemoveRoleCommandHandler(IIdentityService identityService) : IRequestHandler<RemoveRoleCommand, Result>
{
    public Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
        => identityService.RemoveRoleAsync(request.Request.UserId, request.Request.RoleName, cancellationToken);
}
