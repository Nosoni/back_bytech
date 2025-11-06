using Application.Common;
using Application.DTOs.Roles;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Roles.Queries;

public record GetAllRolesQuery : IRequest<Result<IEnumerable<RoleResponse>>>;

public class GetAllRolesQueryHandler(RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<GetAllRolesQuery, Result<IEnumerable<RoleResponse>>>
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<IEnumerable<RoleResponse>>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles
            .Select(r => new RoleResponse
            {
                Id = r.Id.ToString(),
                Name = r.Name!
            })
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<RoleResponse>>.OK(roles);
    }
}
