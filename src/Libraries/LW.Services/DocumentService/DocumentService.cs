using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Services.GradeService;
using LW.Services.LevelServices;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;

namespace LW.Services.DocumentService;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IMapper _mapper;

    public DocumentService(IDocumentRepository documentRepository, IMapper mapper, IGradeRepository gradeRepository)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
        _gradeRepository = gradeRepository;
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

    public async Task<ApiResult<DocumentDto>> GetDocumentById(int id)
    {
        var documentEntity = await _documentRepository.GetDocumentById(id);
        if (documentEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "Document is null !!!");
        }

        var result = _mapper.Map<DocumentDto>(documentEntity);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<DocumentDto>> CreateDocument(DocumentCreateDto documentCreateDto)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(documentCreateDto.GradeId);
        if (gradeEntity==null)
        {
            return new ApiResult<DocumentDto>(false, "GradeId not found !!!");
        }

        var documentEntity = _mapper.Map<Document>(documentCreateDto);
        documentEntity.IsActive = true;
        await _documentRepository.CreateDocument(documentEntity);
        var result = _mapper.Map<DocumentDto>(documentEntity);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<DocumentDto>> UpdateDocument(DocumentUpdateDto documentUpdateDto)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(documentUpdateDto.GradeId);
        if (gradeEntity==null)
        {
            return new ApiResult<DocumentDto>(false, "LevelId not found !!!");
        }

        var documentEntity = await _documentRepository.GetByIdAsync(documentUpdateDto.Id);
        if (documentEntity is null)
        {
            return new ApiResult<DocumentDto>(false, "Document not found !!!");
        }

        var model = _mapper.Map(documentUpdateDto, documentEntity);
        var updateDocument = await _documentRepository.UpdateDocument(model);

        var result = _mapper.Map<DocumentDto>(updateDocument);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteDocument(int id)
    {
        var documentEntity = await _documentRepository.GetByIdAsync(id);
        if (documentEntity is null)
        {
            return new ApiResult<bool>(false, "Document not found !!!");
        }

        var document = await _documentRepository.DeleteDocument(id);
        if (!document)
        {
            return new ApiResult<bool>(false, "Failed Delete Document not found !!!");
        }

        return new ApiSuccessResult<bool>(true, "Delete Document Successfully !!!");
    }
}