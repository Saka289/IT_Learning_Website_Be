using System.Security.Claims;
using LW.Data.Entities;

namespace LW.Services.JwtTokenService;

public interface IJwtTokenService
{
    public string GenerateAccessToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    public string GenerateRefreshToken();
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}