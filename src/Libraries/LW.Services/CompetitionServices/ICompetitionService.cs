using LW.Shared.DTOs.Competition;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;

namespace LW.Services.CompetitionServices;

public interface ICompetitionService
{
    Task<ApiResult<IEnumerable<CompetitionDto>>> GetAllCompetition(bool? status);
    Task<ApiResult<PagedList<CompetitionDto>>> GetAllCompetitionPagination(SearchCompetitionDto searchCompetitionDto);
    Task<ApiResult<CompetitionDto>> GetCompetitionById(int id);
    Task<ApiResult<CompetitionDto>> CreateCompetition(CompetitionCreateDto competitionCreateDto);
    Task<ApiResult<CompetitionDto>> UpdateCompetition(CompetitionUpdateDto competitionUpdateDto);
    Task<ApiResult<bool>> UpdateStatusCompetition(int id);
    Task<ApiResult<bool>> DeleteCompetition(int id);
}