using Assignment.Counters.Application.Exceptions;
using Assignment.Counters.Domain.Entities;
using Assignment.Counters.Infrastructure.Data;
using Assignment.Counters.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment.Counters.Tests;

using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

[TestFixture]
public class CounterManagerTests
{
    private CounterManager _counterManager;
    private Mock<ILogger<CounterManager>> _loggerMock;
    private AppDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<CounterManager>>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        _dbContext = new AppDbContext(options);
        _counterManager = new CounterManager(_loggerMock.Object, _dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Create_ValidCounter_ReturnsCounterDto()
    {
        // Arrange
        var team = new Team { Id = Guid.NewGuid(), Name = "Test Team" };
        await _dbContext.Teams.AddAsync(team);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _counterManager.Create("user1", team.Id);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual("user1", result.UserName);
        Assert.AreEqual(0, result.Steps);
    }

    [Test]
    public async Task Create_DuplicateCounter_ThrowsEntryAlreadyExistsException()
    {
        // Arrange
        var team = new Team { Id = Guid.NewGuid(), Name = "Test Team" };
        await _dbContext.Teams.AddAsync(team);
        await _dbContext.SaveChangesAsync();

        var existingCounter = new Counter { Id = Guid.NewGuid(), UserName = "user1", Team = team };
        await _dbContext.Counters.AddAsync(existingCounter);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        Assert.ThrowsAsync<EntryAlreadyExistsException<Counter>>(async () =>
        {
            await _counterManager.Create("user1", team.Id);
        });
    }

    [Test]
    public async Task Create_NonExistingTeam_ThrowsEntryNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntryNotFoundException<Team>>(async () =>
        {
            await _counterManager.Create("user1", Guid.NewGuid());
        });
    }

    [Test]
    public async Task Delete_ValidCounter_RemovesCounter()
    {
        // Arrange
        var counter = new Counter { Id = Guid.NewGuid(), UserName = "user1" };
        await _dbContext.Counters.AddAsync(counter);
        await _dbContext.SaveChangesAsync();

        // Act
        await _counterManager.Delete(counter.Id);

        // Assert
        Assert.Null(await _dbContext.Counters.FindAsync(counter.Id));
    }

    [Test]
    public async Task Delete_NonExistingCounter_ThrowsEntryNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntryNotFoundException<Counter>>(async () =>
        {
            await _counterManager.Delete(Guid.NewGuid());
        });
    }

    [Test]
    public async Task Increment_ValidCounter_IncrementsSteps()
    {
        // Arrange
        var counter = new Counter { Id = Guid.NewGuid(), StepsMade = 100, LastUpdated = DateTime.UtcNow, UserName = "user-1" };
        await _dbContext.Counters.AddAsync(counter);
        await _dbContext.SaveChangesAsync();

        DateTime originalTimestamp = counter.LastUpdated;
        long incrementValue = 50;

        // Act
        await _counterManager.Increment(counter.Id, incrementValue, originalTimestamp);

        // Assert
        var updatedCounter = await _dbContext.Counters.FindAsync(counter.Id);
        Assert.AreEqual(150, updatedCounter.StepsMade);
    }

    [Test]
    public async Task Increment_ConcurrencyConflict_ThrowsDbUpdateConcurrencyException()
    {
        // Arrange
        var counter = new Counter { Id = Guid.NewGuid(), StepsMade = 100, LastUpdated = DateTime.UtcNow, UserName = "user-1" };
        await _dbContext.Counters.AddAsync(counter);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
        {
            await _counterManager.Increment(counter.Id, 50, DateTime.UtcNow.AddSeconds(-10)); // Wrong timestamp
        });
    }

    [Test]
    public async Task Increment_NonExistingCounter_ThrowsEntryNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntryNotFoundException<Counter>>(async () =>
        {
            await _counterManager.Increment(Guid.NewGuid(), 50, DateTime.UtcNow);
        });
    }

    [Test]
    public async Task GetCounters_ValidTeam_ReturnsCounters()
    {
        // Arrange
        var team = new Team { Id = Guid.NewGuid(), Name = "Team 1" };
        var counter1 = new Counter { Id = Guid.NewGuid(), UserName = "user1", Team = team, StepsMade = 10 };
        var counter2 = new Counter { Id = Guid.NewGuid(), UserName = "user2", Team = team, StepsMade = 20 };

        await _dbContext.Teams.AddAsync(team);
        await _dbContext.Counters.AddRangeAsync(counter1, counter2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _counterManager.GetCounters(team.Id);

        // Assert
        Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task GetCounters_NoCounters_ReturnsEmptyList()
    {
        // Act
        var result = await _counterManager.GetCounters(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task Get_ValidCounter_ReturnsCounterDetailsDto()
    {
        // Arrange
        var counter = new Counter
            { Id = Guid.NewGuid(), UserName = "user1", StepsMade = 50, LastUpdated = DateTime.UtcNow };
        await _dbContext.Counters.AddAsync(counter);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _counterManager.Get(counter.Id);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(counter.Id, result.Id);
        Assert.AreEqual(counter.StepsMade, result.Steps);
    }

    [Test]
    public async Task Get_NonExistingCounter_ThrowsEntryNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<EntryNotFoundException<Counter>>(async () => { await _counterManager.Get(Guid.NewGuid()); });
    }
}