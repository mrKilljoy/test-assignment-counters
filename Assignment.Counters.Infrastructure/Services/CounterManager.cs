using Assignment.Counters.Application.Exceptions;
using Assignment.Counters.Application.Interfaces;
using Assignment.Counters.Application.Models;
using Assignment.Counters.Domain.Entities;
using Assignment.Counters.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assignment.Counters.Infrastructure.Services;

public class CounterManager : ICounterManager
{
    private readonly ILogger<CounterManager> _logger;
    private readonly AppDbContext _dbContext;

    public CounterManager(ILogger<CounterManager> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<CounterDto> Create(string userName, Guid teamId)
    {
        try
        {
            var found = await _dbContext.Counters
                .Include(x => x.Team)
                .FirstOrDefaultAsync(x => x.Team.Id == teamId && x.UserName.Equals(userName));
        
            if (found is not null)
                throw new EntryAlreadyExistsException<Counter>();

            var team = await _dbContext.Teams.FindAsync(teamId);
            if (team is null)
                throw new EntryNotFoundException<Team>(teamId);

            var item = new Counter()
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Team = team
            };
        
            var result = await _dbContext.Counters.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            // todo: mapping
            return new CounterDto()
            {
                Id = result.Entity.Id,
                UserName = result.Entity.UserName,
                Steps = result.Entity.StepsMade
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Create));
            throw;
        }
    }

    public async Task Delete(Guid id)
    {
        try
        {
            var found = await _dbContext.Counters.FindAsync(id);
            if (found is null)
                throw new EntryNotFoundException<Counter>(id);

            _dbContext.Counters.Remove(found);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Delete));
            throw;
        }
    }

    public async Task Increment(Guid id, long steps, DateTime lastUpdated)
    {
        try
        {
            var found = await _dbContext.Counters.FindAsync(id);
            if (found is null)
                throw new EntryNotFoundException<Counter>(id);

            // a simple concurrency token replacement
            if (found.LastUpdated != lastUpdated)
                throw new DbUpdateConcurrencyException("Concurrency conflict. Please, try to update the item again");

            found.StepsMade += steps;
            found.LastUpdated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Increment));
            throw;
        }
    }

    public async Task<IEnumerable<CounterDto>> GetCounters(Guid teamId)
    {
        try
        {
            var found = await _dbContext.Counters.Include(x => x.Team)
                .Where(x => x.Team.Id == teamId)
                .ToListAsync();

            if (found.Count == 0)
                return [];
        
            // todo: mapping
            return found
                .Select(x => new CounterDto() { Id = x.Id, UserName = x.UserName, Steps = x.StepsMade })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetCounters));
            throw;
        }
    }

    public async Task<CounterDetailsDto> Get(Guid id)
    {
        try
        {
            var found = await _dbContext.Counters.FindAsync(id);
            if (found is null)
                throw new EntryNotFoundException<Counter>(id);

            return new CounterDetailsDto()
            {
                Id = id,
                UserName = found.UserName,
                Steps = found.StepsMade,
                TimeStamp = found.LastUpdated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetCounters));
            throw;
        }
    }
}