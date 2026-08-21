using ShopApi.Application.Interfaces.Security;

namespace ShopApi.Infrastructure.Security;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// No Node.js você usaria o pacote 'bcrypt' ou 'bcryptjs'.
/// 
/// Aqui no C#, usamos o pacote 'BCrypt.Net-Next'.
/// O fator de trabalho (work factor / salt rounds) padrão é 11 ou 12,
/// oferecendo excelente resistência contra ataques de força bruta.
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Gera o salt automaticamente e retorna o hash criptográfico
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 11);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        // Compara a senha digitada em texto plano com o hash salvo no banco
        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
