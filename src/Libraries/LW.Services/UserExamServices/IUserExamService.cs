using LW.Shared.DTOs.Topic;
using LW.Shared.DTOs.UserExam;
using LW.Shared.SeedWork;

namespace LW.Services.UserExamServices;

public interface IUserExamService
{
    public Task<ApiResult<bool>> CreateRangeUserExam(ExamFormSubmitDto examFormSubmitDto);
    public Task<ApiResult<UserExamDto>> GetExamResultById(int  id);
    public Task<ApiResult<IEnumerable<UserExamDto>>> GetListResultByUserId(string userId);
    
    

}