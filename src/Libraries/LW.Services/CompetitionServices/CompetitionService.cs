using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.CompetitionRepositories;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Shared.Constant;
using LW.Shared.DTOs.Competition;
using LW.Shared.DTOs.Document;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.CompetitionServices;

public class CompetitionService : ICompetitionService
{
    private readonly IMapper _mapper;
    private readonly ICompetitionRepository _competitionRepository;
    private readonly IElasticSearchService<CompetitionDto, int> _elasticSearchService;
    private readonly IExamRepository _examRepository;
    private readonly IExamCodeRepository _examCodeRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public CompetitionService(IMapper mapper, ICompetitionRepository competitionRepository,
        IElasticSearchService<CompetitionDto, int> elasticSearchService, IExamRepository examRepository, IExamCodeRepository examCodeRepository, ICloudinaryService cloudinaryService)
    {
        _mapper = mapper;
        _competitionRepository = competitionRepository;
        _elasticSearchService = elasticSearchService;
        _examRepository = examRepository;
        _examCodeRepository = examCodeRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResult<IEnumerable<CompetitionDto>>> GetAllCompetition(bool? status)
    {
        var competitionList = await _competitionRepository.GetAllCompetition();
        if (!competitionList.Any())
        {
            return new ApiResult<IEnumerable<CompetitionDto>>(false, "Competitions is null !!!");
        }

        if (status != null)
        {
            competitionList = competitionList.Where(c => c.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<CompetitionDto>>(competitionList);
        return new ApiSuccessResult<IEnumerable<CompetitionDto>>(result);
    }

    public async Task<ApiResult<PagedList<CompetitionDto>>> GetAllCompetitionPagination(SearchCompetitionDto searchCompetitionDto)
    {
        IEnumerable<CompetitionDto> competitionList;
        if (!string.IsNullOrEmpty(searchCompetitionDto.Value))
        {
            var competitionListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticCompetitions, new SearchRequestValue
                {
                    Value = searchCompetitionDto.Value,
                    Size = searchCompetitionDto.Size,
                });
            if (competitionListSearch is null)
            {
                return new ApiResult<PagedList<CompetitionDto>>(false, "Competition not found !!!");
            }

            competitionList = competitionListSearch.ToList();
        }
        else
        {
            var competitionListAll = await _competitionRepository.GetAllCompetition();
            if (!competitionListAll.Any())
            {
                return new ApiResult<PagedList<CompetitionDto>>(false, "Competitions not found !!!");
            }

            competitionList = _mapper.Map<IEnumerable<CompetitionDto>>(competitionListAll);
        }
        
        if (searchCompetitionDto.Status != null)
        {
            competitionList = competitionList.Where(c => c.IsActive == searchCompetitionDto.Status);
        }

        var pagedResult = await PagedList<CompetitionDto>.ToPageListAsync(competitionList.AsQueryable().BuildMock(), searchCompetitionDto.PageIndex, searchCompetitionDto.PageSize, searchCompetitionDto.OrderBy, searchCompetitionDto.IsAscending);
        return new ApiSuccessResult<PagedList<CompetitionDto>>(pagedResult);
    }

    public async Task<ApiResult<CompetitionDto>> GetCompetitionById(int id)
    {
        var competitionEntity = await _competitionRepository.GetCompetitionById(id);
        if (competitionEntity == null)
        {
            return new ApiResult<CompetitionDto>(false, "Competition is null !!!");
        }

        var result = _mapper.Map<CompetitionDto>(competitionEntity);
        return new ApiResult<CompetitionDto>(true, result, "Get competition by id successfully");
    }

    public async Task<ApiResult<CompetitionDto>> CreateCompetition(CompetitionCreateDto competitionCreateDto)
    {
        var competitionEntity = _mapper.Map<Competition>(competitionCreateDto);
        await _competitionRepository.CreateCompetition(competitionEntity);
        var result = _mapper.Map<CompetitionDto>(competitionEntity);
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticCompetitions, result, g => g.Id);
        return new ApiResult<CompetitionDto>(true, result, "Create competition successfully");
    }

    public async Task<ApiResult<CompetitionDto>> UpdateCompetition(CompetitionUpdateDto competitionUpdateDto)
    {
        var competitionEntity = await _competitionRepository.GetCompetitionById(competitionUpdateDto.Id);
        if (competitionEntity == null)
        {
            return new ApiResult<CompetitionDto>(false, "Not found");
        }

        var obj = _mapper.Map(competitionUpdateDto, competitionEntity);
        await _competitionRepository.UpdateCompetition(obj);
        var result = _mapper.Map<CompetitionDto>(obj);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticCompetitions, result, competitionUpdateDto.Id);
        return new ApiResult<CompetitionDto>(true,result, "Update competition successfully");
    }

    public async Task<ApiResult<bool>> UpdateStatusCompetition(int id)
    {
        var competitionEntity = await _competitionRepository.GetCompetitionById(id);
        if (competitionEntity == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }

        competitionEntity.IsActive = !competitionEntity.IsActive;
        await _competitionRepository.UpdateCompetition(competitionEntity);
        var result = _mapper.Map<CompetitionDto>(competitionEntity);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticCompetitions, result, id);
        return new ApiResult<bool>(true, "Update Status of level successfully");
    }

    public async Task<ApiResult<bool>> DeleteCompetition(int id)
    {
        var competition = await _competitionRepository.GetCompetitionById(id);
        if (competition == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }
        // Delete related exams from Elasticsearch and Cloudinary
        var exams = await _examRepository.GetExamByCompetitionId(competition.Id);
        if (exams != null && exams.Any())
        {
            var listExamId = exams.Select(x => x.Id).ToList();
            await _elasticSearchService.DeleteDocumentRangeAsync(ElasticConstant.ElasticExams, listExamId);
        }
        foreach (var exam in exams)
        {
            if (exam.Type == EExamType.TL)
            {
                if (exam.PublicExamEssayId != null)
                {
                    await _cloudinaryService.DeleteFileAsync(exam.PublicExamEssayId);
                }

                if (exam.PublicExamEssaySolutionId != null)
                {
                    await _cloudinaryService.DeleteFileAsync(exam.PublicExamEssaySolutionId);
                }
            }
            else
            {
                var examCodes = await _examCodeRepository.GetAllExamCodeByExamId(exam.Id);
                if (examCodes != null && examCodes.Any())
                {
                    var listPublicId = examCodes.Select(x => x.PublicExamId).ToList();
                    await _cloudinaryService.DeleteRangeFileAsync(listPublicId);
                }
            }
        }
        
        var isDeleted = await _competitionRepository.DeleteCompetition(id);
        if (isDeleted==false)
        {
            return new ApiResult<bool>(false, "Delete competition failed");
        }
        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticCompetitions, id);
        return new ApiResult<bool>(true, "Delete competition successfully");
    }
}