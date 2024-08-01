using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.PostCommentRepositories;
using LW.Data.Repositories.PostRepositories;
using LW.Data.Repositories.VoteCommentRepositories;
using LW.Shared.DTOs.PostComment;
using LW.Shared.DTOs.VoteComment;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.PostCommentServices;

public class PostCommentService : IPostCommentService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVoteCommentRepository _voteCommentRepository;

    public PostCommentService(IPostCommentRepository postCommentRepository, IMapper mapper,
        IPostRepository postRepository, UserManager<ApplicationUser> userManager, IVoteCommentRepository voteCommentRepository)
    {
        _postCommentRepository = postCommentRepository;
        _mapper = mapper;
        _postRepository = postRepository;
        _userManager = userManager;
        _voteCommentRepository = voteCommentRepository;
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
        await _postCommentRepository.DeletePostComment(comment.Id);
        
        return new ApiResult<bool>(true, "Delete comment successfully");
    }
    private async Task DeleteCommentAndReplies(int commentId)
    {
        var childComments = await _postCommentRepository.GetAllPostCommentByParentId(commentId);
        foreach (var childComment in childComments)
        {
            await DeleteCommentAndReplies(childComment.Id); // Đệ quy xóa comment con
        }
        await _postCommentRepository.DeletePostComment(commentId); // Xóa comment hiện tại
    }
    

    public async Task<ApiResult<bool>> VoteCorrectPostComment(int postCommentId, string userId)
    {
        var comment = await _postCommentRepository.GetPostCommentById(postCommentId);
        if (comment == null)
        {
            return new ApiResult<bool>(false, "Not found postComment");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<bool>(false, "Not found user");
        }

        var checkVoted = await _voteCommentRepository.GetPostCommentByUserIdAndPostCommentId(userId, postCommentId);
        if (checkVoted == null)
        {
            var voteCommentCreate = new VoteCommentCreateDto()
            {
                UserId = userId,
                PostCommentId = postCommentId,
                IsCorrectVote = true
            };
            var model = _mapper.Map<VoteComment>(voteCommentCreate);
            //chua vote
            await _voteCommentRepository.CreateVoteComment(model);
            comment.CorrectVote += 1;
        }
        else
        {
            // nguoi dung da vote dung r 
            // xoa o bang vote comment
            await _voteCommentRepository.DeleteVoteComment(checkVoted.Id);
            //update lại correctVote ở comment
            comment.CorrectVote -= 1;
        }
        
        await _postCommentRepository.UpdatePostComment(comment);
        return new ApiResult<bool>(true, "Vote successfully");
    }

    public async Task<ApiResult<PagedList<PostCommentDto>>> GetAllPostCommentByPostIdPagination(int postId,
        PagingRequestParameters pagingRequestParameters)
    {
        var post = await _postRepository.GetPostById(postId);
        if (post == null)
        {
            return new ApiResult<PagedList<PostCommentDto>>(false, "Post not found");
        }

        var postComments = await _postCommentRepository.GetAllPostCommentByPostIdPagination(postId);
        if (!postComments.Any())
        {
            return new ApiResult<PagedList<PostCommentDto>>(false, "List PostComments is null !!!");
        }

        var result = _mapper.ProjectTo<PostCommentDto>(postComments);
        var pagedResult = await PagedList<PostCommentDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<PostCommentDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<PostCommentDto>>> GetAllPostCommentByParentIdPagination(int parentId,
        PagingRequestParameters pagingRequestParameters)
    {
        var post = await _postCommentRepository.GetPostCommentById(parentId);
        if (post == null)
        {
            return new ApiResult<PagedList<PostCommentDto>>(false, "PostCommentParent not found");
        }

        var postComments = await _postCommentRepository.GetAllPostCommentByParentIdPagination(parentId);

        if (!postComments.Any())
        {
            return new ApiResult<PagedList<PostCommentDto>>(false, "List PostComments is null !!!");
        }

        var result = _mapper.ProjectTo<PostCommentDto>(postComments);
        var pagedResult = await PagedList<PostCommentDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<PostCommentDto>>(pagedResult);
    }
}