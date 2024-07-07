using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.QuizValidator;
using LW.Services.QuizServices;
using LW.Shared.DTOs.Quiz;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("GetAllQuiz")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizDto>>>> GetAllQuiz()
        {
            var result = await _quizService.GetAllQuiz();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllQuizPagination")]
        public async Task<ActionResult<ApiResult<PagedList<QuizDto>>>> GetAllQuizPagination([FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _quizService.GetAllQuizPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetAllQuizByTopicIdPagination/{topicId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizDto>>>> GetAllQuizByTopicIdPagination([Required] int topicId, [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _quizService.GetAllQuizByTopicIdPagination(topicId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetAllQuizByLessonIdPagination/{lessonId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizDto>>>> GetAllQuizByLessonId([Required] int lessonId, [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _quizService.GetAllQuizByLessonIdPagination(lessonId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetQuizById/{id}")]
        public async Task<ActionResult<ApiResult<QuizDto>>> GetQuizById([Required] int id)
        {
            var result = await _quizService.GetQuizById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateQuiz")]
        public async Task<ActionResult<ApiResult<QuizDto>>> CreateQuiz([FromBody] QuizCreateDto quizCreateDto)
        {
            var validationResult = await new CreateQuizCommandValidator().ValidateAsync(quizCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _quizService.CreateQuiz(quizCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateQuiz")]
        public async Task<ActionResult<ApiResult<QuizDto>>> UpdateQuiz([FromBody] QuizUpdateDto quizUpdateDto)
        {
            var validationResult = await new UpdateQuizCommandValidator().ValidateAsync(quizUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _quizService.UpdateQuiz(quizUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateStatusQuiz")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusQuiz([Required] int id)
        {
            var result = await _quizService.UpdateQuizStatus(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteQuiz/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteQuiz([Required] int id)
        {
            var result = await _quizService.DeleteQuiz(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}