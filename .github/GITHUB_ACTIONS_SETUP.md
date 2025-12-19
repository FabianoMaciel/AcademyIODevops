# GitHub Actions - Guia de Configuração Rápida

Este guia mostra como configurar e usar os workflows do GitHub Actions no projeto AcademyIODevops.

## 🚀 Quick Start

### 1. Primeiro Commit

Após fazer commit dos workflows:

```bash
git add .github/
git commit -m "ci: add GitHub Actions workflows"
git push origin master
```

### 2. Verificar Execução

1. Vá para https://github.com/[seu-usuario]/AcademyIODevops/actions
2. Você verá os workflows em execução
3. Aguarde a conclusão (aproximadamente 3-5 minutos)

### 3. Visualizar Resultados

- ✅ **CI Build** - Status do build e testes
- 📊 **Coverage** - Relatório de cobertura baixável
- 🐳 **Docker** - Imagens construídas com sucesso
- ✅ **PR Validation** - Validação automática de PRs

---

## 📋 Checklist Pós-Instalação

### Configuração Inicial

- [x] Workflows commitados
- [ ] Primeiro workflow executado com sucesso
- [ ] Artefatos de cobertura gerados
- [ ] Badges adicionados ao README

### Configuração de Branch Protection (Recomendado)

1. Vá para **Settings** > **Branches**
2. Clique em **Add rule**
3. Branch name pattern: `master` (ou `main`)
4. Marque:
   - ✅ **Require status checks to pass before merging**
   - ✅ **Require branches to be up to date before merging**
5. Selecione os checks obrigatórios:
   - `build-and-test`
   - `test-coverage`
   - `docker-compose-test`
6. Marque:
   - ✅ **Require a pull request before merging**
   - Number of approvals: `1`
7. Clique em **Create**

### Configuração de Secrets (Se Necessário)

Para deploy ou integrações externas:

```bash
# Exemplo: DockerHub (se não usar ghcr.io)
DOCKERHUB_USERNAME=seu-usuario
DOCKERHUB_TOKEN=seu-token

# Exemplo: SonarCloud (se quiser análise de código)
SONAR_TOKEN=seu-token
```

Adicione em: **Settings** > **Secrets and variables** > **Actions** > **New repository secret**

---

## 🎯 Cenários de Uso

### Cenário 1: Desenvolvimento Normal

```bash
# 1. Criar branch
git checkout -b feature/nova-funcionalidade

# 2. Fazer alterações
# ... código ...

# 3. Commit e push
git add .
git commit -m "feat: adiciona nova funcionalidade"
git push origin feature/nova-funcionalidade

# 4. Criar PR no GitHub
# Os workflows de PR validation serão executados automaticamente
```

### Cenário 2: Validar PR Antes de Merge

Quando um PR é criado:

1. ✅ **PR Validation** executa automaticamente
2. ✅ Verifica build, testes e cobertura
3. ✅ Adiciona labels automaticamente
4. ✅ Comenta no PR com status
5. ✅ Bloqueia merge se falhar (com branch protection)

### Cenário 3: Deploy de Imagens Docker

Quando merge para `master`/`main`:

1. ✅ CI executa e passa
2. ✅ Docker Build cria imagens
3. ✅ Imagens são enviadas para ghcr.io
4. ✅ Security scan é executado

### Cenário 4: Verificar Cobertura

```bash
# 1. Execute localmente
dotnet test --collect:"XPlat Code Coverage"

# 2. Ou baixe do GitHub:
# Actions > Test Coverage > Latest run > Artifacts > coverage-report

# 3. Extraia e abra index.html
```

---

## 📊 Dashboard e Monitoramento

### Status dos Workflows

Visualize em: **Actions** tab no GitHub

```
✅ CI - Build and Test (2m 34s)
✅ Test Coverage (3m 12s)
✅ Docker Build (5m 45s)
```

### Métricas Importantes

| Métrica | Onde Ver | Objetivo |
|---------|----------|----------|
| Build Time | CI workflow | < 5 min |
| Test Count | CI summary | Aumentando |
| Coverage % | Coverage workflow | > 70% |
| Docker Build | Docker workflow | < 10 min |

### Notificações

Configure notificações em: **Settings** > **Notifications**

Opções:
- 📧 Email em falhas
- 💬 Slack/Discord webhooks
- 📱 Mobile notifications

---

## 🔧 Customizações Comuns

### Alterar Versão do .NET

Em todos os workflows:
```yaml
env:
  DOTNET_VERSION: '8.0.x'  # Altere aqui
```

### Desabilitar Workflow Temporariamente

Adicione no início do workflow:
```yaml
on:
  push:
    branches-ignore:
      - '**'  # Ignora todas as branches
```

Ou renomeie: `ci.yml` → `ci.yml.disabled`

### Adicionar Etapa Customizada

```yaml
- name: 🎨 Minha etapa custom
  run: |
    echo "Executando comando customizado"
    # seus comandos aqui
```

### Executar Apenas em Horários Específicos

```yaml
on:
  schedule:
    - cron: '0 2 * * *'  # Todo dia às 2am UTC
```

---

## 🐛 Problemas Comuns e Soluções

### Problema 1: Workflow não executando

**Causa:** Arquivo YAML inválido

**Solução:**
```bash
# Validar localmente (requer act)
act -l

# Ou use ferramentas online
# https://rhysd.github.io/actionlint/
```

### Problema 2: Build falhando no CI mas passando localmente

**Causa:** Diferenças de ambiente

**Solução:**
```yaml
# Adicione debug ao workflow
- name: 🔍 Debug environment
  run: |
    dotnet --info
    dotnet --list-sdks
    printenv
```

### Problema 3: Testes timeout

**Causa:** Testes muito lentos

**Solução:**
```yaml
# Adicione timeout
- name: 🧪 Run tests
  timeout-minutes: 10  # Adicione esta linha
  run: dotnet test
```

### Problema 4: Docker build lento

**Causa:** Cache não configurado

**Solução:**
```yaml
# Já está configurado nos workflows com:
cache-from: type=gha
cache-to: type=gha,mode=max
```

### Problema 5: Cobertura não gerada

**Causa:** Collector não instalado

**Solução:**
```bash
# Instale localmente para testar
dotnet tool install --global coverlet.console
```

---

## 📈 Próximos Passos

### Melhorias Recomendadas

1. **Adicionar SonarCloud** para análise de código avançada
2. **Configurar dependabot** para atualização de dependências
3. **Adicionar testes de integração** no workflow
4. **Configurar deploy automático** para ambiente de staging
5. **Adicionar testes de performance** em workflow separado

### Workflows Futuros Sugeridos

```
.github/workflows/
├── ci.yml                    ✅ Implementado
├── test-coverage.yml         ✅ Implementado
├── docker-build.yml          ✅ Implementado
├── pr-validation.yml         ✅ Implementado
├── deploy-staging.yml        📋 A fazer
├── deploy-production.yml     📋 A fazer
├── security-scan.yml         📋 A fazer
└── performance-test.yml      📋 A fazer
```

---

## 📚 Recursos Adicionais

### Documentação Oficial

- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [Workflow Syntax](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions)
- [Events that trigger workflows](https://docs.github.com/en/actions/reference/events-that-trigger-workflows)

### Ferramentas Úteis

- [act](https://github.com/nektos/act) - Execute workflows localmente
- [actionlint](https://github.com/rhysd/actionlint) - Lint para workflows
- [GitHub Actions Toolkit](https://github.com/actions/toolkit)

### Exemplos e Templates

- [Starter Workflows](https://github.com/actions/starter-workflows)
- [Awesome Actions](https://github.com/sdras/awesome-actions)

---

## ✅ Checklist de Validação

Antes de considerar o setup completo:

- [ ] Todos os workflows executam sem erros
- [ ] Branch protection configurada
- [ ] Badges adicionados ao README
- [ ] Time foi treinado no uso dos workflows
- [ ] Documentação revisada
- [ ] Notificações configuradas
- [ ] Primeiro PR validado com sucesso
- [ ] Imagens Docker publicadas com sucesso
- [ ] Relatório de cobertura acessível

---

## 🆘 Suporte

### Onde Pedir Ajuda

1. **Issues no GitHub** - Para bugs e problemas
2. **Discussions** - Para dúvidas gerais
3. **Pull Requests** - Para melhorias nos workflows

### Informações Úteis ao Reportar Problemas

- Link do workflow run com falha
- Logs completos do erro
- Commit SHA que causou o problema
- Ambiente (branch, sistema operacional do runner)

---

**Última atualização:** 2025-12-19
**Versão:** 1.0.0
**Status:** ✅ Pronto para uso
