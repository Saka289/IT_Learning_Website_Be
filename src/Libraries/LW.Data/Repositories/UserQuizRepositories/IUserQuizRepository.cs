using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.UserQuizRepositories;

public interface IUserQuizRepository : IRepositoryBase<UserQuiz, int>
{
    Task<IEnumerable<UserQuiz>> GetAllUserQuizByUserId(string userId);
    Task<IEnumerable<UserQuiz>> GetAllUserQuizByQuizId(int quizId);
    Task<UserQuiz?> GetUserQuizById(int id);
    Task<UserQuiz> CreateUserQuiz(UserQuiz userQuiz);
    Task<bool> CreateRangeQuiz(IEnumerable<UserQuiz> userQuizzes);
    Task<UserQuiz> UpdateUserQuiz(UserQuiz userQuiz);
    Task<bool> UpdateRangeUserQuiz(IEnumerable<UserQuiz> userQuizzes);
    Task<bool> DeleteUserQuiz(int id);
    Task<bool> DeleteRangeUserQuiz(IEnumerable<UserQuiz> userQuizzes);
}