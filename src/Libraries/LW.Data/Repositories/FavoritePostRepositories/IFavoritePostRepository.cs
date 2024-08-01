using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.FavoritePostRepositories;

public interface IFavoritePostRepository : IRepositoryBase<FavoritePost, int>
{
    Task<IEnumerable<FavoritePost>> GetAllFavoritePost();
    Task<IEnumerable<FavoritePost>> GetAllFavoritePostOfUser(string userId);
    Task<FavoritePost> GetFavoritePostById(int id);
    
    Task<FavoritePost> CheckFavoritePostExisted(string userId, int postId);
    Task<FavoritePost> CreateFavoritePost(FavoritePost favoritePost);
    Task<bool> DeleteFavoritePost(int id);
}