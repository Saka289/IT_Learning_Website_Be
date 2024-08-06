using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.VoteCommentRepositories;

public interface IVoteCommentRepository : IRepositoryBase<VoteComment,int>
{
   // Task<IEnumerable<VoteComment>> GetAllVoteCommentByUserId(string userId);
    Task<VoteComment?> GetVoteCommentById(int id);
    Task<VoteComment> GetPostCommentByUserIdAndPostCommentId(string userId, int postCommentId);
    Task<VoteComment> CreateVoteComment(VoteComment voteComment);
    Task<VoteComment> UpdateVoteComment(VoteComment voteComment);
    Task<bool> DeleteVoteComment(int id);
}