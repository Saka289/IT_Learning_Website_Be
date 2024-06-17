using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.GradeRepositories;
using LW.Services.LevelServices;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;

namespace LW.Services.GradeService;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly ILevelService _levelService;
    private readonly IMapper _mapper;

    public GradeService(IGradeRepository gradeRepository, IMapper mapper, ILevelService levelService)
    {
        _gradeRepository = gradeRepository;
        _mapper = mapper;
        _levelService = levelService;
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

    public async Task<ApiResult<GradeDto>> CreateGrade(GradeCreateDto gradeCreateDto)
    {
        var levelEntity = await _levelService.GetById(gradeCreateDto.LevelId);
        if (!levelEntity.IsSucceeded)
        {
            return new ApiResult<GradeDto>(false, "LevelId not found !!!");
        }

        var gradeEntity = _mapper.Map<Grade>(gradeCreateDto);
        gradeEntity.IsActive = true;
        await _gradeRepository.CreateGrade(gradeEntity);
        await _gradeRepository.SaveChangesAsync();
        var result = _mapper.Map<GradeDto>(gradeEntity);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<GradeDto>> UpdateGrade(GradeUpdateDto gradeUpdateDto)
    {
        var levelEntity = await _levelService.GetById(gradeUpdateDto.LevelId);
        if (!levelEntity.IsSucceeded)
        {
            return new ApiResult<GradeDto>(false, "LevelId not found !!!");
        }

        var gradeEntity = await _gradeRepository.GetByIdAsync(gradeUpdateDto.Id);
        if (gradeEntity is null)
        {
            return new ApiResult<GradeDto>(false, "Grade not found !!!");
        }

        var model = _mapper.Map(gradeUpdateDto, gradeEntity);
        var updateGrade = await _gradeRepository.UpdateGrade(model);
        await _gradeRepository.SaveChangesAsync();

        var result = _mapper.Map<GradeDto>(updateGrade);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteGrade(int id)
    {
        var gradeEntity = await _gradeRepository.GetByIdAsync(id);
        if (gradeEntity is null)
        {
            return new ApiResult<bool>(false, "Grade not found !!!");
        }

        var grade = await _gradeRepository.DeleteGrade(id);
        if (!grade)
        {
            return new ApiResult<bool>(false, "Failed Delete Grade not found !!!");
        }

        return new ApiSuccessResult<bool>(true, "Delete Grade Successfully !!!");
    }
}