using Assignment.Counters.Application.Exceptions;
using Assignment.Counters.Domain.Entities;
using Assignment.Counters.Infrastructure.Data;
using Assignment.Counters.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment.Counters.Tests;

[TestFixture]
public class TeamManagerTests
{
    private TeamManager _teamManager;
    private Mock<ILogger<TeamManager>> _loggerMock;
    private AppDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<TeamManager>>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        _dbContext = new AppDbContext(options);
        _teamManager = new TeamManager(_loggerMock.Object, _dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Create_ValidTeam_ReturnsTeamDto()
    {
        // Act
        var result = await _teamManager.Create("Team A");

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual("Team A", result.Name);
    }

    [Test]
    public async Task Create_DuplicateTeam_ThrowsEntryAlreadyExistsException()
    {
        // Arrange
        await _teamManager.Create("Team A");

        // Act & Assert
        Assert.ThrowsAsync<EntryAlreadyExistsException<Team>>(async () => { await _teamManager.Create("Team A"); });
    }

    [Test]
    public async Task Delete_ValidTeam_RemovesTeam()
    {
        // Arrange
        var team = new Team { Id = Guid.NewGuid(), Name = "Team B" };
        await _dbContext.Teams.AddAsync(team);
        await _dbContext.SaveChangesAsync();

        // Act
        await _teamManager.Delete(team.Id);

        // Assert
        Assert.Null(await _dbContext.Teams.FindAsync(team.Id));
    }

    [Test]
    public async Task Delete_NonExistingTeam_ThrowsEntryNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntryNotFoundException<Team>>(async () => { await _teamManager.Delete(Guid.NewGuid()); });
    }

    [Test]
    public async Task Get_ValidTeam_ReturnsTeamDto()
    {
        // Arrange
        var team = new Team { Id = Guid.NewGuid(), Name = "Team C" };
        var counter1 = new Counter { Id = Guid.NewGuid(), Team = team, UserName = "user-1", StepsMade = 10 };
        var counter2 = new Counter { Id = Guid.NewGuid(), Team = team, UserName = "user-2", StepsMade = 20 };

        await _dbContext.Teams.AddAsync(team);
        await _dbContext.Counters.AddRangeAsync(counter1, counter2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _teamManager.Get(team.Id);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(team.Id, result.Id);
        Assert.AreEqual("Team C", result.Name);
        Assert.AreEqual(30, result.TotalSteps);
    }

    [Test]
    public async Task Get_NonExistingTeam_ThrowsEntryNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntryNotFoundException<Team>>(async () => { await _teamManager.Get(Guid.NewGuid()); });
    }

    [Test]
    public async Task GetAll_TeamsExist_ReturnsOrderedTeams()
    {
        // Arrange
        var team1 = new Team { Id = Guid.NewGuid(), Name = "Team 1" };
        var team2 = new Team { Id = Guid.NewGuid(), Name = "Team 2" };
        var counter1 = new Counter { Id = Guid.NewGuid(), Team = team1, StepsMade = 50, UserName = "user-1" };
        var counter2 = new Counter { Id = Guid.NewGuid(), Team = team2, StepsMade = 100, UserName = "user-2" };

        await _dbContext.Teams.AddRangeAsync(team1, team2);
        await _dbContext.Counters.AddRangeAsync(counter1, counter2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _teamManager.GetAll();

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("Team 2", result.First().Name); // Team with most steps first
    }

    [Test]
    public async Task GetAll_NoTeams_ReturnsEmptyList()
    {
        // Act
        var result = await _teamManager.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.IsEmpty(result);
    }
}