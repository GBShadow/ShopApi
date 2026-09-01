using System.ComponentModel.DataAnnotations;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.DTOs.Categories;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Nome da categoria é obrigatório")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "o nome da categoria deve conter entre 2 e 100 caracteres."
    )]
    public string Name { get; set; } = String.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "o descrição da categoria deve conter entre 2 e 100 caracteres."
    )]
    public string Description { get; set; } = String.Empty;
}

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Nome da categoria é obrigatório")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "o nome da categoria deve conter entre 2 e 100 caracteres."
    )]
    public string Name { get; set; } = String.Empty;

    [StringLength(
        300,
        MinimumLength = 2,
        ErrorMessage = "o descrição da categoria deve conter entre 2 e 300 caracteres."
    )]
    public string Description { get; set; } = String.Empty;

    public bool IsActive { get; set; } = true;
}

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static CategoryResponseDto FromEntity(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
        };
    }
}
