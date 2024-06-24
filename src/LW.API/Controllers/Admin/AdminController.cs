using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.AdminValidator;
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

        public AdminController(IAdminAuthorService adminAuthorService)
        {
            _adminAuthorService = adminAuthorService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResult<RegisterMemberResponseDto>>> Register([FromBody] RegisterMemberDto registerMemberDto)
        {
            var validationResult = await new RegisterMemberCommandValidator().ValidateAsync(registerMemberDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _adminAuthorService.RegisterMemberAsync(registerMemberDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
       
        [HttpPost("assignRole/{email}/{roleName}")]
        public async Task<ActionResult<ApiResult<bool>>> AssignRoleToEmail(string email, string roleName)
        {
            var result = await _adminAuthorService.AssignRoleAsync(email, roleName);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        [HttpPut("update")]
        public async Task<ActionResult<ApiResult<UpdateAdminDto>>> UpdateAdmin([FromForm] UpdateAdminDto updateAdminDto)
        {
            var validationResult = await new UpdateAdminCommandValidator().ValidateAsync(updateAdminDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _adminAuthorService.UpdateAdminAsync(updateAdminDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("deleteMember")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteMember(string UserId)
        {
            var result = await _adminAuthorService.DeleteAsync(UserId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        [HttpPost("lockMember")]
        public async Task<ActionResult<ApiResult<bool>>> LockMember(string UserId)
        {
            var result = await _adminAuthorService.LockMemberAsync(UserId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("unlockMember")]
        public async Task<ActionResult<ApiResult<bool>>> UnlockMember(string UserId)
        {
            var result = await _adminAuthorService.UnLockMemberAsync(UserId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getAllRoles")] 
        public async Task<ActionResult<ApiResult<List<string>>>> GetAllRoles()
        {
            var result = await _adminAuthorService.GetApplicationRolesAsync();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("getMemberById")]
        public async Task<ActionResult<ApiResult<AdminDto>>> GetMemberById(string UserId)
        {
            var result = await _adminAuthorService.GetByUserIdAsync(UserId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("getMemberByEmail")]
        public async Task<ActionResult<ApiResult<AdminDto>>> GetMemberByEmail(string Email)
        {
            var result = await _adminAuthorService.GetByEmailAsync(Email);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResult<bool>>> ChangePassword([FromBody] ChangePasswordAdminDto changePasswordAdminDto)
        {
            var result = await _adminAuthorService.ChangePasswordAsync(changePasswordAdminDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }   
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ApiResult<bool>>> ForgotPassword( string email)
        {
            var result = await _adminAuthorService.ForgotPasswordAsync(email);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }   
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ApiResult<bool>>> ResetPassword( [FromBody] ResetPasswordAdminDto resetPasswordAdminDto)
        {
            var result = await _adminAuthorService.ResetPasswordAsync(resetPasswordAdminDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }   
    }
}