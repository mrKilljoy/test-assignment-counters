namespace Assignment.Counters.Application.Models.Requests;

public class NewCounterRequest
{
    public string UserName { get; set; }

    public Guid TeamId { get; set; }
}