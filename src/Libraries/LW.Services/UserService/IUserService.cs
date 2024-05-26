using LW.Shared.DTOs.User;
using LW.Shared.SeedWork;

namespace LW.Services.UserService;

public interface IUserService
{
    public Task<ApiResult<RegisterResponseUserDto>> Register(RegisterUserDto registerUserDto);
    public Task<ApiResult<LoginResponseUserDto>> Login(LoginUserDto loginUserDto);
    public Task<ApiResult<LoginResponseUserDto>> LoginGoogle(LoginUserDto loginUserDto);
    public Task<ApiResult<bool>> SendVerifyEmail(string email);
    public Task<ApiResult<bool>> VerifyEmail(string token);
}