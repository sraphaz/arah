# ============================================
# Araponga - Script de Gerenciamento Docker
# ============================================
# Script PowerShell para facilitar o uso do ambiente Docker de desenvolvimento/pré-produção

param(
    [Parameter(Position=0)]
    [ValidateSet("up", "down", "restart", "logs", "status", "clean", "shell", "db-migrate", "db-reset", "help")]
    [string]$Command = "help",
    
    [Parameter()]
    [string]$Service = "",
    
    [Parameter()]
    [switch]$Build,
    
    [Parameter()]
    [switch]$Detached
)

$ErrorActionPreference = "Stop"
$composeFile = "docker-compose.dev.yml"
$envFile = ".env"

# Cores para output
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Show-Help {
    Write-ColorOutput "`n=== Araponga - Docker Dev Environment ===" "Cyan"
    Write-ColorOutput "`nUso: .\scripts\docker-dev.ps1 [comando] [opções]`n" "Yellow"
    Write-ColorOutput "Comandos disponíveis:" "Green"
    Write-ColorOutput "  up              - Inicia todos os serviços"
    Write-ColorOutput "  down            - Para todos os serviços"
    Write-ColorOutput "  restart         - Reinicia todos os serviços"
    Write-ColorOutput "  logs            - Mostra logs (use -Service para filtrar)"
    Write-ColorOutput "  status          - Mostra status dos containers"
    Write-ColorOutput "  clean           - Remove containers, volumes e imagens"
    Write-ColorOutput "  shell           - Abre shell no container da API"
    Write-ColorOutput "  db-migrate      - Aplica migrações do banco de dados"
    Write-ColorOutput "  db-reset        - Reseta o banco de dados (CUIDADO!)"
    Write-ColorOutput "  help            - Mostra esta ajuda`n"
    Write-ColorOutput "Opções:" "Green"
    Write-ColorOutput "  -Service <nome> - Aplica comando a um serviço específico"
    Write-ColorOutput "  -Build          - Força rebuild das imagens"
    Write-ColorOutput "  -Detached       - Roda em background (docker-compose up -d)`n"
    Write-ColorOutput "Exemplos:" "Cyan"
    Write-ColorOutput "  .\scripts\docker-dev.ps1 up -Build"
    Write-ColorOutput "  .\scripts\docker-dev.ps1 logs -Service api"
    Write-ColorOutput "  .\scripts\docker-dev.ps1 shell`n"
}

function Test-EnvFile {
    if (-not (Test-Path $envFile)) {
        Write-ColorOutput "⚠️  Arquivo .env não encontrado!" "Yellow"
        Write-ColorOutput "📝 Criando .env a partir de .env.example..." "Cyan"
        
        if (Test-Path ".env.example") {
            Copy-Item ".env.example" $envFile
            Write-ColorOutput "✅ Arquivo .env criado. Por favor, edite-o e configure o JWT_SIGNINGKEY!" "Green"
            Write-ColorOutput "   Gere um secret com: openssl rand -base64 32" "Yellow"
        } else {
            Write-ColorOutput "❌ Arquivo .env.example não encontrado!" "Red"
            exit 1
        }
    }
}

function Start-Services {
    Test-EnvFile
    
    Write-ColorOutput "🚀 Iniciando ambiente Araponga..." "Cyan"
    
    $buildFlag = if ($Build) { "--build" } else { "" }
    $detachedFlag = if ($Detached) { "-d" } else { "" }
    
    $cmd = "docker-compose -f $composeFile up $buildFlag $detachedFlag"
    if ($Service) {
        $cmd += " $Service"
    }
    
    Write-ColorOutput "Executando: $cmd" "Gray"
    Invoke-Expression $cmd
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "`n✅ Ambiente iniciado com sucesso!" "Green"
        Write-ColorOutput "`n📍 Serviços disponíveis:" "Cyan"
        Write-ColorOutput "   API:        http://localhost:8080" "White"
        Write-ColorOutput "   Swagger:    http://localhost:8080/swagger" "White"
        Write-ColorOutput "   Health:     http://localhost:8080/health" "White"
        Write-ColorOutput "   MinIO API:  http://localhost:9000" "White"
        Write-ColorOutput "   MinIO UI:   http://localhost:9001 (minioadmin/minioadmin)" "White"
        Write-ColorOutput "   PostgreSQL: localhost:5432" "White"
        Write-ColorOutput "   Redis:      localhost:6379`n" "White"
    }
}

function Stop-Services {
    Write-ColorOutput "🛑 Parando serviços..." "Yellow"
    docker-compose -f $composeFile down
    Write-ColorOutput "✅ Serviços parados" "Green"
}

function Restart-Services {
    Write-ColorOutput "🔄 Reiniciando serviços..." "Yellow"
    Stop-Services
    Start-Sleep -Seconds 2
    Start-Services -Detached
}

function Show-Logs {
    $followFlag = if ($Service) { "-f $Service" } else { "-f" }
    docker-compose -f $composeFile logs $followFlag
}

function Show-Status {
    Write-ColorOutput "`n📊 Status dos containers:`n" "Cyan"
    docker-compose -f $composeFile ps
}

function Clean-Environment {
    Write-ColorOutput "⚠️  ATENÇÃO: Isso irá remover TODOS os containers, volumes e dados!" "Red"
    $confirm = Read-Host "Tem certeza? Digite 'sim' para confirmar"
    
    if ($confirm -eq "sim") {
        Write-ColorOutput "🧹 Limpando ambiente..." "Yellow"
        docker-compose -f $composeFile down -v --remove-orphans
        Write-ColorOutput "✅ Ambiente limpo" "Green"
    } else {
        Write-ColorOutput "❌ Operação cancelada" "Yellow"
    }
}

function Open-Shell {
    Write-ColorOutput "🐚 Abrindo shell no container da API..." "Cyan"
    docker exec -it araponga-api /bin/bash
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "⚠️  Tentando com /bin/sh..." "Yellow"
        docker exec -it araponga-api /bin/sh
    }
}

function Invoke-DbMigrate {
    Write-ColorOutput "📦 Aplicando migrações do banco de dados..." "Cyan"
    docker exec -it araponga-api dotnet ef database update --project /src/backend/Araponga.Infrastructure --startup-project /src/backend/Araponga.Api
}

function Reset-Database {
    Write-ColorOutput "⚠️  ATENÇÃO: Isso irá APAGAR todos os dados do banco!" "Red"
    $confirm = Read-Host "Tem certeza? Digite 'sim' para confirmar"
    
    if ($confirm -eq "sim") {
        Write-ColorOutput "🗑️  Resetando banco de dados..." "Yellow"
        docker exec -it araponga-api dotnet ef database drop --force --project /src/backend/Araponga.Infrastructure --startup-project /src/backend/Araponga.Api
        docker exec -it araponga-api dotnet ef database update --project /src/backend/Araponga.Infrastructure --startup-project /src/backend/Araponga.Api
        Write-ColorOutput "✅ Banco de dados resetado" "Green"
    } else {
        Write-ColorOutput "❌ Operação cancelada" "Yellow"
    }
}

# Main
switch ($Command.ToLower()) {
    "up" { Start-Services }
    "down" { Stop-Services }
    "restart" { Restart-Services }
    "logs" { Show-Logs }
    "status" { Show-Status }
    "clean" { Clean-Environment }
    "shell" { Open-Shell }
    "db-migrate" { Invoke-DbMigrate }
    "db-reset" { Reset-Database }
    "help" { Show-Help }
    default { Show-Help }
}
