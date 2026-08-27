---
name: "agent-memory"
description: "Consulta a memória persistente dos agentes (~/projects/agent-memory) antes de editar código, investigar erros e registrar aprendizados do ShopApi (shop-api)."
---

# Memória dos Agentes — `shop-api`

A fonte de verdade das regras, decisões, incidentes e débitos é
`~/projects/agent-memory`. O `AGENTS.md` da raiz contém um brief gerado; ele é
contexto de sessão, não arquivo para edição manual.

## Antes de agir

Sempre rode o comando que corresponde ao risco:

```bash
# vou editar este caminho
python3 ~/projects/agent-memory/scripts/memory.py code "ShopApi.Api/Controllers/ProductsController.cs"

# recebi esta mensagem literal
python3 ~/projects/agent-memory/scripts/memory.py symptom "<mensagem-literal-do-erro>"

# vou decidir algo ou investigar um problema de novo
python3 ~/projects/agent-memory/scripts/memory.py solve "<problema>" --projeto shop-api

# preciso do contexto completo do projeto
python3 ~/projects/agent-memory/scripts/memory.py load shop-api
```

`code` consulta o índice por caminho e as memórias aplicáveis; `symptom` procura
causa raiz já comprovada; `solve` atravessa projetos e inclui o acervo arquivado.
Não investigue do zero quando a busca encontrar uma solução transferível — valide
a equivalência no código atual.

## Filtro de stack

Registros globais com tecnologia discriminante só entram no ShopApi quando a
tecnologia está declarada em `stacks:` no `~/projects/agent-memory/taxonomy.yml`.
Swagger/OpenAPI, .NET, EF Core e JWT são tecnologias da stack deste projeto;
regras de SvelteKit/PocketBase não devem ocupar o brief. IDs roteados
explicitamente na seção do projeto continuam sendo curadoria prioritária.
Tecnologias genéricas (`typescript`, `docker`, `pnpm`, `markdown`, `sqlite` e
`monorepo`) não comprovam afinidade sozinhas. Mecanismo: `DEC-TEC-010`.

## Ao concluir

Só registre fatos comprovados e sempre inclua procedência:

| O que aconteceu | Onde registrar |
| :--- | :--- |
| Bug real resolvido com causa raiz | `~/projects/agent-memory/erros/<area>/` (`ERR-*`) |
| Problema identificado e não corrigido | `~/projects/agent-memory/debitos-tecnicos/` (`DEB-TEC-*`) |
| Decisão técnica ou arquitetural | `~/projects/agent-memory/decisoes-tecnicas/` (`DEC-TEC-*`) |
| Lição generalizável | `~/projects/agent-memory/aprendizados/<area>/` (`APR-*`) |
| Padrão didático sem incidente | `~/projects/agent-memory/exemplos/` (`EXM-*`) |

O frontmatter mínimo exige `evidence` e `source_refs` apontando arquivo/linha
reais. `evidence: verificado` só é válido quando a fonte foi observada. Não
registre em `erros/` uma falha que não aconteceu.

Após salvar no repositório central:

```bash
cd ~/projects/agent-memory
python3 scripts/normalize_tags.py
python3 scripts/score.py
python3 scripts/reindex.py
python3 scripts/validate.py
```

O hook também congela novos registros, regenera brief/MAPA, reinjeta os projetos e
compara snapshots. Registros em `decisoes-tecnicas/`, `erros/`, regras de negócio e
descrição de débitos têm corpo congelado: não edite diretamente; use
`freeze.py --supersede` para nova versão ou `freeze.py --amend` para erro material.

## Regra de sincronização

O bloco `AGENT-MEMORY` do `AGENTS.md` é gerado por:

```bash
python3 ~/projects/agent-memory/scripts/brief.py --all --write
python3 ~/projects/agent-memory/scripts/inject.py
```

Nunca edite esse bloco no ShopApi. Atualize a memória central e reinjete. A captura
da configuração deste projeto é feita com:

```bash
python3 ~/projects/agent-memory/scripts/aiconfig.py --diff shop-api
python3 ~/projects/agent-memory/scripts/aiconfig.py --save shop-api
```
