# ============================================
# Instalar Flutter no Windows (clone estável + PATH)
# ============================================
# Requer: Git instalado. Adiciona Flutter ao PATH do usuário.
# Uso: .\scripts\install-flutter.ps1   ou   .\scripts\install-flutter.ps1 -InstallPath "C:\src\flutter"

param(
    [string]$InstallPath = "$env:LOCALAPPDATA\flutter",
    [switch]$Help
)

$ErrorActionPreference = "Stop"

function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Ok   { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }

if ($Help) {
    Write-Host ""
    Write-Info "Uso: .\scripts\install-flutter.ps1 [-InstallPath pasta]"
    Write-Host "  Instala Flutter (branch stable) via Git e adiciona ao PATH do usuário."
    Write-Host "  Padrão: $env:LOCALAPPDATA\flutter"
    Write-Host ""
    exit 0
}

# Git obrigatório
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Warn "Git não encontrado. Instale o Git for Windows e tente novamente."
    Write-Host "  https://git-scm.com/download/win"
    exit 1
}

$flutterBin = Join-Path $InstallPath "bin"
$currentPath = [Environment]::GetEnvironmentVariable("Path", "User")
if (-not $currentPath) { $currentPath = "" }

if (Test-Path (Join-Path $InstallPath "bin\flutter.bat")) {
    Write-Ok "Flutter já existe em: $InstallPath"
    if ($currentPath -notlike "*$flutterBin*") {
        Write-Info "Adicionando ao PATH do usuário..."
        [Environment]::SetEnvironmentVariable("Path", "$currentPath;$flutterBin", "User")
        $env:Path = "$env:Path;$flutterBin"
        Write-Ok "PATH atualizado. Abra um novo terminal e rode: flutter doctor"
    } else {
        Write-Ok "Flutter já está no PATH."
    }
    Write-Host ""
    & (Join-Path $InstallPath "bin\flutter.bat") doctor -v
    exit 0
}

Write-Info "Instalando Flutter em: $InstallPath"
if (Test-Path $InstallPath) {
    Write-Warn "A pasta já existe. Tentando atualizar (git pull)..."
    Push-Location $InstallPath
    try {
        git fetch origin
        git checkout stable
        git pull
    } finally {
        Pop-Location
    }
} else {
    $parent = Split-Path $InstallPath -Parent
    if (-not (Test-Path $parent)) { New-Item -ItemType Directory -Path $parent -Force | Out-Null }
    Write-Info "Clonando repositório Flutter (branch stable)..."
    git clone --depth 1 --branch stable https://github.com/flutter/flutter.git $InstallPath
    if ($LASTEXITCODE -ne 0) {
        Write-Warn "Clone falhou. Tente sem --depth 1: git clone -b stable https://github.com/flutter/flutter.git $InstallPath"
        exit 1
    }
}

if ($currentPath -notlike "*$flutterBin*") {
    Write-Info "Adicionando ao PATH do usuário..."
    [Environment]::SetEnvironmentVariable("Path", "$currentPath;$flutterBin", "User")
    $env:Path = "$env:Path;$flutterBin"
    Write-Ok "PATH atualizado."
}

Write-Ok "Flutter instalado em $InstallPath"
Write-Info "Rodando flutter doctor (pode baixar Dart SDK na primeira vez)..."
& (Join-Path $InstallPath "bin\flutter.bat") doctor -v
Write-Host ""
Write-Ok "Pronto. Abra um NOVO terminal para usar o comando 'flutter'."
Write-Host "  Depois: cd frontend\araponga.app && flutter pub get && .\scripts\run-app-local.ps1"
Write-Host ""
