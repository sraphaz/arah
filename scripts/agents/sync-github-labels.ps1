#Requires -Version 5.1
<#
.SYNOPSIS
  Sincroniza labels de .github/labels.yml com o repositório GitHub.
.EXAMPLE
  ./sync-github-labels.ps1
  ./sync-github-labels.ps1 -DryRun
#>
param(
    [switch]$DryRun,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$LabelsFile = Join-Path $Root '.github/labels.yml'

if (-not (Test-Path $LabelsFile)) {
    Write-Error "Missing $LabelsFile"
    exit 1
}

if (-not $DryRun -and -not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

$labels = @()
$inLabels = $false
foreach ($line in Get-Content $LabelsFile) {
    if ($line -match '^labels:\s*$') { $inLabels = $true; continue }
    if (-not $inLabels) { continue }
    if ($line -match '^\s+-\s+name:\s*(.+)$') {
        $labels += @{ name = $Matches[1].Trim(); color = ''; description = '' }
    } elseif ($labels.Count -gt 0 -and $line -match '^\s+color:\s*(.+)$') {
        $labels[-1].color = $Matches[1].Trim()
    } elseif ($labels.Count -gt 0 -and $line -match '^\s+description:\s*(.+)$') {
        $labels[-1].description = $Matches[1].Trim().Trim('"')
    }
}

$results = @()
Push-Location $Root
try {
    foreach ($label in $labels) {
        if ($DryRun) {
            $results += @{ name = $label.name; action = 'would-sync' }
            continue
        }
        gh label create $label.name --color $label.color --description $label.description --force 2>&1 | Out-Null
        $results += @{ name = $label.name; action = 'synced'; ok = ($LASTEXITCODE -eq 0) }
    }
} finally {
    Pop-Location
}

$report = [ordered]@{
    count   = $labels.Count
    dry_run = [bool]$DryRun
    labels  = $results
    ok      = ($results | Where-Object { $_.ok -eq $false }).Count -eq 0
}

if ($Json) { $report | ConvertTo-Json -Depth 4 } else {
    Write-Host "Synced $($labels.Count) label(s)"
    if (-not $report.ok -and -not $DryRun) { exit 1 }
}
exit 0
