using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Roles;

public class RoleRequest
{
    [Required(ErrorMessage = "El nombre del rol es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del rol debe tener entre 3 y 50 caracteres")]
    public string Name { get; set; } = string.Empty;
}
