using FluentAssertions;
using ShopApi.Application.DTOs.Products;
using ShopApi.Application.Services;
using ShopApi.Domain.Entities;
using ShopApi.Domain.Exceptions;
using ShopApi.UnitTests.Helpers;

namespace ShopApi.UnitTests.Services;

public class ProductServiceTests
{
    [Fact(DisplayName = "Deve listar todos os produtos cadastrados")]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        context.Products.AddRange(
            new Product { Name = "Teclado", Price = 200m, Stock = 10 },
            new Product { Name = "Mouse", Price = 100m, Stock = 20 }
        );
        await context.SaveChangesAsync();

        var productService = new ProductService(context);

        // Act
        var result = await productService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Teclado");
        result.Should().Contain(p => p.Name == "Mouse");
    }

    [Fact(DisplayName = "Deve criar um produto com sucesso quando o nome for único")]
    public async Task CreateAsync_WithValidData_ShouldPersistAndReturnProduct()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var productService = new ProductService(context);

        var dto = new CreateProductDto
        {
            Name = "Monitor 4K",
            Description = "Monitor 32 polegadas UHD",
            Price = 2500.50m,
            Stock = 8
        };

        // Act
        var result = await productService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Monitor 4K");
        result.Price.Should().Be(2500.50m);
        result.Stock.Should().Be(8);

        // Confirma no banco
        var productInDb = context.Products.FirstOrDefault(p => p.Id == result.Id);
        productInDb.Should().NotBeNull();
        productInDb!.Name.Should().Be("Monitor 4K");
    }

    [Fact(DisplayName = "Deve lançar ConflictException ao tentar criar produto com nome duplicado")]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        context.Products.Add(new Product { Name = "Headset Gamer", Price = 350m, Stock = 5 });
        await context.SaveChangesAsync();

        var productService = new ProductService(context);

        var dto = new CreateProductDto
        {
            Name = "Headset Gamer", // Mesmo nome
            Description = "Outra descrição",
            Price = 400m,
            Stock = 10
        };

        // Act & Assert
        var act = () => productService.CreateAsync(dto);
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Já existe um produto cadastrado com o nome*");
    }

    [Fact(DisplayName = "Deve lançar NotFoundException ao buscar ID inexistente")]
    public async Task GetByIdAsync_WhenNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var productService = new ProductService(context);
        var randomId = Guid.NewGuid();

        // Act & Assert
        var act = () => productService.GetByIdAsync(randomId);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Produto com ID '{randomId}' não foi encontrado.*");
    }

    [Fact(DisplayName = "Deve remover produto com sucesso")]
    public async Task DeleteAsync_WhenProductExists_ShouldRemoveFromDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var product = new Product { Name = "Webcam", Price = 150m, Stock = 12 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var productService = new ProductService(context);

        // Act
        await productService.DeleteAsync(product.Id);

        // Assert
        var productInDb = context.Products.FirstOrDefault(p => p.Id == product.Id);
        productInDb.Should().BeNull();
    }
}
