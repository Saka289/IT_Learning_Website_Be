﻿using LW.Shared.SeedWork;

namespace LW.Contracts.Common;

public interface IElasticSearchService<T, K> where T : class
{
    Task<IEnumerable<T>> GetDocumentAsync(string indexName);
    Task<string> CreateDocumentAsync(string indexName, T document, Func<T, K> idExtractor);
    Task<IEnumerable<string>> CreateDocumentRangeAsync(string indexName, IEnumerable<T> document);
    Task<string> UpdateDocumentAsync(string indexName, T document, K documentId);
    Task<IEnumerable<string>> UpdateDocumentRangeAsync(string indexName, IEnumerable<T> document, Func<T, K> idExtractor);
    Task<IEnumerable<T>> SearchDocumentAsync(string indexName, SearchRequestParameters searchRequestParameters);
    Task<IEnumerable<T>> SearchAllDocumentFieldAsync(string indexName, SearchRequestValue searchRequestValue);
    Task<IEnumerable<T>> SearchDocumentFieldAsync(string indexName, SearchRequestValue searchRequestValue);
    Task<bool> DeleteDocumentAsync(string indexName, K documentId);
    Task<bool> DeleteDocumentRangeAsync(string indexName, IEnumerable<K> documentId);
}