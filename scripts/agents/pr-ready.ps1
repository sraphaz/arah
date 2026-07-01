#Requires -Version 5.1
<#
.SYNOPSIS
  Verifica se PR está pronto para merge (CI + bot review audit).
#>
param(
    [Parameter(Mandatory)]
    [int]$PrNumber,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$ScriptDir = $PSScriptRoot

$auditJson = & (Join-Path $ScriptDir 'address-bot-review.ps1') -PrNumber $PrNumber -Json | Out-String
$audit = ($auditJson -split "`n" | Where-Object { $_ -match '^\s*\{' }) -join "`n" | ConvertFrom-Json

$gatesOk = $true
try {
    & (Join-Path $ScriptDir 'run-gates.ps1') -Gate qa 2>&1 | Out-Null
} catch {
    $gatesOk = $false
}

$ready = $audit.ready -and ($audit.failed_checks.Count -eq 0) -and ($audit.bot_comments -eq 0)
$result = [ordered]@{
    pr              = $PrNumber
    ci_ready        = $audit.ci_ready
    bot_comments    = $audit.bot_comments
    bots_pending    = $audit.bots_pending
    gates_local     = $gatesOk
    ready_for_merge = $ready
    recommendation  = if ($ready) {
        'Adicionar label ready-for-merge; humano pode mergear após revisão final.'
    } elseif ($audit.bot_comments -gt 0) {
        'Resolver ou responder todos os apontamentos de bots antes do merge.'
    } else {
        'Corrigir checks falhos antes do merge.'
    }
}

if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4 }
if (-not $ready) { exit 1 }
exit 0
