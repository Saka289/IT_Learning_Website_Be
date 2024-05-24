using LW.Data.Entities;
using LW.Shared.DTOs.Admin;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.AdminServices;

public interface IAdminAuthorService
{
    public  Task<ApiResult<RegisterAdminResponseDto>> RegisterAdminAsync(RegisterAdminDto model);
    public Task<ApiResult<LoginAdminResponseDto>> LoginAdminAsync(LoginAdminDto model);
    public Task<ApiResult<bool>> AssignRoleAsync(string email, string roleName);
    public Task<ApiResult<bool>> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
    public Task<ApiResult<UpdateAdminDto>> UpdateAdminAsync(UpdateAdminDto updateAdminDto);
    public Task<ApiResult<bool>> DeleteAsync(string UserId);
    public Task<ApiResult<bool>> LockMemberAsync(string UserId);
    public Task<ApiResult<bool>> UnLockMemberAsync(string UserId);
    public Task<ApiResult<List<string>>> GetApplicationRolesAsync();
    public Task<ApiResult<AdminDto>> GetByUserIdAsync(string UserId);
    public Task<ApiResult<AdminDto>> GetByEmailAsync(string Email);
    
    
    
}