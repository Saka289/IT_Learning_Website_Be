using LW.Contracts.Common;
using LW.Shared.SeedWork;
using Nest;
using Serilog;

namespace LW.Infrastructure.Common;

public class ElasticSearchService<T, K> : IElasticSearchService<T, K> where T : class
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger _logger;

    public ElasticSearchService(IElasticClient elasticClient, ILogger logger)
    {
        _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<T>> GetDocumentAsync(string indexName)
    {
        var response = await _elasticClient.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(q => q
                .MatchAll()
            )
        );
        if (!response.IsValid || response.Total == 0)
        {
            _logger.Error($"Get all failed: {response.IsValid.ToString()}");
            return null;
        }

        var result = response.Hits.Select(hit => hit.Source).ToList();
        return result;
    }

    public async Task<string> CreateDocumentAsync(string indexName, T document, Func<T, K> idExtractor)
    {
        var request = new IndexRequest<T>(document, indexName, Convert.ToString(idExtractor(document)));
        var response = await _elasticClient.IndexAsync(request);
        if (!response.IsValid)
        {
            _logger.Error($"Failed to index document: {response.IsValid.ToString()}");
            return response.OriginalException.Message;
        }

        return response.Id;
    }

    public async Task<IEnumerable<string>> CreateDocumentRangeAsync(string indexName, IEnumerable<T> document)
    {
        var response = await _elasticClient.BulkAsync(b => b
            .Index(indexName)
            .IndexMany(document)
        );

        if (!response.IsValid)
        {
            _logger.Error($"Bulk indexing failed: {response.IsValid.ToString()}");
            return null;
        }

        return response.Items.Select(i => i.Id);
    }

    public async Task<string> UpdateDocumentAsync(string indexName, T document, K documentId)
    {
        var response = await _elasticClient.UpdateAsync<T>(Convert.ToString(documentId), u => u
            .Index(indexName)
            .Doc(document)
        );
        if (!response.IsValid)
        {
            _logger.Error($"Failed to update index document: {response.IsValid.ToString()}");
            return response.OriginalException.Message;
        }

        return response.Id;
    }

    public async Task<IEnumerable<T>> SearchDocumentAsync(string indexName,
        SearchRequestParameters searchRequestParameters)
    {
        object searchValue = searchRequestParameters.Value;
        if (int.TryParse(searchRequestParameters.Value, out int intValue))
        {
            searchValue = intValue;
        }

        if (searchValue != null)
        {
            var responseSearch = await _elasticClient.SearchAsync<T>(s => s
                .Index(indexName)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .QueryString(d => d
                                .Query(searchValue is int ? searchValue.ToString() : '*' + searchValue.ToString() + '*')
                                .Fields(f => f
                                    .Field(searchRequestParameters.Key)
                                )
                            )
                        )
                    )
                )
                .Size(searchRequestParameters.Size)
            );
            if (!responseSearch.IsValid || responseSearch.Total == 0)
            {
                _logger.Information($"Search query failed: {responseSearch.IsValid.ToString()}");
                return null;
            }

            var resultSearch = responseSearch.Hits.Select(hit => hit.Source).ToList();
            return resultSearch;
        }

        var response = await _elasticClient.SearchAsync<T>(s => s
            .Index(indexName)
            .Size(searchRequestParameters.Size)
            .Sort(o => o.Ascending("id"))
            .Query(q => q
                .MatchAll()
            )
        );

        if (!response.IsValid || response.Total == 0)
        {
            _logger.Error($"Get all failed: {response.IsValid.ToString()}");
            return null;
        }

        var result = response.Hits.Select(hit => hit.Source).ToList();
        return result;
    }

    public async Task<bool> DeleteDocumentAsync(string indexName, K documentId)
    {
        var request = new DeleteRequest<T>(indexName, Convert.ToString(documentId));
        var response = await _elasticClient.DeleteAsync(request);
        if (!response.IsValid)
        {
            _logger.Error($"Failed to delete document: {response.IsValid.ToString()}");
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteDocumentRangeAsync(string indexName, IEnumerable<K> documentId)
    {
        var bulkDescriptor = new BulkDescriptor();

        foreach (var itemDocument in documentId)
        {
            bulkDescriptor.Delete<T>(d => d.Index(indexName).Id(itemDocument.ToString()));
        }

        var response = await _elasticClient.BulkAsync(bulkDescriptor);
        if (!response.IsValid)
        {
            _logger.Error($"Failed to delete document: {response.IsValid.ToString()}");
            return false;
        }

        return true;
    }
}