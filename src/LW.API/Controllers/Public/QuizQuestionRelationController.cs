using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.QuizQuestionRelationValidator;
using LW.Services.QuizQuestionRelationServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.QuizQuestionRelation;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
    public class QuizQuestionRelationController : ControllerBase
    {
        private readonly IQuizQuestionRelationService _quizQuestionRelationService;

        public QuizQuestionRelationController(IQuizQuestionRelationService quizQuestionRelationService)
        {
            _quizQuestionRelationService = quizQuestionRelationService;
        }
        
        [HttpPost("CreateQuizQuestionRelationByQuizCustom")]
        public async Task<ActionResult<ApiResult<bool>>> CreateQuizQuestionRelationByQuizCustom([FromBody] QuizQuestionRelationCustomCreateDto quizQuestionRelationCustomCreateDto)
        {
            var validationResult = await new CreateQuizQuestionRelationCustomCreateCommandValidator().ValidateAsync(quizQuestionRelationCustomCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _quizQuestionRelationService.CreateQuizQuestionRelationByQuizCustom(quizQuestionRelationCustomCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateQuizQuestionRelation")]
        public async Task<ActionResult<ApiResult<bool>>> CreateQuizQuestionRelation([FromBody] QuizQuestionRelationCreateDto quizQuestionRelationCreateDto)
        {
            var validationResult = await new CreateQuizQuestionRelationCommandValidator().ValidateAsync(quizQuestionRelationCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _quizQuestionRelationService.CreateQuizQuestionRelation(quizQuestionRelationCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateQuizQuestionRelation")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateQuizQuestionRelation([FromBody] QuizQuestionRelationUpdateDto quizQuestionRelationUpdateDto)
        {
            var validationResult = await new UpdateQuizQuestionRelationCommandValidator().ValidateAsync(quizQuestionRelationUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _quizQuestionRelationService.UpdateQuizQuestionRelation(quizQuestionRelationUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteQuizQuestionRelation/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteQuizQuestionRelation([Required] int id)
        {
            var result = await _quizQuestionRelationService.DeleteQuizQuestionRelation(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteRangeQuizQuestionRelation")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteRangeQuizQuestionRelation([Required] IEnumerable<int> ids)
        {
            var result = await _quizQuestionRelationService.DeleteRangeQuizQuestionRelation(ids);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
