using LW.Data.Entities;
using LW.Shared.Constant;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.Member;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.AdminServices;

public interface IAdminAuthorService
{
    // login - register
    public  Task<ApiResult<RegisterMemberResponseDto>> RegisterMemberAsync(RegisterMemberDto model);
    public Task<ApiResult<LoginAdminResponseDto>> LoginAdminAsync(LoginAdminDto model);
    // manage account
    Task<ApiResult<PagedList<MemberDto>>> GetAllMemberByRolePagination(string? role,PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<MemberDto>>> SearchMemberByRolePagination(string? role, SearchRequestValue searchRequestValue);
    public Task<ApiResult<bool>> AssignRoleAsync(string email, string roleName);
    public Task<ApiResult<IEnumerable<string>>> AssignMultiRoleAsync(AssignMultipleRoleDto assignMultipleRoleDto);
    public Task<ApiResult<IEnumerable<string>>> GetAllRoleOfUserAsync(string userId);
    public Task<ApiResult<bool>> DeleteAsync(string userId);
    public Task<ApiResult<bool>> LockMemberAsync(string userId);
    public Task<ApiResult<bool>> UnLockMemberAsync(string userId);
    public Task<ApiResult<IEnumerable<string>>> GetApplicationRolesAsync();
    public Task<ApiResult<AdminDto>> GetByUserIdAsync(string userId);
    public Task<ApiResult<AdminDto>> GetByEmailAsync(string email);
    // manage profile
    public Task<ApiResult<UpdateAdminDto>> UpdateAdminAsync(UpdateAdminDto updateAdminDto);
    public Task<ApiResult<bool>> ChangePasswordAsync(ChangePasswordAdminDto changePasswordAdminDto);
    public Task<ApiResult<bool>> ForgotPasswordAsync(string email);
    public Task<ApiResult<bool>> ResetPasswordAsync(ResetPasswordAdminDto resetPasswordAdminDto);
    // manage role
    public  Task<ApiResult<bool>> CreateRoleAsync(string roleName);
    public Task<ApiResult<bool>> UpdateRoleAsync(string roleId, string newRoleName);
    public Task<ApiResult<bool>> DeleteRoleAsync(string roleId);
    public Task<ApiResult<RoleDto>> GetRoleByIdAsync(string roleId);
    public Task<ApiResult<IEnumerable<RoleDto>>> GetAllRolesAsync();

}