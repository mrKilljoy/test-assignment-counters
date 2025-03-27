using Assignment.Counters.Application.Models;

namespace Assignment.Counters.Application.Interfaces;

public interface ITeamManager
{
    Task<TeamDto> Create(string name);

    Task Delete(Guid id);

    Task<TeamDto> Get(Guid id);
    
    Task<IEnumerable<TeamDto>> GetAll();
}