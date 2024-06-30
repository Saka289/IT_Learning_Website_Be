﻿using LW.Data.Entities;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;

namespace LW.Services.DocumentServices;

public interface IDocumentService
{
    Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocument();
    Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocumentByGrade(int id);
    Task<ApiResult<PagedList<DocumentDto>>> GetAllDocumentPagination(PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<DocumentDto>> GetDocumentById(int id);
    Task<ApiResult<PagedList<DocumentDto>>> SearchByDocumentPagination(SearchDocumentDto searchDocumentDto);
    Task<ApiResult<DocumentDto>> CreateDocument(DocumentCreateDto documentCreateDto);
    Task<ApiResult<DocumentDto>> UpdateDocument(DocumentUpdateDto documentUpdateDto);
    Task<ApiResult<bool>> UpdateStatusDocument(int id);
    Task<ApiResult<bool>> DeleteDocument(int id);
}