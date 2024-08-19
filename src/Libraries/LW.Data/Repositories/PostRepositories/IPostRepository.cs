using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.PostRepositories;

public interface IPostRepository : IRepositoryBase<Post, int>
{
  
    Task<IEnumerable<Post>> GetAllPostPagination();
    
    Task<IEnumerable<Post>> GetAllPostByGradePagination(int grade);
    
    Task<IEnumerable<Post>> GetAllPostByUserPagination(string userId);
    Task<Post> GetPostById(int id);
    Task<Post> CreatePost(Post post);
    Task<Post> UpdatePost(Post post);
    Task<bool> DeletePost(int id);
    Task<IEnumerable<Post>> GetAllPostByUserAndGradePagination(string userId, int gradeId);
    Task<IEnumerable<Post>> GetAllPostNotAnswerPagination();
    Task<IEnumerable<Post>> GetAllPostNotAnswerByGradePagination(int gradeId);
}