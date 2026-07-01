#Requires -Version 5.1

<#

.SYNOPSIS

  GitHub Project v2 + sync da fila PHASE_QUEUE para Issues e Project board.

.EXAMPLE

  ./github-project.ps1 bootstrap

  ./github-project.ps1 sync-queue

  ./github-project.ps1 sync-project

  ./github-project.ps1 reconcile

#>

param(

    [Parameter(Position = 0)]

    [ValidateSet('bootstrap', 'ensure', 'sync-backlog', 'sync-queue', 'sync-project', 'reconcile', 'status', 'help')]

    [string]$Command = 'help',

    [switch]$DryRun,

    [switch]$Json

)



$ErrorActionPreference = 'Stop'

$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

. (Join-Path $PSScriptRoot 'Get-PhaseQueue.ps1')

. (Join-Path $PSScriptRoot 'GitHub-ProjectApi.ps1')



function Show-ProjectHelp {

    @"

github-project — GitHub-native backlog (Issues + Project v2)



  bootstrap     Labels, milestones, project, views, issues, board (pipeline completo)

  ensure        Cria project (se não existir) e grava project_number no YAML

  sync-backlog   Cria issues para todo o roadmap (49 fases + itens da fila)
  sync-queue    Abre issues para fases desbloqueadas sem issue aberta

  sync-project  Adiciona épicos ao Project e atualiza coluna Status

  reconcile     Normaliza issues [Agent] FASE* → [Epic] com labels/milestone

  status        Lista issues de épico (arah-phase-epic)



Requer gh com scope project: gh auth refresh -h github.com -s project,read:project

"@

}



function Get-ProjectColumnsFromConfig {

    param([string]$Root)

    $path = Join-Path $Root '.github/project/arah-sustentacao.yml'

    if (-not (Test-Path $path)) { return @('Backlog', 'Ready', 'In Progress', 'Review', 'Done') }

    $cols = @()

    $inCols = $false

    foreach ($line in Get-Content $path) {

        if ($line -match '^columns:') { $inCols = $true; continue }

        if ($inCols -and $line -match '^\s+-\s+(.+)$') { $cols += $Matches[1].Trim() }

        elseif ($inCols -and $line -match '^\w') { break }

    }

    if ($cols.Count -eq 0) { return @('Backlog', 'Ready', 'In Progress', 'Review', 'Done') }

    return $cols

}



function Get-ProjectViewsFromConfig {

    param([string]$Root)

    $path = Join-Path $Root '.github/project/arah-sustentacao.yml'

    if (-not (Test-Path $path)) {

        return @('Kanban — Sustentação', 'Tabela — Por onda', 'Tabela — Por área')

    }

    $views = @()

    $inViews = $false

    foreach ($line in Get-Content $path) {

        if ($line -match '^views:') { $inViews = $true; continue }

        if ($inViews -and $line -match '^\s+-\s+name:\s*"(.+)"') { $views += $Matches[1] }

        elseif ($inViews -and $line -match '^[a-z_]+:') { break }

    }

    if ($views.Count -eq 0) {

        return @('Kanban — Sustentação', 'Tabela — Por onda', 'Tabela — Por área')

    }

    return $views

}



function Invoke-ProjectEnsure {

    param([switch]$DryRun, [switch]$Json)



    & (Join-Path $PSScriptRoot 'sync-github-labels.ps1') | Out-Null

    & (Join-Path $PSScriptRoot 'sync-github-milestones.ps1') | Out-Null



    $cfg = Get-ProjectConfig -Root $Root

    $title = if ($cfg) { $cfg.title } else { 'Arah — Sustentação (F52–61)' }

    $repo = gh repo view --json nameWithOwner,url -q '{nameWithOwner: .nameWithOwner, url: .url}'

    $repoObj = $repo | ConvertFrom-Json

    $owner = ($repoObj.nameWithOwner -split '/')[0]



    $projectNumber = if ($cfg -and $cfg.project_number -gt 0) { $cfg.project_number } else { 0 }

    if ($projectNumber -le 0) {
        $boot = Ensure-ProjectV2Bootstrap -Root $Root -Title $title -DryRun:$DryRun -Json:$Json
        if ($boot.project_number) { $projectNumber = [int]$boot.project_number }
        if ($projectNumber -le 0) {
            return $boot
        }
    } elseif (-not $DryRun) {
        $existing = Find-ProjectV2ByTitle -Owner $owner -Title $title
        if ($existing) {
            Set-ProjectConfigInYaml -Root $Root -ProjectNumber ([int]$existing.number) -ProjectUrl $existing.url | Out-Null
        }
    }

    $result = [ordered]@{

        project_number = $projectNumber

        title          = $title

        repo           = $repoObj.nameWithOwner

        dry_run        = [bool]$DryRun

    }

    $result | ConvertTo-Json

}



function Invoke-SyncBacklogIssues {
    <#
    .SYNOPSIS
      Cria issues GitHub para todo item do roadmap sem issue (inclui fases concluídas → issue fechada).
    #>
    param([switch]$DryRun, [switch]$Json)

    $roadmap = Get-PhaseRoadmap -Root $Root
    $queueExtras = Get-PhaseQueue -Root $Root | Where-Object { $_.id -notmatch '^FASE' }
    $allItems = @($roadmap) + @($queueExtras)

    $rawIssues = gh issue list --state all --limit 500 --json number,title,state,body,labels,url 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Não foi possível listar issues (rate limit?) — sync-backlog abortado para evitar duplicatas."
        $report = [ordered]@{ created = @(); count = 0; dry_run = [bool]$DryRun; aborted = $true; error = ($rawIssues -join ' ') }
        if ($Json) { $report | ConvertTo-Json -Depth 5 } else { $report | ConvertTo-Json -Depth 5 }
        return
    }
    $allIssues = $rawIssues | ConvertFrom-Json
    $repo = gh repo view --json nameWithOwner -q .nameWithOwner
    $owner = ($repo -split '/')[0]
    $cfg = Get-ProjectConfig -Root $Root
    $projectNumber = if ($cfg) { $cfg.project_number } else { 0 }

    $created = @()

    foreach ($item in $allItems) {
        $existing = Get-PhaseIssueAnyState -Root $Root -PhaseId $item.id -AllIssues $allIssues
        if ($existing) { continue }

        $labels = Get-PhaseIssueLabels -Item $item
        if ($item.status -eq 'completed') {
            $labels += 'status/done'
        } elseif ($item.blocked_by -and $item.blocked_by.Count -gt 0) {
            $blocked = $false
            foreach ($dep in $item.blocked_by) {
                if ($dep -match '^FASE' -and -not (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $dep)) {
                    $blocked = $true
                    break
                }
            }
            if ($blocked) { $labels += 'status/blocked' }
        }

        $title = if ($item.id -match '^FASE') { "[Epic] $($item.title)" } else { "[Backlog] $($item.title)" }
        $body = if ($item.id -match '^FASE') {
            New-PhaseIssueBody -Item $item -Root $Root -Source 'sync-backlog'
        } else {
            @"
## Item de backlog (sync-backlog)

**Id:** $($item.id)
**Documento:** $($item.doc)

---
<!-- arah-next-phase id=$($item.id) -->
<!-- arah-phase-epic -->
"@
        }

        if ($DryRun) {
            $created += @{ id = $item.id; action = 'would-create'; title = $title; closed = ($item.status -eq 'completed') }
            continue
        }

        $labelParams = @()
        foreach ($l in ($labels | Select-Object -Unique)) { $labelParams += '--label'; $labelParams += $l }
        $issueUrl = gh issue create --title $title --body $body @labelParams 2>&1
        if ($LASTEXITCODE -ne 0) {
            if ($issueUrl -match 'rate limit') {
                Write-Warning "Rate limit GitHub — pausando após $($created.Count) issues. Reexecute sync-backlog mais tarde."
                break
            }
            Write-Warning "Falha ao criar issue $($item.id): $issueUrl"
            continue
        }

        Start-Sleep -Milliseconds 1200

        if ($issueUrl -match '/issues/(\d+)') {
            $issueNum = [int]$Matches[1]
            Set-IssueMilestoneForWave -Repo $repo -IssueNumber $issueNum -Root $Root -Wave $item.wave | Out-Null
            if ($item.status -eq 'completed') {
                gh issue close $issueNum --comment 'Fase concluída — issue de histórico para o Project.' 2>$null | Out-Null
            }
            if ($projectNumber -gt 0) {
                Add-IssueToGitHubProject -IssueUrl $issueUrl -ProjectNumber $projectNumber -Owner $owner -Root $Root | Out-Null
            }
            $created += @{ id = $item.id; issue = $issueNum; url = $issueUrl; closed = ($item.status -eq 'completed') }
            $allIssues += [pscustomobject]@{ number = $issueNum; title = $title; state = $(if ($item.status -eq 'completed') { 'CLOSED' } else { 'OPEN' }); body = $body; labels = @{ name = $labels }; url = $issueUrl }
        }
    }

    $report = [ordered]@{ created = $created; count = $created.Count; dry_run = [bool]$DryRun }
    if ($Json) { $report | ConvertTo-Json -Depth 5 } else { $report | ConvertTo-Json -Depth 5 }
}



function Invoke-ProjectSyncBoard {

    param([switch]$DryRun, [switch]$Json)



    $cfg = Get-ProjectConfig -Root $Root

    if (-not $cfg -or $cfg.project_number -le 0) {

        Write-Warning 'project_number não configurado — rode bootstrap ou ensure primeiro'

        return

    }



    $repo = gh repo view --json nameWithOwner -q .nameWithOwner

    $owner = ($repo -split '/')[0]

    $projectNumber = $cfg.project_number

    $columns = Get-ProjectColumnsFromConfig -Root $Root

    $viewNames = Get-ProjectViewsFromConfig -Root $Root

    $queue = Get-PhaseRoadmap -Root $Root

    $allIssues = gh issue list --state all --limit 500 --json number,title,state,body,labels,url | ConvertFrom-Json

    $openPrs = Get-OpenPullRequestsForRepo -Repo $repo



    try {

        $project = Get-ProjectV2Node -Owner $owner -ProjectNumber $projectNumber

    } catch {

        Write-Warning "Não foi possível ler project #$projectNumber : $_"

        return

    }



    if (-not $project) {

        Write-Warning "Project #$projectNumber não encontrado"

        return

    }



    $statusField = Get-StatusFieldFromProject -ProjectNode $project

    if ($statusField -and -not $DryRun) {

        try {

            Update-ProjectStatusOptions -FieldId $statusField.id -OptionNames $columns | Out-Null

            $project = Get-ProjectV2Node -Owner $owner -ProjectNumber $projectNumber

            $statusField = Get-StatusFieldFromProject -ProjectNode $project

        } catch {

            Write-Warning "Status options: $_"

        }

    }



    if (-not $DryRun) {

        Ensure-ProjectViews -ProjectId $project.id -ViewNames $viewNames -ExistingViews $project.views.nodes | Out-Null

    }



    $added = @()

    $updated = @()



    foreach ($item in $queue) {

        if ($item.id -notmatch '^FASE') { continue }

        $issue = $allIssues | Where-Object { Test-PhaseIssueMarker -Issue $_ -PhaseId $item.id } | Select-Object -First 1



        $targetStatus = Resolve-PhaseStatusColumn -Issue $issue -PhaseItem $item -Root $Root -OpenPrs $openPrs

        $existingItem = Find-ProjectItemForPhase -ProjectNode $project -PhaseId $item.id -AllIssues $allIssues



        if (-not $existingItem) {

            if ($DryRun) {

                $added += @{ phase = $item.id; action = 'would-add'; status = $targetStatus }

                continue

            }



            if ($issue) {

                $ok = Add-IssueToGitHubProject -IssueUrl $issue.url -ProjectNumber $projectNumber -Owner $owner

                if ($ok) {

                    $added += @{ phase = $item.id; action = 'issue-added'; issue = $issue.number }

                    $project = Get-ProjectV2Node -Owner $owner -ProjectNumber $projectNumber

                    $existingItem = Find-ProjectItemForPhase -ProjectNode $project -PhaseId $item.id -AllIssues $allIssues

                }

            } else {

                $draftTitle = "[Epic] $($item.title)"

                $draftNote = if ($item.status -eq 'completed') { 'Fase concluída (histórico).' } else { 'Placeholder — issue abre quando fase desbloquear.' }
                $draftBody = "$draftNote`n`n<!-- arah-phase-draft id=$($item.id) -->"

                $draftAdded = $false
                gh project item-create --owner $owner --project-number $projectNumber --title $draftTitle --body $draftBody 2>$null | Out-Null
                if ($LASTEXITCODE -eq 0) { $draftAdded = $true }

                if (-not $draftAdded) {
                    try {
                        Add-ProjectV2DraftIssue -ProjectId $project.id -Title $draftTitle -Body $draftBody | Out-Null
                        $draftAdded = $true
                    } catch {
                        Write-Warning "Draft $($item.id): $_"
                    }
                }

                if ($draftAdded) {

                    $added += @{ phase = $item.id; action = 'draft-added' }

                    $project = Get-ProjectV2Node -Owner $owner -ProjectNumber $projectNumber

                    $existingItem = Find-ProjectItemForPhase -ProjectNode $project -PhaseId $item.id -AllIssues $allIssues

                }

            }

        }



        if ($existingItem -and $statusField -and -not $DryRun) {

            $option = $statusField.options | Where-Object { $_.name -eq $targetStatus } | Select-Object -First 1

            if ($option) {

                $ok = Set-ProjectItemStatus -ProjectId $project.id -ItemId $existingItem.item_id -StatusFieldId $statusField.id -OptionId $option.id

                if ($ok) {

                    $updated += @{ phase = $item.id; status = $targetStatus; item = $existingItem.item_id }

                }

            }

        } elseif ($DryRun) {

            $updated += @{ phase = $item.id; status = $targetStatus; action = 'would-update' }

        }

    }



    $report = [ordered]@{

        project_number = $projectNumber

        added          = $added

        status_updated = $updated

        dry_run        = [bool]$DryRun

    }

    if ($Json) { $report | ConvertTo-Json -Depth 5 } else { $report | ConvertTo-Json -Depth 5 }

}



function Invoke-ProjectReconcile {

    param([switch]$DryRun, [switch]$Json)



    $queue = Get-PhaseRoadmap -Root $Root

    $openIssues = gh issue list --state open --limit 500 --json number,title,body,labels,url | ConvertFrom-Json

    $repo = gh repo view --json nameWithOwner -q .nameWithOwner

    $fixed = @()



    foreach ($item in $queue) {

        if ($item.id -notmatch '^FASE') { continue }



        $issue = $openIssues | Where-Object { $_.title -match [regex]::Escape($item.id) } | Select-Object -First 1

        if (-not $issue) { continue }



        $needsEpicTitle = $issue.title -notmatch '^\[Epic\]'

        $labels = @($issue.labels.name)

        $expected = Get-PhaseIssueLabels -Item $item

        $missingLabels = @($expected | Where-Object { $_ -notin $labels })



        if (-not $needsEpicTitle -and $missingLabels.Count -eq 0 -and $issue.body -match 'arah-phase-epic') { continue }



        if ($DryRun) {

            $fixed += @{ phase = $item.id; issue = $issue.number; would_fix = $true }

            continue

        }



        if ($needsEpicTitle) {

            $newTitle = "[Epic] $($item.title)"

            gh issue edit $issue.number --title $newTitle | Out-Null

        }



        foreach ($label in $missingLabels) {

            gh issue edit $issue.number --add-label $label 2>$null | Out-Null

        }



        if ($issue.body -notmatch 'arah-phase-epic') {

            $marker = "`n<!-- arah-next-phase id=$($item.id) -->`n<!-- arah-phase-epic -->"

            gh issue edit $issue.number --body "$($issue.body)$marker" 2>$null | Out-Null

        } elseif ($issue.body -notmatch "arah-next-phase id=$([regex]::Escape($item.id))") {

            gh issue edit $issue.number --body "$($issue.body)`n<!-- arah-next-phase id=$($item.id) -->" 2>$null | Out-Null

        }



        $milestoneTitle = Resolve-MilestoneForWave -Root $Root -Wave $item.wave

        if ($milestoneTitle) {

            $milestones = gh api "repos/$repo/milestones?state=open&per_page=100" | ConvertFrom-Json

            $ms = $milestones | Where-Object { $_.title -eq $milestoneTitle } | Select-Object -First 1

            if ($ms) {

                Set-IssueMilestoneForWave -Repo $repo -IssueNumber $issue.number -Root $Root -Wave $item.wave | Out-Null

            }

        }



        $fixed += @{ phase = $item.id; issue = $issue.number; labels_added = $missingLabels }

    }



    $report = [ordered]@{ reconciled = $fixed; count = $fixed.Count; dry_run = [bool]$DryRun }

    if ($Json) { $report | ConvertTo-Json -Depth 4 } else { $report | ConvertTo-Json -Depth 4 }

}



if ($Command -eq 'help') { Show-ProjectHelp; exit 0 }



if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {

    Write-Error 'gh CLI required'

    exit 1

}



Push-Location $Root

try {

    switch ($Command) {

        'bootstrap' {

            if (-not $DryRun) {

                Invoke-ProjectEnsure -Json:$Json | Out-Null

                Invoke-ProjectReconcile -Json:$Json | Out-Null

                Invoke-SyncBacklogIssues -Json:$Json | Out-Null

                try {
                    & (Join-Path $PSScriptRoot 'github-project.ps1') -Command sync-queue -Json:$Json | Out-Null
                } catch {
                    Write-Warning "sync-queue: $_"
                }

                Invoke-ProjectSyncBoard -Json:$Json | Out-Null

                & (Join-Path $PSScriptRoot 'export-phase-status.ps1') | Out-Null

            } else {

                Write-Host 'Dry-run bootstrap:'

                Invoke-ProjectEnsure -DryRun -Json:$Json

                Invoke-ProjectReconcile -DryRun -Json:$Json

                & (Join-Path $PSScriptRoot 'github-project.ps1') -Command sync-queue -DryRun -Json:$Json

                Invoke-ProjectSyncBoard -DryRun -Json:$Json

            }

            exit 0

        }

        'ensure' {

            Invoke-ProjectEnsure -DryRun:$DryRun -Json:$Json

            exit 0

        }

        'sync-backlog' {

            Invoke-SyncBacklogIssues -DryRun:$DryRun -Json:$Json

            if (-not $DryRun) { Invoke-ProjectSyncBoard -Json:$Json | Out-Null }

            exit 0

        }

        'sync-queue' {

            & (Join-Path $PSScriptRoot 'sync-github-labels.ps1') | Out-Null

            $queue = Get-PhaseQueue -Root $Root

            $openIssues = gh issue list --state open --limit 500 --json title,body,labels | ConvertFrom-Json

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

        'sync-project' {

            Invoke-ProjectSyncBoard -DryRun:$DryRun -Json:$Json

            exit 0

        }

        'reconcile' {

            Invoke-ProjectReconcile -DryRun:$DryRun -Json:$Json

            exit 0

        }

        'status' {

            $issues = gh issue list --state all --limit 500 --json number,title,state,labels,url,body | ConvertFrom-Json

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


