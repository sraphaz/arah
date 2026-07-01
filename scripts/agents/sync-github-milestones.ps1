#Requires -Version 5.1
<#
.SYNOPSIS
  Sincroniza milestones de .github/milestones.yml com o repositório GitHub.
#>
param(
    [switch]$DryRun,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$File = Join-Path $Root '.github/milestones.yml'

if (-not (Test-Path $File)) {
    Write-Error "Missing $File"
    exit 1
}

if (-not $DryRun -and -not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

$items = @()
$current = $null
foreach ($line in Get-Content $File) {
    if ($line -match '^\s+-\s+title:\s*"(.+)"$') {
        if ($current) { $items += $current }
        $current = @{ title = $Matches[1]; description = ''; state = 'open' }
    } elseif ($current -and $line -match '^\s+description:\s*"(.+)"$') {
        $current.description = $Matches[1]
    } elseif ($current -and $line -match '^\s+state:\s*(.+)$') {
        $current.state = $Matches[1].Trim()
    }
}
if ($current) { $items += $current }

$results = @()
Push-Location $Root
try {
    $repo = if ($DryRun) { 'owner/repo' } else { gh repo view --json nameWithOwner -q .nameWithOwner }
    $existing = @()
    if (-not $DryRun) {
        $existing = gh api "repos/$repo/milestones?state=all&per_page=100" 2>$null | ConvertFrom-Json
        if (-not $existing) { $existing = @() }
    }

    foreach ($m in $items) {
        if ($DryRun) {
            $results += @{ title = $m.title; action = 'would-create-or-update' }
            continue
        }
        $found = $existing | Where-Object { $_.title -eq $m.title } | Select-Object -First 1
        if ($found) {
            gh api -X PATCH "repos/$repo/milestones/$($found.number)" `
                -f title="$($m.title)" -f description="$($m.description)" -f state="$($m.state)" | Out-Null
            $results += @{ title = $m.title; action = 'updated'; number = $found.number }
        } else {
            $created = gh api "repos/$repo/milestones" `
                -f title="$($m.title)" -f description="$($m.description)" | ConvertFrom-Json
            $results += @{ title = $m.title; action = 'created'; number = $created.number }
        }
    }
} finally {
    Pop-Location
}

$report = [ordered]@{ count = $items.Count; dry_run = [bool]$DryRun; milestones = $results }
if ($Json) { $report | ConvertTo-Json -Depth 4 } else { Write-Host "Milestones: $($items.Count) processed" }
exit 0
