using Application.Common;
using Application.DTOs.Roles;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Roles.Commands;

public record CreateRoleCommand(RoleRequest Request) : IRequest<Result<RoleResponse>>;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("El nombre del rol es requerido")
            .Length(3, 50).WithMessage("El nombre del rol debe tener entre 3 y 50 caracteres")
            .Matches("^[a-zA-Z_]+$").WithMessage("El nombre solo puede contener letras y guiones bajos (_)");
    }
}

public class CreateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<CreateRoleCommand, Result<RoleResponse>>
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<RoleResponse>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var existingRole = await _roleManager.FindByNameAsync(command.Request.Name);
        if (existingRole != null)
            return Result<RoleResponse>.Fail("Ya existe un rol con ese nombre");

        var role = new ApplicationRole
        {
            Name = command.Request.Name,
            IsActive = true
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
            Name = role.Name!,
            IsActive = role.IsActive
        });
    }
}
