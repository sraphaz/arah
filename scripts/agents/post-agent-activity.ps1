#Requires -Version 5.1
<#
.SYNOPSIS
  Monta e publica comentário de atividade do agente (checklist + verificações automáticas).
.EXAMPLE
  ./post-agent-activity.ps1 -AgentId backend -IssueNumber 301 -Trigger "issue labeled"
#>
param(
    [Parameter(Mandatory)]
    [string]$AgentId,
    [int]$IssueNumber = 0,
    [int]$PrNumber = 0,
    [string]$Trigger = 'manual',
    [string]$RunId = '',
    [string]$RunUrl = '',
    [string[]]$ChangedFiles = @(),
    [switch]$PostComment,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$AgentsDir = Join-Path $Root '.agents'
$ScriptDir = $PSScriptRoot

function Get-AgentManifestPath {
    param([string]$Id)
    $direct = Join-Path $AgentsDir "$Id.agent.yaml"
    if (Test-Path $direct) { return $direct }
    Get-ChildItem -Path $AgentsDir -Recurse -Filter "$Id.agent.yaml" -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
}

$manifestPath = Get-AgentManifestPath -Id $AgentId
if (-not $manifestPath) {
    Write-Error "Agent not found: $AgentId"
    exit 1
}

$raw = Get-Content $manifestPath -Raw
$name = if ($raw -match '(?m)^name:\s*(.+)$') { $Matches[1].Trim() } else { $AgentId }
$skills = @()
if ($raw -match '(?ms)^skills:\s*\n((?:\s+-\s+\S+\r?\n)+)') {
    $skills = [regex]::Matches($Matches[1], '^\s+-\s+(\S+)', 'Multiline') | ForEach-Object { $_.Groups[1].Value }
}

$checklistRel = if ($raw -match '(?m)^checklist:\s*(.+)$') { $Matches[1].Trim() } else { "checklists/$AgentId.checklist.md" }
$checklistPath = Join-Path $AgentsDir $checklistRel
$sharedPath = Join-Path $AgentsDir 'checklists/_shared.conduct.md'

$checklistBody = if (Test-Path $checklistPath) { Get-Content $checklistPath -Raw } else { "_Checklist não encontrada: $checklistRel_" }
$sharedBody = if (Test-Path $sharedPath) { Get-Content $sharedPath -Raw } else { '' }

$conductRaw = & (Join-Path $ScriptDir 'agent-conduct-check.ps1') -AgentId $AgentId -ChangedFiles $ChangedFiles -Json
$conduct = $conductRaw | ConvertFrom-Json

$autoLines = @()
foreach ($key in $conduct.automated_checks.PSObject.Properties.Name) {
    $item = $conduct.automated_checks.$key
    $icon = if ($item.ok) { '✅' } else { '❌' }
    $line = "$icon $($item.label)"
    if ($item.detail) { $line += " — fora do escopo: $($item.detail)" }
    $autoLines += $line
}

$skillList = if ($skills.Count -gt 0) {
    $n = 1
    ($skills | ForEach-Object { "$n. ``$_``"; $n++ }) -join "`n"
} else {
    '_nenhuma skill listada_'
}

$timestamp = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
$targetNumber = if ($PrNumber -gt 0) { $PrNumber } else { $IssueNumber }
$targetKind = if ($PrNumber -gt 0) { 'PR' } else { 'Issue' }

$runLine = if ($RunUrl) { "**Workflow:** [$RunId]($RunUrl)" } elseif ($RunId) { "**Run ID:** ``$RunId``" } else { '' }

$body = @"
<!-- arah-agent-activity:$AgentId -->
## Agente acionado: $name

**ID:** ``$AgentId`` | **Quando:** $timestamp UTC | **Gatilho:** $Trigger
$runLine

### Verificações automáticas de conduta
$($autoLines -join "`n")

### Conduta compartilhada
$sharedBody

### Checklist do agente
$checklistBody

### Skills sugeridas (ordem)
$skillList

### Manifest
``$($manifestPath.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/'))``

---
_Visível via ``agents.yml`` — artifact ``agent-activity.json``. Merge continua humano._
"@

$result = [ordered]@{
    agent         = $AgentId
    agent_name    = $name
    timestamp_utc = $timestamp
    trigger       = $Trigger
    target        = @{ kind = $targetKind; number = $targetNumber }
    conduct_ok    = $conduct.conduct_ok
    skills        = $skills
    body          = $body
    marker        = "<!-- arah-agent-activity:$AgentId -->"
}

if ($PostComment) {
    if ($targetNumber -le 0) {
        Write-Error 'PostComment requires -IssueNumber or -PrNumber'
        exit 1
    }
    if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
        Write-Error 'gh CLI required for -PostComment'
        exit 1
    }

    Push-Location $Root
    try {
        $repo = gh repo view --json nameWithOwner -q .nameWithOwner
        $existing = gh api "repos/$repo/issues/$targetNumber/comments" --paginate 2>$null | ConvertFrom-Json
        if ($existing -isnot [array]) { $existing = @($existing) }
        $match = $existing | Where-Object { $_.body -and $_.body -match [regex]::Escape($result.marker) } | Select-Object -First 1

        if ($match) {
            gh api -X PATCH "repos/$repo/issues/comments/$($match.id)" -f body="$body" | Out-Null
            $result.comment_action = 'updated'
            $result.comment_id = $match.id
        } else {
            gh issue comment $targetNumber --body "$body" | Out-Null
            $result.comment_action = 'created'
        }
    } finally {
        Pop-Location
    }
}

if ($Json) {
    $out = [ordered]@{
        agent         = $result.agent
        agent_name    = $result.agent_name
        timestamp_utc = $result.timestamp_utc
        trigger       = $result.trigger
        target        = $result.target
        conduct_ok    = $result.conduct_ok
        skills        = $result.skills
        marker        = $result.marker
    }
    if ($result.comment_action) { $out.comment_action = $result.comment_action }
    if ($result.comment_id) { $out.comment_id = $result.comment_id }
    $out | ConvertTo-Json -Depth 6
} else {
    Write-Host $body
}

exit 0
