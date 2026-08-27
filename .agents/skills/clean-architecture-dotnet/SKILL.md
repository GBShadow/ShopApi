---
name: "clean-architecture-dotnet"
description: "Estruturação de soluções .NET 8/9 com Clean Architecture, Ports & Adapters, MediatR, FluentValidation e isolamento do domínio."
id: "SKL-032"
title: "Skill: Clean Architecture & Ports and Adapters em .NET"
date: "2026-08-27"
last_updated: "2026-08-27"
tags: ['skill', 'backend', 'arquitetura', 'dotnet', 'csharp', 'clean-architecture']
category: "skills"
scope: "global"
project: null
status: "vigente"
evidence: "inferido"
verified_at: "2026-08-27"
score: 10
archived: false
---

# 🏛️ Clean Architecture em .NET (.NET 8/9)

## 🎯 Objetivo
Garantir o isolamento total da camada de domínio em relação a frameworks, ORMs e bibliotecas de apresentação em projetos .NET.

---

## 🏗️ Estrutura de Camadas
1. **Domain**: Entidades ricas, Value Objects, Domain Events e interfaces de repositórios. Nenhuma dependência externa.
2. **Application**: Casos de uso (Commands/Queries MediatR), DTOs, Behaviors de validação (FluentValidation) e interfaces de serviços.
3. **Infrastructure**: Implementação do `DbContext` EF Core, repositórios, mensageria e clientes HTTP.
4. **Api / Presentation**: Controllers ou Minimal APIs, filtros globais de exceção, Swagger e autenticação/autorização.
