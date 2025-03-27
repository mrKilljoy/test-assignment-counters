using Assignment.Counters.Application.Exceptions;
using Assignment.Counters.Application.Interfaces;
using Assignment.Counters.Application.Models;
using Assignment.Counters.Domain.Entities;
using Assignment.Counters.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assignment.Counters.Infrastructure.Services;

public class TeamManager : ITeamManager
{
    private readonly ILogger<TeamManager> _logger;
    private readonly AppDbContext _dbContext;

    public TeamManager(ILogger<TeamManager> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<TeamDto> Create(string name)
    {
        try
        {
            var found = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Name.Equals(name));
            if (found is not null)
                throw new EntryAlreadyExistsException<Team>();

            var item = new Team()
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        
            var result = await _dbContext.Teams.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            return new TeamDto() { Id = result.Entity.Id, Name = result.Entity.Name };
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
            var found = await _dbContext.Teams.FindAsync(id);
            if (found is null)
                throw new EntryNotFoundException<Team>(id);

            _dbContext.Teams.Remove(found);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Delete));
            throw;
        }
    }

    public async Task<TeamDto> Get(Guid id)
    {
        try
        {
            var found = await _dbContext.Teams
                .Include(x => x.Counters)
                .Select(x => new { Id = x.Id, Name = x.Name, Steps = x.Counters.Sum(y => y.StepsMade) })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (found is null)
                throw new EntryNotFoundException<Team>(id);
            
            // todo: mapping
            return new TeamDto() { Id = found.Id, Name = found.Name, TotalSteps = found.Steps };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Get));
            throw;
        }
    }

    public async Task<IEnumerable<TeamDto>> GetAll()
    {
        try
        {
            var found = await _dbContext.Teams
                .AsNoTrackingWithIdentityResolution()
                .Include(x => x.Counters)
                .Select(x => new { Id = x.Id, Name = x.Name, Steps = x.Counters.Sum(y => y.StepsMade) })
                .OrderByDescending(x => x.Steps)
                .ToListAsync();

            if (found.Count == 0)
                return [];

            // todo: mapping
            return found
                .Select(x => new TeamDto() { Id = x.Id, Name = x.Name, TotalSteps = x.Steps })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetAll));
            throw;
        }
    }
}