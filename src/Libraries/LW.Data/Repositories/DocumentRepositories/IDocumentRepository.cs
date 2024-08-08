using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.DocumentRepositories;

public interface IDocumentRepository : IRepositoryBase<Document, int>
{
    Task<IEnumerable<Document>> GetAllDocument();
    Task<IEnumerable<Document>> GetAllDocumentByGrade(int id);
    Task<IEnumerable<Document>> GetAllDocumentPagination();
    Task<Document> GetDocumentById(int id);
    Task<Document> CreateDocument(Document document);
    Task<Document> UpdateDocument(Document document);
    Task<bool> DeleteDocument(int id);
    Task<Document> GetAllDocumentIndex(int id);
}
