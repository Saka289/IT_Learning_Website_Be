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

        public async Task<Document> CreateDocument(Document document)
        {
            await CreateAsync(document);
            return await Task.FromResult(document);
        }

        public async Task<bool> DeleteDocument(int id)
        {
            var document = await GetByIdAsync(id);
            if (document == null)
            {
                return false;
            }

            await DeleteAsync(document);
            return true;
        }

        public async Task<IEnumerable<Document>> GetAllDocument()
        {
            var documents = await FindAll().Include(g => g.Grade).ToListAsync();
            return documents;
        }

        public async Task<IEnumerable<Document>> GetAllDocumentByGrade(int id)
        {
            return await FindAll().Include(g => g.Grade).Where(x => x.GradeId == id).ToListAsync();
        }

        public Task<IQueryable<Document>> GetAllDocumentPagination()
        {
            var result = FindAll().Include(g => g.Grade).AsQueryable();
            return Task.FromResult(result);
        }

        public async Task<Document> GetDocumentById(int id)
        {
            var document = await GetByIdAsync(id);
            return document;
        }

        public async Task<Document> UpdateDocument(Document document)
        {
            await UpdateAsync(document);
            return await Task.FromResult(document);
        }

        public async Task<Document> GetAllDocumentIndex(int id)
        {
            return await FindAll()
                .Include(t => t.Topics.Where(t => t.ParentId == null && t.IsActive))
                .ThenInclude(ct => ct.Lessons.Where(ct => ct.IsActive))
                .Include(t => t.Topics)
                .ThenInclude(c => c.ChildTopics.Where(c => c.IsActive))
                .ThenInclude(cl => cl.Lessons.Where(cl => cl.IsActive))
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
        }
    }
}