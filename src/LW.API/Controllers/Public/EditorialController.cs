using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.EditorialValidator;
using LW.Services.EditorialServices;
using LW.Shared.DTOs.Editorial;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditorialController : ControllerBase
    {
        private readonly IEditorialService _editorialService;

        public EditorialController(IEditorialService editorialService)
        {
            _editorialService = editorialService;
        }

        [HttpGet("GetAllEditorial")]
        public async Task<ActionResult<IEnumerable<EditorialDto>>> GetAllEditorial()
        {
            var result = await _editorialService.GetAllEditorial();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetEditorialByProblemId/{problemId}")]
        public async Task<ActionResult<EditorialDto>> GetEditorialByProblemId(int problemId)
        {
            var result = await _editorialService.GetEditorialByProblemId(problemId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetEditorialById/{id}")]
        public async Task<ActionResult<EditorialDto>> GetEditorialById(int id)
        {
            var result = await _editorialService.GetEditorialById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateEditorial")]
        public async Task<ActionResult<EditorialDto>> CreateEditorial([FromForm] EditorialCreateDto editorialCreateDto)
        {
            var validationResult = await new CreateEditorialCommandValidator().ValidateAsync(editorialCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _editorialService.CreateEditorial(editorialCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateEditorial")]
        public async Task<ActionResult<EditorialDto>> UpdateEditorial([FromForm] EditorialUpdateDto editorialUpdateDto)
        {
            var validationResult = await new UpdateEditorialCommandValidator().ValidateAsync(editorialUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _editorialService.UpdateEditorial(editorialUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteEditorial/{id}")]
        public async Task<ActionResult<bool>> DeleteEditorial(int id)
        {
            var result = await _editorialService.DeleteEditorial(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
