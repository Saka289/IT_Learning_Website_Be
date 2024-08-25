using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.ExamAnswerValidator;
using LW.Services.ExamAnswerServices;
using LW.Shared.Constant;
using LW.Shared.DTOs;
using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
    public class ExamAnswerController : ControllerBase
    {
        private readonly IExamAnswerService _examAnswerService;

        public ExamAnswerController(IExamAnswerService examAnswerService)
        {
            _examAnswerService = examAnswerService;
        }

        [HttpGet("GetAllExamAnswerByExamCodeId/{examCodeId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<ExamAnswerDto>>>> GetAllExamAnswerByExamCodeId(int examCodeId)
        {
            var result = await _examAnswerService.GetExamAnswerByExamCodeId(examCodeId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetExamAnswerById/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<ExamAnswerDto>>> GetExamAnswerById(int id)
        {
            var result = await _examAnswerService.GetExamAnswerById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteExamAnswerById/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteExamAnswerById(int id)
        {
            var result = await _examAnswerService.DeleteExamAnswer(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateExamAnswer")]
        public async Task<ActionResult<ApiResult<ExamAnswerDto>>> CreateExamAnswer(
            [FromForm] ExamAnswerCreateDto examAnswerCreateDto)
        {
            var validationResult = await new CreateExamAnswerCommandValidator().ValidateAsync(examAnswerCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examAnswerService.CreateExamAnswer(examAnswerCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateRangeExamAnswer")]
        public async Task<ActionResult<ApiResult<bool>>> CreateRangeExamAnswer(
            [FromBody] ExamAnswerCreateRangeDto examAnswerCreateRangeDtos)
        {
            var validationResult =
                await new CreateRangeExamAnswerCommandValidator().ValidateAsync(examAnswerCreateRangeDtos);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examAnswerService.CreateRangeExamAnswer(examAnswerCreateRangeDtos);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPut("UpdateRangeExamAnswer")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateRangeExamAnswer(
            [FromBody] ExamAnswerUpdateRangeDto examAnswerCreateRangeDtos)
        {
            var validationResult =
                await new UpdateRangeExamAnswerCommandValidator().ValidateAsync(examAnswerCreateRangeDtos);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examAnswerService.UpdateRangeExamAnswer(examAnswerCreateRangeDtos);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateExamAnswer")]
        public async Task<ActionResult<ApiResult<ExamAnswerDto>>> UpdateExamAnswer(
            [FromForm] ExamAnswerUpdateDto examAnswerUpdateDto)
        {
            var validationResult = await new UpdateExamAnswerCommandValidator().ValidateAsync(examAnswerUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examAnswerService.UpdateExamAnswer(examAnswerUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}