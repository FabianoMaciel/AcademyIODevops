# GitHub Actions Workflows - AcademyIODevops

Este diretório contém os workflows de CI/CD para o projeto AcademyIODevops.

## 📋 Workflows Disponíveis

### 1. 🔄 CI - Build and Test (`ci.yml`)

**Trigger:** Push e Pull Request para `master`, `main`, `develop`

**Funções:**
- ✅ Build da solution completa
- ✅ Execução de testes unitários
- ✅ Build em múltiplos sistemas operacionais (Ubuntu e Windows)
- ✅ Análise de código
- ✅ Publicação de resultados de testes

**Jobs:**
- `build-and-test` - Build e testes principais
- `build-matrix` - Build em múltiplos OS
- `analyze` - Análise de código
- `status-check` - Verificação final de status

**Badges:**
```markdown
![CI](https://github.com/[username]/AcademyIODevops/workflows/CI%20-%20Build%20and%20Test/badge.svg)
```

---

### 2. 📊 Test Coverage (`test-coverage.yml`)

**Trigger:** Push e Pull Request para branches principais

**Funções:**
- ✅ Geração de relatórios de cobertura
- ✅ Validação de threshold mínimo (70%)
- ✅ Upload de artefatos
- ✅ Comentário automático em PRs
- ✅ Cobertura por projeto

**Configuração:**
```yaml
env:
  COVERAGE_THRESHOLD: 70  # Ajuste conforme necessário
```

**Jobs:**
- `test-coverage` - Cobertura geral
- `coverage-by-project` - Cobertura individual
- `coverage-badge` - Atualização de badge

**Artefatos Gerados:**
- `coverage-report` (30 dias)
- `coverage-[projeto]` (7 dias)

---

### 3. 🐳 Docker Build (`docker-build.yml`)

**Trigger:**
- Push/PR quando arquivos Docker são modificados
- Manual via `workflow_dispatch`

**Funções:**
- ✅ Build de todas as imagens Docker
- ✅ Push para GitHub Container Registry
- ✅ Validação de docker-compose
- ✅ Security scan com Trivy
- ✅ Cache de layers para build mais rápido

**Serviços:**
- `bff` - API Gateway
- `auth-api` - Autenticação
- `courses-api` - Cursos
- `payments-api` - Pagamentos
- `students-api` - Estudantes

**Registry:**
```
ghcr.io/[username]/academyiodevops-[service]:latest
```

---

### 4. ✅ PR Validation (`pr-validation.yml`)

**Trigger:** Pull Requests (opened, synchronize, reopened)

**Funções:**
- ✅ Informações do PR
- ✅ Análise de qualidade de código
- ✅ Análise de arquivos modificados
- ✅ Verificação de testes
- ✅ Build em múltiplas configurações
- ✅ Auto-labeling
- ✅ Comentário automático no PR

**Verificações:**
- Número de warnings
- Tamanho do PR
- Arquivos de teste incluídos
- Cobertura de código
- Build Debug e Release

**Labels Automáticos:**
- `size/xs`, `size/s`, `size/m`, `size/l`, `size/xl`
- Labels baseados em arquivos modificados (ver `.github/labeler.yml`)

---

## 🚀 Como Usar

### Executar Workflow Manualmente

1. Vá para "Actions" no GitHub
2. Selecione o workflow desejado
3. Clique em "Run workflow"
4. Escolha a branch
5. Clique em "Run workflow"

### Configurar Secrets

Alguns workflows podem precisar de secrets configurados:

```bash
# GitHub Container Registry (automático com GITHUB_TOKEN)
# Nenhuma configuração adicional necessária para push de imagens
```

### Branch Protection

Recomenda-se configurar as seguintes proteções para `master`/`main`:

```yaml
# Configuração sugerida
Require status checks to pass:
  - build-and-test
  - test-coverage
  - docker-build (opcional)
  - pr-validation-summary

Require branches to be up to date: ✅
Require review before merging: ✅ (pelo menos 1)
```

---

## 📊 Status Badges

Adicione ao seu README.md:

```markdown
![CI](https://github.com/[username]/AcademyIODevops/workflows/CI%20-%20Build%20and%20Test/badge.svg)
![Coverage](https://github.com/[username]/AcademyIODevops/workflows/Test%20Coverage/badge.svg)
![Docker](https://github.com/[username]/AcademyIODevops/workflows/Docker%20Build/badge.svg)
```

---

## 🔧 Customização

### Alterar Threshold de Cobertura

Em `test-coverage.yml`:
```yaml
env:
  COVERAGE_THRESHOLD: 70  # Aumente para 80, 90, etc.
```

### Adicionar Novo Serviço

Em `docker-build.yml`:
```yaml
strategy:
  matrix:
    service:
      - name: novo-servico
        path: src/services/AcademyIODevops.NovoServico.API
        dockerfile: src/services/AcademyIODevops.NovoServico.API/Dockerfile
```

### Adicionar Novo Projeto de Teste

Em `test-coverage.yml`:
```yaml
strategy:
  matrix:
    project:
      - 'AcademyIODevops.Courses.API.Tests'
      - 'AcademyIODevops.NovoServico.API.Tests'  # Adicione aqui
```

---

## 📈 Monitoramento

### Ver Logs

1. Vá para "Actions" no GitHub
2. Clique no workflow desejado
3. Clique no run específico
4. Clique no job para ver logs detalhados

### Baixar Artefatos

1. Vá para "Actions" > Workflow run
2. Scroll até "Artifacts"
3. Clique para baixar:
   - `coverage-report` - Relatório de cobertura HTML
   - `test-results` - Resultados de testes

### Visualizar Cobertura

1. Baixe o artefato `coverage-report`
2. Extraia o arquivo
3. Abra `index.html` no navegador

---

## 🐛 Troubleshooting

### Build Falha com "Restore failed"

```bash
# Limpe o cache e tente novamente
dotnet nuget locals all --clear
dotnet restore
```

### Docker Build Falha

```bash
# Verifique o Dockerfile localmente
docker build -f src/services/[Service]/Dockerfile .

# Valide docker-compose
docker-compose config
```

### Testes Falhando

```bash
# Execute localmente primeiro
dotnet test --verbosity detailed

# Limpe e reconstrua
dotnet clean
dotnet build
dotnet test
```

### Coverage Threshold Failed

Duas opções:
1. Adicione mais testes para aumentar cobertura
2. Ajuste o threshold em `test-coverage.yml`

---

## 🔒 Segurança

### Trivy Security Scanning

O workflow Docker inclui scan de segurança com Trivy:
- Detecta vulnerabilidades CRITICAL e HIGH
- Envia resultados para GitHub Security
- Executado apenas em pushes para main/master

### Secrets Management

- Nunca commite secrets no código
- Use GitHub Secrets para valores sensíveis
- GITHUB_TOKEN é automaticamente disponibilizado

---

## 📚 Referências

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [ReportGenerator GitHub Action](https://github.com/danielpalme/ReportGenerator)
- [Test Reporter](https://github.com/dorny/test-reporter)

---

## 🤝 Contribuindo

Ao adicionar novos workflows:

1. Teste localmente quando possível
2. Use nomes descritivos
3. Adicione documentação
4. Use emojis nos step names para melhor visualização
5. Adicione summaries para output claro

---

## 📝 Changelog

### 2025-12-19
- ✅ Workflow CI principal implementado
- ✅ Workflow de cobertura de código
- ✅ Workflow de Docker build
- ✅ Workflow de validação de PR
- ✅ Auto-labeling configurado
- ✅ Documentação completa

---

**Última atualização:** 2025-12-19
**Status:** ✅ Pronto para uso
**Autor:** Claude Code
