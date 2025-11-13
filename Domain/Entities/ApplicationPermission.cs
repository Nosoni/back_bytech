namespace Domain.Entities;

public class ApplicationPermission : BaseEntity
{
    public string Name { get; set; } = string.Empty;        // Ej: "users.create", "products.delete"
    public string Description { get; set; } = string.Empty; // Descripción humana
}
