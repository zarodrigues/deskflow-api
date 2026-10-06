# DeskFlow API — Gestão de Chamados e Helpdesk de TI

## 🎯 Sobre o Projeto

A **DeskFlow API** é uma Web API RESTful construída em **.NET 10** com **Entity Framework Core** e **SQL Server**. Ela centraliza o fluxo de suporte técnico de uma empresa, que antes ficava espalhado em e-mails, mensagens e planilhas.

Com ela é possível:

- cadastrar e gerenciar **categorias** de TI (Hardware, Software, Redes, Gestão de Acessos...);
- **abrir chamados** informando solicitante, prioridade e descrição do problema;
- controlar o **ciclo de vida** do atendimento (Aberto → EmAndamento → Fechado);
- registrar o **histórico de interações** (notas técnicas e comentários) em cada chamado;
- **consultar e filtrar** chamados por status, prioridade e categoria;
- receber **respostas de erro padronizadas** em JSON, sem vazamento de stack trace.

Projeto final do Módulo 01 — Desenvolvedor(a) Back End .NET.

**Autor:** Isaías Rodrigues

## 🛠️ Tecnologias Utilizadas

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 (Migrations)
- SQL Server
- C# (async/await, LINQ, injeção de dependência)
- Git e GitHub (branches por funcionalidade e commits semânticos)

## 🧱 Arquitetura em Camadas

A aplicação segue o fluxo **Controller → Service → Repository**, com cada camada tendo uma única responsabilidade:

```
deskflow-api/
 ├── database/
 │    └── deskflow.sql              → script SQL para criar o banco
 ├── DeskFlow.API/
 │    ├── Controllers/              → rotas HTTP e status codes
 │    ├── Services/                 → regras de negócio e validações
 │    ├── Repositories/             → acesso a dados (EF Core)
 │    ├── Models/
 │    │    ├── Entities/            → Categoria, Chamado, Interacao e enums
 │    │    └── DTOs/                → objetos de entrada e saída da API
 │    ├── Exceptions/               → exceções de negócio
 │    ├── Middlewares/              → tratamento global de erros
 │    ├── Data/                     → AppDbContext
 │    ├── Migrations/               → versionamento do banco
 │    └── Program.cs                → configuração e injeção de dependências
 └── README.md
```

- **Controllers:** recebem a requisição, chamam o Service e escolhem o status HTTP. Não têm regra de negócio.
- **Services:** validam os dados e aplicam as regras (por exemplo, a transição de status do chamado). Lançam exceções quando uma regra é violada.
- **Repositories:** únicos que falam com o banco, através do `AppDbContext`.
- **DTOs:** controlam o que entra e o que sai da API (por exemplo, o cliente não pode definir o status nem a data de abertura de um chamado).
- **Middleware:** captura as exceções de qualquer ponto da aplicação e as converte em respostas JSON padronizadas.
- **Injeção de dependência:** as interfaces dos Repositories e Services são registradas no `Program.cs` com `AddScoped`, e o ASP.NET entrega as implementações nos construtores.

## 🚀 Como Executar a Aplicação

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server em execução (instância padrão, SQL Server Express ou Docker)
- Ferramenta do EF Core:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Passo a passo

1. Clone o repositório:
   ```bash
   git clone https://github.com/zarodrigues/deskflow-api.git
   cd deskflow-api
   ```

2. Ajuste a connection string em `DeskFlow.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=DeskFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
   > Se o seu SQL Server for uma instância nomeada, troque `localhost` por `localhost\SQLEXPRESS`.

3. Crie o banco de dados (escolha **uma** das opções):

   **Opção A — Migrations do EF Core:**
   ```bash
   cd DeskFlow.API
   dotnet ef database update
   ```

   **Opção B — Script SQL:** execute o arquivo `database/deskflow.sql` no SQL Server (ele cria o banco `DeskFlowDb` e as tabelas).

4. Execute a API:
   ```bash
   cd DeskFlow.API
   dotnet run
   ```

5. A API ficará disponível em `http://localhost:5159`. Teste com:
   ```
   GET http://localhost:5159/api/categorias
   ```

## 🧠 Ciclo de Vida do Chamado

```
Aberto  ──(iniciar)──▶  EmAndamento  ──(encerrar)──▶  Fechado
```

| Status | Significado |
|---|---|
| **Aberto** | Chamado registrado pelo solicitante (status e data de abertura definidos pelo sistema). |
| **EmAndamento** | Suporte atendendo o chamado. |
| **Fechado** | Chamado encerrado, com texto de solução e data de fechamento. |

Regras validadas na camada de Service:

- só é possível **iniciar** um chamado com status `Aberto`;
- só é possível **encerrar** um chamado com status `EmAndamento`, e a **solução é obrigatória**;
- não é possível **adicionar interações** a um chamado `Fechado`;
- não é possível **remover uma categoria** que possui chamados;
- não é possível abrir um chamado com **categoria inexistente**.

## 📌 Endpoints

### Categorias

| Método | Rota | Descrição | Sucesso |
|---|---|---|---|
| POST | `/api/categorias` | Cadastra uma categoria | 201 |
| GET | `/api/categorias` | Lista todas as categorias | 200 |
| GET | `/api/categorias/{id}` | Busca uma categoria por Id | 200 |
| PUT | `/api/categorias/{id}` | Atualiza o nome da categoria | 204 |
| DELETE | `/api/categorias/{id}` | Remove (bloqueia se houver chamados) | 204 |

### Chamados

| Método | Rota | Descrição | Sucesso |
|---|---|---|---|
| POST | `/api/chamados` | Abre um novo chamado | 201 |
| GET | `/api/chamados` | Lista com filtros opcionais: `status`, `prioridade`, `categoriaId` | 200 |
| GET | `/api/chamados/{id}` | Detalhes com categoria e interações | 200 |
| POST | `/api/chamados/{id}/iniciar` | Aberto → EmAndamento | 204 |
| POST | `/api/chamados/{id}/encerrar` | EmAndamento → Fechado (exige solução) | 204 |
| POST | `/api/chamados/{id}/interacoes` | Adiciona comentário ao chamado | 201 |

> Os endpoints de transição (`iniciar` e `encerrar`) usam o verbo **POST**, conforme descrito nos requisitos funcionais (RF07 e RF08).

Valores aceitos (enviados como texto no JSON):

- `prioridade`: `Baixa`, `Media`, `Alta`
- `status`: `Aberto`, `EmAndamento`, `Fechado`

### Exemplos de uso

**Cadastrar categoria** — `POST /api/categorias`
```json
{ "nome": "Hardware" }
```

**Abrir chamado** — `POST /api/chamados`
```json
{
  "titulo": "Computador não liga",
  "descricao": "Já testei o cabo de energia",
  "prioridade": "Alta",
  "solicitanteNome": "Maria",
  "categoriaId": 1
}
```

**Encerrar chamado** — `POST /api/chamados/1/encerrar`
```json
{ "solucao": "Troca da fonte de alimentação" }
```

**Adicionar interação** — `POST /api/chamados/1/interacoes`
```json
{ "autor": "Ana", "mensagem": "Verifiquei o cabo de energia" }
```

**Filtrar chamados** — `GET /api/chamados?status=Fechado&prioridade=Alta&categoriaId=1`

## ⚠️ Tratamento de Erros

Um `ExceptionHandlingMiddleware` customizado captura as exceções e devolve um JSON padronizado, sem expor stack trace:

| Situação | Status | Exemplo de resposta |
|---|---|---|
| Recurso não encontrado | 404 | `{"status":404,"erro":"Categoria 99 não encontrada."}` |
| Regra de negócio violada | 400 | `{"status":400,"erro":"O nome da categoria é obrigatório."}` |
| Falha inesperada | 500 | `{"status":500,"erro":"Ocorreu um erro interno. Tente novamente mais tarde."}` |

## 🗃️ Banco de Dados

O banco é versionado com **EF Core Migrations** (pasta `DeskFlow.API/Migrations`). A tabela `__EFMigrationsHistory` registra quais migrations já foram aplicadas, e por isso o `dotnet ef database update` executa apenas as que ainda faltam.

Relacionamentos:

- **Categoria 1:N Chamado**
- **Chamado 1:N Interação**

O script `database/deskflow.sql` foi gerado a partir das migrations e permite criar o banco sem usar o EF Core.

## 🔮 Melhorias Futuras

- Autenticação e autorização com ASP.NET Core Identity e tokens JWT.
- Proteção no banco contra exclusão de categorias com chamados (`DeleteBehavior.Restrict`).
- Paginação na listagem de chamados.
- Testes automatizados (unitários e de integração).

## 🎥 Vídeo de Apresentação

[Clique aqui para assistir ao vídeo de demonstração do projeto](COLE-O-LINK-DO-VIDEO-AQUI)
