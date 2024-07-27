using LW.Shared.DTOs.PostComment;
using LW.Shared.SeedWork;

namespace LW.Services.PostCommentServices;

public interface IPostCommentService
{
    Task<ApiResult<IEnumerable<PostCommentDto>>> GetAllPostCommentByPostId(int postId);
    Task<ApiResult<PostCommentDto>> GetPostCommentById(int id);
    Task<ApiResult<PostCommentDto>> CreatePostComment(PostCommentCreateDto postCommentCreate);
    Task<ApiResult<PostCommentDto>> UpdatePostComment(PostCommentUpdateDto postCommentUpdate);
    Task<ApiResult<bool>> DeletePostComment(int id);
    Task<ApiResult<bool>> VoteCorrectPostComment(int id);
}