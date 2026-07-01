#Requires -Version 5.1
<#
.SYNOPSIS
  Desinfla docs/ movendo artefatos de sessão/PR/análise para _archive/ com stub de redirect.
.EXAMPLE
  ./migrate-docs-deflate.ps1 -DryRun
  ./migrate-docs-deflate.ps1 -Wave 1
#>
param(
    [ValidateSet(1, 2, 3)]
    [int]$Wave = 1,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$DocsRoot = Join-Path $Root 'docs'
$ArchiveRoot = Join-Path $DocsRoot '_archive'

$rules = @{
    1 = @(
        @{ Pattern = '^PR_';                    Subdir = 'pr-summaries' }
        @{ Pattern = '^RESUMO_';               Subdir = 'sessions' }
        @{ Pattern = '^ANALISE_';              Subdir = 'analyses' }
        @{ Pattern = '^AVALIACAO_';            Subdir = 'evaluations' }
        @{ Pattern = '^IMPLEMENTACAO_';        Subdir = 'plans' }
        @{ Pattern = '^REESTRUTURACAO_';       Subdir = 'plans' }
        @{ Pattern = '^VALIDACAO_INCONSIST';   Subdir = 'plans' }
        @{ Pattern = '^PROPOSAL_';             Subdir = 'plans' }
        @{ Pattern = '^PLANO_MIGRACAO_';       Subdir = 'plans' }
    )
}

function New-RedirectStub {
    param(
        [string]$FileName,
        [string]$RelativeTarget
    )
    $title = $FileName -replace '\.md$', '' -replace '_', ' '
    return @"
---
title: "$title (arquivado)"
status: deprecated
redirect: $RelativeTarget
owner: docs-steward
updated: $(Get-Date -Format 'yyyy-MM-dd')
---

> **Documento arquivado** — não é fonte de verdade.
>
> Conteúdo movido para: [$RelativeTarget]($RelativeTarget)
>
> Use [00_INDEX.md](./00_INDEX.md) ou [INDEX.generated.md](./INDEX.generated.md) para navegar documentação ativa.
"@
}

if (-not $rules.ContainsKey($Wave)) {
    Write-Error "Wave $Wave not defined"
    exit 1
}

$moved = 0
$skipped = 0

foreach ($rule in $rules[$Wave]) {
    $targetDir = Join-Path $ArchiveRoot $rule.Subdir
    if (-not $DryRun) {
        New-Item -ItemType Directory -Force -Path $targetDir | Out-Null
    }

    Get-ChildItem -Path $DocsRoot -Filter '*.md' -File |
        Where-Object { $_.Name -match $rule.Pattern } |
        ForEach-Object {
            $dest = Join-Path $targetDir $_.Name
            $relTarget = "_archive/$($rule.Subdir)/$($_.Name)"

            if (Test-Path $dest) {
                Write-Warning "Skip $($_.Name) — already in archive"
                $skipped++
                return
            }

            if ($DryRun) {
                Write-Host "[dry-run] $($_.Name) -> $relTarget"
            } else {
                Move-Item -Path $_.FullName -Destination $dest -Force
                $stub = New-RedirectStub -FileName $_.Name -RelativeTarget $relTarget
                Set-Content -Path $_.FullName -Value $stub -Encoding UTF8
                Write-Host "Moved $($_.Name) -> $relTarget (+ stub)"
            }
            $moved++
        }
}

$remaining = (Get-ChildItem -Path $DocsRoot -Filter '*.md' -File).Count
Write-Host ""
Write-Host "Wave $Wave complete: moved=$moved skipped=$skipped dry_run=$($DryRun.IsPresent)"
Write-Host "MD files in docs/ root now: $remaining (target: <80 after wave 3)"
