namespace Assignment.Counters.Application.Models;

public class CounterDetailsDto
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }

    public long Steps { get; set; }
    
    public DateTime TimeStamp { get; set; }
}