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
            return document;
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
            var documents = await FindAll()
                .Include(g => g.Grade)
                .Include(c => c.CommentDocuments)
                .ToListAsync();
            return documents;
        }

        public async Task<IEnumerable<Document>> GetAllDocumentByGrade(int id)
        {
            return await FindAll()
                .Include(g => g.Grade)
                .Include(c => c.CommentDocuments)
                .Where(x => x.GradeId == id).ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetAllDocumentPagination()
        {
            var result = await FindAll()
                .Include(g => g.Grade)
                .Include(c => c.CommentDocuments)
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Document>> SearchDocumentByTag(string tag, bool order)
        {
            var result = order
                ? await FindAll()
                    .Include(d => d.CommentDocuments)
                    .Where(d => d.KeyWord.Contains(tag)).OrderByDescending(d => d.CreatedDate).ToListAsync()
                : await FindAll()
                    .Include(d => d.CommentDocuments)
                    .Where(d => d.KeyWord.Contains(tag)).ToListAsync();
            return result;
        }

        public async Task<Document> GetDocumentById(int id)
        {
            var document = await FindByCondition(x => x.Id == id)
                .Include(g => g.Grade)
                .Include(c => c.CommentDocuments)
                .FirstOrDefaultAsync();
            return document;
        }

        public async Task<Document> UpdateDocument(Document document)
        {
            await UpdateAsync(document);
            return document;
        }

        public async Task<Document> GetAllDocumentIndex(int id)
        {
            return await FindAll()
                .Include(t => t.Topics.Where(t => t.ParentId == null && t.IsActive))
                .ThenInclude(ct => ct.Lessons.Where(ct => ct.IsActive).OrderBy(ct => ct.Index))
                .Include(t => t.Topics)
                .ThenInclude(c => c.ChildTopics.Where(c => c.IsActive))
                .ThenInclude(cl => cl.Lessons.Where(cl => cl.IsActive).OrderBy(cl => cl.Index))
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
        }
    }
}