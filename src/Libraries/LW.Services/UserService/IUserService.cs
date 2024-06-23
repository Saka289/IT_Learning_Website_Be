using LW.Shared.DTOs.Facebook;
using LW.Shared.DTOs.Google;
using LW.Shared.DTOs.Token;
using LW.Shared.DTOs.User;
using LW.Shared.SeedWork;

namespace LW.Services.UserService;

public interface IUserService
{
    public Task<ApiResult<RegisterResponseUserDto>> Register(RegisterUserDto registerUserDto);
    public Task<ApiResult<LoginResponseUserDto>> Login(LoginUserDto loginUserDto);
    public Task<ApiResult<LoginResponseUserDto>> LoginGoogle(GoogleSignInDto googleSignInDto);
    public Task<ApiResult<LoginResponseUserDto>> LoginFacebook(FacebookSignInDto facebookSignInDto);
    public Task<ApiResult<bool>> ChangePassword(ChangePasswordDto changePasswordDto);
    public Task<ApiResult<bool>> ForgotPassword(string email);
    public Task<ApiResult<bool>> ResetPassword(ResetPasswordDto resetPasswordDto);
    public Task<ApiResult<bool>> SendVerifyEmail(string email);
    public Task<ApiResult<bool>> VerifyEmail(string token);
    public Task<ApiResult<TokenResponseDto>> RefreshToken(TokenRequestDto tokenRequestDto);
    public Task<ApiResult<bool>> Revoke(string emailOrUserName);
    public Task<ApiResult<UpdateResponseUserDto>> UpdateUser(UpdateUserDto updateUserDto);
    public Task<ApiResult<UserResponseDto>> GetUserByUserId(string userId);
}