using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Enum;
using LW.Shared.SeedWork;

namespace LW.Services.EnumServices;

public interface IEnumService
{
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllBookCollection();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllBookType();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllTypeQuestion();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllLevelQuestion();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllQuizType();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllTypeOfExam();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllStatusSubmission();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllStatusProblem();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllTypeDifficulty();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllLevel();
}