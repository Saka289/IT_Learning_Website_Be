using LW.Shared.SeedWork;

namespace LW.Services.Common.CommonServices.FacebookServices;

public interface IFacebookService
{
    Task<FacebookTokenValidationResponse?> ValidateFacebookToken(string accessToken);
    Task<FacebookUserInfoResponse?> GetFacebookUserInformation(string accessToken);
}