using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.SubmissionValidator;
using LW.Services.SubmissionServices;
using LW.Shared.DTOs.Submission;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }
        
        [HttpGet("GetAllSubmission")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<SubmissionDto>>>> GetAllSubmission([FromQuery]SubmissionRequestDto submissionRequestDto)
        {
            var result = await _submissionService.GetAllSubmission(submissionRequestDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetSubmission")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<SubmissionDto>>> GetSubmission([FromQuery]SubmissionRequestDto submissionRequestDto)
        {
            var result = await _submissionService.GetSubmission(submissionRequestDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("SubmitProblem")]
        public async Task<ActionResult<ApiResult<SubmissionDto>>> SubmitProblem([FromBody] SubmitProblemDto submitProblemDto)
        {
            var validationResult = await new CreateSubmissionCommandValidator().ValidateAsync(submitProblemDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _submissionService.SubmitProblem(submitProblemDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}