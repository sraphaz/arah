#Requires -Version 5.1
<#
.SYNOPSIS
  Ativa agentes roteados pelo orquestrador — posta checklists e gera artifact de atividade.
.EXAMPLE
  ./run-agent-activation.ps1 -RouteFile route.json -IssueNumber 301 -Trigger "issue labeled" -PostComment
#>
param(
    [string]$RouteFile = 'route.json',
    [int]$IssueNumber = 0,
    [int]$PrNumber = 0,
    [string]$Trigger = 'orchestrate',
    [string]$RunId = $env:GITHUB_RUN_ID,
    [string]$RunUrl = $env:GITHUB_SERVER_URL,
    [string[]]$ChangedFiles = @(),
    [switch]$PostComment,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ScriptDir = $PSScriptRoot

if ($RunUrl -and $env:GITHUB_REPOSITORY -and $RunId) {
    $RunUrl = "$RunUrl/$env:GITHUB_REPOSITORY/actions/runs/$RunId"
}

if (-not (Test-Path $RouteFile)) {
    Write-Error "Route file not found: $RouteFile"
    exit 1
}

$route = Get-Content $RouteFile -Raw | ConvertFrom-Json
$agentList = @()

if ($route.agent -and $route.agent -ne 'orchestrator') {
    $agentList += $route.agent
}

foreach ($co in @($route.co_agents)) {
    if ($co -and $co -ne 'orchestrator' -and $agentList -notcontains $co) {
        $agentList += $co
    }
}

foreach ($pa in @($route.path_agents)) {
        if ($pa.agent -and $pa.agent -ne 'orchestrator' -and $agentList -notcontains $pa.agent) {
            $agentList += $pa.agent
        }
    }

    if ($PrNumber -gt 0) {
        foreach ($extra in @('qa', 'pr-steward')) {
            if ($agentList -notcontains $extra) { $agentList += $extra }
        }
    }

    $files = @($ChangedFiles)
if ($files.Count -eq 0 -and $route.path_agents) {
    foreach ($pa in @($route.path_agents)) {
        if ($pa.files) { $files += @($pa.files) }
    }
}
$files = @($files | Select-Object -Unique)

$activities = @()
foreach ($agentId in $agentList) {
    $params = @{
        AgentId      = $agentId
        Trigger      = $Trigger
        ChangedFiles = $files
        Json         = $true
    }
    if ($IssueNumber -gt 0) { $params.IssueNumber = $IssueNumber }
    if ($PrNumber -gt 0) { $params.PrNumber = $PrNumber }
    if ($RunId) { $params.RunId = $RunId }
    if ($RunUrl) { $params.RunUrl = $RunUrl }
    if ($PostComment) { $params.PostComment = $true }

    $out = & (Join-Path $ScriptDir 'post-agent-activity.ps1') @params
    $parsed = $out | ConvertFrom-Json
    $activities += $parsed
}

$summary = [ordered]@{
    run_id     = $RunId
    trigger    = $Trigger
    primary    = $route.agent
    activated  = @($agentList)
    activities = $activities
    timestamp  = (Get-Date).ToUniversalTime().ToString('o')
}

$outPath = Join-Path $Root 'agent-activity.json'
$summary | ConvertTo-Json -Depth 8 | Set-Content -Path $outPath -Encoding UTF8

if ($Json) {
    $summary | ConvertTo-Json -Depth 8
} else {
    Write-Host "Activated agents: $($agentList -join ', ')"
    Write-Host "Activity log: $outPath"
}

exit 0
