using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.QuizQuestionValidator;
using LW.Services.QuizQuestionServices;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;

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
        
        [HttpGet("GetAllQuizQuestionPagination")]
        public async Task<ActionResult<ApiResult<PagedList<QuizQuestionDto>>>> GetAllQuizQuestionPagination([FromQuery] SearchAllQuizQuestionDto searchAllQuizQuestionDto)
        {
            var result = await _quizQuestionService.GetAllQuizQuestionPagination(searchAllQuizQuestionDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        
        [HttpGet("GetAllQuizQuestionByQuizId")]
        public async Task<ActionResult<ApiResult<IEnumerable<object>>>> GetAllQuizQuestionByQuizId([FromQuery] SearchQuizQuestionDto searchQuizQuestionDto)
        {
            var result = await _quizQuestionService.GetAllQuizQuestionByQuizId(searchQuizQuestionDto);
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
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateQuizQuestion([FromForm] QuizQuestionCreateDto quizQuestionCreateDto)
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
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> UpdateQuizQuestion([FromForm] QuizQuestionUpdateDto quizQuestionUpdateDto)
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
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateRangeQuizQuestion([FromForm] IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
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
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> UpdateRangeQuizQuestion([FromForm] IEnumerable<QuizQuestionUpdateDto> quizQuestionsUpdateDto)
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


        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel([FromQuery] int checkData = 1)
        {
            byte[] excelData = await _quizQuestionService.ExportExcel(checkData, null);
            string fileName = $"Quiz-{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}.xlsx";
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        
        [HttpGet("ExportExcelFail/{id}")]
        public async Task<IActionResult> ExportExcelFail(string id)
        {
            byte[] excelData = await _quizQuestionService.ExportExcel(1, id);
            string fileName = $"Quiz-Fail-{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}.xlsx";
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        
         [HttpGet("ExportExcelResult/{id}")]
        public async Task<IActionResult> ExportExcelResult(string id)
        {
            byte[] excelData = await _quizQuestionService.ExportExcel(1, id);
            string fileName = $"Quiz-Result-{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}.xlsx";
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("ImportValidate")]
        public async Task<ActionResult<ApiResult<QuizQuestionImportParentDto>>> ImportValidate([FromForm] IFormFile fileImport, [Required] int quizId)
        {
            var imports = await _quizQuestionService.ImportExcel(fileImport);
            if (!imports.IsSucceeded)
            {
                return BadRequest(imports);
            }

            return StatusCode(200, imports);
        }

        [HttpGet("ImportDatabase/{id}/{quizId}")]
        public async Task<ActionResult<ApiResult<bool>>> ImportDatabase(string id, int quizId)
        {
            var imports = await _quizQuestionService.ImportDatabase(id, quizId);
            if (!imports.IsSucceeded)
            {
                return BadRequest(imports);
            }
            return Ok(imports);
        }
    }
}