using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.QuizQuestionRepositories;

public interface IQuizQuestionRepository : IRepositoryBase<QuizQuestion, int>
{
    Task<IEnumerable<QuizQuestion>> GetAllQuizQuestion();
    Task<IQueryable<QuizQuestion>> GetAllQuizQuestionPagination();
    Task<IQueryable<QuizQuestion>> GetAllQuizQuestionByQuizId(int id);
    Task<QuizQuestion?> GetQuizQuestionById(int id);
    Task<QuizQuestion> CreateQuizQuestion(QuizQuestion quizQuestion);
    Task<IEnumerable<QuizQuestion>> CreateRangeQuizQuestion(IEnumerable<QuizQuestion> quizQuestions);
    Task<QuizQuestion> UpdateQuizQuestion(QuizQuestion quizQuestion);
    Task<bool> UpdateRangeQuizQuestion(IEnumerable<QuizQuestion> quizQuestions);
    Task<bool> DeleteQuizQuestion(int id);
}