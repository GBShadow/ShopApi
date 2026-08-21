using FluentAssertions;
using Moq;
using ShopApi.Application.DTOs.Auth;
using ShopApi.Application.Interfaces.Security;
using ShopApi.Application.Services;
using ShopApi.Domain.Entities;
using ShopApi.Domain.Enums;
using ShopApi.Domain.Exceptions;
using ShopApi.UnitTests.Helpers;

namespace ShopApi.UnitTests.Services;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Jest / Vitest vs xUnit + FluentAssertions + Moq):
/// No Jest/Vitest:
/// describe('AuthService', () => {
///   it('should register a new user', async () => {
///     const mockJwt = { sign: vi.fn().mockReturnValue('token123') };
///     expect(result.token).toBe('token123');
///   });
/// });
/// 
/// No C#:
/// - 'xUnit': Framework de testes ('[Fact]' para testes únicos, '[Theory]' para testes parametrizados).
/// - 'Moq': Biblioteca para criar Mocks de interfaces ('Mock<ITokenService>').
/// - 'FluentAssertions': Permite asserções legíveis no estilo BDD ('result.Token.Should().Be("token_jwt_mock")').
/// - Padrão AAA: Arrange (Preparação), Act (Ação), Assert (Verificação).
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;

    public AuthServiceTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();

        // Configuração padrão dos Mocks
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password_123");

        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("token_jwt_valido_xyz");

        _tokenServiceMock.Setup(t => t.GetExpirationDate())
            .Returns(DateTime.UtcNow.AddHours(8));
    }

    [Fact(DisplayName = "Deve cadastrar usuário com sucesso quando os dados forem válidos")]
    public async Task RegisterAsync_WithValidData_ShouldCreateUserAndReturnAuthResponse()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var authService = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);

        var request = new RegisterRequestDto
        {
            Name = "Fulano de Tal",
            Email = "fulano@teste.com",
            Password = "SenhaForte123!",
            Role = Role.User
        };

        // Act
        var response = await authService.RegisterAsync(request);

        // Assert (FluentAssertions)
        response.Should().NotBeNull();
        response.Token.Should().Be("token_jwt_valido_xyz");
        response.TokenType.Should().Be("Bearer");
        response.User.Email.Should().Be("fulano@teste.com");
        response.User.Name.Should().Be("Fulano de Tal");
        response.User.Role.Should().Be("User");

        // Verifica se o usuário foi realmente persistido no banco
        var savedUser = context.Users.FirstOrDefault(u => u.Email == "fulano@teste.com");
        savedUser.Should().NotBeNull();
        savedUser!.PasswordHash.Should().Be("hashed_password_123");
    }

    [Fact(DisplayName = "Deve lançar ConflictException ao tentar cadastrar e-mail já existente")]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        context.Users.Add(new User
        {
            Name = "Usuario Existente",
            Email = "existente@teste.com",
            PasswordHash = "hash"
        });
        await context.SaveChangesAsync();

        var authService = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);

        var request = new RegisterRequestDto
        {
            Name = "Outro Usuario",
            Email = "existente@teste.com", // Mesmo e-mail
            Password = "OutraSenha123!"
        };

        // Act & Assert
        var act = () => authService.RegisterAsync(request);
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*já está em uso*");
    }

    [Fact(DisplayName = "Deve realizar login com sucesso com credenciais válidas")]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Name = "Maria Silva",
            Email = "maria@teste.com",
            PasswordHash = "hash_da_maria",
            Role = Role.Manager
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _passwordHasherMock.Setup(h => h.VerifyPassword("SenhaCorreta123!", "hash_da_maria"))
            .Returns(true);

        var authService = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);

        var request = new LoginRequestDto
        {
            Email = "maria@teste.com",
            Password = "SenhaCorreta123!"
        };

        // Act
        var response = await authService.LoginAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Token.Should().Be("token_jwt_valido_xyz");
        response.User.Email.Should().Be("maria@teste.com");
        response.User.Role.Should().Be("Manager");
    }

    [Fact(DisplayName = "Deve lançar UnauthorizedException quando a senha for incorreta")]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        context.Users.Add(new User
        {
            Name = "Carlos",
            Email = "carlos@teste.com",
            PasswordHash = "hash_do_carlos"
        });
        await context.SaveChangesAsync();

        _passwordHasherMock.Setup(h => h.VerifyPassword("SenhaErrada", "hash_do_carlos"))
            .Returns(false);

        var authService = new AuthService(context, _passwordHasherMock.Object, _tokenServiceMock.Object);

        var request = new LoginRequestDto
        {
            Email = "carlos@teste.com",
            Password = "SenhaErrada"
        };

        // Act & Assert
        var act = () => authService.LoginAsync(request);
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("E-mail ou senha incorretos.");
    }
}
