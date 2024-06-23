using System.Text;
using AutoMapper;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Entities;
using LW.Services.JwtTokenService;
using LW.Shared.Configurations;
using LW.Shared.Constant;
using LW.Shared.DTOs.Admin;
using LW.Shared.SeedWork;
using LW.Shared.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace LW.Services.AdminServices;

public class AdminAuthorService : IAdminAuthorService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UrlBase _urlBase;
    private readonly VerifyEmailSettings _verifyEmailSettings;
    private readonly ISmtpEmailService _emailService;
    private readonly ILogger _logger;
    private readonly ICloudinaryService _cloudinaryService;

    public AdminAuthorService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IOptions<UrlBase> urlBase,
        IOptions<VerifyEmailSettings> verifyEmailSettings,
        ILogger logger,
        ISmtpEmailService emailService,
        IMapper mapper, IJwtTokenService jwtTokenService, ICloudinaryService cloudinaryService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _urlBase = urlBase.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _verifyEmailSettings = verifyEmailSettings.Value;
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _cloudinaryService = cloudinaryService;
        _emailService = emailService;
    }

    private async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
    }

    public async Task<ApiResult<RegisterAdminResponseDto>> RegisterAdminAsync(RegisterAdminDto model)
    {
        if (await CheckEmailExistsAsync(model.Email))
        {
            return new ApiResult<RegisterAdminResponseDto>(false, new RegisterAdminResponseDto(),
                "Email existed");
        }

        ApplicationUser user = new ApplicationUser()
        {
            UserName = model.Username,
            Email = model.Email,
            NormalizedEmail = model.Email.ToLower(),
            FirstName = model.FistName,
            LastName = model.LastName,
            EmailConfirmed = true
        };
        var addUser = await _userManager.CreateAsync(user, model.Password);
        if (addUser.Succeeded)
        {
            // check role exist??
            if (!await _roleManager.RoleExistsAsync(model.RoleString))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.RoleString));
            }

            //regis role to user
            await _userManager.AddToRoleAsync(user, model.RoleString);
        }

        var adminDto = _mapper.Map<RegisterAdminResponseDto>(user);
        return new ApiResult<RegisterAdminResponseDto>(true, adminDto,
            "Register successfully");
    }

    public async Task<ApiResult<LoginAdminResponseDto>> LoginAdminAsync(LoginAdminDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        bool isValid = await _userManager.CheckPasswordAsync(user, model.Password);
        //bool IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

        if (user == null || isValid == false)
        {
            return new ApiResult<LoginAdminResponseDto>(false,
                "The password you entered is incorrect. Please try again.");
        }


        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateAccessToken(user, roles);


        AdminDto adminDto = new()
        {
            Email = user.Email,
            ID = user.Id,
            Name = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
        };

        LoginAdminResponseDto loginResponseDto = new LoginAdminResponseDto()
        {
            Admin = adminDto,
            Token = token,
        };
        return new ApiResult<LoginAdminResponseDto>(true, loginResponseDto,
            "Login Successfully");
    }

    public async Task<ApiResult<bool>> AssignRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }

            await _userManager.AddToRoleAsync(user, roleName);
            return new ApiResult<bool>(true,
                $"Assign {roleName} to user with email {email} successfully !");
        }

        return new ApiResult<bool>(false,
            "Don't find user with email " + email);
    }

    public Task<ApiResult<bool>> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
    {
        throw new NotImplementedException();
    }


    public async Task<ApiResult<UpdateAdminDto>> UpdateAdminAsync(UpdateAdminDto updateAdminDto)
    {
        var checkEmail =
            await _userManager.Users.AnyAsync(
                x => x.Email.Equals(updateAdminDto.Email) && x.Id != updateAdminDto.UserId);
        if (checkEmail)
        {
            return new ApiResult<UpdateAdminDto>(false,
                $"An existing account is using {updateAdminDto.Email}, email address. Please try with another email address");
        }

        var user = await _userManager.FindByIdAsync(updateAdminDto.UserId);
        if (user == null)
        {
            return new ApiResult<UpdateAdminDto>(false,
                $"User Not Found !");
        }

        user.FirstName = updateAdminDto.FirstName;
        user.LastName = updateAdminDto.LastName;
        user.PhoneNumber = updateAdminDto.PhoneNumber;
        user.Email = updateAdminDto.Email;
        user.UserName = updateAdminDto.UserName;
        //upload image to cloudinary
        if (updateAdminDto.Image != null && updateAdminDto.Image.Length > 0)
        {
            if (user.Image == null)
            {
                var rs1 = _cloudinaryService.CreateImageAsync(updateAdminDto.Image, CloudinaryConstant.FolderUserImage);
                if (rs1 == null)
                {
                    return new ApiResult<UpdateAdminDto>(false,
                        $"Upload Image Fail !");
                }

                user.PublicId = rs1.Result.PublicId;
                user.Image = rs1.Result.Url;
            }
            else
            {
                var rs2 = _cloudinaryService.UpdateImageAsync(user.PublicId, updateAdminDto.Image);
                if (rs2 == null)
                {
                    return new ApiResult<UpdateAdminDto>(false,
                        $"Upload Image Fail !");
                }

                user.PublicId = rs2.Result.PublicId;
                user.Image = rs2.Result.Url;
            }
        }

        await _userManager.UpdateAsync(user);

        return new ApiResult<UpdateAdminDto>(false, updateAdminDto,
            $"Update Successfully !");
    }

    public async Task<ApiResult<bool>> DeleteAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            return new ApiResult<bool>(true,
                $"Delete Successfully !");
        }

        return new ApiResult<bool>(false,
            $"User Not Found !");
    }

    public async Task<ApiResult<bool>> LockMemberAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(30));
            return new ApiResult<bool>(true, true,
                $"LockMember Successfully !");
        }

        return new ApiResult<bool>(false,
            $"User Not Found !");
    }

    public async Task<ApiResult<bool>> UnLockMemberAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            return new ApiResult<bool>(true, true,
                $"UnLockMember Successfully !");
        }

        return new ApiResult<bool>(false,
            $"User Not Found !");
    }

    public async Task<ApiResult<List<string>>> GetApplicationRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
        return new ApiResult<List<string>>(true, _mapper.Map<List<string>>(roles),
            $"Get Roles Successfully !");
    }

    public async Task<ApiResult<AdminDto>> GetByUserIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = _mapper.Map<AdminDto>(user);

            return new ApiResult<AdminDto>(true, result,
                $"Get User By Id Successfully !");
        }

        return new ApiResult<AdminDto>(false,
            $"User Not Found !");
    }

    public async Task<ApiResult<AdminDto>> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = _mapper.Map<AdminDto>(user);
            return new ApiResult<AdminDto>(true, result,
                $"Get User By Email Successfully !");
        }

        return new ApiResult<AdminDto>(false,
            $"User Not Found !");
    }

    public async Task<ApiResult<bool>> ChangePasswordAsync(ChangePasswordAdminDto changePasswordAdminDto)
    {
        // find user by email
        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower().Equals(changePasswordAdminDto.Email.ToLower()));
        if (user == null)
        {
            return new ApiResult<bool>(false, "The email you entered is incorrect .");
        }

        // check pass
        var password = await _userManager.CheckPasswordAsync(user, changePasswordAdminDto.Password);
        if (password == false)
        {
            return new ApiResult<bool>(false, "The password you entered is incorrect.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, changePasswordAdminDto.NewPassword);
        if (!result.Succeeded)
        {
            return new ApiResult<bool>(false, result.Errors.FirstOrDefault().Description);
        }

        return new ApiResult<bool>(true, "Changed password successfully !!!");
    }

    public async Task<ApiResult<bool>> ForgotPasswordAsync(string email)
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

    private async Task SendForgotPasswordEmail(ApplicationUser user, string token, CancellationToken cancellationToken)
    {
        var url = $"{_urlBase.ClientUrl}/{_verifyEmailSettings.ResetPasswordPath}?token={token}&email={user.Email}";
        var emailRequest = new MailRequest()
        {
            ToAddress = user.Email,
            Body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
                   $"<p>Username: {user.UserName}.</p>" +
                   "<p>In order to reset your password, please click on the following link.</p>" +
                   $"<p><a href=\"{url}\">Click here</a></p>" +
                   "<p>Thank you,</p>" +
                   $"<br>{_verifyEmailSettings.ApplicationName}",
            Subject = $"Hello, Forgot Password your email."
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

    public async Task<ApiResult<bool>> ResetPasswordAsync(ResetPasswordAdminDto resetPasswordAdminDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordAdminDto.Email);
        if (user == null)
        {
            return new ApiResult<bool>(false, "This email address has not been registered yet");
        }

        var decodedTokenBytes = WebEncoders.Base64UrlDecode(resetPasswordAdminDto.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordAdminDto.NewPassword);
        if (!result.Succeeded)
        {
            return new ApiResult<bool>(false, result.Errors.FirstOrDefault().Description);
        }

        return new ApiResult<bool>(true, "Reset password successfully !!!");
    }
}