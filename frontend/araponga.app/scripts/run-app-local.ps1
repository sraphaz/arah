# ============================================
# Ará (Araponga App) - Rodar apontando para BFF local
# ============================================
# Use depois de subir o stack com: ..\..\scripts\run-local-stack.ps1
#
# Por padrão roda no Chrome (web). Sem dispositivo/emulador conectado, o app abre no navegador.
# Para emulador Android: -Android (usa 10.0.2.2:5001) ou -Device android

param(
    [string]$BffUrl = "http://localhost:5001",
    [switch]$Android,   # Usa 10.0.2.2:5001 para emulador Android
    [string]$Device,   # Dispositivo: chrome (padrão), edge, android, ios, etc. Use "flutter devices" para listar.
    [switch]$Help
)

$ErrorActionPreference = "Stop"
$AppRoot = (Get-Item $PSScriptRoot).Parent.FullName

function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Ok   { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }

function Get-FlutterPath {
    $f = Get-Command flutter -ErrorAction SilentlyContinue
    if ($f) { return "flutter" }
    $candidates = @(
        "$env:LOCALAPPDATA\flutter\bin\flutter.bat",
        "$env:USERPROFILE\flutter\bin\flutter.bat",
        "$env:USERPROFILE\development\flutter\bin\flutter.bat",
        "$env:ProgramFiles\flutter\bin\flutter.bat"
    )
    foreach ($path in $candidates) {
        if ($path -and (Test-Path $path)) { return $path }
    }
    return $null
}

function Show-Help {
    Write-Host ""
    Write-Info "=== Ará - Rodar app contra BFF local ==="
    Write-Host ""
    Write-Host "  Por padrão roda no Chrome (web). Use -Device para outro alvo."
    Write-Host ""
    Write-Host "Uso:"
    Write-Host "  .\scripts\run-app-local.ps1             BFF localhost:5001, app no Chrome"
    Write-Host "  .\scripts\run-app-local.ps1 -Android     BFF 10.0.2.2:5001 (emulador Android)"
    Write-Host "  .\scripts\run-app-local.ps1 -Device edge App no Edge (web)"
    Write-Host "  .\scripts\run-app-local.ps1 -Device android  Emulador/device Android"
    Write-Host "  .\scripts\run-app-local.ps1 -BffUrl http://192.168.1.10:5001"
    Write-Host "  .\scripts\run-app-local.ps1 -Help"
    Write-Host "  flutter devices   lista dispositivos disponíveis"
    Write-Host ""
}

if ($Help) { Show-Help; exit 0 }

if ($Android) { $BffUrl = "http://10.0.2.2:5001" }

# Dispositivo padrão: Chrome (web), para rodar sem phone/emulador
if (-not $Device) { $Device = "chrome" }

$flutterCmd = Get-FlutterPath
if (-not $flutterCmd) {
    Write-Warn "Flutter não encontrado no PATH nem em pastas comuns."
    Write-Host ""
    Write-Host "  Opções:"
    Write-Host "  1. Adicione o Flutter ao PATH e abra um novo terminal."
    Write-Host "  2. Rode manualmente (em um terminal onde 'flutter' funciona):"
    Write-Host ""
    Write-Host "     cd $AppRoot" -ForegroundColor Gray
    Write-Host "     flutter run -d chrome --dart-define=BFF_BASE_URL=$BffUrl" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  3. No Cursor/VS Code, use o terminal integrado onde o Flutter Extension pode ter configurado o PATH."
    exit 1
}

Write-Info "BFF_BASE_URL=$BffUrl"
Write-Info "Dispositivo: $Device"
Write-Info "Iniciando Flutter..."
Push-Location $AppRoot | Out-Null
try {
    & $flutterCmd run -d $Device --dart-define=BFF_BASE_URL=$BffUrl
} finally {
    Pop-Location | Out-Null
}
