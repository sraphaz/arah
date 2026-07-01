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
    $repo = Get-GhRepoFullName -Root $Root
    $issues = Get-AllRepoIssuesRest -Repo $repo -Label 'epic/phase'
    if ($issues.Count -lt 40) {
        $issues = Get-AllRepoIssuesRest -Repo $repo
    }
    $phases = @()
    $counts = @{ completed = 0; in_progress = 0; pending = 0; blocked = 0 }

    foreach ($item in $queue) {
        if ($item.id -notmatch '^FASE') { continue }

        $issue = Get-PhaseIssueAnyState -Root $Root -PhaseId $item.id -AllIssues $issues

        $status = if ($item.status -eq 'completed') { 'completed' }
                  elseif ($issue -and $issue.state -eq 'OPEN') { 'in_progress' }
                  elseif ($issue -and $issue.state -eq 'CLOSED') { 'completed' }
                  elseif (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $item.id) { 'completed' }
                  else { 'pending' }

        $blocked = $false
        foreach ($dep in $item.blocked_by) {
            if ($dep -match '^FASE' -and -not (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $dep)) {
                $blocked = $true
                break
            }
        }
        if ($blocked -and $status -eq 'pending') { $status = 'blocked'; $counts.blocked++ }
        elseif ($status -eq 'completed') { $counts.completed++ }
        elseif ($status -eq 'in_progress') { $counts.in_progress++ }
        else { $counts.pending++ }

        $phases += [ordered]@{
            id           = $item.id
            title        = $item.title
            wave         = $item.wave
            priority     = $item.priority
            status       = $status
            issue_number = if ($issue) { $issue.number } else { $null }
            issue_url    = if ($issue) { $issue.url } else { $null }
            issue_state  = if ($issue) { $issue.state } else { $null }
            doc          = $item.doc
        }
    }

    $report = [ordered]@{
        source    = 'github-issues'
        timestamp = (Get-Date).ToUniversalTime().ToString('o')
        project   = 'https://github.com/users/sraphaz/projects/3'
        summary   = [ordered]@{
            total       = $phases.Count
            completed   = $counts.completed
            in_progress = $counts.in_progress
            pending     = $counts.pending
            blocked     = $counts.blocked
        }
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
