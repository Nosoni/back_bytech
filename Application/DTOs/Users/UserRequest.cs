namespace Application.DTOs.Users;

public class UserRequest
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid>? RoleIds { get; set; }
}
