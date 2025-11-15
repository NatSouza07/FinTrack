#FinTrack - Your finances. Always on track.

#Sistema de Controle Financeiro Pessoal

Gerencie suas finanças pessoais de forma prática e segura com o FinTrack, um sistema web desenvolvido em C# / ASP.NET Core MVC / Entity Framework Core / SQL Server.
Crie contas, cadastre receitas e despesas, defina metas mensais, visualize relatórios e acompanhe gráficos de desempenho financeiro em tempo real.

----------------------------------------
#ÍNDICE
- Objetivo do Sistema
- Funcionalidades
- Estrutura do Banco de Dados
- Autenticação e Segurança
- Tecnologias Utilizadas
- Estrutura do Projeto
- Instalação e Execução
- Consultas e Relatórios
- Autores

----------------------------------------
#OBJETIVO DO SISTEMA

O FinTrack é um sistema web de gestão financeira pessoal criado como projeto acadêmico.
Permite ao usuário:
- Controlar entradas e saídas
- Organizar categorias e contas
- Definir metas financeiras
- Gerar relatórios com totalizações e gráficos
- Visualizar sua situação financeira de forma clara e dinâmica

----------------------------------------
#FUNCIONALIDADES

- Transações: CRUD completo com cálculos automáticos
- Contas: cadastro com saldo inicial
- Categorias: Entrada / Saída
- Tipos de Pagamento: PIX, cartão etc.
- Metas: metas mensais com progresso
- Relatórios: agrupamento, totalização, pivot
- Autenticação: login, registro, recuperação de senha
- Autorização: roles Admin/User
- Interface responsiva com Bootstrap 5
- Filtros dinâmicos
- Seed automático de dados

----------------------------------------
#ESTRUTURA DO BANCO DE DADOS

Tabelas principais:
- Usuario
- Conta
- Categoria
- TipoPagamento
- Transacao
- Meta

----------------------------------------
#AUTENTICAÇÃO E SEGURANÇA

- ASP.NET Core Identity (Scaffold)
- Políticas de senha configuradas
- Perfis Admin / User
- Recuperação de senha
- Seed automático de usuário administrador

----------------------------------------
#TECNOLOGIAS UTILIZADAS

- C# (.NET 8)
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap 5
- Chart.js
- jQuery Validate
- Visual Studio 2022
- GitHub

----------------------------------------
#ESTRUTURA DO PROJETO

FinTrack/
 Controllers/
 Models/
 Data/
 Services/
 Views/
 wwwroot/

----------------------------------------
#INSTALAÇÃO E EXECUÇÃO

Pré-requisitos:
- Visual Studio 2022
- SQL Server LocalDB

Passos:
- git clone https://github.com/NatSouza07/FinTrack.git
- cd FinTrack
- dotnet restore
- dotnet ef database update
- dotnet run

Acesse: https://localhost:5001

----------------------------------------
#CONSULTAS E RELATÓRIOS

Inclui:
- Totais de entradas/saídas
- Agrupamento por categoria
- Pivot (categoria x mês)
- Gráficos com Chart.js

----------------------------------------
#AUTORES

- Natã — Responsável Técnico / Backend
- Fábio — Banco / Relatórios
- Gustavo — Front-End

----------------------------------------

"Your finances. Always on track."

FinTrack.
