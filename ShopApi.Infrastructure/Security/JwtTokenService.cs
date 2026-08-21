using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Application.Interfaces.Security;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Security;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (jsonwebtoken no Node vs C#):
/// No Node.js (jsonwebtoken):
/// jwt.sign({ id: user.id, role: user.role }, process.env.JWT_SECRET, { expiresIn: '8h' })
/// 
/// No C# ASP.NET Core:
/// Usamos o conceito formal de 'Claims' (afirmações sobre a identidade do usuário).
/// O 'ClaimTypes.Role' é crucial! O ASP.NET Core lê essa claim automaticamente para validar
/// os atributos [Authorize(Roles = "Admin")] nos Controllers (RBAC nativo).
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var secretKey = _configuration["Jwt:Secret"] 
            ?? throw new InvalidOperationException("A chave JWT 'Jwt:Secret' não foi configurada no appsettings.json.");

        var issuer = _configuration["Jwt:Issuer"] ?? "ShopApi";
        var audience = _configuration["Jwt:Audience"] ?? "ShopApiUsers";
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationInMinutes"], out var minutes) ? minutes : 480;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims: informações embutidas dentro do Payload do Token JWT
        var claims = new List<Claim>
        {
            // ID do usuário (Subject / NameIdentifier)
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            // Nome e E-mail
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),

            // Papel / Role do Usuário (ESSENCIAL PARA RBAC)
            new(ClaimTypes.Role, user.Role.ToString()),

            // Identificador único do token para evitar replay attacks
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public DateTime GetExpirationDate()
    {
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationInMinutes"], out var minutes) ? minutes : 480;
        return DateTime.UtcNow.AddMinutes(expirationMinutes);
    }
}
