using System;
using Application.Common;
using Application.DTOs.Roles;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<RoleResponse>> CreateRoleAsync(RoleRequest dto)
    {
        var existingRole = await _roleManager.FindByNameAsync(dto.Name);
        if (existingRole != null)
            return Result<RoleResponse>.Fail("Ya existe un rol con ese nombre");

        var role = new ApplicationRole
        {
            Name = dto.Name
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return Result<RoleResponse>.Fail(
                "Error al crear rol",
                result.Errors.Select(e => e.Description).ToList()
            );

        return Result<RoleResponse>.OK(new RoleResponse
        {
            Id = role.Id.ToString(),
            Name = role.Name!
        });
    }

    public async Task<Result<RoleResponse>> UpdateRoleAsync(string roleId, RoleRequest dto)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
            return Result<RoleResponse>.Fail("Rol no encontrado");

        var existingRole = await _roleManager.FindByNameAsync(dto.Name);
        if (existingRole != null && existingRole.Id.ToString() != roleId)
            return Result<RoleResponse>.Fail("Ya existe un rol con ese nombre");

        role.Name = dto.Name;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return Result<RoleResponse>.Fail(
                "Error al actualizar rol",
                result.Errors.Select(e => e.Description).ToList()
            );

        return Result<RoleResponse>.OK(new RoleResponse
        {
            Id = role.Id.ToString(),
            Name = role.Name!
        });
    }

    public async Task<Result<RoleResponse>> GetRoleByIdAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
            return Result<RoleResponse>.Fail("Rol no encontrado");

        return Result<RoleResponse>.OK(new RoleResponse
        {
            Id = role.Id.ToString(),
            Name = role.Name!
        });
    }

    public Task<Result<IEnumerable<RoleResponse>>> GetAllRolesAsync()
    {
        var roles = _roleManager.Roles.ToList();
        var roleResponses = roles.Select(r => new RoleResponse
        {
            Id = r.Id.ToString(),
            Name = r.Name!
        });

        return Task.FromResult(Result<IEnumerable<RoleResponse>>.OK(roleResponses));
    }

    public async Task<Result<bool>> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
            return Result<bool>.Fail("Rol no encontrado");

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return Result<bool>.Fail(
                "Error al eliminar rol",
                result.Errors.Select(e => e.Description).ToList()
            );

        return Result<bool>.OK(true);
    }
}
