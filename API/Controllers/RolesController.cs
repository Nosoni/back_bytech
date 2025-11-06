using Application.DTOs.Roles;
using Application.Features.Roles.Commands;
using Application.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _mediator.Send(new GetAllRolesQuery());
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(string id)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id));
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole(RoleRequest request)
    {
        var result = await _mediator.Send(new CreateRoleCommand(request));
        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetRoleById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(string id, RoleRequest request)
    {
        var result = await _mediator.Send(new UpdateRoleCommand(id, request));
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id));
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
