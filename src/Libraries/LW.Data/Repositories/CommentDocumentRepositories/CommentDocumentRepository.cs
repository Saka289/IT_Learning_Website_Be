﻿using LW.Data.Common;
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
        var result = FindAll().Include(r => r.ParentComment).Where(x => x.DocumentId == id);
        return Task.FromResult(result);
    }

    public Task<IQueryable<CommentDocument>> GetAllCommentByUserIdPagination(string id)
    {
        var result = FindAll().Include(r => r.ParentComment).Where(x => x.UserId == id);
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
        return await FindByCondition(c => c.DocumentId == id, false, c => c.ParentComment).FirstOrDefaultAsync();
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