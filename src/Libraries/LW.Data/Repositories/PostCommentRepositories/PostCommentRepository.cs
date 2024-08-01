using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.PostCommentRepositories;

public class PostCommentRepository : RepositoryBase<PostComment, int>, IPostCommentRepository
{
    public PostCommentRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<PostComment> CreatePostComment(PostComment postComment)
    {
        await CreateAsync(postComment);
        return postComment;
    }

    public async Task<PostComment> UpdatePostComment(PostComment postComment)
    {
        await UpdateAsync(postComment);
        return postComment;
    }

    public async Task<bool> DeletePostComment(int id)
    {
        var postComment = await GetPostCommentById(id);
        if (postComment == null)
        {
            return false;
        }

        await DeleteAsync(postComment);
        return true;
    }
    public async Task<bool> DeleteRangePostComment(IEnumerable<PostComment> postComments)
    {
        await DeleteListAsync(postComments);
        return true;
    }

    public async Task<PostComment> GetPostCommentById(int id)
    {
        var postComment = await FindByCondition(x => x.Id == id, false)
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .FirstOrDefaultAsync();

        if (postComment != null)
        {
            postComment.PostCommentChilds = postComment.PostCommentChilds
                .OrderBy(c => c.CreatedDate)
                .ToList();
        }

        return postComment;
    }

    public async Task<IEnumerable<PostComment>> GetAllPostComment()
    {
        return await FindByCondition(x => x.ParentId == null)
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .ToListAsync();
    }

    public async Task<IEnumerable<PostComment>> GetAllPostCommentByPostId(int postId)
    {
        return await  FindByCondition(x => x.PostId == postId && x.ParentId == null)
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .ToListAsync();    }

    public async Task<IEnumerable<PostComment>> GetAllPostCommentByParentId(int parentId)
    {
        return await FindByCondition(x => x.ParentId == parentId, trackChanges:false)
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .ToListAsync();
    }

    public async Task<IQueryable<PostComment>> GetAllPostCommentPagination()
    {
        return FindAll()
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .AsQueryable();
    }
    public async Task<IQueryable<PostComment>> GetAllPostCommentByPostIdPagination(int postId)
    {
        return  FindByCondition(x => x.PostId == postId && x.ParentId == null)
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .AsQueryable();
    }

    public async Task<IQueryable<PostComment>> GetAllPostCommentByParentIdPagination(int parentId)
    {
        return  FindByCondition(x => x.ParentId == parentId)
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .AsQueryable();
    }

   

    public async Task<PostComment> GetParentCommentById(int postCommentId, int? parentId)
    {
        return await FindByCondition(x => x.Id == postCommentId && x.ParentId == parentId)
            .Include(x => x.ApplicationUser)
            .Include(x => x.Post)
            .Include(x => x.PostCommentChilds).ThenInclude(x => x.ApplicationUser)
            .FirstOrDefaultAsync();
    }
}