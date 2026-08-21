using Microsoft.EntityFrameworkCore;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces.Common;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript vs C#):
/// No NestJS/Node, você injetaria o PrismaService diretamente ou um repositório.
/// 
/// Na Clean Architecture em C#, a camada Application define esta interface abstrata.
/// A camada Infrastructure implementa a interface usando a classe real 'AppDbContext' (Entity Framework).
/// Isso segue o DIP (Dependency Inversion Principle do SOLID):
/// "Módulos de alto nível (Application) não devem depender de módulos de baixo nível (Infrastructure/EF Core).
/// Ambos devem depender de abstrações (Interfaces)."
/// 
/// 'CancellationToken': Em C#, permite cancelar consultas de banco se o cliente HTTP abortar a requisição!
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
