using Domain.Interfaces;

namespace Domain.Entities;

public abstract class BaseEntity : ISoftDelete
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
