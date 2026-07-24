#Requires -Version 5.1
<#
.SYNOPSIS
  Abre issue Agent Task para a próxima fase desbloqueada (PHASE_QUEUE → GitHub Issue).
.EXAMPLE
  ./next-phase.ps1 -DryRun
#>
param(
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
    & (Join-Path $PSScriptRoot 'sync-github-labels.ps1') | Out-Null

    $queue = Get-PhaseQueue -Root $Root
    $openIssues = gh issue list --state open --limit 200 --json title,labels,body | ConvertFrom-Json

    $selected = $null
    foreach ($item in $queue) {
        # next-phase só abre épicos FASE* (sustentação/comunitário).
        # Trilhas transversais (TI-*, kind: track) e maintenance ficam fora —
        # squad paralelo / planner; evita Epic TI-0 enquanto FASE* pendente.
        if (-not (Test-PhaseQueueItemEligibleForNextPhase -Item $item)) { continue }
        if ($item.status -eq 'completed') { continue }

        $blocked = $false
        foreach ($dep in $item.blocked_by) {
            if (-not (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $dep)) {
                # Qualquer dep (FASE*, TI-*, doc-*) bloqueia quando elegível.
                $blocked = $true
                break
            }
        }
        if ($blocked) { continue }
        if (Test-PhaseOpenIssue -Root $Root -PhaseId $item.id -OpenIssues $openIssues) { continue }
        if (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $item.id) { continue }

        $selected = $item
        break
    }

    if (-not $selected) {
        $msg = 'Nenhuma fase desbloqueada pendente de issue (fila vazia ou issues já abertas).'
        if ($Json) { @{ message = $msg } | ConvertTo-Json } else { Write-Host $msg }
        exit 0
    }

    $body = New-PhaseIssueBody -Item $selected -Root $Root -Source 'next-phase'
    $labels = Get-PhaseIssueLabels -Item $selected
    $title = "[Epic] $($selected.title)"

    $result = [ordered]@{
        phase   = $selected.id
        title   = $title
        labels  = $labels
        area    = $selected.area
        doc     = $selected.doc
        dry_run = [bool]$DryRun
    }

    if ($DryRun) {
        $result.message = "Would create issue: $title"
        if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4; Write-Host $result.message }
        exit 0
    }

    $labelParams = @()
    foreach ($l in $labels) { $labelParams += '--label'; $labelParams += $l }
    $issueUrl = gh issue create --title $title --body $body @labelParams 2>&1
    if ($LASTEXITCODE -ne 0) { throw "gh issue create failed: $issueUrl" }

    $repo = gh repo view --json nameWithOwner -q .nameWithOwner
    $milestoneTitle = Resolve-MilestoneForWave -Root $Root -Wave $selected.wave
    if ($milestoneTitle -and $issueUrl -match '/issues/(\d+)') {
        $issueNum = $Matches[1]
        $milestones = gh api "repos/$repo/milestones?state=open" | ConvertFrom-Json
        $ms = $milestones | Where-Object { $_.title -eq $milestoneTitle } | Select-Object -First 1
        if ($ms) {
            gh api -X PATCH "repos/$repo/issues/$issueNum" -f milestone=$ms.number | Out-Null
        }
    }

    $cfg = Get-ProjectConfig -Root $Root
    if ($cfg -and $cfg.project_number -gt 0) {
        $owner = ($repo -split '/')[0]
        Add-IssueToGitHubProject -IssueUrl $issueUrl -ProjectNumber $cfg.project_number -Owner $owner | Out-Null
    }

    $result.issue = $issueUrl
    $result.message = "Issue criada: $issueUrl"

    if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4; Write-Host $result.message }
    exit 0
} finally {
    Pop-Location
}
