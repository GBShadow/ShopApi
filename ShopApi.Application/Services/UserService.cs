using Microsoft.EntityFrameworkCore;
using ShopApi.Application.DTOs.Users;
using ShopApi.Application.Interfaces.Common;
using ShopApi.Application.Interfaces.Services;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);

        return users.Select(UserResponseDto.FromEntity);
    }

    public async Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"Usuário com ID '{id}' não foi encontrado.");
        }

        return UserResponseDto.FromEntity(user);
    }

    public async Task<UserResponseDto> UpdateRoleAsync(Guid id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"Usuário com ID '{id}' não foi encontrado.");
        }

        user.Role = dto.Role;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
