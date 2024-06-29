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
using MockQueryable.Moq;

namespace LW.Services.GradeServices;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IElasticSearchService<GradeDto, int> _elasticSearchService;
    private readonly IMapper _mapper;

    public GradeService(IGradeRepository gradeRepository, IMapper mapper, ILevelRepository levelRepository,
        IElasticSearchService<GradeDto, int> elasticSearchService)
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

    public async Task<ApiResult<IEnumerable<GradeDto>>> GetAllGradeByLevel(int id)
    {
        var gradeList = await _gradeRepository.GetAllGradeByLevel(id);
        if (gradeList == null)
        {
            return new ApiResult<IEnumerable<GradeDto>>(false, "Grade is null !!!");
        }

        var result = _mapper.Map<IEnumerable<GradeDto>>(gradeList);
        return new ApiSuccessResult<IEnumerable<GradeDto>>(result);
    }

    public async Task<ApiResult<PagedList<GradeDto>>> GetAllGradePagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var gradeList = await _gradeRepository.GetAllGradePagination();
        if (gradeList == null)
        {
            return new ApiResult<PagedList<GradeDto>>(false, "Grade is null !!!");
        }

        var result = _mapper.ProjectTo<GradeDto>(gradeList);
        var pagedResult = await PagedList<GradeDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex, pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<GradeDto>>(pagedResult);
    }

    public async Task<ApiResult<GradeDto>> GetGradeById(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id);
        if (gradeEntity == null)
        {
            return new ApiResult<GradeDto>(false, "Grade is null !!!");
        }
        var levelEntity = await _levelRepository.GetLevelById(gradeEntity.LevelId);
        if (levelEntity != null)
        {
            gradeEntity.Level = levelEntity;
        }

        var result = _mapper.Map<GradeDto>(gradeEntity);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<PagedList<GradeDto>>> SearchByGradePagination(SearchGradeDto searchGradeDto)
    {
        var gradeEntity = await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticGrades, searchGradeDto);
        if (gradeEntity is null)
        {
            return new ApiResult<PagedList<GradeDto>>(false, $"Grade not found by {searchGradeDto.Key} !!!");
        }
        
        if (searchGradeDto.LevelId > 0)
        {
            gradeEntity = gradeEntity.Where(d => d.LevelId == searchGradeDto.LevelId).ToList();
        }


        var result = _mapper.Map<IEnumerable<GradeDto>>(gradeEntity);
        var pagedResult = await PagedList<GradeDto>.ToPageListAsync(result.AsQueryable().BuildMock(), searchGradeDto.PageIndex, searchGradeDto.PageSize, searchGradeDto.OrderBy, searchGradeDto.IsAscending);
        return new ApiSuccessResult<PagedList<GradeDto>>(pagedResult);
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
        gradeEntity.Level = levelEntity;
        var result = _mapper.Map<GradeDto>(gradeEntity);
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticGrades, result, g => g.Id);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<GradeDto>> UpdateGrade(GradeUpdateDto gradeUpdateDto)
    {
        var levelEntity = await _levelRepository.GetLevelById(gradeUpdateDto.LevelId);
        if (levelEntity is null)
        {
            return new ApiResult<GradeDto>(false, "LevelId not found !!!");
        }

        var gradeEntity = await _gradeRepository.GetGradeById(gradeUpdateDto.Id);
        if (gradeEntity is null)
        {
            return new ApiResult<GradeDto>(false, "Grade not found !!!");
        }

        var model = _mapper.Map(gradeUpdateDto, gradeEntity);
        model.KeyWord = gradeUpdateDto.Title.RemoveDiacritics();
        var updateGrade = await _gradeRepository.UpdateGrade(model);
        await _gradeRepository.SaveChangesAsync();
        gradeEntity.Level = levelEntity;
        var result = _mapper.Map<GradeDto>(updateGrade);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticGrades, result, gradeUpdateDto.Id);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateGradeStatus(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id);
        if (gradeEntity is null)
        {
            return new ApiResult<bool>(false, "Grade not found !!!");
        }
        var levelEntity = await _levelRepository.GetLevelById(gradeEntity.LevelId);

        gradeEntity.IsActive = !gradeEntity.IsActive;
        await _gradeRepository.UpdateGrade(gradeEntity);
        await _gradeRepository.SaveChangesAsync();
        gradeEntity.Level = levelEntity;
        var result = _mapper.Map<GradeDto>(gradeEntity);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticGrades, result, id);
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

        _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticGrades, id);

        return new ApiSuccessResult<bool>(true, "Delete Grade Successfully !!!");
    }
}