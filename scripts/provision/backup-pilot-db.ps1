#Requires -Version 5.1
<#
.SYNOPSIS
  Backup inicial do Postgres piloto (FASE54 — RPO ≤ 15 min documentado).
.EXAMPLE
  ./backup-pilot-db.ps1 -ContainerName arah-pilot-postgres-1
#>
param(
    [string]$ContainerName = 'pilot-postgres-1',
    [string]$OutputDir = 'infrastructure/pilot/backups',
    [string]$DbName = 'arah_pilot',
    [string]$DbUser = 'arah'
)

$ErrorActionPreference = 'Stop'
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$resolvedDir = Join-Path (Get-Location) $OutputDir
New-Item -ItemType Directory -Force -Path $resolvedDir | Out-Null
$outputFile = Join-Path $resolvedDir "pilot-$timestamp.sql"

docker exec $ContainerName pg_dump -U $DbUser -d $DbName --no-owner --no-acl | Set-Content -Path $outputFile -Encoding utf8

$sizeBytes = (Get-Item $outputFile).Length
[ordered]@{
    file      = $outputFile
    sizeBytes = $sizeBytes
    rpoNote   = 'Schedule pg_dump every 15 min in production (cron/K8s CronJob).'
} | ConvertTo-Json

Write-Host "Backup saved: $outputFile ($sizeBytes bytes)"
