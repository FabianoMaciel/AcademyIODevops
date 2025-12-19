# Comandos Úteis - GitHub Actions

Referência rápida de comandos para trabalhar com GitHub Actions.

## 🚀 Comandos Git

### Commit dos Workflows

```bash
# Adicionar todos os arquivos do GitHub Actions
git add .github/

# Commit com mensagem convencional
git commit -m "ci: add GitHub Actions workflows for CI/CD

- Add CI workflow with build and tests
- Add test coverage workflow with reports
- Add Docker build workflow with security scan
- Add PR validation workflow with auto-labeling
- Add comprehensive documentation"

# Push para remote
git push origin master
```

### Verificar Status

```bash
# Ver arquivos modificados
git status

# Ver diff dos arquivos
git diff .github/

# Ver log de commits
git log --oneline -5
```

---

## 🔧 Comandos .NET

### Build e Testes

```bash
# Restaurar dependências
dotnet restore AcademyIODevops.sln

# Build da solution
dotnet build AcademyIODevops.sln --configuration Release

# Executar testes
dotnet test AcademyIODevops.sln --verbosity normal

# Testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

### Cobertura de Código

```bash
# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Instalar ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Gerar relatório HTML
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Abrir relatório
start coveragereport/index.html  # Windows
open coveragereport/index.html   # macOS
xdg-open coveragereport/index.html  # Linux
```

### Limpar Build

```bash
# Limpar outputs
dotnet clean

# Limpar cache de NuGet
dotnet nuget locals all --clear

# Rebuild completo
dotnet clean && dotnet restore && dotnet build
```

---

## 🐳 Comandos Docker

### Docker Compose

```bash
# Build de todos os serviços
docker-compose build

# Executar serviços
docker-compose up

# Executar em background
docker-compose up -d

# Ver logs
docker-compose logs -f

# Ver logs de um serviço específico
docker-compose logs -f courses-api

# Parar serviços
docker-compose stop

# Parar e remover containers
docker-compose down

# Parar e remover volumes
docker-compose down -v
```

### Docker Build Manual

```bash
# Build de uma imagem específica
docker build -t academyiodevops-courses-api -f src/services/AcademyIODevops.Courses.API/Dockerfile .

# Executar container
docker run -p 5002:80 academyiodevops-courses-api

# Ver imagens
docker images

# Ver containers em execução
docker ps

# Ver todos os containers
docker ps -a

# Remover container
docker rm [container-id]

# Remover imagem
docker rmi [image-id]

# Limpar recursos não utilizados
docker system prune -a
```

### Docker Registry (GitHub Container Registry)

```bash
# Login no GHCR
echo $CR_PAT | docker login ghcr.io -u USERNAME --password-stdin

# Tag da imagem
docker tag academyiodevops-courses-api ghcr.io/username/academyiodevops-courses-api:latest

# Push da imagem
docker push ghcr.io/username/academyiodevops-courses-api:latest

# Pull da imagem
docker pull ghcr.io/username/academyiodevops-courses-api:latest
```

---

## 🔍 Comandos GitHub CLI

### Instalação

```bash
# Windows (Chocolatey)
choco install gh

# macOS
brew install gh

# Linux
sudo apt install gh
```

### Workflows

```bash
# Listar workflows
gh workflow list

# Ver runs de um workflow
gh run list --workflow=ci.yml

# Ver detalhes de um run
gh run view [run-id]

# Ver logs de um run
gh run view [run-id] --log

# Baixar artefatos
gh run download [run-id]

# Executar workflow manualmente
gh workflow run ci.yml

# Ver status de um run
gh run watch
```

### Pull Requests

```bash
# Criar PR
gh pr create --title "Título" --body "Descrição"

# Listar PRs
gh pr list

# Ver detalhes de um PR
gh pr view [pr-number]

# Ver checks de um PR
gh pr checks [pr-number]

# Merge de PR
gh pr merge [pr-number]

# Comentar em PR
gh pr comment [pr-number] --body "Comentário"
```

### Issues

```bash
# Criar issue
gh issue create --title "Bug encontrado" --body "Descrição"

# Listar issues
gh issue list

# Ver issue
gh issue view [issue-number]

# Fechar issue
gh issue close [issue-number]
```

### Releases

```bash
# Criar release
gh release create v1.0.0 --title "v1.0.0" --notes "Release notes"

# Listar releases
gh release list

# Ver release
gh release view v1.0.0

# Download de assets
gh release download v1.0.0
```

---

## 📊 Comandos de Monitoramento

### GitHub Actions

```bash
# Ver status de todos os workflows
gh run list --limit 10

# Ver runs falhados
gh run list --status failure

# Ver runs em progresso
gh run list --status in_progress

# Ver runs por branch
gh run list --branch master

# Re-executar run falhado
gh run rerun [run-id]

# Cancelar run
gh run cancel [run-id]
```

### Métricas de Build

```bash
# Ver tempo médio de build (usando jq)
gh run list --json durationMs --limit 100 | jq '[.[].durationMs] | add/length/1000/60'

# Ver taxa de sucesso
gh run list --json conclusion --limit 100 | jq '[.[] | select(.conclusion=="success")] | length'
```

---

## 🛠️ Comandos de Desenvolvimento

### Branch Management

```bash
# Criar nova branch
git checkout -b feature/nova-funcionalidade

# Listar branches
git branch

# Mudar de branch
git checkout master

# Atualizar branch
git pull origin master

# Merge de branch
git merge feature/nova-funcionalidade

# Deletar branch local
git branch -d feature/nova-funcionalidade

# Deletar branch remota
git push origin --delete feature/nova-funcionalidade
```

### Stash

```bash
# Salvar mudanças temporariamente
git stash

# Ver stashes
git stash list

# Aplicar último stash
git stash pop

# Aplicar stash específico
git stash apply stash@{0}

# Limpar stashes
git stash clear
```

### Rebase

```bash
# Rebase interativo (últimos 3 commits)
git rebase -i HEAD~3

# Rebase com master
git rebase master

# Continuar rebase após resolver conflitos
git rebase --continue

# Abortar rebase
git rebase --abort
```

---

## 🔐 Comandos de Segurança

### Secrets Management

```bash
# Listar secrets (via gh)
gh secret list

# Adicionar secret
gh secret set DOCKER_USERNAME

# Remover secret
gh secret remove DOCKER_USERNAME
```

### Security Scanning

```bash
# Trivy local scan
trivy image academyiodevops-courses-api:latest

# Scan com severidade específica
trivy image --severity HIGH,CRITICAL academyiodevops-courses-api:latest

# Scan de filesystem
trivy fs ./src/services/AcademyIODevops.Courses.API/
```

---

## 📝 Comandos de Documentação

### Gerar Documentação

```bash
# Gerar docs com Docfx (se configurado)
docfx build

# Servir docs localmente
docfx serve _site
```

### Markdown

```bash
# Preview de Markdown (VS Code)
code --preview README.md

# Lint de Markdown
markdownlint **/*.md
```

---

## 🧪 Comandos de Teste Avançados

### Filtros

```bash
# Executar testes de uma classe específica
dotnet test --filter "FullyQualifiedName~CourseTests"

# Executar testes com nome específico
dotnet test --filter "Name~ShouldAddCourse"

# Executar testes por categoria (se usar [Trait])
dotnet test --filter "Category=Integration"

# Executar testes excluindo alguns
dotnet test --filter "FullyQualifiedName!~Integration"
```

### Performance

```bash
# Executar testes em paralelo
dotnet test --parallel

# Executar com timeout
dotnet test --blame-hang-timeout 5m

# Executar com logger específico
dotnet test --logger "console;verbosity=detailed"
```

### Debug

```bash
# Executar testes com debug
dotnet test --logger "console;verbosity=diagnostic"

# Executar testes de um projeto específico
dotnet test tests/AcademyIODevops.Courses.API.Tests/AcademyIODevops.Courses.API.Tests.csproj
```

---

## 🎯 Scripts Úteis

### Build Completo

```bash
# Windows (PowerShell)
$ErrorActionPreference = "Stop"
dotnet clean
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --no-build

# Linux/macOS (Bash)
#!/bin/bash
set -e
dotnet clean
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --no-build
```

### CI Simulation Local

```bash
# Simular CI localmente (requer act)
act -j build-and-test

# Simular com secrets
act -j build-and-test -s GITHUB_TOKEN=your-token

# Dry run
act -n
```

### Limpeza Completa

```bash
#!/bin/bash
# Limpar tudo e reconstruir
echo "Limpando build artifacts..."
dotnet clean

echo "Limpando cache NuGet..."
dotnet nuget locals all --clear

echo "Removendo pastas bin e obj..."
find . -type d -name "bin" -o -name "obj" | xargs rm -rf

echo "Restaurando dependências..."
dotnet restore

echo "Build..."
dotnet build

echo "Testes..."
dotnet test

echo "Concluído!"
```

---

## 📚 Aliases Úteis (Git Bash / ZSH)

Adicione ao seu `.bashrc` ou `.zshrc`:

```bash
# Git
alias gs='git status'
alias ga='git add'
alias gc='git commit'
alias gp='git push'
alias gl='git log --oneline -10'

# .NET
alias dr='dotnet restore'
alias db='dotnet build'
alias dt='dotnet test'
alias dc='dotnet clean'

# Docker
alias dcu='docker-compose up'
alias dcd='docker-compose down'
alias dcb='docker-compose build'
alias dcl='docker-compose logs -f'

# GitHub CLI
alias ghw='gh workflow list'
alias ghr='gh run list'
alias ghp='gh pr list'
```

---

**Última atualização:** 2025-12-19
**Dica:** Use `Ctrl+F` para buscar comando específico
