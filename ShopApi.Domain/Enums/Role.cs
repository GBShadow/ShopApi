namespace ShopApi.Domain.Enums;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// No TypeScript, você provavelmente usaria uma Union Type de strings:
/// type Role = "User" | "Admin" | "Manager";
/// 
/// No C#, Enums são tipos por valor fortemente tipados (internamente inteiros por padrão, 0, 1, 2...).
/// Eles garantem segurança de tipo em tempo de compilação e podem ser mapeados
/// para strings ou inteiros no banco de dados através do Entity Framework Core.
/// </summary>
public enum Role
{
    User = 1,
    Manager = 2,
    Admin = 3
}
