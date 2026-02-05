# ============================================
# Araponga - Stack local integrado (API + BFF)
# ============================================
# Sobe a API (Docker) e o BFF (dotnet run) para você testar e debugar
# o app Flutter apontando para o BFF local, que por sua vez aponta para a API.
#
# Seguro para rerun: pode executar de novo a qualquer momento. Docker compose
# up -d é idempotente (containers já no ar permanecem); falhas exibem mensagem
# clara e código de saída 1.
#
# Uso (sempre com .\ no PowerShell):
#   .\scripts\run-local-stack.ps1           # Na raiz do repo: sobe API + BFF
#   .\run-local-stack.ps1                    # Dentro de scripts\: mesmo efeito
#   .\scripts\run-local-stack.ps1 -Detached  # BFF em background
#
# Depois, em outro terminal:
#   cd frontend\araponga.app
#   .\scripts\run-app-local.ps1
#   # ou: flutter run --dart-define=BFF_BASE_URL=http://localhost:5001
#
# Emulador Android: use BFF_BASE_URL=http://10.0.2.2:5001

param(
    [switch]$Detached,  # Roda BFF em background (Start-Job)
    [switch]$SkipDocker, # Não sobe Docker; assume que API já está em localhost:8080
    [switch]$Help
)

$ErrorActionPreference = "Stop"
$RepoRoot = (Get-Item $PSScriptRoot).Parent.FullName
$BffProject = Join-Path $RepoRoot "backend\Araponga.Api.Bff\Araponga.Api.Bff.csproj"
$ComposeFile = Join-Path $RepoRoot "docker-compose.dev.yml"

function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Ok   { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }
function Write-Err  { param([string]$Message) Write-Host $Message -ForegroundColor Red }

# Retorna $true se a porta está em uso (alguém escutando). Em falha do cmdlet, retorna $false (não bloqueia).
function Test-PortInUse {
    param([int]$Port)
    try {
        $conn = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction Stop
        return ($null -ne $conn -and @($conn).Count -gt 0)
    } catch {
        return $false
    }
}

# Verifica se o daemon Docker está acessível (Docker Desktop rodando).
function Test-DockerDaemon {
    $null = docker info 2>&1
    return ($LASTEXITCODE -eq 0)
}

function Show-StackSummary {
    param([bool]$DockerOk, [string]$ComposeFile, [bool]$ApiHealthy = $true)
    Write-Host ""
    Write-Host "================================================================================" -ForegroundColor DarkGray
    if ($DockerOk) {
        if ($ApiHealthy) {
            Write-Ok "  STACK LOCAL: subiu com sucesso"
        } else {
            Write-Warn "  STACK LOCAL: Docker no ar, mas a API não respondeu em /health"
            Write-Host "  Se o container da API estiver 'Restarting', veja os logs: docker compose -f docker-compose.dev.yml logs api" -ForegroundColor Yellow
        }
        Write-Host ""
        Write-Info "  O que está no ar (Docker):"
        & docker compose -f $ComposeFile ps 2>$null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  (execute 'docker compose -f docker-compose.dev.yml ps' na raiz para listar)" -ForegroundColor DarkGray
        }
        Write-Host ""
        Write-Info "  Como acessar:"
        if (-not $ApiHealthy) {
            Write-Host "    API:      http://localhost:8080          (pode estar em falha - confira os logs acima)" -ForegroundColor Yellow
        } else {
            Write-Host "    API:      http://localhost:8080          (Swagger: http://localhost:8080/swagger)" -ForegroundColor White
        }
        Write-Host "    BFF:      http://localhost:5001          (sobe neste terminal; use no app Flutter)" -ForegroundColor White
        Write-Host "    Postgres: localhost:5432 (user araponga)" -ForegroundColor DarkGray
        Write-Host "    Redis:    localhost:6379" -ForegroundColor DarkGray
        Write-Host "    MinIO:    localhost:9000 (console: 9001)" -ForegroundColor DarkGray
    } else {
        Write-Err "  DOCKER: falhou ao subir os containers."
        Write-Host "  Verifique o Docker Desktop e os erros acima. Para ver logs: docker compose -f docker-compose.dev.yml logs" -ForegroundColor Yellow
    }
    Write-Host "================================================================================" -ForegroundColor DarkGray
    Write-Host ""
}

function Show-Help {
    Write-Host ""
    Write-Info "=== Araponga - Stack local (App -> BFF -> API) ==="
    Write-Host ""
    Write-Host "  Sobe a API em Docker e o BFF com dotnet run para debug integrado."
    Write-Host "  No PowerShell use sempre .\ antes do nome do script."
    Write-Host ""
    Write-Host "Uso (na raiz do repo ou em scripts\):"
    Write-Host "  .\scripts\run-local-stack.ps1              Sobe API (Docker) e BFF neste terminal"
    Write-Host "  .\run-local-stack.ps1                       (se já estiver em scripts\)"
    Write-Host "  .\scripts\run-local-stack.ps1 -SkipDocker    Só sobe BFF (API já em :8080)"
    Write-Host "  .\scripts\run-local-stack.ps1 -Detached     BFF em background"
    Write-Host "  .\scripts\run-local-stack.ps1 -Help         Esta ajuda"
    Write-Host ""
    Write-Host "Depois, em outro terminal, rode o app:"
    Write-Host "  cd frontend\araponga.app"
    Write-Host "  .\scripts\run-app-local.ps1"
    Write-Host "  (ou: flutter run --dart-define=BFF_BASE_URL=http://localhost:5001)"
    Write-Host ""
    Write-Host "URLs locais:"
    Write-Host "  API:  http://localhost:8080   (Swagger: /swagger)"
    Write-Host "  BFF:  http://localhost:5001  (app usa esta)"
    Write-Host ""
}

if ($Help) { Show-Help; exit 0 }

Push-Location $RepoRoot | Out-Null
try {
    # 0) Garantir .env (docker-compose.dev.yml usa variáveis do .env)
    if (-not (Test-Path (Join-Path $RepoRoot ".env"))) {
        Write-Info "Arquivo .env não encontrado. Configurando ambiente..."
        & (Join-Path $RepoRoot "scripts\setup-env.ps1") -CalledByStack
        if ($LASTEXITCODE -ne 0) { exit 1 }
    }

    # 1) Docker: subir API + dependências (postgres, redis, minio)
    $DockerOk = $true
    if (-not $SkipDocker) {
        Write-Info "Verificando Docker e subindo API (postgres, redis, minio, api)..."
        if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
            Write-Err "Docker não encontrado. Use -SkipDocker se a API já estiver rodando em localhost:8080"
            exit 1
        }
        if (-not (Test-DockerDaemon)) {
            Write-Err "Docker está instalado mas o daemon não está rodando. Abra o Docker Desktop e execute o script novamente."
            exit 1
        }
        $env:COMPOSE_PROJECT_NAME = "araponga"
        & docker compose -f $ComposeFile up -d
        if ($LASTEXITCODE -ne 0) {
            $DockerOk = $false
            Show-StackSummary -DockerOk $false -ComposeFile $ComposeFile
            exit 1
        }
        Write-Ok "Docker: containers subiram."
        Write-Info "Aguardando API ficar pronta (até 30s)..."
        $ApiHealthy = $false
        $attempts = 0
        while ($attempts -lt 30) {
            try {
                $r = Invoke-WebRequest -Uri "http://localhost:8080/health" -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
                if ($r.StatusCode -eq 200) { $ApiHealthy = $true; break }
            } catch { }
            $attempts++
            Start-Sleep -Seconds 1
        }
        if (-not $ApiHealthy) {
            Write-Warn "API não respondeu em /health em 30s (container pode estar reiniciando). Subindo BFF mesmo assim..."
        } else {
            Write-Ok "API respondendo em http://localhost:8080/health"
        }
        Show-StackSummary -DockerOk $true -ComposeFile $ComposeFile -ApiHealthy $ApiHealthy
    } else {
        Write-Info "SkipDocker: assumindo API em http://localhost:8080"
        Show-StackSummary -DockerOk $true -ComposeFile $ComposeFile
    }

    # 2) BFF: apontar para API local e escutar em 5001 (padrão do app)
    $env:Bff__ApiBaseUrl = "http://localhost:8080"
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:ASPNETCORE_URLS = "http://localhost:5001"

    if (-not (Test-Path $BffProject)) {
        Write-Err "BFF não encontrado: $BffProject"
        exit 1
    }

    if (Test-PortInUse -Port 5001) {
        Write-Err "Porta 5001 já está em uso. Pare o BFF anterior (ou o processo que a usa) e execute o script novamente."
        Write-Host "  Dica: se o BFF está em outro terminal, use só aquele. Para ver o que usa a porta: Get-NetTCPConnection -LocalPort 5001" -ForegroundColor DarkGray
        exit 1
    }

    if ($Detached) {
        Write-Info "Iniciando BFF em background (job)..."
        $job = Start-Job -ScriptBlock {
            param($proj, $urls, $apiUrl)
            Set-Location (Split-Path $proj -Parent)
            $env:ASPNETCORE_URLS = $urls
            $env:Bff__ApiBaseUrl = $apiUrl
            & dotnet run --project $proj --no-build 2>&1
        } -ArgumentList $BffProject, "http://localhost:5001", "http://localhost:8080"
        Write-Ok "BFF em background (job id $($job.Id)). Para ver logs: Receive-Job -Id $($job.Id) -Keep"
        Write-Host ""
        Write-Info "Para rodar o app: cd frontend\araponga.app; .\scripts\run-app-local.ps1"
        Write-Host "  API: http://localhost:8080/swagger  |  BFF: http://localhost:5001" -ForegroundColor DarkGray
        Write-Host ""
    } else {
        Write-Info "Iniciando BFF em http://localhost:5001 (ApiBaseUrl=http://localhost:8080)"
        Write-Host "  O BFF estará disponível quando aparecer 'Application started' abaixo." -ForegroundColor Cyan
        Write-Host "  Deixe este terminal aberto. Em outro terminal:" -ForegroundColor Yellow
        Write-Host "  cd frontend\araponga.app; .\scripts\run-app-local.ps1" -ForegroundColor White
        Write-Host "  API: http://localhost:8080/swagger  |  BFF: http://localhost:5001" -ForegroundColor DarkGray
        Write-Host ""
        & dotnet run --project $BffProject --urls "http://localhost:5001"
        if ($LASTEXITCODE -ne 0) {
            Write-Err "BFF encerrou com erro (código $LASTEXITCODE). Corrija e execute o script novamente."
            exit $LASTEXITCODE
        }
    }
} catch {
    Write-Err "Erro inesperado: $_"
    Write-Host "  Use .\scripts\run-local-stack.ps1 -Help para opções." -ForegroundColor DarkGray
    exit 1
} finally {
    Pop-Location | Out-Null
}
