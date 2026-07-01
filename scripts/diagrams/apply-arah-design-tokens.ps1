#Requires -Version 5.1
<#
.SYNOPSIS
  Aplica tokens do design system Arah a SVGs exportados (forest palette).
#>
param(
    [string]$SvgDir = '',
    [string]$TokensFile = ''
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$Dir = if ($SvgDir) { $SvgDir } else { Join-Path $Root 'docs/architecture/diagrams/svg' }
$Tokens = if ($TokensFile) { $TokensFile } else { Join-Path $Root 'design-system/colors_and_type.css' }

if (-not (Test-Path $Dir)) {
    New-Item -ItemType Directory -Force -Path $Dir | Out-Null
    Write-Host "apply-arah-design-tokens: no SVG dir yet ($Dir)"
    exit 0
}

$css = if (Test-Path $Tokens) { Get-Content $Tokens -Raw } else { '' }
function Get-TokenColor {
    param([string]$Var)
    if ($css -match "(?m)$Var:\s*(#[0-9A-Fa-f]{3,8})") { return $Matches[1] }
    return $null
}

$bg = Get-TokenColor '--forest-950' ; if (-not $bg) { $bg = '#0E2117' }
$primary = Get-TokenColor '--forest-500' ; if (-not $primary) { $primary = '#4F956F' }
$secondary = Get-TokenColor '--forest-600' ; if (-not $secondary) { $secondary = '#377B57' }
$text = Get-TokenColor '--forest-100' ; if (-not $text) { $text = '#E2F1E8' }
$border = Get-TokenColor '--forest-700' ; if (-not $border) { $border = '#2B6246' }

$styleBlock = @"
<style type="text/css"><![CDATA[
  /* Arah design system — auto-applied */
  .node polygon, .node path, .node ellipse { fill: $primary; stroke: $border; }
  .edge path, .edge polygon { stroke: $secondary; }
  text { fill: $text; font-family: Geist, Sora, sans-serif; }
  .graph { background-color: $bg; }
]]></style>
"@

$count = 0
Get-ChildItem -Path $Dir -Filter '*.svg' -ErrorAction SilentlyContinue | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match 'Arah design system — auto-applied') { return }

    $replacements = @{
        '#ffffff' = $bg
        '#FFFFFF' = $bg
        '#1e90ff' = $primary
        '#0066cc' = $primary
        '#333333' = $text
        '#666666' = $secondary
    }
    foreach ($k in $replacements.Keys) {
        $content = $content.Replace($k, $replacements[$k])
    }

    if ($content -match '<svg[^>]*>') {
        $content = $content -replace '(<svg[^>]*>)', "`$1`n$styleBlock"
    }
    Set-Content -Path $_.FullName -Value $content -Encoding UTF8
    $count++
}

Write-Host "apply-arah-design-tokens: styled $count SVG(s) in $Dir"
exit 0
