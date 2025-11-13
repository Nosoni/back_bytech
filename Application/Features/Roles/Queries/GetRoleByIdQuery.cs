using Application.Common;
using Application.DTOs.Roles;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Roles.Queries;

public record GetRoleByIdQuery(string Id) : IRequest<Result<RoleResponse>>;

public class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del rol es requerido")
            .Must(BeValidGuid).WithMessage("El ID debe ser un GUID válido");
    }

    private bool BeValidGuid(string id) => Guid.TryParse(id, out _);
}

public class GetRoleByIdQueryHandler(RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<GetRoleByIdQuery, Result<RoleResponse>>
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(query.Id);
        if (role == null)
            return Result<RoleResponse>.Fail("Rol no encontrado");

        return Result<RoleResponse>.OK(new RoleResponse
        {
            Id = role.Id.ToString(),
            Name = role.Name!,
            IsActive = role.IsActive
        });
    }
}
