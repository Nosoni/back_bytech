using Application.Common;
using Application.DTOs.Roles;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRoleService
{
    Task<List<RoleResponse>> GetUserRolesAsync(ApplicationUser user);
    Task<Result<bool>> AssignRolesToUserAsync(ApplicationUser user, List<Guid> roleIds);
    Task<Result<bool>> UpdateUserRolesAsync(ApplicationUser user, List<Guid> newRoleIds);
}
