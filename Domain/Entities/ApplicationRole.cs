using Microsoft.AspNetCore.Identity;
using Domain.Interfaces;

namespace Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>, ISoftDelete
{
    public bool IsActive { get; set; } = true;
    public virtual ICollection<ApplicationPermission> ApplicationPermissions { get; set; } = [];
}