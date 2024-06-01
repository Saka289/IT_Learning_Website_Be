using System.Text;
using LW.Cache;
using LW.Cache.Interfaces;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.LevelRepositories;
using LW.Infrastructure.Common;
using LW.Infrastructure.Configurations;
using LW.Infrastructure.Extensions;
using LW.Services.AdminServices;
using LW.Infrastructure.Services;
using LW.Services.FacebookService;
using LW.Services.GradeService;
using LW.Services.JwtTokenService;
using LW.Services.LevelServices;
using LW.Services.UserService;
using LW.Shared.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MySqlConnector;

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

        var googleSettings = services.Configure<GoogleSettings>(configuration.GetSection(nameof(GoogleSettings)));
        services.AddSingleton(googleSettings);

        var facebookSettings = services.Configure<FacebookSettings>(configuration.GetSection(nameof(FacebookSettings)));
        services.AddSingleton(facebookSettings);

        var jwtSettings = services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.AddSingleton(jwtSettings);

        var confirmEmailSettings = services.Configure<VerifyEmailSettings>(configuration.GetSection(nameof(VerifyEmailSettings)));
        services.AddSingleton(confirmEmailSettings);

        var urlBaseSettings = services.Configure<UrlBase>(configuration.GetSection(nameof(UrlBase)));
        services.AddSingleton(urlBaseSettings);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.SignIn.RequireConfirmedEmail = true; })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();
        //Add services HttpClient 
        services.AddHttpClient("Facebook",
            facebookOptions =>
            {
                facebookOptions.BaseAddress = new Uri(configuration.GetValue<string>("FacebookSettings:BaseUrl"));
            });

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
        services.ConfigureAppDbContext(configuration);
        services.ConfigureRedis(configuration);
        services.AddInfrastructureServices();
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

        services.AddStackExchangeRedisCache(options => { options.Configuration = settings.ConnectionString; });
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
        services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryQueryBase<,>));
        services.AddScoped<ILevelRepository, LevelRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();
        services.AddScoped<IAdminAuthorService, AdminAuthorService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILevelService, LevelService>();
        services.AddScoped<IGradeService, GradeService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IFacebookService, FacebookService>();
        services.AddTransient<ISerializeService, SerializeService>();
        services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));
        services.AddTransient(typeof(IRedisCache<>), typeof(RedisCache<>));
        return services;
    }
}