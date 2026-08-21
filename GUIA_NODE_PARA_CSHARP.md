# 🚀 Guia Definitivo: Do Node.js & TypeScript para C# e .NET 8

Este guia foi elaborado especialmente para desenvolvedores com bagagem em **JavaScript, TypeScript, Express, NestJS e Prisma** que estão migrando para o ecossistema moderno do **C# e .NET 8**.

---

## 1. 🧠 Mudança de Modelo Mental (TypeScript vs C#)

| Conceito | TypeScript / Node.js | C# / .NET 8 |
| :--- | :--- | :--- |
| **Execução** | V8 Engine (Single Thread com Event Loop) | CLR (Common Language Runtime) com Multi-threading real e ThreadPool |
| **Tipagem** | Tipagem estática estrutural (apagada no runtime) | Tipagem estática nominal forte (reificada e verificada no runtime) |
| **Ponto de Entrada** | `src/index.ts` ou `src/main.ts` | `Program.cs` (Top-Level Statements) |
| **Variáveis de Ambiente** | `.env` (`process.env.VAR`) | `appsettings.json` + `appsettings.Development.json` + Env Vars |
| **Assincronismo** | `Promise<T>` e `async/await` | `Task<T>` e `async/await` |
| **Coleções e Listas** | `Array.map()`, `Array.filter()`, `Array.reduce()` | **LINQ**: `.Select()`, `.Where()`, `.Aggregate()` |
| **Gerenciador de Pacotes** | `npm`, `pnpm`, `yarn` (`package.json`) | `dotnet` CLI / NuGet (`.csproj`) |
| **Módulos / Dependências** | `node_modules` (duplicado em cada projeto) | Cache global do NuGet em `~/.nuget/packages` (economiza gigabytes de disco) |
| **Hot Reload / Watch** | `nodemon`, `tsx --watch` | `dotnet watch run` (Hot Reload nativo ultrarrápido) |

---

## 2. 🛠️ Como Criar um Projeto do Zero (Linha de Comando)

No .NET corporativo moderno, dividimos a solução em múltiplos projetos (**Clean Architecture**) usando arquivos `.sln` (Solution) e `.csproj` (C# Project):

```bash
# 1. Cria a pasta do projeto e a Solução (.sln)
mkdir MinhaApi && cd MinhaApi
dotnet new sln -n MinhaApi

# 2. Cria as Camadas (Projetos Class Library e Web API)
dotnet new classlib -n MinhaApi.Domain -f net8.0
dotnet new classlib -n MinhaApi.Application -f net8.0
dotnet new classlib -n MinhaApi.Infrastructure -f net8.0
dotnet new webapi -n MinhaApi.Api -f net8.0 --use-controllers
dotnet new xunit -n MinhaApi.UnitTests -f net8.0

# 3. Adiciona todos os projetos à Solução
dotnet sln add MinhaApi.Domain/MinhaApi.Domain.csproj
dotnet sln add MinhaApi.Application/MinhaApi.Application.csproj
dotnet sln add MinhaApi.Infrastructure/MinhaApi.Infrastructure.csproj
dotnet sln add MinhaApi.Api/MinhaApi.Api.csproj
dotnet sln add MinhaApi.UnitTests/MinhaApi.UnitTests.csproj

# 4. Configura as referências de dependência entre projetos (Dependency Inversion)
dotnet add MinhaApi.Application/MinhaApi.Application.csproj reference MinhaApi.Domain/MinhaApi.Domain.csproj
dotnet add MinhaApi.Infrastructure/MinhaApi.Infrastructure.csproj reference MinhaApi.Application/MinhaApi.Application.csproj
dotnet add MinhaApi.Infrastructure/MinhaApi.Infrastructure.csproj reference MinhaApi.Domain/MinhaApi.Domain.csproj
dotnet add MinhaApi.Api/MinhaApi.Api.csproj reference MinhaApi.Infrastructure/MinhaApi.Infrastructure.csproj
dotnet add MinhaApi.Api/MinhaApi.Api.csproj reference MinhaApi.Application/MinhaApi.Application.csproj
dotnet add MinhaApi.UnitTests/MinhaApi.UnitTests.csproj reference MinhaApi.Application/MinhaApi.Application.csproj

# 5. Executar a aplicação com Hot Reload
dotnet watch --project MinhaApi.Api/MinhaApi.Api.csproj run
```

---

## 3. 📦 Gerenciamento de Pacotes: NPM vs NuGet

| Comando no NPM | Comando no .NET (NuGet) | O que faz |
| :--- | :--- | :--- |
| `npm install pacote` | `dotnet add <PROJETO>.csproj package Pacote` | Instala a versão mais recente do pacote |
| `npm install pacote@8.0.0` | `dotnet add <PROJETO>.csproj package Pacote --version 8.0.0` | Instala uma versão específica |
| `npm uninstall pacote` | `dotnet remove <PROJETO>.csproj package Pacote` | Remove a biblioteca do projeto |
| `npm list` / `npm outdated` | `dotnet list package` / `dotnet list package --outdated` | Lista os pacotes instalados e desatualizados |
| `npm install` (CI/CD) | `dotnet restore` | Baixa os pacotes listados nos `.csproj` |
| `npm test` | `dotnet test` | Executa todos os testes da solução |
| `npm run build` | `dotnet build` / `dotnet publish -c Release` | Compila os binários em DLLs otimizadas |

---

## 4. 🗺️ Tabela De-Para das Principais Bibliotecas do Ecossistema

| Finalidade | No Node.js / TypeScript | No C# / .NET 8 |
| :--- | :--- | :--- |
| **ORM / Banco de Dados** | Prisma, TypeORM, Drizzle, Kysely | **Entity Framework Core (EF Core)**, **Dapper** (Micro-ORM de alta performance) |
| **Validação de DTOs** | Zod, Joi, `class-validator`, Yup | **FluentValidation**, `System.ComponentModel.DataAnnotations` |
| **Autenticação & JWT** | `jsonwebtoken`, `passport`, `lucia-auth` | `Microsoft.AspNetCore.Authentication.JwtBearer`, `Duende IdentityServer` |
| **Criptografia de Senhas** | `bcryptjs`, `argon2` | `BCrypt.Net-Next`, `Konscious.Security.Cryptography.Argon2` |
| **Testes Unitários & Mocks** | Jest, Vitest, Vitest-mock | **xUnit**, **NUnit**, **FluentAssertions**, **Moq**, **NSubstitute** |
| **Testes de Integração** | Supertest, Testcontainers-node | `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`), **Testcontainers.NET** |
| **Manipulação de Datas** | `date-fns`, `dayjs`, `luxon` | `DateTimeOffset`, `DateOnly`, `TimeOnly`, **NodaTime** |
| **Logs Estruturados** | Pino, Winston | **Serilog**, `Microsoft.Extensions.Logging` |
| **Mapeamento de Objetos** | Lodash map, class-transformer | **Mapster**, **AutoMapper** |
| **Documentação OpenAPI** | Swagger UI Express, tsoa | **Swashbuckle.AspNetCore**, **NSwag**, **Scalar.AspNetCore** |
| **Filas & Mensageria** | BullMQ, amqplib, kafkajs | **MassTransit** (RabbitMQ/Kafka/SQS/ServiceBus), `System.Threading.Channels` |
| **Tarefas em Background** | Agenda, node-cron | **Hangfire**, **Quartz.NET**, `BackgroundService` |

---

## 5. 📅 Como Lidar com Datas no C#

No JavaScript, o objeto `Date` é notoriamente problemático. No C# .NET 8, você possui tipos especializados:

1. **`DateTime`**:
   - Pode ter `Kind = Utc`, `Local` ou `Unspecified`.
   - **Regra de Ouro:** Sempre use `DateTime.UtcNow` para salvar no banco de dados.
2. **`DateTimeOffset` (Mais Recomendado para APIs REST):**
   - Armazena a data/hora junto com o deslocamento de fuso horário exato (ex: `2026-08-19T20:30:00-03:00`).
   - Evita qualquer ambiguidade entre servidores em fusos diferentes.
3. **`DateOnly` & `TimeOnly` (.NET 6+):**
   - `DateOnly`: Para datas de nascimento, feriados (`2026-12-25`) sem componente de hora.
   - `TimeOnly`: Para horários de abertura de lojas, alarmes (`08:00:00`) sem data associada.
4. **`NodaTime`:**
   - Para regras complexas de fuso horário internacional (equivalente ao `Luxon` / `Temporal`).

```csharp
// Exemplo de manipulação limpa de datas:
DateTime dataUtc = DateTime.UtcNow;
DateTimeOffset dataComOffset = DateTimeOffset.UtcNow;
DateOnly dataNascimento = new DateOnly(1995, 5, 20);

// Formatação ISO 8601:
string isoFormat = dataUtc.ToString("o"); // 2026-08-19T23:45:00.0000000Z
```

---

## 6. 🌊 Streams de Dados & Streaming em Tempo Real

No Node.js você usava `ReadableStream`, `res.write()` e Async Generators (`async function*`). No C#:

### A. Streaming de Dados com `IAsyncEnumerable<T>` (JSON Streaming / NDJSON / SSE)
Permite enviar dados para o cliente HTTP sob demanda sem alocar coleções inteiras na memória:

```csharp
[HttpGet("stream")]
public async IAsyncEnumerable<ProductDto> StreamProducts([EnumeratorCancellation] CancellationToken ct)
{
    // Lê os dados conforme chegam do banco ou fila
    await foreach (var product in _context.Products.AsAsyncEnumerable().WithCancellation(ct))
    {
        yield return ProductDto.FromEntity(product); // Envia imediatamente para o cliente
    }
}
```

### B. Streaming de Arquivos com `FileStreamResult`
Transmite arquivos gigantescos (gigabytes) em pequenos buffers de 4KB/64KB sem esgotar a memória RAM do servidor:

```csharp
[HttpGet("download/{fileName}")]
public IActionResult DownloadFile(string fileName)
{
    var path = Path.Combine("/var/storage", Path.GetFileName(fileName));
    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
    return File(fileStream, "application/pdf", enableRangeProcessing: true);
}
```

---

## 7. 📁 Upload de Arquivos Seguro

No Node.js você usava `multer`. No ASP.NET Core usamos `IFormFile`:

### Checklist de Segurança Obrigatório:
1. **Validação de Tamanho:** Limite bytes (`file.Length <= 5 * 1024 * 1024`).
2. **Whitelist de Extensões:** Permita apenas extensões seguras (`.png`, `.jpg`, `.pdf`).
3. **Prevenção de Path Traversal:** NUNCA use o `file.FileName` original para salvar no disco. Gere um GUID único (`$"{Guid.NewGuid()}{extension}"`).
4. **Verificação de Magic Bytes:** Para uploads críticos, valide os primeiros bytes do arquivo para garantir que um executável `.exe` não foi renomeado para `.jpg`.

---

## 8. 📨 Filas, Background Jobs e Mensageria

### 1. Fila Rápida em Memória (Sem dependência externa)
Use `System.Threading.Channels` (`Channel<T>`) + `BackgroundService`:
- O Controller escreve mensagens no canal (`await _channel.Writer.WriteAsync(job)`).
- Um `BackgroundService` rodando em background consome as mensagens continuamente sem bloquear requisições HTTP.

### 2. Jobs Agendados e Persistentes (Equivalente ao BullMQ)
- **Hangfire**: Interface web embutida incrível com dashboard, retries automáticos e persistência no PostgreSQL/SQL Server/Redis.

### 3. Mensageria Distribuída (Microserviços & Event-Driven)
- **MassTransit**: O framework de mensageria mais poderoso do .NET. Abstrai **RabbitMQ**, **Apache Kafka**, **Azure Service Bus** e **AWS SQS/SNS** com suporte a Saga, Outbox Pattern e Circuit Breaker.

---

## 9. 🔒 Segurança e Boas Práticas

### A. Autenticação e RBAC (Role-Based Access Control)
- **Claims:** Informações embutidas no JWT (`ClaimTypes.NameIdentifier`, `ClaimTypes.Role`).
- **Atributos:**
  - `[Authorize]`: Requer token válido.
  - `[Authorize(Roles = "Admin,Manager")]`: Requer papel específico.
  - `[Authorize(Policy = "MustBeOver18")]`: Regras customizadas de negócio.

### B. Proteção contra SQL Injection
- O **Entity Framework Core** utiliza **consultas parametrizadas por padrão** (como `$1`, `$2` no Postgres). Mesmo usando `.Where(p => p.Name == userInput)`, o EF nunca concatena strings cruas no SQL.

### C. Rate Limiting Nativo (.NET 7+)
Evita ataques de negação de serviço (DDoS) e força bruta:

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});
```

---

## 10. 🧪 Como Rodar e Testar Esta Solução

1. **Rodar os Testes Unitários:**
   ```bash
   dotnet test
   ```

2. **Iniciar a API:**
   ```bash
   dotnet run --project ShopApi.Api/ShopApi.Api.csproj
   ```

3. **Acessar o Swagger UI:**
   - Abra no navegador: `http://localhost:5132` (ou a porta indicada no console).
   - Clique no botão **Authorize** (com ícone de cadeado).
   - Faça login em `POST /api/auth/login` com as credenciais padrão do Seed:
     - **Admin:** `admin@shop.com` / `Admin123!`
     - **Manager:** `manager@shop.com` / `Manager123!`
     - **User:** `user@shop.com` / `User123!`
   - Copie o token retornado e cole no campo: `Bearer {seu_token}`.
