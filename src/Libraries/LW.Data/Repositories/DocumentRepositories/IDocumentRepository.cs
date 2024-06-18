using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.DocumentRepositories;

public interface IDocumentRepository : IRepositoryBase<Document, int>
{
    Task<IEnumerable<Document>> GetAllDocument();
    Task<Document> GetDocumentById(int id);
    Task<Document> CreateDocument(Document Document);
    Task<Document> UpdateDocument(Document Document);
    Task<bool> DeleteDocument(int id);
}
