using Asp.Versioning;
using IdentityManagement.Application.Features.Administration.Commands;
using IdentityManagement.Application.Features.Administration.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers;

[ApiVersion("1.0")]
[Authorize(Roles = "SuperAdmin,Admin")]
[Route("api/v{version:apiVersion}/roles")]
public class RolesController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto request)
        => HandleResult(await Mediator.Send(new CreateRoleCommand(request)));

    [HttpPut("{roleId:guid}")]
    public async Task<IActionResult> Update(Guid roleId, [FromBody] UpdateRoleRequestDto request)
        => HandleResult(await Mediator.Send(new UpdateRoleCommand(roleId, request)));

    [HttpDelete("{roleId:guid}")]
    public async Task<IActionResult> Delete(Guid roleId)
        => HandleResult(await Mediator.Send(new DeleteRoleCommand(roleId)));

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] SetUserRoleRequestDto request)
        => HandleResult(await Mediator.Send(new AssignRoleCommand(request)));

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] SetUserRoleRequestDto request)
        => HandleResult(await Mediator.Send(new RemoveRoleCommand(request)));
}
