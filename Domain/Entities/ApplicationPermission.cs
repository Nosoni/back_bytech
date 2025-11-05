using System;

namespace Domain.Entities;

public class ApplicationPermission
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;        // Ej: "users.create", "products.delete"
    public string Description { get; set; } = string.Empty; // Descripción humana
    public DateTime CreatedAt { get; set; }
}
