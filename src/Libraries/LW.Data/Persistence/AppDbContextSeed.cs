using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Shared.Constant;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.Topic;
using LW.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nest;
using ILogger = Serilog.ILogger;

namespace LW.Data.Persistence;

public class AppDbContextSeed
{
    public static async Task SeedDataAsync(AppDbContext context, ILogger logger, IElasticClient elasticClient,
        IMapper mapper)
    {
        if (!context.Users.Any() && !context.Roles.Any())
        {
            SeedDataUserRoles(context);
            await context.SaveChangesAsync();
            logger.Information("Seeded data User and Roles for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
        }

        if (!context.Levels.Any())
        {
            var dataLevel = SeedLevel();
            await context.Levels.AddRangeAsync(dataLevel);
            await context.SaveChangesAsync();
            var result = mapper.Map<IEnumerable<LevelDto>>(dataLevel);
            logger.Information("Seeded data Levels for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticLevels).IndexMany(result));
            logger.Information("Seeded data Levels for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }

        if (!context.Grades.Any())
        {
            var dataGrade = SeedGrade();
            await context.Grades.AddRangeAsync(dataGrade);
            await context.SaveChangesAsync();
            var result = mapper.Map<IEnumerable<GradeDto>>(dataGrade);
            logger.Information("Seeded data Grades for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticGrades).IndexMany(result));
            logger.Information("Seeded data Grades for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }

        if (!context.Documents.Any())
        {
            var dataDocument = SeedDocument();
            await context.Documents.AddRangeAsync(dataDocument);
            await context.SaveChangesAsync();
            var result = mapper.Map<IEnumerable<DocumentDto>>(dataDocument);
            logger.Information("Seeded data Documents for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticDocuments).IndexMany(result));
            logger.Information("Seeded data Documents for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }

        if (!context.Topics.Any())
        {
            var dataTopic = SeedTopic();
            await context.Topics.AddRangeAsync(dataTopic);
            await context.SaveChangesAsync();
            dataTopic = dataTopic.Where(p => p.ParentId == null);
            var result = mapper.Map<IEnumerable<TopicDto>>(dataTopic);
            logger.Information("Seeded data Topics for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticTopics).IndexMany(result));
            logger.Information("Seeded data dataTopic for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }

        if (!context.Lessons.Any())
        {
            var dataLesson = SeedLesson();
            await context.Lessons.AddRangeAsync(dataLesson);
            await context.SaveChangesAsync();
            var result = mapper.Map<IEnumerable<LessonDto>>(dataLesson);
            logger.Information("Seeded data Lessons for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticLessons).IndexMany(result));
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
                Title = "Lớp 3",
                KeyWord = "lop 3",
                IsActive = true,
                LevelId = 1,
            },
            new()
            {
                Title = "Lớp 4",
                KeyWord = "lop 4",
                IsActive = true,
                LevelId = 1,
            },
            new()
            {
                Title = "Lớp 5",
                KeyWord = "lop 5",
                IsActive = true,
                LevelId = 1,
            },
            new()
            {
                Title = "Lớp 6",
                KeyWord = "lop 6",
                IsActive = true,
                LevelId = 2,
            },
            new()
            {
                Title = "Lớp 7",
                KeyWord = "lop 7",
                IsActive = true,
                LevelId = 2,
            },
            new()
            {
                Title = "Lớp 8",
                KeyWord = "lop 8",
                IsActive = true,
                LevelId = 2,
            },
            new()
            {
                Title = "Lớp 9",
                KeyWord = "lop 9",
                IsActive = true,
                LevelId = 2,
            },
            new()
            {
                Title = "Lớp 10",
                KeyWord = "lop 10",
                IsActive = true,
                LevelId = 3,
            },
            new()
            {
                Title = "Lớp 11",
                KeyWord = "lop 11",
                IsActive = true,
                LevelId = 3,
            },
            new()
            {
                Title = "Lớp 12",
                KeyWord = "lop 12",
                IsActive = true,
                LevelId = 3,
            },
        };
    }

    private static IEnumerable<Document> SeedDocument()
    {
        return new List<Document>()
        {
            new()
            {
                Code = "CD0301",
                Title = "Sách giáo khoa tin học lớp 3 Cánh diều",
                BookCollection = EBookCollection.CanhDieu,
                Description =
                    "Sách giáo khoa Tin học lớp 3 - Cánh diều là một công cụ hữu ích và phổ biến trong việc giáo dục cơ bản tại Việt Nam, dành cho học sinh ở cấp tiểu học. Cuốn sách này được thiết kế để trang bị kiến thức và kỹ năng cơ bản về máy tính và công nghệ thông tin cho học sinh, từ đó giúp các em hòa nhập tốt hơn vào môi trường giáo dục hiện đại và thế giới ngày càng kỹ thuật số. Cuốn sách gồm nhiều chủ đề được chia thành các phần và bài học cụ thể, giúp học sinh có thể dễ dàng theo dõi và tích lũy kiến thức từng bước một",
                KeyWord = "sach giao khoa tin học lop 3 canh dieu",
                GradeId = 1,
                Author = "Hồ Sĩ Đàm",
                PublicationYear = 2022,
                Edition = 1,
                TypeOfBook = EBookType.SGK,
                IsActive = true,
            },
            new()
            {
                Code = "CD0402",
                Title = "Sách giáo khoa tin học lớp 4 Cánh diều",
                BookCollection = EBookCollection.CanhDieu,
                Description =
                    "Sách giáo khoa Tin học lớp 4 Cánh diều được biên soạn theo chương trình giáo dục phổ thông 2018, với mục tiêu giúp học sinh hình thành và phát triển năng lực sử dụng máy tính và công nghệ thông tin một cách hiệu quả.",
                KeyWord = "sach giao khoa tin học lop 4 canh dieu",
                GradeId = 2,
                Author = "Hồ Sĩ Đàm, Hồ Cẩm Hà",
                PublicationYear = 2022,
                Edition = 1,
                TypeOfBook = EBookType.SGK,
                IsActive = true,
            },
            new()
            {
                Code = "CD0503",
                Title = "Sách giáo khoa tin học lớp 5 Cánh diều",
                BookCollection = EBookCollection.CanhDieu,
                Description =
                    "Sách giáo khoa Tin học lớp 5 Cánh diều được biên soạn theo chương trình giáo dục phổ thông 2018, với mục tiêu giúp học sinh hình thành và phát triển năng lực sử dụng máy tính và công nghệ thông tin một cách hiệu quả.",
                KeyWord = "sach giao khoa tin học lop 5 canh dieu",
                GradeId = 3,
                Author = "Hồ Sĩ Đàm, Hồ Cẩm Hà",
                PublicationYear = 2022,
                Edition = 1,
                TypeOfBook = EBookType.SGK,
                IsActive = true,
            },
            new()
            {
                Code = "CD0604",
                Title = "Sách giáo khoa tin học lớp 6 Cánh diều",
                BookCollection = EBookCollection.CanhDieu,
                Description =
                    "Sách giáo khoa Tin học lớp 6 Cánh diều được biên soạn theo chương trình giáo dục phổ thông 2018, với mục tiêu giúp học sinh hình thành và phát triển năng lực sử dụng máy tính và công nghệ thông tin một cách hiệu quả.",
                KeyWord = "sach giao khoa tin học lop 6 canh dieu",
                GradeId = 4,
                Author = "Hồ Sĩ Đàm, Hồ Cẩm Hà",
                PublicationYear = 2022,
                Edition = 1,
                TypeOfBook = EBookType.SGK,
                IsActive = true,
            },
        };
    }

    private static IEnumerable<Topic> SeedTopic()
    {
        return new List<Topic>()
        {
            new()
            {
                //1
                Title = "Chủ đề A Máy tính và em",
                KeyWord = "chu de a may tinh va em",
                Description =
                    "Chủ đề A máy tính và em trong sách giáo khoa Tin học lớp 3 của bộ sách Cánh Diều thường bao gồm các nội dung cơ bản về máy tính và cách sử dụng chúng. Dưới đây là một mô tả tổng quan về những gì có thể được bao gồm trong chủ đề này",
                Objectives =
                    "Hiểu biết cơ bản về máy tính, Sử dụng máy tính, Làm việc với hệ điểu hành, Sử dụng phần mềm cơ bản",
                IsActive = true,
                DocumentId = 1,
                ParentId = null,
            },
            new()
            {
                //2
                Title = "Chủ đề B Mạng máy tính và Internet",
                KeyWord = "chu de b mang may tinh va internet",
                Description =
                    "Chủ đề B mạng máy tính và internet trong sách giáo khoa Tin học lớp 3 của bộ sách Cánh Diều thường bao gồm các nội dung cơ bản về mạng máy tính và Internet. Dưới đây là một mô tả tổng quan về những gì có thể được bao gồm trong chủ đề này: Giới thiệu về mạng máy tính,Kết nối mạng máy tính,Internet là gì?",
                Objectives =
                    "Giúp học sinh hiểu khái niệm cơ bản về mạng máy tính và Internet, nhận biết các loại mạng và thành phần chính, cũng như cách kết nối và sử dụng mạng. Học sinh sẽ biết cách sử dụng trình duyệt web để tìm kiếm thông tin, sử dụng email, và nhận thức được tầm quan trọng của an toàn mạng, bao gồm bảo vệ thông tin cá nhân và quyền riêng tư trực tuyến",
                IsActive = true,
                DocumentId = 1,
                ParentId = null,
            },
            new()
            {
                //3
                Title = "Chủ đề C Tổ chức lưu trữ, tìm kiếm và trao đổi thông tin",
                KeyWord = "chu de d dao duc, phap luat va van hoa trong moi truong so",
                Description =
                    "Chủ đề này cũng nhấn mạnh vai trò của giáo dục và hướng nghiệp trong việc sử dụng công nghệ một cách có trách nhiệm và mang tính xây dựng cho cá nhân và cộng đồng.",
                Objectives = "Hiểu và áp dụng các giá trị đạo đức và đạo lý khi sử dụng công nghệ và Internet.",
                IsActive = true,
                DocumentId = 1,
                ParentId = null
            },
            new()
            {
                //4

                Title = "Chủ đề D Đạo đức, Pháp luật và Văn hóa trong môi trường số",
                KeyWord = "chu de d dao duc, phap luat va van hoa trong moi truong so",
                Description =
                    "Chủ đề này cũng nhấn mạnh vai trò của giáo dục và hướng nghiệp trong việc sử dụng công nghệ một cách có trách nhiệm và mang tính xây dựng cho cá nhân và cộng đồng.",
                Objectives = "Hiểu và áp dụng các giá trị đạo đức và đạo lý khi sử dụng công nghệ và Internet.",
                IsActive = true,
                DocumentId = 1,
                ParentId = null
            },
            new()
            {
                //5

                Title = "Chủ đề E Ứng dụng tin học",
                KeyWord = "chu de e ung dung tin hoc",
                Description =
                    "Chủ đề này giới thiệu cho học sinh về các ứng dụng cụ thể của tin học trong cuộc sống hàng ngày. Học sinh sẽ được hướng dẫn cách sử dụng các phần mềm và công cụ tin học để giải quyết các vấn đề thực tế và hỗ trợ trong học tập.",
                Objectives =
                    "Hiểu và áp dụng các ứng dụng cụ thể của tin học trong đời sống hàng ngày, như việc sử dụng phần mềm văn phòng (word, excel), các ứng dụng học tập và giải trí.",
                IsActive = true,
                DocumentId = 1,
                ParentId = null
            },
            new()
            {
                //6

                Title = "Chủ đề A1 Khám phá máy tính",
                KeyWord = "chu de e ung dung tin hoc",
                Description =
                    "Chủ đề này giới thiệu cho học sinh về các ứng dụng cụ thể của tin học trong cuộc sống hàng ngày. Học sinh sẽ được hướng dẫn cách sử dụng các phần mềm và công cụ tin học để giải quyết các vấn đề thực tế và hỗ trợ trong học tập.",
                Objectives =
                    "Hiểu và áp dụng các ứng dụng cụ thể của tin học trong đời sống hàng ngày, như việc sử dụng phần mềm văn phòng (word, excel), các ứng dụng học tập và giải trí.",
                IsActive = true,
                DocumentId = 1,
                ParentId = 1
            },
            new()
            {
                //7

                Title = "Chủ đề A2 Thông tin và xử lý thông tin",
                KeyWord = "chu de e ung dung tin hoc",
                Description =
                    "Chủ đề này giới thiệu cho học sinh về các ứng dụng cụ thể của tin học trong cuộc sống hàng ngày. Học sinh sẽ được hướng dẫn cách sử dụng các phần mềm và công cụ tin học để giải quyết các vấn đề thực tế và hỗ trợ trong học tập.",
                Objectives =
                    "Hiểu và áp dụng các ứng dụng cụ thể của tin học trong đời sống hàng ngày, như việc sử dụng phần mềm văn phòng (word, excel), các ứng dụng học tập và giải trí.",
                IsActive = true,
                DocumentId = 1,
                ParentId = 1
            },
            new()
            {
                //8

                Title = "Chủ đề A3 Làm quen với cách gõ bàn phím",
                KeyWord = "chu de e ung dung tin hoc",
                Description =
                    "Chủ đề này giới thiệu cho học sinh về các ứng dụng cụ thể của tin học trong cuộc sống hàng ngày. Học sinh sẽ được hướng dẫn cách sử dụng các phần mềm và công cụ tin học để giải quyết các vấn đề thực tế và hỗ trợ trong học tập.",
                Objectives =
                    "Hiểu và áp dụng các ứng dụng cụ thể của tin học trong đời sống hàng ngày, như việc sử dụng phần mềm văn phòng (word, excel), các ứng dụng học tập và giải trí.",
                IsActive = true,
                DocumentId = 1,
                ParentId = 1
            },
        };
    }

    private static IEnumerable<Lesson> SeedLesson()
    {
        return new List<Lesson>()
        {
            new()
            {
                Title = "Bài 1 Các thành phần của máy tính",
                KeyWord = "bai 1 cac thanh phan cua may tinh",
                IsActive = true,
                Content = "Content of Lesson 1",
                FilePath =
                    "https://res.cloudinary.com/itsupport18/raw/upload/v1718987928/LessonFile/FILE-5a5f56e6-6081-47d3-81db-fa35f3a898e0.pdf",
                PublicId = "LessonFile/FILE-5a5f56e6-6081-47d3-81db-fa35f3a898e0.pdf",
                UrlDownload =
                    "https://res.cloudinary.com/itsupport18/raw/upload/fl_attachment/v1/LessonFile/FILE-5a5f56e6-6081-47d3-81db-fa35f3a898e0.pdf",
                TopicId = 6,
            },
            new()
            {
                Title = "Bài 2 Những máy tính thông dụng",
                KeyWord = "bai 2 nhung may tinh thong dung",
                IsActive = true,
                Content = "Content of Lesson 2",
                FilePath =
                    "https://res.cloudinary.com/itsupport18/raw/upload/v1718987951/LessonFile/FILE-a662a89a-8bc1-4ba9-98dc-3580b5ae1782.pdf",
                PublicId = "LessonFile/FILE-a662a89a-8bc1-4ba9-98dc-3580b5ae1782.pdf",
                UrlDownload =
                    "https://res.cloudinary.com/itsupport18/raw/upload/fl_attachment/v1/LessonFile/FILE-a662a89a-8bc1-4ba9-98dc-3580b5ae1782.pdf",
                TopicId = 6,
            },
            new Lesson()
            {
                Title = "Bài 3: Em tập sử dụng chuột",
                KeyWord = "bai 3: em tap su dung chuot",
                IsActive = true,
                Content = "Content of Lesson 3",
                FilePath =
                    "https://res.cloudinary.com/itsupport18/raw/upload/v1718987972/LessonFile/FILE-3a204b73-f357-490d-9a42-f11b42318ca5.pdf",
                PublicId = "LessonFile/FILE-3a204b73-f357-490d-9a42-f11b42318ca5.pdf",
                UrlDownload =
                    "https://res.cloudinary.com/itsupport18/raw/upload/fl_attachment/v1/LessonFile/FILE-3a204b73-f357-490d-9a42-f11b42318ca5.pdf",
                TopicId = 6,
            }
        };
    }
}