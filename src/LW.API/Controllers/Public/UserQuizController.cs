using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.UserQuizValidator;
using LW.Services.UserQuizServices;
using LW.Shared.DTOs.UserQuiz;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserQuizController : ControllerBase
    {
        private readonly IUserQuizService _userQuizService;

        public UserQuizController(IUserQuizService userQuizService)
        {
            _userQuizService = userQuizService;
        }

        [HttpPost("SubmitQuiz")]
        public async Task<ActionResult<ApiResult<UserQuizDto>>> SubmitQuiz([FromBody]UserQuizSubmitDto userQuizSubmitDto)
        {
            var validationResult = await new SubmitUserQuizCommandValidator().ValidateAsync(userQuizSubmitDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _userQuizService.SubmitQuiz(userQuizSubmitDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAllUserQuizByUserId/{userId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<UserQuizDto>>>> GetAllUserQuizByUserId([Required] string userId)
        {
            var result = await _userQuizService.GetAllUserQuizByUserId(userId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteUserQuizById/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteUserQuizById([Required] int id)
        {
            var result = await _userQuizService.DeleteUserQuizById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
