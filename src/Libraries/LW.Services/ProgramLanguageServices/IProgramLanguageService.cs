using CloudinaryDotNet;
using LW.Shared.DTOs.ProgramLanguage;
using LW.Shared.SeedWork;

namespace LW.Services.ProgramLanguageServices;



public interface IProgramLanguageService
{
    Task<ApiResult<IEnumerable<ProgramLanguageDto>>> GetAllProgramLanguage();
    Task<ApiResult<ProgramLanguageDto>> GetProgramLanguageById(int id);
    Task<ApiResult<ProgramLanguageDto>> CreateProgramLanguage(ProgramLanguageCreateDto programLanguageCreateDto);
    Task<ApiResult<ProgramLanguageDto>> UpdateProgramLanguage(ProgramLanguageUpdateDto programLanguageUpdateDto);
    Task<ApiResult<bool>> UpdateStatusProgramLanguage(int id);
    Task<ApiResult<bool>> DeleteProgramLanguage(int id);
}