using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Shared.Constant;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.Topic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nest;
using ILogger = Serilog.ILogger;

namespace LW.Data.Persistence;

public class AppDbContextSeed
{
    public static async Task SeedDataAsync(AppDbContext context, ILogger logger, IElasticClient elasticClient, IMapper mapper)
    {
        if (!context.Users.Any() && !context.Roles.Any())
        {
            SeedDataUserRoles(context);
            await context.SaveChangesAsync();
            logger.Information("Seeded data User and Roles for Education DB associated with context {DbContextName}", nameof(AppDbContext));
        }
        
        if (!context.Levels.Any())
        {
            var dataLevel = SeedLevel();
            var result = mapper.Map<IEnumerable<Level>>(dataLevel);
            await context.Levels.AddRangeAsync(result);
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
            var result = mapper.Map<IEnumerable<Grade>>(dataGrade).AsQueryable().AsNoTracking();
            await context.Grades.AddRangeAsync(result);
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
            var result = mapper.Map<IEnumerable<Document>>(dataDocument);
            await context.Documents.AddRangeAsync(result);
            await context.SaveChangesAsync();
            logger.Information("Seeded data Documents for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticDocuments).IndexMany(dataDocument));
            logger.Information("Seeded data Documents for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }
        
        if (!context.Topics.Any())
        {
            var dataTopic = SeedTopic();
            var result = mapper.Map<IEnumerable<Topic>>(dataTopic);
            await context.Topics.AddRangeAsync(result);
            await context.SaveChangesAsync();
            logger.Information("Seeded data Topics for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticTopics).IndexMany(dataTopic));
            logger.Information("Seeded data dataTopic for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }
        
        if (!context.Lessons.Any())
        {
            var dataLesson = SeedLesson();
            var result = mapper.Map<IEnumerable<Lesson>>(dataLesson);
            await context.Lessons.AddRangeAsync(result);
            await context.SaveChangesAsync();
            logger.Information("Seeded data Lessons for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticLessons).IndexMany(dataLesson));
            logger.Information("Seeded data Lessons for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }
    }

    private static void SeedDataUserRoles(AppDbContext context)
    {
        // Seed Role
        string ADMIN_ID = Guid.NewGuid().ToString();
        string USER_ID = Guid.NewGuid().ToString();

        context.Roles.AddRange(new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = USER_ID,
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = ADMIN_ID,
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
        });

        // Seed User and Admin
        var hasher = new PasswordHasher<ApplicationUser>();
        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@gmail.com",
            FirstName = "Frank",
            LastName = "Lotus",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "1234567890"
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@1234");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "user",
            NormalizedUserName = "USER",
            Email = "user@gmail.com",
            FirstName = "Frank",
            LastName = "Cadi",
            NormalizedEmail = "USER@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "1234567890"
        };
        user.PasswordHash = hasher.HashPassword(user, "User@1234");

        context.Users.AddRange(admin, user);

        // Seed UserRole
        context.UserRoles.AddRange(
            new IdentityUserRole<string>
            {
                RoleId = ADMIN_ID,
                UserId = admin.Id
            },
            new IdentityUserRole<string>
            {
                RoleId = USER_ID,
                UserId = user.Id
            }
        );
    }

    private static IEnumerable<LevelDto> SeedLevel()
    {
        return new List<LevelDto>()
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

    private static IEnumerable<GradeDto> SeedGrade()
    {
        return new List<GradeDto>()
        {
            new()
            {
                Title = "Lớp 3",
                KeyWord = "lop 3",
                IsActive = true,
                LevelId = 1,
                LevelTitle = "Tiểu học"
            },
            new()
            {
                Title = "Lớp 4",
                KeyWord = "lop 4",
                IsActive = true,
                LevelId = 1,
                LevelTitle = "Tiểu học"
            },
            new()
            {
                Title = "Lớp 5",
                KeyWord = "lop 5",
                IsActive = true,
                LevelId = 1,
                LevelTitle = "Tiểu học"
            },
            new()
            {
                Title = "Lớp 6",
                KeyWord = "lop 6",
                IsActive = true,
                LevelId = 2,
                LevelTitle = "Trung học cơ sở"
            },
        };
    }

    private static IEnumerable<DocumentDto> SeedDocument()
    {
        return new List<DocumentDto>()
        {
            new()
            {
                Title = "Sách cánh diều",
                Description = "Sách cánh diều mô tả",
                KeyWord = "sach canh dieu",
                IsActive = true,
                GradeId = 1,
                GradeTitle = "Lớp 3"
            },
            new()
            {
                Title = "Sách chân trời",
                Description = "Sách chân trời mô tả",
                KeyWord = "sach chan troi",
                IsActive = true,
                GradeId = 2,
                GradeTitle = "Lớp 4"
            },
            new()
            {
                Title = "Sách kết nối tri thức",
                Description = "Sách kết nối tri thức mô tả",
                KeyWord = "sach ket noi tri thuc",
                IsActive = true,
                GradeId = 3,
                GradeTitle = "Lớp 5"
            },
        };
    }
    
    private static IEnumerable<TopicDto> SeedTopic()
    {
        return new List<TopicDto>()
        {
            new TopicDto()
            {
                Title = "Toán học",
                KeyWord = "toan hoc",
                Description = "Môn học về toán học",
                Objectives ="Làm chủ về môn toán học",
                IsActive = true,
                DocumentId = 1,
                DocumentTitle = "Sách cánh diều",

                
            },
            new TopicDto()
            {
                Title = "Văn học",
                KeyWord = "van hoc",
                Description = "Môn học về văn học",
                Objectives ="Làm chủ về môn văn học",
                IsActive = true,
                DocumentId = 2,
                DocumentTitle = "Sách chân trời",

            },
            new TopicDto()
            {
                Title = "Khoa học tự nhiên",
                KeyWord = "khoa hoc tu nhien",
                Description = "Môn học về khoa học tự nhiên",
                Objectives ="Làm chủ về môn khoa học tự nhiên",
                IsActive = true,
                DocumentId = 3,
                DocumentTitle = "Sách kết nối tri thức"
            }
        };
    }
    
    private static IEnumerable<LessonDto> SeedLesson()
    {
        return new List<LessonDto>()
        {
            new LessonDto()
            {
                Title = "Lesson 1",
                KeyWord = "lesson 1",
                IsActive = true,
                Content = "Content of Lesson 1",
                FilePath = "https://res.cloudinary.com/itsupport18/raw/upload/v1718987928/LessonFile/FILE-5a5f56e6-6081-47d3-81db-fa35f3a898e0.pdf",
                PublicId = "LessonFile/FILE-5a5f56e6-6081-47d3-81db-fa35f3a898e0.pdf",
                UrlDownload = "https://res.cloudinary.com/itsupport18/raw/upload/fl_attachment/v1/LessonFile/FILE-5a5f56e6-6081-47d3-81db-fa35f3a898e0.pdf",
                TopicId = 1,
                TopicTitle = "Toán học"
            },
            new LessonDto()
            {
                Title = "Lesson 2",
                KeyWord = "lesson 2",
                IsActive = true,
                Content = "Content of Lesson 2",
                FilePath = "https://res.cloudinary.com/itsupport18/raw/upload/v1718987951/LessonFile/FILE-a662a89a-8bc1-4ba9-98dc-3580b5ae1782.pdf",
                PublicId = "LessonFile/FILE-a662a89a-8bc1-4ba9-98dc-3580b5ae1782.pdf",
                UrlDownload = "https://res.cloudinary.com/itsupport18/raw/upload/fl_attachment/v1/LessonFile/FILE-a662a89a-8bc1-4ba9-98dc-3580b5ae1782.pdf",
                TopicId = 2,
                TopicTitle = "Văn học"
            },
            new LessonDto()
            {
                Title = "Lesson 3",
                KeyWord = "lesson 3",
                IsActive = true,
                Content = "Content of Lesson 3",
                FilePath = "https://res.cloudinary.com/itsupport18/raw/upload/v1718987972/LessonFile/FILE-3a204b73-f357-490d-9a42-f11b42318ca5.pdf",
                PublicId = "LessonFile/FILE-3a204b73-f357-490d-9a42-f11b42318ca5.pdf",
                UrlDownload = "https://res.cloudinary.com/itsupport18/raw/upload/fl_attachment/v1/LessonFile/FILE-3a204b73-f357-490d-9a42-f11b42318ca5.pdf",
                TopicId = 3,
                TopicTitle = "Khoa học tự nhiên"
            }
        };
    }
}