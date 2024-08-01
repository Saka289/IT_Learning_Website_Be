using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.PostCommentRepositories;
using LW.Data.Repositories.VoteCommentRepositories;
using LW.Shared.DTOs.VoteComment;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.VoteCommentServices;

public class VoteCommentService : IVoteCommentService
{
    private readonly IVoteCommentRepository _voteCommentRepository;
    private readonly IMapper _mapper;
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public VoteCommentService(IVoteCommentRepository voteCommentRepository, IMapper mapper, IPostCommentRepository postCommentRepository, UserManager<ApplicationUser> userManager)
    {
        _voteCommentRepository = voteCommentRepository;
        _mapper = mapper;
        _postCommentRepository = postCommentRepository;
        _userManager = userManager;
    }
    
    public async Task<ApiResult<VoteCommentDto>> CreateVoteComment(VoteCommentCreateDto voteCommentCreate)
    {
        var user = await _userManager.FindByIdAsync(voteCommentCreate.UserId);
        if (user == null)
        {
            return new ApiResult<VoteCommentDto>(false, $"Not found user with id = {voteCommentCreate.UserId}");
        }
        var postComment = await _postCommentRepository.GetPostCommentById(voteCommentCreate.PostCommentId);
        if (postComment == null)
        {
            return new ApiResult<VoteCommentDto>(false, $"Not found postComment with id = {voteCommentCreate.PostCommentId}");
        }
        var voteComment = _mapper.Map<VoteComment>(voteCommentCreate);
        await _voteCommentRepository.CreateVoteComment(voteComment);
        var result = _mapper.Map<VoteCommentDto>(voteComment);
        return new ApiResult<VoteCommentDto>(true, result, "Create successfully");
    }

    public async Task<ApiResult<VoteCommentDto>> GetVoteCommentById(int id)
    {
        var voteComment = await _voteCommentRepository.GetVoteCommentById(id);
        if (voteComment == null)
        {
            return new ApiResult<VoteCommentDto>(false, "Not found");
        }

        var result = _mapper.Map<VoteCommentDto>(voteComment);
        return new ApiResult<VoteCommentDto>(true, result, "Get by id successfully");
    }

    public async Task<ApiResult<bool>> DeleteVoteCommentById(int id)
    {
        var voteComment = await _voteCommentRepository.GetVoteCommentById(id);
        if (voteComment == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        await _voteCommentRepository.DeleteAsync(voteComment);
        return new ApiResult<bool>(true, "Delete successfully");
    }

    public async Task<ApiResult<bool>> GetPostCommentByUserIdAndPostCommentId(string userId, int postCommentId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<bool>(false, $"Not found user with id = {userId}");
        }
        var postComment = await _postCommentRepository.GetPostCommentById(postCommentId);
        if (postComment == null)
        {
            return new ApiResult<bool>(false, $"Not found postComment with id = {postCommentId}");
        }
        var voteComment = await _voteCommentRepository.GetPostCommentByUserIdAndPostCommentId(userId,postCommentId);
        if (voteComment == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }
        return new ApiResult<bool>(true, "Get by userId and postCommentId successfully");
    }
}