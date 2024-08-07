using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.GradeRepositories;

public class GradeRepository : RepositoryBase<Grade, int>, IGradeRepository
{
    public GradeRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Grade>> GetAllGrade()
    {
        return await FindAll()
            .Include(g => g.Exams)
            .Include(g => g.Documents)
            .ThenInclude(c => c.CommentDocuments)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Lessons)
            .ThenInclude(l => l.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Lessons)
            .ThenInclude(l => l.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Lessons)
            .ThenInclude(l => l.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Lessons)
            .ThenInclude(l => l.Quizzes)
            .ToListAsync();
    }

    public async Task<IEnumerable<Grade>> GetAllGradePagination()
    {
        return await FindAll()
            .Include(g => g.Exams)
            .Include(g => g.Documents)
            .ThenInclude(c => c.CommentDocuments)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Lessons)
            .ThenInclude(l => l.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Lessons)
            .ThenInclude(l => l.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Lessons)
            .ThenInclude(l => l.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Lessons)
            .ThenInclude(l => l.Quizzes)
            .ToListAsync();
    }

    public async Task<Grade?> GetGradeById(int id, bool isInclude)
    {
        if (!isInclude)
        {
            return await GetByIdAsync(id);
        }
        return await FindAll()
            .Include(g => g.Exams)
            .Include(g => g.Documents)
            .ThenInclude(c => c.CommentDocuments)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Lessons)
            .ThenInclude(l => l.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.Lessons)
            .ThenInclude(l => l.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Quizzes)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Lessons)
            .ThenInclude(l => l.Problems)
            .Include(g => g.Documents)
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null))
            .ThenInclude(t => t.ChildTopics)
            .ThenInclude(ct => ct.Lessons)
            .ThenInclude(l => l.Quizzes)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<Grade> CreateGrade(Grade grade)
    {
        Create(grade);
        return Task.FromResult(grade);
    }

    public Task<Grade> UpdateGrade(Grade grade)
    {
        Update(grade);
        return Task.FromResult(grade);
    }

    public async Task<bool> DeleteGrade(int id)
    {
        var grade = await GetByIdAsync(id);
        if (grade == null)
        {
            return false;
        }

        await DeleteAsync(grade);
        return true;
    }
}