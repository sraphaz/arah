# Executa a ingestão de dados do território Camburi (seed-camburi.sql).
# Requer: psql no PATH ou em caminho típico do PostgreSQL no Windows.
#
# Uso:
#   .\scripts\seed\run-seed-camburi.ps1
#   .\run-seed-camburi.ps1   # dentro de scripts/seed
#
# Variáveis de ambiente (opcionais): POSTGRES_HOST, POSTGRES_PORT, POSTGRES_DB, POSTGRES_USER, POSTGRES_PASSWORD

$ErrorActionPreference = "Stop"
$ScriptDir = $PSScriptRoot
$RepoRoot = (Get-Item $ScriptDir).Parent.Parent.FullName
$SqlFile = Join-Path $ScriptDir "seed-camburi.sql"

$Host = if ($env:POSTGRES_HOST) { $env:POSTGRES_HOST } else { "localhost" }
$Port = if ($env:POSTGRES_PORT) { $env:POSTGRES_PORT } else { "5432" }
$Db   = if ($env:POSTGRES_DB)   { $env:POSTGRES_DB }   else { "araponga" }
$User = if ($env:POSTGRES_USER) { $env:POSTGRES_USER } else { "araponga" }
$Pass = if ($env:POSTGRES_PASSWORD) { $env:POSTGRES_PASSWORD } else { "araponga" }

if (-not (Test-Path -LiteralPath $SqlFile)) {
    Write-Error "Arquivo não encontrado: $SqlFile"
}

# Encontrar psql
$psql = $null
if (Get-Command psql -ErrorAction SilentlyContinue) {
    $psql = "psql"
} else {
    $possible = @(
        "C:\Program Files\PostgreSQL\16\bin\psql.exe",
        "C:\Program Files\PostgreSQL\15\bin\psql.exe",
        "C:\Program Files\PostgreSQL\14\bin\psql.exe"
    )
    foreach ($p in $possible) {
        if (Test-Path -LiteralPath $p) {
            $psql = $p
            break
        }
    }
}

if (-not $psql) {
    Write-Error "psql não encontrado. Instale o PostgreSQL client ou adicione psql ao PATH."
}

$env:PGPASSWORD = $Pass
try {
    & $psql -h $Host -p $Port -U $User -d $Db -f $SqlFile
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
    Write-Host "Ingestão Camburi concluída: $SqlFile" -ForegroundColor Green
} finally {
    Remove-Item env:PGPASSWORD -ErrorAction SilentlyContinue
}
