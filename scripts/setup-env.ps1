# ============================================
# Araponga - Configuração do ambiente local
# ============================================
# Garante que .env existe (copia de .env.example), gera JWT para dev se necessário.
# Execute na raiz do repo: .\scripts\setup-env.ps1

param(
    [switch]$Force,  # Recria .env a partir do .env.example (sobrescreve)
    [switch]$CalledByStack,  # Chamado por run-local-stack.ps1: mensagem curta, não pede para rodar o stack de novo
    [switch]$Help
)

$ErrorActionPreference = "Stop"
$RepoRoot = (Get-Item $PSScriptRoot).Parent.FullName
$EnvExample = Join-Path $RepoRoot ".env.example"
$EnvFile = Join-Path $RepoRoot ".env"

function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Ok   { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }

if ($Help) {
    Write-Host ""
    Write-Info "=== Araponga - Setup do ambiente ==="
    Write-Host ""
    Write-Host "  Garante .env na raiz (cópia de .env.example)."
    Write-Host "  Se JWT_SIGNINGKEY estiver com valor padrão, gera um para desenvolvimento."
    Write-Host ""
    Write-Host "Uso: .\scripts\setup-env.ps1  [-Force]  [-CalledByStack]  [-Help]"
    Write-Host "  -Force          recria .env a partir de .env.example"
    Write-Host "  -CalledByStack   uso interno: chamado por run-local-stack.ps1 (mensagem curta)"
    Write-Host ""
    exit 0
}

Push-Location $RepoRoot | Out-Null
try {
    if (-not (Test-Path $EnvExample)) {
        Write-Warn ".env.example não encontrado em $RepoRoot"
        exit 1
    }

    $needCreate = -not (Test-Path $EnvFile) -or $Force
    if ($needCreate) {
        Copy-Item $EnvExample $EnvFile -Force
        Write-Ok ".env criado/atualizado a partir de .env.example"
    } else {
        Write-Info ".env já existe (use -Force para recriar)"
    }

    # Ler .env e, se JWT_SIGNINGKEY for o placeholder, gerar um para dev
    $lines = Get-Content $EnvFile
    $jwtPlaceholder = 'JWT_SIGNINGKEY=CHANGE_THIS_IN_PRODUCTION'
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^JWT_SIGNINGKEY=CHANGE_THIS_IN_PRODUCTION') {
            $bytes = New-Object byte[] 32
            [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
            $key = [Convert]::ToBase64String($bytes)
            $lines[$i] = "JWT_SIGNINGKEY=$key"
            $lines | Set-Content $EnvFile
            Write-Ok "JWT_SIGNINGKEY definido para desenvolvimento (32 bytes em Base64)"
            break
        }
    }

    Write-Host ""
    if ($CalledByStack) {
        Write-Ok "Ambiente configurado. Iniciando Docker e BFF em seguida..."
    } else {
        Write-Info "Próximos passos:"
        Write-Host "  - Revisar .env na raiz (opcional: editar JWT, Redis, MinIO, etc.)."
        Write-Host "  - Subir o stack: .\scripts\run-local-stack.ps1"
    }
    Write-Host ""
} finally {
    Pop-Location | Out-Null
}
