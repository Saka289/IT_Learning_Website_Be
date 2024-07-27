using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.PostCommentRepositories;
using LW.Data.Repositories.PostRepositories;
using LW.Shared.DTOs.PostComment;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.PostCommentServices;

public class PostCommentService : IPostCommentService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public PostCommentService(IPostCommentRepository postCommentRepository, IMapper mapper, IPostRepository postRepository, UserManager<ApplicationUser> userManager)
    {
        _postCommentRepository = postCommentRepository;
        _mapper = mapper;
        _postRepository = postRepository;
        _userManager = userManager;
    }
    public async Task<ApiResult<IEnumerable<PostCommentDto>>> GetAllPostCommentByPostId(int postId)
    {
        var post = await _postRepository.GetPostById(postId);
        if (post == null)
        {
            return new ApiResult<IEnumerable<PostCommentDto>>(false, "Not found post");
        }

        var comments = await _postCommentRepository.GetAllPostCommentByPostId(postId);
        if (!comments.Any())
        {
            return new ApiResult<IEnumerable<PostCommentDto>>(false, "Not found list comment");
        }

        var result = _mapper.Map<IEnumerable<PostCommentDto>>(comments);
        return new ApiResult<IEnumerable<PostCommentDto>>(true, result, "Get list comment by post successfully");
    }

    public async Task<ApiResult<PostCommentDto>> GetPostCommentById(int id)
    {
        var comment = await _postCommentRepository.GetPostCommentById(id);
        if (comment == null)
        {
            return new ApiResult<PostCommentDto>(false, "Not found");
        }

        var result = _mapper.Map<PostCommentDto>(comment);
        return new ApiResult<PostCommentDto>(true, result, "Get comment by id successfully");
    }

    public async Task<ApiResult<PostCommentDto>> CreatePostComment(PostCommentCreateDto postCommentCreate)
    {
        var post = await _postRepository.GetPostById(postCommentCreate.PostId);
        if (post == null)
        {
            return new ApiResult<PostCommentDto>(false, "Not found post");
        }

        var user = await _userManager.FindByIdAsync(postCommentCreate.UserId);
        if (user == null)
        {
            return new ApiResult<PostCommentDto>(false, "Not found user");
        }

        var comment = _mapper.Map<PostComment>(postCommentCreate);
        // auto vote correct begin equal 0
        comment.CorrectVote = 0;
        await _postCommentRepository.CreatePostComment(comment);
        var result = _mapper.Map<PostCommentDto>(comment);
        return new ApiResult<PostCommentDto>(true, result, "Create comment successfully");
    }

    public async Task<ApiResult<PostCommentDto>> UpdatePostComment(PostCommentUpdateDto postCommentUpdate)
    {
        var comment = await _postCommentRepository.GetPostCommentById(postCommentUpdate.Id);
        if (comment == null)
        {
            return new ApiResult<PostCommentDto>(false, "Not found comment to update");
        }
        var post = await _postRepository.GetPostById(postCommentUpdate.PostId);
        if (post == null)
        {
            return new ApiResult<PostCommentDto>(false, "Not found post");
        }

        var user = await _userManager.FindByIdAsync(postCommentUpdate.UserId);
        if (user == null)
        {
            return new ApiResult<PostCommentDto>(false, "Not found user");
        }

        var updateDto = _mapper.Map(postCommentUpdate, comment);
        await _postCommentRepository.UpdatePostComment(updateDto);
        var result = _mapper.Map<PostCommentDto>(updateDto);
        return new ApiResult<PostCommentDto>(true, result, "Update comment successfully");
    }

    public async Task<ApiResult<bool>> DeletePostComment(int id)
    {
        var comment = await _postCommentRepository.GetPostCommentById(id);
        if (comment == null)
        {
            return new ApiResult<bool>(false, "Not found comment");
        }

        await _postCommentRepository.DeletePostComment(id);
        return new ApiResult<bool>(true, "Delete comment successfully");
    }

    public async Task<ApiResult<bool>> VoteCorrectPostComment(int id)
    {
        var comment = await _postCommentRepository.GetPostCommentById(id);
        if (comment == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        comment.CorrectVote += 1;
        await _postCommentRepository.UpdatePostComment(comment);
        return new ApiResult<bool>(true, "Vote successfully");
    }
}