using Microsoft.EntityFrameworkCore;

namespace LW.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();

            try
            {
                logger.LogInformation("Starting migration of MySQL database.");
                // Perform the migration
                ExecuteMigration(context);
                logger.LogInformation("Successfully migrated MySQL database.");
                // Seed the database
                InvokeSeeder(seeder, context, services);
                logger.LogInformation("Successfully invoked seeder for MySQL database.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during the MySQL database migration and seeding process.");
            }
        }

        return host;
    }

    private static void ExecuteMigration<TContext>(TContext context) where TContext : DbContext
    {
        context.Database.Migrate();
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
        IServiceProvider services) where TContext : DbContext
    {
        seeder(context, services);
    }
}