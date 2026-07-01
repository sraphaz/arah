#Requires -Version 5.1
<#
.SYNOPSIS
  Verificações automáticas de conduta do agente (escopo, guardrails).
.EXAMPLE
  ./agent-conduct-check.ps1 -AgentId backend -ChangedFiles backend/Arah.Api/Program.cs
#>
param(
    [string]$AgentId = '',
    [switch]$AllOperational,
    [string[]]$ChangedFiles = @(),
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$AgentsDir = Join-Path $Root '.agents'

function Get-AgentManifestPath {
    param([string]$Id)
    $direct = Join-Path $AgentsDir "$Id.agent.yaml"
    if (Test-Path $direct) { return $direct }
    $nested = Get-ChildItem -Path $AgentsDir -Recurse -Filter "$Id.agent.yaml" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($nested) { return $nested.FullName }
    return $null
}

function Test-PathMatchesGlob {
    param([string]$FilePath, [string]$Pattern)
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

$manifestPath = $null
if ($AllOperational) {
    $failures = @()
    Get-ChildItem -Path $AgentsDir -Recurse -Filter '*.agent.yaml' | ForEach-Object {
        $raw = Get-Content $_.FullName -Raw
        if ($raw -notmatch '(?m)^id:\s*(\S+)') { return }
        $id = $Matches[1].Trim()
        if ($id -in @('orchestrator')) { return }
        if ($raw -match '(?m)^mode:\s*consult' -or $raw -match '(?m)^consult_only:\s*true') { return }
        try {
            & $PSCommandPath -AgentId $id -ChangedFiles $ChangedFiles | Out-Null
            if ($LASTEXITCODE -ne 0) { $failures += $id }
        } catch {
            $failures += $id
        }
    }
    if ($failures.Count -gt 0) {
        Write-Error "Conduct check failed for: $($failures -join ', ')"
        exit 1
    }
    Write-Host 'All operational agent conduct checks passed'
    exit 0
}

if ([string]::IsNullOrWhiteSpace($AgentId)) {
    Write-Error 'AgentId required unless -AllOperational'
    exit 1
}

$manifestPath = Get-AgentManifestPath -Id $AgentId
if (-not $manifestPath) {
    Write-Error "Agent manifest not found: $AgentId"
    exit 1
}

$raw = Get-Content $manifestPath -Raw
$name = if ($raw -match '(?m)^name:\s*(.+)$') { $Matches[1].Trim() } else { $AgentId }
$isConsult = ($raw -match '(?m)^mode:\s*consult') -or ($raw -match '(?m)^consult_only:\s*true')

$paths = @()
$scopeBlock = ''
if ($raw -match '(?ms)^scope:\s*\n(.*?)(?=^[a-zA-Z_].*:|\z)') {
    $scopeBlock = $Matches[1]
    if ($scopeBlock -match '(?ms)^  paths:\s*\n((?:    - .+\r?\n?)+)') {
        $paths = [regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') | ForEach-Object {
            $_.Groups[1].Value.Trim().Trim('"').Trim("'")
        }
    }
}

$guardrails = @{}
if ($raw -match '(?ms)^guardrails:\s*\n((?:\s+\S+.+\r?\n)+)') {
    foreach ($line in ($Matches[1] -split "`n")) {
        if ($line -match '^\s+(\S+):\s*(.+)$') {
            $guardrails[$Matches[1]] = $Matches[2].Trim()
        }
    }
}

$checklistFile = if ($raw -match '(?m)^checklist:\s*(.+)$') { $Matches[1].Trim() } else { "checklists/$AgentId.checklist.md" }
$checklistPath = Join-Path $AgentsDir $checklistFile
$hasChecklist = Test-Path $checklistPath

$files = @($ChangedFiles | ForEach-Object { $_.Trim() } | Where-Object { $_ })
$scopeViolations = @()
$scopeOk = $true

if ($files.Count -gt 0 -and $paths.Count -gt 0 -and -not ($paths -contains '**')) {
    foreach ($file in $files) {
        $allowed = $false
        foreach ($pattern in $paths) {
            if (Test-PathMatchesGlob -FilePath $file -Pattern $pattern) {
                $allowed = $true
                break
            }
        }
        if (-not $allowed) {
            $scopeViolations += $file
        }
    }
    $scopeOk = ($scopeViolations.Count -eq 0)
}

$checks = [ordered]@{
    no_merge          = @{ ok = ($guardrails.no_merge -eq 'true'); label = 'Guardrail no_merge' }
    require_ci        = @{ ok = $true; label = 'Guardrail require_ci (opcional por agente)' }
    checklist_exists  = @{ ok = if ($isConsult) { $true } else { $hasChecklist }; label = "Checklist de conduta ($checklistFile)" }
    scope_respected   = @{
        ok    = if ($isConsult -or $files.Count -eq 0) { $true } elseif ($paths.Count -eq 0 -or ($paths -contains '**')) { $true } else { $scopeOk }
        label = 'Escopo de paths respeitado'
        detail = if ($scopeViolations.Count -gt 0) { ($scopeViolations -join ', ') } else { $null }
    }
}

$allAutoOk = ($checks.Values | ForEach-Object { $_.ok } | Where-Object { -not $_ }).Count -eq 0

$result = [ordered]@{
    agent           = $AgentId
    agent_name      = $name
    manifest        = $manifestPath.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/')
    checklist       = if ($hasChecklist) { $checklistFile } else { $null }
    changed_files   = $files
    scope_paths     = $paths
    guardrails      = $guardrails
    automated_checks = $checks
    conduct_ok      = $allAutoOk
}

if ($Json) {
    $result | ConvertTo-Json -Depth 6
} else {
    $result | ConvertTo-Json -Depth 6
}

if (-not $allAutoOk) { exit 1 }
exit 0
