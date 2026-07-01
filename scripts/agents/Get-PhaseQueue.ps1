#Requires -Version 5.1
<#
.SYNOPSIS
  Parseia docs/_meta/PHASE_QUEUE.yaml (shared por next-phase, github-project, backlog-to-issue).
#>
function Get-PhaseQueue {
    param([string]$Root)

    $path = Join-Path $Root 'docs/_meta/PHASE_QUEUE.yaml'
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
        } elseif ($current -and $line -match '^\s+blocked_by:\s*\[(.*)\]\s*$') {
            $inner = $Matches[1].Trim()
            if ($inner) {
                $current.blocked_by = $inner -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
            }
            $inBlockedList = $false
        } elseif ($current -and $line -match '^\s+blocked_by:\s*$') {
            $current.blocked_by = @()
            $inBlockedList = $true
        } elseif ($inBlockedList -and $line -match '^\s+-\s+(FASE\d+|doc-[\w-]+)\s*$') {
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

function Get-PhaseQueueItem {
    param([string]$Root, [string]$PhaseId)
    $item = Get-PhaseQueue -Root $Root | Where-Object { $_.id -eq $PhaseId } | Select-Object -First 1
    if (-not $item) { throw "Phase not in queue: $PhaseId" }
    return $item
}

function Get-ProjectConfig {
    param([string]$Root)
    $path = Join-Path $Root '.github/project/arah-sustentacao.yml'
    if (-not (Test-Path $path)) { return $null }
    $raw = Get-Content $path -Raw
    $cfg = @{ wave_milestones = @{}; project_number = $null; title = 'Arah Sustentação' }
    if ($raw -match '(?m)^title:\s*"(.+)"') { $cfg.title = $Matches[1] }
    if ($raw -match '(?m)^project_number:\s*(\d+)') { $cfg.project_number = [int]$Matches[1] }
    if ($raw -match '(?ms)^wave_milestones:\s*\n((?:  .+\r?\n)+)') {
        foreach ($line in ($Matches[1] -split "`n")) {
            if ($line -match '^\s+(S\d):\s*"(.+)"') {
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
        [string]$Owner
    )
    if ($ProjectNumber -le 0) { return $false }
    gh project item-add $ProjectNumber --owner $Owner --url $IssueUrl 2>$null | Out-Null
    return ($LASTEXITCODE -eq 0)
}

function Resolve-MilestoneForWave {
    param([string]$Root, [string]$Wave)
    if (-not $Wave) { return $null }
    $cfg = Get-ProjectConfig -Root $Root
    if (-not $cfg -or -not $cfg.wave_milestones.ContainsKey($Wave)) { return $null }
    return $cfg.wave_milestones[$Wave]
}

function Test-PhaseCompleteFromGitHub {
    param([string]$Root, [string]$PhaseId)

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

function Test-PhaseOpenIssue {
    param([string]$Root, [string]$PhaseId, [array]$OpenIssues = @())

    if ($OpenIssues.Count -eq 0 -and (Get-Command gh -ErrorAction SilentlyContinue)) {
        Push-Location $Root
        try {
            $OpenIssues = gh issue list --state open --limit 200 --json title,body,labels | ConvertFrom-Json
        } finally { Pop-Location }
    }

    foreach ($issue in $OpenIssues) {
        if ($issue.body -match "arah-next-phase id=$([regex]::Escape($PhaseId))") { return $true }
        if ($issue.title -match [regex]::Escape($PhaseId)) { return $true }
    }
    return $false
}
