# 👤 UserManagement — CRUD de Gerenciamento de Usuários

API RESTful para gerenciamento de usuários construída com **.NET 10**, **SQLite** e **Clean Architecture**, seguindo boas práticas de Clean Code e Object Calisthenics.

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, onde as dependências sempre apontam para dentro — camadas externas conhecem as internas, nunca o contrário.

```
src/
├── UserManagement.Domain/          # Entidades, Value Objects, Exceções
├── UserManagement.Application/     # Use Cases, DTOs, Interfaces, Validators
├── UserManagement.Infrastructure/  # EF Core, SQLite, Repositórios
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

### 2. Execute a API

```bash
dotnet run --project src/UserManagement.API --launch-profile http
```

A API estará disponível em: **`http://localhost:5071`**

> As migrations são aplicadas automaticamente na inicialização. O banco de dados SQLite (`usermanagement.db`) é criado na pasta da API.

### 3. Execute os testes

```bash
dotnet test
```

---

## 📡 Endpoints

### Base URL: `http://localhost:5071`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/users` | Lista todos os usuários |
| `GET` | `/api/users/{id}` | Busca usuário por ID |
| `POST` | `/api/users` | Cria um novo usuário |
| `PUT` | `/api/users/{id}` | Atualiza um usuário |
| `DELETE` | `/api/users/{id}` | Remove um usuário |

---

### POST `/api/users`

**Request Body:**
```json
{
  "name": "Joao Silva",
  "email": "joao@email.com"
}
```

**Response `201 Created`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Joao Silva",
  "email": "joao@email.com",
  "createdAt": "2026-04-06T15:00:00Z",
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
    "createdAt": "2026-04-06T15:00:00Z",
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
`Name` e `Email` são Value Objects — encapsulam validação e são comparados pelo valor, não por referência. Um `Email` inválido simplesmente não pode ser criado.

```csharp
var email = Email.Create("invalido"); // lança DomainException
```

### Use Case Pattern (SRP)
Cada operação é uma classe isolada (`CreateUserUseCase`, `DeleteUserUseCase`, etc.), seguindo o Princípio da Responsabilidade Única.

### Repository Pattern
`IUserRepository` abstrai o acesso ao banco. A Application só conhece a interface, nunca a implementação concreta — facilitando testes e troca de banco de dados.

### Fail Fast
Validações acontecem na borda do sistema (FluentValidation no Controller) e no domínio (Value Objects), evitando que dados inválidos percorram toda a aplicação.

### Tratamento global de erros
`ExceptionHandlingMiddleware` centraliza o tratamento de exceções, mapeando-as para os status HTTP corretos sem `try/catch` nos controllers.

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
│   │   │   └── Name.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       └── NotFoundException.cs
│   │
│   ├── UserManagement.Application/
│   │   ├── DTOs/
│   │   │   ├── CreateUserRequest.cs
│   │   │   ├── UpdateUserRequest.cs
│   │   │   └── UserResponse.cs
│   │   ├── Extensions/
│   │   │   └── UserExtensions.cs
│   │   ├── Interfaces/
│   │   │   └── IUserRepository.cs
│   │   ├── UseCases/
│   │   │   ├── CreateUserUseCase.cs
│   │   │   ├── DeleteUserUseCase.cs
│   │   │   ├── GetAllUsersUseCase.cs
│   │   │   ├── GetUserByIdUseCase.cs
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
│   │   ├── Migrations/
│   │   └── DependencyInjection.cs
│   │
│   └── UserManagement.API/
│       ├── Controllers/
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

