#Requires -Version 5.1
<#
.SYNOPSIS
  Ativa agentes roteados + coreografia (domínio consultivo, skills autônomas).
#>
param(
    [string]$RouteFile = 'route.json',
    [int]$IssueNumber = 0,
    [int]$PrNumber = 0,
    [string]$Trigger = 'orchestrate',
    [string]$RunId = $env:GITHUB_RUN_ID,
    [string]$RunUrl = $env:GITHUB_SERVER_URL,
    [string[]]$ChangedFiles = @(),
    [switch]$ExecuteAutonomy,
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

$files = @($ChangedFiles)
if ($files.Count -eq 0 -and $route.path_agents) {
    foreach ($pa in @($route.path_agents)) {
        if ($pa.files) { $files += @($pa.files) }
    }
}
$files = @($files | Select-Object -Unique)

$choreoParams = @{
    ChangedFiles    = $files
    RouteFile       = $RouteFile
    Trigger         = $Trigger
    ExecuteAutonomy = [bool]$ExecuteAutonomy
    Json            = $true
}
$choreoRaw = & (Join-Path $ScriptDir 'choreograph-agents.ps1') @choreoParams
$choreo = $choreoRaw | ConvertFrom-Json

$agentList = @($choreo.operational)
$domainList = @($choreo.domain_consults)

$activities = @()
foreach ($agentId in $agentList) {
    if ($agentId -eq 'orchestrator') { continue }
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
    $activities += ($out | ConvertFrom-Json)
}

foreach ($domainId in $domainList) {
    $params = @{
        DomainId     = $domainId
        Trigger      = $Trigger
        ChangedFiles = $files
        Json         = $true
    }
    if ($IssueNumber -gt 0) { $params.IssueNumber = $IssueNumber }
    if ($PrNumber -gt 0) { $params.PrNumber = $PrNumber }
    if ($PostComment) { $params.PostComment = $true }

    $out = & (Join-Path $ScriptDir 'post-domain-consult.ps1') @params
    $activities += ($out | ConvertFrom-Json)
}

$summary = [ordered]@{
    run_id          = $RunId
    trigger         = $Trigger
    primary         = $route.agent
    activated       = @($agentList)
    domain_consults = @($domainList)
    choreography    = @{
        matched_rules   = $choreo.matched_rules
        executed_skills = $choreo.executed_skills
    }
    activities      = $activities
    timestamp       = (Get-Date).ToUniversalTime().ToString('o')
}

$outPath = Join-Path $Root 'agent-activity.json'
$summary | ConvertTo-Json -Depth 10 | Set-Content -Path $outPath -Encoding UTF8

if ($Json) {
    $summary | ConvertTo-Json -Depth 10
} else {
    Write-Host "Activated: $($agentList -join ', ')"
    if ($domainList.Count -gt 0) { Write-Host "Domain consults: $($domainList -join ', ')" }
    Write-Host "Activity log: $outPath"
}

exit 0
