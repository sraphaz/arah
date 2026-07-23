#Requires -Version 5.1
<#
.SYNOPSIS
  Fila operacional (PHASE_QUEUE.yaml) e roadmap completo (backlog + meta).
#>

function Get-PhaseQueueYamlPath {
    param([string]$Root)
    return Join-Path $Root 'docs/_meta/PHASE_QUEUE.yaml'
}

function Get-PhaseRoadmapMetaPath {
    param([string]$Root)
    return Join-Path $Root 'docs/_meta/PHASE_ROADMAP_META.yaml'
}

function Parse-PhaseQueueYaml {
    param([string]$Root)

    $path = Get-PhaseQueueYamlPath -Root $Root
    if (-not (Test-Path $path)) {
        throw "Missing $path"
    }

    $items = @()
    $current = $null
    $inBlockedList = $false

    foreach ($line in Get-Content $path) {
        if ($line -match '^\s+-\s+id:\s*(.+)$') {
            if ($current) { $items += $current }
            $current = @{ id = $Matches[1].Trim(); blocked_by = @() }
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+title:\s*"(.+)"$') {
            $current.title = $Matches[1]
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+area:\s*(.+)$') {
            $current.area = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+doc:\s*(.+)$') {
            $current.doc = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+priority:\s*(.+)$') {
            $current.priority = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+wave:\s*(.+)$') {
            $current.wave = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+frente:\s*(.+)$') {
            $current.frente = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+spec_id:\s*(.+)$') {
            $current.spec_id = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+kind:\s*(.+)$') {
            $current.kind = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+blocked_by:\s*\[(.*)\]\s*$') {
            $inner = $Matches[1].Trim()
            if ($inner) {
                $current.blocked_by = $inner -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
            }
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+blocked_by:\s*$') {
            $current.blocked_by = @()
            $inBlockedList = $true
        } elseif ($inBlockedList -and $line -match '^\s+-\s+(FASE\d+(?:_\w+)?|TI-\d+|doc-[\w-]+)\s*$') {
            $current.blocked_by += $Matches[1].Trim()
        } elseif ($current -and $line -match '^\s+status:\s*(.+)$') {
            $current.status = $Matches[1].Trim()
            $inBlockedList = $false
        } elseif ($current -and -not $inBlockedList) {
            $inBlockedList = $false
        }
    }
    if ($current) { $items += $current }
    return $items
}

function Get-PhaseQueue {
    param([string]$Root)
    return Parse-PhaseQueueYaml -Root $Root
}

function Get-PhaseRoadmapMeta {
    param([string]$Root)

    $path = Get-PhaseRoadmapMetaPath -Root $Root
    $meta = @{ completed = @(); overrides = @{} }
    if (-not (Test-Path $path)) { return $meta }

    $section = ''
    $currentId = $null
    foreach ($line in Get-Content $path) {
        if ($line -match '^completed:\s*$') { $section = 'completed'; continue }
        if ($line -match '^overrides:\s*$') { $section = 'overrides'; continue }
        if ($section -eq 'completed' -and $line -match '^\s+-\s+(\S+)\s*$') {
            $meta.completed += $Matches[1].Trim()
        } elseif ($section -eq 'overrides' -and $line -match '^\s+(\S+):\s*$') {
            $currentId = $Matches[1].Trim()
            $meta.overrides[$currentId] = @{ id = $currentId; blocked_by = @() }
        } elseif ($section -eq 'overrides' -and $currentId -and $line -match '^\s+title:\s*"(.+)"$') {
            $meta.overrides[$currentId].title = $Matches[1]
        } elseif ($section -eq 'overrides' -and $currentId -and $line -match '^\s+doc:\s*(.+)$') {
            $meta.overrides[$currentId].doc = $Matches[1].Trim()
        } elseif ($section -eq 'overrides' -and $currentId -and $line -match '^\s+wave:\s*(.+)$') {
            $meta.overrides[$currentId].wave = $Matches[1].Trim()
        } elseif ($section -eq 'overrides' -and $currentId -and $line -match '^\s+area:\s*(.+)$') {
            $meta.overrides[$currentId].area = $Matches[1].Trim()
        } elseif ($section -eq 'overrides' -and $currentId -and $line -match '^\s+priority:\s*(.+)$') {
            $meta.overrides[$currentId].priority = $Matches[1].Trim()
        } elseif ($section -eq 'overrides' -and $currentId -and $line -match '^\s+blocked_by:\s*\[(.*)\]\s*$') {
            $inner = $Matches[1].Trim()
            if ($inner) {
                $meta.overrides[$currentId].blocked_by = $inner -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
            }
        }
    }
    return $meta
}

function Get-DefaultWaveForPhaseId {
    param([string]$PhaseId)

    if ($PhaseId -match '^FASE(\d+)$') {
        $n = [int]$Matches[1]
        if ($n -ge 52) { return $null }
        if ($n -le 8) { return 'C1' }
        if ($n -le 12) { return 'C2' }
        if ($n -le 16) { return 'C3' }
        if ($n -le 18) { return 'C5' }
        if ($n -le 24) { return 'C6' }
        if ($n -le 28) { return 'C7' }
        if ($n -eq 29) { return 'C8' }
        if ($n -le 31) { return 'C9' }
        return 'C10'
    }
    if ($PhaseId -eq 'FASE14_5') { return 'C3' }
    if ($PhaseId -eq 'FASE17_BFF') { return 'C4' }
    return 'C10'
}

function Get-DefaultAreaForPhaseId {
    param([string]$PhaseId)

    if ($PhaseId -match '^FASE(\d+)$') {
        $n = [int]$Matches[1]
        if ($n -ge 57 -and $n -le 60) {
            if ($n -eq 60) { return 'area/flutter' }
            return 'area/web'
        }
        if ($n -ge 52) { return 'area/ops' }
        if ($n -in 29, 60) { return 'area/flutter' }
        if ($n -in 57) { return 'area/web' }
    }
    return 'area/backend'
}

function Parse-PhaseMarkdownHeader {
    param([string]$FilePath)

    $result = @{
        title = $null
        status_line = $null
        blocked_by = @()
        completed = $false
    }
    if (-not (Test-Path $FilePath)) { return $result }

    $header = Get-Content $FilePath -TotalCount 20
    foreach ($line in $header) {
        if (-not $result.title -and $line -match '^#\s+(.+)$') {
            $result.title = $Matches[1].Trim()
        }
        if ($line -match '^\*\*Status\*\*:\s*(.+)$') {
            $result.status_line = $Matches[1].Trim()
            if ($result.status_line -match '(?i)complet|✅') {
                $result.completed = $true
            }
        }
        if ($line -match '^\*\*Depende de\*\*:\s*(.+)$') {
            $deps = [regex]::Matches($Matches[1], '(?i)Fase\s*(\d+(?:\.\d+)?)|FASE(\d+)')
            foreach ($m in $deps) {
                if ($m.Groups[1].Success -and $m.Groups[1].Value -eq '14.5') {
                    $result.blocked_by += 'FASE14_5'
                } elseif ($m.Groups[1].Success) {
                    $result.blocked_by += "FASE$($m.Groups[1].Value)"
                } elseif ($m.Groups[2].Success) {
                    $result.blocked_by += "FASE$($m.Groups[2].Value)"
                }
            }
        }
    }
    return $result
}

function Get-PhaseSortKey {
    param([string]$PhaseId)

    if ($PhaseId -match '^FASE(\d+)$') { return [int]$Matches[1] * 100 }
    if ($PhaseId -eq 'FASE14_5') { return 145 }
    if ($PhaseId -eq 'FASE17_BFF') { return 1705 }
    return 99999
}

function Merge-PhaseRoadmapItem {
    param(
        [hashtable]$Base,
        [hashtable]$Override
    )

    $merged = @{}
    foreach ($key in $Base.Keys) { $merged[$key] = $Base[$key] }
    if ($Override) {
        foreach ($key in $Override.Keys) {
            if ($null -ne $Override[$key] -and $Override[$key] -ne '') {
                $merged[$key] = $Override[$key]
            }
        }
    }
    if (-not $merged.area) { $merged.area = Get-DefaultAreaForPhaseId -PhaseId $merged.id }
    if (-not $merged.priority) { $merged.priority = 'P1' }
    if (-not $merged.blocked_by) { $merged.blocked_by = @() }
    return $merged
}

function Get-PhaseRoadmap {
    <#
    .SYNOPSIS
      Roadmap completo: todos os FASE*.md do backlog + extras + overrides da fila operacional.
    #>
    param([string]$Root)

    $meta = Get-PhaseRoadmapMeta -Root $Root
    $queueOverrides = @{}
    foreach ($q in (Parse-PhaseQueueYaml -Root $Root)) {
        if ($q.id -match '^FASE') { $queueOverrides[$q.id] = $q }
    }

    $backlogDir = Join-Path $Root 'docs/backlog-api'
    $discovered = @{}

    foreach ($file in Get-ChildItem -Path $backlogDir -Filter 'FASE*.md' -File) {
        if ($file.Name -notmatch '^FASE(\d+)\.md$') { continue }
        $phaseId = $file.BaseName
        $doc = "docs/backlog-api/$($file.Name)"
        $parsed = Parse-PhaseMarkdownHeader -FilePath $file.FullName
        $title = if ($parsed.title) { $parsed.title } else { $phaseId }
        $discovered[$phaseId] = @{
            id = $phaseId
            title = $title
            doc = $doc
            wave = Get-DefaultWaveForPhaseId -PhaseId $phaseId
            area = Get-DefaultAreaForPhaseId -PhaseId $phaseId
            priority = 'P1'
            blocked_by = @($parsed.blocked_by)
            status = if ($parsed.completed -or ($meta.completed -contains $phaseId)) { 'completed' } else { $null }
        }
    }

    foreach ($extra in @('FASE14_5', 'FASE17_BFF')) {
        $fileName = if ($extra -eq 'FASE14_5') { 'FASE14_5.md' } else { 'FASE17_BFF.md' }
        $filePath = Join-Path $backlogDir $fileName
        if (-not (Test-Path $filePath)) { continue }
        $parsed = Parse-PhaseMarkdownHeader -FilePath $filePath
        $discovered[$extra] = @{
            id = $extra
            title = if ($parsed.title) { $parsed.title } else { $extra }
            doc = "docs/backlog-api/$fileName"
            wave = Get-DefaultWaveForPhaseId -PhaseId $extra
            area = Get-DefaultAreaForPhaseId -PhaseId $extra
            priority = 'P1'
            blocked_by = @($parsed.blocked_by)
            status = if ($parsed.completed) { 'completed' } else { $null }
        }
    }

    foreach ($phaseId in $meta.overrides.Keys) {
        if (-not $discovered.ContainsKey($phaseId)) {
            $ov = $meta.overrides[$phaseId]
            $discovered[$phaseId] = @{
                id = $phaseId
                title = $ov.title
                doc = $ov.doc
                wave = $ov.wave
                area = $ov.area
                priority = $ov.priority
                blocked_by = @($ov.blocked_by)
                status = $null
            }
        }
    }

    $roadmap = @()
    foreach ($phaseId in ($discovered.Keys | Sort-Object { Get-PhaseSortKey -PhaseId $_ })) {
        $item = Merge-PhaseRoadmapItem -Base $discovered[$phaseId] -Override $meta.overrides[$phaseId]
        if ($queueOverrides.ContainsKey($phaseId)) {
            $item = Merge-PhaseRoadmapItem -Base $item -Override $queueOverrides[$phaseId]
        }
        if ($meta.completed -contains $phaseId -and $item.status -ne 'completed') {
            $item.status = 'completed'
        }
        if ($item.id -match '^FASE(\d+)$' -and [int]$Matches[1] -ge 52) {
            if (-not $item.era) { $item.era = 'sustentacao' }
        } else {
            if (-not $item.era) { $item.era = 'community' }
        }
        $roadmap += $item
    }
    return $roadmap
}

function Get-PhaseQueueItem {
    param([string]$Root, [string]$PhaseId)

    $fromQueue = Get-PhaseQueue -Root $Root | Where-Object { $_.id -eq $PhaseId } | Select-Object -First 1
    if ($fromQueue) { return $fromQueue }

    $fromRoadmap = Get-PhaseRoadmap -Root $Root | Where-Object { $_.id -eq $PhaseId } | Select-Object -First 1
    if ($fromRoadmap) { return $fromRoadmap }

    throw "Phase not in queue: $PhaseId"
}

function Get-ProjectConfig {
    param([string]$Root)
    $path = Join-Path $Root '.github/project/arah-sustentacao.yml'
    if (-not (Test-Path $path)) { return $null }
    $raw = Get-Content $path -Raw
    $cfg = @{ wave_milestones = @{}; project_number = $null; title = 'Arah Sustentação' }
    if ($raw -match '(?m)^title:\s*"(.+)"') { $cfg.title = $Matches[1] }
    if ($raw -match '(?m)^project_number:\s*(\d+)') { $cfg.project_number = [int]$Matches[1] }
    if ($raw -match '(?m)^project_url:\s*"(.+)"') { $cfg.project_url = $Matches[1] }
    if ($raw -match '(?ms)^wave_milestones:\s*\n((?:  .+\r?\n)+)') {
        foreach ($line in ($Matches[1] -split "`n")) {
            if ($line -match '^\s+((?:C|S)\d+):\s*"(.+)"') {
                $cfg.wave_milestones[$Matches[1]] = $Matches[2]
            }
        }
    }
    return $cfg
}

function Get-SpecIdForPhase {
    param([string]$Root, [string]$PhaseId)
    $specDir = Join-Path $Root 'docs/specs/phases'
    if (-not (Test-Path $specDir)) { return $null }
    $match = Get-ChildItem -Path $specDir -Filter '*.spec.yaml' | Where-Object {
        $raw = Get-Content $_.FullName -Raw
        $raw -match "(?m)^phase:\s*$PhaseId\s*$" -or $_.Name -like "$PhaseId*"
    } | Select-Object -First 1
    if (-not $match) { return $null }
    if ((Get-Content $match.FullName -Raw) -match '(?m)^id:\s*(.+)$') {
        return $Matches[1].Trim()
    }
    return $null
}

function New-PhaseIssueBody {
    param(
        [hashtable]$Item,
        [string]$Root,
        [string]$Source = 'next-phase'
    )
    $areaLabel = if ($Item.area) { $Item.area } else { 'area/planning' }
    $specId = if ($Item.spec_id) { $Item.spec_id } else { Get-SpecIdForPhase -Root $Root -PhaseId $Item.id }
    $specLine = if ($specId) { "**Spec-Id:** ``$specId``" } else { '_Spec SDD pendente — usar spec-author._' }
    $waveLine = if ($Item.wave) { "**Onda:** $($Item.wave)" } else { '' }
    $blockedLine = if ($Item.blocked_by -and $Item.blocked_by.Count -gt 0) {
        "**Depende de:** $($Item.blocked_by -join ', ')"
    } else { '' }

    return @"
## Épico de fase ($Source)

**Fase:** $($Item.id)
**Prioridade:** $($Item.priority)
$waveLine
$blockedLine
**Documento:** [$($Item.doc)]($($Item.doc))
$specLine

### Intenção
Implementar $($Item.title) conforme documento de fase e critérios de aceite.

### Critérios de aceite
- [ ] Itens documentados em ``$($Item.doc)``
- [ ] Harness/spec verde quando aplicável
- [ ] ``run-tests`` passa na área afetada
- [ ] ``sync-docs`` aplicado
- [ ] Apontamentos de bots resolvidos no PR
- [ ] Merge humano após ``ready-for-merge``

### Guardrails
- [ ] Sem merge automático
- [ ] Escopo limitado à área: ``$areaLabel``

---
<!-- arah-next-phase id=$($Item.id) -->
<!-- arah-phase-epic -->
"@
}

function Get-PhaseIssueLabels {
    param([hashtable]$Item)
    $labels = @('agent-task', 'epic/phase')
    if ($Item.area) { $labels += $Item.area }
    if ($Item.wave) { $labels += "wave/$($Item.wave)" }
    if ($Item.priority) { $labels += "priority/$($Item.priority)" }
    return @($labels | Select-Object -Unique)
}

function Add-IssueToGitHubProject {
    param(
        [string]$IssueUrl,
        [int]$ProjectNumber,
        [string]$Owner,
        [string]$Root = ''
    )
    if ($ProjectNumber -le 0) { return $false }

    $ok = $false
    gh project item-add $ProjectNumber --owner $Owner --url $IssueUrl 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { return $true }

    if (-not (Get-Command Invoke-GhGraphql -ErrorAction SilentlyContinue)) {
        $apiPath = Join-Path $PSScriptRoot 'GitHub-ProjectApi.ps1'
        if (Test-Path $apiPath) { . $apiPath }
    }
    if (-not (Test-GhProjectTokenAvailable)) { return $false }

    try {
        $repo = gh repo view --json nameWithOwner -q .nameWithOwner
        $repoName = ($repo -split '/')[1]
        if ($IssueUrl -match '/issues/(\d+)') {
            $num = [int]$Matches[1]
            $project = Get-ProjectV2Node -Owner $Owner -ProjectNumber $ProjectNumber
            $issueNodeId = Get-IssueNodeId -Owner $Owner -RepoName $repoName -IssueNumber $num
            Add-IssueToProjectViaGraphql -ProjectId $project.id -IssueNodeId $issueNodeId | Out-Null
            return $true
        }
    } catch {
        Write-Warning "GraphQL item-add failed: $_"
    }
    return $false
}

function Resolve-MilestoneForWave {
    param([string]$Root, [string]$Wave)
    if (-not $Wave) { return $null }
    $cfg = Get-ProjectConfig -Root $Root
    if (-not $cfg -or -not $cfg.wave_milestones.ContainsKey($Wave)) { return $null }
    return $cfg.wave_milestones[$Wave]
}

function Test-PhaseCompleteFromMeta {
    param([string]$Root, [string]$Id)
    $meta = Get-PhaseRoadmapMeta -Root $Root
    return $meta.completed -contains $Id
}

function Test-PhaseCompleteFromGitHub {
    param([string]$Root, [string]$PhaseId)

    if (Test-PhaseCompleteFromMeta -Root $Root -Id $PhaseId) { return $true }

    if (-not (Get-Command gh -ErrorAction SilentlyContinue)) { return $false }

    Push-Location $Root
    try {
        $closed = gh issue list --state closed --limit 200 --json title,body,labels | ConvertFrom-Json
        foreach ($issue in $closed) {
            if ($issue.body -match "arah-next-phase id=$([regex]::Escape($PhaseId))") { return $true }
            if ($issue.title -match [regex]::Escape($PhaseId) -and ($issue.labels.name -contains 'epic/phase')) { return $true }
        }
    } finally {
        Pop-Location
    }

    $status = Get-Content (Join-Path $Root 'docs/STATUS_FASES.md') -Raw -ErrorAction SilentlyContinue
    if ($status -and $PhaseId -match '^FASE(\d+)$' -and $status -match "$PhaseId[^\n]*\|\s*✅") { return $true }
    return $false
}

function Test-PhaseQueueItemEligibleForNextPhase {
    <#
    .SYNOPSIS
      next-phase só auto-abre épicos FASE*. Trilhas TI (kind: track / id TI-*)
      e itens maintenance/doc ficam fora da seleção automática.
    #>
    param([hashtable]$Item)
    if (-not $Item -or -not $Item.id) { return $false }
    if ($Item.kind -eq 'track' -or $Item.kind -eq 'maintenance') { return $false }
    if ($Item.id -match '^TI-') { return $false }
    if ($Item.id -match '^doc-') { return $false }
    if ($Item.id -notmatch '^FASE\d+') { return $false }
    return $true
}

function Get-PhaseIdFromIssue {
    param([object]$Issue)
    if (-not $Issue) { return $null }
    if ($Issue.body -match 'arah-next-phase id=(FASE[\w\.]+)') { return $Matches[1] }
    if ($Issue.body -match 'arah-phase-draft id=(FASE[\w\.]+)') { return $Matches[1] }
    if ($Issue.title -match '\[Epic\]\s*(FASE[\w\.]+)') { return $Matches[1] }
    if ($Issue.title -match '\[Epic\]\s*Fase\s*(\d+(?:[_.]\d+)?)') {
        $n = $Matches[1] -replace '\.', '_'
        return "FASE$n"
    }
    if ($Issue.title -match '\b(FASE\d+(?:_\w+)?)\b') { return $Matches[1] }
    return $null
}

function Get-GhRepoFullName {
    param([string]$Root = (Get-Location).Path)
    Push-Location $Root
    try {
        $remote = git remote get-url origin 2>$null
        if ($remote -match 'github\.com[:/]([^/]+/[^/.]+)') {
            return $Matches[1] -replace '\.git$', ''
        }
    } finally { Pop-Location }
    $view = gh repo view --json nameWithOwner -q .nameWithOwner 2>$null
    if ($view) { return $view }
    throw 'Não foi possível resolver owner/repo'
}

function Get-AllRepoIssuesRest {
    param(
        [string]$Repo,
        [string]$Label = '',
        [int]$MaxPages = 10
    )
    $all = @()
    for ($page = 1; $page -le $MaxPages; $page++) {
        $path = "repos/$Repo/issues?state=all&per_page=100&page=$page"
        if ($Label) { $path += "&labels=$([uri]::EscapeDataString($Label))" }
        $batch = gh api $path 2>$null | ConvertFrom-Json
        if (-not $batch -or @($batch).Count -eq 0) { break }
        $issues = @($batch | Where-Object { -not $_.pull_request })
        foreach ($issue in $issues) {
            if ($issue.state) {
                $issue | Add-Member -NotePropertyName 'state' -NotePropertyValue ($issue.state.ToUpper()) -Force
            }
            if ($issue.html_url -and -not $issue.url) {
                $issue | Add-Member -NotePropertyName 'url' -NotePropertyValue $issue.html_url -Force
            } elseif ($issue.html_url) {
                $issue | Add-Member -NotePropertyName 'url' -NotePropertyValue $issue.html_url -Force
            }
        }
        $all += $issues
        if (@($batch).Count -lt 100) { break }
        Start-Sleep -Milliseconds 200
    }
    return $all
}

function Close-IssueRest {
    param(
        [string]$Repo,
        [int]$IssueNumber,
        [string]$Comment = ''
    )
    gh api -X PATCH "repos/$Repo/issues/$IssueNumber" -f state=closed 2>$null | Out-Null
    if ($LASTEXITCODE -ne 0) { return $false }
    if ($Comment) {
        gh api -X POST "repos/$Repo/issues/$IssueNumber/comments" -f body=$Comment 2>$null | Out-Null
    }
    return ($LASTEXITCODE -eq 0)
}

function Select-CanonicalPhaseIssue {
    param([array]$Issues)
    if (-not $Issues -or $Issues.Count -eq 0) { return $null }
    $withMarker = @($Issues | Where-Object { $_.body -match 'arah-next-phase id=' })
    if ($withMarker.Count -gt 0) {
        $open = @($withMarker | Where-Object { $_.state -eq 'OPEN' })
        if ($open.Count -gt 0) {
            return $open | Sort-Object { [int]$_.number } | Select-Object -First 1
        }
        return $withMarker | Sort-Object { [int]$_.number } | Select-Object -First 1
    }
    return $Issues | Sort-Object { [int]$_.number } | Select-Object -First 1
}

function Test-PhaseIssueMarker {
    param(
        [object]$Issue,
        [string]$PhaseId
    )
    if (-not $Issue) { return $false }
    $escaped = [regex]::Escape($PhaseId)
    if ($Issue.body -match "arah-next-phase id=$escaped(\s|-->)") { return $true }
    if ($Issue.body -match "arah-phase-draft id=$escaped(\s|-->)") { return $true }
    if ($Issue.title -match "\[Epic\].*\b$escaped\b") { return $true }
    if ($Issue.title -match "\[Backlog\].*\b$escaped\b") { return $true }
    return $false
}

function Get-PhaseIssueAnyState {
    param(
        [string]$Root,
        [string]$PhaseId,
        [array]$AllIssues = @()
    )

    if ($AllIssues.Count -eq 0 -and (Get-Command gh -ErrorAction SilentlyContinue)) {
        Push-Location $Root
        try {
            $AllIssues = gh issue list --state all --limit 500 --json number,title,state,body,labels,url | ConvertFrom-Json
        } finally { Pop-Location }
    }

    $hit = $AllIssues | Where-Object { Test-PhaseIssueMarker -Issue $_ -PhaseId $PhaseId }
    return Select-CanonicalPhaseIssue -Issues @($hit)
}

function Test-PhaseOpenIssue {
    param([string]$Root, [string]$PhaseId, [array]$OpenIssues = @())

    if ($OpenIssues.Count -eq 0 -and (Get-Command gh -ErrorAction SilentlyContinue)) {
        Push-Location $Root
        try {
            $OpenIssues = gh issue list --state open --limit 500 --json title,body,labels | ConvertFrom-Json
        } finally { Pop-Location }
    }

    foreach ($issue in $OpenIssues) {
        if (Test-PhaseIssueMarker -Issue $issue -PhaseId $PhaseId) { return $true }
    }
    return $false
}

function Set-IssueMilestoneForWave {
    param(
        [string]$Repo,
        [int]$IssueNumber,
        [string]$Root,
        [string]$Wave
    )
    if (-not $Wave) { return $false }
    $milestoneTitle = Resolve-MilestoneForWave -Root $Root -Wave $Wave
    if (-not $milestoneTitle) { return $false }
    $milestones = gh api "repos/$Repo/milestones?state=open&per_page=100" | ConvertFrom-Json
    $ms = $milestones | Where-Object { $_.title -eq $milestoneTitle } | Select-Object -First 1
    if (-not $ms) { return $false }
    $milestoneNum = [int]$ms.number
    $patch = @{ milestone = $milestoneNum } | ConvertTo-Json -Compress
    $patch | gh api -X PATCH "repos/$Repo/issues/$IssueNumber" --input - 2>$null | Out-Null
    return ($LASTEXITCODE -eq 0)
}
