using Microsoft.EntityFrameworkCore;
using ShopApi.Infrastructure.Data;

namespace ShopApi.UnitTests.Helpers;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Testes Unitários com Banco de Dados em C#):
/// No Node.js / Prisma, você usava 'prisma-mock' ou um container Docker no Jest.
/// 
/// No C# com Entity Framework Core, podemos usar o provedor 'UseInMemoryDatabase()'!
/// Cada teste recebe um nome de banco único (Guid.NewGuid().ToString()), garantindo
/// que nenhum teste interfira no estado do outro (isolamento total e execução paralela veloz).
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
