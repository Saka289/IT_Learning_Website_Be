using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.QuizAnswerRepositories;

public interface IQuizAnswerRepository : IRepositoryBase<QuizAnswer, int>
{
    Task<IEnumerable<QuizAnswer>> GetAllQuizAnswerByQuizQuestionId(int id);
    Task<QuizAnswer?> GetQuizAnswerByQuizQuestionId(int id);
    Task<QuizAnswer?> GetQuizAnswerById(int id);
    Task<QuizAnswer> CreateQuizAnswer(QuizAnswer quizAnswer);
    Task<bool> CreateRangeQuizAnswer(IEnumerable<QuizAnswer> quizAnswers);
    Task<QuizAnswer> UpdateQuizAnswer(QuizAnswer quizAnswer);
    Task<bool> UpdateRangeQuizAnswer(IEnumerable<QuizAnswer> quizAnswers);
    Task<bool> DeleteQuizAnswer(int id);
}