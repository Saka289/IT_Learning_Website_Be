using System.Net;
using System.Text;
using LW.Contracts.Common;
using LW.Shared.Configurations;
using LW.Shared.DTOs.Request;
using LW.Shared.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace LW.Infrastructure.Common;

public class BaseService<T> : IBaseService<T> where T : class
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UrlBase _urlBase;
    private readonly ILogger _logger;

    public BaseService(IHttpClientFactory httpClientFactory, ILogger logger, IOptions<UrlBase> urlBase)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _urlBase = urlBase.Value;
    }

    public async Task<T?> SendAsync<T>(RequestDto requestDto)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("SendAPI");
            HttpRequestMessage message = new();
            if (requestDto.ContentType == EContentType.MultipartFormData)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }

            if (_urlBase.Judge0IsActive)
            {
                message.Headers.Add(_urlBase.Judge0Host!, _urlBase.Judge0HostValue);
                message.Headers.Add(_urlBase.Judge0Key!, _urlBase.Judge0KeyValue);
            }

            message.RequestUri = new Uri(requestDto.Url);

            if (requestDto.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8,
                    "application/json");
            }

            HttpResponseMessage? apiResponse = null;

            switch (requestDto.ApiType)
            {
                case EApiType.Post:
                    message.Method = HttpMethod.Post;
                    break;
                case EApiType.Delete:
                    message.Method = HttpMethod.Delete;
                    break;
                case EApiType.Put:
                    message.Method = HttpMethod.Put;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await client.SendAsync(message);

            if (apiResponse.StatusCode != HttpStatusCode.OK && apiResponse.StatusCode != HttpStatusCode.Accepted && apiResponse.StatusCode != HttpStatusCode.Created)
            {
                _logger.Error($"HTTP Error: {apiResponse.StatusCode}");
                return default(T);
            }

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(apiContent);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return default(T);
        }
    }
}