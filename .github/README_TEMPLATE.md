# AcademyIODevops

> Template para adicionar ao README.md principal do repositório

## Status do Projeto

[![CI - Build and Test](https://github.com/[SEU-USUARIO]/AcademyIODevops/workflows/CI%20-%20Build%20and%20Test/badge.svg)](https://github.com/[SEU-USUARIO]/AcademyIODevops/actions/workflows/ci.yml)
[![Test Coverage](https://github.com/[SEU-USUARIO]/AcademyIODevops/workflows/Test%20Coverage/badge.svg)](https://github.com/[SEU-USUARIO]/AcademyIODevops/actions/workflows/test-coverage.yml)
[![Docker Build](https://github.com/[SEU-USUARIO]/AcademyIODevops/workflows/Docker%20Build/badge.svg)](https://github.com/[SEU-USUARIO]/AcademyIODevops/actions/workflows/docker-build.yml)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📊 Métricas

- **Testes Unitários**: 99 testes passando
- **Cobertura de Código**: >70%
- **Microsserviços**: 4 APIs + 1 BFF
- **Build Status**: ✅ Passing

## 🚀 Quick Start

### Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### Clonar e Executar

```bash
# Clone o repositório
git clone https://github.com/[SEU-USUARIO]/AcademyIODevops.git
cd AcademyIODevops

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Ou use Docker Compose
docker-compose up
```

## 🏗️ Arquitetura

```
AcademyIODevops/
├── src/
│   ├── api-gateways/
│   │   └── AcademyIODevops.Bff/              # Backend for Frontend
│   ├── services/                              # Microsserviços
│   │   ├── AcademyIODevops.Auth.API/         # Autenticação
│   │   ├── AcademyIODevops.Courses.API/      # Cursos
│   │   ├── AcademyIODevops.Payments.API/     # Pagamentos
│   │   └── AcademyIODevops.Students.API/     # Estudantes
│   └── building-blocks/                       # Componentes compartilhados
│       ├── AcademyIODevops.Core/
│       ├── AcademyIODevops.MessageBus/
│       └── AcademyIODevops.WebAPI.Core/
├── tests/                                     # Testes unitários
└── .github/workflows/                         # CI/CD
```

### Padrões Arquiteturais

- ✅ **Microsserviços** - Serviços independentes e escaláveis
- ✅ **DDD** - Domain-Driven Design
- ✅ **CQRS** - Command Query Responsibility Segregation
- ✅ **Event-Driven** - Comunicação assíncrona via RabbitMQ
- ✅ **API Gateway** - BFF pattern

## 🛠️ Tecnologias

### Backend
- **.NET 8.0** - Framework principal
- **ASP.NET Core** - Web APIs
- **Entity Framework Core** - ORM
- **MediatR** - Mediator pattern
- **FluentValidation** - Validações
- **RabbitMQ** - Message broker

### Database
- **SQL Server** - Produção
- **SQLite** - Desenvolvimento

### DevOps
- **Docker** - Containerização
- **Docker Compose** - Orquestração local
- **GitHub Actions** - CI/CD

### Testes
- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions
- **Moq** - Mocking
- **Coverlet** - Code coverage

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório HTML
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## 🐳 Docker

```bash
# Build de todos os serviços
docker-compose build

# Executar em modo development
docker-compose up

# Executar em background
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

### Serviços Disponíveis

| Serviço | Porta | URL |
|---------|-------|-----|
| BFF | 5000 | http://localhost:5000 |
| Auth API | 5001 | http://localhost:5001 |
| Courses API | 5002 | http://localhost:5002 |
| Payments API | 5003 | http://localhost:5003 |
| Students API | 5004 | http://localhost:5004 |
| RabbitMQ | 15672 | http://localhost:15672 |
| SQL Server | 1433 | localhost,1433 |

## 📚 Documentação

- [Guia de Arquitetura](docs/architecture.md)
- [Guia de Desenvolvimento](docs/development.md)
- [GitHub Actions Setup](.github/GITHUB_ACTIONS_SETUP.md)
- [Workflows](.github/workflows/README.md)
- [Testes](tests/README.md)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'feat: Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Convenções de Commit

Seguimos [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` - Nova funcionalidade
- `fix:` - Correção de bug
- `docs:` - Documentação
- `test:` - Testes
- `refactor:` - Refatoração
- `chore:` - Tarefas de manutenção

## 📋 Roadmap

- [x] Arquitetura de microsserviços
- [x] Testes unitários (Courses API)
- [x] CI/CD com GitHub Actions
- [ ] Testes de integração
- [ ] Deploy automático
- [ ] Monitoramento e observabilidade
- [ ] Kubernetes deployment

## 📊 CI/CD

Este projeto usa GitHub Actions para:

- ✅ Build automático em push/PR
- ✅ Execução de testes
- ✅ Análise de cobertura
- ✅ Build de imagens Docker
- ✅ Validação de PRs
- ✅ Security scanning

[Ver workflows](.github/workflows/)

## 🐛 Issues e Bugs

Encontrou um bug? [Abra uma issue](https://github.com/[SEU-USUARIO]/AcademyIODevops/issues)

## 📝 Licença

Este projeto está sob a licença MIT. Veja [LICENSE](LICENSE) para mais detalhes.

## 👥 Autores

- **Seu Nome** - [GitHub](https://github.com/[SEU-USUARIO])

## 🙏 Agradecimentos

- Claude Code - Implementação dos testes e CI/CD
- Comunidade .NET
- Contributors

---

**⚡ Desenvolvido com .NET 8.0 e muito ☕**
