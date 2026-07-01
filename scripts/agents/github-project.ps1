#Requires -Version 5.1
<#
.SYNOPSIS
  GitHub Project v2 + sync da fila PHASE_QUEUE para Issues.
.EXAMPLE
  ./github-project.ps1 ensure
  ./github-project.ps1 sync-queue -DryRun
  ./github-project.ps1 status
#>
param(
    [Parameter(Position = 0)]
    [ValidateSet('ensure', 'sync-queue', 'status', 'help')]
    [string]$Command = 'help',
    [switch]$DryRun,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
. (Join-Path $PSScriptRoot 'Get-PhaseQueue.ps1')

function Show-ProjectHelp {
    @"
github-project — GitHub-native backlog (Issues + Project v2)

  ensure      Cria project (se não existir) e imprime project_number
  sync-queue  Abre issues para fases desbloqueadas sem issue aberta
  status      Lista issues de épico abertas (arah-phase-epic)

Configure project_number em .github/project/arah-sustentacao.yml após ensure.
"@
}

if ($Command -eq 'help') { Show-ProjectHelp; exit 0 }

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

Push-Location $Root
try {
    $repo = gh repo view --json nameWithOwner -q .nameWithOwner
    $owner = ($repo -split '/')[0]

    switch ($Command) {
        'ensure' {
            & (Join-Path $PSScriptRoot 'sync-github-labels.ps1') | Out-Null
            & (Join-Path $PSScriptRoot 'sync-github-milestones.ps1') | Out-Null

            $cfg = Get-ProjectConfig -Root $Root
            $title = if ($cfg) { $cfg.title } else { 'Arah — Sustentação (F52–61)' }

            try {
                $projects = gh project list --owner $owner --limit 50 --format json 2>$null | ConvertFrom-Json
                $existing = $null
                if ($projects -and $projects.projects) {
                    $existing = $projects.projects | Where-Object { $_.title -eq $title } | Select-Object -First 1
                }

                if ($existing) {
                    $num = $existing.number
                    Write-Host "Project exists: #$num — $title"
                    Write-Host "Set project_number: $num in .github/project/arah-sustentacao.yml"
                    if ($Json) { @{ project_number = $num; title = $title; url = $existing.url } | ConvertTo-Json }
                    exit 0
                }

                if ($DryRun) {
                    Write-Host "Would create project: $title"
                    exit 0
                }

                $created = gh project create --owner $owner --title $title --format json | ConvertFrom-Json
                Write-Host "Created project #$($created.number): $($created.url)"
                Write-Host "Add to .github/project/arah-sustentacao.yml: project_number: $($created.number)"
                if ($Json) { $created | ConvertTo-Json }
                exit 0
            } catch {
                Write-Warning "gh project unavailable (scope read:project / project:write?). Create project manually in GitHub UI."
                Write-Host "Labels/milestones synced. Set project_number in .github/project/arah-sustentacao.yml when ready."
                if ($Json) { @{ warning = 'project_scope_missing'; title = $title } | ConvertTo-Json }
                exit 0
            }
        }

        'sync-queue' {
            & (Join-Path $PSScriptRoot 'sync-github-labels.ps1') | Out-Null
            $queue = Get-PhaseQueue -Root $Root
            $openIssues = gh issue list --state open --limit 200 --json title,body,labels | ConvertFrom-Json
            $created = @()

            foreach ($item in $queue) {
                if ($item.status -eq 'completed') { continue }

                $blocked = $false
                foreach ($dep in $item.blocked_by) {
                    if (-not (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $dep)) {
                        if ($dep -match '^FASE') { $blocked = $true; break }
                    }
                }
                if ($blocked) { continue }
                if (Test-PhaseOpenIssue -Root $Root -PhaseId $item.id -OpenIssues $openIssues) { continue }
                if ($item.id -match '^FASE' -and (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $item.id)) { continue }

                if ($DryRun) {
                    $created += @{ phase = $item.id; action = 'would-create' }
                    continue
                }

                $params = @{ PhaseId = $item.id; Json = $true }
                $out = & (Join-Path $PSScriptRoot 'backlog-to-issue.ps1') @params | ConvertFrom-Json
                $created += @{ phase = $item.id; issue = $out.issue; ok = [bool]$out.issue }
            }

            $report = [ordered]@{ created = $created; count = $created.Count; dry_run = [bool]$DryRun }
            if ($Json) { $report | ConvertTo-Json -Depth 4 } else { $report | ConvertTo-Json -Depth 4 }
            exit 0
        }

        'status' {
            $issues = gh issue list --state all --limit 200 --json number,title,state,labels,url,body | ConvertFrom-Json
            $epics = @($issues | Where-Object {
                $_.body -match 'arah-phase-epic' -or ($_.labels.name -contains 'epic/phase')
            })

            $report = [ordered]@{
                total_epics = $epics.Count
                open        = @($epics | Where-Object { $_.state -eq 'OPEN' }).Count
                closed      = @($epics | Where-Object { $_.state -eq 'CLOSED' }).Count
                items       = @($epics | ForEach-Object {
                    @{
                        number = $_.number
                        title  = $_.title
                        state  = $_.state
                        url    = $_.url
                    }
                })
            }
            if ($Json) { $report | ConvertTo-Json -Depth 5 } else { $report | ConvertTo-Json -Depth 5 }
            exit 0
        }
    }
} finally {
    Pop-Location
}
