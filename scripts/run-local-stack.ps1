# ============================================
# Araponga - Stack local integrado (API + BFF)
# ============================================
# Sobe a API (Docker) e o BFF (dotnet run) para você testar e debugar
# o app Flutter apontando para o BFF local, que por sua vez aponta para a API.
#
# - Na primeira subida, a API aplica migrações no Postgres (pode levar ~30s).
# - Em seguida é executado o seed Camburi (ingestão de dados) via scripts/seed.
#
# Seguro para rerun: pode executar de novo a qualquer momento. Docker compose
# up -d é idempotente (containers já no ar permanecem); falhas exibem mensagem
# clara e código de saída 1.
#
# Uso (sempre com .\ no PowerShell):
#   .\scripts\run-local-stack.ps1           # Sobe API + BFF, abre API/BFF no navegador e inicia o app
#   .\scripts\run-local-stack.ps1 -Detached # BFF em background + abre navegador e app
#   .\scripts\run-local-stack.ps1 -NoStartApp # Não abre navegador nem inicia o app (só sobe BFF)
#   .\scripts\run-local-stack.ps1 -SkipDocker # Só BFF (API já em :8080)
#   .\scripts\run-local-stack.ps1 -ResetDatabase # Recria o banco (apaga volume Postgres) e sobe tudo; aplica a migração única do zero
#
# Depois, em outro terminal:
#   cd frontend\araponga.app
#   .\scripts\run-app-local.ps1
#   # ou: flutter run --dart-define=BFF_BASE_URL=http://localhost:5001
#
# Emulador Android: use BFF_BASE_URL=http://10.0.2.2:5001
#
# Se a API ficar em Restarting: docker compose -f docker-compose.dev.yml logs api
# Após mudanças no backend: docker compose -f docker-compose.dev.yml build api

param(
    [switch]$Detached,       # Roda BFF em background (Start-Job)
    [switch]$SkipDocker,     # Não sobe Docker; assume que API já está em localhost:8080
    [switch]$NoStartApp,     # Não abre navegador nem inicia o app Flutter (só sobe BFF)
    [switch]$ResetDatabase,  # Recria o banco do zero: para containers, remove volume Postgres e sobe de novo (aplica migração única)
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

# Encerra processos que estão escutando na porta indicada (para liberar e reiniciar o BFF).
function Stop-ProcessOnPort {
    param([int]$Port)
    try {
        $conns = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
        if (-not $conns) { return }
        $pids = @($conns | ForEach-Object { $_.OwningProcess } | Sort-Object -Unique)
        foreach ($pid in $pids) {
            $proc = Get-Process -Id $pid -ErrorAction SilentlyContinue
            if ($proc) {
                Write-Warn "Encerrando processo na porta $Port (PID $pid): $($proc.ProcessName)"
                Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
                Start-Sleep -Seconds 1
            }
        }
    } catch {
        Write-Warn "Não foi possível encerrar processo na porta $Port : $_"
    }
}

# Verifica se o daemon Docker está acessível (Docker Desktop rodando).
function Test-DockerDaemon {
    $null = docker info 2>&1
    return ($LASTEXITCODE -eq 0)
}

# Verifica se já existe aba/janela do Chrome com a URL (heurística por título contendo a porta).
function Test-ChromeHasUrl {
    param([string]$PortRef)   # ex: "8080" ou "5001"
    try {
        $chrome = Get-Process -Name chrome -ErrorAction SilentlyContinue
        if (-not $chrome) { return $false }
        $titles = ($chrome | Where-Object { $_.MainWindowTitle } | ForEach-Object { $_.MainWindowTitle }) -join " "
        return ($titles -and $titles -match [regex]::Escape($PortRef))
    } catch {
        return $false
    }
}

# Abre a home da API e o BFF no navegador só se ainda não existir aba com essa URL (evita abrir novas janelas).
function Open-StackInBrowser {
    param([string]$RepoRoot, [switch]$StartApp)
    $apiUrl = "http://localhost:8080"
    $bffUrl = "http://localhost:5001"
    try {
        $openApi = $false
        $openBff = $false
        if (-not (Test-ChromeHasUrl -PortRef "8080")) {
            Start-Process $apiUrl
            $openApi = $true
            Start-Sleep -Milliseconds 300
        }
        if (-not (Test-ChromeHasUrl -PortRef "5001")) {
            Start-Process $bffUrl
            $openBff = $true
        }
        if ($openApi -or $openBff) {
            Write-Ok "Navegador: $(if ($openApi) { 'API (home) ' })$(if ($openBff) { 'BFF ' })aberto(s)."
        } else {
            Write-Ok "Navegador: API e BFF já estavam abertos (nenhuma nova aba)."
        }
    } catch {
        Write-Warn "Não foi possível abrir o navegador: $_"
    }
    if ($StartApp) {
        $appDir = Join-Path $RepoRoot "frontend\araponga.app"
        $appScript = Join-Path $appDir "scripts\run-app-local.ps1"
        if ((Test-Path -LiteralPath $appScript)) {
            try {
                Start-Process powershell -ArgumentList "-NoExit -File `"$appScript`"" -WorkingDirectory $appDir
                Write-Ok "App Flutter iniciado em nova janela."
            } catch {
                Write-Warn "Não foi possível iniciar o app em nova janela. Rode manualmente: cd frontend\araponga.app; .\scripts\run-app-local.ps1"
            }
        }
    }
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
            Write-Host "    API:      http://localhost:8080          (home; Swagger: /swagger)" -ForegroundColor White
        }
        Write-Host "    BFF:      http://localhost:5001          (sobe neste terminal; use no app Flutter)" -ForegroundColor White
        Write-Host "    Postgres: localhost:5432 (user araponga)  |  Seed: scripts\seed\run-seed-camburi.ps1" -ForegroundColor DarkGray
        Write-Host "    Redis:    localhost:6379  |  MinIO: localhost:9000 (console: 9001)" -ForegroundColor DarkGray
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
    Write-Host "  Sobe a API em Docker (postgres, redis, minio, api) e o BFF com dotnet run."
    Write-Host "  Na primeira subida a API aplica migrações no Postgres; em seguida roda o seed Camburi (scripts/seed)."
    Write-Host "  No PowerShell use sempre .\ antes do nome do script."
    Write-Host ""
    Write-Host "Uso (na raiz do repo ou em scripts\):"
    Write-Host "  .\scripts\run-local-stack.ps1              Sobe API + BFF, abre API/BFF no navegador e inicia o app"
    Write-Host "  .\scripts\run-local-stack.ps1 -Detached     BFF em background + abre navegador e app"
    Write-Host "  .\scripts\run-local-stack.ps1 -NoStartApp   Só sobe BFF (não abre navegador nem app)"
    Write-Host "  .\scripts\run-local-stack.ps1 -SkipDocker   Só sobe BFF (API já em :8080)"
    Write-Host "  .\scripts\run-local-stack.ps1 -ResetDatabase  Recria o banco do zero (apaga volume Postgres; aplica migração única)"
    Write-Host "  .\scripts\run-local-stack.ps1 -Help         Esta ajuda"
    Write-Host ""
    Write-Host "Depois, em outro terminal, rode o app:"
    Write-Host "  cd frontend\araponga.app"
    Write-Host "  .\scripts\run-app-local.ps1"
    Write-Host "  (ou: flutter run --dart-define=BFF_BASE_URL=http://localhost:5001)"
    Write-Host ""
    Write-Host "URLs locais:"
    Write-Host "  API:  http://localhost:8080   (home; Swagger: /swagger)"
    Write-Host "  BFF:  http://localhost:5001   (app usa esta)"
    Write-Host ""
    Write-Host "Se a API ficar em Restarting: docker compose -f docker-compose.dev.yml logs api"
    Write-Host "Após mudanças no backend:     docker compose -f docker-compose.dev.yml build api"
    Write-Host "Seed manual (se falhar):       .\scripts\seed\run-seed-camburi.ps1"
    Write-Host "Recriar banco (sem dados):     .\scripts\run-local-stack.ps1 -ResetDatabase"
    Write-Host ""
}

if ($Help) { Show-Help; exit 0 }

Push-Location $RepoRoot | Out-Null
try {
    # 0) Limpar binários antigos do backend (evita DLLs/artefatos desatualizados)
    $BackendRoot = Join-Path $RepoRoot "backend"
    if (Test-Path $BackendRoot) {
        Write-Info "Limpando binários antigos do backend..."
        Get-ChildItem -Path $BackendRoot -Include "bin", "obj" -Recurse -Directory -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
        & dotnet clean (Join-Path $BackendRoot "Araponga.Api.Bff\Araponga.Api.Bff.csproj") --nologo -v q 2>$null
        if ($LASTEXITCODE -eq 0) { Write-Ok "Backend/BFF: bin e obj limpos." } else { Write-Warn "dotnet clean BFF falhou (não bloqueante)." }
    }

    # 1) Garantir .env (docker-compose.dev.yml usa variáveis do .env)
    if (-not (Test-Path (Join-Path $RepoRoot ".env"))) {
        Write-Info "Arquivo .env não encontrado. Configurando ambiente..."
        & (Join-Path $RepoRoot "scripts\setup-env.ps1") -CalledByStack
        if ($LASTEXITCODE -ne 0) { exit 1 }
    }

    # 2) Docker: subir API + dependências (postgres, redis, minio)
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

        if ($ResetDatabase) {
            Write-Warn "ResetDatabase: parando containers e removendo volume do Postgres (todos os dados do banco serão apagados)."
            # down -v escreve "Container xxx Stopping" em stderr; evitar que PowerShell trate como erro (Erro inesperado).
            $prevErrPref = $ErrorActionPreference
            $ErrorActionPreference = "SilentlyContinue"
            try {
                & docker compose -f $ComposeFile down -v *>$null
            } finally {
                $ErrorActionPreference = $prevErrPref
            }
            if ($LASTEXITCODE -ne 0) {
                Write-Warn "docker compose down -v retornou $LASTEXITCODE (pode ser normal se não havia containers)."
            }
            Write-Info "Reconstruindo imagem da API para garantir migração única atual (ex.: coluna PasswordHash)."
            & docker compose -f $ComposeFile build api
            $buildExit = $LASTEXITCODE
            if ($buildExit -ne 0) {
                Write-Err "Falha ao construir a imagem da API (código $buildExit). Corrija e execute novamente."
                exit 1
            }
            Write-Ok "Imagem da API construída. Subindo containers (banco novo; migração única será aplicada)..."
        }

        Write-Info "Subindo containers (postgres, redis, minio, api)..."
        & docker compose -f $ComposeFile up -d
        if ($LASTEXITCODE -ne 0) {
            $DockerOk = $false
            Show-StackSummary -DockerOk $false -ComposeFile $ComposeFile
            exit 1
        }
        Write-Ok "Docker: containers subiram."
        Write-Info "Aguardando API ficar pronta (migrações podem levar até ~60s na primeira subida)..."
        $ApiHealthy = $false
        $attempts = 0
        $maxAttempts = 60
        while ($attempts -lt $maxAttempts) {
            try {
                $r = Invoke-WebRequest -Uri "http://localhost:8080/health" -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
                if ($r.StatusCode -eq 200) { $ApiHealthy = $true; break }
            } catch { }
            $attempts++
            Start-Sleep -Seconds 1
        }
        if (-not $ApiHealthy) {
            Write-Warn "API não respondeu em /health em ${maxAttempts}s (container pode estar reiniciando). Subindo BFF mesmo assim."
            Write-Host "  Dica: docker compose -f docker-compose.dev.yml logs api" -ForegroundColor DarkGray
        } else {
            Write-Ok "API respondendo em http://localhost:8080/health"
        }

        # Seeds: Camburi + Boiçucanga (executa SQL dentro do container Postgres)
        $envFile = Join-Path $RepoRoot ".env"
        if (Test-Path -LiteralPath $envFile) {
            Get-Content $envFile -Encoding UTF8 | ForEach-Object {
                if ($_ -match '^\s*POSTGRES_(\w+)=(.*)$') {
                    $key = "POSTGRES_$($matches[1])"
                    $val = $matches[2].Trim().Trim('"').Trim("'")
                    Set-Item -Path "env:$key" -Value $val -ErrorAction SilentlyContinue
                }
            }
        }
        $PgUser = if ($env:POSTGRES_USER) { $env:POSTGRES_USER } else { "araponga" }
        $PgDb   = if ($env:POSTGRES_DB)   { $env:POSTGRES_DB }   else { "araponga" }
        foreach ($seedName in @('camburi', 'boicucanga')) {
            $SqlFile = Join-Path $RepoRoot "scripts\seed\seed-$seedName.sql"
            if (Test-Path -LiteralPath $SqlFile) {
                try {
                    Write-Info "Executando seed $seedName no container Postgres (UTF-8)..."
                    $SeedPathInContainer = "/tmp/seed-$seedName.sql"
                    docker cp $SqlFile araponga-postgres:${SeedPathInContainer}
                    if ($LASTEXITCODE -ne 0) {
                        Write-Warn "Falha ao copiar seed $seedName para o container."
                    } else {
                        docker exec -e PGCLIENTENCODING=UTF8 araponga-postgres psql -U $PgUser -d $PgDb -f $SeedPathInContainer -v ON_ERROR_STOP=1
                        $runExit = $LASTEXITCODE
                        docker exec araponga-postgres rm -f $SeedPathInContainer 2>$null
                        if ($runExit -eq 0) {
                            Write-Ok "Seed $seedName aplicado."
                        } else {
                            Write-Warn "Seed $seedName falhou (código $runExit)."
                        }
                    }
                } catch {
                    Write-Warn "Seed $seedName não executado: $_"
                }
            } else {
                Write-Warn "Arquivo de seed não encontrado: $SqlFile"
            }
        }

        Show-StackSummary -DockerOk $true -ComposeFile $ComposeFile -ApiHealthy $ApiHealthy
    } else {
        Write-Info "SkipDocker: assumindo API em http://localhost:8080"
        Show-StackSummary -DockerOk $true -ComposeFile $ComposeFile
    }

    # 3) BFF: apontar para API local e escutar em 5001 (padrão do app)
    $env:Bff__ApiBaseUrl = "http://localhost:8080"
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:ASPNETCORE_URLS = "http://localhost:5001"

    if (-not (Test-Path $BffProject)) {
        Write-Err "BFF não encontrado: $BffProject"
        exit 1
    }

    if (Test-PortInUse -Port 5001) {
        Write-Warn "Porta 5001 já está em uso. Encerrando o processo que a usa para reiniciar o BFF..."
        Stop-ProcessOnPort -Port 5001
        if (Test-PortInUse -Port 5001) {
            Write-Err "Não foi possível liberar a porta 5001. Pare o processo manualmente e execute o script novamente."
            Write-Host "  Para ver o que usa a porta: Get-NetTCPConnection -LocalPort 5001" -ForegroundColor DarkGray
            exit 1
        }
        Write-Ok "Porta 5001 liberada. Iniciando BFF..."
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
        Start-Sleep -Seconds 2
        if (-not $NoStartApp) { Open-StackInBrowser -RepoRoot $RepoRoot -StartApp:$true }
        Write-Host ""
        if ($NoStartApp) {
            Write-Info "Para rodar o app: cd frontend\araponga.app; .\scripts\run-app-local.ps1"
        }
        Write-Host "  API: http://localhost:8080  |  BFF: http://localhost:5001" -ForegroundColor DarkGray
        Write-Host ""
    } else {
        Write-Info "Iniciando BFF em http://localhost:5001 (ApiBaseUrl=http://localhost:8080)"
        if (-not $NoStartApp) { Open-StackInBrowser -RepoRoot $RepoRoot -StartApp:$true }
        Write-Host ""
        Write-Host "  O BFF estará disponível quando aparecer 'Application started' abaixo (atualize a aba do BFF se precisar)." -ForegroundColor Cyan
        if ($NoStartApp) {
            Write-Host "  Para rodar o app em outro terminal: cd frontend\araponga.app; .\scripts\run-app-local.ps1" -ForegroundColor Yellow
        }
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
