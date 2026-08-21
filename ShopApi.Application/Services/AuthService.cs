using Microsoft.EntityFrameworkCore;
using ShopApi.Application.DTOs.Auth;
using ShopApi.Application.DTOs.Users;
using ShopApi.Application.Interfaces.Common;
using ShopApi.Application.Interfaces.Security;
using ShopApi.Application.Interfaces.Services;
using ShopApi.Domain.Entities;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Application.Services;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript/NestJS vs C#):
/// No NestJS você usaria '@Injectable()' na classe e declararia as dependências no 'constructor':
/// @Injectable()
/// export class AuthService {
///   constructor(
///     private readonly db: PrismaService,
///     private readonly jwt: JwtService
///   ) {}
/// }
/// 
/// No C#, a injeção de dependência é nativa do framework (sem precisar de decorators).
/// Todas as dependências são recebidas no construtor como interfaces e armazenadas em campos privados readonly.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Normaliza o e-mail para minúsculas para evitar duplicidades por capitalização
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        // 2. Verifica se já existe usuário cadastrado com este e-mail
        // 'AnyAsync' é o equivalente ao 'findFirst' / 'count > 0' do Prisma (executa 'SELECT EXISTS' no SQL)
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException($"O e-mail '{dto.Email}' já está em uso por outro usuário.");
        }

        // 3. Criptografa a senha usando BCrypt
        var passwordHash = _passwordHasher.HashPassword(dto.Password);

        // 4. Cria a entidade User
        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            Role = dto.Role
        };

        // 5. Adiciona e persiste no banco de dados (equivalente ao prisma.user.create())
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Gera o token JWT para login automático após o cadastro
        var token = _tokenService.GenerateToken(user);
        var expiresAt = _tokenService.GetExpirationDate();

        return new AuthResponseDto
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            User = UserResponseDto.FromEntity(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        // Busca o usuário pelo e-mail
        // 'FirstOrDefaultAsync' equivale ao 'prisma.user.findUnique({ where: { email } })'
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // Por segurança contra enumeração de usuários, retorne a mesma mensagem genérica para e-mail não encontrado ou senha errada
        if (user == null)
        {
            throw new UnauthorizedException("E-mail ou senha incorretos.");
        }

        // Valida o hash da senha
        var isPasswordValid = _passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("E-mail ou senha incorretos.");
        }

        // Gera o token JWT com as claims de identificação e role (RBAC)
        var token = _tokenService.GenerateToken(user);
        var expiresAt = _tokenService.GetExpirationDate();

        return new AuthResponseDto
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            User = UserResponseDto.FromEntity(user)
        };
    }
}
