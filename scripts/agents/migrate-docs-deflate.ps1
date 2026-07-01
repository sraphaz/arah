#Requires -Version 5.1
<#
.SYNOPSIS
  Desinfla docs/ movendo artefatos para pastas semânticas ou _archive/ com stub de redirect.
.EXAMPLE
  ./migrate-docs-deflate.ps1 -DryRun
  ./migrate-docs-deflate.ps1 -Wave 1
  ./migrate-docs-deflate.ps1 -Wave 2
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

function Get-SemanticFolder {
    param([string]$FileName)

    if ($FileName -eq '00_INDEX.md') { return $null }
    if ($FileName -match '^10_|^11_|^14_') { return 'architecture' }
    if ($FileName -match '^12_|^13_|^20_|^23_|^60_') { return 'backend' }
    if ($FileName -match '^21_|^22_') { return 'governance' }
    if ($FileName -match '^30_MODERATION') { return 'governance' }
    if ($FileName -match '^41_') { return 'governance' }
    if ($FileName -match '^24_|^25_|^26_|^28_|^29_|^30_FLUTTER|^31_FLUTTER|^32_FLUTTER|^33_FLUTTER|^34_|^38_FLUTTER') {
        return 'frontend/flutter'
    }
    if ($FileName -match '^42_') { return 'frontend/web' }
    if ($FileName -match '^31_ADMIN|^33_ADMIN|^32_TRACEABILITY|^50_|^51_') { return 'ops' }
    if ($FileName -match '^01_|^02_|^03_|^04_|^05_|^27_|^35_|^36_|^37_|^38_MAPA|^39_|^61_|^70_') {
        return 'product'
    }
    if ($FileName -match '^\d{2}_') { return 'product' }
    return $null
}

function New-RedirectStub {
    param(
        [string]$FileName,
        [string]$RelativeTarget,
        [ValidateSet('archived', 'relocated')]
        [string]$Kind = 'archived'
    )
    $title = $FileName -replace '\.md$', '' -replace '_', ' '
    if ($Kind -eq 'relocated') {
        return @"
---
title: "$title (relocado)"
status: deprecated
redirect: $RelativeTarget
owner: docs-steward
updated: $(Get-Date -Format 'yyyy-MM-dd')
---

> **Documento relocado** — conteúdo ativo em subpasta semântica.
>
> Novo caminho: [$RelativeTarget]($RelativeTarget)
>
> Use [00_INDEX.md](./00_INDEX.md) ou [INDEX.generated.md](./INDEX.generated.md) para navegar.
"@
    }
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

function Invoke-Wave1 {
    param([switch]$DryRun)

    $moved = 0
    $skipped = 0

    foreach ($rule in $rules[1]) {
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

                if ((Get-Content $_.FullName -Raw) -match 'status:\s*deprecated') {
                    Write-Warning "Skip $($_.Name) — already stub"
                    $skipped++
                    return
                }

                if ($DryRun) {
                    Write-Host "[dry-run] $($_.Name) -> $relTarget"
                } else {
                    Move-Item -Path $_.FullName -Destination $dest -Force
                    $stub = New-RedirectStub -FileName $_.Name -RelativeTarget $relTarget -Kind 'archived'
                    Set-Content -Path $_.FullName -Value $stub -Encoding UTF8
                    Write-Host "Moved $($_.Name) -> $relTarget (+ stub)"
                }
                $moved++
            }
    }

    return @{ moved = $moved; skipped = $skipped }
}

function Invoke-Wave2 {
    param([switch]$DryRun)

    $moved = 0
    $skipped = 0

    Get-ChildItem -Path $DocsRoot -Filter '*.md' -File |
        Where-Object { $_.Name -match '^\d{2}_' } |
        ForEach-Object {
            $folder = Get-SemanticFolder -FileName $_.Name
            if (-not $folder) {
                return
            }

            if ((Get-Content $_.FullName -Raw) -match 'status:\s*deprecated') {
                Write-Warning "Skip $($_.Name) — already stub"
                $skipped++
                return
            }

            $targetDir = Join-Path $DocsRoot ($folder -replace '/', [IO.Path]::DirectorySeparatorChar)
            $dest = Join-Path $targetDir $_.Name
            $relTarget = "$($folder -replace '\\', '/')/$($_.Name)"

            if (Test-Path $dest) {
                Write-Warning "Skip $($_.Name) — already at $relTarget"
                $skipped++
                return
            }

            if ($DryRun) {
                Write-Host "[dry-run] $($_.Name) -> $relTarget"
            } else {
                New-Item -ItemType Directory -Force -Path $targetDir | Out-Null
                Move-Item -Path $_.FullName -Destination $dest -Force
                $stub = New-RedirectStub -FileName $_.Name -RelativeTarget $relTarget -Kind 'relocated'
                Set-Content -Path $_.FullName -Value $stub -Encoding UTF8
                Write-Host "Moved $($_.Name) -> $relTarget (+ stub)"
            }
            $moved++
        }

    return @{ moved = $moved; skipped = $skipped }
}

if ($Wave -eq 1) {
    $result = Invoke-Wave1 -DryRun:$DryRun
} elseif ($Wave -eq 2) {
    $result = Invoke-Wave2 -DryRun:$DryRun
} else {
    Write-Error "Wave $Wave not defined"
    exit 1
}

$remaining = (Get-ChildItem -Path $DocsRoot -Filter '*.md' -File).Count
Write-Host ""
Write-Host "Wave $Wave complete: moved=$($result.moved) skipped=$($result.skipped) dry_run=$($DryRun.IsPresent)"
Write-Host "MD files in docs/ root now: $remaining (stubs incl.; target: <80 ativos após wave 3)"
