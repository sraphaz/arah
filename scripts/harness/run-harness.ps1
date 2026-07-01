#Requires -Version 5.1
<#
.SYNOPSIS
  Executa harness SDD: valida specs, agentes e comandos ligados.
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

# 1) Validate all specs (or one)
$valParams = @{}
if ($SpecId) { $valParams.SpecId = $SpecId }
if ($Json) { $valParams.Json = $true }
try {
    if ($SpecId) {
        & (Join-Path $HarnessDir 'validate-specs.ps1') @valParams | Out-Null
    } else {
        & (Join-Path $HarnessDir 'validate-specs.ps1') @valParams | Out-Null
    }
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

# 3) Spec-linked commands
$specFiles = Get-ChildItem -Path $SpecsRoot -Recurse -Filter '*.spec.yaml' |
    Where-Object { $_.Name -ne '_template.spec.yaml' }

foreach ($file in $specFiles) {
    $raw = Get-Content $file.FullName -Raw
    if ($raw -notmatch '(?m)^id:\s*(.+)$') { continue }
    $id = $Matches[1].Trim()
    if ($SpecId -and $id -ne $SpecId) { continue }

    if ($raw -match '(?ms)^harness:\s*\n(?:.*?\n)*?\s+commands:\s*\n((?:\s+-\s+.+\r?\n)+)') {
        $block = $Matches[1]
        $commands = [regex]::Matches($block, '^\s+-\s+"(.+)"|^\s+-\s+(.+)$', 'Multiline') |
            ForEach-Object { if ($_.Groups[1].Value) { $_.Groups[1].Value } else { $_.Groups[2].Value.Trim() } }

        if (-not $SkipTests) {
            foreach ($cmd in $commands) {
                if ([string]::IsNullOrWhiteSpace($cmd)) { continue }
                Push-Location $Root
                try {
                    Invoke-Expression $cmd
                    if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) { throw "exit $LASTEXITCODE" }
                    Add-Step "command:$id" $true $cmd
                } catch {
                    Add-Step "command:$id" $false "$cmd — $($_.Exception.Message)"
                } finally {
                    Pop-Location
                }
            }
        }
    }

    if ($raw -match '(?ms)^harness:\s*\n(?:.*?\n)*?\s+scripts:\s*\n((?:\s+-\s+.+\r?\n)+)') {
        $block = $Matches[1]
        $scripts = [regex]::Matches($block, '^\s+-\s+(.+)$', 'Multiline') |
            ForEach-Object { $_.Groups[1].Value.Trim() }
        foreach ($rel in $scripts) {
            $path = Join-Path $Root ($rel -replace '/', [IO.Path]::DirectorySeparatorChar)
            if (-not (Test-Path $path)) {
                Add-Step "script:$id" $false "missing $rel"
                continue
            }
            try {
                & $path | Out-Null
                Add-Step "script:$id" $true $rel
            } catch {
                Add-Step "script:$id" $false "$rel — $($_.Exception.Message)"
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
$report | ConvertTo-Json -Depth 6 | Set-Content -Path $outFile -Encoding UTF8

if ($Json) { $report | ConvertTo-Json -Depth 6 } else {
    $report | ConvertTo-Json -Depth 6
    Write-Host "Report: $outFile"
}

if (-not $allOk) { exit 1 }
exit 0
