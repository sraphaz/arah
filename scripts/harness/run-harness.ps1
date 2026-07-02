#Requires -Version 5.1
<#
.SYNOPSIS
  Executa harness SDD: valida specs, agentes, guardrails, links e comandos ligados.
.EXAMPLE
  ./run-harness.ps1
  ./run-harness.ps1 -SpecId FASE53-arah-core -Json
#>
param(
    [string]$SpecId = '',
    [switch]$SkipTests,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$HarnessDir = $PSScriptRoot
$AgentsDir = Join-Path $Root 'scripts/agents'
$SpecsRoot = Join-Path $Root 'docs/specs'
$results = @()

function Add-Step {
    param([string]$Name, [bool]$Ok, [string]$Detail = '')
    $script:results += [ordered]@{ step = $Name; ok = $Ok; detail = $Detail }
}

function Get-YamlSectionBlock {
    param([string]$Raw, [string]$Key)
    if ($Raw -match "(?ms)^$Key\s*:\s*\n(.*?)(?=^[a-zA-Z_].*:|\z)") {
        return $Matches[1]
    }
    return ''
}

function Get-IndentedList {
    param([string]$Block, [string]$Key, [int]$Indent = 2)
    $items = @()
    $prefix = ' ' * $Indent
    $itemPrefix = ' ' * ($Indent + 2)
    $inList = $false
    foreach ($line in ($Block -split "`r?`n")) {
        if ($line -match "^$([regex]::Escape($prefix))$([regex]::Escape($Key)):\s*$") {
            $inList = $true
            continue
        }
        if (-not $inList) { continue }
        if ($line -match "^$([regex]::Escape($itemPrefix))- `"(.+)`"$") {
            $items += $Matches[1]
            continue
        }
        if ($line -match "^$([regex]::Escape($itemPrefix))- '(.+)'$") {
            $items += $Matches[1]
            continue
        }
        if ($line -match "^$([regex]::Escape($itemPrefix))- (.+)$") {
            $items += $Matches[1].Trim()
            continue
        }
        if ($line -match "^$([regex]::Escape($prefix))\S") { break }
        if ($line -match '^\S') { break }
    }
    return $items
}

function Test-Guardrail {
    param([string]$Id, [string]$RootPath)
    switch ($Id) {
        'no-secrets-in-repo' {
            $gitignore = Join-Path $RootPath '.gitignore'
            if (-not (Test-Path $gitignore)) { return $false, '.gitignore missing' }
            $gi = Get-Content $gitignore -Raw
            if ($gi -notmatch '(?m)^\.env') { return $false, '.gitignore must list .env' }
            return $true, 'ok'
        }
        'production-gate-human' {
            $wf = Join-Path $RootPath '.github/workflows/deploy-production.yml'
            if (-not (Test-Path $wf)) { return $false, 'deploy-production.yml missing' }
            $content = Get-Content $wf -Raw
            if ($content -notmatch 'confirm') { return $false, 'production deploy must require confirm input' }
            return $true, 'ok'
        }
        'clean-architecture' {
            $coreProj = Join-Path $RootPath 'backend/Arah.Core/Arah.Core.csproj'
            if (-not (Test-Path $coreProj)) { return $false, 'Arah.Core.csproj missing' }
            $refs = Get-Content $coreProj -Raw
            if ($refs -match 'Arah\.Api|Arah\.Infrastructure') {
                return $false, 'Arah.Core must not reference Api/Infrastructure'
            }
            return $true, 'ok'
        }
        'territory-data-stays-on-instance' {
            $coreDir = Join-Path $RootPath 'backend/Arah.Core'
            # Diretório federado (DirectoryTerritoryEntry) é permitido — só metadados, não dados sociais.
            $hits = Get-ChildItem -Path $coreDir -Recurse -Filter '*.cs' -ErrorAction SilentlyContinue |
                Select-String -Pattern '\bMembership\b|using Arah\.Domain\.(Territor|Membership)|\bclass Territory\b'
            if ($hits) { return $false, 'Arah.Core references territory/membership domain types' }
            return $true, 'ok'
        }
        'no-merge-automatic' {
            $manifest = Join-Path $RootPath '.agents/pr-steward.agent.yaml'
            if (-not (Test-Path $manifest)) { return $false, 'pr-steward manifest missing' }
            $content = Get-Content $manifest -Raw
            if ($content -notmatch '(?m)^guardrails:\s*\n(?:.*\n)*?\s+no_merge:\s*true') {
                return $false, 'pr-steward must set guardrails.no_merge: true'
            }
            return $true, 'ok'
        }
        'sync-docs-on-behavior-change' {
            $script = Join-Path $RootPath 'scripts/agents/sync-docs-check.ps1'
            if (-not (Test-Path $script)) { return $false, 'sync-docs-check.ps1 missing' }
            return $true, 'ok'
        }
        default { return $true, "unknown guardrail '$Id' (skipped)" }
    }
}

function Resolve-AgentManifest {
    param([string]$AgentId, [string]$RootPath)
    $candidates = @(
        (Join-Path $RootPath ".agents/$AgentId.agent.yaml"),
        (Join-Path $RootPath ".agents/domain/$AgentId.agent.yaml"),
        (Join-Path $RootPath ".agents/specialists/$AgentId.agent.yaml")
    )
    foreach ($c in $candidates) {
        if (Test-Path $c) { return $c.Replace($RootPath + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/') }
    }
    return $null
}

function Invoke-HarnessCommand {
    param([string]$Command)
    if ($Command -match '^(dotnet|npm|flutter)\s+') {
        if ($Command -match 'dotnet\s+build') {
            & (Join-Path $HarnessDir 'stop-arah-local-processes.ps1') | Out-Null
        }
        Invoke-Expression $Command
        if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) { throw "exit $LASTEXITCODE" }
        return
    }
    if ($Command -match '\.(ps1|sh|cmd)$') {
        $path = Join-Path $Root ($Command -replace '/', [IO.Path]::DirectorySeparatorChar)
        if (-not (Test-Path $path)) { throw "missing script $Command" }
        & $path
        return
    }
    $path = Join-Path $Root ($Command -replace '/', [IO.Path]::DirectorySeparatorChar)
    if (Test-Path $path) {
        if ($path -match '\.(md|html|yaml|yml|json|cs|csproj)$') { return }
        & $path
        return
    }
    throw "unsupported harness command: $Command"
}

# 1) Validate specs
try {
    & (Join-Path $HarnessDir 'validate-specs.ps1') | Out-Null
    Add-Step 'validate-specs' $true
} catch {
    Add-Step 'validate-specs' $false $_.Exception.Message
}

# 2) Agent manifests
try {
    & (Join-Path $AgentsDir 'validate-manifests.ps1') | Out-Null
    Add-Step 'validate-manifests' $true
} catch {
    Add-Step 'validate-manifests' $false $_.Exception.Message
}

# 3) Per-spec harness
$specFiles = Get-ChildItem -Path $SpecsRoot -Recurse -Filter '*.spec.yaml' |
    Where-Object { $_.Name -ne '_template.spec.yaml' }

foreach ($file in $specFiles) {
    $raw = Get-Content $file.FullName -Raw
    if ($raw -notmatch '(?m)^id:\s*(.+)$') { continue }
    $id = $Matches[1].Trim()
    if ($SpecId -and $id -ne $SpecId) { continue }

    # Specs draft ainda não têm implementação (spec-before-code): valida estrutura
    # (agents, guardrails, links), mas não executa scripts/commands da fase.
    $specStatus = if ($raw -match '(?m)^status:\s*(\S+)') { $Matches[1].Trim() } else { '' }
    $isDraft = $specStatus -eq 'draft'

    $harnessBlock = Get-YamlSectionBlock -Raw $raw -Key 'harness'
    $linksBlock = Get-YamlSectionBlock -Raw $raw -Key 'links'
    $guardrails = Get-IndentedList -Block $raw -Key 'guardrails' -Indent 0

    foreach ($agentId in (Get-IndentedList -Block $harnessBlock -Key 'agents')) {
        $manifest = Resolve-AgentManifest -AgentId $agentId -RootPath $Root
        if ($manifest) {
            Add-Step "agent:$id" $true $manifest
        } else {
            Add-Step "agent:$id" $false "missing agent manifest for '$agentId'"
        }
    }

    foreach ($gr in $guardrails) {
        $ok, $detail = Test-Guardrail -Id $gr -RootPath $Root
        Add-Step "guardrail:$id" $ok "$gr — $detail"
    }

    foreach ($linkKey in @('docs', 'handoff')) {
        $list = @()
        $in = $false
        foreach ($line in ($linksBlock -split "`r?`n")) {
            if ($line -match "^  $linkKey\s*:\s*$") { $in = $true; continue }
            if ($in -and $line -match '^    - (.+)$') { $list += $Matches[1].Trim().Trim('"'); continue }
            if ($in -and $line -match '^  \w') { break }
        }
        foreach ($rel in $list) {
            $path = Join-Path $Root ($rel -replace '/', [IO.Path]::DirectorySeparatorChar)
            if (Test-Path $path) {
                Add-Step "link:$id" $true $rel
            } else {
                Add-Step "link:$id" $false "missing $rel"
            }
        }
    }

    foreach ($rel in (Get-IndentedList -Block $harnessBlock -Key 'scripts')) {
        $path = Join-Path $Root ($rel -replace '/', [IO.Path]::DirectorySeparatorChar)
        if (-not (Test-Path $path)) {
            Add-Step "script:$id" $false "missing $rel"
            continue
        }
        if ($isDraft) {
            Add-Step "script:$id" $true "$rel — skipped (spec draft)"
            continue
        }
        try {
            if ($rel -match 'agent-conduct-check\.ps1$') {
                & $path -AllOperational | Out-Null
            } else {
                & $path | Out-Null
            }
            Add-Step "script:$id" $true $rel
        } catch {
            Add-Step "script:$id" $false "$rel — $($_.Exception.Message)"
        }
    }

    if (-not $SkipTests) {
        foreach ($cmd in (Get-IndentedList -Block $harnessBlock -Key 'commands')) {
            if ([string]::IsNullOrWhiteSpace($cmd)) { continue }
            if ($isDraft) {
                Add-Step "command:$id" $true "$cmd — skipped (spec draft)"
                continue
            }
            Push-Location $Root
            try {
                Invoke-HarnessCommand -Command $cmd
                Add-Step "command:$id" $true $cmd
            } catch {
                Add-Step "command:$id" $false "$cmd — $($_.Exception.Message)"
            } finally {
                Pop-Location
            }
        }
    }
}

$allOk = ($results | Where-Object { -not $_.ok }).Count -eq 0
$report = [ordered]@{
    spec_filter = $(if ($SpecId) { $SpecId } else { 'all' })
    steps       = $results
    ok          = $allOk
    timestamp   = (Get-Date).ToUniversalTime().ToString('o')
}

$outFile = Join-Path $Root 'harness-report.json'
$jsonText = $report | ConvertTo-Json -Depth 6 -Compress:$false
[System.IO.File]::WriteAllText($outFile, $jsonText, [System.Text.UTF8Encoding]::new($false))

if ($Json) { $jsonText } else {
    Write-Output $jsonText
    Write-Host "Report: $outFile"
}

if (-not $allOk) { exit 1 }
exit 0
