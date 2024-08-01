using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.PostRepositories;

public interface IPostRepository : IRepositoryBase<Post, int>
{
    Task<IEnumerable<Post>> GetAllPost();
    Task<IEnumerable<Post>> GetAllPostByGrade(int gradeId);
    Task<IEnumerable<Post>> GetAllPostByUser(string userId);
    Task<IQueryable<Post>> GetAllPostPagination();
    
    Task<IQueryable<Post>> GetAllPostByGradePagination(int grade);
    
    Task<IQueryable<Post>> GetAllPostByUserPagination(string userId);
    Task<Post> GetPostById(int id);
    Task<Post> CreatePost(Post post);
    Task<Post> UpdatePost(Post post);
    Task<bool> DeletePost(int id);
    Task<IQueryable<Post>> GetAllPostByUserAndGradePagination(string userId, int gradeId);
    Task<IQueryable<Post>> GetAllPostNotAnswerPagination();
    Task<IQueryable<Post>> GetAllPostNotAnswerByGradePagination(int gradeId);
}