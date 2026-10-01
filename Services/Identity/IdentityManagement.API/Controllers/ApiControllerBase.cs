using IdentityManagement.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers;

/// <summary>
/// Base API controller providing MediatR access and consistent Result-to-ActionResult mapping.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected string? IpAddress =>
        Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded)
            ? forwarded.ToString()
            : HttpContext.Connection.RemoteIpAddress?.ToString();

    protected IActionResult HandleResult<T>(Result<T> result)
        => result.Succeeded
            ? Ok(result.Data)
            : BadRequest(new { errors = result.Errors });

    protected IActionResult HandleResult(Result result)
        => result.Succeeded
            ? Ok()
            : BadRequest(new { errors = result.Errors });
}
