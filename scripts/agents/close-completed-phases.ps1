#Requires -Version 5.1
<#
.SYNOPSIS
  Fecha issues de fases marcadas como completed (PHASE_ROADMAP_META) na ordem de dependência.
.EXAMPLE
  ./close-completed-phases.ps1 -Json
  ./close-completed-phases.ps1 -PhaseId FASE53
#>
param(
    [string]$PhaseId = '',
    [switch]$DryRun,
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
    $repo = Get-GhRepoFullName -Root $Root
    $roadmap = Get-PhaseRoadmap -Root $Root
    $issues = Get-AllRepoIssuesRest -Repo $repo -Label 'epic/phase'

    $candidates = @($roadmap | Where-Object {
        $_.id -match '^FASE' -and (Test-PhaseCompleteFromMeta -Root $Root -Id $_.id)
    })

    if ($PhaseId) {
        $candidates = @($candidates | Where-Object { $_.id -eq $PhaseId })
    }

    $ordered = $candidates | Sort-Object {
        if ($_.id -match '^FASE(\d+)') { [int]$Matches[1] } else { 9999 }
    }
    $closed = @()
    $skipped = @()

    foreach ($item in $ordered) {
        $issue = Get-PhaseIssueAnyState -Root $Root -PhaseId $item.id -AllIssues $issues
        if (-not $issue) {
            $skipped += @{ phase = $item.id; reason = 'no_issue' }
            continue
        }
        if ($issue.state -eq 'CLOSED') {
            $skipped += @{ phase = $item.id; issue = $issue.number; reason = 'already_closed' }
            continue
        }

        if ($DryRun) {
            $closed += @{ phase = $item.id; issue = $issue.number; action = 'would_close' }
            continue
        }

        $comment = "Fase **$($item.id)** concluída — critérios de aceite atendidos. Fechada por ``close-completed-phases``."
        $ok = Close-IssueRest -Repo $repo -IssueNumber $issue.number -Comment $comment
        if ($ok) {
            $closed += @{ phase = $item.id; issue = $issue.number; action = 'closed' }
        } else {
            $skipped += @{ phase = $item.id; issue = $issue.number; reason = 'close_failed' }
        }
        Start-Sleep -Milliseconds 400
    }

    $report = [ordered]@{
        closed  = $closed
        skipped = $skipped
        count   = $closed.Count
        dry_run = [bool]$DryRun
    }
    if ($Json) { $report | ConvertTo-Json -Depth 5 } else { $report | ConvertTo-Json -Depth 5 }
    exit 0
} finally {
    Pop-Location
}
