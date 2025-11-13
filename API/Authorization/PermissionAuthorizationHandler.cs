using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

/// <summary>
/// Handler que valida si el usuario autenticado tiene el permiso requerido
/// Lee los claims del JWT para verificar los permisos del usuario
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Obtener todos los claims de tipo "permission" del usuario autenticado
        var permissions = context.User
            .FindAll(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        // Si el usuario tiene el permiso requerido, marcamos el requirement como exitoso
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
