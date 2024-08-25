using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.ExamCodeValidator;
using LW.Services.ExamCodeServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.ExamCode;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
    public class ExamCodeController : ControllerBase
    {
        private readonly IExamCodeService _examCodeService;
        public ExamCodeController(IExamCodeService examCodeService)
        {
            _examCodeService = examCodeService;
        }
        
        [HttpGet("GetAllExamCodeByExamId/{examId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<ExamCodeDto>>>> GetAllExamCodeByExamId(int examId)
        {
            var result = await _examCodeService.GetExamCodeByExamId(examId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetExamCodeById/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<ExamCodeDto>>> GetExamCodeById(int id)
        {
            var result = await _examCodeService.GetExamCodeById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteExamCodeById/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteExamCodeById(int id)
        {
            var result = await _examCodeService.DeleteExamCode(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateExamCode")]
        public async Task<ActionResult<ApiResult<ExamCodeDto>>> CreateExamCode(
            [FromForm] ExamCodeCreateDto examCodeCreateDto)
        {
            var validationResult = await new CreateExamCodeCommandValidator().ValidateAsync(examCodeCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examCodeService.CreateExamCode(examCodeCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateRangeExamCode/{examId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<ExamCodeDto>>>> CreateRangeExamCode(int examId, [FromForm] List<CodeDto> codeDto)
        {
            var examCodeCreateRangeDto = new ExamCodeCreateRangeDto()
            {
                ExamId = examId,
                CodeDtos = codeDto
            };
            var result = await _examCodeService.CreateRangeExamCode(examCodeCreateRangeDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateExamCode")]
        public async Task<ActionResult<ApiResult<ExamCodeDto>>> UpdateExamCode(
            [FromForm] ExamCodeUpdateDto examCodeUpdateDto)
        {
            var validationResult = await new UpdateExamCodeCommandValidator().ValidateAsync(examCodeUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examCodeService.UpdateExamCode(examCodeUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
