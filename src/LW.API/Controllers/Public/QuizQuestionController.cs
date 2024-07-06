using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.QuizQuestionValidator;
using LW.Services.QuizQuestionServices;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizQuestionController : ControllerBase
    {
        private readonly IQuizQuestionService _quizQuestionService;

        public QuizQuestionController(IQuizQuestionService quizQuestionService)
        {
            _quizQuestionService = quizQuestionService;
        }

        [HttpGet("GetAllQuizQuestion")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> GetAllQuizQuestion()
        {
            var result = await _quizQuestionService.GetAllQuizQuestion();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllQuizQuestionByQuizId/{quizId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> GetAllQuizQuestionByQuizId([Required] int quizId)
        {
            var result = await _quizQuestionService.GetAllQuizQuestionByQuizId(quizId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetQuizQuestionById/{id}")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> GetQuizQuestionById([Required] int id)
        {
            var result = await _quizQuestionService.GetQuizQuestionById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpPost("CreateQuizQuestion")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateQuizQuestion([FromBody] QuizQuestionCreateDto quizQuestionCreateDto)
        {
            var validationResult = await new CreateQuizQuestionCommandValidator().ValidateAsync(quizQuestionCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _quizQuestionService.CreateQuizQuestion(quizQuestionCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateQuizQuestion")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> UpdateQuizQuestion([FromBody] QuizQuestionUpdateDto quizQuestionUpdateDto)
        {
            var validationResult = await new UpdateQuizQuestionCommandValidator().ValidateAsync(quizQuestionUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _quizQuestionService.UpdateQuizQuestion(quizQuestionUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPost("CreateRangeQuizQuestion")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateRangeQuizQuestion([FromBody] IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
        {
            var validator =  new CreateQuizQuestionCommandValidator();
            var validationResults =  quizQuestionsCreateDto.Select(async model => await validator.ValidateAsync(model));
            if (validationResults.Any(result => !result.IsCompleted))
            {
                return BadRequest(validationResults);
            }
            var result = await _quizQuestionService.CreateRangeQuizQuestion(quizQuestionsCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateRangeQuizQuestion")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> UpdateRangeQuizQuestion([FromBody] IEnumerable<QuizQuestionUpdateDto> quizQuestionsUpdateDto)
        {
            var validator =  new UpdateQuizQuestionCommandValidator();
            var validationResults =  quizQuestionsUpdateDto.Select(async model => await validator.ValidateAsync(model));
            if (validationResults.Any(result => !result.IsCompleted))
            {
                return BadRequest(validationResults);
            }
            var result = await _quizQuestionService.UpdateRangeQuizQuestion(quizQuestionsUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateStatusQuizQuestion")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusQuizQuestion([Required] int id)
        {
            var result = await _quizQuestionService.UpdateStatusQuizQuestion(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteQuizQuestion/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteQuiz([Required] int id)
        {
            var result = await _quizQuestionService.DeleteQuizQuestion(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
