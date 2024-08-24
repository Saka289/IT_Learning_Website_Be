using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
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

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor contextAccessor) :
        base(options)
    {
        _contextAccessor = contextAccessor;
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Level> Levels { get; set; }

    public DbSet<Grade> Grades { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<CommentDocument> CommentDocuments { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamAnswer> ExamAnswers { get; set; }
    public DbSet<UserExam> UserExams { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<QuizQuestionRelation> QuizQuestionRelations { get; set; }
    public DbSet<QuizAnswer> QuizAnswers { get; set; }
    public DbSet<UserQuiz> UserQuizzes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ExamCode> ExamCodes { get; set; }
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Solution> Solutions { get; set; }
    public DbSet<ExecuteCode> ExecuteCodes { get; set; }
    public DbSet<ProgramLanguage> ProgramLanguages { get; set; }
    public DbSet<TestCase> TestCases { get; set; }
    public DbSet<Editorial> Editorials { get; set; }

    public DbSet<Post> Posts { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<VoteComment> VoteComments { get; set; }
    public DbSet<FavoritePost> FavoritePosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        RemoveAspNet(modelBuilder);
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
                            string familyName = token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.FamilyName)?.Value ?? string.Empty;
                            string givenName = token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.GivenName)?.Value ?? string.Empty;
                            addedUserEntity.LastModifiedBy = $"{familyName} {givenName}";
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
                            string familyName = token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.FamilyName)?.Value ?? string.Empty;
                            string givenName = token.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.GivenName)?.Value ?? string.Empty;
                            modifiedUserEntity.LastModifiedBy = $"{familyName} {givenName}";
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