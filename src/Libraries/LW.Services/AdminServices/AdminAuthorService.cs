using AutoMapper;
using LW.Data.Entities;
using LW.Shared.DTOs.Admin;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LW.Services.AdminServices;

public class AdminAuthorService : IAdminAuthorService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;


    public AdminAuthorService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
        RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _configuration = configuration;
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
            LoginAdminResponseDto responseDto = new LoginAdminResponseDto()
            {
                Admin = null,
                Token = ""
            };
            return new ApiResult<LoginAdminResponseDto>(false, responseDto,
                "The password you entered is incorrect. Please try again.");
        }


        var roles = await _userManager.GetRolesAsync(user);
        //var token = _jwtTokenGenerator.GenerateToken(user, roles);

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
            Token = "Tí nữa có jwt thì nhận được",
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
            return new ApiResult<bool>(true, true,
                $"Assign {roleName} to user with email {email} successfully !");
        }

        return new ApiResult<bool>(false, false,
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
            return new ApiResult<UpdateAdminDto>(false, null,
                $"An existing account is using {updateAdminDto.Email}, email address. Please try with another email address");
        }

        var user = await _userManager.FindByIdAsync(updateAdminDto.UserId);
        if (user == null)
        {
            return new ApiResult<UpdateAdminDto>(false, null,
                $"User Not Found !");
        }

        user.FirstName = updateAdminDto.FirstName;
        user.LastName = updateAdminDto.LastName;
        user.PhoneNumber = updateAdminDto.PhoneNumber;
        user.Email = updateAdminDto.Email;
        user.UserName = updateAdminDto.UserName;

        await _userManager.UpdateAsync(user);

        return new ApiResult<UpdateAdminDto>(false, updateAdminDto,
            $"Update Successfully !");
    }

    public async Task<ApiResult<bool>> DeleteAsync(string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            return new ApiResult<bool>(true, true,
                $"Delete Successfully !");
        }

        return new ApiResult<bool>(false, false,
            $"User Not Found !");
    }

    public async Task<ApiResult<bool>> LockMemberAsync(string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(30));
            return new ApiResult<bool>(true, true,
                $"LockMember Successfully !");
        }

        return new ApiResult<bool>(false, false,
            $"User Not Found !");
    }

    public async Task<ApiResult<bool>> UnLockMemberAsync(string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            return new ApiResult<bool>(true, true,
                $"UnLockMember Successfully !");
        }

        return new ApiResult<bool>(false, false,
            $"User Not Found !");
    }

    public async Task<ApiResult<List<string>>> GetApplicationRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
        return new ApiResult<List<string>>(true, _mapper.Map<List<string>>(roles),
            $"Get Roles Successfully !");
    }

    public async Task<ApiResult<AdminDto>> GetByUserIdAsync(string UserId)
    {
        var user = await _userManager.FindByIdAsync(UserId);
        if (user != null)
        {
            var result = _mapper.Map<AdminDto>(user);
            
            return new ApiResult<AdminDto>(true, result,
                $"Get User By Id Successfully !");
        }

        return new ApiResult<AdminDto>(false, null,
            $"User Not Found !");
    }

    public async Task<ApiResult<AdminDto>> GetByEmailAsync(string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);
        if (user != null)
        {
            var result = _mapper.Map<AdminDto>(user);
            return new ApiResult<AdminDto>(true, result,
                $"Get User By Email Successfully !");
        }
        return new ApiResult<AdminDto>(false, null,
            $"User Not Found !");    }
}