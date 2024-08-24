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

    public async Task<IEnumerable<Grade>> GetAllGrade(bool isInclude = false)
    {
        if (!isInclude)
        {
            return await FindAll().ToListAsync();
        }
        return await FindAll()
            .Include(g => g.Exams.Where(e => e.IsActive))
            .Include(g => g.QuizzesCustom.Where(q => q.IsActive))
            .Include(g => g.ProblemsCustom.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(c => c.CommentDocuments)
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Quizzes.Where(q => q.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Quizzes.Where(q => q.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Quizzes.Where(q => q.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Quizzes.Where(q => q.IsActive))
            .ToListAsync();
    }

    public async Task<Grade?> GetGradeById(int id, bool isInclude)
    {
        if (!isInclude)
        {
            return await GetByIdAsync(id);
        }
        return await FindAll()
            .Include(g => g.Exams.Where(e => e.IsActive))
            .Include(g => g.QuizzesCustom.Where(q => q.IsActive))
            .Include(g => g.ProblemsCustom.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(c => c.CommentDocuments)
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Quizzes.Where(q => q.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Quizzes.Where(q => q.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Quizzes.Where(q => q.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Problems.Where(p => p.IsActive))
            .Include(g => g.Documents.Where(d => d.IsActive))
            .ThenInclude(d => d.Topics.Where(t => t.ParentId == null && t.IsActive))
            .ThenInclude(t => t.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(ct => ct.Lessons.Where(l => l.IsActive))
            .ThenInclude(l => l.Quizzes.Where(q => q.IsActive))
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

    public async Task<IEnumerable<Grade>> GetAllGradeByLevelId(int LevelId)
    {
        return await FindByCondition(x => x.LevelId== LevelId).ToListAsync();
    }
}