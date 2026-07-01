#Requires -Version 5.1
<#
.SYNOPSIS
  Gate SDD: spec-before-code e Spec-Id válido em PRs de implementação.
.EXAMPLE
  ./spec-gate-check.ps1 -ChangedFiles backend/Arah.Core/Domain/CoreInstance.cs
  ./spec-gate-check.ps1 -PrBody "Spec-Id: FASE53-arah-core" -ChangedFiles backend/Arah.Core/
#>
param(
    [string[]]$ChangedFiles = @(),
    [string]$PrBody = '',
    [string]$BaseRef = 'origin/main'
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$SpecsRoot = Join-Path $Root 'docs/specs'
$warnings = @()
$errors = @()

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

$phaseCodePatterns = @(
    '^backend/Arah\.Core/',
    '^backend/Arah\.Api/Controllers/Platform/CoreController',
    '^backend/Arah\.Api/Contracts/Core/'
)

$hasPhaseCode = $false
foreach ($f in $files) {
    $norm = $f.Replace('\', '/')
    foreach ($p in $phaseCodePatterns) {
        if ($norm -match $p) { $hasPhaseCode = $true; break }
    }
}

$hasSpecChange = $files | Where-Object { $_.Replace('\', '/') -match '^docs/specs/' }
$hasHarnessChange = $files | Where-Object { $_.Replace('\', '/') -match '^scripts/harness/' }

$specId = $null
if ($PrBody -match '(?mi)^Spec-Id:\s*(\S+)') {
    $specId = $Matches[1].Trim()
}

if ($hasPhaseCode -and -not $hasSpecChange -and -not $specId) {
    $warnings += 'Código de fase (Arah Core) sem Spec-Id no PR nem alteração em docs/specs/ — adicione Spec-Id: FASE53-arah-core ou atualize a spec.'
}

if ($specId) {
    $specFiles = Get-ChildItem -Path $SpecsRoot -Recurse -Filter '*.spec.yaml' |
        Where-Object { $_.Name -ne '_template.spec.yaml' }
    $found = $false
    foreach ($sf in $specFiles) {
        $raw = Get-Content $sf.FullName -Raw
        if ($raw -match "(?m)^id:\s*$([regex]::Escape($specId))\s*$") {
            $found = $true
            if ($raw -match '(?m)^status:\s*deprecated') {
                $errors += "Spec $specId está deprecated."
            }
            break
        }
    }
    if (-not $found) {
        $errors += "Spec-Id '$specId' não encontrado em docs/specs/."
    }
}

if ($hasHarnessChange -or $hasSpecChange) {
    try {
        & (Join-Path $Root 'scripts/harness/validate-specs.ps1') | Out-Null
    } catch {
        $errors += "validate-specs falhou: $($_.Exception.Message)"
    }
}

foreach ($w in $warnings) { Write-Warning $w }
foreach ($e in $errors) { Write-Error $e }

if ($errors.Count -gt 0) { exit 1 }
Write-Host "spec-gate-check: OK ($($warnings.Count) aviso(s))"
exit 0
