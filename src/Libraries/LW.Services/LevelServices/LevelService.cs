using System.Text;
using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.LevelRepositories;
using Serilog;
using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;

namespace LW.Services.LevelServices;

public class LevelService : ILevelService
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ILevelRepository _levelRepository;

    public LevelService(ILogger logger, ILevelRepository levelRepository, IMapper mapper)
    {
        _logger = logger;
        _levelRepository = levelRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<bool>> Create(LevelDtoForCreate model)
    {
        var level = _mapper.Map<Level>(model);
        level.KeyWord = level.Title.ToLower();
        await _levelRepository.CreateLevel(level);
        return new ApiResult<bool>(true, "Create level successfully");
    }


    public async Task<ApiResult<bool>> Update(LevelDtoForUpdate model)
    {
        var levelInDb = await _levelRepository.GetLevelById(model.Id);
        if (levelInDb == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }
        var obj = _mapper.Map(model, levelInDb);
        // Sau khi ánh xạ, levelIbDb sẽ có các giá trị từ model:
        var level = _mapper.Map<Level>(obj);
        level.KeyWord = level.Title.ToLower();
        await _levelRepository.UpdateLevel(level);
        return new ApiResult<bool>(true, "Update level successfully");
    }
    public async Task<ApiResult<bool>> UpdateStatus(int id)
    {
        var objLevel = await _levelRepository.GetLevelById(id);
        if (objLevel == null)
        {
            return new ApiResult<bool>(true, "Not found !");
        }
        objLevel.IsActive = !objLevel.IsActive;
        await _levelRepository.UpdateLevel(objLevel);
        return new ApiResult<bool>(true, "Update Status of level successfully");
    }

    public async Task<ApiResult<bool>> Delete(int id)
    {
        var level = await _levelRepository.GetLevelById(id);
        if (level == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }
        var isDeleted = await _levelRepository.DeleteLevel(id);
        if (!isDeleted)
        {
            return new ApiResult<bool>(false, "Delete level failed");
        }
        return new ApiResult<bool>(true, "Delete level successfully");
    }

    public async Task<ApiResult<IEnumerable<LevelDto>>> GetAll()
    {
        var list =await _levelRepository.GetAllLevel();
        var result = _mapper.Map<IEnumerable<LevelDto>>(list);
        return new ApiResult<IEnumerable<LevelDto>>(true,result, "Get all level successfully");
    }

    public async Task<ApiResult<LevelDto>> GetById(int id)
    {
        var level = await _levelRepository.GetLevelById(id);
        if (level == null)
        {
            return new ApiResult<LevelDto>(false, "Not Found");
        }
        var result = _mapper.Map<LevelDto>(level);
        return new ApiResult<LevelDto>(true,result, "Get level successfully");
    }
}