using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.EnumServices;
using LW.Shared.DTOs.Enum;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumController : ControllerBase
    {
        private readonly IEnumService _enumService;

        public EnumController(IEnumService enumService)
        {
            _enumService = enumService;
        }

        [HttpGet("GetAllBookCollection")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetBookCollection()
        {
            var result = await _enumService.GetAllBookCollection();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAllBookType")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetBookType()
        {
            var result = await _enumService.GetAllBookType();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetTypeQuestion")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetTypeQuestion()
        {
            var result = await _enumService.GetAllTypeQuestion();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetQuestionLevel")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetQuestionLevel()
        {
            var result = await _enumService.GetAllLevelQuestion();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        

        [HttpGet("GetTypeQuiz")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetTypeQuiz()
        {
            var result = await _enumService.GetAllQuizType();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
            
        [HttpGet("GetTypeOfExam")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetTypeOfExam()
        {
            var result = await _enumService.GetAllTypeOfExam();

            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}