namespace Application.DTOs.Roles;

public class RoleRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
