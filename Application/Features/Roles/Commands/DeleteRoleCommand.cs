using Application.Common;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Roles.Commands;

public record DeleteRoleCommand(string Id) : IRequest<Result<bool>>;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del rol es requerido")
            .Must(BeValidGuid).WithMessage("El ID debe ser un GUID válido");
    }

    private bool BeValidGuid(string id) => Guid.TryParse(id, out _);
}

public class DeleteRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<DeleteRoleCommand, Result<bool>>
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<bool>> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(command.Id);
        if (role == null)
            return Result<bool>.Fail("Rol no encontrado");

        role.IsActive = false;
        var result = await _roleManager.UpdateAsync(role);
        
        if (!result.Succeeded)
            return Result<bool>.Fail(
                "Error al eliminar rol",
                result.Errors.Select(e => e.Description).ToList()
            );

        return Result<bool>.OK(true);
    }
}
