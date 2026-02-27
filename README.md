# Agenda PICS

O **Agenda PICS** é um sistema web para gerenciamento de consultas vinculadas às Práticas Integrativas e Complementares em Saúde (PICS). Facilita o agendamento, acompanhamento e administração dos atendimentos, organizando fluxos e otimizando a gestão dos serviços ofertados.

## Sumário

- [Funcionalidades](#funcionalidades)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Como Executar](#como-executar)
  - [Com Docker Compose (recomendado)](#com-docker-compose-recomendado)
  - [Localmente](#localmente)
- [Testes](#testes)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Contribuindo](#contribuindo)

---

## Funcionalidades

- Cadastro e gerenciamento de instituições e práticas
- Agendamento e cancelamento de consultas
- Controle de acesso por perfil (administrador e instituição)
- Notificações por e-mail via API Mailjet
- Interface responsiva

## Arquitetura

O projeto segue uma arquitetura limpa em 4 camadas:

```
AgendaPics.Domain         — Entidades, enums, interfaces de domínio, padrão Result<T>
AgendaPics.Application    — Features CQRS, mediator customizado, FluentValidation
AgendaPics.Infrastructure — EF Core, repositórios, segurança (BCrypt), serviços
AgendaPics.Web            — ASP.NET Core MVC, Areas (Admin/Usuario), controllers, views
```

O padrão CQRS é utilizado via mediator customizado. Cada caso de uso possui um `Command` ou `Query` com seu respectivo `Handler`, e retorna `Result<T>` — sem exceções para falhas de negócio.

## Tecnologias

- **Backend**: C# / .NET 8 / ASP.NET Core MVC
- **Banco de Dados**: SQL Server 2022 (EF Core 8 — code-first com migrations)
- **Segurança**: BCrypt, cookie auth, rate limiting, CSRF
- **E-mail**: Mailjet API
- **Infraestrutura**: Docker, Docker Compose
- **Testes**: xUnit, Moq, FluentAssertions
- **CI**: GitHub Actions

## Como Executar

### Com Docker Compose (recomendado)

Pré-requisitos: [Docker Desktop](https://www.docker.com/products/docker-desktop)

```bash
git clone https://github.com/fabiojosee/agenda-pics.git
cd agenda-pics
docker compose up -d --build
```

O banco de dados é criado e populado **automaticamente** na primeira inicialização via EF Core migrations. O processo aguarda o SQL Server ficar disponível (pode levar até 90 segundos na primeira vez).

Acesse: **http://localhost:32033**

Login padrão: `admin` / `123`

> Se o container da aplicação reiniciar antes do SQL Server estar pronto, execute:
> ```bash
> docker compose restart agendapics
> ```
> Acompanhe os logs com `docker logs agendapicsweb -f`

---

### Localmente

Pré-requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download) e uma instância SQL Server acessível.

**1. Clone o repositório**

```bash
git clone https://github.com/fabiojosee/agenda-pics.git
cd agenda-pics
```

**2. Configure a string de conexão**

Edite `src/AgendaPics.Web/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=dbAgendaPics;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True;"
}
```

**3. Execute**

```bash
cd src/AgendaPics.Web && dotnet run
```

O banco será criado e populado automaticamente ao subir a aplicação.

Acesse: **https://localhost:5001** (ou a porta exibida no terminal)

Login padrão: `admin` / `123`

> Altere a senha padrão após o primeiro acesso.

## Testes

O projeto possui uma suíte de testes unitários cobrindo os 26 handlers da camada Application (120 testes). Os testes são completamente isolados — sem dependências de banco de dados ou serviços externos.

```bash
dotnet test tests/AgendaPics.Application.Tests/AgendaPics.Application.Tests.csproj --configuration Release --verbosity normal
```

Os testes são executados automaticamente pelo GitHub Actions em todo Pull Request direcionado à branch `master`.

## Estrutura de Pastas

```
src/
├── AgendaPics.Domain/          # Entidades, enums, interfaces, Result<T>
├── AgendaPics.Application/     # CQRS: features, commands, queries, handlers
├── AgendaPics.Infrastructure/  # EF Core, repositórios, segurança, serviços, migrations
└── AgendaPics.Web/             # MVC: controllers, views, areas, program.cs
    ├── Areas/Admin/            # Login, instituições, práticas
    └── Areas/Usuario/          # Agendamentos e atendimentos
tests/
└── AgendaPics.Application.Tests/  # Testes unitários (xUnit + Moq + FluentAssertions)
```

## Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b minha-feature`)
3. Commit suas alterações (`git commit -m 'feat: descrição da mudança'`)
4. Push para a branch (`git push origin minha-feature`)
5. Abra um Pull Request

---

> Para dúvidas ou problemas, abra uma [issue](https://github.com/fabiojosee/agenda-pics/issues).
