using Application.Common;
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Queries;

public record GetUserByIdQuery(string Id) : IRequest<Result<UserResponse>>;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del usuario es requerido")
            .Must(BeValidGuid).WithMessage("El ID debe ser un GUID válido");
    }

    private bool BeValidGuid(string id) => Guid.TryParse(id, out _);
}

public class GetUserByIdQueryHandler(
    UserManager<ApplicationUser> userManager,
    IUserRoleService userRoleService) : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserRoleService _userRoleService = userRoleService;

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(query.Id);
        if (user == null)
            return Result<UserResponse>.Fail("Usuario no encontrado");

        var roles = await _userRoleService.GetUserRolesAsync(user);

        return Result<UserResponse>.OK(new UserResponse
        {
            Id = user.Id.ToString(),
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles
        });
    }
}
