using System.Reflection;
using Assignment.Counters.Api.Infrastructure;
using Assignment.Counters.Api.Infrastructure.Filters;
using Assignment.Counters.Application.Interfaces;
using Assignment.Counters.Infrastructure.Data;
using Assignment.Counters.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

namespace Assignment.Counters.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddLogging(x => x.AddDebug());
        builder.Services.AddRouting(x => x.LowercaseUrls = true);
        
        builder.Services.AddDbContext<AppDbContext>(b =>
            b.UseInMemoryDatabase(Constants.Data.DatabaseName));

        builder.Services.AddTransient<ICounterManager, CounterManager>();
        builder.Services.AddTransient<ITeamManager, TeamManager>();
        
        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Leaderboards APIs"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                    x.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });
        
        builder.Services.AddControllers(x => x.Filters.Add<CustomGlobalExceptionFilter>());
        
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.MapControllers();
        
        // add test data?

        app.Run();
    }
}