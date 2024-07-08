﻿using AutoMapper;
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
                //1
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
                //2
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
                //3
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
                //4
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
            new()
            {
                //5
                Code = "CD0605",
                Title = "Sách giáo khoa tin học lớp 7 Cánh diều",
                BookCollection = EBookCollection.CanhDieu,
                Description =
                    "Sách giáo khoa Tin học lớp 7 Cánh diều được biên soạn theo chương trình giáo dục phổ thông 2018, với mục tiêu giúp học sinh hình thành và phát triển năng lực sử dụng máy tính và công nghệ thông tin một cách hiệu quả.",
                KeyWord = "sach giao khoa tin học lop 7 canh dieu",
                GradeId = 5,
                Author = "Hồ Sĩ Đàm, Hồ Cẩm Hà",
                PublicationYear = 2022,
                Edition = 1,
                TypeOfBook = EBookType.SGK,
                IsActive = true,
            },
            new()
            {
                //6
                Code = "CD0606",
                Title = "Sách giáo khoa tin học lớp 8 Cánh diều",
                BookCollection = EBookCollection.CanhDieu,
                Description =
                    "Sách giáo khoa Tin học lớp 8 Cánh diều được biên soạn theo chương trình giáo dục phổ thông 2018, với mục tiêu giúp học sinh hình thành và phát triển năng lực sử dụng máy tính và công nghệ thông tin một cách hiệu quả.",
                KeyWord = "sach giao khoa tin học lop 8 canh dieu",
                GradeId = 6,
                Author = "Hồ Sĩ Đàm, Hồ Cẩm Hà",
                PublicationYear = 2022,
                Edition = 1,
                TypeOfBook = EBookType.SGK,
                IsActive = true,
            },
            new()
            {
                //7
                Code = "CD0606",
                Title = "Sách giáo khoa tin học lớp 9 Cánh diều",
                BookCollection = EBookCollection.CanhDieu,
                Description =
                    "Sách giáo khoa Tin học lớp 9 Cánh diều được biên soạn theo chương trình giáo dục phổ thông 2018, với mục tiêu giúp học sinh hình thành và phát triển năng lực sử dụng máy tính và công nghệ thông tin một cách hiệu quả.",
                KeyWord = "sach giao khoa tin học lop 9 canh dieu",
                GradeId = 7,
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

                Title = "Chủ đề F: Giải quyết vấn đề với sự trợ giúp của máy tính",
                KeyWord = "chu de f: giai quyet van de voi su tro giup cua may tinh",
                Description =
                    "Chủ đề này giúp học sinh hiểu cách sử dụng máy tính để giải quyết các vấn đề trong cuộc sống hàng ngày. Các em sẽ học cách áp dụng các kỹ năng tin học để tìm kiếm thông tin, xử lý dữ liệu và đưa ra quyết định. Chủ đề này khuyến khích tư duy sáng tạo và logic thông qua việc sử dụng các phần mềm và công cụ hỗ trợ học tập.",
                Objectives =
                    "Nhận diện và xác định vấn đề: Học sinh có khả năng nhận diện các vấn đề cần giải quyết trong học tập và cuộc sống.\nTìm kiếm và thu thập thông tin: Học sinh biết cách sử dụng máy tính để tìm kiếm và thu thập thông tin liên quan đến vấn đề cần giải quyết",
                IsActive = true,
                DocumentId = 1,
                ParentId = null
            },
            new()
            {
                //7

                Title = "Chủ đề A1. Khám phá máy tính",
                KeyWord = "chu de a1. ung dung tin hoc",
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

                Title = "Chủ đề A2. Thông tin và xử lí thông tin",
                KeyWord = "chu de a2. thong tin va xu li thong tin",
                Description =
                    "Chủ đề này giúp học sinh hiểu về khái niệm thông tin và cách thông tin được xử lý trong máy tính. Học sinh sẽ được giới thiệu về các loại thông tin khác nhau, cách thu thập và tổ chức thông tin, cũng như các phương pháp cơ bản để xử lý và sử dụng thông tin một cách hiệu quả. Qua đó, học sinh sẽ nắm bắt được vai trò của thông tin và xử lý thông tin trong học tập và cuộc sống.",
                Objectives =
                    "Hiểu khái niệm thông tin: Nhận biết và định nghĩa các khái niệm cơ bản về thông tin. Phân loại thông tin: Phân loại thông tin theo các tiêu chí khác nhau như âm thanh, hình ảnh, văn bản, v.v. Thu thập và tổ chức thông tin: Thu thập thông tin từ các nguồn khác nhau và tổ chức một cách có hệ thống. Xử lý thông tin: Sử dụng các công cụ và phần mềm đơn giản để xử lý thông tin. Sử dụng thông tin hiệu quả: Áp dụng thông tin đã xử lý để giải quyết các vấn đề cụ thể trong học tập và cuộc sống.",
                IsActive = true,
                DocumentId = 1,
                ParentId = 1
            },
            new()
            {
                //9

                Title = "Chủ đề A3. Làm quen với cách gõ bàn phím",
                KeyWord = "chu de a3. lam quen voi cach go ban phim",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 1
            },
            new()
            {
                //10

                Title = "Chủ đề C1. Sắp xếp để dễ tìm",
                KeyWord = "chu de c1. sap xep de de tim",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 3
            },
            new()
            {
                //11

                Title = "Chủ đề C2. Làm quen với thư mục lưu trữ thông tin trong máy tính",
                KeyWord = "chu de c2. lam quen voi thu muc luu tru thong tin trong may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 3
            },
            new()
            {
                //12

                Title = "Chủ đề E1. Làm quen với bài trình chiếu đơn giản",
                KeyWord = "chu de e1. lam quen voi bai trinh chieu don gian",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 5
            },
            new()
            {
                //13

                Title = "Chủ đề E2. Sử dụng phần mềm luyện tập thao tác với chuột máy tính",
                KeyWord = "chu de e2. su dung phan mem luyen tap va thao tac voi chuot may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 5
            },
            new()
            {
                //14

                Title = "Chủ đề E3. Sử dụng công cụ đa phương tiện để tìm hiểu thế giới tự nhiên",
                KeyWord = "chu de e3. su dung cong cu da phuong tien de tim hieu the gioi tu nhien",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 5
            },
            new()
            {
                //15

                Title = "Chủ đề F1. Thực hiện công việc theo các bước",
                KeyWord = "chu de f1. thuc hien cong viec theo cac buoc",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 6
            },
            new()
            {
                //16

                Title = "Chủ đề F2. Nhiệm vụ của em và sự trợ giúp của máy tính",
                KeyWord = "chu de f2. nhiem vu cua em va su tro giup cua may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 1,
                ParentId = 6
            },
            
            //Lop 4 
            new()
            {
                //17
                Title = "Chủ đề A Máy tính và em",
                KeyWord = "chu de a may tinh va em",
                Description =
                    "Chủ đề A máy tính và em trong sách giáo khoa Tin học lớp 3 của bộ sách Cánh Diều thường bao gồm các nội dung cơ bản về máy tính và cách sử dụng chúng. Dưới đây là một mô tả tổng quan về những gì có thể được bao gồm trong chủ đề này",
                Objectives =
                    "Hiểu biết cơ bản về máy tính, Sử dụng máy tính, Làm việc với hệ điểu hành, Sử dụng phần mềm cơ bản",
                IsActive = true,
                DocumentId = 2,
                ParentId = null,
            },
            new()
            {
                //18
                Title = "Chủ đề B Mạng máy tính và Internet - Thông tin trên trang web",
                KeyWord = "chu de b mang may tinh va internet - thong tin tren trang web",
                Description =
                    "Chủ đề B mạng máy tính và internet trong sách giáo khoa Tin học lớp 3 của bộ sách Cánh Diều thường bao gồm các nội dung cơ bản về mạng máy tính và Internet. Dưới đây là một mô tả tổng quan về những gì có thể được bao gồm trong chủ đề này: Giới thiệu về mạng máy tính,Kết nối mạng máy tính,Internet là gì?",
                Objectives =
                    "Giúp học sinh hiểu khái niệm cơ bản về mạng máy tính và Internet, nhận biết các loại mạng và thành phần chính, cũng như cách kết nối và sử dụng mạng. Học sinh sẽ biết cách sử dụng trình duyệt web để tìm kiếm thông tin, sử dụng email, và nhận thức được tầm quan trọng của an toàn mạng, bao gồm bảo vệ thông tin cá nhân và quyền riêng tư trực tuyến",
                IsActive = true,
                DocumentId = 2,
                ParentId = null,
            },
            new()
            {
                //19
                Title = "Chủ đề C Tổ chức lưu trữ, tìm kiếm và trao đổi thông tin",
                KeyWord = "chu de d dao duc, phap luat va van hoa trong moi truong so",
                Description =
                    "Chủ đề này cũng nhấn mạnh vai trò của giáo dục và hướng nghiệp trong việc sử dụng công nghệ một cách có trách nhiệm và mang tính xây dựng cho cá nhân và cộng đồng.",
                Objectives = "Hiểu và áp dụng các giá trị đạo đức và đạo lý khi sử dụng công nghệ và Internet.",
                IsActive = true,
                DocumentId =2 ,
                ParentId = null
            },
            new()
            {
                //20

                Title = "Chủ đề D Đạo đức, Pháp luật và Văn hóa trong môi trường số - bản quyền sử dụng phần mềm",
                KeyWord = "chu de d dao duc, phap luat va van hoa trong moi truong so - ban quyen su dung phan mem",
                Description =
                    "Chủ đề này cũng nhấn mạnh vai trò của giáo dục và hướng nghiệp trong việc sử dụng công nghệ một cách có trách nhiệm và mang tính xây dựng cho cá nhân và cộng đồng.",
                Objectives = "Hiểu và áp dụng các giá trị đạo đức và đạo lý khi sử dụng công nghệ và Internet.",
                IsActive = true,
                DocumentId = 2,
                ParentId = null
            },
            new()
            {
                //21

                Title = "Chủ đề E Ứng dụng tin học",
                KeyWord = "chu de e ung dung tin hoc",
                Description =
                    "Chủ đề này giới thiệu cho học sinh về các ứng dụng cụ thể của tin học trong cuộc sống hàng ngày. Học sinh sẽ được hướng dẫn cách sử dụng các phần mềm và công cụ tin học để giải quyết các vấn đề thực tế và hỗ trợ trong học tập.",
                Objectives =
                    "Hiểu và áp dụng các ứng dụng cụ thể của tin học trong đời sống hàng ngày, như việc sử dụng phần mềm văn phòng (word, excel), các ứng dụng học tập và giải trí.",
                IsActive = true,
                DocumentId = 2,
                ParentId = null
            },
            new()
            {
                //22

                Title = "Chủ đề F: Giải quyết vấn đề với sự trợ giúp của máy tính",
                KeyWord = "chu de f: giai quyet van de voi su tro giup cua may tinh",
                Description =
                    "Chủ đề này giúp học sinh hiểu cách sử dụng máy tính để giải quyết các vấn đề trong cuộc sống hàng ngày. Các em sẽ học cách áp dụng các kỹ năng tin học để tìm kiếm thông tin, xử lý dữ liệu và đưa ra quyết định. Chủ đề này khuyến khích tư duy sáng tạo và logic thông qua việc sử dụng các phần mềm và công cụ hỗ trợ học tập.",
                Objectives =
                    "Nhận diện và xác định vấn đề: Học sinh có khả năng nhận diện các vấn đề cần giải quyết trong học tập và cuộc sống.\nTìm kiếm và thu thập thông tin: Học sinh biết cách sử dụng máy tính để tìm kiếm và thu thập thông tin liên quan đến vấn đề cần giải quyết",
                IsActive = true,
                DocumentId = 2,
                ParentId = null
            },
             new()
            {
                //23

                Title = "Chủ đề A1. Phần cứng và phần mềm",
                KeyWord = "chu de a1. phan cung va phan mem",
                Description =
                    "Chủ đề này giới thiệu cho học sinh về các ứng dụng cụ thể của tin học trong cuộc sống hàng ngày. Học sinh sẽ được hướng dẫn cách sử dụng các phần mềm và công cụ tin học để giải quyết các vấn đề thực tế và hỗ trợ trong học tập.",
                Objectives =
                    "Hiểu và áp dụng các ứng dụng cụ thể của tin học trong đời sống hàng ngày, như việc sử dụng phần mềm văn phòng (word, excel), các ứng dụng học tập và giải trí.",
                IsActive = true,
                DocumentId = 2,
                ParentId = 17
            },
            new()
            {
                //24

                Title = "Chủ đề A2. Lợi ích của việc gõ bàn phím đúng cách",
                KeyWord = "chu de a2. loi ich cua viec go ban phim dung cach",
                Description =
                    "Chủ đề này giúp học sinh hiểu về khái niệm thông tin và cách thông tin được xử lý trong máy tính. Học sinh sẽ được giới thiệu về các loại thông tin khác nhau, cách thu thập và tổ chức thông tin, cũng như các phương pháp cơ bản để xử lý và sử dụng thông tin một cách hiệu quả. Qua đó, học sinh sẽ nắm bắt được vai trò của thông tin và xử lý thông tin trong học tập và cuộc sống.",
                Objectives =
                    "Hiểu khái niệm thông tin: Nhận biết và định nghĩa các khái niệm cơ bản về thông tin. Phân loại thông tin: Phân loại thông tin theo các tiêu chí khác nhau như âm thanh, hình ảnh, văn bản, v.v. Thu thập và tổ chức thông tin: Thu thập thông tin từ các nguồn khác nhau và tổ chức một cách có hệ thống. Xử lý thông tin: Sử dụng các công cụ và phần mềm đơn giản để xử lý thông tin. Sử dụng thông tin hiệu quả: Áp dụng thông tin đã xử lý để giải quyết các vấn đề cụ thể trong học tập và cuộc sống.",
                IsActive = true,
                DocumentId = 2,
                ParentId = 17
            },
           
            new()
            {
                //25

                Title = "Chủ đề C1. Bước đầu tìm kiếm thông tin trên internet",
                KeyWord = "chu de c1. buoc dau tim kiem thong tin tren internet",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 2,
                ParentId = 19
            },
            new()
            {
                //26

                Title = "Chủ đề C2. Tổ chức cây thư mục lưu trữ thông tin trong máy tính",
                KeyWord = "chu de c2. to chuc cay thu muc luu tru thong tin trong may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 2,
                ParentId = 19
            },
            new()
            {
                //27

                Title = "Chủ đề E1. Tạo bài trình chiếu",
                KeyWord = "chu de e1. lam quen voi bai trinh chieu don gian",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 2,
                ParentId = 21
            },
            new()
            {
                //28

                Title = "Chủ đề E2. Tập soạn thảo văn bản",
                KeyWord = "chu de e2. su dung phan mem luyen tap va thao tac voi chuot may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 2,
                ParentId = 21
            },
            new()
            {
                //29

                Title = "Lựa chọn 1. Sử dụng công cụ đa phương tiện để tìm hiểu lịch sử, văn hóa",
                KeyWord = "lua chon 1. su dung cong cu da phuong tien de tim hieu lich su, van hoa",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 2,
                ParentId = 21
            },
            new()
            {
                //30

                Title = "Lựa chọn 2. Sử dụng phần mềm luyện gõ bàn phím",
                KeyWord = "lua chon 2. su dung phan mem luyen go ban phim",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 2,
                ParentId = 21
            },
            //Lop 5
             new()
            {
                //31
                Title = "Chủ đề A Máy tính và em - Những việc em có thể làm được nhờ máy tính",
                KeyWord = "chu de a may tinh va em - nhung viec em co the lam duoc nho may tinh",
                Description =
                    "Chủ đề A máy tính và em trong sách giáo khoa Tin học lớp 3 của bộ sách Cánh Diều thường bao gồm các nội dung cơ bản về máy tính và cách sử dụng chúng. Dưới đây là một mô tả tổng quan về những gì có thể được bao gồm trong chủ đề này",
                Objectives =
                    "Hiểu biết cơ bản về máy tính, Sử dụng máy tính, Làm việc với hệ điểu hành, Sử dụng phần mềm cơ bản",
                IsActive = true,
                DocumentId = 3,
                ParentId = null,
            },
            new()
            {
                //32
                Title = "Chủ đề B Mạng máy tính và Internet - Tìm kiếm thông tin trên website",
                KeyWord = "chu de b mang may tinh va internet - tim kiem thong tin tren website",
                Description =
                    "Chủ đề B mạng máy tính và internet trong sách giáo khoa Tin học lớp 3 của bộ sách Cánh Diều thường bao gồm các nội dung cơ bản về mạng máy tính và Internet. Dưới đây là một mô tả tổng quan về những gì có thể được bao gồm trong chủ đề này: Giới thiệu về mạng máy tính,Kết nối mạng máy tính,Internet là gì?",
                Objectives =
                    "Giúp học sinh hiểu khái niệm cơ bản về mạng máy tính và Internet, nhận biết các loại mạng và thành phần chính, cũng như cách kết nối và sử dụng mạng. Học sinh sẽ biết cách sử dụng trình duyệt web để tìm kiếm thông tin, sử dụng email, và nhận thức được tầm quan trọng của an toàn mạng, bao gồm bảo vệ thông tin cá nhân và quyền riêng tư trực tuyến",
                IsActive = true,
                DocumentId = 3,
                ParentId = null,
            },
            new()
            {
                //33
                Title = "Chủ đề C Tổ chức lưu trữ, tìm kiếm và trao đổi thông tin",
                KeyWord = "chu de d dao duc, phap luat va van hoa trong moi truong so",
                Description =
                    "Chủ đề này cũng nhấn mạnh vai trò của giáo dục và hướng nghiệp trong việc sử dụng công nghệ một cách có trách nhiệm và mang tính xây dựng cho cá nhân và cộng đồng.",
                Objectives = "Hiểu và áp dụng các giá trị đạo đức và đạo lý khi sử dụng công nghệ và Internet.",
                IsActive = true,
                DocumentId =3,
                ParentId = null
            },
            new()
            {
                //34

                Title = "Chủ đề D Đạo đức, Pháp luật và Văn hóa trong môi trường số - bản quyền nội dung thông tin",
                KeyWord = "chu de d dao duc, phap luat va van hoa trong moi truong so - ban quyen noi dung thong tin",
                Description =
                    "Chủ đề này cũng nhấn mạnh vai trò của giáo dục và hướng nghiệp trong việc sử dụng công nghệ một cách có trách nhiệm và mang tính xây dựng cho cá nhân và cộng đồng.",
                Objectives = "Hiểu và áp dụng các giá trị đạo đức và đạo lý khi sử dụng công nghệ và Internet.",
                IsActive = true,
                DocumentId = 3,
                ParentId = null
            },
            new()
            {
                //35

                Title = "Chủ đề E Ứng dụng tin học - Trình soạn thảo văn bản",
                KeyWord = "chu de e ung dung tin hoc",
                Description =
                    "Chủ đề này giới thiệu cho học sinh về các ứng dụng cụ thể của tin học trong cuộc sống hàng ngày. Học sinh sẽ được hướng dẫn cách sử dụng các phần mềm và công cụ tin học để giải quyết các vấn đề thực tế và hỗ trợ trong học tập.",
                Objectives =
                    "Hiểu và áp dụng các ứng dụng cụ thể của tin học trong đời sống hàng ngày, như việc sử dụng phần mềm văn phòng (word, excel), các ứng dụng học tập và giải trí.",
                IsActive = true,
                DocumentId = 3,
                ParentId = null
            },
            new()
            {
                //36

                Title = "Chủ đề F: Giải quyết vấn đề với sự trợ giúp của máy tính - Chơi và khám phá trong môi trường lập trình trực quan",
                KeyWord = "chu de f: giai quyet van de voi su tro giup cua may tinh - choi va kham pha trong moi truong lap trinh truc quan",
                Description =
                    "Chủ đề này giúp học sinh hiểu cách sử dụng máy tính để giải quyết các vấn đề trong cuộc sống hàng ngày. Các em sẽ học cách áp dụng các kỹ năng tin học để tìm kiếm thông tin, xử lý dữ liệu và đưa ra quyết định. Chủ đề này khuyến khích tư duy sáng tạo và logic thông qua việc sử dụng các phần mềm và công cụ hỗ trợ học tập.",
                Objectives =
                    "Nhận diện và xác định vấn đề: Học sinh có khả năng nhận diện các vấn đề cần giải quyết trong học tập và cuộc sống.\nTìm kiếm và thu thập thông tin: Học sinh biết cách sử dụng máy tính để tìm kiếm và thu thập thông tin liên quan đến vấn đề cần giải quyết",
                IsActive = true,
                DocumentId = 3,
                ParentId = null
            },
           
            new()
            {
                //37

                Title = "Chủ đề C1. Tìm kiếm thông tin trong giải quyết vấn đề",
                KeyWord = "chu de c1. tim kiem thong tin trong giai quyet van de",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 3,
                ParentId = 33
            },
            new()
            {
                //38

                Title = "Lựa chọn 1. Sử dụng phần mềm đồ họa tạo sản phẩm số đơn giản",
                KeyWord = "chu de c2. cay thu muc va tim kiem tep tren may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 3,
                ParentId = 35
            },
            new()
            {
                //39

                Title = "Lựa chon 2. Sử dụng công cụ đa phương tiện hỗ trợ tạo sản phẩm đơn giản",
                KeyWord = "chu de e1. lam quen voi bai trinh chieu don gian",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 3,
                ParentId = 35
            },
            
            // Lớp 6
            new()
            {
                //40
                Title = "Chủ đề A. Máy tính và cộng đồng",
                KeyWord = "chu de a. may tinh va cong dong",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 4,
                ParentId = null
            },
            new()
            {
                //41
                Title = "Chủ đề B. Mạng máy tính và internet",
                KeyWord = "chu de b. Mạng máy tính và internet",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 4,
                ParentId = null
            },
            new()
            {
                //42
                Title = "Chủ đề C. Tổ chức lưu trữ, tìm kiếm và trao đổi thông tin",
                KeyWord = "chu de c. to chuc luu tru, tim kiem va trao doi thong tin",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 4,
                ParentId = null
            },
            new()
            {
                //43
                Title = "Chủ đề D. Đạo đức, pháp luật và văn hóa trong môi trường số",
                KeyWord = "chu de d. dao duc, phap luat va van hoa trong moi truong so",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 4,
                ParentId = null
            }, 
            new()
            {
                //44
                Title = "Chủ đề E. Ứng dụng tin học",
                KeyWord = "chu de e. ung dung tin hoc",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 4,
                ParentId = null
            }, 
            new()
            {
                //45
                Title = "Chủ đề F. Giải quyết vấn đề với sự trợ giúp của máy tính",
                KeyWord = "chu de f. giai quyet van de voi su tro giup cua may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 4,
                ParentId = null
            }, 
            
            
              // Lớp 7 - Canh dieu
            new()
            {
                //46
                Title = "Chủ đề A. Máy tính và cộng đồng - sơ lược về các thành phần của máy tính. Khái niệm hệ điều hành và phần mềm ứng dụng",
                KeyWord = "chu de a. may tinh va cong dong - so luoc ve cac thanh phan cua may tinh. khai niem he dieu hanh va phan mem ung dung",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 5,
                ParentId = null
            },
           
            new()
            {
                //47
                Title = "Chủ đề C. Tổ chức lưu trữ, tìm kiếm và trao đổi thông tin - mạng xã hội và một số kênh trao đổi thông tin thông dụng trên internet",
                KeyWord = "chu de c. to chuc luu tru, tim kiem va trao doi thong tin - mang xa hoi va mot so kenh trao doi thong tin thong dung tren internet",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 5,
                ParentId = null
            },
            new()
            {
                //48
                Title = "Chủ đề D. Đạo đức, pháp luật và văn hóa trong môi trường số - văn hóa ứng xử qua phương tiện truyền thông số",
                KeyWord = "chu de d. dao duc, phap luat va van hoa trong moi truong so - van hoa ung xu qua phuong tien truyen thong so",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 5,
                ParentId = null
            }, 
            new()
            {
                //49
                Title = "Chủ đề E. Ứng dụng tin học - bảng tính điện tử cơ bản, phần mềm trình chiếu cơ bản",
                KeyWord = "chu de e. ung dung tin hoc - bảng tính điện tử cơ bản, phần mềm trình chiếu cơ bản",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 5,
                ParentId = null
            }, 
            new()
            {
                //50
                Title = "Chủ đề F. Giải quyết vấn đề với sự trợ giúp của máy tính - Một số thuật toán sắp xếp và tìm kiếm cơ bản",
                KeyWord = "chu de f. giai quyet van de voi su tro giup cua may tinh - mot so thuat toan sap xep va tim kiem co ban",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 5,
                ParentId = null
            }, 
                 // Lớp 8 - Canh dieu
            new()
            {
                //51
                Title = "Chủ đề A. Máy tính và cộng đồng - sơ lược về lịch sử phát triển máy tính",
                KeyWord = "chu de a. may tinh va cong dong - so luoc ve lich su phat trien may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = null
            },
           
            new()
            {
                //52
                Title = "Chủ đề C. Tổ chức lưu trữ, tìm kiếm và trao đổi thông tin - đặc điểm của thông tin trong môi trường số thông tin với giải quyết vấn đề",
                KeyWord = "chu de c. to chuc luu tru, tim kiem va trao doi thong tin - dac diem cua thong tin trong moi truong so thong tin voi giai quyet van de",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = null
            },
            new()
            {
                //53
                Title = "Chủ đề D. Đạo đức, pháp luật và văn hóa trong môi trường số - đạo đức và văn hóa trong sử dụng công nghệ kĩ thuật số",
                KeyWord = "chu de d. dao duc, phap luat va van hoa trong moi truong so - dao duc va van hoa trong su dung cong nghe ki thuat so",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = null
            }, 
            new()
            {
                //54
                Title = "Chủ đề E. Ứng dụng tin học",
                KeyWord = "chu de e. ung dung tin hoc",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = null
            }, 
            new()
            {
                //55
                Title = "Chủ đề E1. Xử lí và trực quan hóa dữ liệu bằng bảng điện tử",
                KeyWord = "chu de e1. xu li va truc quan hoa du lieu bang bang dien tu",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = 54
            }, 
            new()
            {
                //56
                Title = "Chủ đề E2. Soạn thảo văn bản và phần mềm trình chiếu nâng cao",
                KeyWord = "chu de e. soan thao van ban va phan mem trinh chieu nang cao",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = 54
            }, 
            new()
            {
                //57
                Title = "Chủ đề E3. Làm quen với phần mềm chỉnh sửa ảnh",
                KeyWord = "chu de e. lam quen voi phan mem chinh sua anh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = 54
            }, 
            new()
            {
                //58
                Title = "Chủ đề F. Giải quyết vấn đề với sự trợ giúp của máy tính - Lập trình trực quan",
                KeyWord = "chu de f. giai quyet van de voi su tro giup cua may tinh - lap trinh truc quan",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = null
            }, 
            new()
            {
                //59
                Title = "Chủ đề G. Hướng nghiệp với tin học - tin học và hành nghề",
                KeyWord = "chu de g. huong nghiep voi tin hoc - tin hoc va hanh nghe",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 6,
                ParentId = null
            }, 
            // Lớp 9 Cánh diều
            new()
            {
                //60
                Title = "Chủ đề A Máy tính và cộng đồng - vai trò của máy tính trong đời sống",
                KeyWord = "chu de a may tinh va cong dong - vai tro cua may tinh trong doi song",
                Description =
                    "Chủ đề A máy tính và em trong sách giáo khoa Tin học lớp 9 của bộ sách Cánh Diều thường bao gồm các nội dung cơ bản về máy tính và cách sử dụng chúng. Dưới đây là một mô tả tổng quan về những gì có thể được bao gồm trong chủ đề này",
                Objectives =
                    "Hiểu biết cơ bản về máy tính, Sử dụng máy tính, Làm việc với hệ điểu hành, Sử dụng phần mềm cơ bản",
                IsActive = true,
                DocumentId = 7,
                ParentId = null,
            },
            
            new()
            {
                //61
                Title = "Chủ đề C. Tổ chức lưu trữ, tìm kiếm và trao đổi thông tin - đánh giá chất lượng thông tin trong giải quyết vấn đề",
                KeyWord = "chu de c. to chuc luu tru, tim kiem va trao doi thong tin - danh gia chat luong thong tin trong giai quyet van de",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = null
            },
            new()
            {
                //62
                Title = "Chủ đề D. Đạo đức, pháp luật và văn hóa trong môi trường số - một số vấn đề pháp lí về sử dụng dịch vụ internet",
                KeyWord = "chu de d. dao duc, phap luat va van hoa trong moi truong so - mot so van de phap li ve su dung dich vu internet",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = null
            }, 
            new()
            {
                //63
                Title = "Chủ đề E. Ứng dụng tin học",
                KeyWord = "chu de e. ung dung tin hoc",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = null
            }, 
            //
            new()
            {
                //64
                Title = "Chủ đề E1. Phần mềm mô phỏng và khám phá tri thức",
                KeyWord = "chu de e. phan mem mo phong va kham pha tri thuc",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = 63
            },  new()
            {
                //65
                Title = "Chủ đề E2. Trình bày thông tin trong trao đổi và hợp tác",
                KeyWord = "chu de e2. trinh bay thong tin trong trao doi va hop tac",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = 63
            }, 
            new()
            {
                //66
                Title = "Chủ đề E3. Sử dụng bảng tính điện tử nâng cao",
                KeyWord = "chu de e3. su dung bang tinh dien tu nang cao",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = 63
            }, 
            new()
            {
                //67
                Title = "Chủ đề E4. Làm quen với phần mềm làm video",
                KeyWord = "chu de e4. lam quen voi phan mem lam video",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = 63
            }, 
            new()
            {
                //68
                Title = "Chủ đề F. Giải quyết vấn đề với sự trợ giúp của máy tính - giải bài toán bằng máy tính",
                KeyWord = "chu de f. giai quyet van de voi su tro giup cua may tinh - giai bai toan bang may tinh",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = null
            }, 
            new()
            {
                //69
                Title = "Chủ đề G. Hướng nghiệp với tin học - tin học và định hướng nghề nghiệp",
                KeyWord = "chu de g. huong nghiep voi tin hoc - tin hoc va dinh huong nghe nghiep",
                Description =
                    "Chủ đề này giúp học sinh làm quen với bàn phím máy tính, từ đó phát triển kỹ năng gõ phím một cách chính xác và hiệu quả. Học sinh sẽ học cách nhận biết và sử dụng các phím chức năng, phím chữ cái, phím số, và các phím đặc biệt khác. Mục tiêu là giúp học sinh trở nên thành thạo trong việc sử dụng bàn phím để soạn thảo văn bản và thực hiện các tác vụ khác trên máy tính.",
                Objectives =
                    "Nhận biết các phím trên bàn phím, Học cách gõ phím đúng, Thực hành văn bản, Sử dụng phím chức năng, Tăng tốc độ và hiệu quả",
                IsActive = true,
                DocumentId = 7,
                ParentId = null
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