using System.Text;
using AutoMapper;
using LW.Contracts.Common;
using LW.Contracts.Services;
using LW.Data.Entities;
using LW.Infrastructure.Extensions;
using LW.Services.Common.CommonServices.JwtTokenServices;
using LW.Services.Common.ModelMapping;
using LW.Shared.Configurations;
using LW.Shared.Constant;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.Member;
using LW.Shared.SeedWork;
using LW.Shared.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Serilog;

namespace LW.Services.AdminServices;

public class AdminAuthorService : IAdminAuthorService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UrlBase _urlBase;
    private readonly VerifyEmailSettings _verifyEmailSettings;
    private readonly ISmtpEmailService _emailService;
    private readonly ILogger _logger;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IElasticSearchService<MemberDto, string> _elasticSearchService;

    public AdminAuthorService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IOptions<UrlBase> urlBase,
        IOptions<VerifyEmailSettings> verifyEmailSettings,
        ILogger logger,
        ISmtpEmailService emailService,
        IMapper mapper, IJwtTokenService jwtTokenService, ICloudinaryService cloudinaryService,
        IElasticSearchService<MemberDto, string> elasticSearchService, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _urlBase = urlBase.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _verifyEmailSettings = verifyEmailSettings.Value;
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _cloudinaryService = cloudinaryService;
        _elasticSearchService = elasticSearchService;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
    }

    public async Task<ApiResult<RegisterMemberResponseDto>> RegisterMemberAsync(RegisterMemberDto model)
    {
        if (await CheckEmailExistsAsync(model.Email))
        {
            return new ApiResult<RegisterMemberResponseDto>(false, new RegisterMemberResponseDto(),
                "Email existed");
        }

        ApplicationUser user = new ApplicationUser()
        {
            UserName = model.Username,
            Email = model.Email,
            NormalizedEmail = model.Email.ToLower(),
            FirstName = model.FistName,
            LastName = model.LastName,
            EmailConfirmed = true,
            Image = CloudinaryConstant.Avatar,
            PublicId = CloudinaryConstant.AvatarPublicKey
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

        var adminDto = _mapper.Map<RegisterMemberResponseDto>(user);
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), a => a.Id);
        return new ApiResult<RegisterMemberResponseDto>(true, adminDto, "Register successfully");
    }

    public async Task<ApiResult<LoginAdminResponseDto>> LoginAdminAsync(LoginAdminDto model)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(model.Email));
        if (user == null)
        {
            return new ApiResult<LoginAdminResponseDto>(false, "Invalid Email !!!");
        }

        var isRoles = await _userManager.IsInRoleAsync(user, RoleConstant.RoleAdmin);
        if (!isRoles)
        {
            return new ApiResult<LoginAdminResponseDto>(false, "User is not an Admin !!!");
        }

        var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
        if (checkPassword.IsLockedOut)
        {
            return new ApiResult<LoginAdminResponseDto>(false,
                $"Your account has been locked. You should wait until {user.LockoutEnd} (UTC time) to be able to login");
        }

        if (!checkPassword.Succeeded)
        {
            return new ApiResult<LoginAdminResponseDto>(false,
                "The password you entered is incorrect. Please try again.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        AdminDto adminDto = new()
        {
            ID = user.Id,
            Email = user.Email,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
        LoginAdminResponseDto loginAdminResponseDto = new()
        {
            Admin = adminDto,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        return new ApiResult<LoginAdminResponseDto>(true, loginAdminResponseDto, "Login Admin successfully !!!");
    }

    public async Task<ApiResult<PagedList<MemberDto>>> GetAllMemberByRolePagination(SearchAdminDto searchAdminDto)
    {
        var user = new List<MemberDto>();
        if (!string.IsNullOrEmpty(searchAdminDto.Value))
        {
            var userSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticUsers,
                new SearchRequestValue
                {
                    Value = searchAdminDto.Value,
                    Size = searchAdminDto.Size,
                });
            if (userSearch is null)
            {
                return new ApiResult<PagedList<MemberDto>>(false, "User not found !!!");
            }

            user = userSearch.ToList();
        }
        else
        {
            user = await _userManager.Users.Select(u => u.ToMemberDto(_userManager)).ToListAsync();
            if (!user.Any())
            {
                return new ApiResult<PagedList<MemberDto>>(false, "User not found !!!");
            }
        }

        if (!string.IsNullOrEmpty(searchAdminDto.Role))
        {
            user = user.Where(u => u.Roles.Any(r => r.ToLower().Trim().Equals(searchAdminDto.Role.ToLower().Trim())))
                .ToList();
        }

        var pagedResult = await PagedList<MemberDto>.ToPageListAsync(user.AsQueryable().BuildMock(),
            searchAdminDto.PageIndex, searchAdminDto.PageSize, searchAdminDto.OrderBy, searchAdminDto.IsAscending);
        return new ApiSuccessResult<PagedList<MemberDto>>(pagedResult);
    }

    public async Task<ApiResult<bool>> AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<bool>(false, "Don't find user with userId " + userId);
        }
        
        if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
        }

        await _userManager.AddToRoleAsync(user, roleName);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), user.Id);
        return new ApiResult<bool>(true, $"Assign {roleName} to user with userId {userId} successfully !");
    }

    public async Task<ApiResult<IEnumerable<string>>> AssignMultiRoleAsync(AssignMultipleRoleDto assignMultipleRoleDto)
    {
        var user = await _userManager.FindByIdAsync(assignMultipleRoleDto.UserId);
        if (user == null)
        {
            return new ApiResult<IEnumerable<string>>(false,
                $"User Not Found !");
        }

        // get all role of user 
        var oldRoleName = (await _userManager.GetRolesAsync(user)).ToArray();

        // If present in OldRoleName but not in new Roles, those are the roles that need to be deleted
        var deleteRole = oldRoleName.Where(r => !assignMultipleRoleDto.Roles.Contains(r));

        // If there are in new Roles but not in oldRoleName, then those are the Roles that need to be added
        var addRole = assignMultipleRoleDto.Roles.Where(r => !oldRoleName.Contains(r));

        // thuc hien xoa cac role o trong delete Role
        var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRole);
        if (!resultDelete.Succeeded)
        {
            return new ApiResult<IEnumerable<string>>(false, "Error when delete old roles");
        }

        var resultAdd = await _userManager.AddToRolesAsync(user, addRole);
        if (!resultDelete.Succeeded)
        {
            return new ApiResult<IEnumerable<string>>(false, "Error when add new roles");
        }

        var roleOfUserAfterUpdate = (await _userManager.GetRolesAsync(user)).ToArray();
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager),
            user.Id);
        return new ApiResult<IEnumerable<string>>(true, roleOfUserAfterUpdate,
            $"Assign multi roles for user with id = {assignMultipleRoleDto.UserId} ");
    }

    public async Task<ApiResult<IEnumerable<string>>> GetAllRoleOfUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<IEnumerable<string>>(false,
                $"User Not Found !");
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToArray();
        if (!roles.Any())
        {
            return new ApiResult<IEnumerable<string>>(false, "Not found roles");
        }

        return new ApiResult<IEnumerable<string>>(true, roles, "Get success");
    }

    public async Task<ApiResult<UpdateAdminDto>> UpdateAdminAsync(UpdateAdminDto updateAdminDto)
    {
        var user = await _userManager.FindByIdAsync(updateAdminDto.UserId);
        if (user == null)
        {
            return new ApiResult<UpdateAdminDto>(false,
                $"User Not Found !");
        }

        user.FirstName = updateAdminDto.FirstName;
        user.LastName = updateAdminDto.LastName;
        user.PhoneNumber = updateAdminDto.PhoneNumber;
        user.Dob = DateOnly.FromDateTime(updateAdminDto.Dob);

        //upload image to cloudinary
        if (updateAdminDto.Image != null && updateAdminDto.Image.Length > 0)
        {
            if (user.Image == null)
            {
                var createImage =
                    await _cloudinaryService.CreateImageAsync(updateAdminDto.Image, CloudinaryConstant.FolderUserImage);
                if (createImage == null)
                {
                    return new ApiResult<UpdateAdminDto>(false,
                        $"Upload Image Fail !");
                }

                user.PublicId = createImage.PublicId;
                user.Image = createImage.Url;
            }
            else
            {
                var updateImage = await _cloudinaryService.UpdateImageAsync(user.PublicId, updateAdminDto.Image);
                if (updateImage == null)
                {
                    return new ApiResult<UpdateAdminDto>(false,
                        $"Upload Image Fail !");
                }

                user.PublicId = updateImage.PublicId;
                user.Image = updateImage.Url;
            }
        }

        await _userManager.UpdateAsync(user);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager),
            updateAdminDto.UserId);
        return new ApiResult<UpdateAdminDto>(true, updateAdminDto, $"Update Successfully !");
    }

    public async Task<ApiResult<bool>> DeleteAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            return new ApiResult<bool>(true, $"Delete Successfully !");
        }

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticUsers, userId);
        return new ApiResult<bool>(false, $"User Not Found !");
    }

    public async Task<ApiResult<bool>> LockMemberAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<bool>(false, $"User Not Found !");
        }

        await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(30));
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager), userId);
        return new ApiResult<bool>(true, true, $"LockMember Successfully !");
    }

    public async Task<ApiResult<bool>> UnLockMemberAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<bool>(false, $"User Not Found !");
        }

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticUsers, user.ToMemberDto(_userManager),
            userId);
        return new ApiResult<bool>(true, true, $"UnLockMember Successfully !");
    }

    public async Task<ApiResult<IEnumerable<RoleDto>>> GetApplicationRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return new ApiResult<IEnumerable<RoleDto>>(true, _mapper.Map<IEnumerable<RoleDto>>(roles),
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

    //Role manage
    public async Task<ApiResult<bool>> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return new ApiResult<bool>(false, "Role already exists");
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (result.Succeeded)
        {
            return new ApiResult<bool>(true, "Role created successfully");
        }

        return new ApiResult<bool>(false, result.Errors.FirstOrDefault()?.Description ?? "Failed to create role");
    }

    public async Task<ApiResult<bool>> UpdateRoleAsync(string roleId, string newRoleName)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return new ApiResult<bool>(false, "Role not found");
        }

        role.Name = newRoleName;
        var result = await _roleManager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return new ApiResult<bool>(true, "Role updated successfully");
        }

        return new ApiResult<bool>(false, result.Errors.FirstOrDefault()?.Description ?? "Fail to update role");
    }

    public async Task<ApiResult<bool>> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return new ApiResult<bool>(false, "Role not found");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return new ApiResult<bool>(true, "Delete role successfully");
        }

        return new ApiResult<bool>(false, result.Errors.FirstOrDefault()?.Description ?? "Fail to delete role");
    }

    public async Task<ApiResult<IEnumerable<RoleDto>>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
        return new ApiResult<IEnumerable<RoleDto>>(true, roleDtos, "Roles retrieved successfully");
    }

    public async Task<ApiResult<RoleDto>> GetRoleByIdAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return new ApiResult<RoleDto>(false, null, "Role not found");
        }

        var roleDto = _mapper.Map<RoleDto>(role);
        return new ApiResult<RoleDto>(true, roleDto, "Role retrieved successfully");
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