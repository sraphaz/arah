#Requires -Version 5.1
<#
.SYNOPSIS
  Obtém JWT de SystemAdmin piloto via social login (FASE54).
#>
param(
    [string]$ApiBaseUrl = 'http://localhost:8080',
    [string]$AuthProvider = 'google',
    [string]$ExternalId = 'admin-external',
    [int]$MaxAttempts = 30,
    [int]$DelaySeconds = 2
)

$ErrorActionPreference = 'Stop'
$base = $ApiBaseUrl.TrimEnd('/')
$loginUri = "$base/api/v1/auth/social"
$body = @{
    authProvider = $AuthProvider
    externalId   = $ExternalId
    displayName  = 'Pilot System Admin'
    cpf          = '000.000.000-00'
    email        = 'pilot-admin@arah.local'
} | ConvertTo-Json -Compress

for ($attempt = 1; $attempt -le $MaxAttempts; $attempt++) {
    try {
        $response = Invoke-RestMethod -Method Post -Uri $loginUri -Body $body -ContentType 'application/json'
        if ($response.token) {
            return $response.token
        }
    }
    catch {
        if ($attempt -eq $MaxAttempts) {
            throw
        }
        Start-Sleep -Seconds $DelaySeconds
    }
}

throw "Unable to obtain admin token from $loginUri after $MaxAttempts attempts."
