using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Shared.SeedWork;

namespace LW.Data.Repositories.QuizRepositories;

public interface IQuizRepository : IRepositoryBase<Quiz, int>
{
    Task<IEnumerable<Quiz>> GetAllQuiz();
    Task<IEnumerable<Quiz>> GetAllQuizCustom();
    Task<IEnumerable<Quiz>> GetAllQuizByTopicId(int topicId, bool isInclude = false);
    Task<IEnumerable<Quiz>> GetAllQuizByLessonId(int lessonId, bool isInclude = false);
    Task<IEnumerable<Quiz>> SearchQuizByTag(string tag, bool order);
    Task<Quiz?> GetQuizById(int id);
    Task<Quiz> CreateQuiz(Quiz quiz);
    Task<Quiz> UpdateQuiz(Quiz quiz);
    Task<bool> DeleteQuiz(int id);
    Task<bool> DeleteRangeQuiz(IEnumerable<Quiz> quizzes);
}