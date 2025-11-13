using Application.Common;
using Application.DTOs.Roles;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Roles.Commands;

public record UpdateRoleCommand(string Id, RoleRequest Request) : IRequest<Result<RoleResponse>>;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del rol es requerido")
            .Must(BeValidGuid).WithMessage("El ID debe ser un GUID válido");

        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("El nombre del rol es requerido")
            .Length(3, 50).WithMessage("El nombre del rol debe tener entre 3 y 50 caracteres")
            .Matches("^[a-zA-Z_]+$").WithMessage("El nombre solo puede contener letras y guiones bajos (_)");
    }

    private bool BeValidGuid(string id) => Guid.TryParse(id, out _);
}

public class UpdateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<UpdateRoleCommand, Result<RoleResponse>>
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<RoleResponse>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(command.Id);
        if (role == null)
            return Result<RoleResponse>.Fail("Rol no encontrado");

        var existingRole = await _roleManager.FindByNameAsync(command.Request.Name);
        if (existingRole != null && existingRole.Id.ToString() != command.Id)
            return Result<RoleResponse>.Fail("Ya existe un rol con ese nombre");

        role.Name = command.Request.Name;
        role.IsActive = command.Request.IsActive;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return Result<RoleResponse>.Fail(
                "Error al actualizar rol",
                result.Errors.Select(e => e.Description).ToList()
            );

        return Result<RoleResponse>.OK(new RoleResponse
        {
            Id = role.Id.ToString(),
            Name = role.Name!,
            IsActive = role.IsActive
        });
    }
}
