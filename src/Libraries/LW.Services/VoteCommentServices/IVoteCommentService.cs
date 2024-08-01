using LW.Shared.DTOs.UserExam;
using LW.Shared.DTOs.VoteComment;
using LW.Shared.SeedWork;

namespace LW.Services.VoteCommentServices;

public interface IVoteCommentService
{
    public Task<ApiResult<VoteCommentDto>> CreateVoteComment(VoteCommentCreateDto voteCommentCreate);
    public Task<ApiResult<VoteCommentDto>> GetVoteCommentById(int id);
    public Task<ApiResult<bool>> DeleteVoteCommentById(int id);
    public Task<ApiResult<bool>> GetPostCommentByUserIdAndPostCommentId(string userId, int postCommentId);
}