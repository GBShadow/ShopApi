using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces.Common;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Data;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Prisma vs Entity Framework Core):
/// O 'DbContext' no EF Core é o coração do ORM (equivalente à instância do 'PrismaClient').
/// Ele gerencia a conexão com o banco de dados, mapeamento de tabelas, transações e o Unit of Work.
/// 
/// O método 'OnModelCreating' permite configurar regras avançadas do banco (Fluent API),
/// como índices únicos, tipos de dados SQL, precisão decimal e conversão de Enums.
/// </summary>
public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações da Entidade User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            // Índice Único no E-mail (garante no nível do banco que não existam dois e-mails iguais)
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            // Salva o enum Role como texto ("Admin", "User") no banco em vez de número (1, 2)
            // Isso torna a leitura direta do banco muito mais clara e legível!
            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Configurações da Entidade Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Description)
                .HasMaxLength(500);

            // Precisão decimal para moeda (18 dígitos no total, 2 casas decimais)
            entity.Property(p => p.Price)
                .HasPrecision(18, 2);

            entity.Property(p => p.Stock)
                .IsRequired();
        });
    }
}
