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
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("GetAllDocument")]
        public async Task<ActionResult<ApiResult<IEnumerable<DocumentDto>>>> GetAllDocument()
        {
            var result = await _documentService.GetAllDocument();
            return Ok(result);
        }

        [HttpGet("GetDocumentById/{id}")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> GetDocumentById([Required] int id)
        {
            var result = await _documentService.GetDocumentById(id);
            return Ok(result);
        }

        [HttpPost("CreateDocument")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> CreateDocument([FromBody] DocumentCreateDto documentCreateDto)
        {
            var result = await _documentService.CreateDocument(documentCreateDto);
            return Ok(result);
        }

        [HttpPut("UpdateDocument")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> UpdateDocument([FromBody] DocumentUpdateDto documentUpdateDto)
        {
            var result = await _documentService.UpdateDocument(documentUpdateDto);
            return Ok(result);
        }

        [HttpDelete("DeleteDocument/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteDocument([Required] int id)
        {
            var result = await _documentService.DeleteDocument(id);
            return Ok(result);
        }
    }

}
