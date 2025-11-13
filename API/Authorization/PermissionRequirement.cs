using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

/// <summary>
/// Requirement que define que se necesita un permiso específico para acceder a un recurso
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
}
