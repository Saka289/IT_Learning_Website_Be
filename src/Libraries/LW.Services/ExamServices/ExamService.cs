using System.Reflection.Metadata;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.CompetitionRepositories;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.File;
using LW.Shared.DTOs.Lesson;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.ExamServices;

public class ExamService : IExamService
{
    private readonly IMapper _mapper;
    private readonly IExamRepository _examRepository;
    private readonly IElasticSearchService<ExamDto, int> _elasticSearchService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IExamCodeRepository _examCodeRepository;
    private readonly ICompetitionRepository _competitionRepository;

    public ExamService(IMapper mapper, IExamRepository examRepository,
        IElasticSearchService<ExamDto, int> elasticSearchService, ICloudinaryService cloudinaryService,
        IExamCodeRepository examCodeRepository, ICompetitionRepository competitionRepository)
    {
        _mapper = mapper;
        _examRepository = examRepository;
        _elasticSearchService = elasticSearchService;
        _cloudinaryService = cloudinaryService;
        _examCodeRepository = examCodeRepository;
        _competitionRepository = competitionRepository;
    }

    public async Task<ApiResult<IEnumerable<ExamDto>>> GetAllExam()
    {
        var listExam = await _examRepository.GetAllExam();
        if (listExam.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamDto>>(false, "Not Found");
        }

        var result = _mapper.Map<IEnumerable<ExamDto>>(listExam);
        return new ApiResult<IEnumerable<ExamDto>>(true, result, "Get list exam successfully");
    }

    public async Task<ApiResult<PagedList<ExamDto>>> GetAllExamPagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var examList = await _examRepository.GetAllExamByPagination();
        if (examList == null)
        {
            return new ApiResult<PagedList<ExamDto>>(false, "Exam is null !!!");
        }

        var result = _mapper.ProjectTo<ExamDto>(examList);
        var pagedResult = await PagedList<ExamDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<ExamDto>>(pagedResult);
    }

    public async Task<ApiResult<ExamDto>> GetExamById(int id)
    {
        var exam = await _examRepository.GetExamById(id);
        if (exam == null)
        {
            return new ApiResult<ExamDto>(false, "Not Found");
        }

        var result = _mapper.Map<ExamDto>(exam);
        return new ApiResult<ExamDto>(true, result, "Get Exam By Id Successfully");
    }

    public async Task<ApiResult<IEnumerable<ExamDto>>> GetExamByType(EExamType type)
    {
        var listExam = await _examRepository.GetExamByType(type);
        if (listExam == null)
        {
            return new ApiResult<IEnumerable<ExamDto>>(false, "NotFound");
        }

        var result = _mapper.Map<IEnumerable<ExamDto>>(listExam);
        return new ApiResult<IEnumerable<ExamDto>>(true, result, "Get All Exam By Type Successfully");
    }

    public async Task<ApiResult<PagedList<ExamDto>>> SearchByExamPagination(SearchExamDto searchExamDto)
    {
        var listExam =
            await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticExams, searchExamDto);
        if (listExam is null)
        {
            return new ApiResult<PagedList<ExamDto>>(false, $"Exam not found by {searchExamDto.Key} !!!");
        }
        if (searchExamDto.CompetitionId > 0)
        {
            listExam = listExam.Where(t => t.CompetitionId == searchExamDto.CompetitionId).ToList();
        }
        if (!string.IsNullOrEmpty(searchExamDto.Province))
        {
            listExam = listExam.Where(t => t.Province.ToLower().Contains(searchExamDto.Province.ToLower())).ToList();
        }
        if (searchExamDto.Year > 0)
        {
            listExam = listExam.Where(t => t.Year == searchExamDto.Year).ToList();
        }
        if (searchExamDto.Type > 0)
        {
            listExam = listExam.Where(t => t.Type == searchExamDto.Type).ToList();
        }
        
        var result = _mapper.Map<IEnumerable<ExamDto>>(listExam);
        var pagedResult = await PagedList<ExamDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchExamDto.PageIndex, searchExamDto.PageSize, searchExamDto.OrderBy, searchExamDto.IsAscending);
        return new ApiSuccessResult<PagedList<ExamDto>>(pagedResult);
    }

    public async Task<ApiResult<ExamDto>> CreateExam(ExamCreateDto examCreateDto)
    {
        var competition = await _competitionRepository.GetCompetitionById(examCreateDto.CompetitionId);
        if (competition == null)
        {
            return new ApiResult<ExamDto>(false, "Competition not found !!!");
        }
        var obj = _mapper.Map<Exam>(examCreateDto);
        if (examCreateDto.ExamEssayFileUpload != null && examCreateDto.ExamEssayFileUpload.Length > 0)
        {
            var filePath = await _cloudinaryService.CreateFileAsync(examCreateDto.ExamEssayFileUpload,
                CloudinaryConstant.FolderExamFilePdf);
            obj.ExamEssayFile = filePath.Url;
            obj.PublicExamEssayId = filePath.PublicId;
        }

        if (examCreateDto.ExamSolutionFileUpload != null && examCreateDto.ExamSolutionFileUpload.Length > 0)
        {
            var filePath = await _cloudinaryService.CreateFileAsync(examCreateDto.ExamSolutionFileUpload,
                CloudinaryConstant.FolderExamFilePdf);
            obj.ExamSolutionFile = filePath.Url;
            obj.PublicExamEssaySolutionId = filePath.PublicId;
            obj.UrlDownloadSolutionFile = filePath.UrlDownload;
        }

        var keyWordValue = examCreateDto.tagValues.ConvertToTagString();
        obj.KeyWord = keyWordValue;
        await _examRepository.CreateExam(obj);
        obj.Competition = competition;
        var result = _mapper.Map<ExamDto>(obj);
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticExams, result, x => x.Id);
        return new ApiResult<ExamDto>(true, result, "Create exam successfully");
    }

    public async Task<ApiResult<ExamDto>> UpdateExam(ExamUpdateDto examUpdateDto)
    {
        var competition = await _competitionRepository.GetCompetitionById(examUpdateDto.CompetitionId);
        if (competition == null)
        {
            return new ApiResult<ExamDto>(false, "Competition not found !!!");
        }
        
        var exam = await _examRepository.GetExamById(examUpdateDto.Id);
        if (exam == null)
        {
            return new ApiResult<ExamDto>(false, "Not Found");
        }

        var objUpdate = _mapper.Map(examUpdateDto, exam);
        if (examUpdateDto.ExamEssayFileUpload != null && examUpdateDto.ExamEssayFileUpload.Length > 0)
        {
            var filePath =
                await _cloudinaryService.UpdateFileAsync(exam.PublicExamEssayId, examUpdateDto.ExamEssayFileUpload);
            objUpdate.PublicExamEssayId = filePath.PublicId;
            objUpdate.ExamEssayFile = filePath.Url;
        }

        if (examUpdateDto.ExamSolutionFileUpload != null && examUpdateDto.ExamSolutionFileUpload.Length > 0)
        {
            var filePath = await _cloudinaryService.UpdateFileAsync(exam.PublicExamEssaySolutionId,
                examUpdateDto.ExamSolutionFileUpload);
            objUpdate.PublicExamEssaySolutionId = filePath.PublicId;
            objUpdate.ExamSolutionFile = filePath.Url;
            objUpdate.UrlDownloadSolutionFile = filePath.UrlDownload;
        }

        if (examUpdateDto.tagValues != null && examUpdateDto.tagValues.Any())
        {
            var keyWordValue = examUpdateDto.tagValues.ConvertToTagString();
            objUpdate.KeyWord = keyWordValue;
        }

        await _examRepository.UpdateExam(objUpdate);
        var examDto = _mapper.Map<ExamDto>(objUpdate);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticExams, examDto, examUpdateDto.Id);
        return new ApiResult<ExamDto>(true, examDto, "Update exam successfully");
    }

    public async Task<ApiResult<bool>> UpdateExamStatus(int id)
    {
        var exam = await _examRepository.GetExamById(id);
        if (exam == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        exam.IsActive = !exam.IsActive;
        await _examRepository.UpdateExam(exam);
        var examDto = _mapper.Map<ExamDto>(exam);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticExams, examDto, exam.Id);
        return new ApiResult<bool>(true, "Update status exam successfully");
    }

    public async Task<ApiResult<bool>> DeleteExam(int id)
    {
        var exam = await _examRepository.GetExamById(id);
        if (exam == null)
        {
            return new ApiResult<bool>(false, "Not Found");
        }

        // delete file essay pdf 
        if (!string.IsNullOrEmpty(exam.ExamEssayFile))
        {
            await _cloudinaryService.DeleteFileAsync(exam.PublicExamEssayId);
        }

        // delete file essay solution pdf 
        if (!string.IsNullOrEmpty(exam.ExamSolutionFile))
        {
            await _cloudinaryService.DeleteFileAsync(exam.PublicExamEssaySolutionId);
        }

        // delete file multichoice for examcode
        var examCodes = await _examCodeRepository.GetAllExamCodeByExamId(exam.Id);
        if (examCodes.Any())
        {
            var listPublicId = examCodes.Select(x => x.PublicExamId).ToList();
            await _cloudinaryService.DeleteRangeFileAsync(listPublicId);
        }

        var result = await _examRepository.DeleteExam(id);
        if (result == false)
        {
            return new ApiResult<bool>(false, "Delete Exam Failed");
        }

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticExams, exam.Id);
        return new ApiResult<bool>(true, "Delete Exam Successfully");
    }

    
}