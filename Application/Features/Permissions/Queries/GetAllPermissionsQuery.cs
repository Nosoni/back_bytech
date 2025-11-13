using System;
using Application.Common;
using Application.DTOs.Permissions;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Permissions.Queries;

public record GetAllPermissionsQuery : IRequest<Result<IEnumerable<PermissionResponse>>>;

public class GetAllPermissionsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAllPermissionsQuery, Result<IEnumerable<PermissionResponse>>>
{
    private readonly IApplicationDbContext _context = context;
    public async Task<Result<IEnumerable<PermissionResponse>>> Handle(GetAllPermissionsQuery query, CancellationToken cancellationToken)
    {
        var permissions = await _context.Permissions
            .Select(p => new PermissionResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<PermissionResponse>>.OK(permissions);
    }
}