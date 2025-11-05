using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

// Clases personalizadas de Identity que usan Guid
public class ApplicationUser : IdentityUser<Guid>
{
    // Puedes agregar propiedades personalizadas aquí en el futuro
}