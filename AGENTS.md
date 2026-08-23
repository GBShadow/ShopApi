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

Gerado por `scripts/brief.py`. Carregado em toda sessão. Detalhe completo: `memory.py code <arquivo>` / `memory.py symptom "<erro>"`.

### 🚫 Proibições técnicas

- NUNCA referenciar Entity Framework Core, ASP.NET Core ou bibliotecas de I/O dentro de `Domain` `REG-ARQ-001`
- NUNCA injetar `DbContext` diretamente em Controllers da API `REG-ARQ-001`
- NUNCA vazar Entidades de Domínio diretamente no retorno dos endpoints HTTP `REG-ARQ-001`
- NUNCA colocar regras de negócio em Controllers `REG-ARQ-001`
- NUNCA usar segredos (Secret Keys) curtos (< 256 bits / 32 caracteres) `REG-SEC-002`
- NUNCA armazenar senhas em texto puro ou com algoritmos obsoletos (MD5, SHA1) `REG-SEC-002`
- NUNCA inverter a ordem dos Middlewares de Autenticação no pipeline HTTP `REG-SEC-002`
- NUNCA incluir dados sensíveis (senhas, dados bancários) no payload do Token JWT `REG-SEC-002`
- NUNCA concatenar ou interpolar variáveis diretamente em strings SQL `REG-SEC-001`
- NUNCA usar `ExecuteSqlRaw` ou `FromSqlRaw` com interpolação de strings `$""` `REG-SEC-001`
- NUNCA confiar em filtros do lado do cliente (Frontend) para sanitização SQL `REG-SEC-001`
- NUNCA expor mensagens de erro detalhadas de SQL no retorno da API `REG-SEC-001`

### ⚠️ Débitos abertos

- **media** — Ausência de Rotação de Refresh Token no ShopApi `DEB-TEC-004`

### ♻️ Já resolvido em outro projeto (aplica-se aqui)

- Kanban addComment sem verificação de permissão — origem `svelte-app-clean-arch` `DEB-TEC-005`
- createCard/updateCard/moveCard sem escopo de projeto — origem `svelte-app-clean-arch` `DEB-TEC-006`

> Padrão agnóstico de stack. Antes de aplicar, confira a equivalência em `index-por-sintoma.md` (seção ♻️). Busca: `memory.py solve "<problema>"`.

### 🏛️ Decisões vigentes

- Orquestração de modelos por carga cognitiva com fallback funcional `DEC-TEC-005`
- Papéis de Usuário (RBAC) e Catálogo Público vs Administrativo `DEC-NEG-003`
- Arquitetura em 5 Camadas (Clean Architecture) para ShopApi em .NET 8 `DEC-TEC-002`

### 🧭 Antes de agir

1. Vou editar um arquivo → `memory.py code <caminho>`
2. Recebi um erro → `memory.py symptom "<mensagem>"`
3. Vou decidir algo → checar `decisoes-tecnicas/` (protocolo `SKI-GER-001`)
4. Ao concluir → registrar com `evidence` + `source_refs`, depois `score.py` e `reindex.py`
<!-- END:AGENT-MEMORY -->
