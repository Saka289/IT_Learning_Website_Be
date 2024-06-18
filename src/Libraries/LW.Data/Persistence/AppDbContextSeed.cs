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
        if (!context.Documents.Any())
        {
            context.AddRange(SeedDocument());
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
                Title = "Tiểu học",
                KeyWord = "tieu hoc",
                IsActive = true
            },
            new()
            {
                Title = "Trung học cơ sở",
                KeyWord = "trung hoc co so",
                IsActive = true
            },
            new()
            {
                Title = "Trung học phổ thông",
                KeyWord = "trung hoc pho thong",
                IsActive = true
            },
        };
    }

    private static IEnumerable<Grade> SeedGrade()
    {
        return new List<Grade>()
        {
            new()
            {
                Title = "Cấp 1",
                KeyWord = "cap 1",
                IsActive = true,
                LevelId = 1
            },
            new()
            {
                Title = "Cấp 2",
                KeyWord = "cap 2",
                IsActive = true,
                LevelId = 2
            },
            new()
            {
                Title = "Cấp 3",
                KeyWord = "cap 3",
                IsActive = true,
                LevelId = 3
            },
        };
    }
    private static IEnumerable<Document> SeedDocument()
    {
        return new List<Document>()
        {
            new()
            {
                Title = "Sách cánh diều",
                Description = "Sách cánh diều mô tả",
                KeyWord = "sach canh dieu",
                IsActive= true,
                GradeId =1
            },
            new()
            {
                Title = "Sách chân trời",
                Description = "Sách chân trời mô tả",
                KeyWord = "sach chan troi",
                IsActive= true,
                GradeId =2
            },
            new()
            {
                Title = "Sách kết nối tri chức",
                Description = "Sách kết nối tri thức mô tả",
                KeyWord = "sach ket noi tri thuc",
                IsActive= true,
                GradeId =3
            },
        };
    }
}