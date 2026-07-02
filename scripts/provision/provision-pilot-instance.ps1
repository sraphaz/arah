#Requires -Version 5.1
<#
.SYNOPSIS
  Sobe compose piloto e executa verificação FASE54 completa.
.EXAMPLE
  ./provision-pilot-instance.ps1 -SkipStripeCheck
.EXAMPLE
  ./provision-pilot-instance.ps1 -WithHttps
#>
param(
    [string]$ComposeFile = 'infrastructure/pilot/docker-compose.pilot.yml',
    [string]$ApiBaseUrl = 'http://localhost:8080',
    [string]$InstanceBaseUrl = 'http://localhost:8080',
    [switch]$SkipStripeCheck,
    [switch]$WithHttps,
    [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '../..')
Push-Location $repoRoot

try {
    $composeArgs = @('-f', $ComposeFile)
    if ($WithHttps) {
        $composeArgs += '--profile', 'https'
        $InstanceBaseUrl = 'https://localhost:8443'
    }

    $upArgs = @('compose') + $composeArgs + @('up', '-d')
    if (-not $NoBuild) {
        $upArgs += '--build'
    }

    Write-Host "FASE54 — docker compose up ..."
    & docker @upArgs

    $verifyArgs = @{
        ApiBaseUrl      = $ApiBaseUrl
        InstanceBaseUrl = $InstanceBaseUrl
    }
    if ($SkipStripeCheck) {
        $verifyArgs.SkipStripeCheck = $true
    }
    if ($WithHttps) {
        $verifyArgs.RequireHttps = $true
        $verifyArgs.HttpsBaseUrl = 'https://localhost:8443'
    }

    & (Join-Path $PSScriptRoot 'verify-pilot-instance.ps1') @verifyArgs
}
finally {
    Pop-Location
}
