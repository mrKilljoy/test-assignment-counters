namespace Assignment.Counters.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}