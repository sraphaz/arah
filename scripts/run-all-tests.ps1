#!/usr/bin/env pwsh
# Roda build e todos os testes antes do commit.
# Inclui: backend (.NET), wiki (Jest), devportal (Jest), app Flutter.
# Uso: ./scripts/run-all-tests.ps1
# Testes de stress .NET são pulados por padrão; use $env:RUN_STRESS_TESTS="true" para incluí-los.

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $root

# --- Backend .NET ---
Write-Host "Backend: Restore..." -ForegroundColor Cyan
dotnet restore Arah.sln
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Backend: Build Release..." -ForegroundColor Cyan
dotnet build Arah.sln --no-restore --configuration Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Backend: Testes da solution (~2.400 testes)..." -ForegroundColor Cyan
dotnet test Arah.sln --no-build --configuration Release --verbosity minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# --- Wiki (Jest) ---
Write-Host "Wiki: npm install + test..." -ForegroundColor Cyan
Push-Location "$root/frontend/wiki"
try {
  npm install
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
  npm test -- --passWithNoTests
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }

# --- DevPortal (Jest) ---
Write-Host "DevPortal: npm install + test..." -ForegroundColor Cyan
Push-Location "$root/frontend/devportal"
try {
  npm install
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
  npm test -- --passWithNoTests
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }

# --- App Flutter ---
Write-Host "App Flutter: pub get + test..." -ForegroundColor Cyan
Push-Location "$root/frontend/arah.app"
try {
  flutter pub get
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
  flutter test
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }

Write-Host "`nTodos os testes passaram (backend, wiki, devportal, app)." -ForegroundColor Green
