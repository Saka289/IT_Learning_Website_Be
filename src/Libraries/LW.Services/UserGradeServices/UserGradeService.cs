using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.UserGradeRepositories;
using LW.Services.GradeServices;
using LW.Shared.DTOs;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.UserGradeServices;

public class UserGradeService : IUserGradeService
{
    private readonly IUserGradeRepository _userGradeRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGradeRepository _gradeRepository;

    public UserGradeService(IUserGradeRepository userGradeRepository, IMapper mapper,
        UserManager<ApplicationUser> userManager, IGradeService gradeService, IGradeRepository gradeRepository)
    {
        _userGradeRepository = userGradeRepository;
        _mapper = mapper;
        _userManager = userManager;
        _gradeRepository = gradeRepository;
    }

    public async Task<ApiResult<bool>> CreatUserGrade(UserGradeCreateDto model)
    {
        var gradeCheck = await _gradeRepository.GetGradeById(model.GradeId, false);
        if (gradeCheck == null)
        {
            return new ApiResult<bool>(false, "Grade not found");
        }

        var userCheck = await _userManager.FindByIdAsync(model.UserId);
        if (userCheck == null)
        {
            return new ApiResult<bool>(false, "User not found");
        }

        var userGrade = _mapper.Map<UserGrade>(model);
        await _userGradeRepository.CreateUserGrade(userGrade);
        return new ApiResult<bool>(true, "Create user grade successfully");
    }

    public async Task<ApiResult<bool>> CreatRangeUserGrade(IEnumerable<UserGradeCreateDto> models)
    {
        var userGrades = _mapper.Map<IEnumerable<UserGrade>>(models);
        await _userGradeRepository.CreateRangeUserGrade(userGrades);
        return new ApiResult<bool>(true, "Create range user grade successfully");
    }

    public async Task<ApiResult<bool>> UpdateUserGrade(UserGradeUpdateDto model)
    {
        var obj = await _userGradeRepository.GetUserGradeById(model.Id);
        if (obj == null)
        {
            return new ApiResult<bool>(false, "Not Found");
        }

        var gradeCheck = await _gradeRepository.GetGradeById(model.GradeId, false);
        if (gradeCheck == null)
        {
            return new ApiResult<bool>(false, "Grade not found");
        }

        var userCheck = await _userManager.FindByIdAsync(model.UserId);
        if (userCheck == null)
        {
            return new ApiResult<bool>(false, "User not found");
        }

        var userGradeUpdate = _mapper.Map<UserGrade>(model);
        await _userGradeRepository.UpdateUserGrade(userGradeUpdate);
        return new ApiResult<bool>(true, "Update user grade successfully");
    }

    public async Task<ApiResult<bool>> DeleteUserGrade(int id)
    {
        var userGrade = await _userGradeRepository.GetUserGradeById(id);
        if (userGrade == null)
        {
            return new ApiResult<bool>(false, "UserGrade is null !!!");
        }

        await _userGradeRepository.DeleteUserGrade(id);
        return new ApiSuccessResult<bool>(true, "Delete user grade successfully");
    }

    public async Task<ApiResult<bool>> DeleteRangeUserGrade(string userId)
    {
        var listUserGrade = await _userGradeRepository.GetAllByUserId(userId);
        if (listUserGrade.Count() == 0)
        {
            return new ApiResult<bool>(false, "Not Found !!!");
        }
        await _userGradeRepository.DeleteRangeUserGrade(listUserGrade);
        return new ApiSuccessResult<bool>(true, "Delete list user grade successfully");
    }

    public async Task<ApiResult<IEnumerable<UserGradeDto>>> GetAllUserGrade()
    {
        var userGradeList = await _userGradeRepository.GetAllUserGrade();
        if (userGradeList == null)
        {
            return new ApiResult<IEnumerable<UserGradeDto>>(false, "List UserGrade is null !!!");
        }

        var result = _mapper.Map<IEnumerable<UserGradeDto>>(userGradeList);
        return new ApiSuccessResult<IEnumerable<UserGradeDto>>(result);
    }

    public async Task<ApiResult<UserGradeDto>> GeUserGradeById(int id)
    {
        var userGrade = await _userGradeRepository.GetUserGradeById(id);
        if (userGrade == null)
        {
            return new ApiResult<UserGradeDto>(false, "List UserGrade is null !!!");
        }

        var result = _mapper.Map<UserGradeDto>(userGrade);
        return new ApiSuccessResult<UserGradeDto>(result);
    }

    public  async Task<ApiResult<IEnumerable<UserGradeDto>>> GetAllUserGradeByUserId(string userId)
    {
        var userGrades = await _userGradeRepository.GetAllUserGradeByUserId(userId);
        if (userGrades.Count()==0)
        {
            return new ApiResult<IEnumerable<UserGradeDto>>(false, "List UserGrade is null !!!");
        }
        var result = _mapper.Map<IEnumerable<UserGradeDto>>(userGrades);
        return new ApiSuccessResult<IEnumerable<UserGradeDto>>(result);
    }
}