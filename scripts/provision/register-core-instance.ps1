#Requires -Version 5.1
<#
.SYNOPSIS
  Registra instância piloto no Arah Core (FASE54 — passo 3 do pipeline).
.EXAMPLE
  ./register-core-instance.ps1 -ApiBaseUrl http://localhost:5178 -InstanceBaseUrl https://piloto.example -AdminToken $token
#>
param(
    [string]$ApiBaseUrl = 'http://localhost:5178',
    [Parameter(Mandatory)]
    [string]$InstanceBaseUrl,
    [string]$Mode = 'managed',
    [string]$Version = '0.1.0',
    [Parameter(Mandatory)]
    [string]$AdminToken
)

$ErrorActionPreference = 'Stop'
$uri = "$($ApiBaseUrl.TrimEnd('/'))/api/v1/core/instances"
$body = @{
    mode    = $Mode
    baseUrl = $InstanceBaseUrl
    version = $Version
} | ConvertTo-Json -Compress

$headers = @{
    Authorization = "Bearer $AdminToken"
    Accept        = 'application/json'
}

$response = Invoke-RestMethod -Method Post -Uri $uri -Headers $headers -Body $body -ContentType 'application/json'
[ordered]@{
    instanceId        = $response.instance.id
    instanceAuthToken = $response.instanceAuthToken
    publicKeyPem      = $response.instance.publicKeyPem
    status            = $response.instance.status
} | ConvertTo-Json -Depth 4

Write-Host "Guarde instanceAuthToken em secret — não será exibido novamente."
