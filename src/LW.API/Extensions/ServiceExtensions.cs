﻿using System.Text;
using LW.API.Mappings;
using LW.Cache;
using LW.Cache.Interfaces;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Data.Repositories.CommentDocumentRepositories;
using LW.Data.Repositories.CompetitionRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Infrastructure.Common;
using LW.Infrastructure.Configurations;
using LW.Infrastructure.Extensions;
using LW.Services.AdminServices;
using LW.Infrastructure.Services;
using LW.Services.GradeServices;
using LW.Services.UserServices;
using LW.Shared.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MySqlConnector;
using LW.Services.DocumentServices;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.EditorialRepositories;
using LW.Data.Repositories.ExamAnswerRepositories;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Data.Repositories.FavoritePostRepositories;
using LW.Data.Repositories.ExecuteCodeRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.LevelRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.ProgramLanguageRepositories;
using LW.Data.Repositories.NotificationRepositories;
using LW.Data.Repositories.PostCommentRepositories;
using LW.Data.Repositories.PostRepositories;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRelationRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Data.Repositories.SubmissionRepositories;
using LW.Data.Repositories.TagRepositories;
using LW.Data.Repositories.TestCaseRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Data.Repositories.UserExamRepositories;
using LW.Data.Repositories.UserGradeRepositories;
using LW.Data.Repositories.UserQuizRepositories;
using LW.Data.Repositories.VoteCommentRepositories;
using LW.Infrastructure.Hubs;
using LW.Services.CommentDocumentServices;
using LW.Services.Common;
using LW.Services.Common.CommonServices.CompileServices;
using LW.Services.Common.CommonServices.FacebookServices;
using LW.Services.Common.CommonServices.JwtTokenServices;
using LW.Services.NotificationServices;
using LW.Services.CompetitionServices;
using LW.Services.EditorialServices;
using LW.Services.EnumServices;
using LW.Services.ExamAnswerServices;
using LW.Services.ExamCodeServices;
using LW.Services.ExamServices;
using LW.Services.ExecuteCodeServices;
using LW.Services.IndexServices;
using LW.Services.LessonServices;
using LW.Services.LevelServices;
using LW.Services.ProblemServices;
using LW.Services.ProgramLanguageServices;
using LW.Services.PostCommentServices;
using LW.Services.PostServices;
using LW.Services.QuizQuestionRelationServices;
using LW.Services.QuizQuestionServices;
using LW.Services.QuizServices;
using LW.Services.SolutionServices;
using LW.Services.StatisticServices;
using LW.Services.SubmissionServices;
using LW.Services.TagServices;
using LW.Services.TestCaseServices;
using LW.Services.TopicServices;
using LW.Services.UserExamServices;
using LW.Services.UserGradeServices;
using LW.Services.UserQuizServices;
using Nest;
using StackExchange.Redis;

namespace LW.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting)).Get<SMTPEmailSetting>();
        services.AddSingleton(emailSettings);

        var cacheSettings = configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
        services.AddSingleton(cacheSettings);

        var elasticSettings = configuration.GetSection(nameof(ElasticsearchSettings)).Get<ElasticsearchSettings>();
        services.AddSingleton(elasticSettings);

        var googleSettings = services.Configure<GoogleSettings>(configuration.GetSection(nameof(GoogleSettings)));
        services.AddSingleton(googleSettings);

        var facebookSettings = services.Configure<FacebookSettings>(configuration.GetSection(nameof(FacebookSettings)));
        services.AddSingleton(facebookSettings);

        var jwtSettings = services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.AddSingleton(jwtSettings);

        var confirmEmailSettings =
            services.Configure<VerifyEmailSettings>(configuration.GetSection(nameof(VerifyEmailSettings)));
        services.AddSingleton(confirmEmailSettings);

        var cloudinarySettings =
            services.Configure<CloudinarySettings>(configuration.GetSection(nameof(CloudinarySettings)));
        services.AddSingleton(cloudinarySettings);

        var urlBaseSettings = services.Configure<UrlBase>(configuration.GetSection(nameof(UrlBase)));
        services.AddSingleton(urlBaseSettings);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.
        services.AddControllers();
        services.AddSignalR();
        services.AddHttpContextAccessor();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

        //Add services HttpClient 
        services.AddHttpClient("Facebook",
            facebookOptions =>
            {
                facebookOptions.BaseAddress = new Uri(configuration.GetValue<string>("FacebookSettings:BaseUrl"));
            });
        services.AddHttpClient("SendAPI");

        services.ConfigureSwagger();
        services.ConfigureAppDbContext(configuration);
        services.ConfigureRedis(configuration);
        services.ConfigureElasticSearch();
        services.AddInfrastructureServices();
        services.ConfiguringCors();
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        return services;
    }

    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {
        var settingsJwt = builder.Services.GetOptions<JwtSettings>(nameof(JwtSettings));
        var settingsGoogle = builder.Services.GetOptions<GoogleSettings>(nameof(GoogleSettings));
        var settingsFacebook = builder.Services.GetOptions<FacebookSettings>(nameof(FacebookSettings));

        var key = Encoding.ASCII.GetBytes(settingsJwt.Secret);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = settingsJwt.Issuer,
                ValidAudience = settingsJwt.Audience,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,
            };
        }).AddGoogle(x =>
        {
            x.ClientId = settingsGoogle.ClientId;
            x.ClientSecret = settingsGoogle.ClientSecret;
        }).AddFacebook(x =>
        {
            x.AppId = settingsFacebook.AppId;
            x.AppSecret = settingsFacebook.AppSecret;
        });

        return builder;
    }

    private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
                securityScheme: new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new List<string>()
                }
            });
        });
        return services;
    }

    private static IServiceCollection ConfigureAppDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        var builder = new MySqlConnection(connectionString);

        services.AddDbContext<AppDbContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString), e =>
            {
                e.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                e.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));

        return services;
    }

    private static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetOptions<CacheSettings>("CacheSettings");
        if (string.IsNullOrEmpty(settings.ConnectionString))
        {
            throw new ArgumentNullException("Redis Connection string is not configured.");
        }
        
        var configurationOptions = ConfigurationOptions.Parse(settings.ConnectionString);
        configurationOptions.Password = settings.Password;
        
        var lazyConnection = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));

        services.AddSingleton(lc => lazyConnection.Value);
        
        services.AddStackExchangeRedisCache(options => { options.ConfigurationOptions = configurationOptions; });
        
        return services;
    }

    private static IServiceCollection ConfigureElasticSearch(this IServiceCollection service)
    {
        var settings = service.GetOptions<ElasticsearchSettings>(nameof(ElasticsearchSettings));
        var settingsElasticSearch = new ConnectionSettings(new Uri(settings.Uri))
            .BasicAuthentication(settings.Username, settings.Password)
            .PrettyJson()
            .DefaultIndex(settings.DefaultIndex);
        MappingElastic.AddDefaultMappings(settingsElasticSearch);
        var client = new ElasticClient(settingsElasticSearch);
        service.AddSingleton<IElasticClient>(client);
        return service;
    }

    private static IServiceCollection ConfiguringCors(this IServiceCollection service)
    {
        var settings = service.GetOptions<UrlBase>(nameof(UrlBase));
        service.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(settings.ClientUrl).AllowAnyHeader().AllowAnyMethod()
                    .WithExposedHeaders("X-Pagination");
            });
        });
        return service;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // IBaseRepository 
        services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
        services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryQueryBase<,>));
        services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));
        services.AddScoped(typeof(IElasticSearchService<,>), typeof(ElasticSearchService<,>));
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddTransient(typeof(IRedisCache<>), typeof(RedisCache<>));
        services.AddTransient<ISerializeService, SerializeService>();
        // IRepository 
        services.AddScoped<ILevelRepository, LevelRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<ICommentDocumentRepository, CommentDocumentRepository>();
        services.AddScoped<IUserGradeRepository, UserGradeRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IExamAnswerRepository, ExamAnswerRepository>();
        services.AddScoped<IUserExamRepository, UserExamRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuizQuestionRepository, QuizQuestionRepository>();
        services.AddScoped<IQuizAnswerRepository, QuizAnswerRepository>();
        services.AddScoped<IUserQuizRepository, UserQuizRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IQuizQuestionRelationRepository, QuizQuestionRelationRepository>();
        services.AddScoped<IExamCodeRepository, ExamCodeRepository>();
        services.AddScoped<ICompetitionRepository, CompetitionRepository>();
        services.AddScoped<IProgramLanguageRepository, ProgramLanguageRepository>();
        services.AddScoped<ISolutionRepository, SolutionRepository>();
        services.AddScoped<IProblemRepository, ProblemRepository>();
        services.AddScoped<IEditorialRepository, EditorialRepository>();
        services.AddScoped<ITestCaseRepository, TestCaseRepository>();
        services.AddScoped<IExecuteCodeRepository, ExecuteCodeRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostCommentRepository, PostCommentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IVoteCommentRepository, VoteCommentRepository>();
        services.AddScoped<IFavoritePostRepository, FavoritePostRepository>();
        // IService 
        services.AddScoped<IAdminAuthorService, AdminAuthorService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILevelService, LevelService>();
        services.AddScoped<IGradeService, GradeService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IFacebookService, FacebookService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ICommentDocumentService, CommentDocumentService>();
        services.AddScoped<IIndexService, IndexService>();
        services.AddScoped<IEnumService, EnumService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IExamAnswerService, ExamAnswerService>();
        services.AddScoped<IUserExamService, UserExamService>();
        services.AddScoped<IUserGradeService, UserGradeService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IQuizQuestionService, QuizQuestionService>();
        services.AddScoped<IUserQuizService, UserQuizService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IQuizQuestionRelationService, QuizQuestionRelationService>();
        services.AddScoped<IExamCodeService, ExamCodeService>();
        services.AddScoped<ICompetitionService, CompetitionService>();
        services.AddScoped<IProgramLanguageService, ProgramLanguageService>();
        services.AddScoped<IProgramLanguageService, ProgramLanguageService>();
        services.AddScoped<ISolutionService, SolutionService>();
        services.AddScoped<IProblemService, ProblemService>();
        services.AddScoped<IEditorialService, EditorialService>();
        services.AddScoped<ITestCaseService, TestCaseService>();
        services.AddScoped<IExecuteCodeService, ExecuteCodeService>();
        services.AddScoped<ICompileService, CompileService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostCommentService, PostCommentService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IStatisticService, StatisticService>();
        return services;
    }
}