using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MyWebApp.Services;

public class TokenService : ITokenService
{
  private readonly IConfiguration _configuration;
  public TokenService(IConfiguration configuration)
  {
    _configuration = configuration;
  }
  public string CreateToken(int userId, string email, string role)
  {
    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
      new Claim(ClaimTypes.Email, email),
      new Claim(ClaimTypes.Role, role)
    };

    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(
        _configuration["Jwt:SecretKey"]!
      )
    );

    var credentials = new SigningCredentials(
      key,
      SecurityAlgorithms.HmacSha256
    );

    var token = new JwtSecurityToken(
      issuer: _configuration["Jwt:Issuer"],
      audience: _configuration["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddHours(1),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}