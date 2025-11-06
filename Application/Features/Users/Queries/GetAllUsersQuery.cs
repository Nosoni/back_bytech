using Application.Common;
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Queries;

public record GetAllUsersQuery : IRequest<Result<IEnumerable<UserResponse>>>;

public class GetAllUsersQueryHandler(
    UserManager<ApplicationUser> userManager,
    IUserRoleService userRoleService) : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserResponse>>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserRoleService _userRoleService = userRoleService;

    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();
        var userResponses = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userRoleService.GetUserRolesAsync(user);
            userResponses.Add(new UserResponse
            {
                Id = user.Id.ToString(),
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles
            });
        }

        return Result<IEnumerable<UserResponse>>.OK(userResponses);
    }
}
