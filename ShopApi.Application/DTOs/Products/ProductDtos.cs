using System.ComponentModel.DataAnnotations;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.DTOs.Products;

/// <summary>
/// DTO para criação de novos produtos (POST /api/products)
/// </summary>
public class CreateProductDto
{
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1000000.00, ErrorMessage = "O preço deve ser maior que zero (0.01 a 1.000.000,00).")]
    public decimal Price { get; set; }

    [Range(0, 100000, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Stock { get; set; }
}

/// <summary>
/// DTO para atualização de produtos existentes (PUT /api/products/{id})
/// </summary>
public class UpdateProductDto
{
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1000000.00, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Price { get; set; }

    [Range(0, 100000, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Stock { get; set; }
}

/// <summary>
/// DTO de saída com as informações formatadas do produto
/// </summary>
public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static ProductResponseDto FromEntity(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
