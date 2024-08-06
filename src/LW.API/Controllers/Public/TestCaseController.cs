using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.TestCaseValidator;
using LW.Services.TestCaseServices;
using LW.Shared.DTOs.TestCase;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestCaseController : ControllerBase
    {
        private readonly ITestCaseService _testCaseService;

        public TestCaseController(ITestCaseService testCaseService)
        {
            _testCaseService = testCaseService;
        }
        
        [HttpGet("GetAllTestCase")]
        public async Task<ActionResult<ApiResult<IEnumerable<TestCaseDto>>>> GetAllTestCase()
        {
            var result = await _testCaseService.GetAllTestCase();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllTestCaseByProblemId/{problemId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<TestCaseDto>>>> GetAllTestCaseByProblemId(int problemId)
        {
            var result = await _testCaseService.GetAllTestCaseByProblemId(problemId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetTestCaseById/{id}")]
        public async Task<ActionResult<ApiResult<TestCaseDto>>> GetTestCaseById(int id)
        {
            var result = await _testCaseService.GetTestCaseById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpPost("CreateTestCase")]
        public async Task<ActionResult<ApiResult<TestCaseDto>>> CreateTestCase([FromBody] TestCaseCreateDto testCaseCreateDto)
        {
            var validationResult = await new CreateTestCaseCommandValidator().ValidateAsync(testCaseCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _testCaseService.CreateTestCase(testCaseCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateTestCase")]
        public async Task<ActionResult<ApiResult<TestCaseDto>>> UpdateTestCase([FromBody] TestCaseUpdateDto testCaseUpdateDto)
        {
            var validationResult = await new UpdateTestCaseCommandValidator().ValidateAsync(testCaseUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _testCaseService.UpdateTestCase(testCaseUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPost("CreateRangeTestCase")]
        public async Task<ActionResult<ApiResult<bool>>> CreateRangeTestCase([FromBody] IEnumerable<TestCaseCreateDto> testCaseCreateDto)
        {
            var validator =  new CreateTestCaseCommandValidator();
            var validationResults =  testCaseCreateDto.Select(async model => await validator.ValidateAsync(model));
            if (validationResults.Any(result => !result.IsCompleted))
            {
                return BadRequest(validationResults);
            }
            var result = await _testCaseService.CreateRangeTestCase(testCaseCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateRangeTestCase")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateRangeTestCase([FromBody] IEnumerable<TestCaseUpdateDto> testCaseUpdateDto)
        {
            var validator =  new UpdateTestCaseCommandValidator();
            var validationResults =  testCaseUpdateDto.Select(async model => await validator.ValidateAsync(model));
            if (validationResults.Any(result => !result.IsCompleted))
            {
                return BadRequest(validationResults);
            }
            var result = await _testCaseService.UpdateRangeTestCase(testCaseUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteTestCase/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteTestCase([Required] int id)
        {
            var result = await _testCaseService.DeleteTestCase(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteRangeTestCase")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteRangeTestCase(IEnumerable<int> ids)
        {
            var result = await _testCaseService.DeleteRangeTestCase(ids);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
