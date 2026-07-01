#Requires -Version 5.1
<#
.SYNOPSIS
  CLI unificada para operação por agentes (orquestração, skills, validação).
.EXAMPLE
  ./arah-agents.ps1 orchestrate -Labels area/backend
.EXAMPLE
  ./arah-agents.ps1 orchestrate -EventPath $env:GITHUB_EVENT_PATH
.EXAMPLE
  ./arah-agents.ps1 route-pr -ChangedFiles backend/Arah.Api/Program.cs,frontend/arah.app/lib/main.dart
#>
param(
    [Parameter(Position = 0)]
    [ValidateSet('orchestrate', 'validate', 'skill', 'route-pr', 'ensure-labels', 'doc-index', 'doc-deflate', 'gates', 'bot-review', 'pr-ready', 'next-phase', 'help')]
    [string]$Command = 'help',

    [ValidateSet('issue', 'pull_request', 'issues', 'pull_request_target', 'manual', 'workflow_dispatch')]
    [string]$Event = 'manual',

    [string[]]$Labels = @(),
    [string]$EventPath = $env:GITHUB_EVENT_PATH,
    [string[]]$ChangedFiles = @(),
    [string]$Skill = '',
    [string]$Area = 'backend',
    [int]$Wave = 1,
    [int]$PrNumber = 0,
    [switch]$Json,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$AgentsDir = Join-Path $Root '.agents'

function Normalize-FileList {
    param([string[]]$Files)
    $normalized = @()
    foreach ($item in $Files) {
        if ($item -match ',') {
            $normalized += $item -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
        } elseif ($item) {
            $normalized += $item.Trim()
        }
    }
    return @($normalized | Select-Object -Unique)
}

function Get-RoutingTable {
    $orchestrator = Join-Path $AgentsDir 'orchestrator.agent.yaml'
    if (-not (Test-Path $orchestrator)) {
        throw "Missing orchestrator manifest: $orchestrator"
    }

    $routes = @{}
    foreach ($line in (Get-Content $orchestrator)) {
        if ($line -match '^\s+(area/\S+):\s+(\S+)') {
            $routes[$Matches[1]] = $Matches[2]
        }
    }
    return $routes
}

function Get-AgentManifestPath {
    param([string]$AgentId)
    $direct = Join-Path $AgentsDir "$AgentId.agent.yaml"
    if (Test-Path $direct) { return $direct }

    $nested = Get-ChildItem -Path $AgentsDir -Recurse -Filter "$AgentId.agent.yaml" -ErrorAction SilentlyContinue |
        Select-Object -First 1
    if ($nested) { return $nested.FullName }

    return $null
}

function Read-AgentSummary {
    param([string]$AgentId)

    $path = Get-AgentManifestPath -AgentId $AgentId
    if (-not $path) {
        return @{
            id      = $AgentId
            name    = $AgentId
            skills  = @()
            paths   = @()
            manifest = $null
        }
    }

    $raw = Get-Content $path -Raw
    $name = if ($raw -match '(?m)^name:\s*(.+)$') { $Matches[1].Trim() } else { $AgentId }
    $skills = @()
    if ($raw -match '(?ms)^skills:\s*\n((?:\s+-\s+\S+\r?\n)+)') {
        $skills = [regex]::Matches($Matches[1], '^\s+-\s+(\S+)', 'Multiline') |
            ForEach-Object { $_.Groups[1].Value }
    }

    $paths = @()
    if ($raw -match '(?ms)^scope:\s*\n(?:.*?\n)*?\s+paths:\s*\n((?:\s+-\s+.+\r?\n)+)') {
        $paths = [regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') |
            ForEach-Object { $_.Groups[1].Value.Trim() }
    }

    return @{
        id       = $AgentId
        name     = $name
        skills   = $skills
        paths    = $paths
        manifest = $path.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/')
    }
}

function Resolve-AgentFromLabels {
    param([string[]]$LabelList)

    $routes = Get-RoutingTable
    foreach ($label in $LabelList) {
        if ($routes.ContainsKey($label)) {
            return $routes[$label]
        }
    }
    return $null
}

function Parse-IssueBodyForArea {
    param([string]$Body)

    if ([string]::IsNullOrWhiteSpace($Body)) { return $null }
    if ($Body -match '(?m)^###\s*Área\s*\r?\n\r?\n(area/\S+)') { return $Matches[1] }
    if ($Body -match '(?m)^area/(backend|flutter|web|docs|ops|security|planning)\b') { return $Matches[0].Trim() }
    return $null
}

function Read-GitHubEvent {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path $Path)) {
        return @{
            eventName = $Event
            labels    = @($Labels)
            body      = ''
            number    = 0
            changed   = @()
        }
    }

    $payload = Get-Content $Path -Raw | ConvertFrom-Json
    $eventName = if ($env:GITHUB_EVENT_NAME) { $env:GITHUB_EVENT_NAME } else { $Event }
    $labelList = @()
    $body = ''
    $number = 0
    $changed = @()

    if ($payload.issue) {
        $number = [int]$payload.issue.number
        $body = [string]$payload.issue.body
        if ($payload.issue.labels) {
            $labelList = @($payload.issue.labels | ForEach-Object { $_.name })
        }
        if ($payload.label -and $payload.label.name) {
            if ($labelList -notcontains $payload.label.name) {
                $labelList += $payload.label.name
            }
        }
    }

    if ($payload.pull_request) {
        $number = [int]$payload.pull_request.number
        $body = [string]$payload.pull_request.body
        if ($payload.pull_request.labels) {
            $labelList = @($payload.pull_request.labels | ForEach-Object { $_.name })
        }
    }

    if ($payload.pull_request -and $payload.pull_request.changed_files) {
        $changed = @($payload.pull_request.changed_files)
    }

    return @{
        eventName = $eventName
        labels    = $labelList
        body      = $body
        number    = $number
        changed   = $changed
    }
}

function Test-PathMatchesGlob {
    param(
        [string]$FilePath,
        [string]$Pattern
    )

    $normalized = $FilePath.Replace('\', '/')
    $glob = $Pattern.Replace('\', '/')

    if ($glob -eq '**' -or $glob -eq '**/*') { return $true }
    if ($glob.EndsWith('/**')) {
        $prefix = $glob.Substring(0, $glob.Length - 3)
        return $normalized.StartsWith("$prefix/")
    }
    if ($glob.EndsWith('**')) {
        $prefix = $glob.TrimEnd('*').TrimEnd('/')
        return $normalized.StartsWith($prefix)
    }

    $regex = '^' + [regex]::Escape($glob).Replace('\*\*', '.*').Replace('\*', '[^/]*') + '$'
    return $normalized -match $regex
}

function Resolve-AgentsFromPaths {
    param([string[]]$Files)

    if ($Files.Count -eq 0) { return @() }

    $agents = @{}
    Get-ChildItem -Path $AgentsDir -Recurse -Filter '*.agent.yaml' | ForEach-Object {
        $raw = Get-Content $_.FullName -Raw
        if ($raw -notmatch '(?m)^id:\s*(\S+)') { return }
        $agentId = $Matches[1]
        if ($agentId -in @('orchestrator', 'qa')) { return }

        $paths = @()
        if ($raw -match '(?ms)^scope:\s*\n(?:.*?\n)*?\s+paths:\s*\n((?:\s+-\s+.+\r?\n)+)') {
            $paths = [regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') |
                ForEach-Object { $_.Groups[1].Value.Trim() }
        }
        if ($paths.Count -eq 0) { return }

        foreach ($file in $Files) {
            foreach ($pattern in $paths) {
                if (Test-PathMatchesGlob -FilePath $file -Pattern $pattern) {
                    if (-not $agents.ContainsKey($agentId)) {
                        $agents[$agentId] = @()
                    }
                    if ($agents[$agentId] -notcontains $file) {
                        $agents[$agentId] += $file
                    }
                }
            }
        }
    }

    return $agents.GetEnumerator() | Sort-Object Name | ForEach-Object {
        @{
            agent  = $_.Key
            files  = $_.Value
            summary = Read-AgentSummary -AgentId $_.Key
        }
    }
}

function Invoke-Orchestrate {
    $gh = Read-GitHubEvent -Path $EventPath
    $effectiveEvent = if ($Event -ne 'manual') { $Event } else { $gh.eventName }
    $labelList = @($Labels)
    if ($labelList.Count -eq 0) {
        $labelList = @($gh.labels)
    }

    $areaFromBody = Parse-IssueBodyForArea -Body $gh.body
    if ($areaFromBody -and $labelList -notcontains $areaFromBody) {
        $labelList += $areaFromBody
    }

    $files = Normalize-FileList -Files $ChangedFiles
    $agentId = Resolve-AgentFromLabels -LabelList $labelList
    $pathAgents = @()
    if (-not $agentId -and $files.Count -gt 0) {
        $pathAgents = @(Resolve-AgentsFromPaths -Files $files)
        if ($pathAgents.Count -eq 1) {
            $agentId = $pathAgents[0].agent
        }
    }
    if (-not $agentId -and $gh.changed.Count -gt 0) {
        $pathAgents = @(Resolve-AgentsFromPaths -Files (Normalize-FileList -Files $gh.changed))
        if ($pathAgents.Count -eq 1) {
            $agentId = $pathAgents[0].agent
        }
    }

    $resolvedId = if ($agentId) { $agentId } else { 'orchestrator' }
    $summary = Read-AgentSummary -AgentId $resolvedId

    $message = if ($agentId) {
        "Delegate to agent '$agentId' ($($summary.name)). Read AGENTS.md and apply skills from .skills/."
    } elseif ($pathAgents.Count -gt 1) {
        $names = ($pathAgents | ForEach-Object { $_.agent }) -join ', '
        "Multiple agents match changed paths: $names. Add an area/* label or split the PR."
    } else {
        'No area/* label matched. Add area/backend|flutter|web|docs|ops|security|planning or label agent-task with Área filled.'
    }

    $result = [ordered]@{
        event         = $effectiveEvent
        issue_number  = $gh.number
        labels        = $labelList
        agent         = $resolvedId
        agent_name    = $summary.name
        manifest      = $summary.manifest
        skills        = @($summary.skills)
        path_agents   = @($pathAgents | ForEach-Object {
            @{
                agent = $_.agent
                files = $_.files
            }
        })
        suggest_label = $areaFromBody
        message       = $message
        dry_run       = [bool]$DryRun
    }

    if ($Json) {
        $result | ConvertTo-Json -Depth 6
    } else {
        $result | ConvertTo-Json -Depth 6
        Write-Host $message
    }

    exit 0
}

function Invoke-RoutePr {
    $files = Normalize-FileList -Files $ChangedFiles
    if ($files.Count -eq 0) {
        Write-Error 'route-pr requires -ChangedFiles'
        exit 1
    }

    $matches = @(Resolve-AgentsFromPaths -Files $files)
    $output = [ordered]@{
        changed_files = $files
        agents        = @($matches | ForEach-Object {
            @{
                agent   = $_.agent
                name    = $_.summary.name
                files   = $_.files
                skills  = $_.summary.skills
                manifest = $_.summary.manifest
            }
        })
    }

    $output | ConvertTo-Json -Depth 6
    if ($matches.Count -eq 0) { exit 1 }
    exit 0
}

function Show-Help {
    @"
arah-agents — CLI de operação por agentes (Arah)

Comandos:
  orchestrate   Roteia labels/evento GitHub para agente
  route-pr      Infere agentes a partir de arquivos alterados
  skill         Executa skill (delega invoke-skill.ps1)
  validate      Valida manifests .agents/ e .skills/
  ensure-labels Imprime comandos gh para criar labels area/*
  doc-index     Regenera docs/INDEX.generated.md
  doc-deflate   Arquiva PR/RESUMO/ANALISE da raiz docs/ (wave 1)
  gates         Executa run-gates.ps1 (qa/security/release)
  bot-review    Audita apontamentos de bots em um PR (obrigatório antes merge)
  pr-ready      Verifica CI + bots; indica ready-for-merge
  next-phase    Abre issue [Agent] da próxima fase (PHASE_QUEUE.yaml)

Exemplos:
  ./arah-agents.ps1 orchestrate -Labels area/backend
  ./arah-agents.ps1 bot-review -PrNumber 300
  ./arah-agents.ps1 pr-ready -PrNumber 300
  ./arah-agents.ps1 next-phase -DryRun
  ./arah-agents.ps1 orchestrate -EventPath `$env:GITHUB_EVENT_PATH -Json
  ./arah-agents.ps1 route-pr -ChangedFiles backend/Arah.Api/Program.cs
  ./arah-agents.ps1 skill -Skill run-tests -Area backend
  ./arah-agents.ps1 validate

Documentação: AGENTS.md, docs/ops/AGENT_OPERATION.md
"@
}

switch ($Command) {
    'orchestrate' { Invoke-Orchestrate }
    'route-pr'    { Invoke-RoutePr }
    'skill' {
        if (-not $Skill) {
            Write-Error 'skill requires -Skill'
            exit 1
        }
        & (Join-Path $PSScriptRoot 'invoke-skill.ps1') -Skill $Skill -Area $Area
    }
    'validate' {
        & (Join-Path $PSScriptRoot 'validate-manifests.ps1')
    }
    'ensure-labels' {
        $areas = @(
            @{ name = 'agent-task'; color = '0E8A16'; description = 'Tarefa operada por agente' },
            @{ name = 'area/backend'; color = '1D76DB'; description = 'Backend .NET / BFF' },
            @{ name = 'area/flutter'; color = 'FBCA04'; description = 'App Flutter arah.app' },
            @{ name = 'area/web'; color = '5319E7'; description = 'Wiki, portal, devportal' },
            @{ name = 'area/docs'; color = '0075CA'; description = 'Documentação e taxonomia' },
            @{ name = 'area/ops'; color = 'D93F0B'; description = 'CI/CD, release, infra' },
            @{ name = 'area/security'; color = 'B60205'; description = 'Segurança e compliance' },
            @{ name = 'area/planning'; color = 'C5DEF5'; description = 'Backlog e planejamento' },
            @{ name = 'area/pr-review'; color = '1B7F37'; description = 'PR Steward — review e bots' },
            @{ name = 'ready-for-merge'; color = '0E8A16'; description = 'PR aprovado pelo steward; humano mergeia' }
        )
        foreach ($label in $areas) {
            Write-Host "gh label create `"$($label.name)`" --color $($label.color) --description `"$($label.description)`" --force"
        }
    }
    'doc-index' {
        & (Join-Path $PSScriptRoot 'generate-docs-index.ps1')
    }
    'doc-deflate' {
        $params = @{ Wave = $Wave }
        if ($DryRun) { $params.DryRun = $true }
        & (Join-Path $PSScriptRoot 'migrate-docs-deflate.ps1') @params
        & (Join-Path $PSScriptRoot 'generate-docs-index.ps1')
    }
    'gates' {
        $params = @{}
        if ($ChangedFiles.Count -gt 0) { $params.ChangedFiles = $ChangedFiles }
        & (Join-Path $PSScriptRoot 'run-gates.ps1') @params
    }
    'bot-review' {
        if ($PrNumber -le 0) {
            Write-Error 'bot-review requires -PrNumber'
            exit 1
        }
        $params = @{ PrNumber = $PrNumber }
        if ($Json) { $params.Json = $true }
        & (Join-Path $PSScriptRoot 'address-bot-review.ps1') @params
    }
    'pr-ready' {
        if ($PrNumber -le 0) {
            Write-Error 'pr-ready requires -PrNumber'
            exit 1
        }
        $params = @{ PrNumber = $PrNumber }
        if ($Json) { $params.Json = $true }
        & (Join-Path $PSScriptRoot 'pr-ready.ps1') @params
    }
    'next-phase' {
        $params = @{}
        if ($DryRun) { $params.DryRun = $true }
        if ($Json) { $params.Json = $true }
        & (Join-Path $PSScriptRoot 'next-phase.ps1') @params
    }
    default { Show-Help }
}
