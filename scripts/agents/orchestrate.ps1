#Requires -Version 5.1
<#
.SYNOPSIS
  Roteia labels de issue para agente (wrapper legado — prefira arah-agents.ps1).
.EXAMPLE
  ./orchestrate.ps1 -Labels area/backend,agent-task
#>
param(
    [string[]]$Labels = @(),
    [ValidateSet('issue', 'pull_request', 'manual')]
    [string]$Event = 'manual'
)

$params = @{
    Command = 'orchestrate'
    Event   = $Event
    Labels  = $Labels
}
& (Join-Path $PSScriptRoot 'arah-agents.ps1') @params
