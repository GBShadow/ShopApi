using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShopApi.Api.Middlewares;
using ShopApi.Application.Interfaces.Security;
using ShopApi.Domain.Entities;
using ShopApi.Domain.Enums;
using ShopApi.Infrastructure;
using ShopApi.Infrastructure.Data;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Node.js index.ts / NestJS main.ts vs C# Program.cs):
/// No Node.js (Express):
/// const app = express(); app.use(express.json()); app.listen(3000);
/// No NestJS:
/// async function bootstrap() { const app = await NestFactory.create(AppModule); await app.listen(3000); }
/// 
/// No C# .NET 8 (Top-Level Statements & Minimal Hosting Model):
/// 'WebApplication.CreateBuilder(args)' cria o container de injeção de dependências (builder.Services).
/// 'builder.Build()' compila o container e gera o pipeline HTTP ('app').
/// 'app.Use...()' registra os middlewares em ordem de execução.
/// 'app.Run()' inicia o servidor web ultrarrápido (Kestrel).
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 1. REGISTRO DE SERVIÇOS NO CONTAINER DE INJEÇÃO DE DEPENDÊNCIA (IoC Container)
// ==============================================================================

// Suporte a Controllers REST
builder.Services.AddControllers();

// Configuração do Swagger / OpenAPI com suporte ao botão de Autenticação JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShopApi - C# .NET 8 Clean Architecture",
        Version = "v1",
        Description = "API Completa para estudo de C#, Autenticação JWT, RBAC, EF Core, Uploads e Streams."
    });

    // Adiciona o esquema de segurança JWT no Swagger (Botão 'Authorize' com cadeado)
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT desta forma: Bearer {seu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// Registra a camada de Infraestrutura e Banco de Dados (EF Core, Repositórios, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Registra a Autenticação JWT e as Políticas de Autorização RBAC
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configuração de CORS (Permite chamadas de SPAs em React, Vue, Svelte, Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ==============================================================================
// 2. CONSTRUÇÃO DO APP E CONFIGURAÇÃO DO PIPELINE HTTP (MIDDLEWARES)
// ==============================================================================

var app = builder.Build();

// Middleware Global de Tratamento de Exceções (SEMPRE O PRIMEIRO para capturar qualquer erro abaixo)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Habilita o Swagger no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopApi v1");
        c.RoutePrefix = string.Empty; // Abre o Swagger diretamente na raiz (http://localhost:5000/)
    });
}

app.UseCors("AllowAll");

// A ORDEM DESTES DOIS MIDDLEWARES É CRÍTICA NO ASP.NET CORE:
// 1º Authentication: "Quem é você?" (Decodifica e valida o Token JWT e cria o ClaimsPrincipal)
app.UseAuthentication();
// 2º Authorization: "O que você tem permissão para fazer?" (Valida [Authorize(Roles = "Admin")])
app.UseAuthorization();

// Mapeia os endpoints dos Controllers
app.MapControllers();

// ==============================================================================
// 3. SEED AUTOMÁTICO DO BANCO DE DADOS (Cria tabelas e insere dados iniciais)
// ==============================================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var hasher = services.GetRequiredService<IPasswordHasher>();

        // Garante que o banco SQLite e as tabelas sejam criados se não existirem
        context.Database.EnsureCreated();

        // Seed de Usuários Iniciais se a tabela estiver vazia
        if (!context.Users.Any())
        {
            var adminUser = new User
            {
                Name = "Administrador do Sistema",
                Email = "admin@shop.com",
                PasswordHash = hasher.HashPassword("Admin123!"),
                Role = Role.Admin
            };

            var managerUser = new User
            {
                Name = "Gerente da Loja",
                Email = "manager@shop.com",
                PasswordHash = hasher.HashPassword("Manager123!"),
                Role = Role.Manager
            };

            var standardUser = new User
            {
                Name = "Cliente Padrão",
                Email = "user@shop.com",
                PasswordHash = hasher.HashPassword("User123!"),
                Role = Role.User
            };

            context.Users.AddRange(adminUser, managerUser, standardUser);
            context.SaveChanges();
        }

        // Seed de Produtos Iniciais
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product
                {
                    Name = "Notebook Dell XPS 13",
                    Description = "Intel Core i7 13ª Geração, 16GB RAM, 512GB SSD NVMe",
                    Price = 8499.90m,
                    Stock = 15
                },
                new Product
                {
                    Name = "Monitor Gamer UltraWide 34\"",
                    Description = "Painel IPS 144Hz 1ms HDR400 FreeSync Premium",
                    Price = 2799.00m,
                    Stock = 28
                },
                new Product
                {
                    Name = "Teclado Mecânico Sem Fio",
                    Description = "Switches Red silenciosos, Iluminação RGB, Bluetooth 5.2",
                    Price = 450.00m,
                    Stock = 50
                }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao executar o seed inicial do banco de dados.");
    }
}

app.Run();
