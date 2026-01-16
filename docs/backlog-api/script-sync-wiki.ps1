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
# Backlog API - Araponga

**Data de Criação**: 2025-01-13  
**Última Revisão**: 2025-01-13  
**Objetivo**: Elevar a aplicação de 7.4-8.0/10 para 10/10 em todas as categorias  
**Estimativa Total**: 380 dias sequenciais / ~170 dias com paralelização  
**Status Atual**: 9.2/10 (após implementação das fases 1-7)

---

## 📋 Índice

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

## 🔗 Links Úteis

- [Repositório Principal](https://github.com/$REPO_OWNER/$REPO_NAME)
- [Documentação Completa no Repositório](https://github.com/$REPO_OWNER/$REPO_NAME/tree/main/docs/backlog-api)

---

**⭐ Ver**: [Reorganização Estratégica Final](Reorganização-Estratégica-Final) para análise detalhada
"@
$homeContent | Out-File -FilePath "Home.md" -Encoding UTF8
Write-Host "  ✅ Home.md criado" -ForegroundColor Green

# Função para copiar e adaptar documento
function Copy-DocumentToWiki {
    param($sourceFile, $targetName)
    
    if (Test-Path $sourceFile) {
        $content = Get-Content $sourceFile -Raw -Encoding UTF8
        
        # Ajustar links relativos para links da Wiki
        $content = $content -replace '\.\/FASE(\d+)\.md', '[Fase $1](Fase-$1)'
        $content = $content -replace '\.\/RESUMO_([^.]+)\.md', '[Resumo $1](Resumo-$1)'
        $content = $content -replace '\.\/REORGANIZACAO_([^.]+)\.md', '[Reorganização $1](Reorganização-$1)'
        $content = $content -replace '\.\/ROADMAP_([^.]+)\.md', '[Roadmap $1](Roadmap-$1)'
        $content = $content -replace '\.\/MAPA_([^.]+)\.md', '[Mapa $1](Mapa-$1)'
        $content = $content -replace '\.\/REVISAO_([^.]+)\.md', '[Revisão $1](Revisão-$1)'
        
        # Adicionar link para documento completo no repositório
        $repoPath = $sourceFile -replace '^\.\\docs\\', '' -replace '\\', '/'
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

Write-Host "`n✅ Total de documentos copiados: $docsCopied" -ForegroundColor Green

# Commit e push
Write-Host "`n💾 Fazendo commit..." -ForegroundColor Yellow
git add .
$commitMessage = "docs: Sincronização completa do plano de ação 10/10 para Wiki

- Adicionada página Home com índice completo
- Migrados $docsCopied documentos principais e fases
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
