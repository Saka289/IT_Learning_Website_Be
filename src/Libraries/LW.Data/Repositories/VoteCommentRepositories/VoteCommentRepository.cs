using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.VoteCommentRepositories;

public class VoteCommentRepository:RepositoryBase<VoteComment,int>, IVoteCommentRepository
{
    public VoteCommentRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<VoteComment?> GetVoteCommentById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<VoteComment> GetPostCommentByUserIdAndPostCommentId(string userId, int postCommentId)
    {
        return await FindByCondition(x => x.UserId == userId && x.PostCommentId == postCommentId).FirstOrDefaultAsync();
    }

    public async Task<VoteComment> CreateVoteComment(VoteComment voteComment)
    {
        await CreateAsync(voteComment);
        return voteComment;
    }

    public async Task<VoteComment> UpdateVoteComment(VoteComment voteComment)
    {
        await UpdateAsync(voteComment);
        return voteComment;
    }

    public async Task<bool> DeleteVoteComment(int id)
    {
        var voteComment = await GetVoteCommentById(id);
        if (voteComment == null)
        {
            return false;
        }

        await DeleteAsync(voteComment);
        return true;
    }
}