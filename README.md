# Agenda PICS

O **Agenda PICS** é um aplicativo desenvolvido para gerenciar consultas vinculadas às Práticas Integrativas e Complementares em Saúde (PICS). Este sistema facilita o agendamento, acompanhamento e administração dos atendimentos, visando organizar fluxos, otimizar a gestão e garantir a qualidade dos serviços ofertados.

## Sumário

- [Visão Geral](#visão-geral)
- [Funcionalidades Principais](#funcionalidades-principais)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Instalação e Execução](#instalação-e-execução)
- [Configuração do Banco de Dados](#configuração-do-banco-de-dados)
- [Executando com Docker Compose](#executando-com-docker-compose)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Contribuindo](#contribuindo)

---

## Visão Geral

O sistema Agenda PICS foi desenvolvido para facilitar a gestão das práticas integrativas em ambientes de saúde, permitindo um controle eficiente de pacientes e agendamentos. O aplicativo oferece uma interface intuitiva e mecanismos de controle de acesso que auxiliam a administração e a tomada de decisões.

## Funcionalidades Principais

- **Cadastro de Instituições**: Adição, edição e exclusão de instituições.
- **Cadastro de Práticas**: Adição, edição e exclusão de práticas.
- **Agendamento de Consultas**: Agendamento e cancelamento de consultas.
- **Controle de Usuários**: Gerenciamento de acesso e permissões.
- **Interface Responsiva**: Utilização facilitada em diferentes dispositivos.
- **Notificação por e-mail**: Envio automático de e-mails para notificar novos agendamentos.

## Arquitetura do Projeto

O projeto é uma aplicação web desenvolvida com o padrão MVC (Model-View-Controller), utilizando C# (.NET Core) no backend e HTML, CSS e JavaScript no frontend.

### Camadas Principais

- **Model**: Representa as entidades do sistema (Agendamento, Pratica, Instituicao, etc.).
- **View**: Telas e páginas HTML/CSS utilizadas pelos usuários.
- **Controller**: Lógica de negócio, manipulação dos dados e regras do sistema.

## Tecnologias Utilizadas

- **Backend**: C# (.NET Core)
- **Frontend**: HTML, CSS, JavaScript, C#
- **Banco de Dados**: SQL Server
- **Outros**: jQuery, Docker, Docker Compose

## Instalação e Execução

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/fabiojosee/gerenciadorConsultasPICS.git
   ```

2. **Abra o projeto no Visual Studio (ou editor compatível)**

3. **Configure o banco de dados** (veja instruções detalhadas abaixo).

4. **Edite a string de conexão** no arquivo de configuração (`appsettings.Development.json` ou `appsettings.Production.json`) para refletir as informações do seu servidor e banco de dados.

5. **Execute a aplicação**:
   - No Visual Studio, pressione F5 ou execute o comando:
     ```bash
     dotnet run
     ```
   - Acesse `http://localhost:xxxx` no navegador.

## Configuração do Banco de Dados

> **Atenção:** O projeto ainda não utiliza migrations automáticas. Portanto, você deve criar o banco de dados e suas tabelas manualmente, antes de rodar a aplicação.

### 1. Crie o Banco de Dados e as Tabelas

No seu SGBD (por exemplo, SQL Server Management Studio), execute o script localizado em "\Data\Scripts\create_database.sql" para criar o banco de dados e as tabelas do sistema.

### 2. Adicione os Registros Inicias

Execute o script localizado em "\Data\Scripts\init_insert.sql" para inserir registros iniciais nas tabelas do sistema, como os estados brasileiros e algumas de suas cidades (esses dados podem ser ajustados conforme necessário). O script também cria um usuário administrador padrão com login "admin" e senha "123".

> **Importante:** Altere a senha padrão após os testes iniciais. A senha é armazenada criptografada utilizando o algoritmo SHA-256 para garantir segurança.

### 3. Configure a String de Conexão

No arquivo `appsettings.Development.json` ou `appsettings.Production.json`, configure a string de conexão para apontar ao banco recém-criado. Exemplo para SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=dbAgendaPics;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True;
}
```

## Executando com Docker Compose

Se preferir, você pode executar o projeto utilizando **Docker Compose**. Isso facilita a configuração do ambiente, subindo automaticamente a aplicação e o banco de dados.

1. Certifique-se de ter o [Docker](https://www.docker.com/) e o [Docker Compose](https://docs.docker.com/compose/) instalados em sua máquina.

2. Na raiz do projeto, execute o comando abaixo para subir todos os serviços necessários:
   ```bash
   docker compose up -d
   ```

3. O sistema estará disponível em `http://localhost:xxxx` (ajuste a porta conforme configuração do seu `docker-compose.yml`).

> **Observação:**  
> Caso esteja usando Docker Compose para o banco de dados, lembre-se de que, na primeira execução, será necessário acessar o container do banco e criar manualmente as tabelas e usuários iniciais, conforme orientações acima.

## Estrutura de Pastas

```
/Areas            # Divide as áreas principais do sistema: usuário e administrador
/Controllers      # Lógica de negócio e roteamento
/Models           # Definição das entidades
/Views            # Páginas HTML
/wwwroot          # Arquivos estáticos (CSS, JS, imagens)
/Data             # Contexto de banco de dados
/Repositories     # Acesso e manipulação de dados
/Services         # Serviços utilizados para gestão (e-mail, segurança)
```

## Contribuindo

Contribuições são bem-vindas! Siga os passos abaixo:

1. Faça um fork do projeto.
2. Crie uma branch para sua feature (`git checkout -b minha-feature`)
3. Commit suas alterações (`git commit -m 'Adiciona nova feature'`)
4. Faça push para o branch (`git push origin minha-feature`)
5. Abra um Pull Request.

---

> Para dúvidas, sugestões ou problemas, abra uma [issue](https://github.com/fabiojosee/gerenciadorConsultasPICS/issues).