# Cyberpunk Market API

API REST em .NET 8 para o marketplace Cyberpunk Market, com autenticação JWT, CRUD de usuários e arquitetura em camadas.

---

## Tecnologias

- **.NET 8**
- **ASP.NET Core** (Web API)
- **Entity Framework Core 8** + SQL Server
- **JWT Bearer** (autenticação)
- **BCrypt.Net-Next** (hash de senha)
- **Swagger/OpenAPI**

---

## Estrutura do projeto

```
cyberpunk-market-api/
├── Program.cs                 # Bootstrap, DI, middleware
├── appsettings.Example.json   # Modelo de configuração (copiar para appsettings.json; appsettings está no .gitignore)
├── src/
│   ├── controllers/           # Controllers da API (rotas por recurso)
│   ├── contexts/              # DbContext do EF Core
│   ├── dtos/                  # Objetos de transferência (request/response)
│   ├── interfaces/            # Contratos dos serviços
│   ├── mappers/               # Mapeamento entre entidades, DTOs e responses
│   ├── models/                # Entidades de domínio e enums
│   ├── responses/             # Modelos de resposta da API
│   └── services/              # Lógica de negócio e serviços de infraestrutura
└── Migrations/                # Migrations do EF Core
```

---

## Configuração

### Pré-requisitos

- .NET 8 SDK
- SQL Server (local ou instância acessível)

### appsettings (modelo)

O repositório não contém `appsettings.json` (está no `.gitignore`). Só sobe o modelo:

1. Copie `appsettings.Example.json` para `appsettings.json`.
2. Ajuste os valores em `appsettings.json` (connection string, `Jwt:Key`, origens CORS, etc.).

Quem clona o projeto repete o passo 1 e 2 na máquina local. Em produção, você pode continuar usando variáveis de ambiente (elas sobrescrevem o appsettings).

---

## Executando o projeto

```bash
# Restaurar pacotes
dotnet restore

# Aplicar migrations
dotnet ef database update

# Rodar a API
dotnet run
```

A API sobe por padrão em `https://localhost:7xxx` (ou porta definida em `launchSettings.json`). Documentação Swagger: `https://localhost:<porta>/swagger`.

---

## Endpoints

Base URL: `/api/User`

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| POST   | `/api/User/login`  | Não | Login (email + senha), retorna JWT e dados do usuário |
| POST   | `/api/User/buyer`  | Não | Registrar comprador (nome, email, senha) |
| POST   | `/api/User/seller` | Não | Registrar vendedor (nome, email, senha, storeName, bio); cria User + Seller |
| GET    | `/api/User`        | Sim | Listar todos os usuários |
| GET    | `/api/User/{id}`   | Sim | Buscar usuário por GUID |
| PUT    | `/api/User/{id}`   | Sim | Atualizar usuário (campos opcionais) |
| DELETE | `/api/User/{id}`   | Sim | Remover usuário |

### Autenticação

Rotas protegidas exigem o header:

```
Authorization: Bearer <token>
```

O token é retornado no body do `POST /api/User/login` no campo `data.token`.

---

## Modelo de dados (resumo)

- **BaseEntity**: `Id` (Guid), `CreatedAt`, `UpdatedAt`
- **User**: Name, Email, PasswordHash, Role (enum: Buyer, Seller); relação opcional 1:1 com **Seller**
- **Seller**: UserId, StoreName, Bio; relação 1:N com **Product**
- **Category**: Name, Slug; relação 1:N com **Product**
- **Product**: Name, Description, Price, StockQuantity, IsActive; FKs para Seller e Category

O contexto usa Fluent API (índice único em `User.Email`, precisão em `Product.Price`, `OnDelete.Restrict` em Product → Seller).

---

## Respostas da API

Padrão de resposta encapsulada em `ApiResponse<T>`:

- `success`: boolean
- `message`: string
- `data`: objeto da operação (ou null)
- `errors`: lista de erros (quando `success` é false)

Exemplo de sucesso no login:

```json
{
  "success": true,
  "message": "Login realizado com sucesso.",
  "data": {
    "token": "eyJhbGc...",
    "user": {
      "id": "guid",
      "name": "Nome",
      "email": "email@example.com",
      "role": 1,
      "createdAt": "2025-01-01T00:00:00Z"
    }
  },
  "errors": null
}
```

---

## Health check

- **GET** `/health` — verificação de saúde da aplicação (sem autenticação).

---

## Desenvolvimento

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Build
dotnet build
```

---

## Autor

[ianfelps](https://github.com/ianfelps)
