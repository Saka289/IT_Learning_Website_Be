using System.Reflection.Metadata;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.CompetitionRepositories;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.TagRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.File;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Tag;
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
    private readonly IGradeRepository _gradeRepository;
    private readonly ITagRepository _tagRepository;

    public ExamService(IMapper mapper, IExamRepository examRepository,
        IElasticSearchService<ExamDto, int> elasticSearchService, ICloudinaryService cloudinaryService,
        IExamCodeRepository examCodeRepository, ICompetitionRepository competitionRepository,
        IGradeRepository gradeRepository, ITagRepository tagRepository)
    {
        _mapper = mapper;
        _examRepository = examRepository;
        _elasticSearchService = elasticSearchService;
        _cloudinaryService = cloudinaryService;
        _examCodeRepository = examCodeRepository;
        _competitionRepository = competitionRepository;
        _gradeRepository = gradeRepository;
        _tagRepository = tagRepository;
    }

    public async Task<ApiResult<IEnumerable<ExamDto>>> GetAllExam(bool? status)
    {
        var listExam = await _examRepository.GetAllExam();
        if (listExam.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamDto>>(false, "Not Found");
        }

        if (status != null)
        {
            listExam = listExam.Where(e => e.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<ExamDto>>(listExam);
        return new ApiResult<IEnumerable<ExamDto>>(true, result, "Get list exam successfully");
    }

    public async Task<ApiResult<PagedList<ExamDto>>> GetAllExamPagination(SearchExamDto searchExamDto)
    {
        IEnumerable<ExamDto> examList;
        if (!string.IsNullOrEmpty(searchExamDto.Value))
        {
            var examListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticExams,
                new SearchRequestValue
                {
                    Value = searchExamDto.Value,
                    Size = searchExamDto.Size,
                });
            if (examListSearch is null)
            {
                return new ApiResult<PagedList<ExamDto>>(false, "Exam not found !!!");
            }

            examList = examListSearch.ToList();
        }
        else
        {
            var examListAll = await _examRepository.GetAllExam();
            if (!examListAll.Any())
            {
                return new ApiResult<PagedList<ExamDto>>(false, "Exam is null !!!");
            }

            examList = _mapper.Map<IEnumerable<ExamDto>>(examListAll);
        }

        if (searchExamDto.Status != null)
        {
            examList = examList.Where(e => e.IsActive == searchExamDto.Status);
        }

        if (searchExamDto.CompetitionId > 0)
        {
            examList = examList.Where(t => t.CompetitionId == searchExamDto.CompetitionId);
        }

        if (searchExamDto.LevelId > 0)
        {
            examList = examList.Where(t => t.LevelId == searchExamDto.LevelId);
        }

        if (searchExamDto.GradeId > 0)
        {
            examList = examList.Where(t => t.GradeId == searchExamDto.GradeId);
        }

        if (!string.IsNullOrEmpty(searchExamDto.Province))
        {
            examList = examList.Where(t => t.Province.ToLower().Contains(searchExamDto.Province.ToLower()));
        }

        if (searchExamDto.Year > 0)
        {
            examList = examList.Where(t => t.Year == searchExamDto.Year);
        }

        if (searchExamDto.Type > 0)
        {
            examList = examList.Where(t => t.Type == searchExamDto.Type);
        }

        var pagedResult = await PagedList<ExamDto>.ToPageListAsync(examList.AsQueryable().BuildMock(),
            searchExamDto.PageIndex, searchExamDto.PageSize, searchExamDto.OrderBy, searchExamDto.IsAscending);
        return new ApiSuccessResult<PagedList<ExamDto>>(pagedResult);
    }

    public async Task<ApiResult<IEnumerable<TagDto>>> GetExamIdByTag(int id)
    {
        var exam = await _examRepository.GetExamById(id);
        if (exam is null)
        {
            return new ApiResult<IEnumerable<TagDto>>(false, "Exam not found !!!");
        }

        var listStringTag = exam.KeyWord!.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
        var listTag = new List<Tag>();
        foreach (var item in listStringTag)
        {
            var tagEntity = await _tagRepository.GetTagByKeyword(item);
            if (tagEntity is not null)
            {
                listTag.Add(tagEntity);
            }
        }

        var result = _mapper.Map<IEnumerable<TagDto>>(listTag);
        return new ApiSuccessResult<IEnumerable<TagDto>>(result);
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

    public async Task<ApiResult<IEnumerable<ExamDto>>> GetExamByType(EExamType type, bool? status)
    {
        var listExam = await _examRepository.GetExamByType(type);
        if (listExam == null)
        {
            return new ApiResult<IEnumerable<ExamDto>>(false, "NotFound");
        }

        if (status != null)
        {
            listExam = listExam.Where(e => e.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<ExamDto>>(listExam);
        return new ApiResult<IEnumerable<ExamDto>>(true, result, "Get All Exam By Type Successfully");
    }

    public async Task<ApiResult<ExamDto>> CreateExam(ExamCreateDto examCreateDto)
    {
        var competition = await _competitionRepository.GetCompetitionById(examCreateDto.CompetitionId);
        if (competition == null)
        {
            return new ApiResult<ExamDto>(false, "Competition not found !!!");
        }


        if (examCreateDto.GradeId > 0)
        {
            var gradeExist = await _gradeRepository.GetGradeById(Convert.ToInt32(examCreateDto.GradeId), false);
            if (gradeExist == null)
            {
                return new ApiResult<ExamDto>(false, "Grade not found");
            }

            examCreateDto.LevelId = null;
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

        var keyWordValue = (examCreateDto.TagValues is not null)
            ? examCreateDto.TagValues.ConvertToTagString()
            : examCreateDto.Title!.RemoveDiacritics();
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

        if (examUpdateDto.GradeId > 0)
        {
            var gradeExist = await _gradeRepository.GetGradeById(Convert.ToInt32(examUpdateDto.GradeId), false);
            if (gradeExist == null)
            {
                return new ApiResult<ExamDto>(false, "Grade not found");
            }

            examUpdateDto.LevelId = null;
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
            if (!string.IsNullOrEmpty(exam.PublicExamEssaySolutionId))
            {
                var filePath = await _cloudinaryService.UpdateFileAsync(exam.PublicExamEssaySolutionId,
                    examUpdateDto.ExamSolutionFileUpload);
                objUpdate.PublicExamEssaySolutionId = filePath.PublicId;
                objUpdate.ExamSolutionFile = filePath.Url;
                objUpdate.UrlDownloadSolutionFile = filePath.UrlDownload;
            }
            else
            {
                var filePath = await _cloudinaryService.CreateFileAsync(examUpdateDto.ExamSolutionFileUpload,
                    CloudinaryConstant.FolderExamFilePdf);
                objUpdate.PublicExamEssaySolutionId = filePath.PublicId;
                objUpdate.ExamSolutionFile = filePath.Url;
                objUpdate.UrlDownloadSolutionFile = filePath.UrlDownload;
            }
        }

        objUpdate.KeyWord = (examUpdateDto.TagValues is not null)
            ? examUpdateDto.TagValues.ConvertToTagString()
            : examUpdateDto.Title!.RemoveDiacritics();
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