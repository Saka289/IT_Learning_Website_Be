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
        return await FindAll().ToListAsync();
    }

    public Task<IQueryable<Lesson>> GetAllLessonPagination()
    {
        var result = FindAll();
        return Task.FromResult(result);
    }

    public async Task<Lesson> GetLessonById(int id)
    {
        return await GetByIdAsync(id);
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
}