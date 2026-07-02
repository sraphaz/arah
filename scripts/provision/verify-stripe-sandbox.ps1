#Requires -Version 5.1
<#
.SYNOPSIS
  Verifica conexão com Stripe sandbox (balance API) quando STRIPE__SECRETKEY está definido.
#>
param(
    [string]$SecretKey = $env:STRIPE__SECRETKEY
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($SecretKey)) {
    if ($env:CI -eq 'true') {
        Write-Warning 'STRIPE__SECRETKEY not set — skipping PSP sandbox check (configure secret in staging for full AC).'
        return
    }
    throw 'STRIPE__SECRETKEY is required for PSP sandbox verification.'
}

if (-not $SecretKey.StartsWith('sk_test_')) {
    Write-Warning 'Stripe key is not sk_test_* — ensure sandbox credentials in pilot/staging.'
}

$headers = @{
    Authorization = "Bearer $SecretKey"
    Accept        = 'application/json'
}

Invoke-RestMethod -Method Get -Uri 'https://api.stripe.com/v1/balance' -Headers $headers | Out-Null
Write-Host 'Stripe sandbox connection OK (balance API).'
