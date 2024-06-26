using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LW.Data.Entities;
using LW.Shared.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LW.Services.JwtTokenServices;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateAccessToken(ApplicationUser applicationUser, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var signingCredentials =
            new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

        var claimList = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName),
            new Claim(JwtRegisteredClaimNames.FamilyName, applicationUser.FirstName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.GivenName, applicationUser.LastName ?? string.Empty),
            new Claim("picture", applicationUser.Image ?? string.Empty),
        };

        claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = signingCredentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var numberGenerator = RandomNumberGenerator.Create())
        {
            numberGenerator.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}