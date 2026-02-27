# CLAUDE.md

Este arquivo fornece orientações ao Claude Code (claude.ai/code) ao trabalhar com este repositório.

## Comandos

```bash
# Executar a aplicação localmente
cd src/AgendaPics.Web && dotnet run

# Build da solução
dotnet build agenda-pics.sln

# Executar via Docker Compose (app exposta na porta 32033)
docker compose up -d --build
```

### Testes

```bash
# Executar testes unitários
dotnet test tests/AgendaPics.Application.Tests/AgendaPics.Application.Tests.csproj --configuration Release --verbosity normal
```

O projeto de testes fica em `tests/AgendaPics.Application.Tests/` (xUnit + Moq + FluentAssertions). São 120 testes unitários cobrindo os 26 handlers da camada Application — sem dependências externas (banco, e-mail, etc.).

## Banco de Dados

O banco de dados é criado e populado **automaticamente na inicialização** via EF Core (`Database.Migrate()` em `Program.cs`), com lógica de retry (20 tentativas × 5 segundos) para aguardar o SQL Server ficar disponível.

A migration `CriacaoInicial` cria todas as tabelas e insere os dados iniciais (estados, cidades e o usuário admin padrão: `login: admin`, `senha: 123`) via `HasData()`.

No Docker Compose, o SQL Server pode demorar até 60 segundos para inicializar. Se o container da aplicação falhar antes disso, reinicie apenas ele:

```bash
docker compose restart agendapics
```

Acompanhe os logs para verificar o progresso:

```bash
docker logs agendapicsweb -f
```

## Arquitetura

Arquitetura limpa em 4 camadas em `src/`:

```
AgendaPics.Domain         — Entidades, enums, interfaces de domínio, padrão Result<T>
AgendaPics.Application    — Features CQRS, mediator customizado, FluentValidation
AgendaPics.Infrastructure — EF Core (AppDbContext), repositórios, segurança, serviços
AgendaPics.Web            — ASP.NET Core MVC, Areas (Admin/Usuario), controllers, views
```

### Padrão CQRS

As features ficam em `src/AgendaPics.Application/Features/[Domínio]/Commands/` e `Queries/`. Cada comando/query implementa `IRequest<Result<T>>` e possui um handler correspondente. O mediator customizado é registrado em `DependencyInjection.cs` via `AddCustomMediator()`.

**Fluxo exemplo:** Controller → `IMediator.Send(new LoginCommand(...))` → `LoginCommandHandler` → `Result<LoginResult>`

### Tratamento de Erros

Todos os casos de uso retornam `Result<T>` (de `AgendaPics.Domain.Common`). Os controllers verificam `result.IsSuccess` e tratam os casos de falha sem lançar exceções para erros de negócio.

### Autorização

Autenticação baseada em cookies com claims. Duas políticas definidas em `Program.cs`:
- `"ApenasAdmin"` — exige claim `idPerfil = "1"`
- `"ApenasInstituicao"` — exige claim `idPerfil = "2"`

Rota de login: `/Admin/Login/Login`. Requisições não autenticadas são redirecionadas para lá.

### Segurança

- Senhas são hasheadas com **BCrypt**. Hashes legados em SHA-256 são migrados transparentemente para BCrypt no próximo login (dentro de `LoginCommandHandler`).
- Rate limiting configurado por endpoint em `appsettings.json` em `IpRateLimiting` (5 req/min no login, 3 req/min na recuperação de senha).
- Cookies de sessão: HttpOnly, Secure, SameSite=Strict, timeout de 30 minutos.
- `UseHttpsRedirection` é aplicado apenas fora do ambiente de desenvolvimento (evita `ERR_EMPTY_RESPONSE` no Docker).

### Registros de Infraestrutura

Ver `src/AgendaPics.Infrastructure/DependencyInjection.cs`:
- `IUnitOfWork` → `UnitOfWork` (scoped)
- `IPasswordHasher` → `PasswordHasherBcrypt` (singleton)
- `IEmailService` → `EmailService` via API Mailjet (transient)
- Todos os repositórios registrados como scoped contra suas interfaces de domínio

### Roteamento

Roteamento baseado em Areas:
- `/Admin/...` — gerenciamento de admin e instituições (login, instituições, práticas)
- `/Usuario/...` — agendamentos e atendimentos do usuário final
- `/Home/...` — páginas públicas (index, acesso negado)

## Arquivos Críticos

| Arquivo | Propósito |
|---|---|
| `agenda-pics.sln` | Ponto de entrada da solução |
| `src/AgendaPics.Web/Program.cs` | Startup: CORS, rate limiting, auth, sessão, migrations automáticas, roteamento |
| `src/AgendaPics.Web/appsettings.Development.json` | Config de desenvolvimento (connection string, Mailjet, rate limits) |
| `src/AgendaPics.Infrastructure/Data/AppDbContext.cs` | Modelo EF Core com 10+ entidades |
| `src/AgendaPics.Infrastructure/DependencyInjection.cs` | Registros de IoC para as camadas de infra e aplicação |
| `src/AgendaPics.Application/Common/Mediator/` | Implementação do mediator CQRS customizado |
| `docker-compose.yml` | Orquestração dos containers (app + SQL Server 2022 Express) |
| `.github/workflows/tests.yml` | GitHub Actions: executa os testes unitários em PRs para `master` |
