using LW.Data.Entities;
using LW.Shared.DTOs.CommentDocumentDto;
using LW.Shared.SeedWork;

namespace LW.Services.CommentDocumentServices;

public interface ICommentDocumentService
{
    Task<ApiResult<PagedList<CommentDocumentDto>>> GetAllCommentByDocumentIdPagination(int id, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<CommentDocumentDto>>> GetAllCommentDocumentByUserIdPagination(string id, int documentId, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<CommentDocumentDto>> GetCommentDocumentById(int id);
    Task<ApiResult<CommentDocumentDto>> CreateCommentDocument(CommentDocumentCreateDto commentDocumentCreateDto);
    Task<ApiResult<CommentDocumentDto>> UpdateCommentDocument(CommentDocumentUpdateDto commentDocumentUpdateDto);
    Task<ApiResult<bool>> DeleteCommentDocument(int id);
}