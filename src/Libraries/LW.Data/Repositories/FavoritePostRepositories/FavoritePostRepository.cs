using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.FavoritePostRepositories;

public class FavoritePostRepository: RepositoryBase<FavoritePost,int>, IFavoritePostRepository
{
    public FavoritePostRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<FavoritePost>> GetAllFavoritePost()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<FavoritePost>> GetAllFavoritePostOfUser(string userId)
    {
        return await FindByCondition(x => x.UserId == userId).OrderByDescending(x=>x.CreatedDate).ToListAsync();
    }

    public async Task<FavoritePost> GetFavoritePostById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<FavoritePost> CheckFavoritePostExisted(string userId, int postId)
    {
        return await FindByCondition(x => x.UserId == userId && x.PostId == postId).FirstOrDefaultAsync();
    }

    public async Task<FavoritePost> CreateFavoritePost(FavoritePost favoritePost)
    {
        await CreateAsync(favoritePost);
        return favoritePost;
    }

    public async Task<bool> DeleteFavoritePost(int id)
    {
        var entity = await GetFavoritePostById(id);
        if (entity == null)
        {
            return false;
        }

        await DeleteAsync(entity);
        return true;
    }
}