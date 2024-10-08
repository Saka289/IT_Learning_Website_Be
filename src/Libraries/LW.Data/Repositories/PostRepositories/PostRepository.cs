﻿using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.PostRepositories;

public class PostRepository : RepositoryBase<Post, int>, IPostRepository
{
    public PostRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Post>> GetAllPostPagination()
    {
        return await FindAll().Include(x => x.Grade)
            .Include(x => x.ApplicationUser)
            .Include(x => x.PostComments)
            .Include(x => x.FavoritePosts)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllPostByGradePagination(int grade)
    {
        return await FindByCondition(x => x.GradeId == grade)
            .Include(x => x.Grade)
            .Include(x => x.ApplicationUser)
            .Include(x => x.PostComments)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllPostByUserPagination(string userId)
    {
        return await FindByCondition(x => x.UserId == userId)
            .Include(x => x.Grade)
            .Include(x => x.ApplicationUser)
            .Include(x => x.PostComments)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllPostByUserAndGradePagination(string userId, int gradeId)
    {
        return await FindByCondition(x => x.UserId == userId && x.GradeId == gradeId)
            .Include(x => x.Grade)
            .Include(x => x.ApplicationUser)
            .Include(x => x.PostComments)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllPostNotAnswerPagination()
    {
        return await FindByCondition(x => x.PostComments.Count == 0)
            .Include(x => x.Grade)
            .Include(x => x.ApplicationUser)
            .Include(x => x.PostComments).ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllPostNotAnswerByGradePagination(int gradeId)
    {
        return await FindByCondition(x => x.PostComments.Count == 0 && x.GradeId == gradeId)
            .Include(x => x.Grade)
            .Include(x => x.ApplicationUser)
            .Include(x => x.PostComments).ToListAsync();
    }

    public async Task<Post> GetPostById(int id)
    {
        return await FindByCondition(x => x.Id == id)
            .Include(x => x.Grade)
            .Include(x => x.ApplicationUser)
            .Include(x => x.PostComments)
            .FirstOrDefaultAsync();
    }

    public async Task<Post> CreatePost(Post post)
    {
        await CreateAsync(post);
        return post;
    }

    public async Task<Post> UpdatePost(Post post)
    {
        await UpdateAsync(post);
        return post;
    }

    public async Task<bool> DeletePost(int id)
    {
        var post = await GetPostById(id);
        if (post == null)
        {
            return false;
        }

        await DeleteAsync(post);
        return true;
    }
}