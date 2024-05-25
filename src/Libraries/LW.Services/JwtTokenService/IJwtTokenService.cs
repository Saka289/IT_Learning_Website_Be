using LW.Data.Entities;

namespace LW.Services.JwtTokenService;

public interface IJwtTokenService
{
    public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
}