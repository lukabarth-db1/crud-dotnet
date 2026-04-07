# 👤 UserManagement — CRUD de Gerenciamento de Usuários

API RESTful para gerenciamento de usuários construída com **.NET 10**, **SQLite** e **Clean Architecture**, seguindo boas práticas de Clean Code e Object Calisthenics.

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, onde as dependências sempre apontam para dentro — camadas externas conhecem as internas, nunca o contrário.

```
src/
├── UserManagement.Domain/          # Entidades, Value Objects, Exceções
├── UserManagement.Application/     # Use Cases, DTOs, Interfaces, Validators
├── UserManagement.Infrastructure/  # EF Core, SQLite, Repositórios, JWT, BCrypt
└── UserManagement.API/             # Controllers, Middlewares, Program.cs

tests/
└── UserManagement.Tests/           # Testes unitários (xUnit + Moq)
```

### Fluxo de dependências

```
Domain ← Application ← Infrastructure
                    ↖ API → Infrastructure
```

---

## 🚀 Tecnologias

| Tecnologia | Versão | Uso |
|---|---|---|
| .NET | 10 | Framework principal |
| ASP.NET Core | 10 | Web API |
| Entity Framework Core | 10 | ORM |
| SQLite | — | Banco de dados |
| FluentValidation | 12 | Validação de DTOs |
| BCrypt.Net | 4 | Hash de senhas |
| JwtBearer | 10 | Autenticação via JWT |
| xUnit | — | Testes unitários |
| Moq | 4 | Mocking nos testes |

---

## 📋 Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/)

---

## ⚙️ Como executar

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd crud
```

### 2. Configure a SecretKey via User Secrets

A `SecretKey` do JWT **não deve ser commitada**. Configure-a localmente com User Secrets:

```bash
dotnet user-secrets init --project src/UserManagement.API
dotnet user-secrets set "Jwt:SecretKey" "<sua-chave-de-32-chars-ou-mais>" --project src/UserManagement.API
```

Para gerar uma chave segura aleatória via PowerShell:

```powershell
[Convert]::ToBase64String((1..32 | ForEach-Object { [byte](Get-Random -Max 256) }))
```

### 3. Execute a API

```bash
dotnet run --project src/UserManagement.API --launch-profile http
```

A API estará disponível em: **`http://localhost:5071`**

> As migrations são aplicadas automaticamente na inicialização. O banco de dados SQLite (`usermanagement.db`) é criado na pasta da API.

### 4. Execute os testes

```bash
dotnet test
```

---

## 📡 Endpoints

### Base URL: `http://localhost:5071`

#### Autenticação

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `POST` | `/api/auth/login` | Autentica e retorna o token JWT | ❌ Pública |

#### Usuários

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/users` | Lista todos os usuários | ✅ Requer token |
| `GET` | `/api/users/{id}` | Busca usuário por ID | ✅ Requer token |
| `POST` | `/api/users` | Cria um novo usuário | ✅ Requer token |
| `PUT` | `/api/users/{id}` | Atualiza um usuário | ✅ Requer token |
| `DELETE` | `/api/users/{id}` | Remove um usuário | ✅ Requer token |

---

### POST `/api/auth/login`

**Request Body:**
```json
{
  "email": "joao@email.com",
  "password": "senha123"
}
```

**Response `200 OK`:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-04-07T16:00:00Z"
}
```

**Response `401 Unauthorized`:**
```json
{ "error": "Invalid credentials." }
```

> Use o token retornado nas demais requisições via header: `Authorization: Bearer <token>`

---

### POST `/api/users`

**Request Body:**
```json
{
  "name": "Joao Silva",
  "email": "joao@email.com",
  "password": "senha123"
}
```

**Response `201 Created`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Joao Silva",
  "email": "joao@email.com",
  "createdAt": "2026-04-07T15:00:00Z",
  "updatedAt": null
}
```

---

### GET `/api/users`

**Response `200 OK`:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Joao Silva",
    "email": "joao@email.com",
    "createdAt": "2026-04-07T15:00:00Z",
    "updatedAt": null
  }
]
```

---

### GET `/api/users/{id}`

**Response `200 OK`:** objeto do usuário  
**Response `404 Not Found`:**
```json
{ "error": "User with id '...' not found." }
```

---

### PUT `/api/users/{id}`

**Request Body:**
```json
{
  "name": "Joao Santos",
  "email": "joao.santos@email.com"
}
```

**Response `200 OK`:** objeto atualizado com `updatedAt` preenchido  
**Response `404 Not Found`:** usuário não encontrado  
**Response `422 Unprocessable Entity`:** email já em uso

---

### DELETE `/api/users/{id}`

**Response `204 No Content`:** deletado com sucesso  
**Response `404 Not Found`:** usuário não encontrado

---

## ⚠️ Respostas de erro

| Status | Descrição |
|---|---|
| `401 Unauthorized` | Token ausente, inválido ou expirado |
| `404 Not Found` | Recurso não encontrado |
| `422 Unprocessable Entity` | Violação de regra de negócio ou dados inválidos |
| `500 Internal Server Error` | Erro inesperado no servidor |

**Formato padrão de erro:**
```json
{ "error": "Mensagem descritiva do erro." }
```

**Formato de erro de validação (`422`):**
```json
["Name must have at least 2 characters.", "A valid email address is required."]
```

---

## 🧱 Conceitos e boas práticas aplicadas

### Clean Architecture
Separação em camadas com dependências apontando para o domínio. A camada de domínio não conhece nenhuma outra camada.

### Value Objects
`Name`, `Email` e `Password` são Value Objects — encapsulam validação e são comparados pelo valor, não por referência. Um `Email` inválido simplesmente não pode ser criado.

```csharp
var email = Email.Create("invalido"); // lança DomainException
```

### Use Case Pattern (SRP)
Cada operação é uma classe isolada (`CreateUserUseCase`, `LoginUseCase`, etc.), seguindo o Princípio da Responsabilidade Única.

### Repository Pattern
`IUserRepository` abstrai o acesso ao banco. A Application só conhece a interface, nunca a implementação concreta — facilitando testes e troca de banco de dados.

### Inversão de Dependência no JWT
A camada Application define `IJwtTokenGenerator`, sem saber nada sobre JWT. A Infrastructure implementa com `JwtTokenGenerator`. O Use Case de login apenas chama a interface:

```csharp
return _tokenGenerator.GenerateToken(user); // retorna AuthResponse com token + expiresAt
```

### Encapsulamento da expiração
O `JwtTokenGenerator` calcula `expiresAt` uma única vez e usa o mesmo valor tanto para o token quanto para o `AuthResponse`, garantindo consistência entre os dois.

### Segurança nas credenciais inválidas
O `LoginUseCase` retorna a mesma mensagem genérica para email inexistente e senha errada, prevenindo **enumeração de usuários**:

```csharp
if (user is null || !_passwordHasher.Verify(request.Password, user.GetPasswordHash()))
    throw new UnauthorizedException("Invalid credentials.");
```

### Fail Fast
Validações acontecem na borda do sistema (FluentValidation no Controller) e no domínio (Value Objects), evitando que dados inválidos percorram toda a aplicação.

### Tratamento global de erros
`ExceptionHandlingMiddleware` centraliza o tratamento de exceções, mapeando-as para os status HTTP corretos sem `try/catch` nos controllers.

---

## 🔐 Configuração do JWT

O `appsettings.json` contém as configurações não sensíveis do JWT:

```json
"Jwt": {
  "SecretKey": "",
  "Issuer": "UserManagement.API",
  "Audience": "UserManagement.Client",
  "ExpirationInMinutes": 60
}
```

A `SecretKey` deve ser configurada via **User Secrets** (desenvolvimento) ou **variável de ambiente** (produção):

```bash
# Desenvolvimento
dotnet user-secrets set "Jwt:SecretKey" "<sua-chave>" --project src/UserManagement.API

# Produção — o .NET mapeia __ como separador de seção automaticamente
Jwt__SecretKey=<sua-chave>
```

---

## 🔑 Como o JWT funciona neste projeto

### Visão geral do fluxo

```
1. Cliente envia POST /api/auth/login  { email, password }
          ↓
2. AuthController → LoginUseCase
          ↓
3. Busca o usuário pelo e-mail no banco (IUserRepository)
          ↓
4. Verifica a senha com BCrypt (IPasswordHasher)
          ↓
5. Gera o token JWT (IJwtTokenGenerator)
          ↓
6. Retorna { token, expiresAt }

─────────────────────────────────────────────────────────

7. Cliente envia GET /api/users
   Header: Authorization: Bearer <token>
          ↓
8. JwtBearer Middleware intercepta a requisição
   └── Valida assinatura, issuer, audience e expiração
          ↓
9. [Authorize] verifica se o usuário está autenticado
          ↓
10. Controller executa normalmente
```

---

### Estrutura do token

Um token JWT é composto por 3 partes separadas por `.`, todas em Base64:

```
eyJhbGciOiJIUzI1NiJ9          ← Header  (algoritmo: HS256)
.eyJzdWIiOiIxMjM0In0           ← Payload (claims: sub, email, jti, exp...)
.SflKxwRJSMeKKF2QT4fwpMeJf36   ← Signature (HMAC-SHA256 da chave secreta)
```

Os **claims** embutidos no payload deste projeto são:

| Claim | Valor | Descrição |
|---|---|---|
| `sub` | `user.Id` | Identificador do usuário (padrão JWT) |
| `email` | `user.Email.Value` | E-mail do usuário |
| `jti` | `Guid.NewGuid()` | ID único do token — útil para revogação futura |
| `exp` | `UtcNow + ExpirationInMinutes` | Data/hora de expiração |

---

### Como a Clean Architecture foi respeitada

O grande desafio ao implementar JWT com Clean Architecture é **não vazar detalhes de infraestrutura para as camadas internas**. Veja como foi resolvido:

```
┌─────────────────────────────────────────────────────┐
│ Application                                         │
│                                                     │
│  LoginUseCase                                       │
│    └── chama IJwtTokenGenerator.GenerateToken()     │  ← interface, sem saber o que é JWT
│          └── retorna AuthResponse                   │
└─────────────────────────────────────────────────────┘
                         ↑ depende da abstração
┌─────────────────────────────────────────────────────┐
│ Infrastructure                                      │
│                                                     │
│  JwtTokenGenerator : IJwtTokenGenerator             │
│    └── usa System.IdentityModel.Tokens.Jwt          │  ← detalhe de infra isolado aqui
│    └── lê JwtSettings via IOptions<JwtSettings>     │
└─────────────────────────────────────────────────────┘
```

A camada **Application nunca referencia** `System.IdentityModel`, `JwtSecurityToken` ou qualquer detalhe de JWT — ela só conhece `IJwtTokenGenerator` e `AuthResponse`.

---

### Por que o `GenerateToken` retorna `AuthResponse` e não `string`?

Retornar apenas a `string` do token forçaria o `LoginUseCase` a calcular o `expiresAt` por conta própria — o que exigiria que ele conhecesse o valor de `ExpirationInMinutes`, uma configuração que pertence à Infrastructure.

Retornando `AuthResponse` diretamente, o `JwtTokenGenerator` calcula `expiresAt` **uma única vez** e usa o mesmo valor tanto para o token (`expires:`) quanto para a resposta ao cliente — garantindo consistência:

```csharp
// JwtTokenGenerator.cs
var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes);

var token = new JwtSecurityToken(expires: expiresAt, ...); // ← mesmo valor
return new AuthResponse(tokenString, expiresAt);           // ← mesmo valor
```

---

### Validação automática pelo middleware

Ao chamar `app.UseAuthentication()` no pipeline, o ASP.NET Core intercepta **toda requisição** e, se houver um header `Authorization: Bearer <token>`, valida automaticamente:

| Validação | O que verifica |
|---|---|
| `ValidateIssuerSigningKey` | A assinatura foi gerada com a `SecretKey` correta |
| `ValidateIssuer` | O token foi emitido por `UserManagement.API` |
| `ValidateAudience` | O token é destinado a `UserManagement.Client` |
| `ValidateLifetime` | O token não está expirado (`exp` > agora) |

Se qualquer validação falhar, o middleware retorna **401 Unauthorized** antes mesmo de chegar ao controller — sem nenhum código adicional necessário.

---

### Proteção dos endpoints

O atributo `[Authorize]` no `UsersController` instrui o ASP.NET Core a exigir um token válido em todos os seus endpoints. O `AuthController` não tem esse atributo pois precisa ser público (é ele quem fornece o token):

```csharp
[Authorize]          // ← todos os endpoints de /api/users exigem token
public sealed class UsersController : ControllerBase { ... }

// sem [Authorize]   ← /api/auth/login é público
public sealed class AuthController : ControllerBase { ... }
```

---

## 🧪 Testes

14 testes unitários cobrindo todos os Use Cases:

```
✅ CreateUserUseCase  — 4 testes
✅ GetUserByIdUseCase — 2 testes
✅ GetAllUsersUseCase — 2 testes
✅ UpdateUserUseCase  — 4 testes
✅ DeleteUserUseCase  — 2 testes
```

```bash
dotnet test --logger "console;verbosity=normal"
```

---

## 📁 Estrutura completa

```
crud/
├── src/
│   ├── UserManagement.Domain/
│   │   ├── Entities/
│   │   │   └── User.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── Name.cs
│   │   │   └── Password.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       ├── NotFoundException.cs
│   │       └── UnauthorizedException.cs
│   │
│   ├── UserManagement.Application/
│   │   ├── DTOs/
│   │   │   ├── AuthResponse.cs
│   │   │   ├── CreateUserRequest.cs
│   │   │   ├── LoginRequest.cs
│   │   │   ├── UpdateUserRequest.cs
│   │   │   └── UserResponse.cs
│   │   ├── Extensions/
│   │   │   └── UserExtensions.cs
│   │   ├── Interfaces/
│   │   │   ├── IJwtTokenGenerator.cs
│   │   │   ├── IPasswordHasher.cs
│   │   │   └── IUserRepository.cs
│   │   ├── UseCases/
│   │   │   ├── CreateUserUseCase.cs
│   │   │   ├── DeleteUserUseCase.cs
│   │   │   ├── GetAllUsersUseCase.cs
│   │   │   ├── GetUserByIdUseCase.cs
│   │   │   ├── LoginUseCase.cs
│   │   │   └── UpdateUserUseCase.cs
│   │   ├── Validators/
│   │   │   ├── CreateUserRequestValidator.cs
│   │   │   └── UpdateUserRequestValidator.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── UserManagement.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Configurations/
│   │   │   │   └── UserConfiguration.cs
│   │   │   └── AppDbContext.cs
│   │   ├── Repositories/
│   │   │   └── UserRepository.cs
│   │   ├── Security/
│   │   │   ├── JwtSettings.cs
│   │   │   ├── JwtTokenGenerator.cs
│   │   │   └── PasswordHasher.cs
│   │   ├── Migrations/
│   │   └── DependencyInjection.cs
│   │
│   └── UserManagement.API/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   └── UsersController.cs
│       ├── Middlewares/
│       │   └── ExceptionHandlingMiddleware.cs
│       ├── appsettings.json
│       └── Program.cs
│
└── tests/
    └── UserManagement.Tests/
        └── UseCases/
            ├── CreateUserUseCaseTests.cs
            ├── DeleteUserUseCaseTests.cs
            ├── GetAllUsersUseCaseTests.cs
            ├── GetUserByIdUseCaseTests.cs
            └── UpdateUserUseCaseTests.cs
```