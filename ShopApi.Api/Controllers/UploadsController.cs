using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Api.Controllers;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Upload de Arquivos: Multer/Node vs C#):
/// No Node/Express, você usaria o middleware 'multer' (upload.single('file')).
/// 
/// No C# ASP.NET Core:
/// O tipo 'IFormFile' é o tipo nativo padrão para receber arquivos via 'multipart/form-data'.
/// Ele fornece:
/// - 'file.FileName': Nome original do arquivo enviado pelo cliente (CUIDADO: sempre sanitize!).
/// - 'file.Length': Tamanho em bytes.
/// - 'file.ContentType': MIME Type (ex: 'image/png', 'application/pdf').
/// - 'file.OpenReadStream()': Stream para leitura eficiente sem carregar o arquivo todo na RAM.
/// - 'file.CopyToAsync(targetStream)': Copia em chunks diretamente para o disco ou S3.
/// </summary>
public class UploadsController : BaseApiController
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadsController> _logger;

    // Extensões permitidas (Whitelist de segurança)
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
    // Tamanho máximo permitido: 5 MB
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UploadsController(IWebHostEnvironment environment, ILogger<UploadsController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Realiza o upload de um arquivo com validações de segurança
    /// </summary>
    /// <param name="file">Arquivo enviado no formato multipart/form-data</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadFile(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        // 1. Validação de presença do arquivo
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Nenhum arquivo foi enviado ou o arquivo está vazio." });
        }

        // 2. Validação de tamanho máximo (5 MB)
        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest(new { message = $"O arquivo excede o limite máximo permitido de {MaxFileSizeBytes / (1024 * 1024)} MB." });
        }

        // 3. Validação de extensão (Whitelist de extensões seguras)
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(fileExtension))
        {
            return BadRequest(new { message = $"Extensão de arquivo '{fileExtension}' não é permitida. Extensões aceitas: {string.Join(", ", AllowedExtensions)}" });
        }

        // 4. Gera um nome único e seguro usando GUID (EVITA Path Traversal e sobrescrita de arquivos de outros usuários)
        var safeFileName = $"{Guid.NewGuid()}{fileExtension}";

        // 5. Define a pasta de destino (wwwroot/uploads ou pasta isolada)
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fullPath = Path.Combine(uploadsFolder, safeFileName);

        // 6. Salva o arquivo no disco usando FileStream de forma assíncrona
        // 'using' garante que o Stream seja fechado e liberado da memória ao terminar
        await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        _logger.LogInformation("Upload concluído com sucesso: {SafeFileName} ({Bytes} bytes) por {User}", 
            safeFileName, file.Length, GetCurrentUserEmail());

        return Ok(new
        {
            message = "Upload realizado com sucesso!",
            originalName = file.FileName,
            storedFileName = safeFileName,
            sizeInBytes = file.Length,
            contentType = file.ContentType,
            downloadUrl = $"/api/uploads/{safeFileName}"
        });
    }

    /// <summary>
    /// Download/visualização de arquivo salvo
    /// </summary>
    [HttpGet("{fileName}")]
    [AllowAnonymous]
    public IActionResult GetFile(string fileName)
    {
        // Sanitiza para evitar ataques de Path Traversal (ex: ../../etc/passwd)
        var cleanFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_environment.ContentRootPath, "uploads", cleanFileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { message = "Arquivo não encontrado." });
        }

        var extension = Path.GetExtension(cleanFileName).ToLowerInvariant();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };

        // Retorna o arquivo como Stream direto para o cliente
        return PhysicalFile(filePath, contentType, enableRangeProcessing: true);
    }
}
