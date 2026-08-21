using System.Net;
using System.Text.Json;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Api.Middlewares;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Express / NestJS Error Handler vs C# Middleware):
/// No Express: app.use((err, req, res, next) => res.status(err.status || 500).json(...))
/// No NestJS: @Catch() ExceptionFilters
/// 
/// No C#, um Middleware é uma função/classe que intercepta o pipeline HTTP.
/// O 'RequestDelegate _next' representa o próximo middleware na cadeia (similar ao 'next()' no Express).
/// Envolvemos a execução em um 'try/catch' global para capturar qualquer exceção não tratada na API.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Passa a requisição para o próximo middleware / Controller
            await _next(context);
        }
        catch (Exception ex)
        {
            // Captura qualquer erro ocorrido nos controllers/services
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode statusCode;
        string message;
        string errorType;

        if (exception is AppException appException)
        {
            // Erro de domínio esperado (ex: 404 NotFound, 400 BadRequest, 409 Conflict)
            statusCode = appException.StatusCode;
            message = appException.Message;
            errorType = appException.GetType().Name;
            _logger.LogWarning("Aviso de Negócio [{StatusCode}]: {Message}", (int)statusCode, message);
        }
        else
        {
            // Erro inesperado do servidor (Bug, falha de banco, etc. -> 500 Internal Server Error)
            statusCode = HttpStatusCode.InternalServerError;
            message = _env.IsDevelopment() 
                ? exception.Message 
                : "Ocorreu um erro interno no servidor. Tente novamente mais tarde.";
            errorType = "InternalServerError";
            _logger.LogError(exception, "Erro não tratado ocorrido na requisição: {Message}", exception.Message);
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            error = errorType,
            message,
            timestamp = DateTime.UtcNow,
            // Em desenvolvimento, adicionamos detalhes adicionais para debug
            details = _env.IsDevelopment() ? exception.StackTrace : null
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
