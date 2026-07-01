#Requires -Version 5.1
<#
.SYNOPSIS
  Exporta status operacional das fases a partir de Issues GitHub (JSON para agentes/CI).
.EXAMPLE
  ./export-phase-status.ps1 -Json
#>
param(
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
. (Join-Path $PSScriptRoot 'Get-PhaseQueue.ps1')

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

Push-Location $Root
try {
    $queue = Get-PhaseRoadmap -Root $Root
    $issues = gh issue list --state all --limit 500 --json number,title,state,body,labels,url | ConvertFrom-Json
    $phases = @()

    foreach ($item in $queue) {
        if ($item.id -notmatch '^FASE') { continue }

        $issue = $issues | Where-Object { Test-PhaseIssueMarker -Issue $_ -PhaseId $item.id } | Select-Object -First 1
        if (-not $issue) {
            $issue = $issues | Where-Object { $_.title -match [regex]::Escape($item.id) -and ($_.labels.name -contains 'epic/phase') } | Select-Object -First 1
        }

        $status = if ($item.status -eq 'completed') { 'completed' }
                  elseif ($issue -and $issue.state -eq 'OPEN') { 'in_progress' }
                  elseif ($issue -and $issue.state -eq 'CLOSED') { 'completed' }
                  elseif (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $item.id) { 'completed' }
                  else { 'pending' }

        $phases += [ordered]@{
            id           = $item.id
            title        = $item.title
            wave         = $item.wave
            priority     = $item.priority
            status       = $status
            issue_number = if ($issue) { $issue.number } else { $null }
            issue_url    = if ($issue) { $issue.url } else { $null }
            doc          = $item.doc
        }
    }

    $report = [ordered]@{
        source    = 'github-issues'
        timestamp = (Get-Date).ToUniversalTime().ToString('o')
        phases    = $phases
    }

    $outPath = Join-Path $Root '.github/phase-status.generated.json'
    $report | ConvertTo-Json -Depth 5 | Set-Content -Path $outPath -Encoding UTF8

    if ($Json) { $report | ConvertTo-Json -Depth 5 } else {
        Write-Host "Exported $($phases.Count) phases → $outPath"
    }
    exit 0
} finally {
    Pop-Location
}
