# Cyberpunk Market API

API REST em .NET 8 para o marketplace Cyberpunk Market, com autenticação JWT, CRUD de usuários, produtos, endereços e avaliações, paginação e filtros nas listagens, rate limiting e arquitetura em camadas.

---

## Tecnologias

- **.NET 8**
- **ASP.NET Core** (Web API)
- **Entity Framework Core 8** + SQL Server
- **JWT Bearer** (autenticação)
- **BCrypt.Net-Next** (hash de senha)
- **Swagger/OpenAPI**
- **Rate limiting** (ASP.NET Core built-in)

---

## Estrutura do projeto

```
cyberpunk-market-api/
├── Program.cs                 # Bootstrap, DI, middleware
├── appsettings.Example.json   # Modelo de configuração (copiar para appsettings.json)
├── src/
│   ├── constants/             # Constantes (ex.: paginação)
│   ├── controllers/           # Controllers da API (rotas por recurso)
│   ├── contexts/              # DbContext do EF Core
│   ├── dtos/                  # Objetos de transferência por entidade (User, Product, Address, Review)
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

### Popular o banco com dados de exemplo

Após criar o banco e aplicar as migrations, você pode popular com dados iniciais executando o script `scripts/populate_database.sql` diretamente no SQL Server.

---

## Endpoints

### Usuários

Base URL: `/api/User`

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| POST   | `/api/User/login`  | Não | Login (email + senha), retorna JWT e dados do usuário (rate limit: 20/min) |
| POST   | `/api/User/buyer`  | Não | Registrar comprador (nome, email, senha) (rate limit: 20/min) |
| POST   | `/api/User/seller` | Não | Registrar vendedor (nome, email, senha, storeName, bio); cria User + Seller (rate limit: 20/min) |
| GET    | `/api/User`        | Sim | Listar usuários (paginado). Query: `page`, `pageSize`, `name`, `email`, `role` |
| GET    | `/api/User/{id}`   | Sim | Buscar usuário por GUID |
| PUT    | `/api/User/{id}`   | Sim | Atualizar usuário (campos opcionais) |
| DELETE | `/api/User/{id}`   | Sim | Remover usuário |

### Produtos

Base URL: `/api/Product`

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| GET    | `/api/Product`        | Sim (Buyer, Seller) | Listar produtos (paginado). Query: `page`, `pageSize`, `name`, `categoryId`, `minPrice`, `maxPrice`, `isActive` |
| GET    | `/api/Product/{id}`   | Sim (Buyer, Seller) | Buscar produto por GUID |
| POST   | `/api/Product`        | Sim (Seller)        | Criar novo produto vinculado ao vendedor autenticado |
| PUT    | `/api/Product/{id}`   | Sim (Seller)        | Atualizar produto do vendedor autenticado |
| DELETE | `/api/Product/{id}`   | Sim (Seller)        | Remover produto do vendedor autenticado |

### Endereços

Base URL: `/api/Address` (requer autenticação Buyer ou Seller)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/Address`        | Listar endereços do usuário (paginado). Query: `page`, `pageSize`, `city`, `zipCode` |
| GET    | `/api/Address/{id}`   | Buscar endereço por GUID |
| POST   | `/api/Address`        | Cadastrar endereço |
| PUT    | `/api/Address/{id}`   | Atualizar endereço |
| DELETE | `/api/Address/{id}`   | Remover endereço |

### Avaliações

Base URL: `/api/Review`

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| GET    | `/api/Review`        | Não | Listar avaliações por produto (paginado). Query: `productId`, `page`, `pageSize`, `minRating`, `maxRating` |
| GET    | `/api/Review/{id}`   | Não | Buscar avaliação por GUID |
| POST   | `/api/Review`        | Sim (Buyer, Seller) | Criar avaliação (rating 1–5, comment opcional) |
| PUT    | `/api/Review/{id}`   | Sim (Buyer, Seller) | Atualizar própria avaliação |
| DELETE | `/api/Review/{id}`   | Sim (Buyer, Seller) | Remover própria avaliação |

### Carrinho (Cart)

Base URL: `/api/Cart` (requer autenticação Buyer ou Seller)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/Cart`               | Obter carrinho atual do usuário autenticado |
| POST   | `/api/Cart/items`         | Adicionar item ao carrinho (`productId`, `quantity`) |
| PUT    | `/api/Cart/items/{id}`    | Atualizar quantidade de um item do carrinho (`quantity`; `0` remove o item) |
| DELETE | `/api/Cart/items/{id}`    | Remover item específico do carrinho |
| DELETE | `/api/Cart`               | Esvaziar carrinho (remover todos os itens) |

### Wishlist

Base URL: `/api/Wishlist` (requer autenticação Buyer ou Seller)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/Wishlist`          | Listar itens da wishlist do usuário autenticado |
| POST   | `/api/Wishlist`          | Adicionar produto à wishlist (`productId`, `notifyOnPriceDrop`) |
| PUT    | `/api/Wishlist/{id}`     | Atualizar item da wishlist (ex.: `notifyOnPriceDrop`) |
| DELETE | `/api/Wishlist/{id}`     | Remover item da wishlist |

### Pedidos (Order)

Base URL: `/api/Order` (requer autenticação Buyer ou Seller)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/Order`            | Listar pedidos do usuário autenticado (paginado). Query: `page`, `pageSize`, `status` (1=Pending, 2=Paid, 3=Shipped, 4=Completed, 5=Canceled) |
| GET    | `/api/Order/{id}`       | Buscar pedido por GUID (apenas do usuário autenticado) |
| POST   | `/api/Order`            | Criar pedido a partir do carrinho (`shippingAddressId`, `paymentMethod`) |
| POST   | `/api/Order/{id}/cancel`| Cancelar pedido pendente do usuário autenticado |

### Pagamentos (Payment)

Base URL: `/api/Payment` (requer autenticação Buyer ou Seller)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/Payment/{orderId}`          | Buscar informações de pagamento de um pedido do usuário autenticado |
| POST   | `/api/Payment/{orderId}/complete` | Marcar pagamento como concluído (`externalId` opcional via query) |
| POST   | `/api/Payment/{orderId}/fail`     | Marcar pagamento como falho (`externalId` opcional via query) |

### Autenticação

Rotas protegidas exigem o header:

```
Authorization: Bearer <token>
```

O token é retornado no body do `POST /api/User/login` no campo `data.token`.

---

## Paginação e filtros

As rotas de listagem retornam **resposta paginada** (`PagedResponse<T>` em `data`):

- **Query params comuns**: `page` (default: 1), `pageSize` (default: 10, máx: 50).
- **Resposta**: `items`, `totalCount`, `page`, `pageSize`, `totalPages`, `hasPreviousPage`, `hasNextPage`.

Filtros por recurso:

| Recurso   | Filtros (query) |
|-----------|------------------|
| User      | `name`, `email`, `role` (1=Buyer, 2=Seller) |
| Product   | `name`, `categoryId`, `minPrice`, `maxPrice`, `isActive` |
| Address   | `city`, `zipCode` |
| Review    | `productId` (obrigatório), `minRating`, `maxRating` (1–5) |
| Order     | `status` (1=Pending, 2=Paid, 3=Shipped, 4=Completed, 5=Canceled) |

---

## Rate limiting

- **Global**: 100 requisições por minuto por IP (janela fixa de 1 minuto). Resposta **429** quando excedido.
- **Rotas de autenticação** (login, cadastro buyer, cadastro seller): 20 requisições por minuto por IP para mitigar abuso.

---

## Modelo de dados (resumo)

- **BaseEntity**: `Id` (Guid), `CreatedAt`, `UpdatedAt`
- **User**: Name, Email, PasswordHash, Role (enum: Buyer, Seller); relação opcional 1:1 com **Seller**
- **Seller**: UserId, StoreName, Bio; relação 1:N com **Product**
- **Category**: Name, Slug; relação 1:N com **Product**
- **Product**: Name, Description, Price, StockQuantity, IsActive; FKs para Seller e Category
- **Address**: UserId, Street, Number, Complement, Neighborhood, City, State, ZipCode, IsDefault
- **Review**: UserId, ProductId, Rating (1–5), Comment; índice único (UserId, ProductId)
- **Cart**: UserId, relação 1:N com **CartItem**
- **CartItem**: CartId, ProductId, Quantity
- **WishlistItem**: UserId, ProductId, NotifyOnPriceDrop; índice único (UserId, ProductId)
- **Order**: BuyerId, ShippingAddressId, OrderDate, TotalAmount, Status; relação 1:N com **OrderItem** e 1:1 com **Payment**
- **OrderItem**: OrderId, ProductId, Quantity, UnitPrice, Discount
- **Payment**: OrderId, Amount, Method (enum: CreditCard, DebitCard, Pix, BankTransfer), Status (enum: Pending, Completed, Failed, Refunded), ExternalId, PaidAt

O contexto usa Fluent API (índice único em `User.Email`, precisão em `Product.Price`, `OnDelete.Restrict`/`Cascade` conforme o caso).

---

## Respostas da API

Padrão de resposta encapsulada em `ApiResponse<T>`:

- `success`: boolean
- `message`: string
- `data`: objeto da operação (ou null). Em listagens paginadas, é um `PagedResponse<T>` com `items`, `totalCount`, `page`, `pageSize`, `totalPages`, etc.
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

Exemplo de listagem paginada (`GET /api/Product?page=1&pageSize=10`):

```json
{
  "success": true,
  "message": "Operação realizada com sucesso",
  "data": {
    "items": [ { "id": "...", "name": "...", "price": 99.90, ... } ],
    "totalCount": 42,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
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

## Decisões técnicas

- **Autenticação**: JWT Bearer com validação de emissor, audiência e chave; token também aceito via cookie `accessToken` para integração com frontend.
- **Senhas**: BCrypt para hash; não há exposição de senha em resposta ou log.
- **Respostas da API**: Todas as rotas retornam `ApiResponse<T>` com `success`, `message`, `data` e `errors`, permitindo tratamento uniforme no cliente.
- **DTOs por entidade**: DTOs organizados em subpastas (`dtos/User`, `dtos/Product`, `dtos/Address`, `dtos/Review`, `dtos/Cart`, `dtos/Wishlist`, `dtos/Order`) para separação por domínio.
- **Entity Framework**: Fluent API para configuração (índices, precisão decimal, relacionamentos e `OnDelete`). Uso de `Restrict` em FKs críticas (ex.: Order → User, Address → User) para evitar exclusão em cascata indesejada; `Cascade` apenas onde faz sentido (ex.: Review → Product).
- **Exclusão de usuário**: Verificação explícita de dependentes (pedidos, endereços, carrinho, avaliações, lista de desejos, perfil vendedor com produtos) antes de permitir exclusão; mensagens de erro específicas para cada caso.
- **Endereço padrão**: Ao marcar um endereço como `IsDefault`, os demais do mesmo usuário são atualizados para não padrão no serviço de endereços.
- **Paginação**: Listagens usam `page` e `pageSize` (máx. 50), com resposta `PagedResponse<T>`; filtros por recurso via query string.
- **Rate limiting**: Limite global por IP (100 req/min); política mais restritiva (20 req/min) em login e cadastro.
- **Arquitetura em camadas**: Controllers expõem apenas as rotas HTTP; regras de negócio ficam em services (`src/services`), que implementam interfaces (`src/interfaces`) e conversões entre domínio/DTO/response são centralizadas em mappers (`src/mappers`). Entidades de domínio vivem em `src/models` e não conhecem web ou banco diretamente.
- **Organização por contexto**: Cada área funcional (usuários, produtos, endereços, avaliações, carrinho, wishlist, pedidos, pagamentos) possui DTOs, services, interfaces e, quando necessário, mappers e responses específicos, mantendo baixo acoplamento e facilitando evolução isolada.
- **Fluxo de checkout**: O carrinho (`Cart`) é a origem dos itens do pedido; o serviço de pedidos cria `Order` + `OrderItem` + `Payment` a partir do carrinho, limpa o carrinho após a criação e controla estados de pedido/pagamento (por exemplo, apenas pedidos `Pending` podem ser cancelados).
- **Estados de pedido e pagamento**: Os enums `OrderStatus` e `PaymentStatus` modelam o ciclo de vida; o serviço de pagamento sincroniza o status do pedido com o resultado do pagamento (ex.: pagamento concluído muda pedido para `Paid`, falha em pagamento pendente pode cancelar o pedido).

---