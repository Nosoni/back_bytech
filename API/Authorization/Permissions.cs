namespace API.Authorization;

/// <summary>
/// Constantes para todos los permisos del sistema.
/// Estos nombres deben coincidir exactamente con los registros en la tabla Permissions de la BD.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Permisos relacionados con la gestión de usuarios
    /// </summary>
    public static class Users
    {
        public const string Create = "Users.Create";
        public const string Read = "Users.Read";
        public const string Update = "Users.Update";
        public const string Delete = "Users.Delete";
    }

    /// <summary>
    /// Permisos relacionados con la gestión de roles
    /// </summary>
    public static class Roles
    {
        public const string Create = "Roles.Create";
        public const string Read = "Roles.Read";
        public const string Update = "Roles.Update";
        public const string Delete = "Roles.Delete";
    }

    /// <summary>
    /// Permisos relacionados con la gestión de permisos
    /// </summary>
    public static class PermissionsManagement
    {
        public const string Create = "Permissions.Create";
        public const string Read = "Permissions.Read";
        public const string Update = "Permissions.Update";
        public const string Delete = "Permissions.Delete";
    }

    /// <summary>
    /// Obtiene todos los permisos definidos en el sistema
    /// </summary>
    public static string[] GetAll()
    {
        return
        [
            // Users
            Users.Create,
            Users.Read,
            Users.Update,
            Users.Delete,

            // Roles
            Roles.Create,
            Roles.Read,
            Roles.Update,
            Roles.Delete,

            // Permissions
            PermissionsManagement.Create,
            PermissionsManagement.Read,
            PermissionsManagement.Update,
            PermissionsManagement.Delete
        ];
    }
}
