using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Post;
using LW.Shared.SeedWork;

namespace LW.Services.PostServices;

public interface IPostService
{
    Task<ApiResult<IEnumerable<PostDto>>> GetAllPost();
    Task<ApiResult<IEnumerable<PostDto>>> GetAllPostByGrade(int gradeId);
    Task<ApiResult<IEnumerable<PostDto>>> GetAllPostByUser(string userId);
    Task<ApiResult<PagedList<PostDto>>> GetAllPostPagination(PagingRequestParameters pagingRequestParameters);
    
    Task<ApiResult<PagedList<PostDto>>> GetAllPostByGradePagination(int gradeId, PagingRequestParameters pagingRequestParameters);
    
    Task<ApiResult<PagedList<PostDto>>> GetAllPostByUserPagination(string userId, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PostDto>> GetPostById(int id);
    Task<ApiResult<PostDto>> CreatePost(PostCreateDto postCreateDto);
    Task<ApiResult<PostDto>> UpdatePost(PostUpdateDto postUpdateDto);
    Task<ApiResult<bool>> DeletePost(int id);
     Task<ApiResult<PagedList<PostDto>>> GetAllPostByUserAndGradePagination(string userId, int gradeId,
        PagingRequestParameters pagingRequestParameters);

     Task<ApiResult<PagedList<PostDto>>> GetAllPostNotAnswerPagination(
         PagingRequestParameters pagingRequestParameters);

     Task<ApiResult<PagedList<PostDto>>> GetAllPostNotAnswerByGradePagination(int gradeId,
         PagingRequestParameters pagingRequestParameters);
     Task<ApiResult<bool>> VoteFavoritePost(string userId, int postId);
     Task<ApiResult<PagedList<PostDto>>> GetAllFavoritePostOfUserPagination(string userId, PagingRequestParameters pagingRequestParameters);

}