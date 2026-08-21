using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.DTOs.Products;
using ShopApi.Application.Interfaces.Services;

namespace ShopApi.Api.Controllers;

/// <summary>
/// 🎓 NOTA PEDAGÓGICA (Streams de Dados: Node.js Streams vs C# Streams & IAsyncEnumerable):
/// No Node.js: 'ReadableStream', 'TransformStream', 'res.write(chunk)', e Async Generators ('async function* () { yield chunk; }').
/// 
/// No C#:
/// 1. 'IAsyncEnumerable<T>': Permite que a API envie itens JSON um a um conforme eles são gerados ou lidos do banco,
///    sem precisar carregar uma lista de 100.000 itens inteira na memória RAM primeiro!
/// 2. 'Stream' / 'FileStream' / 'MemoryStream': Abstrações de baixo nível para leitura/escrita contínua em buffers de bytes.
/// 3. 'FileStreamResult': Transmite arquivos gigantes do disco para a resposta HTTP em chunks pequenos (ex: 4KB/64KB).
/// </summary>
public class StreamsController : BaseApiController
{
    private readonly IProductService _productService;

    public StreamsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Demonstração de Streaming de Dados em tempo real usando IAsyncEnumerable (Similar a Async Generator no JS)
    /// </summary>
    [HttpGet("products-stream")]
    [AllowAnonymous]
    [Produces("application/x-ndjson", "application/json")]
    public async IAsyncEnumerable<ProductResponseDto> StreamProducts(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);

        foreach (var product in products)
        {
            // Simula um delay ou processamento sob demanda (ex: lendo de fila, hardware ou IA)
            await Task.Delay(200, cancellationToken);

            // 'yield return' emite cada item individualmente através do stream HTTP
            yield return product;
        }
    }

    /// <summary>
    /// Demonstração de Streaming de Arquivo/Relatório gerado dinamicamente em memória sem salvar no disco
    /// </summary>
    [HttpGet("generate-report-stream")]
    [Authorize]
    public async Task<IActionResult> GenerateCsvReportStream(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);

        // Cria um MemoryStream para gerar o CSV sob demanda
        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
        {
            // Cabeçalho do CSV
            await writer.WriteLineAsync("ID;Nome;Preco;Estoque;DataCriacao");

            foreach (var product in products)
            {
                var line = $"{product.Id};{product.Name};{product.Price:F2};{product.Stock};{product.CreatedAt:yyyy-MM-dd HH:mm:ss}";
                await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
            }

            await writer.FlushAsync(cancellationToken);
        }

        // Reposiciona o cursor do stream no início para leitura
        memoryStream.Position = 0;

        // Retorna o Stream com o cabeçalho 'Content-Disposition: attachment' para forçar o download no navegador
        return File(memoryStream, "text/csv", $"relatorio-produtos-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
    }
}
