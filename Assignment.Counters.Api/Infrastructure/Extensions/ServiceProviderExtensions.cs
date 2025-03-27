using Assignment.Counters.Infrastructure.Data;
using Assignment.Counters.Infrastructure.Extensions;

namespace Assignment.Counters.Api.Infrastructure.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task AddTestData(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.AddTestData();
    }
}