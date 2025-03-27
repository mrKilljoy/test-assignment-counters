namespace Assignment.Counters.Application.Models.Requests;

public class IncrementCounterRequest
{
    public Guid CounterId { get; set; }

    public long Value { get; set; }

    public DateTime LastTimestamp { get; set; }
}