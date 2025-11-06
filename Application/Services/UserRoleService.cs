using Application.Common;
using Application.DTOs.Roles;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserRoleService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IUserRoleService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<List<RoleResponse>> GetUserRolesAsync(ApplicationUser user)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        var roles = new List<RoleResponse>();

        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                roles.Add(new RoleResponse
                {
                    Id = role.Id.ToString(),
                    Name = role.Name!
                });
            }
        }

        return roles;
    }

    public async Task<Result<bool>> AssignRolesToUserAsync(ApplicationUser user, List<Guid> roleIds)
    {
        foreach (var roleId in roleIds)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return Result<bool>.Fail($"Rol con ID {roleId} no encontrado");

            var result = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!result.Succeeded)
                return Result<bool>.Fail(
                    $"Error al asignar rol {role.Name}",
                    result.Errors.Select(e => e.Description).ToList()
                );
        }

        return Result<bool>.OK(true);
    }

    public async Task<Result<bool>> UpdateUserRolesAsync(ApplicationUser user, List<Guid> newRoleIds)
    {
        // Obtener roles actuales
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Remover todos los roles actuales
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return Result<bool>.Fail(
                    "Error al remover roles actuales",
                    removeResult.Errors.Select(e => e.Description).ToList()
                );
        }

        // Asignar nuevos roles
        if (newRoleIds.Any())
        {
            return await AssignRolesToUserAsync(user, newRoleIds);
        }

        return Result<bool>.OK(true);
    }
}
