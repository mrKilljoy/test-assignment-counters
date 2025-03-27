namespace Assignment.Counters.Api.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static bool UseTestData(this IConfiguration configuration)
    {
        var value = configuration[Constants.Data.UseTestData];
        return bool.TryParse(value, out var useTestData) ? useTestData : false;
    }
}