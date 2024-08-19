using System.Collections;
using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.FavoritePostRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.PostRepositories;
using LW.Shared.DTOs.Post;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;

namespace LW.Services.PostServices;

public class PostService : IPostService
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFavoritePostRepository _favoritePostRepository;

    public PostService(IMapper mapper, IPostRepository postRepository, IGradeRepository gradeRepository,
        UserManager<ApplicationUser> userManager, IFavoritePostRepository favoritePostRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _gradeRepository = gradeRepository;
        _userManager = userManager;
        _favoritePostRepository = favoritePostRepository;
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostByUserAndGradePagination(string userId, int gradeId,
        PagingRequestParameters pagingRequestParameters)
    {
        var userCheck = await _userManager.FindByIdAsync(userId);
        if (userCheck == null)
        {
            return new ApiResult<PagedList<PostDto>>(false, "User not found");
        }

        var grade = await _gradeRepository.GetGradeById(gradeId, false);
        if (grade == null)
        {
            return new ApiResult<PagedList<PostDto>>(false, "Grade not found");
        }

        var posts = await _postRepository.GetAllPostByUserAndGradePagination(userId, gradeId);
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "Not found list post");
        }
        // assign role for each poster
        foreach (var p in posts)
        {
            var roles = (await _userManager.GetRolesAsync(p.ApplicationUser)).ToArray();
            p.Roles = roles;
        }
        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
       
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result.AsQueryable().BuildMock(), pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostPagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var posts = await _postRepository.GetAllPostPagination();
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "List Posts is null !!!");
        }
        // assign role for each poster
        foreach (var p in posts)
        {
            var roles = (await _userManager.GetRolesAsync(p.ApplicationUser)).ToArray();
            p.Roles = roles;
        }
        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
        
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result.AsQueryable().BuildMock(), pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostByGradePagination(int gradeId,
        PagingRequestParameters pagingRequestParameters)
    {
        var grade = await _gradeRepository.GetGradeById(gradeId, false);
        if (grade == null)
        {
            return new ApiResult<PagedList<PostDto>>(false, "Grade not found");
        }

        var posts = await _postRepository.GetAllPostByGradePagination(gradeId);
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "List Posts by grade is null !!!");
        }
        // assign role for each poster
        foreach (var p in posts)
        {
            var roles = (await _userManager.GetRolesAsync(p.ApplicationUser)).ToArray();
            p.Roles = roles;
        }
        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
        
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result.AsQueryable().BuildMock(), pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostByUserPagination(string userId,
        PagingRequestParameters pagingRequestParameters)
    {
        var userCheck = await _userManager.FindByIdAsync(userId);
        if (userCheck == null)
        {
            return new ApiResult<PagedList<PostDto>>(false, "User not found");
        }

        var posts = await _postRepository.GetAllPostByUserPagination(userId);
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "List Posts by userId is null !!!");
        }
        // assign role for each poster
        foreach (var p in posts)
        {
            var roles = (await _userManager.GetRolesAsync(p.ApplicationUser)).ToArray();
            p.Roles = roles;
        }
        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
        
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result.AsQueryable().BuildMock(), pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }

    public async Task<ApiResult<PostDto>> GetPostById(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            return new ApiResult<PostDto>(false, "Not found");
        }

        var result = _mapper.Map<PostDto>(post);
        return new ApiResult<PostDto>(true, result, "Get by id successfully");
    }

    public async Task<ApiResult<PostDto>> CreatePost(PostCreateDto postCreateDto)
    {
        var userExist = await _userManager.FindByIdAsync(postCreateDto.UserId);
        if (userExist == null)
        {
            return new ApiResult<PostDto>(false, "Not found user");
        }

        var gradeExist = await _gradeRepository.GetGradeById(postCreateDto.GradeId, false);
        if (gradeExist == null)
        {
            return new ApiResult<PostDto>(false, "Not found grade");
        }

        var post = _mapper.Map<Post>(postCreateDto);
        await _postRepository.CreatePost(post);
        var result = _mapper.Map<PostDto>(post);
        return new ApiResult<PostDto>(true, result, "Create Post Successfully");
    }

    public async Task<ApiResult<PostDto>> UpdatePost(PostUpdateDto postUpdateDto)
    {
        var postExist = await _postRepository.GetPostById(postUpdateDto.Id);
        if (postExist == null)
        {
            return new ApiResult<PostDto>(false, "Not found post to update");
        }

        var userExist = await _userManager.FindByIdAsync(postUpdateDto.UserId);
        if (userExist == null)
        {
            return new ApiResult<PostDto>(false, "Not found user");
        }

        var gradeExist = await _gradeRepository.GetGradeById(postUpdateDto.GradeId, false);
        if (gradeExist == null)
        {
            return new ApiResult<PostDto>(false, "Not found grade");
        }

        var post = _mapper.Map(postUpdateDto, postExist);
        await _postRepository.UpdatePost(post);
        post.Grade = gradeExist;
        var result = _mapper.Map<PostDto>(post);
        return new ApiResult<PostDto>(true, result, "Update post successfully");
    }

    public async Task<ApiResult<bool>> DeletePost(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        await _postRepository.DeletePost(id);
        return new ApiResult<bool>(true, "Delete post successfully");
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostNotAnswerByGradePagination(int gradeId,
        PagingRequestParameters pagingRequestParameters)
    {
        IEnumerable<Post> posts  ;
        if (gradeId >0 )
        {
            posts= await _postRepository.GetAllPostNotAnswerByGradePagination(gradeId);
        }
        else
        {
            posts= await _postRepository.GetAllPostNotAnswerPagination();
        }
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "List Posts is null !!!");
        }
        // assign role for each poster
        foreach (var p in posts)
        {
            var roles = (await _userManager.GetRolesAsync(p.ApplicationUser)).ToArray();
            p.Roles = roles;
        }
        
        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
       
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result.AsQueryable().BuildMock(), pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }

    public async Task<ApiResult<bool>> VoteFavoritePost(string userId, int postId)
    {
        var userExist = await _userManager.FindByIdAsync(userId);
        if (userExist == null)
        {
            return new ApiResult<bool>(false, "Not found user");
        }

        var postExist = await _postRepository.GetPostById(postId);
        if (postExist == null)
        {
            return new ApiResult<bool>(false, "Not found post");
        }

        var favoritePostExist = await _favoritePostRepository.CheckFavoritePostExisted(userId, postId);
        if (favoritePostExist == null)
        {
            var favoritePost = new FavoritePost()
            {
                UserId = userId,
                PostId = postId
            };
            await _favoritePostRepository.CreateFavoritePost(favoritePost);
            return new ApiResult<bool>(true, "Create favorite post for user successfully !");
        }

        await _favoritePostRepository.DeleteFavoritePost(favoritePostExist.Id);
        return new ApiResult<bool>(true, "Delete favorite post for user successfully !");
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllFavoritePostOfUserPagination(string userId,
        PagingRequestParameters pagingRequestParameters)
    {
        var userExist = await _userManager.FindByIdAsync(userId);
        if (userExist == null)
        {
            return new ApiResult<PagedList<PostDto>>(false, "Not found user");
        }
        var favoritePosts = await _favoritePostRepository.GetAllFavoritePostOfUser(userId);
        var listPostId = favoritePosts.Select(x => x.PostId).ToList();
        var posts = new List<Post>();
        foreach (var id in listPostId)
        {
            var post = await _postRepository.GetPostById(id);
            if (post != null)
            {
                posts.Add(post);
            }
        }
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "Not found list favorite post of this user");
        }
        // assign role for each poster
        foreach (var p in posts)
        {
            var roles = (await _userManager.GetRolesAsync(p.ApplicationUser)).ToArray();
            p.Roles = roles;
        }
        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
        
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result.AsQueryable().BuildMock(), pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }
}