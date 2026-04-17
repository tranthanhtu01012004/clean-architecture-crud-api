using System.IdentityModel.Tokens.Jwt; // JWT classes
using System.Security.Claims; // Claim
using System.Text; // Encoding
using Microsoft.IdentityModel.Tokens; // SigningCredentials, SymmetricSecurityKey

namespace CleanCrudDemo.Api.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration; // Đọc config từ appsettings.json

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username, string role)
    {
        var jwtSection = _configuration.GetSection("Jwt"); // Lấy section Jwt

        var key = jwtSection["Key"]
                  ?? throw new InvalidOperationException("JWT Key is missing.");

        var issuer = jwtSection["Issuer"]
                     ?? throw new InvalidOperationException("JWT Issuer is missing.");

        var audience = jwtSection["Audience"]
                       ?? throw new InvalidOperationException("JWT Audience is missing.");

        var expireMinutes = int.TryParse(jwtSection["ExpireMinutes"], out var minutes)
            ? minutes
            : 120;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username), // Claim username
            new(ClaimTypes.Role, role), // Claim role
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT id
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)); // Secret key
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // Ký token

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token); // Trả token dạng string
    }
}