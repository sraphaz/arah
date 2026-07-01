#Requires -Version 5.1
<#
.SYNOPSIS
  Gate arquitetural: exige ADR ou registry update em mudanças estruturais.
#>
param(
    [string[]]$ChangedFiles = @(),
    [string]$BaseRef = 'origin/main'
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

if ($ChangedFiles.Count -eq 0) {
    Push-Location $Root
    try {
        git fetch origin main --depth=1 2>$null
        $ChangedFiles = @(git diff --name-only "$BaseRef...HEAD" 2>$null | Where-Object { $_ })
    } finally { Pop-Location }
}

$structural = @(
    '^backend/Arah\.Core/',
    '^backend/Arah\.Modules\.[^/]+/',
    '^backend/Arah\.(Api|Application|Domain|Infrastructure)/',
    '^docs/architecture/likec4/',
    '^docs/design/.*\.likec4$'
)

$hasStructural = $false
foreach ($f in $ChangedFiles) {
    $n = $f.Replace('\', '/')
    foreach ($p in $structural) {
        if ($n -match $p) { $hasStructural = $true; break }
    }
}

$hasAdrChange = $ChangedFiles | Where-Object {
    $x = $_.Replace('\', '/')
    $x -match '^docs/architecture/adrs/ADR-' -or
    $x -eq 'docs/architecture/ADR-REGISTRY.yaml' -or
    ($x -eq 'docs/architecture/10_ARCHITECTURE_DECISIONS.md' -and $hasStructural)
}

$warnings = @()
$errors = @()

if ($hasStructural -and -not $hasAdrChange) {
    $warnings += 'Mudança estrutural sem ADR — use: ./scripts/agents/register-adr.ps1 -Title "..." -Context "..." -Decision "..." -Consequences "..."'
}

foreach ($w in $warnings) { Write-Warning $w }
foreach ($e in $errors) { Write-Error $e }

if ($errors.Count -gt 0) { exit 1 }
Write-Host "architecture-review-check: OK ($($warnings.Count) aviso(s))"
exit 0
