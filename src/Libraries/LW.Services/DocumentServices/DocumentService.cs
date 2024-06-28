using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.DocumentServices;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IElasticSearchService<DocumentDto, int> _elasticSearchService;
    private readonly IMapper _mapper;

    public DocumentService(IDocumentRepository documentRepository, IMapper mapper, IGradeRepository gradeRepository,
        IElasticSearchService<DocumentDto, int> elasticSearchService)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
        _gradeRepository = gradeRepository;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocument()
    {
        var documentList = await _documentRepository.GetAllDocument();
        if (documentList == null)
        {
            return new ApiResult<IEnumerable<DocumentDto>>(false, "Document is null !!!");
        }

        var result = _mapper.Map<IEnumerable<DocumentDto>>(documentList);
        return new ApiSuccessResult<IEnumerable<DocumentDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocumentByGrade(int id)
    {
        var documentList = await _documentRepository.GetAllDocumentByGrade(id);
        if (documentList == null)
        {
            return new ApiResult<IEnumerable<DocumentDto>>(false, "Document is null !!!");
        }

        var result = _mapper.Map<IEnumerable<DocumentDto>>(documentList);
        return new ApiSuccessResult<IEnumerable<DocumentDto>>(result);
    }

    public async Task<ApiResult<PagedList<DocumentDto>>> GetAllDocumentPagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var documentList = await _documentRepository.GetAllDocumentPagination();
        if (documentList == null)
        {
            return new ApiResult<PagedList<DocumentDto>>(false, "Document is null !!!");
        }

        var result = _mapper.ProjectTo<DocumentDto>(documentList);
        var pagedResult = await PagedList<DocumentDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<DocumentDto>>(pagedResult);
    }

    public async Task<ApiResult<DocumentDto>> GetDocumentById(int id)
    {
        var documentEntity = await _documentRepository.GetDocumentById(id);
        if (documentEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "Document is null !!!");
        }

        var gradeEntity = await _gradeRepository.GetGradeById(documentEntity.GradeId);
        if (gradeEntity != null)
        {
            documentEntity.Grade = gradeEntity;
        }

        var result = _mapper.Map<DocumentDto>(documentEntity);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<PagedList<DocumentDto>>> SearchByDocumentPagination(SearchDocumentDto searchDocumentDto)
    {
        var documentEntity =
            await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticDocuments, searchDocumentDto);
        if (documentEntity is null)
        {
            return new ApiResult<PagedList<DocumentDto>>(false, $"Document not found by {searchDocumentDto.Key} !!!");
        }

        if (searchDocumentDto.GradeId > 0)
        {
            documentEntity = documentEntity.Where(d => d.GradeId == searchDocumentDto.GradeId).ToList();
        }

        var result = _mapper.Map<IEnumerable<DocumentDto>>(documentEntity);
        var pagedResult = await PagedList<DocumentDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchDocumentDto.PageIndex, searchDocumentDto.PageSize, searchDocumentDto.OrderBy,
            searchDocumentDto.IsAscending);
        return new ApiSuccessResult<PagedList<DocumentDto>>(pagedResult);
    }

    public async Task<ApiResult<DocumentDto>> CreateDocument(DocumentCreateDto documentCreateDto)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(documentCreateDto.GradeId);
        if (gradeEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "GradeID not found !!!");
        }

        var documentEntity = _mapper.Map<Document>(documentCreateDto);
        documentEntity.KeyWord = documentEntity.Title.RemoveDiacritics();
        await _documentRepository.CreateDocument(documentEntity);
        documentEntity.Grade = gradeEntity;
        var result = _mapper.Map<DocumentDto>(documentEntity);
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticDocuments, result, g => g.Id);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<DocumentDto>> UpdateDocument(DocumentUpdateDto documentUpdateDto)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(documentUpdateDto.GradeId);
        if (gradeEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "GradeID not found !!!");
        }

        var documentEntity = await _documentRepository.GetByIdAsync(documentUpdateDto.Id);
        if (documentEntity is null)
        {
            return new ApiResult<DocumentDto>(false, "Document not found !!!");
        }

        var model = _mapper.Map(documentUpdateDto, documentEntity);
        model.KeyWord = documentUpdateDto.Title.RemoveDiacritics();
        var updateDocument = await _documentRepository.UpdateDocument(model);
        updateDocument.Grade = gradeEntity;
        var result = _mapper.Map<DocumentDto>(updateDocument);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticDocuments, result, documentUpdateDto.Id);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateStatusDocument(int id)
    {
        var documentEntity = await _documentRepository.GetDocumentById(id);
        if (documentEntity is null)
        {
            return new ApiResult<bool>(false, "Document not found !!!");
        }

        var gradeEntity = await _gradeRepository.GetGradeById(documentEntity.GradeId);

        documentEntity.IsActive = !documentEntity.IsActive;
        await _documentRepository.UpdateDocument(documentEntity);
        await _documentRepository.SaveChangesAsync();
        documentEntity.Grade = gradeEntity;
        var result = _mapper.Map<DocumentDto>(documentEntity);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticDocuments, result, id);
        return new ApiSuccessResult<bool>(true, "Grade update successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteDocument(int id)
    {
        var documentEntity = await _documentRepository.GetDocumentById(id);
        if (documentEntity is null)
        {
            return new ApiResult<bool>(false, "Document not found !!!");
        }

        var document = await _documentRepository.DeleteDocument(id);
        if (!document)
        {
            return new ApiResult<bool>(false, "Failed Delete Document not found !!!");
        }

        _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticDocuments, id);

        return new ApiSuccessResult<bool>(true, "Delete Document Successfully !!!");
    }
}