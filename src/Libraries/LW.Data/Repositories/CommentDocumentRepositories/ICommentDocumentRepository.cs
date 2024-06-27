using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using Document = System.Reflection.Metadata.Document;

namespace LW.Data.Repositories.CommentDocumentRepositories;

public interface ICommentDocumentRepository : IRepositoryBase<CommentDocument, int>
{
    Task<IQueryable<CommentDocument>> GetAllCommentByDocumentIdPagination(int id);
    Task<IQueryable<CommentDocument>> GetAllCommentByUserIdPagination(string id);
    Task<CommentDocument> CreateComment(CommentDocument commentDocument);
    Task<CommentDocument> UpdateComment(CommentDocument commentDocument);
    Task<CommentDocument> GetCommentById(int id);
    Task<IEnumerable<CommentDocument>> GetAllParentCommentById(int id);
    Task<CommentDocument> GetParentCommentById(int commentDocumentId, int? parentId);
    Task<bool> DeleteComment(int id);
}