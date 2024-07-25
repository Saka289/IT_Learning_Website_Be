using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Data.Entities;
using LW.Services.AdminServices;
using LW.Shared.DTOs.Admin;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IAdminAuthorService _adminAuthorService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RoleController(IAdminAuthorService adminAuthorService, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _adminAuthorService = adminAuthorService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("getAllRoles")]
        public async Task<ActionResult<ApiResult<IEnumerable<RoleDto>>>> GetAllRoles()
        {
            var result = await _adminAuthorService.GetAllRolesAsync();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("getRoleById/{id}")]
        public async Task<ActionResult<ApiResult<RoleDto>>> GetRoleById(string id)
        {
            var result = await _adminAuthorService.GetRoleByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("createRole/{roleName}")]
        public async Task<ActionResult<ApiResult<bool>>> CreateRole(string roleName)
        {
            var result = await _adminAuthorService.CreateRoleAsync(roleName);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("updateRole/{id}/{newRoleName}")]
        public async Task<ActionResult<ApiResult<bool>>> CreateRole(string id, string newRoleName)
        {
            var result = await _adminAuthorService.UpdateRoleAsync(id, newRoleName);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("deleteRole/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteRole(string id)
        {
            var result = await _adminAuthorService.DeleteRoleAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("AssignMultiRoleForUser")]
        public async Task<ActionResult<ApiResult<IEnumerable<string>>>> AssignMultiRoleForUser(AssignMultipleRoleDto assignMultipleRoleDto)
        {
            var result = await _adminAuthorService.AssignMultiRoleAsync(assignMultipleRoleDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}