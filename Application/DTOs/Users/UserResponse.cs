using Application.DTOs.Roles;

namespace Application.DTOs.Users;

public class UserResponse
{
    public required string Id { get; set; } = string.Empty;
    public required string UserName { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
    public List<RoleResponse> Roles { get; set; } = [];
}
