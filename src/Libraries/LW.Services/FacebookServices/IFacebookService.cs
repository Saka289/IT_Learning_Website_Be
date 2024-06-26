using LW.Shared.SeedWork;

namespace LW.Services.FacebookServices;

public interface IFacebookService
{
    Task<ApiResult<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken);
    Task<ApiResult<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken);
}