# ShopApi — Guia para IA

Guia raiz para agentes que trabalham na API de e-commerce. O contexto persistente
fica em `~/projects/agent-memory`; este arquivo contém o contexto operacional local.

## Prioridade das fontes

1. Código e testes atuais do repositório.
2. Regras específicas do projeto na memória central (`PRJ-SHOP-002`).
3. `README.md`, `GUIA_CRIACAO_CRUD.md` e `GUIA_NODE_PARA_CSHARP.md`.
4. Brief injetado em `AGENTS.md` (`agent-memory`).

Se código e documentação divergirem, confirme o comportamento no código/testes e
registre a correção na memória central quando a divergência for relevante.

## Memória dos agentes — obrigatória

Antes de editar um arquivo, carregue o contexto do caminho e do projeto:

```bash
python3 ~/projects/agent-memory/scripts/memory.py load shop-api
python3 ~/projects/agent-memory/scripts/memory.py code "<arquivo-que-vou-editar>"
python3 ~/projects/agent-memory/scripts/memory.py symptom "<mensagem-literal-do-erro>"
python3 ~/projects/agent-memory/scripts/memory.py solve "<problema>" --projeto shop-api
```

Use `symptom` quando houver erro observável e `solve` antes de investigar um padrão
que pode já ter sido resolvido em outro projeto. O bloco `AGENT-MEMORY` do
`AGENTS.md` é gerado: **não o edite**; a origem é `~/projects/agent-memory`.

## Stack

- **Linguagem/runtime:** C# com .NET 8.
- **API:** ASP.NET Core Web API.
- **Arquitetura:** Clean Architecture + DDD, em camadas `Domain → Application → Infrastructure → Api`.
- **Persistência:** Entity Framework Core Code-First; SQLite no cenário local e banco relacional compatível.
- **Autenticação/autorização:** JWT e RBAC.
- **Contrato/documentação:** Swagger/OpenAPI.
- **Testes:** xUnit, Moq, FluentAssertions e `TestDbContextFactory`.

## Estrutura

```text
ShopApi/
├── ShopApi.Domain/          # Entidades, enums, exceções e regras puras
├── ShopApi.Application/     # DTOs, interfaces, casos de uso e services
├── ShopApi.Infrastructure/  # EF Core, DbContext, migrations e repositórios
├── ShopApi.Api/             # Controllers, middleware, DI e configuração
├── ShopApi.UnitTests/       # Testes unitários
├── ShopApi.sln
├── README.md
├── GUIA_CRIACAO_CRUD.md
└── GUIA_NODE_PARA_CSHARP.md
```

Dependências fluem para dentro: `Api` usa `Application`; `Infrastructure` implementa
as abstrações necessárias; `Domain` não depende de EF Core, ASP.NET Core ou I/O.
Não importe um detalhe de infraestrutura no domínio.

## Comandos

```bash
# restaurar e compilar
dotnet restore
dotnet build

# executar a API
dotnet run --project ShopApi.Api

# banco / migrations
dotnet ef migrations add NomeDaMigration --project ShopApi.Infrastructure --startup-project ShopApi.Api
dotnet ef database update --project ShopApi.Infrastructure --startup-project ShopApi.Api
dotnet ef migrations remove --project ShopApi.Infrastructure --startup-project ShopApi.Api

# testes
dotnet test
dotnet test --filter "FullyQualifiedName~ShopApi.UnitTests.Services.ProductServiceTests"

# documentação local
# https://localhost:5001/swagger ou http://localhost:5000/swagger
```

Antes de declarar uma alteração pronta, rode ao menos `dotnet build` e `dotnet test`;
para mudança de migration, rode também o fluxo de migration aplicável. Relate o
comando e o resultado observado; não declare verde sem executar.

## Regras inegociáveis

### Domínio e entidades

- Toda nova entidade de banco herda de `BaseEntity` (`Id`, `CreatedAt`, `UpdatedAt`).
- Nunca coloque atributos EF Core (`[Table]`, `[Column]`) em `ShopApi.Domain`.
- Exceções de domínio são `NotFoundException`, `ConflictException` ou
  `BadRequestException` de `ShopApi.Domain.Exceptions.AppExceptions`.

### DTOs e API

- Separe DTOs de entrada (`CreateXxxDto`, `UpdateXxxDto`) e saída (`XxxResponseDto`).
- DTO de entrada usa DataAnnotations com mensagens descritivas em português.
- DTO de resposta expõe `FromEntity(Entity entity)` para o mapeamento.
- Controllers apenas orquestram o service e herdam de `BaseApiController`.
- Mutação administrativa em categorias/produtos exige `[Authorize(Roles = "Admin")]`.
- DTO de contrato não deve vazar entidades de domínio nem tipos implícitos do EF Core.

### EF Core e segurança

- Métodos `GetAllAsync` e `GetByIdAsync` usam `.AsNoTracking()`.
- Métodos assíncronos de services/controllers aceitam
  `CancellationToken cancellationToken = default`.
- Acesso a dados passa pelo EF Core; nunca concatene/interpole entrada em SQL.
- Nunca use `ExecuteSqlRaw`/`FromSqlRaw` com interpolação de strings.
- Autenticação usa JWT existente e RBAC; não reimplemente o mecanismo.
- Segredos devem ter no mínimo 256 bits/32 caracteres; senhas nunca ficam em texto
  puro nem usam MD5/SHA-1.
- `UseAuthentication()` deve preceder `UseAuthorization()` no pipeline.
- Não coloque senhas ou dados bancários no payload do JWT.

### Swagger/OpenAPI

Todo novo endpoint documenta o contrato completo:

- `@ApiOperation` com `summary` **e** `description` contextualizada.
- `@ApiResponse` para todos os status possíveis, não apenas 200/201.
- Exemplos representam JSON real; nunca use `example: "string"` ou `example: 0`.
- DTOs de entrada/saída têm decorators de propriedade e peculiaridades dos
  parâmetros de rota documentadas.

## Fluxo de alteração

1. Consultar a memória e ler o contexto do caminho.
2. Para mudança não trivial, descrever objetivo, escopo, impacto em contrato e
   estratégia de teste antes do código; use os guias existentes.
3. Escrever testes que defendam o comportamento alterado.
4. Implementar na camada correta, mantendo o fluxo `Module → Controller → Service`
   e as fronteiras da solução.
5. Rodar os gates (`dotnet build`, `dotnet test` e os específicos do escopo).
6. Revisar diff e documentação. Mudança de regra de negócio ou arquitetura deve
   ser registrada como decisão antes de substituir uma decisão vigente.
7. Registrar na memória central o que foi aprendido:

| Ocorrência comprovada | Registro |
| :--- | :--- |
| Bug resolvido com causa raiz | `~/projects/agent-memory/erros/` (`ERR-*`) |
| Problema encontrado e não corrigido | `~/projects/agent-memory/debitos-tecnicos/` (`DEB-TEC-*`) |
| Decisão técnica | `~/projects/agent-memory/decisoes-tecnicas/` (`DEC-TEC-*`) |
| Lição reutilizável | `~/projects/agent-memory/aprendizados/` (`APR-*`) |

Todo registro exige `evidence` correto e `source_refs` com arquivo/linha reais.
Problema não comprovado não entra em `erros/`; use `exemplos/` ou `debitos-tecnicos/`.
Depois, no repo de memória, rode `normalize_tags.py`, `score.py`, `reindex.py` e
`validate.py` conforme o protocolo. Registros congelados não são editados diretamente;
use `freeze.py --supersede` ou `--amend`.

## Limites de alteração

- Não altere migration aplicada para “corrigir” histórico; crie migration nova.
- Não mova regra de negócio para controller.
- Não introduza DDD/Clean Architecture diferente da estrutura já adotada.
- Não invente incidentes para preencher documentação.
- Não faça commit/push em nome do usuário sem autorização explícita; apresente o diff.
- Não adicione `Co-Authored-By` ou footer de ferramenta aos commits.

## Referências

- Arquitetura e execução: `README.md`
- CRUD: `GUIA_CRIACAO_CRUD.md`
- Migração conceitual Node → C#: `GUIA_NODE_PARA_CSHARP.md`
- Regras persistentes: `AGENTS.md` e `~/projects/agent-memory`
