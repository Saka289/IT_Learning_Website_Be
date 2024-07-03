using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.LessonRepositories;

public class LessonRepository : RepositoryBase<Lesson, int>, ILessonRepository
{
    public LessonRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Lesson>> GetAllLesson()
    {
        return await FindAll().Include(t => t.Topic).ToListAsync();
    }

    public async Task<IEnumerable<Lesson>> GetAllLessonByTopic(int id)
    {
        return await FindAll(true).Include(t => t.Topic).Where(x => x.TopicId == id).ToListAsync();
    }

    public Task<IQueryable<Lesson>> GetAllLessonPagination()
    {
        var result = FindAll();
        return Task.FromResult(result);
    }

    public async Task<Lesson> GetLessonById(int id)
    {
        return await FindByCondition(x => x.Id == id).Include(t => t.Topic).FirstOrDefaultAsync();
    }

    public Task<Lesson> CreateLesson(Lesson lesson)
    {
        Create(lesson);
        return Task.FromResult(lesson);
    }

    public Task<Lesson> UpdateLesson(Lesson lesson)
    {
        Update(lesson);
        return Task.FromResult(lesson);
    }

    public async Task<bool> UpdateRangeLesson(IEnumerable<Lesson> lessons)
    {
        lessons = lessons.Where(l => l != null);
        if (!lessons.Any())
        {
            return false;
        }

        await UpdateListAsync(lessons);
        return true;
    }

    public async Task<bool> DeleteLesson(int id)
    {
        var grade = await GetByIdAsync(id);
        if (grade == null)
        {
            return false;
        }

        await DeleteAsync(grade);
        return true;
    }

    public async Task<bool> DeleteRangeLesson(IEnumerable<Lesson> lessons)
    {
        lessons = lessons.Where(l => l != null);
        if (!lessons.Any())
        {
            return false;
        }

        await DeleteListAsync(lessons);
        return true;
    }

    public async Task<Lesson> GetAllLessonIndex(int id)
    {
        return await FindAll()
            .Include(t => t.Topic)
            .ThenInclude(c => c.ParentTopic)
            .ThenInclude(l => l.Lessons)
            .Include(t => t.Topic)
            .ThenInclude(d => d.Document)
            .FirstOrDefaultAsync(l => l.Id == id);
    }
}