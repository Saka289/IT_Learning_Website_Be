using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.PostRepositories;
using LW.Shared.DTOs.Post;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.PostServices;

public class PostService : IPostService
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostService(IMapper mapper, IPostRepository postRepository, IGradeRepository gradeRepository,
        UserManager<ApplicationUser> userManager)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _gradeRepository = gradeRepository;
        _userManager = userManager;
    }

    public async Task<ApiResult<IEnumerable<PostDto>>> GetAllPost()
    {
        var posts = await _postRepository.GetAllPost();
        if (!posts.Any())
        {
            return new ApiResult<IEnumerable<PostDto>>(false, "Not found");
        }

        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
        return new ApiResult<IEnumerable<PostDto>>(true, result, "Get all post successfully");
    }

    public async Task<ApiResult<IEnumerable<PostDto>>> GetAllPostByGrade(int gradeId)
    {
        var grade = await _gradeRepository.GetGradeById(gradeId);
        if (grade == null)
        {
            return new ApiResult<IEnumerable<PostDto>>(false, "Grade not found");
        }

        var posts = await _postRepository.GetAllPostByGrade(gradeId);
        if (!posts.Any())
        {
            return new ApiResult<IEnumerable<PostDto>>(false, "Not found list post");
        }

        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
        return new ApiResult<IEnumerable<PostDto>>(true, result, "Get all post by grade successfully");
    }

    public async Task<ApiResult<IEnumerable<PostDto>>> GetAllPostByUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<IEnumerable<PostDto>>(false, "User not found");
        }

        var posts = await _postRepository.GetAllPostByUser(userId);
        if (!posts.Any())
        {
            return new ApiResult<IEnumerable<PostDto>>(false, "Not found list post");
        }

        var result = _mapper.Map<IEnumerable<PostDto>>(posts);
        return new ApiResult<IEnumerable<PostDto>>(true, result, "Get all post by user successfully");
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostPagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var posts = await _postRepository.GetAllPostPagination();
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "List Posts is null !!!");
        }

        var result = _mapper.ProjectTo<PostDto>(posts);
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostByGradePagination(int gradeId,
        PagingRequestParameters pagingRequestParameters)
    {
        var grade = await _gradeRepository.GetGradeById(gradeId);
        if (grade == null)
        {
            return new ApiResult<PagedList<PostDto>>(false, "Grade not found");
        }
        var posts = await _postRepository.GetAllPostByGradePagination(gradeId);
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "List Posts by grade is null !!!");
        }

        var result = _mapper.ProjectTo<PostDto>(posts);
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<PostDto>>> GetAllPostByUserPagination(string userId,
        PagingRequestParameters pagingRequestParameters)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<PagedList<PostDto>>(false, "User not found");
        }
        var posts = await _postRepository.GetAllPostByUserPagination(userId);
        if (!posts.Any())
        {
            return new ApiResult<PagedList<PostDto>>(false, "List Posts by userId is null !!!");
        }

        var result = _mapper.ProjectTo<PostDto>(posts);
        var pagedResult = await PagedList<PostDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
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

        var gradeExist = await _gradeRepository.GetGradeById(postCreateDto.GradeId);
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

        var gradeExist = await _gradeRepository.GetGradeById(postUpdateDto.GradeId);
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
}