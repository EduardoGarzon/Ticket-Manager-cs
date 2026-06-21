# TicketManager

Sistema de gestão de tickets de refeição em C# (.NET 10), com arquitetura em camadas (Domain, Application, Infrastructure, Console), persistência em MySQL via Dapper e testes automatizados com xUnit/Moq.

## Sumário

- [Sobre o projeto](#sobre-o-projeto)
- [Tecnologias utilizadas](#tecnologias-utilizadas)
- [Pré-requisitos](#pré-requisitos)
- [Como baixar o projeto](#como-baixar-o-projeto)
- [Configurando o banco de dados](#configurando-o-banco-de-dados)
- [Como executar o programa](#como-executar-o-programa)
- [Como executar os testes automatizados](#como-executar-os-testes-automatizados)
- [Documentação completa](#documentação-completa)
- [Estrutura do projeto](#estrutura-do-projeto)

## Sobre o projeto

A empresa fictícia utilizada como cenário possui mais de 1000 funcionários e, atualmente, o controle de tickets de refeição é feito por meio de um caderno físico. Este sistema permite:

- Cadastrar e editar funcionários (nome, CPF, situação);
- Registrar e editar a entrega de tickets de refeição;
- Listar funcionários (todos, ativos ou inativos);
- Emitir relatórios de tickets entregues — por funcionário, por período ou completo do sistema.

Uma regra é inegociável em todo o sistema: **nenhum registro pode ser excluído**. Toda desativação é lógica, feita por meio de um campo de situação (`A` para ativo, `I` para inativo).

## Tecnologias utilizadas

- [C#](https://learn.microsoft.com/dotnet/csharp/) / [.NET 10.0.301](https://dotnet.microsoft.com/download)
- [MySQL](https://www.mysql.com/)
- [Dapper](https://github.com/DapperLib/Dapper) (micro-ORM)
- [MySqlConnector](https://mysqlconnector.net/)
- [xUnit](https://xunit.net/) + [Moq](https://github.com/devlooped/moq) (testes automatizados)

## Pré-requisitos

Antes de começar, você vai precisar ter instalado na sua máquina:

- [.NET SDK 10.0.301](https://dotnet.microsoft.com/download) ou superior
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (8.0 ou superior recomendado)
- [Git](https://git-scm.com/downloads)

Opcional, mas recomendado:

- [MySQL Workbench](https://dev.mysql.com/downloads/workbench/), para gerenciar o banco de dados visualmente
- [VS Code](https://code.visualstudio.com/) com a extensão **C# Dev Kit**

## Como baixar o projeto

Clone o repositório e entre na pasta:

```bash
git clone https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git
cd SEU-REPOSITORIO
```

> Substitua a URL acima pela URL real deste repositório.

## Configurando o banco de dados

**1.** Conecte-se ao seu MySQL (via terminal, Workbench, ou outro cliente de sua preferência) e execute o script abaixo para criar o banco de dados e as tabelas:

```sql
CREATE DATABASE ticket_manager_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE ticket_manager_db;

CREATE TABLE funcionarios (
    id              INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nome            VARCHAR(150) NOT NULL,
    cpf             CHAR(11) NOT NULL,
    situacao        CHAR(1) NOT NULL DEFAULT 'A',
    data_alteracao  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                    ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT uq_funcionarios_cpf UNIQUE (cpf),
    CONSTRAINT ck_funcionarios_situacao CHECK (situacao IN ('A', 'I'))
) ENGINE = InnoDB;

CREATE TABLE tickets_entregues (
    id              INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    funcionario_id  INT UNSIGNED NOT NULL,
    quantidade      INT NOT NULL,
    situacao        CHAR(1) NOT NULL DEFAULT 'A',
    data_entrega    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_tickets_funcionario
        FOREIGN KEY (funcionario_id) REFERENCES funcionarios (id),
    CONSTRAINT ck_tickets_situacao CHECK (situacao IN ('A', 'I')),
    CONSTRAINT ck_tickets_quantidade CHECK (quantidade > 0)
) ENGINE = InnoDB;
```

**2.** Configure a string de conexão em `src/TicketManager.Console/Program.cs` com as suas próprias credenciais:

```csharp
var connectionString = "Server=localhost;Port=3306;Database=ticket_manager_db;User=root;Password=SUA_SENHA_AQUI;";
```

> ⚠️ **Atenção:** se este repositório for público, evite commitar uma senha real nesse arquivo. Troque por um valor genérico antes de subir suas alterações ou, melhor ainda, mova a string de conexão para uma variável de ambiente ou um arquivo de configuração incluído no `.gitignore`.

## Como executar o programa

Na pasta raiz do projeto, execute:

```bash
dotnet run --project src/TicketManager.Console
```

Alternativamente, você pode abrir o arquivo `TicketManager.slnx` no Visual Studio ou no VS Code e executar a partir da IDE.

## Como executar os testes automatizados

Os testes das camadas de Domínio e Aplicação estão localizados na pasta `tests/`. Para executá-los, rode na raiz do projeto:

```bash
dotnet test
```

## Documentação completa

A documentação técnica completa do projeto — arquitetura, decisões técnicas, modelagem do banco de dados, casos de uso e próximos passos — está disponível em [`docs/documentacao.pdf`](./docs/documentacao.pdf).

> Ajuste o caminho acima conforme o local onde o PDF compilado for salvo no repositório.

## Estrutura do projeto

```
TicketManager/
├── src/
│   ├── TicketManager.Domain/          # Entidades e regras de negócio (não depende de nada)
│   ├── TicketManager.Application/     # Casos de uso (depende só do Domain)
│   ├── TicketManager.Infrastructure/  # Acesso ao MySQL (depende só do Domain)
│   └── TicketManager.Console/         # Interface de console (depende de Application e Infrastructure)
└── tests/
    ├── TicketManager.Domain.Tests/
    └── TicketManager.Application.Tests/
```
