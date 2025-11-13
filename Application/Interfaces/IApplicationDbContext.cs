using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationPermission> Permissions { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
