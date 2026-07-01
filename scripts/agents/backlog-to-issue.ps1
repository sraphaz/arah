#Requires -Version 5.1
<#
.SYNOPSIS
  Cria issue GitHub a partir de fase (PHASE_QUEUE ou FASE*.md).
.EXAMPLE
  ./backlog-to-issue.ps1 -PhaseId FASE53
  ./backlog-to-issue.ps1 -SourceFile docs/backlog-api/FASE55.md -DryRun
#>
param(
    [string]$PhaseId = '',
    [string]$SourceFile = '',
    [switch]$DryRun,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
. (Join-Path $PSScriptRoot 'Get-PhaseQueue.ps1')

if (-not $PhaseId -and $SourceFile) {
    if ($SourceFile -match 'FASE(\d+)') { $PhaseId = "FASE$($Matches[1])" }
}

if (-not $PhaseId) {
    Write-Error 'Provide -PhaseId or -SourceFile with FASE reference'
    exit 1
}

$item = $null
try {
    $item = Get-PhaseQueueItem -Root $Root -PhaseId $PhaseId
} catch {
    $doc = if ($SourceFile) { $SourceFile } else { "docs/backlog-api/$PhaseId.md" }
    $title = $PhaseId
    if (Test-Path (Join-Path $Root $doc)) {
        $first = Get-Content (Join-Path $Root $doc) -TotalCount 5 | Where-Object { $_ -match '^#\s+' } | Select-Object -First 1
        if ($first -match '^#\s+(.+)$') { $title = $Matches[1].Trim() }
    }
    $item = @{
        id = $PhaseId
        title = $title
        area = 'area/planning'
        doc = $doc
        priority = 'P1'
        blocked_by = @()
    }
}

if ($SourceFile -and (Test-Path (Join-Path $Root $SourceFile))) {
    $item.doc = $SourceFile.Replace('\', '/')
}

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

Push-Location $Root
try {
    if (Test-PhaseOpenIssue -Root $Root -PhaseId $item.id) {
        $msg = "Issue já aberta para $($item.id)"
        if ($Json) { @{ ok = $false; message = $msg; phase = $item.id } | ConvertTo-Json } else { Write-Host $msg }
        exit 0
    }

    $body = New-PhaseIssueBody -Item $item -Root $Root -Source 'backlog-to-issue'
    $labels = Get-PhaseIssueLabels -Item $item
    $title = "[Epic] $($item.title)"

    $result = [ordered]@{
        phase   = $item.id
        title   = $title
        labels  = $labels
        doc     = $item.doc
        dry_run = [bool]$DryRun
    }

    if ($DryRun) {
        $result.message = "Would create: $title"
        if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4 }
        exit 0
    }

    $labelParams = @()
    foreach ($l in $labels) { $labelParams += '--label'; $labelParams += $l }
    $issueUrl = gh issue create --title $title --body $body @labelParams 2>&1
    if ($LASTEXITCODE -ne 0) { throw "gh issue create failed: $issueUrl" }

    $repo = gh repo view --json nameWithOwner -q .nameWithOwner
        $milestones = gh api "repos/$repo/milestones?state=open" | ConvertFrom-Json
        $ms = $milestones | Where-Object { $_.title -eq $milestoneTitle } | Select-Object -First 1
        if ($ms) {
            gh api -X PATCH "repos/$repo/issues/$issueNum" -f milestone=$ms.number | Out-Null
        }
    }

    $cfg = Get-ProjectConfig -Root $Root
    if ($cfg -and $cfg.project_number -gt 0 -and $issueUrl) {
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
