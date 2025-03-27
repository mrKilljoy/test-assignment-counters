namespace Assignment.Counters.Application.Models;

public class TeamDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public long TotalSteps { get; set; }
}