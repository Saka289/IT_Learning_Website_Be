using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.LevelRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace LW.Services.GradeService;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IElasticSearchService<Grade, int> _elasticSearchService;
    private readonly IMapper _mapper;

    public GradeService(IGradeRepository gradeRepository, IMapper mapper, ILevelRepository levelRepository,
        IElasticSearchService<Grade, int> elasticSearchService)
    {
        _gradeRepository = gradeRepository;
        _mapper = mapper;
        _levelRepository = levelRepository;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ApiResult<IEnumerable<GradeDto>>> GetAllGrade()
    {
        var gradeList = await _gradeRepository.GetAllGrade();
        if (gradeList == null)
        {
            return new ApiResult<IEnumerable<GradeDto>>(false, "Grade is null !!!");
        }

        var result = _mapper.Map<IEnumerable<GradeDto>>(gradeList);
        return new ApiSuccessResult<IEnumerable<GradeDto>>(result);
    }

    public async Task<ApiResult<PagedList<GradeDto>>> GetAllGradePagination(PagingRequestParameters pagingRequestParameters)
    {
        var gradeList = await _gradeRepository.GetAllGradePagination();
        if (gradeList == null)
        {
            return new ApiResult<PagedList<GradeDto>>(false, "Grade is null !!!");
        }
        var result = _mapper.ProjectTo<GradeDto>(gradeList);
        var pagedResult = await PagedList<GradeDto>.ToPageList(result, pagingRequestParameters.PageIndex, pagingRequestParameters.PageSize);

        return new ApiSuccessResult<PagedList<GradeDto>>(pagedResult);
    }

    public async Task<ApiResult<GradeDto>> GetGradeById(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id);
        if (gradeEntity == null)
        {
            return new ApiResult<GradeDto>(false, "Grade is null !!!");
        }

        var result = _mapper.Map<GradeDto>(gradeEntity);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<IEnumerable<GradeDto>>> SearchByGrade(SearchGradeDto searchGradeDto)
    {
        var gradeEntity =
            await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticGrades, searchGradeDto);
        if (gradeEntity is null)
        {
            return new ApiResult<IEnumerable<GradeDto>>(false, $"Grade not found by {searchGradeDto.Key} !!!");
        }

        var result = _mapper.Map<IEnumerable<GradeDto>>(gradeEntity);
        return new ApiSuccessResult<IEnumerable<GradeDto>>(result);
    }

    public async Task<ApiResult<GradeDto>> CreateGrade(GradeCreateDto gradeCreateDto)
    {
        var levelEntity = await _levelRepository.GetLevelById(gradeCreateDto.LevelId);
        if (levelEntity is null)
        {
            return new ApiResult<GradeDto>(false, "LevelId not found !!!");
        }

        var gradeEntity = _mapper.Map<Grade>(gradeCreateDto);
        gradeEntity.KeyWord = gradeCreateDto.Title.RemoveDiacritics();
        await _gradeRepository.CreateGrade(gradeEntity);
        await _gradeRepository.SaveChangesAsync();
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticGrades, gradeEntity, g => g.Id);
        var result = _mapper.Map<GradeDto>(gradeEntity);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<GradeDto>> UpdateGrade(GradeUpdateDto gradeUpdateDto)
    {
        var levelEntity = await _levelRepository.GetLevelById(gradeUpdateDto.LevelId);
        if (levelEntity is null)
        {
            return new ApiResult<GradeDto>(false, "LevelId not found !!!");
        }

        var gradeEntity = await _gradeRepository.GetByIdAsync(gradeUpdateDto.Id);
        if (gradeEntity is null)
        {
            return new ApiResult<GradeDto>(false, "Grade not found !!!");
        }

        var model = _mapper.Map(gradeUpdateDto, gradeEntity);
        model.KeyWord = gradeUpdateDto.Title.RemoveDiacritics();
        var updateGrade = await _gradeRepository.UpdateGrade(model);
        await _gradeRepository.SaveChangesAsync();
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticGrades, updateGrade, gradeUpdateDto.Id);
        var result = _mapper.Map<GradeDto>(updateGrade);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateGradeStatus(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id);
        if (gradeEntity is null)
        {
            return new ApiResult<bool>(false, "Grade not found !!!");
        }

        gradeEntity.IsActive = !gradeEntity.IsActive;
        await _gradeRepository.UpdateGrade(gradeEntity);
        await _gradeRepository.SaveChangesAsync();
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticGrades, gradeEntity, id);
        return new ApiSuccessResult<bool>(true, "Grade update successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteGrade(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id);
        if (gradeEntity is null)
        {
            return new ApiResult<bool>(false, "Grade not found !!!");
        }

        var grade = await _gradeRepository.DeleteGrade(id);
        if (!grade)
        {
            return new ApiResult<bool>(false, "Failed Delete Grade not found !!!");
        }

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticGrades, id);

        return new ApiSuccessResult<bool>(true, "Delete Grade Successfully !!!");
    }
}