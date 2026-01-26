# Controle Financeiro – SaaS

Sistema de controle financeiro pessoal desenvolvido no modelo **SaaS (Software as a Service)**, com foco em organização, clareza e confiabilidade das informações financeiras do usuário.

O projeto foi concebido para evoluir de forma incremental, iniciando com uma aplicação web e preparado para futuras extensões como aplicativo mobile e integração conversacional via WhatsApp.

---

## Visão Geral

O sistema permite que usuários gerenciem sua vida financeira de forma centralizada, incluindo:

- Registro de receitas e despesas
- Controle de lançamentos futuros, recorrentes e parcelados
- Organização por contas e categorias
- Conciliação de lançamentos com extratos bancários
- Acompanhamento de orçamento
- Relatórios financeiros e exportação de dados
- Integração futura via WhatsApp para lançamentos e consultas

---

## Arquitetura

O backend foi desenvolvido em **.NET Core / C#**, utilizando princípios de **Clean Architecture**, garantindo:

- Separação clara de responsabilidades
- Baixo acoplamento
- Facilidade de manutenção e evolução
- Preparação para crescimento do domínio financeiro

### Estrutura da Solution

---

## Tecnologias Utilizadas

- .NET 7/8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger (OpenAPI)
- Clean Architecture

---

## Funcionalidades Implementadas (até o momento)

- Estrutura base do projeto
- Cadastro de usuários
- Persistência com Entity Framework Core
- Hash seguro de senha
- API documentada via Swagger

---

## Funcionalidades em Desenvolvimento

- Login com autenticação JWT
- Recuperação de senha
- Proteção de endpoints
- Gestão financeira (contas, lançamentos, orçamento)
- Relatórios
- Integração com WhatsApp
- Frontend Web
- Aplicativo Mobile

---

## Execução do Projeto

### Pré-requisitos
- .NET SDK 7 ou superior
- SQL Server

### Rodar a aplicação
```bash
dotnet run --project src/ControleFinanceiro.API

