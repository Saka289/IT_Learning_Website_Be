using Elasticsearch.Net;
using LW.Contracts.Common;
using LW.Shared.SeedWork;
using Nest;
using Serilog;
using SearchRequestParameters = LW.Shared.SeedWork.SearchRequestParameters;

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
        var request = new IndexRequest<T>(document, indexName, Convert.ToString(idExtractor(document)))
        {
            Refresh = Refresh.True
        };
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
            .Refresh(Refresh.True)
        );

        if (!response.ApiCall.Success)
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
            .Refresh(Refresh.True)
        );
        if (!response.IsValid)
        {
            _logger.Error($"Failed to update index document: {response.IsValid.ToString()}");
            return null;
        }

        return response.Id;
    }

    public async Task<IEnumerable<string>> UpdateDocumentRangeAsync(string indexName, IEnumerable<T> document,
        Func<T, K> idExtractor)
    {
        var bulkDescriptor = new BulkDescriptor();

        foreach (var itemDocument in document)
        {
            bulkDescriptor.Update<T>(d => d
                .Index(indexName)
                .Id(Convert.ToString(idExtractor(itemDocument)))
                .Doc(itemDocument)).Refresh(Refresh.True);
        }

        var response = await _elasticClient.BulkAsync(bulkDescriptor);
        if (!response.ApiCall.Success)
        {
            _logger.Error($"Failed to bulk update documents: {response.Errors.ToString()}");
            return null;
        }

        return response.Items.Select(i => i.Id).Where(id => !string.IsNullOrEmpty(id));
    }

    public async Task<IEnumerable<T>> SearchDocumentAsync(string indexName, SearchRequestParameters searchRequestParameters)
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

    public async Task<IEnumerable<T>> SearchAllDocumentFieldAsync(string indexName, SearchRequestValue searchRequestValue)
    {
        object searchValue = searchRequestValue.Value;
        if (int.TryParse(searchRequestValue.Value, out int intValue))
        {
            searchValue = intValue;
        }

        if (searchValue != null)
        {
            var responseSearch = await _elasticClient.SearchAsync<T>(s => s
                .Index(indexName)
                .Query(q => q
                    .QueryString(d => d
                        .Query(searchValue is int ? searchValue.ToString() : '*' + searchValue.ToString() + '*')
                        .DefaultField("*")
                    )
                )
                .Size(searchRequestValue.Size)
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
            .Size(searchRequestValue.Size)
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

    public async Task<IEnumerable<T>> SearchDocumentFieldAsync(string indexName, SearchRequestValue searchRequestValue)
    {
        if (string.IsNullOrEmpty(searchRequestValue.Value))
        {
            _logger.Information("Value is null !!!");
            return null;
        }
        
        object searchValue = searchRequestValue.Value;
        if (int.TryParse(searchRequestValue.Value, out int intValue))
        {
            searchValue = intValue;
        }
        
        var responseSearch = await _elasticClient.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(q => q
                .QueryString(d => d
                    .Query(searchValue is int ? searchValue.ToString() : '*' + searchValue.ToString() + '*')
                    .DefaultField("*")
                )
            )
            .Size(searchRequestValue.Size)
        );
        
        if (!responseSearch.IsValid || responseSearch.Total == 0)
        {
            _logger.Information($"Search query failed: {responseSearch.IsValid.ToString()}");
            return null;
        }

        var resultSearch = responseSearch.Hits.Select(hit => hit.Source).ToList();
        return resultSearch;
    }

    public async Task<bool> DeleteDocumentAsync(string indexName, K documentId)
    {
        var request = new DeleteRequest<T>(indexName, Convert.ToString(documentId))
        {
            Refresh = Refresh.True
        };
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
            bulkDescriptor.Delete<T>(d => d.Index(indexName).Id(itemDocument.ToString())).Refresh(Refresh.True);
        }

        var response = await _elasticClient.BulkAsync(bulkDescriptor);
        if (!response.ApiCall.Success)
        {
            _logger.Error($"Failed to delete document: {response.IsValid.ToString()}");
            return false;
        }

        return true;
    }
}