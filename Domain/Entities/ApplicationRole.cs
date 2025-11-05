using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    // Puedes agregar propiedades personalizadas aquí en el futuro
    public virtual ICollection<ApplicationPermission> ApplicationPermissions { get; set; } = [];
}