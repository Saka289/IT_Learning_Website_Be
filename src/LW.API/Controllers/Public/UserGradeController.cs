using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.UserGradeValidator;
using LW.Services.UserGradeServices;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGradeController : ControllerBase
    {
        private readonly IUserGradeService _userGradeService;
        public UserGradeController(IUserGradeService userGradeService)
        {
            _userGradeService = userGradeService;
        }
        [HttpGet("GetAllUserGrade")]
        public async Task<ActionResult<ApiResult<IEnumerable<UserGradeDto>>>> GetAllUserGrade()
        {
            var result = await _userGradeService.GetAllUserGrade();
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("GetUserGradeById/{id}")]
        public async Task<ActionResult<ApiResult<UserGradeDto>>> GetUserGradeById(int id)
        {
            var result = await _userGradeService.GeUserGradeById(id);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost("CreateUserGrade")]
        public async Task<ActionResult<ApiResult<bool>>> CreateUserGrade(UserGradeCreateDto model)
        {
            var validationResult = await new CreateUserGradeCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _userGradeService.CreatUserGrade(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("CreateRangeUserGrade")]
        public async Task<ActionResult<ApiResult<bool>>> CreateRangeUserGrade(IEnumerable<UserGradeCreateDto> models)
        {
            foreach (var model in models)
            {
                var validationResult = await new CreateUserGradeCommandValidator().ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult);
                }
            }
            
            var result = await _userGradeService.CreatRangeUserGrade(models);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        [HttpPut("UpdateUserGrade")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateUserGrade(UserGradeUpdateDto model)
        {
            var result = await _userGradeService.UpdateUserGrade(model);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpDelete("DeleteUserGrade")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteUserGrade(int id)
        {
            var result = await _userGradeService.DeleteUserGrade(id);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        // [HttpDelete("DeleteRangeUserGrade")]
        // public async Task<ActionResult<ApiResult<bool>>> DeleteRangeUserGrade(IEnumerable<int> ids)
        // {
        //     var result = await _userGradeService.DeleteRangeUserGrade(ids);
        //     if (!result.IsSucceeded)
        //     {
        //         return BadRequest(result);
        //     }
        //     return Ok(result);
        // }
    }
}
