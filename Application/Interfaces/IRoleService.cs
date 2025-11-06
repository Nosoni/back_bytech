using System;
using Application.Common;
using Application.DTOs.Roles;

namespace Application.Interfaces;

public interface IRoleService
{
    Task<Result<RoleResponse>> CreateRoleAsync(RoleRequest dto);
    Task<Result<RoleResponse>> UpdateRoleAsync(string roleId, RoleRequest dto);
    Task<Result<RoleResponse>> GetRoleByIdAsync(string roleId);
    Task<Result<IEnumerable<RoleResponse>>> GetAllRolesAsync();
    Task<Result<bool>> DeleteRoleAsync(string roleId);
}
