namespace ShopApi.Domain.Entities;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// No JavaScript/TypeScript, números decimais e inteiros são todos 'number' (IEEE 754 float).
/// Isso pode causar erros de arredondamento bizarros como 0.1 + 0.2 === 0.30000000000000004.
/// 
/// No C#, para valores monetários/financeiros, NUNCA usamos 'double' ou 'float';
/// usamos 'decimal' (128 bits de alta precisão sem erros de ponto flutuante).
/// Para quantidades inteiras usamos 'int' (32 bits) ou 'long' (64 bits).
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // 'decimal' é o tipo padrão obrigatório para preços/dinheiro no C#
    public decimal Price { get; set; }

    // Quantidade em estoque (inteiro não fracionado)
    public int Stock { get; set; }
}
