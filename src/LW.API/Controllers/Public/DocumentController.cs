using LW.Services.DocumentService;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _DocumentService;

        public DocumentController(IDocumentService DocumentService)
        {
            _DocumentService = DocumentService;
        }

        [HttpGet("GetAllDocument")]
        public async Task<ActionResult<ApiResult<IEnumerable<DocumentDto>>>> GetAllDocument()
        {
            var result = await _DocumentService.GetAllDocument();
            return Ok(result);
        }

        [HttpGet("GetDocumentById/{id}")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> GetDocumentById([Required] int id)
        {
            var result = await _DocumentService.GetDocumentById(id);
            return Ok(result);
        }

        [HttpPost("CreateDocument")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> CreateDocument([FromBody] DocumentCreateDto DocumentCreateDto)
        {
            var result = await _DocumentService.CreateDocument(DocumentCreateDto);
            return Ok(result);
        }

        [HttpPut("UpdateDocument")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> UpdateDocument([FromBody] DocumentUpdateDto DocumentUpdateDto)
        {
            var result = await _DocumentService.UpdateDocument(DocumentUpdateDto);
            return Ok(result);
        }

        [HttpDelete("DeleteDocument/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteDocument([Required] int id)
        {
            var result = await _DocumentService.DeleteDocument(id);
            return Ok(result);
        }
    }

}
