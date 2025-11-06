using Application.Common;
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands;

public record UpdateUserCommand(string Id, UserRequest Request) : IRequest<Result<UserResponse>>;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserCommandValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del usuario es requerido")
            .Must(BeValidGuid).WithMessage("El ID debe ser un GUID válido");

        RuleFor(x => x.Request.UserName)
            .Length(3, 50).WithMessage("El nombre de usuario debe tener entre 3 y 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Request.UserName));

        RuleFor(x => x.Request.Email)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MustAsync(async (cmd, email, ct) => await BeUniqueEmailForUpdate(cmd.Id, email, ct))
            .WithMessage("El email ya está en uso")
            .When(x => !string.IsNullOrEmpty(x.Request.Email));

        RuleFor(x => x.Request.Password)
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Request.Password));
    }

    private bool BeValidGuid(string id) => Guid.TryParse(id, out _);

    private async Task<bool> BeUniqueEmailForUpdate(string userId, string? email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email)) return true;
        
        var existingUser = await _userManager.FindByEmailAsync(email);
        return existingUser == null || existingUser.Id.ToString() == userId;
    }
}

public class UpdateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUserRoleService userRoleService) : IRequestHandler<UpdateUserCommand, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserRoleService _userRoleService = userRoleService;

    public async Task<Result<UserResponse>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.Id);
        if (user == null)
            return Result<UserResponse>.Fail("Usuario no encontrado");

        var hasChange = false;

        // Actualizar UserName
        if (!string.IsNullOrEmpty(command.Request.UserName))
        {
            user.UserName = command.Request.UserName;
            hasChange = true;
        }

        // Actualizar Email
        if (!string.IsNullOrEmpty(command.Request.Email))
        {
            user.Email = command.Request.Email;
            hasChange = true;
        }

        // Actualizar Password si viene
        if (!string.IsNullOrEmpty(command.Request.Password))
        {
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
                return Result<UserResponse>.Fail(
                    "Error al eliminar la contraseña actual",
                    removePasswordResult.Errors.Select(e => e.Description).ToList()
                );

            var addPasswordResult = await _userManager.AddPasswordAsync(user, command.Request.Password);
            if (!addPasswordResult.Succeeded)
                return Result<UserResponse>.Fail(
                    "Error al actualizar la contraseña",
                    addPasswordResult.Errors.Select(e => e.Description).ToList()
                );
        }

        // Actualizar roles si se proporcionaron
        if (command.Request.RoleIds != null)
        {
            var roleUpdateResult = await _userRoleService.UpdateUserRolesAsync(user, command.Request.RoleIds);
            if (!roleUpdateResult.Success)
                return Result<UserResponse>.Fail(
                    roleUpdateResult.Message ?? "Error al actualizar roles",
                    roleUpdateResult.Errors
                );
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

        var userRoles = await _userRoleService.GetUserRolesAsync(user);

        return Result<UserResponse>.OK(new UserResponse
        {
            Id = user.Id.ToString(),
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = userRoles
        });
    }
}
