#Requires -Version 5.1
<#
.SYNOPSIS
  Executa gates de QA, Security e Release em PRs de agentes.
.EXAMPLE
  ./run-gates.ps1 -Gate all
#>
param(
    [ValidateSet('qa', 'security', 'release', 'all')]
    [string]$Gate = 'all',
    [string[]]$ChangedFiles = @(),
    [string]$BaseRef = 'origin/main'
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ScriptDir = $PSScriptRoot
$failures = 0

function Invoke-Step {
    param([string]$Name, [scriptblock]$Action)
    Write-Host "`n=== Gate: $Name ==="
    try {
        & $Action
        Write-Host "$Name — OK"
    } catch {
        Write-Warning "$Name — FALHOU: $_"
        $script:failures++
    }
}

Push-Location $Root
try {
    if ($Gate -in 'qa', 'all') {
        Invoke-Step 'sync-docs-check' {
            & (Join-Path $ScriptDir 'sync-docs-check.ps1') -ChangedFiles $ChangedFiles -BaseRef $BaseRef
        }
        Invoke-Step 'register-bff-journey-check' {
            & (Join-Path $ScriptDir 'register-bff-journey-check.ps1') -ChangedFiles $ChangedFiles -BaseRef $BaseRef
        }
        Invoke-Step 'validate-manifests' {
            & (Join-Path $ScriptDir 'validate-manifests.ps1')
        }
        Invoke-Step 'spec-gate-check' {
            & (Join-Path $ScriptDir 'spec-gate-check.ps1') -ChangedFiles $ChangedFiles -BaseRef $BaseRef
        }
        Invoke-Step 'architecture-review-check' {
            & (Join-Path $ScriptDir 'architecture-review-check.ps1') -ChangedFiles $ChangedFiles -BaseRef $BaseRef
        }
    }

    if ($Gate -in 'security', 'all') {
        Invoke-Step 'dep-audit-dotnet' {
            $sln = Get-ChildItem -Path $Root -Filter 'Arah.sln' | Select-Object -First 1
            if ($sln) {
                dotnet list $sln.FullName package --vulnerable --include-transitive 2>&1 | Tee-Object -Variable out
                if ($out -match 'has the following vulnerable') { throw 'Vulnerabilidades encontradas' }
            }
        }
        Invoke-Step 'secrets-scan' {
            $patterns = @('api_key\s*=', 'password\s*=\s*[''"][^''"]+', 'secret\s*=\s*[''"]')
            $hits = @()
            foreach ($p in $patterns) {
                $m = git diff "$BaseRef...HEAD" 2>$null | Select-String -Pattern $p -SimpleMatch:$false
                if ($m) { $hits += $m }
            }
            if ($hits.Count -gt 0) { throw "Possível secret no diff ($($hits.Count) linha(s))" }
        }
    }

    if ($Gate -in 'release', 'all') {
        Invoke-Step 'dotnet-build' {
            $sln = Get-ChildItem -Path $Root -Filter 'Arah.sln' | Select-Object -First 1
            if ($sln) {
                dotnet build $sln.FullName --configuration Release
                if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
            }
        }
    }
} finally {
    Pop-Location
}

if ($failures -gt 0) {
    Write-Error "run-gates: $failures gate(s) falharam"
    exit 1
}
Write-Host "`nrun-gates: todos os gates passaram"
exit 0
