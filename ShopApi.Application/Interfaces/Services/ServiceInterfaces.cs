using ShopApi.Application.DTOs.Auth;
using ShopApi.Application.DTOs.Products;
using ShopApi.Application.DTOs.Users;

namespace ShopApi.Application.Interfaces.Services;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// 'Task<T>' em C# é o equivalente direto de 'Promise<T>' no TypeScript/JavaScript.
/// 'async/await' no C# funciona de forma muito similar ao JS/TS (baseado em State Machine assíncrona).
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);
}

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponseDto> UpdateRoleAsync(Guid id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
}
