# Script para Sincronizar Documentação do Backlog API para Wiki do GitHub
# Uso: .\script-sync-wiki.ps1
# Versão: 2.0 - Estrutura Livre e Organizada

$ErrorActionPreference = "Stop"

# Configurações
$REPO_OWNER = "sraphaz"
$REPO_NAME = "araponga"
$WIKI_REPO = "https://github.com/$REPO_OWNER/$REPO_NAME.wiki.git"

# Obter diretórios
$SCRIPT_DIR = Split-Path -Parent $MyInvocation.MyCommand.Path
$ROOT_DIR = Split-Path -Parent (Split-Path -Parent $SCRIPT_DIR)
$WIKI_DIR = Join-Path $ROOT_DIR "wiki-temp"
$DOCS_DIR = $SCRIPT_DIR
$DOCS_ROOT = Join-Path $ROOT_DIR "docs"

Write-Host "🚀 Iniciando sincronização estruturada para Wiki do GitHub..." -ForegroundColor Green
Write-Host "📂 Diretório de documentos: $DOCS_DIR" -ForegroundColor Cyan
Write-Host "📂 Diretório raiz: $ROOT_DIR" -ForegroundColor Cyan

# Mudar para diretório raiz
Set-Location $ROOT_DIR

# Limpar diretório temporário se existir
if (Test-Path $WIKI_DIR) {
    Write-Host "📁 Limpando diretório temporário..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $WIKI_DIR
}

# Clonar Wiki
Write-Host "📥 Clonando Wiki do GitHub..." -ForegroundColor Yellow
$wikiExists = $false
try {
    $result = git clone $WIKI_REPO $WIKI_DIR 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Wiki clonada com sucesso!" -ForegroundColor Green
        $wikiExists = $true
    } else {
        throw "Clone failed"
    }
} catch {
    Write-Host "⚠️  Wiki não existe ainda ou não está habilitada." -ForegroundColor Yellow
    Write-Host "💡 Para habilitar a Wiki:" -ForegroundColor Cyan
    Write-Host "   1. Vá para: https://github.com/$REPO_OWNER/$REPO_NAME/settings" -ForegroundColor Cyan
    Write-Host "   2. Em 'Features', habilite 'Wikis'" -ForegroundColor Cyan
    Write-Host "   3. Execute este script novamente" -ForegroundColor Cyan
    Write-Host "`n📝 Criando estrutura local para quando a Wiki for habilitada..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $WIKI_DIR -Force | Out-Null
    Set-Location $WIKI_DIR
    git init
    git remote add origin $WIKI_REPO
    Set-Location $ROOT_DIR
}

Set-Location $WIKI_DIR

# Função para copiar e adaptar documento
function Copy-DocumentToWiki {
    param($sourceFile, $targetName)
    
    if (Test-Path $sourceFile) {
        $content = Get-Content $sourceFile -Raw -Encoding UTF8
        
        # Ajustar links relativos para links da Wiki
        # Links do backlog-api
        $content = $content -replace '\.\/FASE(\d+)\.md', '[Fase $1](Fase-$1)'
        $content = $content -replace '\.\/RESUMO_([^.]+)\.md', '[Resumo $1](Resumo-$1)'
        $content = $content -replace '\.\/REORGANIZACAO_([^.]+)\.md', '[Reorganização $1](Reorganização-$1)'
        $content = $content -replace '\.\/ROADMAP_([^.]+)\.md', '[Roadmap $1](Roadmap-$1)'
        $content = $content -replace '\.\/MAPA_([^.]+)\.md', '[Mapa $1](Mapa-$1)'
        $content = $content -replace '\.\/REVISAO_([^.]+)\.md', '[Revisão $1](Revisão-$1)'
        
        # Links para documentos da raiz docs/ (onboarding e outros)
        $content = $content -replace '\.\.\/00_INDEX\.md', '[Índice](00-Índice)'
        $content = $content -replace '\.\.\/01_PRODUCT_VISION\.md', '[Visão do Produto](01-Visão-do-Produto)'
        $content = $content -replace '\.\.\/02_ROADMAP\.md', '[Roadmap](02-Roadmap)'
        $content = $content -replace '\.\.\/03_BACKLOG\.md', '[Backlog](03-Backlog)'
        $content = $content -replace '\.\.\/40_CHANGELOG\.md', '[Changelog](40-Changelog)'
        $content = $content -replace '\.\.\/41_CONTRIBUTING\.md', '[Contribuindo](41-Contribuindo)'
        
        # Links de onboarding (com e sem ../docs/)
        $content = $content -replace '\.\.\/ONBOARDING_PUBLICO\.md', '[Onboarding Público](Onboarding-Público)'
        $content = $content -replace '\.\.\/ONBOARDING_DEVELOPERS\.md', '[Onboarding Desenvolvedores](Onboarding-Desenvolvedores)'
        $content = $content -replace '\.\.\/ONBOARDING_ANALISTAS_FUNCIONAIS\.md', '[Onboarding Analistas Funcionais](Onboarding-Analistas-Funcionais)'
        $content = $content -replace '\.\.\/CARTILHA_COMPLETA\.md', '[Cartilha Completa](Cartilha-Completa)'
        $content = $content -replace '\.\.\/DISCORD_SETUP\.md', '[Discord Setup](Discord-Setup)'
        $content = $content -replace '\.\.\/docs\/ONBOARDING_PUBLICO\.md', '[Onboarding Público](Onboarding-Público)'
        $content = $content -replace '\.\.\/docs\/ONBOARDING_DEVELOPERS\.md', '[Onboarding Desenvolvedores](Onboarding-Desenvolvedores)'
        $content = $content -replace '\.\.\/docs\/ONBOARDING_ANALISTAS_FUNCIONAIS\.md', '[Onboarding Analistas Funcionais](Onboarding-Analistas-Funcionais)'
        $content = $content -replace '\.\.\/docs\/CARTILHA_COMPLETA\.md', '[Cartilha Completa](Cartilha-Completa)'
        $content = $content -replace '\.\.\/docs\/DISCORD_SETUP\.md', '[Discord Setup](Discord-Setup)'
        $content = $content -replace '\.\/ONBOARDING_PUBLICO\.md', '[Onboarding Público](Onboarding-Público)'
        $content = $content -replace '\.\/ONBOARDING_DEVELOPERS\.md', '[Onboarding Desenvolvedores](Onboarding-Desenvolvedores)'
        $content = $content -replace '\.\/ONBOARDING_ANALISTAS_FUNCIONAIS\.md', '[Onboarding Analistas Funcionais](Onboarding-Analistas-Funcionais)'
        $content = $content -replace '\.\/CARTILHA_COMPLETA\.md', '[Cartilha Completa](Cartilha-Completa)'
        $content = $content -replace '\.\/DISCORD_SETUP\.md', '[Discord Setup](Discord-Setup)'
        
        # Links para outros documentos docs/
        $content = $content -replace '\.\.\/MEDIA_SYSTEM\.md', '[Media System](Media-System)'
        $content = $content -replace '\.\.\/MONITORING\.md', '[Monitoring](Monitoring)'
        $content = $content -replace '\.\.\/METRICS\.md', '[Metrics](Metrics)'
        $content = $content -replace '\.\.\/RUNBOOK\.md', '[Runbook](Runbook)'
        $content = $content -replace '\.\.\/TROUBLESHOOTING\.md', '[Troubleshooting](Troubleshooting)'
        $content = $content -replace '\.\.\/INCIDENT_PLAYBOOK\.md', '[Incident Playbook](Incident-Playbook)'
        $content = $content -replace '\.\.\/SECURITY_CONFIGURATION\.md', '[Security Configuration](Security-Configuration)'
        $content = $content -replace '\.\.\/SECURITY_AUDIT\.md', '[Security Audit](Security-Audit)'
        
        # Links para backlog-api
        $content = $content -replace '\.\.\/backlog-api\/FASE(\d+)\.md', '[Fase $1](Fase-$1)'
        $content = $content -replace '\.\.\/backlog-api\/README\.md', '[Backlog API](Backlog-API)'
        $content = $content -replace '\.\.\/backlog-api\/implementacoes\/FASE(\d+)_([^.]+)\.md', '[Fase $1 $2](Home#backlog-api)'
        $content = $content -replace '\.\/implementacoes\/FASE(\d+)_([^.]+)\.md', '[Fase $1 $2](Home#backlog-api)'
        
        # Links absolutos do GitHub (transformar em links da Wiki quando for documentação local)
        $content = $content -replace 'https://github.com/sraphaz/araponga/blob/main/docs/([^.]+)\.md', '[${1}](${1})'
        $content = $content -replace 'https://github.com/sraphaz/araponga/blob/main/docs/ONBOARDING_PUBLICO\.md', '[Onboarding Público](Onboarding-Público)'
        $content = $content -replace 'https://github.com/sraphaz/araponga/blob/main/docs/ONBOARDING_DEVELOPERS\.md', '[Onboarding Desenvolvedores](Onboarding-Desenvolvedores)'
        $content = $content -replace 'https://github.com/sraphaz/araponga/blob/main/docs/ONBOARDING_ANALISTAS_FUNCIONAIS\.md', '[Onboarding Analistas Funcionais](Onboarding-Analistas-Funcionais)'
        $content = $content -replace 'https://github.com/sraphaz/araponga/blob/main/docs/CARTILHA_COMPLETA\.md', '[Cartilha Completa](Cartilha-Completa)'
        $content = $content -replace 'https://github.com/sraphaz/araponga/blob/main/docs/DISCORD_SETUP\.md', '[Discord Setup](Discord-Setup)'
        
        # Adicionar link para documento completo no repositório
        $repoPath = $sourceFile.Replace($ROOT_DIR, "").Replace("\", "/").TrimStart("/")
        if ($repoPath -notmatch "^docs/") {
            $repoPath = "docs/" + $repoPath
        }
        $content += "`n`n---`n`n**📄 Documento completo**: [Ver no repositório](https://github.com/$REPO_OWNER/$REPO_NAME/blob/main/$repoPath)"
        
        $targetFile = Join-Path $WIKI_DIR "$targetName.md"
        $content | Out-File -FilePath $targetFile -Encoding UTF8
        return $true
    } else {
        return $false
    }
}

# ============================================
# CRIAR PÁGINAS DE ÍNDICE POR CATEGORIA
# ============================================

Write-Host "`n📋 Criando estrutura organizada..." -ForegroundColor Yellow

# 1. Home.md - Página Principal
# Usar o conteúdo elevado e consciente do WIKI_HOME.md se existir
$wikiHomeFile = Join-Path $DOCS_ROOT "WIKI_HOME.md"
if (Test-Path $wikiHomeFile) {
    Write-Host "  📖 Usando WIKI_HOME.md com conteúdo elevado..." -ForegroundColor Cyan
    $homeContent = Get-Content $wikiHomeFile -Raw -Encoding UTF8
    # Ajustar links para estrutura da Wiki
    $homeContent = $homeContent -replace '\.\./docs/', ''
    $homeContent = $homeContent -replace 'docs/', ''
    $homeContent = $homeContent -replace 'https://github.com/sraphaz/araponga/blob/main/docs/([^.]+)\.md', '[$1]($1)'
    $homeContent = $homeContent -replace 'ONBOARDING_PUBLICO', 'Onboarding-Público'
    $homeContent = $homeContent -replace 'ONBOARDING_DEVELOPERS', 'Onboarding-Desenvolvedores'
    $homeContent = $homeContent -replace 'ONBOARDING_ANALISTAS_FUNCIONAIS', 'Onboarding-Analistas-Funcionais'
    $homeContent = $homeContent -replace 'CARTILHA_COMPLETA', 'Cartilha-Completa'
    $homeContent = $homeContent -replace 'DISCORD_SETUP', 'Discord-Setup'
} else {
    # Fallback para conteúdo padrão
    $homeContent = @"
# 🦜 Araponga - Documentação Completa

**Status Atual**: 9.3/10 | **Fases Completas**: 1-8 ✅  
**Última Atualização**: 2025-01-20

---

## 🚀 Início Rápido

- **[📖 Guia de Início](Início-Rápido)** - Comece aqui se é novo no projeto
- **[📊 Status do Projeto](Status-do-Projeto)** - Visão geral do estado atual
- **[🎯 Backlog API](Backlog-API)** - Plano completo de 29 fases
- **[📚 Índice Completo](00-Índice)** - Todos os documentos organizados

---

## 🌱 Para Conhecer o Projeto

**[🌟 Onboarding Público](Onboarding-Público)** - Sua porta de entrada para o Araponga

---

## 📋 Navegação Principal

### 🎯 Produto e Visão
- [Visão do Produto](01-Visão-do-Produto)
- [Roadmap](02-Roadmap)
- [Backlog](03-Backlog)
- [User Stories](04-User-Stories)
- [Glossário](05-Glossário)

### 🏗️ Arquitetura
- [Decisões Arquiteturais (ADRs)](10-Decisões-Arquiteturais)
- [Arquitetura de Services](11-Arquitetura-de-Services)
- [Modelo de Domínio](12-Modelo-de-Domínio)
- [Domain Routing](13-Domain-Routing)

### 🔧 Desenvolvimento
- [Plano de Implementação](20-Plano-de-Implementação)
- [Revisão de Código](21-Revisão-de-Código)
- [Análise de Coesão e Testes](22-Análise-de-Coesão-e-Testes)
- [Implementação de Recomendações](23-Implementação-de-Recomendações)

### 🛡️ Operações
- [Moderação](30-Moderação)
- [Admin e Observabilidade](31-Admin-e-Observabilidade)
- [Rastreabilidade](32-Rastreabilidade)
- [System Config e Work Queue](33-System-Config-e-Work-Queue)
- [API - Lógica de Negócio](60-API-Lógica-de-Negócio)
- [Preferências de Usuário](61-Preferências-de-Usuário)

### 🔒 Segurança
- [Configuração de Segurança](Security-Configuration)
- [Security Audit](Security-Audit)

### 📊 Produção
- [Avaliação Completa para Produção](50-Produção-Avaliação-Completa)
- [Avaliação Geral da Aplicação](70-Avaliação-Geral-Aplicação)
- [Plano de Requisitos Desejáveis](51-Produção-Plano-Desejáveis)

### 📝 Histórico
- [Changelog](40-Changelog)
- [Contribuindo](41-Contribuindo)

### 🔧 Operação
- [Runbook](Runbook)
- [Troubleshooting](Troubleshooting)
- [Incident Playbook](Incident-Playbook)
- [Monitoring](Monitoring)
- [Metrics](Metrics)
- [Media System](Media-System)
- [Deployment Multi-Instance](Deployment-Multi-Instance)

---

## 📋 Backlog API - 29 Fases

### ✅ Fases Completas (1-8)
- [Fase 1: Segurança e Fundação Crítica](Fase-1-Segurança-Fundação-Crítica) ✅
- [Fase 2: Qualidade de Código](Fase-2-Qualidade-Código) ✅
- [Fase 3: Performance e Escalabilidade](Fase-3-Performance-Escalabilidade) ✅
- [Fase 4: Observabilidade](Fase-4-Observabilidade) ✅
- [Fase 5: Segurança Avançada](Fase-5-Segurança-Avançada) ✅
- [Fase 6: Sistema de Pagamentos](Fase-6-Sistema-Pagamentos) ✅
- [Fase 7: Sistema de Payout](Fase-7-Sistema-Payout) ✅
- [Fase 8: Infraestrutura de Mídia](Fase-8-Infraestrutura-Mídia) ✅

### 🔴 Onda 1: MVP Essencial (9-11)
- [Fase 9: Perfil de Usuário Completo](Fase-9-Perfil-Usuário-Completo)
- [Fase 10: Mídias em Conteúdo](Fase-10-Mídias-Conteúdo)
- [Fase 11: Edição e Gestão](Fase-11-Edição-Gestão)

### 🔴 Onda 2: Comunicação e Governança (13-14)
- [Fase 13: Conector de Emails](Fase-13-Conector-Emails)
- [Fase 14: Governança Comunitária](Fase-14-Governança-Comunitária)

### 🔴 Onda 3: Soberania Territorial (17-18)
- [Fase 17: Gamificação Harmoniosa](Fase-17-Gamificação-Harmoniosa)
- [Fase 18: Saúde Territorial](Fase-18-Saúde-Territorial)

### 🔴 Onda 4: Economia Local (20, 23-24)
- [Fase 20: Moeda Territorial](Fase-20-Moeda-Territorial)
- [Fase 23: Compra Coletiva](Fase-23-Compra-Coletiva)
- [Fase 24: Sistema de Trocas](Fase-24-Sistema-Trocas)

### 🟡 Onda 5: Conformidade e Inteligência (12, 15)
- [Fase 12: Otimizações Finais](Fase-12-Otimizações-Finais)
- [Fase 15: Inteligência Artificial](Fase-15-Inteligência-Artificial)

### 🟢 Onda 6: Diferenciais (16, 19, 21-22)
- [Fase 16: Entregas Territoriais](Fase-16-Entregas-Territoriais)
- [Fase 19: Arquitetura Modular](Fase-19-Arquitetura-Modular)
- [Fase 21: Criptomoedas](Fase-21-Criptomoedas)
- [Fase 22: Integrações Externas](Fase-22-Integrações-Externas)

### 🟢 Onda 7: Autonomia Digital e Economia Circular (25-28)
- [Fase 25: Hub de Serviços Digitais](Fase-25-Hub-Serviços-Digitais)
- [Fase 26: Chat com IA e Consumo Consciente](Fase-26-Chat-IA-Consumo-Consciente)
- [Fase 27: Negociação Territorial](Fase-27-Negociação-Territorial)
- [Fase 28: Banco de Sementes e Mudas](Fase-28-Banco-Sementes-Mudas)

### 🟡 Onda 8: Mobile Avançado (29)
- [Fase 29: Suporte Mobile Avançado](Fase-29-Suporte-Mobile-Avançado)

**📊 Ver**: [Backlog API Completo](Backlog-API) | [Reorganização Estratégica](Reorganização-Estratégica-Final)

---

## 🔗 Links Úteis

- [Repositório Principal](https://github.com/$REPO_OWNER/$REPO_NAME)
- [Documentação no Repositório](https://github.com/$REPO_OWNER/$REPO_NAME/tree/main/docs)
- [Backlog API no Repositório](https://github.com/$REPO_OWNER/$REPO_NAME/tree/main/docs/backlog-api)
- [Issues](https://github.com/$REPO_OWNER/$REPO_NAME/issues)
- [Pull Requests](https://github.com/$REPO_OWNER/$REPO_NAME/pulls)

---

**⭐ Dica**: Use a barra lateral da Wiki para navegação rápida entre páginas!
"@
$homeContent | Out-File -FilePath "Home.md" -Encoding UTF8
Write-Host "  ✅ Home.md criado" -ForegroundColor Green

# 2. Página de Início Rápido
$quickStartContent = @"
# 🚀 Início Rápido

Bem-vindo à documentação do **Araponga**! Este guia ajuda você a começar rapidamente.

## 📖 Para Desenvolvedores

### Primeiros Passos
1. **[Visão do Produto](01-Visão-do-Produto)** - Entenda o que é o Araponga
2. **[Arquitetura](10-Decisões-Arquiteturais)** - Conheça as decisões arquiteturais
3. **[Modelo de Domínio](12-Modelo-de-Domínio)** - Entenda a estrutura de dados
4. **[API - Lógica de Negócio](60-API-Lógica-de-Negócio)** - Documentação completa da API

### Desenvolvimento
- **[Plano de Implementação](20-Plano-de-Implementação)** - O que está implementado
- **[Revisão de Código](21-Revisão-de-Código)** - Padrões e boas práticas
- **[Contribuindo](41-Contribuindo)** - Como contribuir

### Operação
- **[Runbook](Runbook)** - Operação em produção
- **[Troubleshooting](Troubleshooting)** - Resolução de problemas
- **[Monitoring](Monitoring)** - Monitoramento e métricas

## 📊 Para Gestores/Product Owners

### Visão Estratégica
1. **[Visão do Produto](01-Visão-do-Produto)** - Visão geral e princípios
2. **[Roadmap](02-Roadmap)** - Planejamento de funcionalidades
3. **[Backlog](03-Backlog)** - Lista de funcionalidades
4. **[Status do Projeto](Status-do-Projeto)** - Estado atual

### Planejamento
- **[Backlog API](Backlog-API)** - Plano completo de 29 fases
- **[Avaliação para Produção](50-Produção-Avaliação-Completa)** - Prontidão atual
- **[Reorganização Estratégica](Reorganização-Estratégica-Final)** - Estratégia de implementação

## 🔒 Para Security/DevOps

### Segurança
- **[Configuração de Segurança](Security-Configuration)** - Configuração completa
- **[Security Audit](Security-Audit)** - Checklist e penetration testing
- **[Fase 1: Segurança](Fase-1-Segurança-Fundação-Crítica)** - Implementações de segurança
- **[Fase 5: Segurança Avançada](Fase-5-Segurança-Avançada)** - 2FA, CSRF, etc.

### Operação
- **[Deployment Multi-Instance](Deployment-Multi-Instance)** - Deploy distribuído
- **[Incident Playbook](Incident-Playbook)** - Resposta a incidentes
- **[Metrics](Metrics)** - Métricas do sistema

## 📚 Estrutura da Documentação

A documentação está organizada em categorias:

- **00-09**: Índices e guias
- **10-19**: Arquitetura e Design
- **20-29**: Desenvolvimento e Implementação
- **30-39**: Operações e Governança
- **40-49**: Histórico e Mudanças
- **50-59**: Produção e Deploy
- **60-69**: API e Funcionalidades
- **70-79**: Avaliações

## 🎯 Próximos Passos

1. Explore a **[Visão do Produto](01-Visão-do-Produto)**
2. Veja o **[Status Atual](Status-do-Projeto)**
3. Consulte o **[Backlog API](Backlog-API)** para o que vem por aí
4. Leia a **[Arquitetura](10-Decisões-Arquiteturais)** para entender o sistema

---

**💡 Dica**: Use `Ctrl+F` ou `Cmd+F` para buscar dentro de qualquer página!
"@
$quickStartContent | Out-File -FilePath "Início-Rápido.md" -Encoding UTF8
Write-Host "  ✅ Início-Rápido.md criado" -ForegroundColor Green

# 3. Página de Status do Projeto
$statusContent = @"
# 📊 Status do Projeto

**Última Atualização**: 2025-01-16

---

## 🎯 Status Geral

**Nota Atual**: **9.3/10**  
**Fases Completas**: **1-8 ✅**  
**Pronto para Produção**: ✅ **SIM**

---

## ✅ Fases Completas (1-8)

| Fase | Nome | Status | Data |
|------|------|--------|------|
| 1 | Segurança e Fundação Crítica | ✅ Completo | 2025-01 |
| 2 | Qualidade de Código | ✅ Completo | 2025-01-15 |
| 3 | Performance e Escalabilidade | ✅ Completo | 2025-01-15 |
| 4 | Observabilidade | ✅ Completo | 2025-01-15 |
| 5 | Segurança Avançada | ✅ Completo | 2025-01-15 |
| 6 | Sistema de Pagamentos | ✅ Completo | 2025-01 |
| 7 | Sistema de Payout | ✅ Completo | 2025-01 |
| 8 | Infraestrutura de Mídia | ✅ Completo | 2025-01-16 |

---

## 📊 Avaliação por Categoria

| Categoria | Nota | Status |
|-----------|------|--------|
| **Funcionalidades** | 9/10 | ✅ Excelente |
| **Arquitetura** | 9/10 | ✅ Excelente |
| **Design Patterns** | 9/10 | ✅ Excelente |
| **Segurança** | 9/10 | ✅ Excelente |
| **Performance** | 9/10 | ✅ Excelente |
| **Tratamento de Erros** | 9/10 | ✅ Excelente |
| **Testes** | 9/10 | ✅ Excelente (>90% cobertura) |
| **Observabilidade** | 9/10 | ✅ Excelente |
| **Configuração** | 8/10 | ✅ Boa |
| **Documentação** | 9/10 | ✅ Excelente |

**Nota Final**: **9.3/10**

---

## 🔄 Próximas Fases

### Onda 1: MVP Essencial (9-11) 🔴 CRÍTICO
- Fase 9: Perfil de Usuário Completo
- Fase 10: Mídias em Conteúdo
- Fase 11: Edição e Gestão

### Onda 2: Comunicação e Governança (13-14) 🔴 CRÍTICO
- Fase 13: Conector de Emails
- Fase 14: Governança Comunitária

**Ver**: [Backlog API Completo](Backlog-API) para todas as 29 fases

---

## 📈 Progresso

- **Fases Completas**: 8/29 (28%)
- **Valor Entregue**: ~40% (Ondas 1-2 críticas)
- **Tempo Estimado Restante**: ~170 dias com paralelização

---

## 🔗 Links Relacionados

- [Avaliação Completa para Produção](50-Produção-Avaliação-Completa)
- [Avaliação Geral da Aplicação](70-Avaliação-Geral-Aplicação)
- [Backlog API](Backlog-API)
- [Changelog](40-Changelog)
"@
$statusContent | Out-File -FilePath "Status-do-Projeto.md" -Encoding UTF8
Write-Host "  ✅ Status-do-Projeto.md criado" -ForegroundColor Green

# 4. Página do Backlog API
$backlogContent = @"
# 📋 Backlog API - 29 Fases Estratégicas

**Status Atual**: 9.3/10 | **Fases Completas**: 1-8 ✅  
**Última Atualização**: 2025-01-20

---

## 🎯 Visão Geral

O Backlog API organiza 29 fases em **8 Ondas Estratégicas** para elevar a aplicação de 7.4-8.0/10 para 10/10 em todas as categorias.

**Estimativa Total**: 380 dias sequenciais / ~170 dias com paralelização  
**90% do valor em 233 dias (47 semanas)**

---

## ✅ Fases Completas (1-8)

- [Fase 1: Segurança e Fundação Crítica](Fase-1-Segurança-Fundação-Crítica) ✅
- [Fase 2: Qualidade de Código](Fase-2-Qualidade-Código) ✅
- [Fase 3: Performance e Escalabilidade](Fase-3-Performance-Escalabilidade) ✅
- [Fase 4: Observabilidade](Fase-4-Observabilidade) ✅
- [Fase 5: Segurança Avançada](Fase-5-Segurança-Avançada) ✅
- [Fase 6: Sistema de Pagamentos](Fase-6-Sistema-Pagamentos) ✅
- [Fase 7: Sistema de Payout](Fase-7-Sistema-Payout) ✅
- [Fase 8: Infraestrutura de Mídia](Fase-8-Infraestrutura-Mídia) ✅

---

## 📊 Ondas Estratégicas

### 🔴 Onda 1: MVP Essencial (65 dias) - 40% do Valor
- [Fase 9: Perfil de Usuário Completo](Fase-9-Perfil-Usuário-Completo) - 15 dias
- [Fase 10: Mídias em Conteúdo](Fase-10-Mídias-Conteúdo) - 20 dias
- [Fase 11: Edição e Gestão](Fase-11-Edição-Gestão) - 15 dias

### 🔴 Onda 2: Comunicação e Governança (21 dias) - 10% do Valor
- [Fase 13: Conector de Emails](Fase-13-Conector-Emails) - 14 dias
- [Fase 14: Governança Comunitária](Fase-14-Governança-Comunitária) - 21 dias

### 🔴 Onda 3: Soberania Territorial (63 dias) - 25% do Valor
- [Fase 17: Gamificação Harmoniosa](Fase-17-Gamificação-Harmoniosa) - 28 dias
- [Fase 18: Saúde Territorial](Fase-18-Saúde-Territorial) - 35 dias

### 🔴 Onda 4: Economia Local (84 dias) - 25% do Valor
- [Fase 20: Moeda Territorial](Fase-20-Moeda-Territorial) - 35 dias
- [Fase 23: Compra Coletiva](Fase-23-Compra-Coletiva) - 28 dias
- [Fase 24: Sistema de Trocas](Fase-24-Sistema-Trocas) - 21 dias

### 🟡 Onda 5: Conformidade e Inteligência (49 dias) - 5% do Valor
- [Fase 12: Otimizações Finais](Fase-12-Otimizações-Finais) - 28 dias
- [Fase 15: Inteligência Artificial](Fase-15-Inteligência-Artificial) - 28 dias

### 🟢 Onda 6: Diferenciais (98 dias) - 5% do Valor
- [Fase 16: Entregas Territoriais](Fase-16-Entregas-Territoriais) - 28 dias
- [Fase 19: Arquitetura Modular](Fase-19-Arquitetura-Modular) - 35 dias
- [Fase 21: Criptomoedas](Fase-21-Criptomoedas) - 28 dias
- [Fase 22: Integrações Externas](Fase-22-Integrações-Externas) - 35 dias

### 🟢 Onda 7: Autonomia Digital e Economia Circular (84 dias) - 10% do Valor
- [Fase 25: Hub de Serviços Digitais](Fase-25-Hub-Serviços-Digitais) - 21 dias
- [Fase 26: Chat com IA e Consumo Consciente](Fase-26-Chat-IA-Consumo-Consciente) - 14 dias
- [Fase 27: Negociação Territorial](Fase-27-Negociação-Territorial) - 28 dias
- [Fase 28: Banco de Sementes e Mudas](Fase-28-Banco-Sementes-Mudas) - 21 dias

### 🟡 Onda 8: Mobile Avançado (14 dias) - 2% do Valor
- [Fase 29: Suporte Mobile Avançado](Fase-29-Suporte-Mobile-Avançado) - 14 dias

---

## 📚 Documentos Estratégicos

- [Resumo Executivo Estratégico](Resumo-Executivo-Estratégico)
- [Roadmap Visual](Roadmap-Visual)
- [Mapa de Correlação de Funcionalidades](Mapa-Correlação-Funcionalidades)
- [Reorganização Estratégica Final](Reorganização-Estratégica-Final)
- [Revisão Completa de Prioridades](Revisão-Completa-Prioridades)
- [Resumo da Reorganização](Resumo-Reorganização-Final)

---

## 🔗 Links Úteis

- [Backlog API no Repositório](https://github.com/$REPO_OWNER/$REPO_NAME/tree/main/docs/backlog-api)
- [Status do Projeto](Status-do-Projeto)
- [Avaliação para Produção](50-Produção-Avaliação-Completa)
"@
$backlogContent | Out-File -FilePath "Backlog-API.md" -Encoding UTF8
Write-Host "  ✅ Backlog-API.md criado" -ForegroundColor Green

# ============================================
# COPIAR DOCUMENTOS
# ============================================

# Copiar documentos principais do backlog-api
Write-Host "`n📚 Copiando documentos estratégicos..." -ForegroundColor Yellow
$docsCopied = 0
$docsCopied += [int](Copy-DocumentToWiki "$DOCS_DIR\RESUMO_EXECUTIVO_ESTRATEGICO.md" "Resumo-Executivo-Estratégico")
$docsCopied += [int](Copy-DocumentToWiki "$DOCS_DIR\ROADMAP_VISUAL.md" "Roadmap-Visual")
$docsCopied += [int](Copy-DocumentToWiki "$DOCS_DIR\MAPA_CORRELACAO_FUNCIONALIDADES.md" "Mapa-Correlação-Funcionalidades")
$docsCopied += [int](Copy-DocumentToWiki "$DOCS_DIR\REORGANIZACAO_ESTRATEGICA_FINAL.md" "Reorganização-Estratégica-Final")
$docsCopied += [int](Copy-DocumentToWiki "$DOCS_DIR\REVISAO_COMPLETA_PRIORIDADES.md" "Revisão-Completa-Prioridades")
$docsCopied += [int](Copy-DocumentToWiki "$DOCS_DIR\RESUMO_REORGANIZACAO_FINAL.md" "Resumo-Reorganização-Final")

# Mapeamento de nomes de fases
$phaseNames = @{
    1 = "Fase-1-Segurança-Fundação-Crítica"
    2 = "Fase-2-Qualidade-Código"
    3 = "Fase-3-Performance-Escalabilidade"
    4 = "Fase-4-Observabilidade"
    5 = "Fase-5-Segurança-Avançada"
    6 = "Fase-6-Sistema-Pagamentos"
    7 = "Fase-7-Sistema-Payout"
    8 = "Fase-8-Infraestrutura-Mídia"
    9 = "Fase-9-Perfil-Usuário-Completo"
    10 = "Fase-10-Mídias-Conteúdo"
    11 = "Fase-11-Edição-Gestão"
    12 = "Fase-12-Otimizações-Finais"
    13 = "Fase-13-Conector-Emails"
    14 = "Fase-14-Governança-Comunitária"
    15 = "Fase-15-Inteligência-Artificial"
    16 = "Fase-16-Entregas-Territoriais"
    17 = "Fase-17-Gamificação-Harmoniosa"
    18 = "Fase-18-Saúde-Territorial"
    19 = "Fase-19-Arquitetura-Modular"
    20 = "Fase-20-Moeda-Territorial"
    21 = "Fase-21-Criptomoedas"
    22 = "Fase-22-Integrações-Externas"
    23 = "Fase-23-Compra-Coletiva"
    24 = "Fase-24-Sistema-Trocas"
    25 = "Fase-25-Hub-Serviços-Digitais"
    26 = "Fase-26-Chat-IA-Consumo-Consciente"
    27 = "Fase-27-Negociação-Territorial"
    28 = "Fase-28-Banco-Sementes-Mudas"
    29 = "Fase-29-Suporte-Mobile-Avançado"
    27 = "Fase-27-Negociação-Territorial"
    28 = "Fase-28-Banco-Sementes-Mudas"
    29 = "Fase-29-Suporte-Mobile-Avançado"
}

# Copiar todas as fases
Write-Host "`n📄 Copiando fases (1-29)..." -ForegroundColor Yellow
for ($i = 1; $i -le 29; $i++) {
    $phaseFile = "$DOCS_DIR\FASE$i.md"
    $phaseName = $phaseNames[$i]
    
    if (Copy-DocumentToWiki $phaseFile $phaseName) {
        $docsCopied++
        Write-Host "  ✅ $phaseName.md" -ForegroundColor Green
    }
}

# Copiar documentos da raiz docs/
Write-Host "`n📚 Copiando documentação geral..." -ForegroundColor Yellow

# Mapeamento de documentos principais
$mainDocs = @{
    "00_INDEX.md" = "00-Índice"
    "01_PRODUCT_VISION.md" = "01-Visão-do-Produto"
    "02_ROADMAP.md" = "02-Roadmap"
    "03_BACKLOG.md" = "03-Backlog"
    "04_USER_STORIES.md" = "04-User-Stories"
    "05_GLOSSARY.md" = "05-Glossário"
    "10_ARCHITECTURE_DECISIONS.md" = "10-Decisões-Arquiteturais"
    "11_ARCHITECTURE_SERVICES.md" = "11-Arquitetura-de-Services"
    "12_DOMAIN_MODEL.md" = "12-Modelo-de-Domínio"
    "13_DOMAIN_ROUTING.md" = "13-Domain-Routing"
    "20_IMPLEMENTATION_PLAN.md" = "20-Plano-de-Implementação"
    "21_CODE_REVIEW.md" = "21-Revisão-de-Código"
    "22_COHESION_AND_TESTS.md" = "22-Análise-de-Coesão-e-Testes"
    "23_IMPLEMENTATION_RECOMMENDATIONS.md" = "23-Implementação-de-Recomendações"
    "30_MODERATION.md" = "30-Moderação"
    "31_ADMIN_OBSERVABILITY.md" = "31-Admin-e-Observabilidade"
    "32_TRACEABILITY.md" = "32-Rastreabilidade"
    "33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md" = "33-System-Config-e-Work-Queue"
    "40_CHANGELOG.md" = "40-Changelog"
    "41_CONTRIBUTING.md" = "41-Contribuindo"
    "50_PRODUCAO_AVALIACAO_COMPLETA.md" = "50-Produção-Avaliação-Completa"
    "51_PRODUCAO_PLANO_DESEJAVEIS.md" = "51-Produção-Plano-Desejáveis"
    "ONBOARDING_PUBLICO.md" = "Onboarding-Público"
    "ONBOARDING_DEVELOPERS.md" = "Onboarding-Desenvolvedores"
    "ONBOARDING_ANALISTAS_FUNCIONAIS.md" = "Onboarding-Analistas-Funcionais"
    "CARTILHA_COMPLETA.md" = "Cartilha-Completa"
    "DISCORD_SETUP.md" = "Discord-Setup"
    "60_API_LÓGICA_NEGÓCIO.md" = "60-API-Lógica-de-Negócio"
    "61_USER_PREFERENCES_PLAN.md" = "61-Preferências-de-Usuário"
    "70_AVALIACAO_GERAL_APLICACAO.md" = "70-Avaliação-Geral-Aplicação"
    "AVALIACAO_COMPLETA_APLICACAO.md" = "AVALIACAO-COMPLETA-APLICACAO"
    "SECURITY_CONFIGURATION.md" = "Security-Configuration"
    "SECURITY_AUDIT.md" = "Security-Audit"
    "RUNBOOK.md" = "Runbook"
    "TROUBLESHOOTING.md" = "Troubleshooting"
    "INCIDENT_PLAYBOOK.md" = "Incident-Playbook"
    "MONITORING.md" = "Monitoring"
    "METRICS.md" = "Metrics"
    "MEDIA_SYSTEM.md" = "Media-System"
    "DEPLOYMENT_MULTI_INSTANCE.md" = "Deployment-Multi-Instance"
}

foreach ($doc in $mainDocs.GetEnumerator()) {
    $sourceFile = Join-Path $DOCS_ROOT $doc.Key
    if (Copy-DocumentToWiki $sourceFile $doc.Value) {
        $docsCopied++
        Write-Host "  ✅ $($doc.Value).md" -ForegroundColor Green
    }
}

Write-Host "`n✅ Total de documentos copiados: $docsCopied" -ForegroundColor Green

# Commit e push
Write-Host "`n💾 Fazendo commit..." -ForegroundColor Yellow
git add .
$commitMessage = "docs: Estrutura livre e organizada da Wiki

- Home.md: Página principal com navegação intuitiva
- Início-Rápido.md: Guia para novos usuários
- Status-do-Projeto.md: Status atual e progresso
- Backlog-API.md: Índice completo do backlog
- $docsCopied documentos organizados
- Links ajustados para estrutura da Wiki
- Navegação melhorada por categorias"
git commit -m $commitMessage

Write-Host "📤 Fazendo push para Wiki..." -ForegroundColor Yellow
git push origin master

Set-Location $ROOT_DIR

Write-Host "`n✅ Sincronização completa!" -ForegroundColor Green
Write-Host "🌐 Wiki disponível em: https://github.com/$REPO_OWNER/$REPO_NAME/wiki" -ForegroundColor Cyan

# Limpar diretório temporário
Write-Host "`n🧹 Limpando diretório temporário..." -ForegroundColor Yellow
Remove-Item -Recurse -Force $WIKI_DIR -ErrorAction SilentlyContinue

Write-Host "`n✨ Concluído!" -ForegroundColor Green
