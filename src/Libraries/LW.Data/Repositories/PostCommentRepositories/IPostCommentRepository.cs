using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.PostCommentRepositories;

public interface IPostCommentRepository : IRepositoryBase<PostComment, int>
{
    Task<PostComment> CreatePostComment(PostComment postComment);
    Task<PostComment> UpdatePostComment(PostComment postComment);
    Task<bool> DeletePostComment(int id);
    Task<PostComment> GetPostCommentById(int id);
    Task<IEnumerable<PostComment>> GetAllPostComment();
    Task<IEnumerable<PostComment>> GetAllPostCommentByPostId(int postId);
    
    Task<IEnumerable<PostComment>> GetAllPostCommentByParentId(int parentId);
    Task<IQueryable<PostComment>> GetAllPostCommentPagination();

    Task<PostComment> GetParentCommentById(int postCommentId, int? parentId);
}