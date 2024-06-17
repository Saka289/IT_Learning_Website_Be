using LW.Data.Entities;
using ILogger = Serilog.ILogger;

namespace LW.Data.Persistence;

public class AppDbContextSeed
{
    public static async Task SeedDataAsync(AppDbContext context, ILogger logger)
    {
        if (!context.Levels.Any())
        {
            context.AddRange(SeedLevel());
            await context.SaveChangesAsync();
            logger.Information("Seeded data for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
        }
        if (!context.Grades.Any())
        {
            context.AddRange(SeedGrade());
            await context.SaveChangesAsync();
            logger.Information("Seeded data for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
        }
    }
    private static IEnumerable<Level> SeedLevel()
    {
        return new List<Level>()
        {
            new()
            {
                Name =  "Tiểu học",
                Active = true
            },
            new()
            {
                Name =  "Trung học cơ sở",
                Active = true
            },
            new()
            {
                Name =  "Trung học phổ thông",
                Active = true
            },
        };
    }
    
    private static IEnumerable<Grade> SeedGrade()
    {
        return new List<Grade>()
        {
            new()
            {
                Name =  "Cấp 1",
                Active = true,
                LevelId = 1
            },
            new()
            {
                Name =  "Cấp 2",
                Active = true,
                LevelId = 2
            },
            new()
            {
                Name =  "Cấp 3",
                Active = true,
                LevelId = 3
            },
        };
    }
}