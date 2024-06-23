using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Services.AdminServices;
using LW.Shared.DTOs.Admin;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LW.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminAuthorService _adminAuthorService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICloudinaryService _cloudinaryService;

        public AdminController(IAdminAuthorService adminAuthorService, UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, ICloudinaryService cloudinaryService)
        {
            _adminAuthorService = adminAuthorService;
            _userManager = userManager;
            _signInManager = signInManager;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResult<RegisterAdminResponseDto>>> Register([FromBody] RegisterAdminDto registerAdminDto)
        {
            var result = await _adminAuthorService.RegisterAdminAsync(registerAdminDto);
            return Ok(result);
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<ApiResult<RegisterAdminResponseDto>>> Login([FromBody] LoginAdminDto loginAdminDto)
        {
            var result = await _adminAuthorService.LoginAdminAsync(loginAdminDto);
            return Ok(result);
        }
       
        [HttpPost("assignRole/{email}/{roleName}")]
        public async Task<ActionResult<ApiResult<bool>>> AssignRoleToEmail(string email, string roleName)
        {
            var result = await _adminAuthorService.AssignRoleAsync(email, roleName);
            return Ok(result);
        }
        
        [HttpPut("update")]
        public async Task<ActionResult<ApiResult<UpdateAdminDto>>> UpdateAdmin([FromForm] UpdateAdminDto updateAdminDto)
        {
            var result = await _adminAuthorService.UpdateAdminAsync(updateAdminDto);
            return Ok(result);
        }
        [HttpDelete("deleteMember")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteMember(string UserId)
        {
            var result = await _adminAuthorService.DeleteAsync(UserId);
            return Ok(result);
        }
        
        [HttpPost("lockMember")]
        public async Task<ActionResult<ApiResult<bool>>> LockMember(string UserId)
        {
            var result = await _adminAuthorService.LockMemberAsync(UserId);
            return Ok(result);
        }
        [HttpPost("unlockMember")]
        public async Task<ActionResult<ApiResult<bool>>> UnlockMember(string UserId)
        {
            var result = await _adminAuthorService.UnLockMemberAsync(UserId);
            return Ok(result);
        }
        
        [HttpPut]
        public async Task<ActionResult<ApiResult<bool>>> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var result = await _adminAuthorService.UpdateRoleAsync(updateRoleDto);
            return Ok(result);
        }

        [HttpGet("getAllRoles")] 
        public async Task<ActionResult<ApiResult<List<string>>>> GetAllRoles()
        {
            var result = await _adminAuthorService.GetApplicationRolesAsync();
            return Ok(result);
        }
        [HttpGet("getMemberById")]
        public async Task<ActionResult<ApiResult<AdminDto>>> GetMemberById(string UserId)
        {
            var result = await _adminAuthorService.GetByUserIdAsync(UserId);
            return Ok(result);
        }
        [HttpGet("getMemberByEmail")]
        public async Task<ActionResult<ApiResult<AdminDto>>> GetMemberByEmail(string Email)
        {
            var result = await _adminAuthorService.GetByEmailAsync(Email);
            return Ok(result);
        }
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResult<bool>>> ChangePassword([FromBody] ChangePasswordAdminDto changePasswordAdminDto)
        {
            var result = await _adminAuthorService.ChangePasswordAsync(changePasswordAdminDto);
            return Ok(result);
        }   
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ApiResult<bool>>> ForgotPassword( string email)
        {
            var result = await _adminAuthorService.ForgotPasswordAsync(email);
            return Ok(result);
        }   
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ApiResult<bool>>> ResetPassword( [FromBody] ResetPasswordAdminDto resetPasswordAdminDto)
        {
            var result = await _adminAuthorService.ResetPasswordAsync(resetPasswordAdminDto);
            return Ok(result);
        }   
    }
}