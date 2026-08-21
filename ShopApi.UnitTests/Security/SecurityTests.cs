using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using ShopApi.Domain.Entities;
using ShopApi.Domain.Enums;
using ShopApi.Infrastructure.Security;

namespace ShopApi.UnitTests.Security;

public class SecurityTests
{
    [Fact(DisplayName = "BCrypt deve gerar hash válido e verificar senha corretamente")]
    public void BcryptPasswordHasher_ShouldHashAndVerifyPassword()
    {
        // Arrange
        var hasher = new BcryptPasswordHasher();
        var rawPassword = "MinhaSenhaSuperSecreta123!";

        // Act
        var hash = hasher.HashPassword(rawPassword);
        var isValid = hasher.VerifyPassword(rawPassword, hash);
        var isInvalid = hasher.VerifyPassword("SenhaIncorreta", hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(rawPassword); // Garante que foi criptografado
        isValid.Should().BeTrue();
        isInvalid.Should().BeFalse();
    }

    [Fact(DisplayName = "JwtTokenService deve gerar token JWT válido com claims e roles de RBAC")]
    public void JwtTokenService_ShouldGenerateValidJwtWithRoleClaims()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "ChaveSuperSecretaComMaisDe256BitsParaAssinarOsTestesDeToken123456789!@#$" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpirationInMinutes", "60" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenService = new JwtTokenService(configuration);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Admin Master",
            Email = "admin.master@shop.com",
            Role = Role.Admin
        };

        // Act
        var tokenString = tokenService.GenerateToken(user);

        // Assert
        tokenString.Should().NotBeNullOrEmpty();

        // Decodifica o Token para inspecionar os Claims embutidos no payload
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        jwtToken.Issuer.Should().Be("TestIssuer");
        jwtToken.Audiences.Should().Contain("TestAudience");

        // Verifica os Claims de RBAC
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        roleClaim.Should().NotBeNull();
        roleClaim!.Value.Should().Be("Admin");

        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == ClaimTypes.Email);
        emailClaim.Should().NotBeNull();
        emailClaim!.Value.Should().Be("admin.master@shop.com");
    }
}
