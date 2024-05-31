using LW.Contracts.Common;
using LW.Shared.Configurations;
using LW.Shared.SeedWork;
using Microsoft.Extensions.Options;
using Serilog;

namespace LW.Services.FacebookService;

public class FacebookService : IFacebookService
{
    private readonly HttpClient _httpClient;
    private readonly FacebookSettings _facebookSettings;
    private readonly ISerializeService _serializeService;
    private readonly ILogger _logger;

    public FacebookService(IHttpClientFactory httpClientFactory, IOptions<FacebookSettings> facebookSettings,
        ILogger logger, ISerializeService serializeService)
    {
        _httpClient = httpClientFactory.CreateClient("Facebook");
        _facebookSettings = facebookSettings.Value;
        _logger = logger;
        _serializeService = serializeService;
    }

    public async Task<ApiResult<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken)
    {
        try
        {
            string tokenValidationUrl = _facebookSettings.TokenValidationUrl;
            var url = string.Format(tokenValidationUrl, accessToken, _facebookSettings.AppId, _facebookSettings.AppSecret); 
            
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var tokenValidationResponse =
                    _serializeService.Deserialize<FacebookTokenValidationResponse>(responseAsString);
                return new ApiSuccessResult<FacebookTokenValidationResponse>(tokenValidationResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
        }

        return new ApiResult<FacebookTokenValidationResponse>(false, null, "Failed to get response");
    }

    public async Task<ApiResult<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken)
    {
        try
        {
            string userInfoUrl = _facebookSettings.UserInfoUrl;
            string url = string.Format(userInfoUrl, accessToken);

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var userInfoResponse = _serializeService.Deserialize<FacebookUserInfoResponse>(responseAsString);
                return new ApiSuccessResult<FacebookUserInfoResponse>(userInfoResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
        }

        return new ApiResult<FacebookUserInfoResponse>(false, null, "Failed to get response");
    }
}