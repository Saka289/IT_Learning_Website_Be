using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Services.GradeService;
using LW.Services.LevelServices;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;

namespace LW.Services.DocumentService;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _DocumentRepository;
    private readonly IGradeService _gradeService;
    private readonly IMapper _mapper;

    public DocumentService(IDocumentRepository DocumentRepository, IMapper mapper, IGradeService gradeService)
    {
        _DocumentRepository = DocumentRepository;
        _mapper = mapper;
        _gradeService = gradeService;
    }

    public async Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocument()
    {
        var DocumentList = await _DocumentRepository.GetAllDocument();
        if (DocumentList == null)
        {
            return new ApiResult<IEnumerable<DocumentDto>>(false, "Document is null !!!");
        }

        var result = _mapper.Map<IEnumerable<DocumentDto>>(DocumentList);
        return new ApiSuccessResult<IEnumerable<DocumentDto>>(result);
    }

    public async Task<ApiResult<DocumentDto>> GetDocumentById(int id)
    {
        var DocumentEntity = await _DocumentRepository.GetDocumentById(id);
        if (DocumentEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "Document is null !!!");
        }

        var result = _mapper.Map<DocumentDto>(DocumentEntity);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<DocumentDto>> CreateDocument(DocumentCreateDto DocumentCreateDto)
    {
        var levelEntity = await _gradeService.GetGradeById(DocumentCreateDto.GradeId);
        if (!levelEntity.IsSucceeded)
        {
            return new ApiResult<DocumentDto>(false, "GradeId not found !!!");
        }

        var DocumentEntity = _mapper.Map<Document>(DocumentCreateDto);
        DocumentEntity.IsActive = true;
        await _DocumentRepository.CreateDocument(DocumentEntity);
        var result = _mapper.Map<DocumentDto>(DocumentEntity);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<DocumentDto>> UpdateDocument(DocumentUpdateDto DocumentUpdateDto)
    {
        var levelEntity = await _gradeService.GetGradeById(DocumentUpdateDto.GradeId);
        if (!levelEntity.IsSucceeded)
        {
            return new ApiResult<DocumentDto>(false, "LevelId not found !!!");
        }

        var DocumentEntity = await _DocumentRepository.GetByIdAsync(DocumentUpdateDto.Id);
        if (DocumentEntity is null)
        {
            return new ApiResult<DocumentDto>(false, "Document not found !!!");
        }

        var model = _mapper.Map(DocumentUpdateDto, DocumentEntity);
        var updateDocument = await _DocumentRepository.UpdateDocument(model);

        var result = _mapper.Map<DocumentDto>(updateDocument);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteDocument(int id)
    {
        var DocumentEntity = await _DocumentRepository.GetByIdAsync(id);
        if (DocumentEntity is null)
        {
            return new ApiResult<bool>(false, "Document not found !!!");
        }

        var Document = await _DocumentRepository.DeleteDocument(id);
        if (!Document)
        {
            return new ApiResult<bool>(false, "Failed Delete Document not found !!!");
        }

        return new ApiSuccessResult<bool>(true, "Delete Document Successfully !!!");
    }
}