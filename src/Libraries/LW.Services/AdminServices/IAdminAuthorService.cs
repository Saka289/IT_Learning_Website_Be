using LW.Data.Entities;
using LW.Shared.DTOs.Admin;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.AdminServices;

public interface IAdminAuthorService
{
    public  Task<ApiResult<RegisterMemberResponseDto>> RegisterMemberAsync(RegisterMemberDto model);
    public Task<ApiResult<LoginAdminResponseDto>> LoginAdminAsync(LoginAdminDto model);
    public Task<ApiResult<bool>> AssignRoleAsync(string email, string roleName);
    public Task<ApiResult<UpdateAdminDto>> UpdateAdminAsync(UpdateAdminDto updateAdminDto);
    public Task<ApiResult<bool>> DeleteAsync(string userId);
    public Task<ApiResult<bool>> LockMemberAsync(string userId);
    public Task<ApiResult<bool>> UnLockMemberAsync(string userId);
    public Task<ApiResult<List<string>>> GetApplicationRolesAsync();
    public Task<ApiResult<AdminDto>> GetByUserIdAsync(string userId);
    public Task<ApiResult<AdminDto>> GetByEmailAsync(string email);
    public Task<ApiResult<bool>> ChangePasswordAsync(ChangePasswordAdminDto changePasswordAdminDto);
    public Task<ApiResult<bool>> ForgotPasswordAsync(string email);
    public Task<ApiResult<bool>> ResetPasswordAsync(ResetPasswordAdminDto resetPasswordAdminDto);
    public  Task<ApiResult<bool>> CreateRoleAsync(string roleName);
    public Task<ApiResult<bool>> UpdateRoleAsync(string roleId, string newRoleName);
    public Task<ApiResult<bool>> DeleteRoleAsync(string roleId);
    public Task<ApiResult<RoleDto>> GetRoleByIdAsync(string roleId);
    public Task<ApiResult<IEnumerable<RoleDto>>> GetAllRolesAsync();

}