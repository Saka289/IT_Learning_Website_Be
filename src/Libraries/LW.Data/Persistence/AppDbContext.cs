using System.IdentityModel.Tokens.Jwt;
using LW.Contracts.Domains.Interfaces;
using LW.Data.Entities;
using LW.Shared.Constant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace LW.Data.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger _logger;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor contextAccessor, ILogger logger) :
        base(options)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Lesson> Lessons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        RemoveAspNet(modelBuilder);
        SeedDataUserRoles(modelBuilder);
    }

    private static void RemoveAspNet(ModelBuilder modelBuilder)
    {
        // xóa bỏ các tiền tố AspNet trong các bảng Identity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }
    }

    private static void SeedDataUserRoles(ModelBuilder modelBuilder)
    {
        //Seed Role
        string ADMIN_ID = Guid.NewGuid().ToString();
        string USER_ID = Guid.NewGuid().ToString();

        modelBuilder.Entity<IdentityRole>().HasData(new List<IdentityRole>
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

        //Seed User and Admin
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
        admin.PasswordHash = hasher.HashPassword(admin, "Password123!");

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
        user.PasswordHash = hasher.HashPassword(user, "Password123!");
        modelBuilder.Entity<ApplicationUser>().HasData(admin, user);

        //Seed UserRole
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var accessToken = string.Empty;
        var modified = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified ||
                        e.State == EntityState.Added ||
                        e.State == EntityState.Deleted);

        if (_contextAccessor.HttpContext is not null)
        {
            accessToken = _contextAccessor.HttpContext.GetTokenAsync("access_token").GetAwaiter().GetResult();
        }

        foreach (var item in modified)
        {
            switch (item.State)
            {
                case EntityState.Added:
                    if (item.Entity is IDateTracking addedDateEntity)
                    {
                        addedDateEntity.CreatedDate = DateTime.UtcNow;
                        item.State = EntityState.Added;
                    }

                    if (item.Entity is IUserTracking addedUserEntity)
                    {
                        if (!accessToken.IsNullOrEmpty())
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var token = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
                            addedUserEntity.CreatedBy = $"{token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.FamilyName).Value} {token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.GivenName).Value}";
                            item.State = EntityState.Added;
                        }
                        else
                        {
                            addedUserEntity.CreatedBy = RoleConstant.RoleAdmin;
                            item.State = EntityState.Added; 
                        }
                    }

                    break;

                case EntityState.Modified:
                    Entry(item.Entity).Property("Id").IsModified = false;
                    if (item.Entity is IDateTracking modifiedDateEntity)
                    {
                        modifiedDateEntity.LastModifiedDate = DateTime.UtcNow;
                        item.State = EntityState.Modified;
                    }

                    if (item.Entity is IUserTracking modifiedUserEntity)
                    {
                        if (!accessToken.IsNullOrEmpty())
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var token = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
                            modifiedUserEntity.LastModifiedBy = $"{token.Claims.FirstOrDefault(u => u.Type == "firstname").Value} {token.Claims.FirstOrDefault(u => u.Type == "lastname").Value}";
                            item.State = EntityState.Modified;
                        }
                        else
                        {
                            modifiedUserEntity.LastModifiedBy = RoleConstant.RoleAdmin;
                            item.State = EntityState.Modified;
                        }
                    }

                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}