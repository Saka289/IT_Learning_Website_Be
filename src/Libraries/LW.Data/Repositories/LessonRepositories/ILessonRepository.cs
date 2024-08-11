using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.LessonRepositories;

public interface ILessonRepository : IRepositoryBase<Lesson, int>
{
    Task<IEnumerable<Lesson>> GetAllLesson();
    Task<IEnumerable<Lesson>> GetAllLessonByTopic(int id);
    Task<Lesson?> GetLessonById(int id);
    Task<Lesson> CreateLesson(Lesson lesson);
    Task<Lesson> UpdateLesson(Lesson lesson);
    Task<bool> UpdateRangeLesson(IEnumerable<Lesson> lessons);
    Task<bool> DeleteLesson(int id);
    Task<bool> DeleteRangeLesson(IEnumerable<int> ids);
    Task<Lesson> GetAllLessonIndex(int id);
    Task<IEnumerable<Lesson>> SearchLessonByTag(string tag, bool order);
}