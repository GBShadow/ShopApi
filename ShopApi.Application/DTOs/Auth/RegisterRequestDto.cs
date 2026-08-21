using System.ComponentModel.DataAnnotations;
using ShopApi.Domain.Enums;

namespace ShopApi.Application.DTOs.Auth;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// No NestJS/Node.js, você usaria 'class-validator' (@IsEmail(), @IsNotEmpty(), @MinLength(6))
/// ou Zod no Express (z.object({ email: z.string().email(), ... })).
/// 
/// No C#, usamos atributos de 'System.ComponentModel.DataAnnotations'.
/// O ASP.NET Core valida esses atributos automaticamente quando a requisição chega no Controller!
/// Se algum campo falhar, ele retorna automaticamente um HTTP 400 Bad Request detalhado.
/// </summary>
public class RegisterRequestDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve conter no mínimo 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    // Papel opcional no cadastro (padrão é User se não informado)
    public Role Role { get; set; } = Role.User;
}
