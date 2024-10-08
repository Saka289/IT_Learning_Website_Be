using AutoMapper;
using LW.API.Extensions;
using LW.Data.Persistence;
using LW.Logging;
using Nest;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);

    builder.Host.AddAppConfiguration();

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddConfigurationSettings(builder.Configuration);

    builder.AddAppAuthentication();

    var app = builder.Build();

    app.UseInfrastructure(builder.Environment);

    app.MigrateDatabase<AppDbContext>((context, _) =>
    {
        var elasticClient = app.Services.GetRequiredService<IElasticClient>();
        var mapper = app.Services.GetRequiredService<IMapper>();
        AppDbContextSeed.SeedDataAsync(context, Log.Logger, elasticClient, mapper).Wait();
    }).Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}