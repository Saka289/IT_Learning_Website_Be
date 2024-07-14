using LW.Data.Entities;
using LW.Shared.DTOs.UserQuiz;
using LW.Shared.SeedWork;

namespace LW.Services.UserQuizServices;

public interface IUserQuizService
{
     Task<ApiResult<UserQuizDto>> SubmitQuiz(UserQuizSubmitDto userQuizSubmitDto);
     Task<ApiResult<IEnumerable<UserQuizDto>>> GetAllUserQuizByUserId(string userId);
     Task<ApiResult<bool>> DeleteUserQuizById(int id);
}