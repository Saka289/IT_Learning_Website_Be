using System.Globalization;
using System.Text;
using AutoMapper;
using Google.Apis.Auth;
using LW.Cache.Interfaces;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Services.Common.CommonServices.FacebookServices;
using LW.Services.Common.CommonServices.JwtTokenServices;
using LW.Services.Common.ModelMapping;
using LW.Shared.Configurations;
using LW.Shared.Constant;
using LW.Shared.DTOs.Email;
using LW.Shared.DTOs.Facebook;
using LW.Shared.DTOs.Google;
using LW.Shared.DTOs.Member;
using LW.Shared.DTOs.Token;
using LW.Shared.DTOs.User;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using LW.Shared.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Serilog;

namespace LW.Services.UserServices;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ISmtpEmailService _emailService;
    private readonly VerifyEmailSettings _verifyEmailSettings;
    private readonly UrlBase _urlBase;
    private readonly IRedisCache<VerifyEmailTokenDto> _redisCacheService;
    private readonly ISerializeService _serializeService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly GoogleSettings _googleSettings;
    private readonly IFacebookService _facebookService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IElasticSearchService<MemberDto, string> _elasticSearchService;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper,
        ISmtpEmailService emailService, ILogger logger, IOptions<VerifyEmailSettings> verifyEmailSettings,
        IOptions<UrlBase> urlBase, IRedisCache<VerifyEmailTokenDto> redisCacheService,
        ISerializeService serializeService, IJwtTokenService jwtTokenService, IOptions<GoogleSettings> googleSettings,
        IFacebookService facebookService, ICloudinaryService cloudinaryService, IElasticSearchService<MemberDto, string> elasticSearchService, SignInManager<ApplicationUser> signInManager)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        _serializeService = serializeService ?? throw new ArgumentNullException(nameof(serializeService));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _facebookService = facebookService ?? throw new ArgumentNullException(nameof(facebookService));
        _cloudinaryService = cloudinaryService;
        _elasticSearchService = elasticSearchService;
        _signInManager = signInManager;
        _googleSettings = googleSettings.Value;
        _urlBase = urlBase.Value;
        _verifyEmailSettings = verifyEmailSettings.Value;
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<ApiResult<RegisterResponseUserDto>> Register(RegisterUserDto registerUserDto)
    {
        var verifyEmail = await _redisCacheService.GetStringKey(registerUserDto.Email);
        if (verifyEmail == null)
        {
            return new ApiResult<RegisterResponseUserDto>(false,
                "Your email verification link has expired. Please request a new one.");
        }

        if (!verifyEmail.IsVerifyEmail)
        {
            return new ApiResult<RegisterResponseUserDto>(false,
                "An error occurred while verifying your email. Please try again later.");
        }

        var emailExist = await _userManager.Users.AnyAsync(x => x.Email.ToLower() == registerUserDto.Email.ToLower());
        if (emailExist)
        {
            return new ApiResult<RegisterResponseUserDto>(false,
                $"An existing account is using {registerUserDto.Email}");
        }

        var userNameExist = await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == registerUserDto.UserName.ToLower());
        if (userNameExist)
        {
            return new ApiResult<RegisterResponseUserDto>(false,
                $"An existing username is using {registerUserDto.UserName}");
        }

        var user = _mapper.Map<ApplicationUser>(registerUserDto);

            user.EmailConfirmed = true;

        user.EmailConfirmed = true;
        user.Image = CloudinaryConstant.Avatar;
        user.PublicId = CloudinaryConstant.AvatarPublicKey;

        var result = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
        {
            return new ApiResult<RegisterResponseUserDto>(false, result.Errors.FirstOrDefault().Description);
        }

        await _userManager.AddToRoleAsync(user, RoleConstant.RoleUser);
        var userDto = new RegisterResponseUserDto
        {
            ID = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), a => a.Id);
        return new ApiResult<RegisterResponseUserDto>(true, userDto, "Create User Successfully");
    }

    public async Task<ApiResult<LoginResponseUserDto>> Login(LoginUserDto loginUserDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.UserName.ToLower().Equals(loginUserDto.EmailOrUserName) ||
            u.Email.ToLower().Equals(loginUserDto.EmailOrUserName));
        if (user == null)
        {
            return new ApiResult<LoginResponseUserDto>(false, "Invalid UserName or Email !!!");
        }

        var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, true);
        if (checkPassword.IsLockedOut)
        {
            return new ApiResult<LoginResponseUserDto>(false, $"Your account has been locked. You should wait until {user.LockoutEnd} (UTC time) to be able to login");
        }
        if (!checkPassword.Succeeded)
        {
            return new ApiResult<LoginResponseUserDto>(false, "The password you entered is incorrect. Please try again.");
        }
        
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        UserDto userDto = new()
        {
            ID = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
        LoginResponseUserDto loginResponseUserDto = new()
        {
            UserDto = userDto,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        return new ApiResult<LoginResponseUserDto>(true, loginResponseUserDto, "Login successfully !!!");
    }

    public async Task<ApiResult<LoginResponseUserDto>> LoginGoogle(GoogleSignInDto googleSignInDto)
    {
        GoogleJsonWebSignature.Payload payload = new();
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

            settings.Audience = new List<string>()
            {
                _googleSettings.ClientId
            };

            payload = GoogleJsonWebSignature.ValidateAsync(googleSignInDto.IdToken, settings).Result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
            return new ApiResult<LoginResponseUserDto>(false, ex.Message);
        }

        var userCreated = new CreateUserFromSocialLogin()
        {
            FirstName = payload.FamilyName,
            LastName = payload.GivenName,
            Email = payload.Email,
            ProfilePicture = payload.Picture,
            LoginProviderSubject = payload.Subject,
        };

        var user = await _userManager.CreateUserFromSocialLogin(userCreated, ELoginProvider.Google);
        var isLockedOut  = await _userManager.IsLockedOutAsync(user);
        if (isLockedOut)
        {
            return new ApiResult<LoginResponseUserDto>(false, $"Your account has been locked. You should wait until {user.LockoutEnd} (UTC time) to be able to login");
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        UserDto userDto = new()
        {
            ID = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
        LoginResponseUserDto loginResponseUserDto = new()
        {
            UserDto = userDto,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), a => a.Id);
        return new ApiResult<LoginResponseUserDto>(true, loginResponseUserDto, "Login successfully !!!");
    }

    public async Task<ApiResult<LoginResponseUserDto>> LoginFacebook(FacebookSignInDto facebookSignInDto)
    {
        var validatedFbToken = await _facebookService.ValidateFacebookToken(facebookSignInDto.AccessToken);
        if (validatedFbToken is null)
        {
            return new ApiResult<LoginResponseUserDto>(false, null, "Failed to get response");
        }

        var userInfo = await _facebookService.GetFacebookUserInformation(facebookSignInDto.AccessToken);
        if (userInfo is null)
        {
            return new ApiResult<LoginResponseUserDto>(false, null, "Failed to get response");
        }

        var userCreated = new CreateUserFromSocialLogin()
        {
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            Email = userInfo.Email,
            ProfilePicture = userInfo.Picture.Data.Url.AbsoluteUri,
            LoginProviderSubject = userInfo.Id,
        };

        var user = await _userManager.CreateUserFromSocialLogin(userCreated, ELoginProvider.Facebook);
        var isLockedOut  = await _userManager.IsLockedOutAsync(user);
        if (isLockedOut)
        {
            return new ApiResult<LoginResponseUserDto>(false, $"Your account has been locked. You should wait until {user.LockoutEnd} (UTC time) to be able to login");
        }
        
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        UserDto userDto = new()
        {
            ID = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
        LoginResponseUserDto loginResponseUserDto = new()
        {
            UserDto = userDto,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), a => a.Id);
        return new ApiResult<LoginResponseUserDto>(true, loginResponseUserDto, "Login successfully !!!");
    }

    public async Task<ApiResult<bool>> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower().Equals(changePasswordDto.Email.ToLower()));
        var password = await _userManager.CheckPasswordAsync(user, changePasswordDto.Password);
        if (password == false)
        {
            return new ApiResult<bool>(false, "The password you entered is incorrect.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return new ApiResult<bool>(false, result.Errors.FirstOrDefault().Description);
        }

        return new ApiResult<bool>(true, "Changed password successfully !!!");
    }

    public async Task<ApiResult<bool>> ForgotPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new ApiResult<bool>(false, "This email address has not been registered yet");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        SendForgotPasswordEmail(user, token, new CancellationToken());

        return new ApiResult<bool>(true, $"Send email to: {email}");
    }

    public async Task<ApiResult<bool>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return new ApiResult<bool>(false, "This email address has not been registered yet");
        }

        var decodedTokenBytes = WebEncoders.Base64UrlDecode(resetPasswordDto.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return new ApiResult<bool>(false, result.Errors.FirstOrDefault().Description);
        }

        return new ApiResult<bool>(true, "Reset password successfully !!!");
    }

    public async Task<ApiResult<bool>> SendVerifyEmail(string email)
    {
        var emailExist = await _userManager.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        if (emailExist)
        {
            return new ApiResult<bool>(false, $"An existing account is using {email}");
        }

        var verifyEmailTokenDto = new VerifyEmailTokenDto { Email = email };

        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        await _redisCacheService.SetStringKey(email, verifyEmailTokenDto, options);

        var token = _serializeService.Serialize(verifyEmailTokenDto);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        SendConfirmEmailAsync(email, token, new CancellationToken());

        return new ApiResult<bool>(true, $"Send email to: {email}");
    }

    public async Task<ApiResult<bool>> VerifyEmail(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return new ApiResult<bool>(false, $"Token is null or empty !!!");
        }

        try
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var verifyEmailDecode = _serializeService.Deserialize<VerifyEmailTokenDto>(decodedToken);

            var result = await _redisCacheService.GetStringKey(verifyEmailDecode.Email);
            if (result == null)
            {
                return new ApiResult<bool>(false,
                    "Your email verification link has expired. Please request a new one.");
            }

            if (verifyEmailDecode.Id.Equals(result.Id))
            {
                verifyEmailDecode.IsVerifyEmail = true;
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await _redisCacheService.SetStringKey(verifyEmailDecode.Email, verifyEmailDecode, options);
                return new ApiResult<bool>(true, "Email verified successfully.");
            }

            return new ApiResult<bool>(false, "Invalid email verification link.");
        }
        catch (FormatException ex)
        {
            return new ApiResult<bool>(false, "Invalid token format.");
        }
        catch (Exception ex)
        {
            return new ApiResult<bool>(false, "An error occurred while verifying your email. Please try again later.");
        }
    }

    public async Task<ApiResult<TokenResponseDto>> RefreshToken(TokenRequestDto tokenRequestDto)
    {
        if (string.IsNullOrEmpty(tokenRequestDto.AccessToken))
        {
            return new ApiResult<TokenResponseDto>(false, $"AccessToken is null or empty !!!");
        }

        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(tokenRequestDto.AccessToken);
        var userName = principal.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Name).Value;
        if (userName is null)
        {
            return new ApiResult<TokenResponseDto>(false, $"Invalid AccessToken or refresh token !!!");
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(userName));
        if (user == null || user.RefreshToken != tokenRequestDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return new ApiResult<TokenResponseDto>(false, $"Invalid AccessToken or refresh token !!!");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(user);

        var tokenResponse = new TokenResponseDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return new ApiResult<TokenResponseDto>(true, tokenResponse, "RefreshToken successfully !!!");
    }

    public async Task<ApiResult<bool>> Revoke(string emailOrUserName)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.UserName.ToLower().Equals(emailOrUserName) || u.Email.ToLower().Equals(emailOrUserName));
        if (user == null)
        {
            return new ApiResult<bool>(false, "Invalid UserName or Email !!!");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);
        return new ApiSuccessResult<bool>(true, "Revoke successfully !!!");
    }

    public async Task<ApiResult<UpdateResponseUserDto>> UpdateUser(UpdateUserDto updateUserDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.ToLower().Equals(updateUserDto.UserId) || u.Email.ToLower().Equals(updateUserDto.Email));
        if (user == null)
        {
            return new ApiResult<UpdateResponseUserDto>(false, $"User Not Found !");
        }

        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.PhoneNumber = updateUserDto.PhoneNumber;
        user.Dob = DateOnly.FromDateTime(updateUserDto.Dob);

        if (user.Image == null && updateUserDto.Image != null)
        {
            var imageCreatePath =
                await _cloudinaryService.CreateImageAsync(updateUserDto.Image, CloudinaryConstant.FolderUserImage);

            user.Image = imageCreatePath.Url;
            user.PublicId = imageCreatePath.PublicId;

            await _userManager.UpdateAsync(user);

            var userResponseCreate = new UpdateResponseUserDto()
            {
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                PhoneNumber = user.PhoneNumber,
                Image = user.Image
            };
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), updateUserDto.UserId);
            return new ApiResult<UpdateResponseUserDto>(true, userResponseCreate, $"Update User Successfully !");
        }

        if (updateUserDto.Image == null)
        {
            await _userManager.UpdateAsync(user);

            var userResponseCreate = new UpdateResponseUserDto()
            {
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                PhoneNumber = user.PhoneNumber,
                Image = user.Image
            };
            _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), updateUserDto.UserId);
            return new ApiResult<UpdateResponseUserDto>(true, userResponseCreate, $"Update User Successfully !");
        }

        var imageUpdatePath = await _cloudinaryService.UpdateImageAsync(user.PublicId, updateUserDto.Image);

        user.Image = imageUpdatePath.Url;
        user.PublicId = imageUpdatePath.PublicId;

        await _userManager.UpdateAsync(user);

        var userResponseUpdate = new UpdateResponseUserDto()
        {
            Email = user.Email,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
            Image = user.Image,
            Dob = user.Dob.ToString()
        };
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), updateUserDto.UserId);
        return new ApiResult<UpdateResponseUserDto>(true, userResponseUpdate, $"Update User Successfully !");
    }

    public async Task<ApiResult<UserResponseDto>> GetUserByUserId(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new ApiResult<UserResponseDto>(false, "UserId is null or empty !!!");
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return new ApiResult<UserResponseDto>(false, "User not found !!!");
        }

        var result = _mapper.Map<UserResponseDto>(user);

        return new ApiSuccessResult<UserResponseDto>(result);
    }

    private async Task SendConfirmEmailAsync(string email, string token, CancellationToken cancellationToken)
    {
        var url = $"{_urlBase.ClientUrl}/{_verifyEmailSettings.VerifyEmailPath}?token={token}&email={email}";
        var emailRequest = new MailRequest()
        {
            ToAddress = email,
            Body = "<h4>Xác thực tài khoản</h4>" +
                   $"<h4>Chào bạn, {_verifyEmailSettings.ApplicationName} xin kính chào!</h4>" +
                   "<span>Vui lòng xác thực tài khoản của bạn bằng cách nhấp vào liên kết dưới đây: </span>" +
                   $"<span><a class=\"button\" href=\"{url}\" target=\"_blank\">Click here</a></span>" +
                   "<p>Nếu bạn không yêu cầu xác thực tài khoản, vui lòng bỏ qua email này.</p>" +
                   "<h4>Trân trọng,</h4>" +
                   $"<h4>{_verifyEmailSettings.ApplicationName}</h4>",
            Subject = $"Xin chào, Xác thực email của bạn !!!"
        };

        try
        {
            await _emailService.SendEmailAsync(emailRequest, cancellationToken);
            _logger.Information($"Verify your email {email}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Verify your email {email} failed due to an error with the email service: {ex.Message}");
        }
    }

    private async Task SendForgotPasswordEmail(ApplicationUser user, string token, CancellationToken cancellationToken)
    {
        var url = $"{_urlBase.ClientUrl}/{_verifyEmailSettings.ResetPasswordPath}?token={token}&email={user.Email}";
        var emailRequest = new MailRequest()
        {
            ToAddress = user.Email,
            Body = $"<h4>Xin chào {user.FirstName} {user.LastName}</h4>" +
                   $"<p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn với tên đăng nhập: {user.UserName}.</p>" +
                   "<span>Để tiến hành đặt lại mật khẩu, vui lòng nhấp vào liên kết dưới đây: </span>" +
                   $"<span><a class=\"button\" href=\"{url}\" target=\"_blank\">Click here</a></span>" +
                   "<p>Nếu bạn không yêu cầu thay đổi mật khẩu, hãy bỏ qua email này.</p>" +
                   "<h4>Trân trọng,</h4>" +
                   $"<h4>{_verifyEmailSettings.ApplicationName}</h4>",
            Subject = $"Xin chào, Đặt lại mật khẩu tài khoản của bạn !!!"
        };

        try
        {
            await _emailService.SendEmailAsync(emailRequest, cancellationToken);
            _logger.Information($"Forgot Password your email {user.Email}");
        }
        catch (Exception ex)
        {
            _logger.Error(
                $"Forgot Password your email {user.Email} failed due to an error with the email service: {ex.Message}");
        }
    }
}