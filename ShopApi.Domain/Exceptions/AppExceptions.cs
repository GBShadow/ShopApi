using System.Net;

namespace ShopApi.Domain.Exceptions;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (TypeScript/NestJS vs C#):
/// No NestJS você lançaria exceções HTTP como:
/// throw new HttpException('Mensagem', HttpStatus.BAD_REQUEST);
/// throw new NotFoundException('Produto não encontrado');
/// 
/// No C#, criamos classes de exceção personalizadas que herdam de 'Exception'.
/// Um middleware global na API captura essas exceções e as traduz em respostas JSON
/// padronizadas (RFC 7807 ProblemDetails / JSON customizado) com o StatusCode apropriado.
/// </summary>
public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }
}

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, HttpStatusCode.Conflict)
    {
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Credenciais inválidas ou acesso não autenticado.")
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Você não tem permissão para executar esta ação.")
        : base(message, HttpStatusCode.Forbidden)
    {
    }
}
