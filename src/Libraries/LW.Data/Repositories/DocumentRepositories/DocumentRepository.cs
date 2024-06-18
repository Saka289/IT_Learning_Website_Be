using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.Data.Repositories.DocumentRepositories
{
    public class DocumentRepository : RepositoryBase<Document, int>, IDocumentRepository
    {
        public DocumentRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public async Task<Document> CreateDocument(Document Document)
        {
            await CreateAsync(Document);
            return await  Task.FromResult(Document) ;
        }

        public async Task<bool> DeleteDocument(int id)
        {
            var document = await GetByIdAsync(id);
            if(document == null)
            {
                return false; 
            }
            await DeleteAsync(document);
            return true;
        }

        public async Task<IEnumerable<Document>> GetAllDocument()
        {
            var documents = await FindAll().ToListAsync();
            return documents;
        }

        public async Task<Document> GetDocumentById(int id)
        {
            var document = await GetByIdAsync(id);
            return document;
        }

        public async Task<Document> UpdateDocument(Document Document)
        {
            await UpdateAsync(Document);
            return await Task.FromResult(Document);
        }
    }
}
