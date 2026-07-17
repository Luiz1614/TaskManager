# TaskManager API

API RESTful para gestão de tarefas, desenvolvida como solução para o desafio técnico de Sistema de Gestão de Tarefas. Permite criar, listar (com filtros), editar e excluir tarefas, com documentação via Swagger e testes automatizados.

## Tecnologias

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core (InMemory)** — persistência em memória, sem necessidade de banco de dados
- **Mapster** — mapeamento entre entidades e DTOs
- **Swashbuckle (Swagger)** — documentação interativa da API
- **xUnit + Moq** — testes automatizados das camadas de domínio e aplicação

## Arquitetura

O projeto segue uma arquitetura em camadas, inspirada nos princípios da Clean Architecture, com as responsabilidades separadas em projetos independentes:

```
TaskManager.sln
├── TaskManager                 → Camada de apresentação (API): controllers, middlewares e configuração (DI, Swagger)
├── TaskManager.Application     → Camada de aplicação: serviços com as regras de orquestração dos casos de uso
├── TaskManager.Domain          → Camada de domínio: entidades, enums e regras de negócio
├── TaskManager.Contracts       → DTOs (requests/responses) usados na comunicação entre camadas
├── TaskManager.Infrastructure  → Camada de infraestrutura: DbContext (EF Core InMemory) e repositórios
├── TaskManager.Transform       → Perfis de mapeamento (Mapster) entre entidades e DTOs
└── TaskManager.Tests           → Testes unitários (xUnit + Moq)
```

### Por que esse modelo?

- **Separação de responsabilidades (SRP)**: cada projeto tem um papel único — o controller só lida com HTTP, o serviço orquestra os casos de uso, a entidade concentra as regras de negócio e o repositório isola o acesso a dados.
- **Inversão de dependência (DIP)**: as camadas dependem de abstrações (`ITaskItemService`, `ITaskItemRepository`), registradas via injeção de dependência no `Program.cs`. Isso permite, por exemplo, trocar o EF InMemory por um banco real sem alterar a camada de aplicação.
- **Testabilidade**: com as dependências abstraídas em interfaces, os serviços são testados isoladamente com mocks (Moq), e o domínio é testado sem nenhuma infraestrutura.
- **Domínio rico**: as regras de negócio (validações de título e data, transições de status) vivem na entidade `TaskItem`, e não espalhadas pelos controllers — o que evita duplicação e mantém o modelo consistente.

## Regras de negócio

Além do CRUD, o domínio aplica as seguintes regras:

- O título é obrigatório (3 a 200 caracteres); a descrição é opcional (até 500 caracteres).
- A data de vencimento, quando informada, não pode ser anterior à data atual (regra aplicada na criação e na edição).
- Não é permitido cadastrar duas tarefas com o mesmo título.
- Toda tarefa é criada com status **Pendente**.
- Transições de status controladas:
  - Uma tarefa **Pendente** precisa passar por **Em Progresso** antes de ser **Concluída**.
  - Uma tarefa **Concluída** não pode voltar para **Pendente**.
  - Não é permitido alterar o status para o mesmo status atual.
- Uma tarefa **Concluída** não pode ser editada.

## Como rodar o projeto

### Pré-requisitos

- [SDK do .NET 10](https://dotnet.microsoft.com/download) (ou superior)

### Executando a API

Na raiz do repositório:

```bash
dotnet run --project TaskManager
```

A API sobe em `http://localhost:5002` (perfil padrão).



### Acessando o Swagger

Com a aplicação rodando em ambiente de desenvolvimento, acesse:

```
http://localhost:5002/swagger
```

A documentação interativa permite testar todos os endpoints diretamente pelo navegador.

> **Observação:** como a persistência usa o provider InMemory do EF Core, os dados são voláteis — ao reiniciar a aplicação, as tarefas cadastradas são perdidas.

## Endpoints

| Método | Rota                 | Descrição                                                        |
|--------|----------------------|------------------------------------------------------------------|
| GET    | `/api/TaskItem`      | Lista todas as tarefas, com filtros opcionais `status` e `dueDate` |
| GET    | `/api/TaskItem/{id}` | Retorna uma tarefa pelo seu identificador                        |
| POST   | `/api/TaskItem`      | Cria uma nova tarefa e retorna o seu código único (GUID)         |
| PUT    | `/api/TaskItem/{id}` | Atualiza título, descrição, data de vencimento e status          |
| DELETE | `/api/TaskItem/{id}` | Exclui uma tarefa                                                |

Os valores possíveis de status são `Pending`, `InProgress` e `Completed` (aceitos como texto graças ao `JsonStringEnumConverter`).

As respostas de consulta incluem o campo calculado `isOverdue`, que indica se a tarefa está atrasada (data de vencimento no passado e status diferente de `Completed`).

### Exemplos

**Criar tarefa** — `POST /api/TaskItem`

Toda tarefa por padrão é criada com o status "Pending"

```json
{
  "title": "Estudar .NET",
  "description": "Revisar injeção de dependência e EF Core",
  "dueDate": "2026-12-31T23:59:00Z"
}
```

**Atualizar tarefa** — `PUT /api/TaskItem/{id}`

```json
{
  "title": "Estudar .NET",
  "description": "Revisar injeção de dependência, EF Core e Mapster",
  "dueDate": "2026-12-31T23:59:00Z",
  "status": "InProgress"
}
```

**Listar com filtros** — `GET /api/TaskItem?status=Pending&dueDate=2026-12-31`

### Códigos de status HTTP

| Código | Quando ocorre                                                            |
|--------|--------------------------------------------------------------------------|
| 200    | Consulta realizada com sucesso                                |
| 201    | Criação realizada com sucesso                                |
| 204    | Atualização ou exclusão realizada com sucesso                            |
| 400    | Dados inválidos (validações de entrada) ou violação de regra de negócio  |
| 404    | Tarefa não encontrada                                                    |
| 500    | Erro inesperado no servidor                                              |

## Tratamento de erros e logging

Todas as exceções passam pelo `ExceptionHandlingMiddleware`, que:

- Converte exceções de domínio/aplicação nos códigos HTTP apropriados (`KeyNotFoundException` → 404, `InvalidOperationException`/`ArgumentException` → 400, demais → 500);
- Registra logs estruturados via `ILogger` (warning para erros de negócio, error para falhas inesperadas);
- Retorna as mensagens de erro em JSON no formato `{ "message": "..." }`.

As validações de entrada dos DTOs usam Data Annotations e são aplicadas automaticamente pelo `[ApiController]`, retornando 400 com os detalhes dos campos inválidos.

## Como rodar os testes

Na raiz do repositório:

```bash
dotnet test
```

Os testes cobrem:

- **Domínio** (`TaskManager.Tests/Domain`): criação e validações da entidade `TaskItem`, regras de transição de status, edição e verificação de atraso (`IsOverdue`);
- **Aplicação** (`TaskManager.Tests/Application`): casos de uso do `TaskItemService` (criação, listagem, busca, atualização e exclusão), com o repositório mockado via Moq.
