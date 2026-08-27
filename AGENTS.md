# ShopApi

<!-- BEGIN:AGENT-MEMORY -->
## 🧠 Memória Persistente dos Agentes

> Bloco gerado por `agent-memory/scripts/inject.py`. **Não edite à mão** — as
> alterações vão em `~/projects/agent-memory` e são reinjetadas de lá.

Repositório de memória: `~/projects/agent-memory`
Antes de qualquer tarefa neste projeto, este brief já está carregado. Para o
detalhe completo use a skill `agent-memory` ou os comandos abaixo.

```bash
python3 ~/projects/agent-memory/scripts/memory.py code "<arquivo-que-vou-editar>"
python3 ~/projects/agent-memory/scripts/memory.py symptom "<mensagem-de-erro>"
python3 ~/projects/agent-memory/scripts/memory.py load shop-api
```

> ⚙️ Gerado por `scripts/brief.py` — **não edite à mão**. Carregado em toda sessão. Detalhe completo: `memory.py code <arquivo>` / `memory.py symptom "<erro>"`.

### 🚫 Proibições técnicas (17)

- NUNCA referenciar Entity Framework Core, ASP.NET Core ou bibliotecas de I/O dentro de `Domain` `REG-ARQ-001`
- NUNCA injetar `DbContext` diretamente em Controllers da API `REG-ARQ-001`
- NUNCA vazar Entidades de Domínio diretamente no retorno dos endpoints HTTP `REG-ARQ-001`
- NUNCA colocar regras de negócio em Controllers `REG-ARQ-001`
- NUNCA publicar `@ApiOperation` sem o campo `description` ou apenas com `summary` raso `REG-DOC-001`
- NUNCA omitir respostas de erro em `@ApiResponse` cobrindo apenas status 200/201 `REG-DOC-001`
- NUNCA usar exemplos genéricos ou fictícios como `example: "string"` ou `example: 0` `REG-DOC-001`
- NUNCA deixar DTOs de entrada ou saída sem decorators de propriedade `REG-DOC-001`
- NUNCA esconder peculiaridades de parâmetros de rota `REG-DOC-001`
- NUNCA usar segredos (Secret Keys) curtos (< 256 bits / 32 caracteres) `REG-SEC-002`
- NUNCA armazenar senhas em texto puro ou com algoritmos obsoletos (MD5, SHA1) `REG-SEC-002`
- NUNCA inverter a ordem dos Middlewares de Autenticação no pipeline HTTP `REG-SEC-002`
- NUNCA incluir dados sensíveis (senhas, dados bancários) no payload do Token JWT `REG-SEC-002`
- NUNCA concatenar ou interpolar variáveis diretamente em strings SQL `REG-SEC-001`
- NUNCA usar `ExecuteSqlRaw` ou `FromSqlRaw` com interpolação de strings `$""` `REG-SEC-001`
- NUNCA confiar em filtros do lado do cliente (Frontend) para sanitização SQL `REG-SEC-001`
- _… +1 em REG-SEC-001 — `memory.py search <ID>` para o texto completo._

### ⚠️ Débitos abertos

- **media** — Ausência de Rotação de Refresh Token no ShopApi `DEB-TEC-004`

### 🏛️ Decisões vigentes

- Orquestração de modelos por carga cognitiva com fallback funcional `DEC-TEC-005`
- Filtro de Stack no Roteamento Global da Memória, por Tecnologia Discriminante `DEC-TEC-010`
- Papéis de Usuário (RBAC) e Catálogo Público vs Administrativo `DEC-NEG-003`
- Arquitetura em 5 Camadas (Clean Architecture) para ShopApi em .NET 8 `DEC-TEC-002`

### ♻️ Já resolvido em outro contexto (aplica-se aqui)

- Kanban addComment sem verificação de permissão — origem `svelte-app-clean-arch` `DEB-TEC-005`
- createCard/updateCard/moveCard sem escopo de projeto — origem `svelte-app-clean-arch` `DEB-TEC-006`
- Gerador de Manifesto Sobrescreve Identidade: id e tag Derivados por Heurística Hardcoded… — origem `global` `ERR-INF-004`
- _… +3 — `memory.py solve "<problema>"`._

> Padrão agnóstico de stack: confira a equivalência em `index-por-sintoma.md` (♻️) antes de aplicar.

### 🧭 Antes de agir

1. Vou editar um arquivo → `memory.py code <caminho>`
2. Recebi um erro → `memory.py symptom "<mensagem>"`
3. Vou decidir algo → checar `decisoes-tecnicas/` (protocolo `SKI-GER-001`)
4. Ao concluir → registrar com `evidence` + `source_refs`, depois `score.py` e `reindex.py`
<!-- END:AGENT-MEMORY -->
