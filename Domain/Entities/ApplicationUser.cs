using Microsoft.AspNetCore.Identity;
using Domain.Interfaces;

namespace Domain.Entities;

// Clases personalizadas de Identity que usan Guid
public class ApplicationUser : IdentityUser<Guid>, ISoftDelete
{
    public bool IsActive { get; set; } = true;
}