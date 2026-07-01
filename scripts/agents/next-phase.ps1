#Requires -Version 5.1
<#
.SYNOPSIS
  Abre issue Agent Task para a próxima fase desbloqueada (PHASE_QUEUE.yaml).
.EXAMPLE
  ./next-phase.ps1 -DryRun
#>
param(
    [switch]$DryRun,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$QueueFile = Join-Path $Root 'docs/_meta/PHASE_QUEUE.yaml'

if (-not (Test-Path $QueueFile)) {
    Write-Error "Missing $QueueFile"
    exit 1
}

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

function Get-SimpleYamlQueue {
    param([string]$Path)
    $items = @()
    $current = $null
    $inBlockedList = $false
    foreach ($line in Get-Content $Path) {
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
        } elseif ($current -and -not $inBlockedList) {
            $inBlockedList = $false
        }
    }
    if ($current) { $items += $current }
    return $items
}

Push-Location $Root
try {
    $queue = Get-SimpleYamlQueue -Path $QueueFile
    $openIssues = gh issue list --state open --limit 200 --json title,labels,body | ConvertFrom-Json

    function Test-PhaseComplete {
        param([string]$PhaseId)
        if ($PhaseId -match '^FASE(\d+)$') {
            $status = Get-Content (Join-Path $Root 'docs/STATUS_FASES.md') -Raw -ErrorAction SilentlyContinue
            if ($status -match "$PhaseId[^\n]*\|\s*✅") { return $true }
        }
        return $false
    }

    function Test-PhaseOpenIssue {
        param([string]$PhaseId)
        foreach ($issue in $openIssues) {
            if ($issue.title -match [regex]::Escape($PhaseId)) { return $true }
            if ($issue.body -and $issue.body -match [regex]::Escape($PhaseId)) { return $true }
        }
        return $false
    }

    $selected = $null
    foreach ($item in $queue) {
        $blocked = $false
        foreach ($dep in $item.blocked_by) {
            if (-not (Test-PhaseComplete -PhaseId $dep)) {
                if ($dep -match '^FASE') { $blocked = $true; break }
            }
        }
        if ($blocked) { continue }
        if (Test-PhaseOpenIssue -PhaseId $item.id) { continue }
        if ($item.id -match '^FASE' -and (Test-PhaseComplete -PhaseId $item.id)) { continue }
        $selected = $item
        break
    }

    if (-not $selected) {
        $msg = 'Nenhuma fase desbloqueada pendente de issue (fila vazia ou issues já abertas).'
        if ($Json) { @{ message = $msg } | ConvertTo-Json } else { Write-Host $msg }
        exit 0
    }

    $areaLabel = if ($selected.area) { $selected.area } else { 'area/planning' }
    $body = @"
## Próxima fase (automático — PR Steward)

**Fase**: $($selected.id)
**Prioridade**: $($selected.priority)
**Documento**: [$($selected.doc)]($($selected.doc))

### Intenção
Implementar $($selected.title) conforme documento de fase e critérios de aceite.

### Critérios de aceite
- [ ] Itens da fase documentados em ``$($selected.doc)``
- [ ] ``run-tests`` passa na área afetada
- [ ] ``sync-docs`` aplicado
- [ ] Apontamentos de bots resolvidos no PR
- [ ] Merge humano após ``ready-for-merge``

### Guardrails
- [ ] Sem merge automático
- [ ] Escopo limitado à área: ``$areaLabel``

---
<!-- arah-next-phase id=$($selected.id) -->
"@

    $result = [ordered]@{
        phase   = $selected.id
        title   = $selected.title
        area    = $areaLabel
        doc     = $selected.doc
        dry_run = [bool]$DryRun
    }

    if ($DryRun) {
        $result.message = "Would create issue: [Agent] $($selected.title)"
        if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4; Write-Host $result.message }
        exit 0
    }

    $title = "[Agent] $($selected.title)"
    $issueUrl = gh issue create --title $title --body $body --label "agent-task" --label $areaLabel 2>&1
    $result.issue = $issueUrl
    $result.message = "Issue criada: $issueUrl"

    if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4; Write-Host $result.message }
    exit 0
} finally {
    Pop-Location
}
