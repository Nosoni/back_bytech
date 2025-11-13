using Application.DTOs.Roles;
using Application.Features.Roles.Commands;
using Application.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Authorization;
using static API.Authorization.Permissions;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [RequirePermission(Roles.Read)]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _mediator.Send(new GetAllRolesQuery());
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [RequirePermission(Roles.Read)]
    public async Task<IActionResult> GetRoleById(string id)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id));
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(Roles.Create)]
    public async Task<IActionResult> CreateRole(RoleRequest request)
    {
        var result = await _mediator.Send(new CreateRoleCommand(request));
        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetRoleById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [RequirePermission(Roles.Update)]
    public async Task<IActionResult> UpdateRole(string id, RoleRequest request)
    {
        var result = await _mediator.Send(new UpdateRoleCommand(id, request));
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [RequirePermission(Roles.Delete)]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id));
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
