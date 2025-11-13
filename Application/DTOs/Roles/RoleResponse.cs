using System;

namespace Application.DTOs.Roles;

public class RoleResponse
{
    public required string Id { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
}
