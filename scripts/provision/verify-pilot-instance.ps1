#Requires -Version 5.1
<#
.SYNOPSIS
  Valida piloto FASE54: health, JWT, registro Core, heartbeat e status Online.
.EXAMPLE
  ./verify-pilot-instance.ps1 -ApiBaseUrl http://localhost:8080 -InstanceBaseUrl http://localhost:8080
#>
param(
    [string]$ApiBaseUrl = 'http://localhost:8080',
    [string]$InstanceBaseUrl = 'http://localhost:8080',
    [string]$Version = '0.1.0',
    [string]$Mode = 'managed',
    [switch]$SkipStripeCheck,
    [switch]$RequireHttps,
    [string]$HttpsBaseUrl = 'https://localhost:8443'
)

$ErrorActionPreference = 'Stop'
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$base = $ApiBaseUrl.TrimEnd('/')

function Wait-HealthOk {
    param([string]$Url, [int]$MaxAttempts = 60, [int]$DelaySeconds = 2)
    for ($i = 1; $i -le $MaxAttempts; $i++) {
        try {
            $health = Invoke-RestMethod -Method Get -Uri "$Url/health" -TimeoutSec 5
            if ($health.status -eq 'Healthy' -or $health.status -eq 'healthy') {
                return
            }
        }
        catch {
            if ($i -eq $MaxAttempts) { throw "Health check failed at $Url/health — $($_.Exception.Message)" }
        }
        Start-Sleep -Seconds $DelaySeconds
    }
}

Write-Host "FASE54 — waiting for /health at $base ..."
Wait-HealthOk -Url $base

if ($RequireHttps) {
    $https = $HttpsBaseUrl.TrimEnd('/')
    Write-Host "FASE54 — verifying HTTPS at $https ..."
    if (-not (Get-Command curl -ErrorAction SilentlyContinue)) {
        throw 'curl is required for HTTPS verification (-RequireHttps).'
    }
    curl -skf "$https/health" | Out-Null
}

Write-Host 'FASE54 — obtaining SystemAdmin JWT ...'
$adminToken = & (Join-Path $scriptRoot 'get-pilot-admin-token.ps1') -ApiBaseUrl $base

Write-Host 'FASE54 — verifying JWT on protected Core endpoint ...'
$coreHeaders = @{ Authorization = "Bearer $adminToken"; Accept = 'application/json' }
$releases = Invoke-RestMethod -Method Get -Uri "$base/api/v1/core/releases" -Headers $coreHeaders
if ($null -eq $releases) {
    throw 'JWT verification failed — /core/releases returned empty.'
}

Write-Host 'FASE54 — registering instance in Core ...'
$registrationJson = & (Join-Path $scriptRoot 'register-core-instance.ps1') `
    -ApiBaseUrl $base `
    -InstanceBaseUrl $InstanceBaseUrl `
    -AdminToken $adminToken `
    -Mode $Mode `
    -Version $Version
$registration = $registrationJson | ConvertFrom-Json

Write-Host 'FASE54 — sending heartbeat ...'
$heartbeatUri = "$base/api/v1/core/instances/$($registration.instanceId)/heartbeat"
$heartbeatBody = @{
    uptimeSeconds = 120
    services      = @{ api = 'healthy' }
} | ConvertTo-Json -Compress
$heartbeatHeaders = @{
    'X-Arah-Instance-Token' = $registration.instanceAuthToken
    Accept                  = 'application/json'
}
Invoke-RestMethod -Method Post -Uri $heartbeatUri -Headers $heartbeatHeaders -Body $heartbeatBody -ContentType 'application/json' | Out-Null

Write-Host 'FASE54 — confirming instance Online ...'
$instance = Invoke-RestMethod -Method Get -Uri "$base/api/v1/core/instances/$($registration.instanceId)" -Headers $coreHeaders
if ($instance.status -ne 'Online') {
    throw "Expected instance status Online, got $($instance.status)."
}

if (-not $SkipStripeCheck) {
    & (Join-Path $scriptRoot 'verify-stripe-sandbox.ps1')
}

[ordered]@{
    health       = 'ok'
    jwt          = 'ok'
    instanceId   = $registration.instanceId
    status       = $instance.status
    baseUrl      = $InstanceBaseUrl
} | ConvertTo-Json -Depth 3

Write-Host 'FASE54 — pilot instance verification complete.'
