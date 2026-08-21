# 🛒 ShopApi

API RESTful para e-commerce desenvolvida em **.NET 8** e **C#**, estruturada seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**.

---

## 🏛️ Arquitetura do Projeto

A solução está dividida em camadas bem definidas e desacopladas:

```
ShopApi/
├── ShopApi.Domain/          # Entidades de negócio, Enums e regras de domínio puras
├── ShopApi.Application/     # Casos de uso, DTOs, Interfaces de repositório e Serviços
├── ShopApi.Infrastructure/  # Acesso a dados (EF Core / SQLite / PostgreSQL), Migrations e Repositórios
├── ShopApi.Api/             # Controllers, Middlewares, Injeção de Dependências e Configurações
└── ShopApi.UnitTests/       # Testes unitários automatizados (xUnit / Moq / FluentAssertions)
```

---

## 🚀 Tecnologias e Bibliotecas

- **.NET 8 / C#**
- **ASP.NET Core Web API**
- **Entity Framework Core** (Code-First)
- **SQLite / Relational Database**
- **JWT (JSON Web Tokens)** para autenticação e autorização
- **Swagger / OpenAPI** para documentação interativa
- **xUnit** para testes unitários

---

## ⚙️ Como Executar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado

### Executando a API
```bash
# 1. Restaurar dependências
dotnet restore

# 2. Executar as migrações do banco de dados (se necessário)
dotnet ef database update --project ShopApi.Infrastructure --startup-project ShopApi.Api

# 3. Iniciar a API
dotnet run --project ShopApi.Api
```

Acesse a documentação Swagger em: `https://localhost:5001/swagger` ou `http://localhost:5000/swagger`.

### Executando os Testes
```bash
dotnet test
```
