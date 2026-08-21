using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Application.Interfaces.Common;
using ShopApi.Application.Interfaces.Security;
using ShopApi.Application.Interfaces.Services;
using ShopApi.Application.Services;
using ShopApi.Infrastructure.Data;
using ShopApi.Infrastructure.Security;

namespace ShopApi.Infrastructure;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Injeção de Dependência no C# vs Node.js):
/// No Node.js (Express), você faz 'import { authService } from "./auth.service"' (módulos são Singletons por padrão).
/// No NestJS, você declara no '@Module({ providers: [AuthService] })'.
/// 
/// No C#, o ASP.NET Core possui um DI Container nativo ultra-rápido com 3 ciclos de vida principais:
/// 1. 'AddTransient': Uma NOVA instância é criada toda vez que o serviço é solicitado.
/// 2. 'AddScoped': Uma ÚNICA instância é criada POR REQUISIÇÃO HTTP (Ideal para DbContext e Services de negócio).
/// 3. 'AddSingleton': Uma ÚNICA instância para toda a aplicação durante todo o tempo em que o servidor estiver rodando.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuração do Banco de Dados com Entity Framework Core e SQLite
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=shop.db";
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // Registra IApplicationDbContext apontando para a implementação AppDbContext
        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<AppDbContext>());

        // 2. Registra os serviços de segurança (Criptografia e JWT)
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        // 3. Registra os serviços de aplicação (Regras de Negócio)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    /// <summary>
    /// Configura a Autenticação JWT Bearer e as Políticas de Autorização (RBAC)
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:Secret"] 
            ?? "ChaveSuperSecretaPadraoParaDesenvolvimento123456789!@#$";
        var issuer = configuration["Jwt:Issuer"] ?? "ShopApi";
        var audience = configuration["Jwt:Audience"] ?? "ShopApiUsers";

        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Em produção altere para true
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Sem tolerância de tempo extra na expiração do token
            };
        });

        // Configuração de Políticas de Autorização baseadas em Roles (RBAC)
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));
            options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
        });

        return services;
    }
}
