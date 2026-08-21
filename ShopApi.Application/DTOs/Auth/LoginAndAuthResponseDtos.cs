using System.ComponentModel.DataAnnotations;
using ShopApi.Application.DTOs.Users;

namespace ShopApi.Application.DTOs.Auth;

/// <summary>
/// DTO de entrada para login (Body da requisição POST /api/auth/login)
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO de resposta de autenticação com Token JWT e dados do usuário logado
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public DateTime ExpiresAt { get; set; }

    public UserResponseDto User { get; set; } = default!;
}
