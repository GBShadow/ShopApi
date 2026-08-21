using ShopApi.Domain.Entities;
using ShopApi.Domain.Enums;

namespace ShopApi.Application.DTOs.Users;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// NUNCA retorne a entidade do banco de dados diretamente na resposta da API!
/// Se você retornar a entidade 'User', poderá vazar acidentalmente 'PasswordHash' para o cliente.
/// 
/// O DTO (Data Transfer Object) define o contrato público exato do que a API entrega.
/// O método estático 'FromEntity' funciona como um factory/mapper limpo (similar ao class-transformer no NestJS).
/// </summary>
public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static UserResponseDto FromEntity(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}

public class UpdateRoleDto
{
    public Role Role { get; set; }
}
