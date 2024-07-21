using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.CommentDocumentRepositories;

public class CommentDocumentRepository : RepositoryBase<CommentDocument, int>, ICommentDocumentRepository
{
    public CommentDocumentRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public Task<IQueryable<CommentDocument>> GetAllCommentByDocumentIdPagination(int id)
    {
        var result = FindAll()
            .Include(r => r.ChildCommentDocuments)
            .ThenInclude(u => u.ApplicationUser)
            .Include(u => u.ApplicationUser)
            .Where(x => x.DocumentId == id);
        return Task.FromResult(result);
    }

    public Task<IQueryable<CommentDocument>> GetAllCommentByUserIdPagination(string id)
    {
        var result = FindAll()
            .Include(r => r.ChildCommentDocuments)
            .ThenInclude(u => u.ApplicationUser)
            .Include(u => u.ApplicationUser)
            .Where(x => x.UserId == id);
        return Task.FromResult(result);
    }

    public async Task<CommentDocument> CreateComment(CommentDocument commentDocument)
    {
        await CreateAsync(commentDocument);
        return await Task.FromResult(commentDocument);
    }

    public async Task<CommentDocument> UpdateComment(CommentDocument commentDocument)
    {
        await UpdateAsync(commentDocument);
        return await Task.FromResult(commentDocument);
    }

    public async Task<CommentDocument> GetCommentById(int id)
    {
        return await FindByCondition(c => c.Id == id)
            .Include(r => r.ChildCommentDocuments)
            .ThenInclude(u => u.ApplicationUser)
            .Include(u => u.ApplicationUser)
            .FirstOrDefaultAsync();
    }

    public Task<IQueryable<CommentDocument>> GetAllCommentByDocumentId(int documentId)
    {
        var result = FindByCondition(x => x.DocumentId == documentId);
        return Task.FromResult(result);
    }

    public async Task<IEnumerable<CommentDocument>> GetAllParentCommentById(int id)
    {
        return await FindByCondition(c => c.ParentId == id).ToListAsync();
    }

    public async Task<CommentDocument> GetParentCommentById(int commentDocumentId, int? parentId)
    {
        return await FindByCondition(r => r.Id == commentDocumentId && r.ParentId == parentId).FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteComment(int id)
    {
        var comment = await GetByIdAsync(id);
        if (comment == null)
        {
            return false;
        }

        await DeleteAsync(comment);
        return true;
    }
}