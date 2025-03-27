namespace Assignment.Counters.Application.Models;

public class CounterDto
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }

    public long Steps { get; set; }
}