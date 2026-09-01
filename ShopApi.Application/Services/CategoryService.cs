using Microsoft.EntityFrameworkCore;
using ShopApi.Application.DTOs.Categories;
using ShopApi.Application.Interfaces.Common;
using ShopApi.Application.Interfaces.Services;
using ShopApi.Domain.Entities;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(
        CancellationToken cancellationToken
    )
    {
        var categories = await _context
            .Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(CategoryResponseDto.FromEntity);
    }

    public async Task<CategoryResponseDto> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var category = await _context
            .Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException($"Categoria com ID '{id}' não foi encontrada");
        }

        return CategoryResponseDto.FromEntity(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken
    )
    {
        var nameNormalized = dto.Name.Trim();

        var exists = await _context.Categories.AnyAsync(
            c => c.Name.ToLower() == nameNormalized.ToLower(),
            cancellationToken
        );

        if (exists)
        {
            throw new NotFoundException($"Já existe uma categoria com o nome '{nameNormalized}'.");
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return CategoryResponseDto.FromEntity(category);
    }

    public async Task<CategoryResponseDto> UpdateAsync(
        Guid id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken
    )
    {
        var category = await _context.Categories.FirstOrDefaultAsync(
            c => c.Id == id,
            cancellationToken
        );

        if (category == null)
            throw new NotFoundException($"Categoria com ID '{id}' não foi encontrada.");

        var nameNormalized = dto.Name.Trim();

        var duplicateName = await _context.Categories.AnyAsync(
            c => c.Name.ToLower() == nameNormalized.ToLower() && c.Id != id,
            cancellationToken
        );

        if (duplicateName)
            throw new ConflictException(
                $"Já existe outra categoria com este nome '{nameNormalized}'."
            );

        category.Name = nameNormalized;
        category.Description = dto.Description.Trim() ?? String.Empty;
        category.IsActive = dto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(
            c => c.Id == id,
            cancellationToken
        );

        if (category == null)
            throw new NotFoundException($"Categoria com ID '{id}' não foi encontrada.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
