using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Users;

public class UserRequest
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string? Email { get; set; }

    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string? Password { get; set; }

    public List<Guid>? RoleIds { get; set; }
}
