using Application.DTOs.Roles;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _roleService.GetAllRolesAsync();
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(string id)
    {
        var result = await _roleService.GetRoleByIdAsync(id);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole(RoleRequest dto)
    {
        var result = await _roleService.CreateRoleAsync(dto);
        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetRoleById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(string id, RoleRequest dto)
    {
        var result = await _roleService.UpdateRoleAsync(id, dto);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
