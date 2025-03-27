using System.Reflection;
using Assignment.Counters.Api.Infrastructure;
using Assignment.Counters.Api.Infrastructure.Extensions;
using Assignment.Counters.Api.Infrastructure.Filters;
using Assignment.Counters.Api.Infrastructure.Validation;
using Assignment.Counters.Application.Interfaces;
using Assignment.Counters.Application.Models.Requests;
using Assignment.Counters.Infrastructure.Data;
using Assignment.Counters.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

namespace Assignment.Counters.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddLogging(x => x.AddDebug());
        builder.Services.AddRouting(x => x.LowercaseUrls = true);
        
        builder.Services.AddDbContext<AppDbContext>(b =>
            b.UseInMemoryDatabase(Constants.Data.DatabaseName));

        builder.Services.AddTransient<IValidator<NewTeamRequest>, NewTeamValidator>();
        builder.Services.AddTransient<IValidator<NewCounterRequest>, NewCounterValidator>();
        builder.Services.AddTransient<IValidator<IncrementCounterRequest>, IncrementCounterValidator>();
        
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

        if (app.Configuration.UseTestData())
        {
            await app.Services.AddTestData();
        }

        app.UseRouting();

        app.MapControllers();
        
        // add test data?

        await app.RunAsync();
    }
}