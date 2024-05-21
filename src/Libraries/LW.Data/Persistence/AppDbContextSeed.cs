using ILogger = Serilog.ILogger;

namespace LW.Data.Persistence;

public class AppDbContextSeed
{
    public static async Task SeedDataAsync(AppDbContext context, ILogger logger)
    {
        // if (!context.Products.Any())
        // {
        //     context.AddRange(GetData());
        //     await context.SaveChangesAsync();
        //     logger.Information("Seeded data for Education DB associated with context {DbContextName}",
        //         nameof(AppDbContext));
        // }
    }

    // private static IEnumerable<> GetData()
    // {
    //
    // }
}