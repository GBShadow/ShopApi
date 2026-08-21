using ShopApi.Domain.Enums;

namespace ShopApi.Domain.Entities;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// No Prisma, você definiria isso no 'schema.prisma':
/// model User {
///   id        String   @id @default(uuid())
///   name      String
///   email     String   @unique
///   password  String
///   role      Role     @default(User)
///   createdAt DateTime @default(now())
///   updatedAt DateTime @updatedAt
/// }
/// 
/// No Entity Framework Core (Code-First), criamos classes puras em C# (POCOs - Plain Old CLR Objects).
/// 'string.Empty' é uma boa prática para inicializar strings no C# moderno para evitar avisos de nulidade (nullability warnings).
/// </summary>
public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Nunca salvamos a senha em texto puro! Guardamos o hash criptografado (BCrypt)
    public string PasswordHash { get; set; } = string.Empty;

    // Papel do usuário no sistema (RBAC: Role-Based Access Control)
    public Role Role { get; set; } = Role.User;
}
