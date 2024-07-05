using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.ExamValidator;
using LW.API.Application.Validators.GradeValidator;
using LW.Services.ExamServices;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet("GetAllExam")]
        public async Task<ActionResult<ApiResult<IEnumerable<ExamDto>>>> GetAllExam()
        {
            var result = await _examService.GetAllExam();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }


        [HttpGet("GetAllExamPagination")]
        public async Task<ActionResult<ApiResult<PagedList<ExamDto>>>> GetAllExamPagination(
            [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _examService.GetAllExamPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetExamById/{id}")]
        public async Task<ActionResult<ApiResult<ExamDto>>> GetExamById( int id)
        {
            var result = await _examService.GetExamById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("SearchByExamPagination")]
        public async Task<ActionResult<ApiResult<PagedList<ExamDto>>>> SearchByExamPagination(
            [FromQuery] SearchExamDto searchExamDto)
        {
            var result = await _examService.SearchByExamPagination(searchExamDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpPost("CreateExam")]
        public async Task<ActionResult<ApiResult<GradeDto>>> CreateExam([FromForm] ExamCreateDto examCreateDto)
        {
            var validationResult = await new CreateExamCommandValidator().ValidateAsync(examCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examService.CreateExam(examCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateExam")]
        public async Task<ActionResult<ApiResult<ExamDto>>> UpdateExam([FromForm] ExamUpdateDto examUpdateDto)
        {
            var validationResult = await new UpdateExamCommandValidator().ValidateAsync(examUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _examService.UpdateExam(examUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateStatusExam")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusExam(int id)
        {
            var result = await _examService.UpdateExamStatus(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteExam/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteExam( int id)
        {
            var result = await _examService.DeleteExam(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}

