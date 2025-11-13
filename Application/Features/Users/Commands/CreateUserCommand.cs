using Application.Common;
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands;

public record CreateUserCommand(UserRequest Request) : IRequest<Result<UserResponse>>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Request.UserName)
            .NotEmpty().WithMessage("El nombre de usuario es requerido")
            .Length(3, 50).WithMessage("El nombre de usuario debe tener entre 3 y 50 caracteres");

        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MustAsync(BeUniqueEmail).WithMessage("El email ya está registrado");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres");

        RuleFor(x => x.Request.Password)
            .Must((cmd, password) => !password!.Contains(cmd.Request.UserName!))
            .When(x => !string.IsNullOrEmpty(x.Request.Password) && !string.IsNullOrEmpty(x.Request.UserName))
            .WithMessage("La contraseña no puede contener el nombre de usuario");
    }

    private async Task<bool> BeUniqueEmail(string? email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email)) return true;
        var user = await _userManager.FindByEmailAsync(email);
        return user == null;
    }
}

public class CreateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUserRoleService userRoleService) : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserRoleService _userRoleService = userRoleService;

    public async Task<Result<UserResponse>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = command.Request.UserName,
            Email = command.Request.Email,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, command.Request.Password!);
        if (!result.Succeeded)
            return Result<UserResponse>.Fail(
                "Error al crear usuario",
                result.Errors.Select(e => e.Description).ToList()
            );

        // Asignar roles si se proporcionaron
        if (command.Request.RoleIds != null && command.Request.RoleIds.Any())
        {
            var roleAssignmentResult = await _userRoleService.AssignRolesToUserAsync(user, command.Request.RoleIds);
            if (!roleAssignmentResult.Success)
            {
                // Si falla la asignación de roles, eliminar el usuario creado
                await _userManager.DeleteAsync(user);
                return Result<UserResponse>.Fail(
                    roleAssignmentResult.Message ?? "Error al asignar roles",
                    roleAssignmentResult.Errors
                );
            }
        }

        // Obtener roles asignados para la respuesta
        var roles = await _userRoleService.GetUserRolesAsync(user);

        return Result<UserResponse>.OK(new UserResponse
        {
            Id = user.Id.ToString(),
            UserName = user.UserName!,
            Email = user.Email!,
            IsActive = user.IsActive,
            Roles = roles
        });
    }
}
