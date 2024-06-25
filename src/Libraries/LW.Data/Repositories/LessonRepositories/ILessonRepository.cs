using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.LessonRepositories;

public interface ILessonRepository : IRepositoryBase<Lesson, int>
{
    Task<IEnumerable<Lesson>> GetAllLesson();
    Task<IEnumerable<Lesson>> GetAllLessonByTopic(int id);
    Task<IQueryable<Lesson>> GetAllLessonPagination();
    Task<Lesson> GetLessonById(int id);
    Task<Lesson> CreateLesson(Lesson lesson);
    Task<Lesson> UpdateLesson(Lesson lesson);
    Task<bool> DeleteLesson(int id);
    Task<bool> DeleteRangeLesson(IEnumerable<Lesson> lessons);
}