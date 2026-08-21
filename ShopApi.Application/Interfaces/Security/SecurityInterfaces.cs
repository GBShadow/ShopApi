using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces.Security;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// No Node.js você usaria 'bcrypt.hash(password, 10)' e 'bcrypt.compare(password, hash)'.
/// 
/// Aqui criamos uma interface para que o serviço de autenticação não fique acoplado
/// a uma biblioteca específica de criptografia (podendo trocar BCrypt por Argon2 ou PBKDF2 facilmente).
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

/// <summary>
/// Contrato para geração e validação de tokens JWT (JSON Web Tokens)
/// </summary>
public interface ITokenService
{
    string GenerateToken(User user);
    DateTime GetExpirationDate();
}
