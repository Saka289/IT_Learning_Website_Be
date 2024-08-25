using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.UserGradeValidator;
using LW.Data.Entities;
using LW.Services.UserGradeServices;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserGradeController : ControllerBase
    {
        private readonly IUserGradeService _userGradeService;
        public UserGradeController(IUserGradeService userGradeService)
        {
            _userGradeService = userGradeService;
        }
        
        [HttpGet("GetAllUserGrade")]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<UserGradeDto>>> GetUserGradeById(int id)
        {
            var result = await _userGradeService.GeUserGradeById(id);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        
        [HttpGet("GetUserGradeByUserId/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<UserGradeDto>>> GetUserGradeByUserId(string userId)
        {
            var result = await _userGradeService.GetAllUserGradeByUserId(userId);
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
        
        [HttpPost("UpdateRangeUserGrade")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateRangeUserGrade(UserGradeCreateRangeDto models)
        {
            var listUserGradeDto = new List<UserGradeCreateDto>();
            if (models.GradeIds.Count() == 0)
            {
                return BadRequest("Need to select the class you are interested in");
            }
            foreach (var x in models.GradeIds)
            {
                listUserGradeDto.Add(new UserGradeCreateDto()
                {
                    UserId = models.UserId,
                    GradeId = x
                });
            }

            await _userGradeService.DeleteRangeUserGrade(models.UserId);
            
            var result = await _userGradeService.CreatRangeUserGrade(listUserGradeDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        [HttpPost("CreateRangeUserGrade")]
        public async Task<ActionResult<ApiResult<bool>>> CreateRangeUserGrade(UserGradeCreateRangeDto models)
        {
            var listUserGradeDto = new List<UserGradeCreateDto>();
            if (models.GradeIds.Count() == 0)
            {
                return BadRequest("Need to select the class you are interested in");
            }
            foreach (var x in models.GradeIds)
            {
                listUserGradeDto.Add(new UserGradeCreateDto()
                {
                    UserId = models.UserId,
                    GradeId = x
                });
            }
            var result = await _userGradeService.CreatRangeUserGrade(listUserGradeDto);
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
        
        [HttpDelete("DeleteRangeUserGrade/{userId}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteRangeUserGrade(string userId)
        {
            var result = await _userGradeService.DeleteRangeUserGrade(userId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
