using LW.Shared.DTOs;
using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.SeedWork;

namespace LW.Services.ExamAnswerServices;

public interface IExamAnswerService
{
    Task<ApiResult<IEnumerable<ExamAnswerDto>>> GetAllExamAnswer();
    Task<ApiResult<ExamAnswerDto>> GetExamAnswerById(int id);
    Task<ApiResult<IEnumerable<ExamAnswerDto>>> GetExamAnswerByExamId(int examId);
    Task<ApiResult<ExamAnswerDto>> CreateExamAnswer(ExamAnswerCreateDto examAnswerCreateDto);
    
    Task<ApiResult<bool>> CreateRangeExamAnswer(IEnumerable<ExamAnswerCreateDto> examAnswerCreateDtos);
    Task<ApiResult<ExamAnswerDto>> UpdateExamAnswer(ExamAnswerUpdateDto examAnswerUpdateDto);
    Task<ApiResult<bool>> DeleteExamAnswer(int id);
}