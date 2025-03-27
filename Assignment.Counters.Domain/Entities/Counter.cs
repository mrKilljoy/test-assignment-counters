namespace Assignment.Counters.Domain.Entities;

public class Counter : BaseEntity
{
    public string UserName { get; set; }

    public long StepsMade { get; set; }

    public Team Team { get; set; }
}