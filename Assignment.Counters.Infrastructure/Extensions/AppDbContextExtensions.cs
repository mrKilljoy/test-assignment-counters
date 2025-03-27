using Assignment.Counters.Domain.Entities;
using Assignment.Counters.Infrastructure.Data;

namespace Assignment.Counters.Infrastructure.Extensions;

public static class AppDbContextExtensions
{
    public static async Task AddTestData(this AppDbContext dbContext)
    {
        var rnd = new Random();
        int teams = 5;
        int counters = 3;

        var teamList = new List<Team>();
        var counterList = new List<Counter>();

        for (int i = 0; i < teams; i++)
        {
            var team = new Team()
            {
                Id = Guid.NewGuid(),
                Name = $"team-{i + 1}",
                LastUpdated = DateTime.UtcNow
            };

            for (int j = 0; j < counters; j++)
            {
                var ctr = new Counter()
                {
                    Id = Guid.NewGuid(),
                    Team = team,
                    UserName = $"usr-{j + 1}",
                    LastUpdated = DateTime.UtcNow,
                    StepsMade = rnd.Next(0, 100)
                };
                counterList.Add(ctr);
            }
            teamList.Add(team);
        }

        await dbContext.Counters.AddRangeAsync(counterList);
        await dbContext.Teams.AddRangeAsync(teamList);

        await dbContext.SaveChangesAsync();
    }
}