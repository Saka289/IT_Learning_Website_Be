using System.Text;
using AutoMapper;
using LW.Cache.Interfaces;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Entities;
using LW.Services.JwtTokenService;
using LW.Shared.Configurations;
using LW.Shared.Constant;
using LW.Shared.DTOs.Email;
using LW.Shared.DTOs.User;
using LW.Shared.SeedWork;
using LW.Shared.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;

namespace LW.Services.UserService;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ISmtpEmailService _emailService;
    private readonly VerifyEmailSettings _verifyEmailSettings;
    private readonly UrlBase _urlBase;
    private readonly IRedisCache<VerifyEmailTokenDto> _redisCacheService;
    private readonly ISerializeService _serializeService;
    private readonly IJwtTokenService _jwtTokenService;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper,
        ISmtpEmailService emailService, ILogger logger, IOptions<VerifyEmailSettings> verifyEmailSettings,
        IOptions<UrlBase> urlBase, IRedisCache<VerifyEmailTokenDto> redisCacheService,
        ISerializeService serializeService, IJwtTokenService jwtTokenService)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        _serializeService = serializeService ?? throw new ArgumentNullException(nameof(serializeService));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _urlBase = urlBase.Value;
        _verifyEmailSettings = verifyEmailSettings.Value;
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<ApiResult<RegisterResponseUserDto>> Register(RegisterUserDto registerUserDto)
    {
        var user = _mapper.Map<ApplicationUser>(registerUserDto);
        user.EmailConfirmed = true;
        var emailExist = await _userManager.Users.AnyAsync(x => x.Email.ToLower() == registerUserDto.Email.ToLower());
        if (emailExist)
        {
            return new ApiResult<RegisterResponseUserDto>(false,
                $"An existing account is using {registerUserDto.Email}");
        }

        var userNameExist =
            await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == registerUserDto.UserName.ToLower());
        if (userNameExist)
        {
            return new ApiResult<RegisterResponseUserDto>(false,
                $"An existing username is using {registerUserDto.UserName}");
        }

        var result = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
        {
            return new ApiResult<RegisterResponseUserDto>(false, result.Errors.FirstOrDefault().Description);
        }

        await _userManager.AddToRoleAsync(user, "user");
        var userDto = new RegisterResponseUserDto
        {
            ID = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
        return new ApiResult<RegisterResponseUserDto>(true, userDto, "Create User Successfully");
    }

    public async Task<ApiResult<LoginResponseUserDto>> Login(LoginUserDto loginUserDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.UserName.ToLower().Equals(loginUserDto.UserName) || u.Email.ToLower().Equals(loginUserDto.Email));
        if (user == null)
        {
            return new ApiResult<LoginResponseUserDto>(false, "Invalid UserName or Email !!!");
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!checkPassword)
        {
            return new ApiResult<LoginResponseUserDto>(false,
                "The password you entered is incorrect. Please try again.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user, roles);

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
            token = token,
        };
        return new ApiResult<LoginResponseUserDto>(true, loginResponseUserDto, "Login successfully !!!");
    }

    public async Task<ApiResult<LoginResponseUserDto>> LoginGoogle(LoginUserDto loginUserDto)
    {
        return new ApiResult<LoginResponseUserDto>();
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

        SendEmailAsync(email, token, new CancellationToken());

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
                await _redisCacheService.RemoveStringKey(result.Email);
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

    private async Task SendEmailAsync(string email, string token, CancellationToken cancellationToken)
    {
        var url = $"{_urlBase.ClientUrl}/{_verifyEmailSettings.VerifyEmailPath}?token={token}";
        var emailRequest = new MailRequest()
        {
            ToAddress = email,
            Body = $"<p>{_verifyEmailSettings.ApplicationName}</p>" +
                   "<p>Please verify your email address by clicking on the following link.</p>" +
                   $"<p><a href=\"{url}\">Click here</a></p>" +
                   "<p>Thank you,</p>" +
                   $"<br>{_verifyEmailSettings.ApplicationName}",
            Subject = $"Hello, Verify your email."
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
}