# Guia de Desenvolvimento com Docker

Este guia explica como usar o ambiente de desenvolvimento com Docker que suporta **hot-reload** e **debug remoto**.

## Diferenças entre Produção e Desenvolvimento

### Produção (`docker-compose.yml`)
- Usa Dockerfiles multi-stage otimizados
- Compila o código dentro da imagem
- Imagens menores e otimizadas
- Precisa reconstruir a imagem a cada alteração

### Desenvolvimento (`docker-compose.dev.yml`)
- Usa um único `Dockerfile.dev` com SDK completo
- Monta o código fonte como volume
- Hot-reload automático com `dotnet watch`
- Debug remoto configurado
- Não precisa reconstruir imagens

## Como Usar

### 1. Subir o Ambiente de Desenvolvimento

```bash
# Primeira vez (constrói as imagens)
docker compose -f docker-compose.dev.yml up --build

# Próximas vezes (usa cache)
docker compose -f docker-compose.dev.yml up
```

### 2. Hot-Reload Automático

Qualquer alteração que você fizer nos arquivos `.cs` será detectada automaticamente e a aplicação será recarregada:

1. Edite qualquer arquivo em `src/`
2. Salve o arquivo
3. O `dotnet watch` detecta a mudança
4. A aplicação reinicia automaticamente (você verá nos logs)

**Exemplo de log quando o hot-reload acontece:**
```
auth-api-dev | dotnet watch ⌚ File changed: /src/services/AcademyIODevops.Auth.API/Controllers/AuthController.cs.
auth-api-dev | dotnet watch 🔥 Hot reload of changes succeeded.
```

### 3. Debug Remoto

#### No VS Code

1. Certifique-se que os containers estão rodando
2. Abra o painel de Debug (Ctrl+Shift+D)
3. Selecione a configuração desejada:
   - `Docker: Attach to Auth API`
   - `Docker: Attach to Courses API`
   - `Docker: Attach to Payments API`
   - `Docker: Attach to Students API`
   - `Docker: Attach to BFF`
4. Pressione F5 ou clique em "Start Debugging"
5. Coloque breakpoints no código
6. Faça requisições para a API e o breakpoint será atingido

#### No Visual Studio

1. Vá em `Debug > Attach to Process`
2. Em "Connection type", selecione `Docker (Linux Container)`
3. Em "Connection target", clique em "Find..." e selecione o container desejado:
   - `auth-api-dev`
   - `courses-api-dev`
   - `payments-api-dev`
   - `students-api-dev`
   - `bff-api-dev`
4. Selecione o processo `dotnet` e clique em "Attach"
5. Coloque breakpoints e teste

### 4. Comandos Úteis

```bash
# Ver logs de todos os serviços
docker compose -f docker-compose.dev.yml logs -f

# Ver logs de um serviço específico
docker compose -f docker-compose.dev.yml logs -f auth-api

# Parar todos os containers
docker compose -f docker-compose.dev.yml down

# Parar e remover volumes (limpa banco de dados)
docker compose -f docker-compose.dev.yml down -v

# Rebuild de um serviço específico
docker compose -f docker-compose.dev.yml up --build auth-api

# Entrar no container para debug manual
docker exec -it auth-api-dev bash
```

### 5. Estrutura de Volumes

Os volumes são montados da seguinte forma:

```
./src -> /src (no container)
```

Isso significa que todos os arquivos em `src/` no seu host são visíveis dentro do container. Os volumes nomeados (`auth-api-obj`, `auth-api-bin`, etc.) são usados para cache de build, melhorando a performance.

### 6. Portas Expostas

#### APIs
- Auth API: `http://localhost:5077`
- Courses API: `http://localhost:5078`
- Payments API: `http://localhost:5272`
- Students API: `http://localhost:5275`
- BFF: `http://localhost:5107`

#### Debug (caso precise configurar manualmente)
- Auth API: `5177`
- Courses API: `5178`
- Payments API: `5372`
- Students API: `5375`
- BFF: `5207`

#### Infraestrutura
- SQL Server: `localhost:1433`
- RabbitMQ: `localhost:5672`
- RabbitMQ Management: `http://localhost:15672` (guest/guest)

### 7. Dicas de Performance

1. **Windows**: Se estiver no Windows, considere usar WSL2 com Docker Desktop. Volumes montados são MUITO mais rápidos no WSL2 do que no sistema de arquivos Windows.

2. **Volumes nomeados para obj/bin**: Os volumes nomeados (`*-obj`, `*-bin`) evitam que arquivos temporários sejam sincronizados entre host e container, melhorando a performance.

3. **Build inicial**: A primeira vez que você subir o ambiente, pode demorar para instalar o debugger e restaurar pacotes. Nas próximas vezes será muito mais rápido.

### 8. Troubleshooting

#### Hot-reload não está funcionando
- Verifique se `DOTNET_USE_POLLING_FILE_WATCHER=true` está configurado
- No Windows, certifique-se que está usando WSL2

#### Debug não conecta
- Verifique se o container está rodando: `docker ps`
- Verifique se o vsdbg está instalado: `docker exec -it auth-api-dev ls /vsdbg`
- Tente reiniciar o container

#### Performance lenta
- Use WSL2 no Windows
- Considere aumentar recursos do Docker (CPU/RAM)
- Verifique se não há antivírus escaneando os volumes montados

#### Erro de "Cannot access a disposed object"
- Reinicie o container específico: `docker compose -f docker-compose.dev.yml restart auth-api`

## Fluxo de Trabalho Recomendado

1. **Desenvolvimento normal**: Use `docker-compose.dev.yml` com hot-reload
2. **Debug de problema específico**: Attach debugger no serviço específico
3. **Testar mudanças grandes**: Pare e suba novamente: `docker compose -f docker-compose.dev.yml up --build`
4. **Antes de commit**: Teste com `docker-compose.yml` (produção) para garantir que tudo funciona

## Próximos Passos

- Configure variáveis de ambiente específicas em um arquivo `.env` se necessário
- Adicione mais serviços ao `docker-compose.dev.yml` conforme necessário
- Configure testes integrados que rodam no ambiente Docker
