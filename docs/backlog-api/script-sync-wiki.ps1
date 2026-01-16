# Script para Sincronizar Documentação do Backlog API para Wiki do GitHub
# Uso: .\script-sync-wiki.ps1

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

Write-Host "🚀 Iniciando sincronização para Wiki do GitHub..." -ForegroundColor Green
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

# Criar Home.md (página principal)
Write-Host "📝 Criando Home.md..." -ForegroundColor Yellow
$homeContent = @"
# Documentação Araponga

**Última Atualização**: 2025-01-16

---

## 📋 Índice Geral

### 🎯 Visão e Produto
- [Índice da Documentação](00-Índice)
- [Visão do Produto](01-Visão-do-Produto)
- [Roadmap](02-Roadmap)
- [Backlog](03-Backlog)
- [User Stories](04-User-Stories)
- [Glossário](05-Glossário)

### 🏗️ Arquitetura e Design
- [Decisões Arquiteturais](10-Decisões-Arquiteturais)
- [Arquitetura de Services](11-Arquitetura-de-Services)
- [Modelo de Domínio](12-Modelo-de-Domínio)
- [Domain Routing](13-Domain-Routing)

### 🔧 Desenvolvimento e Implementação
- [Plano de Implementação](20-Plano-de-Implementação)
- [Revisão de Código](21-Revisão-de-Código)
- [Análise de Coesão e Testes](22-Análise-de-Coesão-e-Testes)
- [Implementação de Recomendações](23-Implementação-de-Recomendações)

### 🛡️ Operações e Governança
- [Moderação](30-Moderação)
- [Admin e Observabilidade](31-Admin-e-Observabilidade)
- [Rastreabilidade](32-Rastreabilidade)
- [System Config e Work Queue](33-System-Config-e-Work-Queue)
- [API - Lógica de Negócio](60-API-Lógica-de-Negócio)
- [Preferências de Usuário](61-Preferências-de-Usuário)

### 🔒 Segurança
- [Configuração de Segurança](SECURITY-Configuration)
- [Security Audit](SECURITY-Audit)

### 📝 Histórico e Mudanças
- [Changelog](40-Changelog)
- [Contribuindo](41-Contribuindo)

### 🚀 Produção e Deploy
- [Avaliação Completa para Produção](50-Produção-Avaliação-Completa)
- [Plano de Requisitos Desejáveis](51-Produção-Plano-Desejáveis)
- [Avaliação Geral da Aplicação](70-Avaliação-Geral-Aplicação)
- [Avaliação Completa da Aplicação](AVALIACAO-COMPLETA-APLICACAO)

### 📊 Monitoramento e Operação
- [Runbook](RUNBOOK)
- [Troubleshooting](TROUBLESHOOTING)
- [Incident Playbook](INCIDENT-Playbook)
- [Monitoring](MONITORING)
- [Metrics](METRICS)
- [Media System](MEDIA-System)
- [Deployment Multi-Instance](DEPLOYMENT-Multi-Instance)

---

## 📋 Backlog API

### 🎯 Visão Geral
- [Resumo Executivo Estratégico](Resumo-Executivo-Estratégico)
- [Roadmap Visual](Roadmap-Visual)
- [Mapa de Correlação de Funcionalidades](Mapa-Correlação-Funcionalidades)

### 📊 Estratégia
- [Reorganização Estratégica Final](Reorganização-Estratégica-Final)
- [Revisão Completa de Prioridades](Revisão-Completa-Prioridades)
- [Resumo da Reorganização](Resumo-Reorganização-Final)

### 📄 Fases

#### Fases Completas (1-7) ✅
- [Fase 1: Segurança e Fundação Crítica](Fase-1-Segurança-Fundação-Crítica)
- [Fase 2: Qualidade de Código](Fase-2-Qualidade-Código)
- [Fase 3: Performance e Escalabilidade](Fase-3-Performance-Escalabilidade)
- [Fase 4: Observabilidade](Fase-4-Observabilidade)
- [Fase 5: Segurança Avançada](Fase-5-Segurança-Avançada)
- [Fase 6: Sistema de Pagamentos](Fase-6-Sistema-Pagamentos)
- [Fase 7: Sistema de Payout](Fase-7-Sistema-Payout)

#### Onda 1: MVP Essencial (8-11) 🔴 CRÍTICO
- [Fase 8: Infraestrutura de Mídia](Fase-8-Infraestrutura-Mídia)
- [Fase 9: Perfil de Usuário Completo](Fase-9-Perfil-Usuário-Completo)
- [Fase 10: Mídias em Conteúdo](Fase-10-Mídias-Conteúdo)
- [Fase 11: Edição e Gestão](Fase-11-Edição-Gestão)

#### Onda 2: Comunicação e Governança (13-14) 🔴 CRÍTICO
- [Fase 13: Conector de Emails](Fase-13-Conector-Emails)
- [Fase 14: Governança Comunitária](Fase-14-Governança-Comunitária)

#### Onda 3: Soberania Territorial (17-18) 🔴 ALTA
- [Fase 18: Saúde Territorial](Fase-18-Saúde-Territorial)
- [Fase 17: Gamificação Harmoniosa](Fase-17-Gamificação-Harmoniosa)

#### Onda 4: Economia Local (20, 23-24) 🔴 ALTA
- [Fase 20: Moeda Territorial](Fase-20-Moeda-Territorial)
- [Fase 23: Compra Coletiva](Fase-23-Compra-Coletiva)
- [Fase 24: Sistema de Trocas](Fase-24-Sistema-Trocas)

#### Onda 5: Conformidade e Inteligência (12, 15) 🟡 IMPORTANTE
- [Fase 12: Otimizações Finais](Fase-12-Otimizações-Finais)
- [Fase 15: Inteligência Artificial](Fase-15-Inteligência-Artificial)

#### Onda 6: Diferenciais (16, 19, 21-22) 🟢 OPCIONAL
- [Fase 16: Entregas Territoriais](Fase-16-Entregas-Territoriais)
- [Fase 19: Arquitetura Modular](Fase-19-Arquitetura-Modular)
- [Fase 21: Criptomoedas](Fase-21-Criptomoedas)
- [Fase 22: Integrações Externas](Fase-22-Integrações-Externas)

---

---

## 🔗 Links Úteis

- [Repositório Principal](https://github.com/$REPO_OWNER/$REPO_NAME)
- [Documentação Completa no Repositório](https://github.com/$REPO_OWNER/$REPO_NAME/tree/main/docs)
- [Backlog API no Repositório](https://github.com/$REPO_OWNER/$REPO_NAME/tree/main/docs/backlog-api)

---

**⭐ Ver**: [Reorganização Estratégica Final](Reorganização-Estratégica-Final) para análise detalhada do backlog
"@
$homeContent | Out-File -FilePath "Home.md" -Encoding UTF8
Write-Host "  ✅ Home.md criado" -ForegroundColor Green

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
        
        # Links para documentos da raiz docs/
        $content = $content -replace '\.\.\/00_INDEX\.md', '[Índice](00-Índice)'
        $content = $content -replace '\.\.\/01_PRODUCT_VISION\.md', '[Visão do Produto](01-Visão-do-Produto)'
        $content = $content -replace '\.\.\/02_ROADMAP\.md', '[Roadmap](02-Roadmap)'
        $content = $content -replace '\.\.\/03_BACKLOG\.md', '[Backlog](03-Backlog)'
        $content = $content -replace '\.\.\/40_CHANGELOG\.md', '[Changelog](40-Changelog)'
        $content = $content -replace '\.\.\/MEDIA_SYSTEM\.md', '[Media System](MEDIA-System)'
        $content = $content -replace '\.\.\/MONITORING\.md', '[Monitoring](MONITORING)'
        $content = $content -replace '\.\.\/METRICS\.md', '[Metrics](METRICS)'
        $content = $content -replace '\.\.\/RUNBOOK\.md', '[Runbook](RUNBOOK)'
        $content = $content -replace '\.\.\/TROUBLESHOOTING\.md', '[Troubleshooting](TROUBLESHOOTING)'
        $content = $content -replace '\.\.\/INCIDENT_PLAYBOOK\.md', '[Incident Playbook](INCIDENT-Playbook)'
        $content = $content -replace '\.\.\/SECURITY_CONFIGURATION\.md', '[Security Configuration](SECURITY-Configuration)'
        $content = $content -replace '\.\.\/SECURITY_AUDIT\.md', '[Security Audit](SECURITY-Audit)'
        
        # Links para backlog-api
        $content = $content -replace '\.\.\/backlog-api\/FASE(\d+)\.md', '[Fase $1](Fase-$1)'
        $content = $content -replace '\.\.\/backlog-api\/README\.md', '[Backlog API](Home#backlog-api)'
        
        # Adicionar link para documento completo no repositório
        $repoPath = $sourceFile.Replace($ROOT_DIR, "").Replace("\", "/").TrimStart("/")
        if ($repoPath -notmatch "^docs/") {
            $repoPath = "docs/" + $repoPath
        }
        $content += "`n`n---`n`n**📄 Documento completo**: [Ver no repositório](https://github.com/$REPO_OWNER/$REPO_NAME/blob/main/$repoPath)"
        
        $targetFile = Join-Path $WIKI_DIR "$targetName.md"
        $content | Out-File -FilePath $targetFile -Encoding UTF8
        Write-Host "  ✅ $targetName.md" -ForegroundColor Green
        return $true
    } else {
        Write-Host "  ⚠️  Arquivo não encontrado: $sourceFile" -ForegroundColor Yellow
        return $false
    }
}

# Copiar documentos principais
Write-Host "`n📚 Copiando documentos principais..." -ForegroundColor Yellow

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
}

# Copiar todas as fases
Write-Host "`n📄 Copiando fases (1-24)..." -ForegroundColor Yellow
for ($i = 1; $i -le 24; $i++) {
    $phaseFile = "$DOCS_DIR\FASE$i.md"
    $phaseName = $phaseNames[$i]
    
    if (Copy-DocumentToWiki $phaseFile $phaseName) {
        $docsCopied++
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
    "60_API_LÓGICA_NEGÓCIO.md" = "60-API-Lógica-de-Negócio"
    "61_USER_PREFERENCES_PLAN.md" = "61-Preferências-de-Usuário"
    "70_AVALIACAO_GERAL_APLICACAO.md" = "70-Avaliação-Geral-Aplicação"
    "AVALIACAO_COMPLETA_APLICACAO.md" = "AVALIACAO-COMPLETA-APLICACAO"
    "SECURITY_CONFIGURATION.md" = "SECURITY-Configuration"
    "SECURITY_AUDIT.md" = "SECURITY-Audit"
    "RUNBOOK.md" = "RUNBOOK"
    "TROUBLESHOOTING.md" = "TROUBLESHOOTING"
    "INCIDENT_PLAYBOOK.md" = "INCIDENT-Playbook"
    "MONITORING.md" = "MONITORING"
    "METRICS.md" = "METRICS"
    "MEDIA_SYSTEM.md" = "MEDIA-System"
    "DEPLOYMENT_MULTI_INSTANCE.md" = "DEPLOYMENT-Multi-Instance"
}

foreach ($doc in $mainDocs.GetEnumerator()) {
    $sourceFile = Join-Path $DOCS_ROOT $doc.Key
    if (Copy-DocumentToWiki $sourceFile $doc.Value) {
        $docsCopied++
    }
}

Write-Host "`n✅ Total de documentos copiados: $docsCopied" -ForegroundColor Green

# Commit e push
Write-Host "`n💾 Fazendo commit..." -ForegroundColor Yellow
git add .
$commitMessage = "docs: Sincronização completa da documentação para Wiki

- Adicionada página Home com índice completo
- Migrados $docsCopied documentos (backlog-api + docs/)
- Documentação organizada por categorias
- Links ajustados para estrutura da Wiki
- Links para documentos completos no repositório"
git commit -m $commitMessage

Write-Host "📤 Fazendo push para Wiki..." -ForegroundColor Yellow
git push origin master

Set-Location ..

Write-Host "`n✅ Sincronização completa!" -ForegroundColor Green
Write-Host "🌐 Wiki disponível em: https://github.com/$REPO_OWNER/$REPO_NAME/wiki" -ForegroundColor Cyan

# Limpar diretório temporário
Write-Host "`n🧹 Limpando diretório temporário..." -ForegroundColor Yellow
Remove-Item -Recurse -Force $WIKI_DIR -ErrorAction SilentlyContinue

Write-Host "`n✨ Concluído!" -ForegroundColor Green
