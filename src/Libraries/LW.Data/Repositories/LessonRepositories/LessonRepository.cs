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
        return await FindAll().Include(t => t.Topic).Where(x => x.TopicId == id).ToListAsync();
    }

    public async Task<Lesson?> GetLessonById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
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
        var lesson = await GetByIdAsync(id);
        if (lesson == null)
        {
            return false;
        }

        await DeleteAsync(lesson);
        return true;
    }

    public async Task<bool> DeleteRangeLesson(IEnumerable<int> ids)
    {
        var listLesson = await FindAll().Where(lesson => ids.Contains(lesson.Id)).ToListAsync();
        if (!listLesson.Any())
        {
            return false;
        }

        await DeleteListAsync(listLesson);
        return true;
    }

    public async Task<Lesson> GetAllLessonIndex(int id)
    {
        return await FindAll()
            .Include(t => t.Topic)
            .ThenInclude(c => c.ParentTopic)
            .ThenInclude(l => l.Lessons.Where(l => l.IsActive == true))
            .Where(t => t.Topic.IsActive)
            .Include(t => t.Topic)
            .ThenInclude(d => d.Document)
            .Where(t => t.Topic.Document.IsActive)
            .FirstOrDefaultAsync(l => l.Id == id && l.IsActive);
    }
}