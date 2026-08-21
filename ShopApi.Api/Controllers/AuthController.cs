using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.DTOs.Auth;
using ShopApi.Application.DTOs.Users;
using ShopApi.Application.Interfaces.Services;

namespace ShopApi.Api.Controllers;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Express Routes / NestJS AuthController vs C#):
/// No Express: router.post('/register', authMiddleware, controller.register)
/// No NestJS: @Controller('auth') com @Post('register')
/// 
/// No C# ASP.NET Core:
/// - [AllowAnonymous] permite acesso público sem token.
/// - [Authorize] exige um Token JWT válido no cabeçalho 'Authorization: Bearer <TOKEN>'.
/// - 'ActionResult<T>' permite retornar tanto o tipo tipado 'T' com 'Ok(data)' (200) quanto 'CreatedAtAction' (201).
/// </summary>
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    /// Cadastra um novo usuário no sistema
    /// </summary>
    /// <param name="dto">Dados de cadastro do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição</param>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterRequestDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Realiza login e gera o Token JWT
    /// </summary>
    /// <param name="dto">Credenciais de e-mail e senha</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição</param>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retorna as informações do usuário autenticado no momento (baseado no Token JWT enviado)
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponseDto>> GetProfile(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        var user = await _userService.GetByIdAsync(currentUserId, cancellationToken);
        return Ok(user);
    }
}
