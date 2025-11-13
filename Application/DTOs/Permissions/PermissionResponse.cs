using System;

namespace Application.DTOs.Permissions;

public class PermissionResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
}
