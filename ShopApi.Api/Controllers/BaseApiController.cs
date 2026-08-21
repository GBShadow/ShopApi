using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Api.Controllers;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript/NestJS vs C#):
/// ControllerBase é a classe base para APIs REST sem suporte a Views HTML (Razor/MVC).
/// O atributo [ApiController] ativa validação automática de modelos (ModelState), inferência de bindings ([FromBody], [FromQuery])
/// e respostas padrão HTTP 400 em caso de erros de validação.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Extrai o ID do usuário logado a partir das Claims do Token JWT (ClaimTypes.NameIdentifier)
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Usuário não autenticado ou ID inválido no token.");
        }

        return userId;
    }

    /// <summary>
    /// Extrai o e-mail do usuário autenticado
    /// </summary>
    protected string GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Extrai o Papel/Role do usuário autenticado (User, Admin, Manager)
    /// </summary>
    protected string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
