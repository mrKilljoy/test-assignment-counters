using Assignment.Counters.Application.Models;

namespace Assignment.Counters.Application.Interfaces;

public interface ICounterManager
{
    Task<CounterDto> Create(string userName, Guid teamId);
    
    Task Delete(Guid id);

    Task Increment(Guid id, long steps, DateTime lastUpdated);
    
    Task<IEnumerable<CounterDto>> GetCounters(Guid teamId);
    
    Task<CounterDetailsDto> Get(Guid id);
}