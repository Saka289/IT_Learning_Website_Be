using AutoMapper;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.LevelRepositories;
using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;

namespace LW.Services.LevelServices;

public class LevelService : ILevelService
{
    private readonly ILevelRepository _levelRepository;
    private readonly IMapper _mapper;

    public LevelService(ILevelRepository levelRepository, IMapper mapper)
    {
        _levelRepository = levelRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<IEnumerable<LevelDto>>> GetAllLevel()
    {
        var levels = await _levelRepository.GetAllLevel();
        if (!levels.Any())
        {
            return new ApiResult<IEnumerable<LevelDto>>(false, "Levels is null !!!");
        }

        var result = _mapper.Map<IEnumerable<LevelDto>>(levels);
        return new ApiSuccessResult<IEnumerable<LevelDto>>(result);
    }

    public async Task<ApiResult<LevelDto>> GetLevelById(int id)
    {
        var level = await _levelRepository.GetLevelById(id);
        if (level == null)
        {
            return new ApiResult<LevelDto>(false, "Not found level");
        }

        var result = _mapper.Map<LevelDto>(level);
        return new ApiResult<LevelDto>(true, result, "Get by id successfully");
    }
}