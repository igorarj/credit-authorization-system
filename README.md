# Sistema de Autorização de Crédito

Este projeto implementa um **sistema simplificado de autorização de crédito** utilizando uma arquitetura baseada em microserviços.  
Ele foi desenvolvido como parte de um **teste técnico** e demonstra conceitos importantes como separação de serviços, autenticação, mensageria e testes unitários.

---

# Arquitetura

O sistema é composto por três serviços principais:

## Auth API

Responsável pela **autenticação** do sistema.

Funções:
- validar credenciais
- gerar **tokens JWT**
- permitir que outras APIs validem requisições autenticadas

---

## Customers API

Responsável pelo **gerenciamento de clientes**.

Funções:
- cadastro e consulta de clientes
- armazenamento do **limite de crédito**
- consumo de eventos de transações aprovadas

---

## Transactions API

Responsável pelo **processamento de transações**.

Fluxo de execução:

1. Recebe uma solicitação de transação
2. Consulta a **Customers API** para obter os dados do cliente
3. Verifica o **limite de crédito disponível**
4. Caso aprovado, registra a transação
5. Publica um evento no **RabbitMQ**

---

# Comunicação entre serviços

A comunicação entre os serviços ocorre de duas formas:

## HTTP REST

Utilizado para chamadas **síncronas** entre APIs.

Exemplo:
Transactions API → Customers API

---

## Mensageria (RabbitMQ)

Utilizada para comunicação **assíncrona baseada em eventos**.

Evento publicado:
`TransactionApprovedEvent
{
  IdCliente
  ValorDebitado
  IdTransacao
}`

Esse evento pode ser consumido por outros serviços, como a **Customers API**, para atualização de dados financeiros.

---

# Autenticação

O sistema utiliza **JWT (JSON Web Token)** para autenticação.

Fluxo:

1. O cliente realiza login na **Auth API**
2. A Auth API gera um **token JWT**
3. O token é utilizado para acessar **Customers API** e **Transactions API**
4. As APIs validam o token utilizando a mesma **chave secreta**

Também existe autenticação **entre serviços**, onde uma API obtém um token na Auth API com usuário de serviço antes de realizar chamadas para outra API.

---

## Camadas da Aplicação

### Domain

Contém:

- entidades
- interfaces de repositório
- regras centrais do domínio

### Application

Contém:

- serviços de aplicação
- regras de negócio

### Infrastructure

Contém implementações de dependências externas:

- repositórios
- mensageria
- clientes HTTP

### API

Responsável por:

- controllers
- endpoints HTTP
- configuração da aplicação

---

## Testes Unitários

Os testes unitários focam nas regras de negócio da camada **Application**.

Dependências externas são simuladas utilizando **Moq**, incluindo:

- repositórios
- clientes HTTP
- publishers de eventos

Frameworks utilizados:

- xUnit
- Moq

- ---

## Tecnologias Utilizadas

- .NET 8
- ASP.NET Core Web API
- JWT Authentication
- RabbitMQ
- xUnit
- Moq
- Swagger / OpenAPI

---

## Observações

Devido ao tempo limitado da avaliação técnica, foram implementados apenas alguns **testes unitários focados nas principais regras de negócio**.

Da mesma forma, foi utilizado um **repositório em memória** para simplificar a execução do projeto e evitar dependências externas durante a avaliação.

A arquitetura foi estruturada de forma que seja simples substituir a persistência em memória por um banco de dados real em um cenário de produção.
