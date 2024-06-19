using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Shared.Constant;
using Nest;
using ILogger = Serilog.ILogger;

namespace LW.Data.Persistence;

public class AppDbContextSeed
{
    public static async Task SeedDataAsync(AppDbContext context, ILogger logger, IElasticClient elasticClient)
    {
        if (!context.Levels.Any())
        {
            var dataLevel = SeedLevel();
            context.AddRange(dataLevel);
            await context.SaveChangesAsync();
            logger.Information("Seeded data Levels for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticLevels).IndexMany(dataLevel));
            logger.Information("Seeded data Levels for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }

        if (!context.Grades.Any())
        {
            var dataGrade = SeedGrade();
            context.AddRange(dataGrade);
            await context.SaveChangesAsync();
            logger.Information("Seeded data Grades for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticGrades).IndexMany(dataGrade));
            logger.Information("Seeded data Grades for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }

        if (!context.Documents.Any())
        {
            var dataDocument = SeedDocument();
            context.AddRange(dataDocument);
            await context.SaveChangesAsync();
            logger.Information("Seeded data Documents for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticDocuments).IndexMany(dataDocument));
            logger.Information("Seeded data Documents for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
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
                IsActive = true,
                GradeId = 1
            },
            new()
            {
                Title = "Sách chân trời",
                Description = "Sách chân trời mô tả",
                KeyWord = "sach chan troi",
                IsActive = true,
                GradeId = 2
            },
            new()
            {
                Title = "Sách kết nối tri chức",
                Description = "Sách kết nối tri thức mô tả",
                KeyWord = "sach ket noi tri thuc",
                IsActive = true,
                GradeId = 3
            },
        };
    }
}