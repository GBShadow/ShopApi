---
name: "efcore-migration-manager"
description: "Gerenciamento de migrations no Entity Framework Core, queries compiladas, split queries e prevenção de tracking desnecessário."
id: "SKL-038"
title: "Skill: EF Core Migrations & Performance Tuning"
date: "2026-08-27"
last_updated: "2026-08-27"
tags: ['skill', 'backend', 'dados', 'ef-core', 'dotnet']
category: "skills"
scope: "global"
project: null
status: "vigente"
evidence: "inferido"
verified_at: "2026-08-27"
score: 10
archived: false
---

# 🗃️ Entity Framework Core Performance

## 🎯 Objetivo
Otimizar consultas e gerenciar migrations de banco de dados com segurança e performance no EF Core.

---

## 🛠️ Regras Mandatórias
1. **AsNoTracking em Leitura**: Sempre usar `.AsNoTracking()` em consultas somente-leitura.
2. **Split Queries**: Usar `.AsSplitQuery()` em consultas com múltiplos `.Include()` para evitar explosão cartesiana.
3. **Migrations Idempotentes**: Gerar scripts SQL de migração com `--idempotent` para deploys em produção.
