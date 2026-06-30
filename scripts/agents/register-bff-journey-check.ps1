#Requires -Version 5.1
<#
.SYNOPSIS
  Valida que alterações BFF incluem BffJourneyRegistry e testes (skill register-bff-journey).
#>
param(
    [string[]]$ChangedFiles = @(),
    [string]$BaseRef = 'origin/main'
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$registry = 'backend/Arah.Api.Bff/Journeys/BffJourneyRegistry.cs'
$testGlob = 'backend/Tests/Arah.Tests/Bff/'

Push-Location $Root
try {
    if ($ChangedFiles.Count -eq 0) {
        git fetch origin main --depth=1 2>$null
        $ChangedFiles = @(git diff --name-only "$BaseRef...HEAD" 2>$null | Where-Object { $_ })
    }
} finally { Pop-Location }

$norm = $ChangedFiles | ForEach-Object { $_.Replace('\', '/') }
$bffTouched = $norm | Where-Object { $_ -match '^backend/Arah\.Api\.Bff/' -and $_ -ne $registry }
if (-not $bffTouched) {
    Write-Host 'register-bff-journey-check: nenhuma alteração BFF além do registry — OK'
    exit 0
}

$errors = @()
if ($norm -notcontains $registry) {
    $errors += "Alteração em Arah.Api.Bff sem atualizar $registry"
}
$hasBffTest = $norm | Where-Object { $_ -like "$testGlob*" }
if (-not $hasBffTest) {
    $errors += "Alteração BFF sem testes em $testGlob"
}

foreach ($e in $errors) { Write-Error $e }
Write-Host 'register-bff-journey-check: OK'
exit 0
