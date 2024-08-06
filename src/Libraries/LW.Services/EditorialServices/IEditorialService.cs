using LW.Shared.DTOs.Editorial;
using LW.Shared.SeedWork;

namespace LW.Services.EditorialServices;

public interface IEditorialService
{
    Task<ApiResult<IEnumerable<EditorialDto>>> GetAllEditorial();
    Task<ApiResult<EditorialDto>> GetEditorialByProblemId(int problemId);
    Task<ApiResult<EditorialDto>> GetEditorialById(int id);
    Task<ApiResult<EditorialDto>> CreateEditorial(EditorialCreateDto editorialCreateDto);
    Task<ApiResult<EditorialDto>> UpdateEditorial(EditorialUpdateDto editorialUpdateDto);
    Task<ApiResult<bool>> DeleteEditorial(int id);
}