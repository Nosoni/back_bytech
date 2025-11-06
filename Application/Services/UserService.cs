using System;
using Application.Common;
using Application.DTOs.Roles;
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        public async Task<Result<UserResponse>> CreateUserAsync(UserRequest dto)
        {
            // Validaciones para creación
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return Result<UserResponse>.Fail("El nombre de usuario es requerido");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return Result<UserResponse>.Fail("El email es requerido");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return Result<UserResponse>.Fail("La contraseña es requerida");

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Result<UserResponse>.Fail("Email ya registrado");

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<UserResponse>.Fail(
                    "Error al crear usuario",
                    result.Errors.Select(e => e.Description).ToList()
                );

            // Asignar roles si se proporcionaron
            if (dto.RoleIds != null && dto.RoleIds.Any())
            {
                var roleAssignmentResult = await AssignRolesToUserAsync(user, dto.RoleIds);
                if (!roleAssignmentResult.Success)
                {
                    // Si falla la asignación de roles, eliminar el usuario creado
                    await _userManager.DeleteAsync(user);
                    return Result<UserResponse>.Fail(roleAssignmentResult.Message ?? "Error al asignar roles", roleAssignmentResult.Errors);
                }
            }

            // Obtener roles asignados para la respuesta
            var roles = await GetUserRolesAsync(user);

            return Result<UserResponse>.OK(new UserResponse
            {
                Id = user.Id.ToString(),
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles
            });
        }

        public async Task<Result<UserResponse>> UpdateUserAsync(string userId, UserRequest dto)
        {
            var hasChange = false;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<UserResponse>.Fail("Usuario no encontrado");

            if (!string.IsNullOrEmpty(dto.UserName))
            {
                user.UserName = dto.UserName;
                hasChange = true;
            }

            if (!string.IsNullOrEmpty(dto.Email))
            {
                var emailExists = await _userManager.FindByEmailAsync(dto.Email);
                if (emailExists != null && emailExists.Id.ToString() != userId)
                    return Result<UserResponse>.Fail("Email ya está en uso");

                user.Email = dto.Email;
                hasChange = true;
            }

            // Actualizar Password si viene (solo admin puede hacerlo)
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                    return Result<UserResponse>.Fail(
                        "Error al eliminar la contraseña actual",
                        removePasswordResult.Errors.Select(e => e.Description).ToList()
                    );

                var addPasswordResult = await _userManager.AddPasswordAsync(user, dto.Password);
                if (!addPasswordResult.Succeeded)
                    return Result<UserResponse>.Fail(
                        "Error al actualizar la contraseña",
                        addPasswordResult.Errors.Select(e => e.Description).ToList()
                    );
            }

            // Actualizar roles si se proporcionaron
            if (dto.RoleIds != null)
            {
                var roleUpdateResult = await UpdateUserRolesAsync(user, dto.RoleIds);
                if (!roleUpdateResult.Success)
                    return Result<UserResponse>.Fail(roleUpdateResult.Message ?? "Error al actualizar roles", roleUpdateResult.Errors);
            }

            // Actualizar usuario (solo si hay cambios en UserName o Email)
            if (hasChange)
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return Result<UserResponse>.Fail(
                        "Error al actualizar usuario",
                        result.Errors.Select(e => e.Description).ToList()
                    );
            }

            var userRoles = await GetUserRolesAsync(user);

            return Result<UserResponse>.OK(new UserResponse
            {
                Id = user.Id.ToString(),
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = userRoles
            });
        }

        public async Task<Result<UserResponse>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<UserResponse>.Fail("Usuario no encontrado");

            var roles = await GetUserRolesAsync(user);

            return Result<UserResponse>.OK(new UserResponse
            {
                Id = user.Id.ToString(),
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles
            });
        }

        public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userResponses = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await GetUserRolesAsync(user);
                userResponses.Add(new UserResponse
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Roles = roles
                });
            }

            return Result<IEnumerable<UserResponse>>.OK(userResponses);
        }

        public async Task<Result<bool>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Fail("Usuario no encontrado");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Result<bool>.Fail(
                    "Error al eliminar usuario",
                    result.Errors.Select(e => e.Description).ToList()
                );

            return Result<bool>.OK(true);
        }

        // Métodos auxiliares privados
        private async Task<List<RoleResponse>> GetUserRolesAsync(ApplicationUser user)
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

        private async Task<Result<bool>> AssignRolesToUserAsync(ApplicationUser user, List<Guid> roleIds)
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

        private async Task<Result<bool>> UpdateUserRolesAsync(ApplicationUser user, List<Guid> newRoleIds)
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
}
