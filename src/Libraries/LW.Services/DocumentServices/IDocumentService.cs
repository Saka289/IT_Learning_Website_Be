using LW.Data.Entities;
using LW.Shared.DTOs.Document;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Services.DocumentServices;

public interface IDocumentService
{
    Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocument(bool? status);
    Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocumentByGrade(int id, bool? status);
    Task<ApiResult<PagedList<DocumentDto>>> GetAllDocumentPagination(SearchDocumentDto searchDocumentDto);
    Task<ApiResult<DocumentDto>> GetDocumentById(int id);
    Task<ApiResult<DocumentDto>> CreateDocument(DocumentCreateDto documentCreateDto);
    Task<ApiResult<DocumentDto>> UpdateDocument(DocumentUpdateDto documentUpdateDto);
    Task<ApiResult<bool>> UpdateStatusDocument(int id);
    Task<ApiResult<bool>> DeleteDocument(int id);
}