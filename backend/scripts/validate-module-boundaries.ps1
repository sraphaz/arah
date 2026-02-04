<#
.SYNOPSIS
    Valida que arquivos na pasta Domain/ de cada módulo não referenciam tipos da pasta Infrastructure/ do mesmo módulo.
.DESCRIPTION
    Regra de convenção: dentro de um módulo (Araponga.Modules.*), a pasta Domain/ não deve referenciar
    a pasta Infrastructure/ do mesmo projeto. Este script falha (exit 1) se encontrar violações.
    Uso em CI: ./scripts/validate-module-boundaries.ps1
.NOTES
    Ver backend/docs/IMPROVEMENTS_AND_KNOWN_ISSUES.md e BACKEND_LAYERS_AND_NAMING.md.
#>

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendRoot = Split-Path -Parent $scriptDir
if (-not (Test-Path (Join-Path $backendRoot "Araponga.Api"))) {
    $backendRoot = $scriptDir
}
$modulesRoot = $backendRoot
$violations = @()

Get-ChildItem -Path $modulesRoot -Directory -Filter "Araponga.Modules.*" | ForEach-Object {
    $modulePath = $_.FullName
    $moduleName = $_.Name
    $domainPath = Join-Path $modulePath "Domain"
    $infraNamespace = "$($moduleName -replace '\.csproj$','').Infrastructure"
    if (-not (Test-Path $domainPath)) { return }
    Get-ChildItem -Path $domainPath -Recurse -Filter "*.cs" | ForEach-Object {
        $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $content) { return }
        if ($content -match "using\s+$([regex]::Escape($infraNamespace))" -or $content -match "using\s+$([regex]::Escape($moduleName))\.Infrastructure") {
            $violations += "$($_.FullName): referencia Infrastructure do mesmo módulo (namespace $infraNamespace)"
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host "Violações da regra 'Domain não referencia Infrastructure do mesmo módulo':" -ForegroundColor Red
    $violations | ForEach-Object { Write-Host "  $_" }
    exit 1
}
Write-Host "OK: Nenhuma violação (Domain não referencia Infrastructure nos módulos)." -ForegroundColor Green
exit 0
