using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LW.Data.Entities;
using LW.Shared.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LW.Services.JwtTokenService;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

        var claimList = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName),
            new Claim(JwtRegisteredClaimNames.FamilyName, applicationUser.FirstName),
            new Claim(JwtRegisteredClaimNames.GivenName, applicationUser.LastName),
            new Claim("picture", applicationUser.Image ?? string.Empty),
        };

        claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = signingCredentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}