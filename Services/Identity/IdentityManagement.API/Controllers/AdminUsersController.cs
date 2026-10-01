using Asp.Versioning;
using IdentityManagement.Application.Features.Administration.Commands;
using IdentityManagement.Application.Features.Administration.Dtos;
using IdentityManagement.Application.Features.Administration.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers;

[ApiVersion("1.0")]
[Authorize(Roles = "SuperAdmin,Admin")]
[Route("api/v{version:apiVersion}/admin/users")]
public class AdminUsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] bool? isActive, [FromQuery] bool? emailConfirmed, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        => HandleResult(await Mediator.Send(new GetUsersQuery(new UserSearchRequestDto(search, isActive, emailConfirmed, pageNumber, pageSize))));

    [HttpGet("{userId:guid}/roles")]
    public async Task<IActionResult> GetUserRoles(Guid userId)
        => HandleResult(await Mediator.Send(new GetUserRolesQuery(userId)));

    [HttpPost("{userId:guid}/activate")]
    public async Task<IActionResult> Activate(Guid userId)
        => HandleResult(await Mediator.Send(new ActivateUserCommand(userId)));

    [HttpPost("{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid userId)
        => HandleResult(await Mediator.Send(new DeactivateUserCommand(userId)));

    [HttpPost("{userId:guid}/unlock")]
    public async Task<IActionResult> Unlock(Guid userId)
        => HandleResult(await Mediator.Send(new UnlockUserCommand(userId)));

    [HttpDelete("{userId:guid}/soft")]
    public async Task<IActionResult> SoftDelete(Guid userId)
        => HandleResult(await Mediator.Send(new SoftDeleteUserCommand(userId)));

    [HttpDelete("{userId:guid}/hard")]
    public async Task<IActionResult> HardDelete(Guid userId)
        => HandleResult(await Mediator.Send(new HardDeleteUserCommand(userId)));
}
