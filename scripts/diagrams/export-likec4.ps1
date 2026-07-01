#Requires -Version 5.1
<#
.SYNOPSIS
  Exporta diagramas LikeC4 → PNG (+ dot → SVG quando Graphviz disponível).
#>
param(
    [string]$Theme = 'dark',
    [string]$LikeC4Dir = '',
    [switch]$SkipPng,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$LikeRoot = if ($LikeC4Dir) { $LikeC4Dir } else { Join-Path $Root 'docs/architecture/likec4' }
$OutPng = Join-Path $Root 'docs/architecture/diagrams/png'
$OutSvg = Join-Path $Root 'docs/architecture/diagrams/svg'
$OutDot = Join-Path $Root 'docs/architecture/diagrams/dot'
$results = @()

foreach ($d in @($OutPng, $OutSvg, $OutDot)) {
    New-Item -ItemType Directory -Force -Path $d | Out-Null
}

if (-not (Test-Path (Join-Path $LikeRoot 'arah.likec4'))) {
    Write-Error "LikeC4 model not found in $LikeRoot"
    exit 1
}

Push-Location $LikeRoot
try {
    if (-not $SkipPng) {
        try {
            $themeArg = if ($Theme -eq 'dark') { '--dark' } else { '--light' }
            npx --yes likec4@latest export png -o $OutPng $themeArg . 2>&1 | Out-Null
            if ($LASTEXITCODE -ne 0) { throw "likec4 export png exit $LASTEXITCODE" }
            $results += @{ step = 'likec4-png'; ok = $true; out = $OutPng }
        } catch {
            Write-Warning "likec4 PNG export skipped (Playwright/CI?): $($_.Exception.Message)"
            $results += @{ step = 'likec4-png'; ok = $false; detail = $_.Exception.Message }
        }
    }

    try {
        npx --yes likec4@latest codegen dot -o $OutDot . 2>&1 | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "codegen dot exit $LASTEXITCODE" }
        $results += @{ step = 'likec4-dot'; ok = $true; out = $OutDot }

        $dotExe = Get-Command dot -ErrorAction SilentlyContinue
        if ($dotExe) {
            Get-ChildItem -Path $OutDot -Filter '*.dot' -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
                $svgName = [IO.Path]::ChangeExtension($_.Name, '.svg')
                $svgPath = Join-Path $OutSvg $svgName
                & dot -Tsvg $_.FullName -o $svgPath 2>&1 | Out-Null
            }
            $results += @{ step = 'dot-svg'; ok = $true; out = $OutSvg }
        } else {
            Write-Warning 'Graphviz (dot) not in PATH — SVG via dot skipped. Install graphviz or use PNG.'
            $results += @{ step = 'dot-svg'; ok = $false; detail = 'dot not found' }
        }
    } catch {
        Write-Warning "likec4 dot codegen failed: $($_.Exception.Message)"
        $results += @{ step = 'likec4-dot'; ok = $false; detail = $_.Exception.Message }
    }
} finally {
    Pop-Location
}

& (Join-Path $PSScriptRoot 'apply-arah-design-tokens.ps1') -SvgDir $OutSvg | Out-Null

$allOk = ($results | Where-Object { $_.step -match 'likec4-dot|dot-svg' -and -not $_.ok }).Count -eq 0
$report = [ordered]@{ steps = $results; ok = $allOk }
if ($Json) { $report | ConvertTo-Json -Depth 4 } else { $report | ConvertTo-Json -Depth 4 }

if (-not $allOk -and ($results | Where-Object { $_.ok }).Count -eq 0) { exit 1 }
exit 0
