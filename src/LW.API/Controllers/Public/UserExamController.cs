using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.UserValidator;
using LW.Services.UserExamServices;
using LW.Shared.DTOs.User;
using LW.Shared.DTOs.UserExam;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserExamController : ControllerBase
    {
        private readonly IUserExamService _userExamService;

        public UserExamController(IUserExamService userExamService)
        {
            _userExamService = userExamService;
        }

        [HttpPost("SubmitExam")]
        public async Task<ActionResult<ApiResult<int>>> SubmitExam([FromBody] ExamFormSubmitDto examFormSubmitDto)
        {
            var result = await _userExamService.CreateRangeUserExam(examFormSubmitDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetUserExamById/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> GetUserExamById(int id)
        {
            var result = await _userExamService.GetExamResultById(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetListResultExamOfUserByUserId/{userId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<UserExamDto>>>> GetListResultExamOfUserByUserId(string  userId)
        {
            var result = await _userExamService.GetListResultByUserId(userId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}