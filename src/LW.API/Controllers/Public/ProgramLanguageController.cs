using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.ProgramLanguageValidator;
using LW.Services.ProgramLanguageServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.ProgramLanguage;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
    public class ProgramLanguageController : ControllerBase
    {
        private readonly IProgramLanguageService _programLanguageService;

        public ProgramLanguageController(IProgramLanguageService programLanguageService)
        {
            _programLanguageService = programLanguageService;
        }

        [HttpGet("GetAllProgramLanguage")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<ProgramLanguageDto>>>> GetAllProgramLanguage()
        {
            var result = await _programLanguageService.GetAllProgramLanguage();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetProgramLanguageById/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<ProgramLanguageDto>>> GetProgramLanguageById(int id)
        {
            var result = await _programLanguageService.GetProgramLanguageById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateProgramLanguage")]
        public async Task<ActionResult<ApiResult<ProgramLanguageDto>>> CreateProgramLanguage(
            [FromBody] ProgramLanguageCreateDto programLanguageCreateDto)
        {
            var validationResult =
                await new CreateProgramLanguageCommandValidator().ValidateAsync(programLanguageCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _programLanguageService.CreateProgramLanguage(programLanguageCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateProgramLanguage")]
        public async Task<ActionResult<ApiResult<ProgramLanguageDto>>> UpdateProgramLanguage(
            [FromBody] ProgramLanguageUpdateDto programLanguageUpdateDto)
        {
            var validationResult =
                await new UpdateProgramLanguageCommandValidator().ValidateAsync(programLanguageUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _programLanguageService.UpdateProgramLanguage(programLanguageUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateStatusProgramLanguage/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusProgramLanguage([Required] int id)
        {
            var result = await _programLanguageService.UpdateStatusProgramLanguage(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteProgramLanguage/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteProgramLanguage([Required] int id)
        {
            var result = await _programLanguageService.DeleteProgramLanguage(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}