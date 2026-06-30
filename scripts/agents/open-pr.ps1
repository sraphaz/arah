#Requires -Version 5.1
<#
.SYNOPSIS
  Prepara branch e PR estruturado (skill open-pr). Não faz merge.
.EXAMPLE
  ./open-pr.ps1 -Agent backend -Title "feat(api): endpoint X" -Issue 42
#>
param(
    [Parameter(Mandatory)]
    [string]$Agent,
    [Parameter(Mandatory)]
    [string]$Title,
    [int]$Issue = 0,
    [string[]]$Skills = @('run-tests', 'sync-docs'),
    [string]$Slug = ''
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

if (-not $Slug) {
    $Slug = ($Title -replace '[^a-zA-Z0-9]+', '-').Trim('-').ToLower()
    if ($Slug.Length -gt 40) { $Slug = $Slug.Substring(0, 40) }
}

$branch = "agent/$Agent-$Slug"
$bodyFile = Join-Path $Root '.agents/templates/pr-body.md'
$tempBody = Join-Path $env:TEMP "arah-pr-body-$([guid]::NewGuid().ToString('n')).md"

$template = Get-Content $bodyFile -Raw
$template = $template -replace '<!-- backend \| flutter \| web \| docs-steward \| release -->', $Agent
$template = $template -replace '<!-- run-tests, sync-docs, \.\.\. -->', ($Skills -join ', ')
if ($Issue -gt 0) { $template = $template -replace '#', "#$Issue" }
$template | Set-Content -Path $tempBody -Encoding UTF8

Push-Location $Root
try {
    $current = git rev-parse --abbrev-ref HEAD
    git checkout -b $branch 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Branch $branch já existe ou checkout falhou — continuando."
    }
    Write-Host "Branch: $branch"
    Write-Host "Body template: $tempBody"
    if (Get-Command gh -ErrorAction SilentlyContinue) {
        gh pr create --title $Title --body-file $tempBody --label "agent-task" 2>&1
    } else {
        Write-Host 'gh CLI não encontrado. Após push:'
        Write-Host "  gh pr create --title `"$Title`" --body-file `"$tempBody`""
    }
} finally {
    Pop-Location
}
