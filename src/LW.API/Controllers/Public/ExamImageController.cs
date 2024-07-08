using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.ExamImageValidator;
using LW.Services.ExamImageServices;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Exam;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamImageController : ControllerBase
    {
        private readonly IExamImageService _examImageService;
        public ExamImageController(IExamImageService examImageService)
        {
            _examImageService = examImageService;
        }
        [HttpGet("GetAllExamImage")]
        public async Task<ActionResult<ApiResult<IEnumerable<ExamImageDto>>>> GetAllExamImage()
        {
            var result = await _examImageService.GetAllExamImage();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        [HttpGet("GetAllExamImageByExamId/{examId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<ExamImageDto>>>> GetAllExamImageByExamId(int examId)
        {
            var result = await _examImageService.GetExamImageByExamId(examId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        [HttpGet("GetExamImageById/{id}")]
        public async Task<ActionResult<ApiResult<ExamImageDto>>> GetExamImageById(int id)
        {
            var result = await _examImageService.GetExamImageById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        [HttpDelete("DeleteExamImageById/{id}")]
        public async Task<ActionResult<ApiResult<ExamImageDto>>> DeleteExamImageById(int id)
        {
            var result = await _examImageService.DeleteExamImage(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        [HttpPost("CreateExamImage")]
        public async Task<ActionResult<ApiResult<ExamImageDto>>> CreateExamImage([FromForm]ExamImageCreateDto examImageCreateDto)
        {
            var validationResult = await new CreateExamImageCommandValidator().ValidateAsync(examImageCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _examImageService.CreateExamImage(examImageCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPut("UpdateExamImage")]
        public async Task<ActionResult<ApiResult<ExamImageDto>>> UpdateExamImage([FromForm]ExamImageUpdateDto examImageUpdateDto)
        {
            var validationResult = await new UpdateExamImageCommandValidator().ValidateAsync(examImageUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _examImageService.UpdateExamImage(examImageUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
