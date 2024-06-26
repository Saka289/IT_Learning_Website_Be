using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.CommentDocumentRepositories;
using LW.Shared.DTOs.CommentDocumentDto;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LW.Services.CommentDocumentServices;

public class CommentDocumentService : ICommentDocumentService
{
    private readonly ICommentDocumentRepository _commentDocumentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public CommentDocumentService(ICommentDocumentRepository commentDocumentRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _commentDocumentRepository = commentDocumentRepository;
        _mapper = mapper;
        _userManager = userManager;
    }
    
    public async Task<ApiResult<PagedList<CommentDocumentDto>>> GetAllCommentByDocumentIdPagination(int id, PagingRequestParameters pagingRequestParameters)
    {
        var commentList = await _commentDocumentRepository.GetAllCommentByDocumentIdPagination(id);
        if (commentList == null)
        {
            return new ApiResult<PagedList<CommentDocumentDto>>(false, "Comment is null !!!");
        }

        var result = _mapper.ProjectTo<CommentDocumentDto>(commentList);
        var pagedResult = await PagedList<CommentDocumentDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex, pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<CommentDocumentDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<CommentDocumentDto>>> GetAllCommentDocumentByUserIdPagination(string id, PagingRequestParameters pagingRequestParameters)
    {
        var commentList = await _commentDocumentRepository.GetAllCommentByUserIdPagination(id);
        if (commentList == null)
        {
            return new ApiResult<PagedList<CommentDocumentDto>>(false, "Comment is null !!!");
        }

        var result = _mapper.ProjectTo<CommentDocumentDto>(commentList);
        var pagedResult = await PagedList<CommentDocumentDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex, pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<CommentDocumentDto>>(pagedResult);
    }

    public async Task<ApiResult<CommentDocumentDto>> GetCommentDocumentById(int id)
    {
        var commentEntity = await _commentDocumentRepository.GetCommentById(id);
        if (commentEntity == null)
        {
            return new ApiResult<CommentDocumentDto>(false, "Comment is null !!!");
        }

        var result = _mapper.Map<CommentDocumentDto>(commentEntity);
        return new ApiSuccessResult<CommentDocumentDto>(result);
    }

    public async Task<ApiResult<CommentDocumentDto>> CreateCommentDocument(CommentDocumentCreateDto commentDocumentCreateDto)
    {
        var userEntity = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(commentDocumentCreateDto.UserId));
        if (userEntity == null)
        {
            return new ApiResult<CommentDocumentDto>(false, "UserId not found !!!");
        }

        var commentDocumentEntity = _mapper.Map<CommentDocument>(commentDocumentCreateDto);
        await _commentDocumentRepository.CreateComment(commentDocumentEntity);
        var result = _mapper.Map<CommentDocumentDto>(commentDocumentEntity);
        return new ApiSuccessResult<CommentDocumentDto>(result);
    }

    public async Task<ApiResult<CommentDocumentDto>> UpdateCommentDocument(CommentDocumentUpdateDto commentDocumentUpdateDto)
    {
        var commentEntity = await _commentDocumentRepository.GetCommentById(commentDocumentUpdateDto.DocumentId);
        if (commentEntity == null)
        {
            return new ApiResult<CommentDocumentDto>(false, "DocumentId not found !!!");
        }
        var userEntity = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(commentDocumentUpdateDto.UserId));
        if (userEntity == null)
        {
            return new ApiResult<CommentDocumentDto>(false, "UserId not found !!!");
        }

        var model = _mapper.Map(commentDocumentUpdateDto, commentEntity); 
        await _commentDocumentRepository.UpdateComment(model);
        var result = _mapper.Map<CommentDocumentDto>(model);
        return new ApiSuccessResult<CommentDocumentDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteCommentDocument(int id)
    {
        var commentEntity = await _commentDocumentRepository.GetCommentById(id);
        if (commentEntity is null)
        {
            return new ApiResult<bool>(false, "Comment not found !!!");
        }

        var document = await _commentDocumentRepository.DeleteComment(id);
        if (!document)
        {
            return new ApiResult<bool>(false, "Failed Delete Comment not found !!!");
        }

        return new ApiSuccessResult<bool>(true, "Delete Document Successfully !!!");
    }
}