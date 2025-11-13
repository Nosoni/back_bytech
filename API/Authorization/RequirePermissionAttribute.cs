using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

/// <summary>
/// Atributo personalizado para requerir un permiso específico en un endpoint
/// Uso: [RequirePermission("Users.Create")]
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentException("Permission cannot be null or empty", nameof(permission));
        }

        // Asignar la política con el nombre del permiso
        // La política se registrará dinámicamente en Program.cs
        Policy = permission;
    }
}
