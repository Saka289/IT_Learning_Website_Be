using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.LessonRepositories;

public interface ILessonRepository : IRepositoryBase<Lesson, int>
{
    Task<IEnumerable<Lesson>> GetAllLesson();
    Task<IQueryable<Lesson>> GetAllLessonPagination();
    Task<Lesson> GetLessonById(int id);
    Task<Lesson> CreateLesson(Lesson lesson);
    Task<Lesson> UpdateLesson(Lesson lesson);
    Task<bool> DeleteLesson(int id);
}