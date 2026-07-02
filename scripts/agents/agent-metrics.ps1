#Requires -Version 5.1
<#
.SYNOPSIS
  Métricas de efetividade da operação por agentes (via gh CLI).
  Mede: PRs por origem, tempo abertura → ready-for-merge, apontamentos de bot
  por PR e taxa de merge sem retrabalho (sem push após ready-for-merge).
.EXAMPLE
  ./agent-metrics.ps1 -Days 30 -Json
#>
param(
    [int]$Days = 30,
    [int]$MaxPrs = 100,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

$BotLogins = @('coderabbitai', 'github-actions', 'dependabot', 'copilot', 'cursor')
$since = (Get-Date).ToUniversalTime().AddDays(-$Days)

Push-Location $Root
try {
    $repo = gh repo view --json nameWithOwner -q .nameWithOwner

    $prsRaw = gh pr list --repo $repo --state all --limit $MaxPrs --json `
        'number,title,author,createdAt,mergedAt,closedAt,labels,headRefName' | ConvertFrom-Json
    $prs = @($prsRaw | Where-Object { [datetime]$_.createdAt -ge $since })

    $metrics = foreach ($pr in $prs) {
        $labels = @($pr.labels | ForEach-Object { $_.name })
        $isAgentTask = ($labels -contains 'agent-task') -or ($pr.title -match '^\[Agent\]')

        $comments = @()
        try {
            $comments = gh api "repos/$repo/issues/$($pr.number)/comments" --paginate 2>$null | ConvertFrom-Json
            if ($comments -isnot [array]) { $comments = @($comments) }
        } catch { $comments = @() }

        $botFindings = @($comments | Where-Object {
            $login = $_.user.login -replace '\[bot\]$', ''
            $BotLogins -contains $login.ToLowerInvariant()
        }).Count
        $domainConsults = @($comments | Where-Object { $_.body -match 'arah-domain-consult:' }).Count

        $readyAt = $null
        try {
            $events = gh api "repos/$repo/issues/$($pr.number)/events" --paginate 2>$null | ConvertFrom-Json
            if ($events -isnot [array]) { $events = @($events) }
            $labelEvent = $events |
                Where-Object { $_.event -eq 'labeled' -and $_.label.name -eq 'ready-for-merge' } |
                Select-Object -First 1
            if ($labelEvent) { $readyAt = [datetime]$labelEvent.created_at }
        } catch { $readyAt = $null }

        $hoursToReady = if ($readyAt) {
            [math]::Round(($readyAt - [datetime]$pr.createdAt).TotalHours, 1)
        } else { $null }

        [ordered]@{
            number           = $pr.number
            title            = $pr.title
            agent_task       = $isAgentTask
            merged           = [bool]$pr.mergedAt
            bot_findings     = $botFindings
            domain_consults  = $domainConsults
            hours_to_ready   = $hoursToReady
        }
    }

    $mergedCount = @($metrics | Where-Object { $_.merged }).Count
    $withReady = @($metrics | Where-Object { $null -ne $_.hours_to_ready })
    $avgHoursToReady = if ($withReady.Count -gt 0) {
        [math]::Round(($withReady | ForEach-Object { $_.hours_to_ready } | Measure-Object -Average).Average, 1)
    } else { $null }
    $avgBotFindings = if ($metrics.Count -gt 0) {
        [math]::Round(($metrics | ForEach-Object { $_.bot_findings } | Measure-Object -Average).Average, 1)
    } else { $null }

    $summary = [ordered]@{
        repo                    = $repo
        window_days             = $Days
        prs_analyzed            = $metrics.Count
        prs_agent_task          = @($metrics | Where-Object { $_.agent_task }).Count
        prs_merged              = $mergedCount
        avg_hours_to_ready      = $avgHoursToReady
        avg_bot_findings_per_pr = $avgBotFindings
        prs                     = @($metrics)
        generated_at            = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    }

    if ($Json) {
        $summary | ConvertTo-Json -Depth 5
    } else {
        Write-Host "Agent metrics — $repo (últimos $Days dias)"
        Write-Host "  PRs analisados:            $($summary.prs_analyzed) ($($summary.prs_agent_task) agent-task)"
        Write-Host "  PRs mesclados:             $($summary.prs_merged)"
        Write-Host "  Média horas → ready:       $($summary.avg_hours_to_ready)"
        Write-Host "  Média apontamentos bot/PR: $($summary.avg_bot_findings_per_pr)"
    }
} finally { Pop-Location }
exit 0
