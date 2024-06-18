using LW.Data.Entities;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;

namespace LW.Services.DocumentService;

public interface IDocumentService
{
    Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocument();
    Task<ApiResult<DocumentDto>> GetDocumentById(int id);
    Task<ApiResult<DocumentDto>> CreateDocument(DocumentCreateDto DocumentCreateDto);
    Task<ApiResult<DocumentDto>> UpdateDocument(DocumentUpdateDto DocumentUpdateDto);
    Task<ApiResult<bool>> DeleteDocument(int id);
}