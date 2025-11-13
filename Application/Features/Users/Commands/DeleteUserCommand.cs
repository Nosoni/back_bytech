using Application.Common;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands;

public record DeleteUserCommand(string Id) : IRequest<Result<bool>>;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del usuario es requerido")
            .Must(BeValidGuid).WithMessage("El ID debe ser un GUID válido");
    }

    private bool BeValidGuid(string id) => Guid.TryParse(id, out _);
}

public class DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<bool>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.Id);
        if (user == null)
            return Result<bool>.Fail("Usuario no encontrado");

        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
            return Result<bool>.Fail(
                "Error al eliminar usuario",
                result.Errors.Select(e => e.Description).ToList()
            );

        return Result<bool>.OK(true);
    }
}
