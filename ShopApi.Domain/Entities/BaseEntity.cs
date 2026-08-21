namespace ShopApi.Domain.Entities;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// Esta é uma classe abstrata base (semelhante a 'abstract class BaseEntity' no TypeScript/TypeORM).
/// Em C#, 'abstract' impede que ela seja instanciada diretamente com 'new BaseEntity()',
/// servindo apenas como modelo compartilhado para outras entidades herdarem propriedades comuns.
/// 
/// 'Guid' equivale ao 'string' UUID no Node (ex: crypto.randomUUID() ou uuidv4()).
/// 'DateTime' representa datas e horários com precisão de timezone (UTC recomendado).
/// </summary>
public abstract class BaseEntity
{
    // Identificador único universal (UUID v4)
    public Guid Id { get; set; } = Guid.NewGuid();

    // Data de criação em UTC (sempre prefira UTC para consistência global)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Data de atualização opcional ('DateTime?' no C# é nullable, equivalente a 'Date | undefined' no TS)
    public DateTime? UpdatedAt { get; set; }
}
