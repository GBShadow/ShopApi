using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.DTOs.Users;
using ShopApi.Application.Interfaces.Services;

namespace ShopApi.Api.Controllers;

/// <summary>
/// Controlador de Gestão de Usuários (Acesso restrito exclusivamente para administradores)
/// </summary>
[Authorize(Roles = "Admin")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Lista todos os usuários cadastrados na plataforma (Exclusivo Admin)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Busca detalhes de um usuário pelo ID (Exclusivo Admin)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Altera o papel / permissão (Role) de um usuário (Exclusivo Admin)
    /// </summary>
    [HttpPatch("{id:guid}/role")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> UpdateRole(
        Guid id,
        [FromBody] UpdateRoleDto dto,
        CancellationToken cancellationToken)
    {
        var updatedUser = await _userService.UpdateRoleAsync(id, dto, cancellationToken);
        return Ok(updatedUser);
    }
}
