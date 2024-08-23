using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Common.ModelMapping;
using LW.Data.Entities;
using LW.Shared.Constant;
using LW.Shared.DTOs.Competition;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Member;
using LW.Shared.DTOs.Tag;
using LW.Shared.DTOs.Topic;
using LW.Shared.Enums;
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
            var result = await context.Users.Select(u => u.ToUserDto(context)).ToListAsync();
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticUsers).IndexMany(result));
            logger.Information("Seeded data User and Roles for ElasticSearch associated with {IElasticClient}", nameof(IElasticClient));
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

        if (!context.Tags.Any())
        {
            var dataTag = SeedTag();
            await context.Tags.AddRangeAsync(dataTag);
            await context.SaveChangesAsync();
            var result = mapper.Map<IEnumerable<TagDto>>(dataTag);
            logger.Information("Seeded data tags for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticTags).IndexMany(result));
            logger.Information("Seeded data Tags for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }

        if (!context.Competitions.Any())
        {
            var dataCompetitions = SeedCompetition();
            await context.Competitions.AddRangeAsync(dataCompetitions);
            await context.SaveChangesAsync();
            var result = mapper.Map<IEnumerable<CompetitionDto>>(dataCompetitions);
            logger.Information("Seeded data Competitions for Education DB associated with context {DbContextName}",
                nameof(AppDbContext));
            await elasticClient.BulkAsync(b => b.Index(ElasticConstant.ElasticCompetitions).IndexMany(result));
            logger.Information("Seeded data Competitions for ElasticSearch associated with {IElasticClient}",
                nameof(IElasticClient));
        }
    }

    private static void SeedDataUserRoles(AppDbContext context)
    {
        // Seed Role
        string ADMIN_ID = Guid.NewGuid().ToString();
        string USER_ID = Guid.NewGuid().ToString();
        string CONTENTMANAGER_ID = Guid.NewGuid().ToString();
        
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
            new IdentityRole
            {
                Id = CONTENTMANAGER_ID,
                Name = "ContentManager",
                NormalizedName = "CONTENTMANAGER"
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
            PhoneNumber = "1234567890",
            Image = CloudinaryConstant.Avatar,
            PublicId = CloudinaryConstant.AvatarPublicKey,
            Dob = DateOnly.FromDateTime(DateTime.Now),
            LockoutEnabled = true,
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
            PhoneNumber = "1234567890",
            Image = CloudinaryConstant.Avatar,
            PublicId = CloudinaryConstant.AvatarPublicKey,
            Dob = DateOnly.FromDateTime(DateTime.Now),
            LockoutEnabled = true,
        };
        user.PasswordHash = hasher.HashPassword(user, "User@1234");

        var contentManager = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "contentmanager",
            NormalizedUserName = "CONTENTMANAGER",
            Email = "contentmanager@gmail.com",
            FirstName = "Michael",
            LastName = "Paul",
            NormalizedEmail = "CONTENTMANAGER@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "1234567890",
            Image = CloudinaryConstant.Avatar,
            PublicId = CloudinaryConstant.AvatarPublicKey,
            Dob = DateOnly.FromDateTime(DateTime.Now),
            LockoutEnabled = true,
        };
        contentManager.PasswordHash = hasher.HashPassword(contentManager, "Manager@1234");

        context.Users.AddRange(admin, user, contentManager);

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
            },
            new IdentityUserRole<string>
            {
                RoleId = CONTENTMANAGER_ID,
                UserId = contentManager.Id
            }
        );
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
            },
            new()
            {
                Title = "Lớp 4",
                KeyWord = "lop 4",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 5",
                KeyWord = "lop 5",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 6",
                KeyWord = "lop 6",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 7",
                KeyWord = "lop 7",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 8",
                KeyWord = "lop 8",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 9",
                KeyWord = "lop 9",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 10",
                KeyWord = "lop 10",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 11",
                KeyWord = "lop 11",
                IsActive = true,
            },
            new()
            {
                Title = "Lớp 12",
                KeyWord = "lop 12",
                IsActive = true,
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
                DocumentId = 2,
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
                Index = 1,
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
                Index = 2,
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
                Index = 3,
            }
        };
    }

    private static IEnumerable<Tag> SeedTag()
    {
        return new List<Tag>()
        {
            new()
            {
                Title = "Đề thi tin",
                KeyWord = "dethitin",
                IsActive = true
            },
            new()
            {
                Title = "Hà Nội",
                KeyWord = "hanoi",
                IsActive = true
            },
            new()
            {
                Title = "Hải Phòng",
                KeyWord = "haiphong",
                IsActive = true
            },
            new()
            {
                Title = "Tin học",
                KeyWord = "tinhoc",
                IsActive = true
            },
            new()
            {
                Title = "Lớp 10",
                KeyWord = "lop10",
                IsActive = true
            },
            new()
            {
                Title = "Lớp 11",
                KeyWord = "lop11",
                IsActive = true
            },
            new()
            {
                Title = "Lớp 12",
                KeyWord = "lop12",
                IsActive = true
            },
            new()
            {
                Title = "Lớp 12",
                KeyWord = "lop12",
                IsActive = true
            },
            new()
            {
                Title = "2024",
                KeyWord = "2024",
                IsActive = true
            },
            new()
            {
                Title = "2023",
                KeyWord = "2023",
                IsActive = true
            },
            new()
            {
                Title = "2022",
                KeyWord = "2022",
                IsActive = true
            },
            new()
            {
                Title = "Đề thi thử",
                KeyWord = "dethithu",
                IsActive = true
            },
            new()
            {
                Title = "Đề thi chính thức",
                KeyWord = "dethichinhthuc",
                IsActive = true
            },
        };
    }

    private static IEnumerable<Competition> SeedCompetition()
    {
        return new List<Competition>()
        {
            new()
            {
                Title = "Cuộc thi tin học trẻ",
                Description =
                    "Cuộc thi Tin học trẻ là một sân chơi thường niên dành cho học sinh, nhằm khuyến khích và phát triển tài năng tin học trẻ. Cuộc thi bao gồm các vòng thi lý thuyết và thực hành, nơi thí sinh thể hiện khả năng lập trình, giải thuật, và xử lý các bài toán tin học. Đây là cơ hội để học sinh trau dồi kiến thức, kỹ năng công nghệ thông tin, và phát triển tư duy logic, đồng thời thúc đẩy sự đam mê học tập và nghiên cứu khoa học kỹ thuật. Các thí sinh xuất sắc có thể giành được giải thưởng và cơ hội tham gia các cuộc thi quốc gia và quốc tế.",
                IsActive = true
            },
            new()
            {
                Title = "Tốt nghiệp trung học phổ thông",
                Description =
                    "Kỳ thi Tốt nghiệp Trung học Phổ thông (THPT) là kỳ thi quan trọng ở Việt Nam, được tổ chức hàng năm dành cho học sinh lớp 12. Kỳ thi này nhằm đánh giá kết quả học tập sau 12 năm học và là cơ sở để xét tốt nghiệp THPT cũng như xét tuyển vào các trường đại học, cao đẳng. Trong kỳ thi này, các thí sinh phải tham gia các môn thi bắt buộc và tự chọn, trong đó có môn Tin học",
                IsActive = true
            },
            new()
            {
                Title = "HKICO Việt Nam",
                Description =
                    "Kỳ thi Olympic Tin học Quốc tế, được tổ chức thường niên bởi Trung tâm Giáo dục Vô địch Olympic Hong Kong (Olympiad Champion Education Centre from Hong Kong), nhằm truyền cảm hứng khám phá các ngôn ngữ lập trình, hướng tới phát triển các kĩ năng giải quyết vấn đề và các khái niệm Khoa học tin học. Đây là cuộc thi về lập trình dành cho các bạn từ lớp 2 đến lớp 12 với các bộ môn SCRATCH, BLOCKLY, PYTHON, JAVA, C++.  Đề thi sử dụng Tiếng Anh, được phân loại theo ngôn ngữ lập trình phù hợp với độ tuổi của học sinh. Theo đó, học sinh khối 2, 3, 4 sẽ thi ngôn ngữ lập trình SCRATCH; khối 5, 6, 7 – thi BLOCKLY; từ khối 8 đến 12 được chọn thi PYTHON, hoặc C++, hoặc JAVA.",
                IsActive = true
            },
            new()
            {
                Title = "Kỳ thi tuyển sinh lớp 10",
                Description =
                    "Kỳ thi tuyển sinh lớp 10 là một kỳ thi quan trọng dành cho học sinh lớp 9 tại Việt Nam, nhằm đánh giá và tuyển chọn học sinh vào các trường trung học phổ thông (THPT). Đây là bước chuyển quan trọng trong quá trình học tập của học sinh, đánh dấu sự kết thúc của bậc trung học cơ sở (THCS) và bắt đầu hành trình THPT.",
                IsActive = true
            },
        };
    }
}