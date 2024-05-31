using LW.Shared.SeedWork;

namespace LW.Services.FacebookService;

public interface IFacebookService
{
    Task<ApiResult<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken);
    Task<ApiResult<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken);
}