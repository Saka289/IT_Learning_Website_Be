using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.LessonValidator;
using LW.Services.LessonService;
using LW.Shared.DTOs.Lesson;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        
        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("GetAllLesson")]
        public async Task<ActionResult<ApiResult<IEnumerable<LessonDto>>>> GetAllLesson()
        {
            var result = await _lessonService.GetAllLesson();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        
        [HttpGet("GetAllLessonPagination")]
        public async Task<ActionResult<ApiResult<PagedList<LessonDto>>>> GetAllLessonPagination(
            [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _lessonService.GetAllLessonPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetLessonById/{id}")]
        public async Task<ActionResult<ApiResult<LessonDto>>> GetLessonById([Required] int id)
        {
            var result = await _lessonService.GetLessonById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
        
        [HttpGet("SearchByLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> SearchByLesson([FromQuery] SearchLessonDto searchLessonDto)
        {
            var result = await _lessonService.SearchByLesson(searchLessonDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("CreateLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> CreateLesson([FromForm] LessonCreateDto lessonCreateDto)
        {
            var validationResult = await new CreateLessonCommandValidator().ValidateAsync(lessonCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _lessonService.CreateLesson(lessonCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        [HttpPut("UpdateLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> UpdateLesson([FromForm] LessonUpdateDto lessonUpdateDto)
        {
            var validationResult = await new UpdateLessonCommandValidator().ValidateAsync(lessonUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _lessonService.UpdateLesson(lessonUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        [HttpPut("UpdateStatusLesson")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusLesson(int id)
        {
            var result = await _lessonService.UpdateLessonStatus(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }

        [HttpDelete("DeleteLesson/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteLesson([Required] int id)
        {
            var result = await _lessonService.DeleteLesson(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
    }
}
