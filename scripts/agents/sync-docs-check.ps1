#Requires -Version 5.1
<#
.SYNOPSIS
  Verifica se arquivos alterados exigem atualização de documentação (skill sync-docs).
.EXAMPLE
  ./sync-docs-check.ps1 -ChangedFiles docs/CHANGELOG.md,backend/Arah.Api/Program.cs
#>
param(
    [string[]]$ChangedFiles = @(),
    [string]$BaseRef = 'origin/main'
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

function Normalize-FileList {
    param([string[]]$Files)
    $out = @()
    foreach ($item in $Files) {
        if ($item -match ',') {
            $out += $item -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
        } elseif ($item) { $out += $item.Trim() }
    }
    return @($out | Select-Object -Unique)
}

if ($ChangedFiles.Count -eq 0) {
    Push-Location $Root
    try {
        git fetch origin main --depth=1 2>$null
        $raw = git diff --name-only "$BaseRef...HEAD" 2>$null
        if (-not $raw) { $raw = git diff --name-only HEAD~1 2>$null }
        $ChangedFiles = @($raw | Where-Object { $_ })
    } finally { Pop-Location }
}

$files = Normalize-FileList -Files $ChangedFiles
$warnings = @()
$errors = @()

$codePatterns = @(
    '^backend/',
    '^frontend/arah\.app/lib/',
    '^frontend/wiki/',
    '^frontend/devportal/',
    '^frontend/portal/'
)
$docFiles = @{
    changelog = 'docs/CHANGELOG.md'
    status    = 'docs/STATUS_FASES.md'
    api       = 'docs/60_API_LÓGICA_NEGÓCIO.md'
}

$hasCodeChange = $false
foreach ($f in $files) {
    $norm = $f.Replace('\', '/')
    foreach ($p in $codePatterns) {
        if ($norm -match $p) { $hasCodeChange = $true; break }
    }
}

if ($hasCodeChange) {
    if ($files -notcontains $docFiles.changelog) {
        $warnings += "Código alterado sem $($docFiles.changelog) — avalie se a mudança é significativa."
    }
    $hasApiChange = $files | Where-Object { $_ -match '^backend/Arah\.Api/' }
    if ($hasApiChange -and ($files -notcontains $docFiles.api)) {
        $warnings += "API alterada — considere atualizar $($docFiles.api)."
    }
}

$hasAgentsChange = $files | Where-Object { $_ -match '^\.agents/|^\.skills/|^scripts/agents/' }
if ($hasAgentsChange -and ($files -notcontains 'docs/ops/AGENT_OPERATION.md')) {
    $warnings += 'Infra de agentes alterada — atualize docs/ops/AGENT_OPERATION.md.'
}

$inflationPattern = '^(PR_|RESUMO_|ANALISE_|AVALIACAO_|IMPLEMENTACAO_.*_PLANO|CORRECOES_PENDENTES)'
foreach ($f in $files) {
    $norm = $f.Replace('\', '/')
    if ($norm -match "^docs/[^/]+\.md$" -and (Split-Path $norm -Leaf) -match $inflationPattern) {
        $errors += "Doc inflacionada na raiz de docs/: $norm — use backlog-api/, ops/, issue/PR ou _archive/. Ver docs/_meta/DOC_DEFLATION_PLAN.md"
    }
}

foreach ($w in $warnings) { Write-Warning $w }
foreach ($e in $errors) { Write-Error $e }

if ($errors.Count -gt 0) { exit 1 }
if ($warnings.Count -gt 0) {
    Write-Host "sync-docs-check: $($warnings.Count) aviso(s)."
    exit 0
}
Write-Host 'sync-docs-check: OK'
exit 0
