using Microsoft.EntityFrameworkCore;
using ShopApi.Application.DTOs.Products;
using ShopApi.Application.Interfaces.Common;
using ShopApi.Application.Interfaces.Services;
using ShopApi.Domain.Entities;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Application.Services;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Performance e EF Core):
/// '.AsNoTracking()': Por padrão, o Entity Framework rastreia todas as entidades retornadas na memória
/// para detectar se alguma propriedade foi alterada. Em consultas somente leitura (GET), o rastreamento é
/// desperdício de CPU/RAM. Usar '.AsNoTracking()' desativa esse rastreamento e acelera drasticamente as consultas!
/// </summary>
public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // SELECT * FROM Products ORDER BY CreatedAt DESC
        var products = await _context.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        // Mapeia a lista de entidades para DTOs (equivalente ao Array.map no JS)
        return products.Select(ProductResponseDto.FromEntity);
    }

    public async Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException($"Produto com ID '{id}' não foi encontrado.");
        }

        return ProductResponseDto.FromEntity(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        // Regra de negócio: não permitir dois produtos com o mesmo nome exato
        var nameExists = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == dto.Name.Trim().ToLower(), cancellationToken);

        if (nameExists)
        {
            throw new ConflictException($"Já existe um produto cadastrado com o nome '{dto.Name}'.");
        }

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            Stock = dto.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        // Aqui NÃO usamos AsNoTracking() porque vamos alterar as propriedades da entidade
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException($"Produto com ID '{id}' não foi encontrado.");
        }

        // Verifica se outro produto já possui esse nome (excluindo o atual)
        var nameConflict = await _context.Products
            .AnyAsync(p => p.Id != id && p.Name.ToLower() == dto.Name.Trim().ToLower(), cancellationToken);

        if (nameConflict)
        {
            throw new ConflictException($"Já existe outro produto cadastrado com o nome '{dto.Name}'.");
        }

        product.Name = dto.Name.Trim();
        product.Description = dto.Description.Trim();
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException($"Produto com ID '{id}' não foi encontrado.");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
