using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Shared.SeedWork;

namespace LW.Data.Repositories.QuizRepositories;

public interface IQuizRepository : IRepositoryBase<Quiz, int>
{
    Task<IEnumerable<Quiz>> GetAllQuiz();
    Task<IEnumerable<Quiz>> GetAllQuizPagination();
    Task<IQueryable<Quiz>> GetAllQuizByTopicIdPagination(int topicId);
    Task<IQueryable<Quiz>> GetAllQuizByLessonIdPagination(int lessonId);
    Task<Quiz?> GetQuizById(int id);
    Task<Quiz> CreateQuiz(Quiz quiz);
    Task<Quiz> UpdateQuiz(Quiz quiz);
    Task<bool> DeleteQuiz(int id);
}