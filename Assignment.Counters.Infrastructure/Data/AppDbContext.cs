using System.Reflection;
using Assignment.Counters.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Counters.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Team> Teams { get; set; }

    public DbSet<Counter> Counters { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}