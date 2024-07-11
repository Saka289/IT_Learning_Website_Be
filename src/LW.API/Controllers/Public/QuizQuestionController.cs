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
        public async Task<ActionResult<ApiResult<PagedList<QuizQuestionDto>>>> GetAllQuizQuestionPagination([FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _quizQuestionService.GetAllQuizQuestionPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        
        [HttpGet("GetAllQuizQuestionByQuizIdPractice/{quizId}")]
        public async Task<ActionResult<ApiResult<PagedList<QuizQuestionDto>>>> GetAllQuizQuestionByQuizIdPractice([Required] int quizId)
        {
            var result = await _quizQuestionService.GetAllQuizQuestionByQuizIdPractice(quizId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        
        [HttpGet("GetAllQuizQuestionByQuizIdPaginationTest/{quizId}")]
        public async Task<ActionResult<ApiResult<PagedList<QuizQuestionTestDto>>>> GetAllQuizQuestionByQuizIdPaginationTest([Required] int quizId, [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _quizQuestionService.GetAllQuizQuestionByQuizIdPaginationTest(quizId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        
        [HttpGet("SearchQuizQuestionPagination")]
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> SearchQuizQuestionPagination([FromQuery] SearchQuizQuestionDto searchQuizQuestionDto)
        {
            var result = await _quizQuestionService.SearchQuizQuestion(searchQuizQuestionDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
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
<<<<<<< HEAD
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateQuizQuestion(
            [FromBody] QuizQuestionCreateDto quizQuestionCreateDto)
=======
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateQuizQuestion([FromForm] QuizQuestionCreateDto quizQuestionCreateDto)
>>>>>>> 4267fc22ea8e16fc2b9dbeea30b598b79d5afcb6
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
<<<<<<< HEAD
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> UpdateQuizQuestion(
            [FromBody] QuizQuestionUpdateDto quizQuestionUpdateDto)
=======
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> UpdateQuizQuestion([FromForm] QuizQuestionUpdateDto quizQuestionUpdateDto)
>>>>>>> 4267fc22ea8e16fc2b9dbeea30b598b79d5afcb6
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
<<<<<<< HEAD
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateRangeQuizQuestion(
            [FromBody] IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
=======
        public async Task<ActionResult<ApiResult<IEnumerable<QuizQuestionDto>>>> CreateRangeQuizQuestion([FromForm] IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
>>>>>>> 4267fc22ea8e16fc2b9dbeea30b598b79d5afcb6
        {
            var validator = new CreateQuizQuestionCommandValidator();
            var validationResults = quizQuestionsCreateDto.Select(async model => await validator.ValidateAsync(model));
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
            var validator = new UpdateQuizQuestionCommandValidator();
            var validationResults = quizQuestionsUpdateDto.Select(async model => await validator.ValidateAsync(model));
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

        [HttpPost("ImportValidate")]
        public async Task<IActionResult> ImportEmployee([FromForm] IFormFile fileImport, [Required] int quizId)
        {
            var imports = await _quizQuestionService.ImportExcel(fileImport, quizId);
            if (!imports.QuizQuestionImportDtos.Any())
            {
                return BadRequest(imports);
            }

            return StatusCode(200, imports);
        }

        [HttpGet("ImportExcel/{id}")]
        public IActionResult ImportDatabase(string id)
        {
            var imports = _quizQuestionService.ImportDatabase(id);
            return StatusCode(200, imports);
        }
    }
}