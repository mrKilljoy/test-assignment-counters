namespace Assignment.Counters.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; }

    public IList<Counter> Counters { get; set; } = new List<Counter>();
}