# CI/CD Pipeline - Araponga

**Última Atualização**: 2026-01-21  
**Versão**: 1.0

---

## 📋 Visão Geral

O pipeline CI/CD do Araponga está configurado no GitHub Actions e automatiza:
- Build e testes
- Análise de código e segurança
- Build de imagem Docker
- Deploy para staging/produção

---

## 🔄 Fluxo do Pipeline

```
Push/PR → CI (Build + Testes + Security) → CD (Build Docker + Deploy)
```

---

## 📁 Estrutura de Workflows

### `.github/workflows/ci.yml`
**Trigger**: Push para `main` ou Pull Request  
**Jobs**:
1. **build-test**
   - Setup .NET 8.0
   - Restore dependências
   - Build (Release)
   - Testes com code coverage
   - Upload coverage para Codecov
   - Security scan (Trivy)
   - Upload resultados de segurança

### `.github/workflows/cd.yml`
**Trigger**: Push para `main` ou tags `v*.*.*`  
**Jobs**:
1. **build-and-push**
   - Build imagem Docker
   - Push para GHCR (GitHub Container Registry)
   - Tags: `latest` e `{sha}`

---

## 🚀 Execução Local

### Executar Testes

```bash
# Todos os testes
dotnet test backend/Araponga.Tests/Araponga.Tests.csproj

# Com coverage
dotnet test backend/Araponga.Tests/Araponga.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
```

### Build Docker Local

```bash
# Build
docker build -f backend/Araponga.Api/Dockerfile -t araponga-api:local .

# Run
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e JWT__SIGNINGKEY=dev-only-change-me \
  araponga-api:local
```

---

## 🔐 Segurança

### Security Scan (Trivy)

O pipeline executa Trivy para:
- Scan de vulnerabilidades em dependências
- Scan de configurações inseguras
- Upload de resultados para GitHub Security

**Resultados**: Disponível em `Security` → `Code scanning alerts`

### Secrets

Secrets configurados no GitHub:
- `GITHUB_TOKEN` (automático)
- `DOCKER_USERNAME` (se necessário)
- `DOCKER_PASSWORD` (se necessário)

---

## 📊 Code Coverage

### Visualização

- **Codecov**: Dashboard automático após cada PR
- **Local**: Gerar relatório HTML
  ```bash
  dotnet test --collect:"XPlat Code Coverage" --results-directory:./coverage
  reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/html
  ```

### Meta de Cobertura

- **Atual**: ~99.6% (716/718 testes passando)
- **Meta**: Manter > 95%

---

## 🐳 Docker

### Imagem

- **Base**: `mcr.microsoft.com/dotnet/aspnet:8.0`
- **Build**: Multi-stage (SDK → Runtime)
- **Porta**: 8080
- **Health Check**: `/health`

### Registry

- **GHCR**: `ghcr.io/[org]/araponga-api`
- **Tags**:
  - `latest` - Última build de `main`
  - `{sha}` - Build específica por commit

---

## 🚢 Deploy

### Staging

**Automático**: Push para `main` → Deploy automático para staging

**Manual**:
```bash
# Fazer pull da imagem
docker pull ghcr.io/[org]/araponga-api:latest

# Deploy (ver OPERATIONS_MANUAL.md)
```

### Produção

**Manual**: Via workflow_dispatch ou tags `v*.*.*`

**Processo**:
1. Criar tag de release: `git tag v1.0.0 && git push origin v1.0.0`
2. Workflow CD executa automaticamente
3. Deploy manual para produção (ver OPERATIONS_MANUAL.md)

---

## 🔧 Configuração

### Variáveis de Ambiente

Ver `docs/OPERATIONS_MANUAL.md` para lista completa.

### Cache

O pipeline usa cache para:
- Dependências NuGet (`.NET`)
- Docker layers (se configurado)

---

## 📝 Troubleshooting

### Pipeline Falhando

1. **Testes falhando**:
   - Verificar logs do job `build-test`
   - Executar testes localmente
   - Verificar dependências

2. **Build Docker falhando**:
   - Verificar Dockerfile
   - Verificar contexto de build
   - Verificar permissões do registry

3. **Security scan falhando**:
   - Verificar vulnerabilidades reportadas
   - Atualizar dependências se necessário
   - Avaliar se vulnerabilidades são críticas

### Melhorias Futuras

- [ ] Deploy automático para staging
- [ ] Deploy automático para produção (com aprovação)
- [ ] Testes de performance no pipeline
- [ ] Testes de integração com banco real
- [ ] Notificações de deploy (Slack/Email)

---

**Última Atualização**: 2026-01-21
