# Cria território de socorro no Postgres local (Docker ou psql direto).
# Use quando o onboarding só mostra Camburi/Boiçucanga e você está em outra região.
#
# Uso rápido (São Paulo capital — preset):
#   .\scripts\seed\run-seed-territory-local.ps1 -Preset sao-paulo-centro
#
# Uso customizado (sua cidade — pegue lat/lng no Google Maps: clique direito no mapa):
#   .\scripts\seed\run-seed-territory-local.ps1 `
#     -Name "Centro" -City "Curitiba" -State "PR" `
#     -Latitude -25.4284 -Longitude -49.2733 -RadiusKm 10
#
# Variáveis: POSTGRES_* ou .env na raiz do repo (como run-local-stack.ps1)

param(
    [ValidateSet('sao-paulo-centro', 'custom')]
    [string]$Preset = 'custom',
    [string]$Name = 'Centro',
    [string]$City = 'São Paulo',
    [string]$State = 'SP',
    [double]$Latitude = -23.55052,
    [double]$Longitude = -46.63331,
    [double]$RadiusKm = 12.0,
    [string]$Description = '',
    [string]$ContainerName = 'arah-postgres',
    [switch]$Help
)

$ErrorActionPreference = 'Stop'
$ScriptDir = $PSScriptRoot
$RepoRoot = (Get-Item $ScriptDir).Parent.Parent.FullName

function Show-Help {
    Write-Host @"

=== Seed de território local (massa de socorro) ===

Presets:
  sao-paulo-centro   Aplica seed-sao-paulo-centro.sql (capital SP, raio 12 km)

Customizado:
  -Name, -City, -State, -Latitude, -Longitude, -RadiusKm

Exemplos:
  .\scripts\seed\run-seed-territory-local.ps1 -Preset sao-paulo-centro
  .\scripts\seed\run-seed-territory-local.ps1 -Name Jardins -City 'São Paulo' -State SP -Latitude -23.561 -Longitude -46.669 -RadiusKm 8

"@
}

if ($Help) { Show-Help; exit 0 }

if ($Preset -eq 'sao-paulo-centro') {
    $sqlFile = Join-Path $ScriptDir 'seed-sao-paulo-centro.sql'
    if (-not (Test-Path -LiteralPath $sqlFile)) {
        Write-Error "Preset não encontrado: $sqlFile"
    }
    $applyFile = $sqlFile
} else {
    if ([string]::IsNullOrWhiteSpace($Description)) {
        $Description = "Território de socorro — $Name, $City/$State (dev/local)."
    }
    $safeName = $Name.Replace("'", "''")
    $safeCity = $City.Replace("'", "''")
    $safeState = $State.Replace("'", "''")
    $safeDesc = $Description.Replace("'", "''")
    $idSeed = [guid]::NewGuid().ToString()
    $userId = [guid]::NewGuid().ToString()
    $membershipId = [guid]::NewGuid().ToString()
    $postId = [guid]::NewGuid().ToString()

    $applyFile = Join-Path $env:TEMP "arah-seed-$($City)-$($Name).sql"
    @"
SET client_encoding = 'UTF8';

INSERT INTO territories (
    "Id", "ParentTerritoryId", "Name", "Description", "Status", "City", "State",
    "Latitude", "Longitude", "CreatedAtUtc", "RadiusKm", "BoundaryPolygonJson"
)
SELECT
    '$idSeed'::uuid,
    NULL,
    '$safeName',
    '$safeDesc',
    2,
    '$safeCity',
    '$safeState',
    $Latitude,
    $Longitude,
    NOW() AT TIME ZONE 'UTC',
    $RadiusKm,
    NULL
WHERE NOT EXISTS (
    SELECT 1 FROM territories
    WHERE "Name" = '$safeName' AND "City" = '$safeCity' AND "State" = '$safeState'
);

INSERT INTO users (
    "Id", "DisplayName", "Email", "AuthProvider", "ExternalId",
    "TwoFactorEnabled", "IdentityVerificationStatus", "CreatedAtUtc", "ForeignDocument"
)
VALUES (
    '$userId'::uuid,
    'Comunidade $safeName',
    'seed-$($safeCity.ToLower())@arah.local',
    'seed',
    'seed-$idSeed',
    false,
    1,
    NOW() AT TIME ZONE 'UTC',
    'SEED'
)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO territory_memberships (
    "Id", "UserId", "TerritoryId", "Role", "ResidencyVerification", "CreatedAtUtc", "RowVersion"
)
SELECT
    '$membershipId'::uuid,
    '$userId'::uuid,
    t."Id",
    2,
    0,
    NOW() AT TIME ZONE 'UTC',
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = '$safeName' AND t."City" = '$safeCity' AND t."State" = '$safeState'
LIMIT 1
ON CONFLICT ("UserId", "TerritoryId") DO NOTHING;
"@ | Set-Content -Path $applyFile -Encoding UTF8
}

# Credenciais Postgres ( .env ou defaults )
$envFile = Join-Path $RepoRoot '.env'
if (Test-Path -LiteralPath $envFile) {
    Get-Content $envFile -Encoding UTF8 | ForEach-Object {
        if ($_ -match '^\s*POSTGRES_(\w+)=(.*)$') {
            $key = "POSTGRES_$($matches[1])"
            $val = $matches[2].Trim().Trim('"').Trim("'")
            Set-Item -Path "env:$key" -Value $val -ErrorAction SilentlyContinue
        }
    }
}
$PgUser = if ($env:POSTGRES_USER) { $env:POSTGRES_USER } else { 'arah' }
$PgDb   = if ($env:POSTGRES_DB)   { $env:POSTGRES_DB }   else { 'arah' }

$dockerRunning = $false
try {
    docker inspect $ContainerName 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { $dockerRunning = $true }
} catch { }

if ($dockerRunning) {
    Write-Host "Aplicando seed via Docker ($ContainerName)..." -ForegroundColor Cyan
    $containerPath = '/tmp/arah-seed-local.sql'
    docker cp $applyFile "${ContainerName}:${containerPath}"
    if ($LASTEXITCODE -ne 0) { Write-Error 'Falha ao copiar SQL para o container.' }
    docker exec -e PGCLIENTENCODING=UTF8 $ContainerName psql -U $PgUser -d $PgDb -f $containerPath -v ON_ERROR_STOP=1
    $exit = $LASTEXITCODE
    docker exec $ContainerName rm -f $containerPath 2>$null
    if ($exit -ne 0) { exit $exit }
} else {
    Write-Host "Container $ContainerName não encontrado; tentando psql local..." -ForegroundColor Yellow
    $PgHost = if ($env:POSTGRES_HOST) { $env:POSTGRES_HOST } else { 'localhost' }
    $Port   = if ($env:POSTGRES_PORT) { $env:POSTGRES_PORT } else { '5432' }
    $Pass   = if ($env:POSTGRES_PASSWORD) { $env:POSTGRES_PASSWORD } else { 'arah' }
    $psql = Get-Command psql -ErrorAction SilentlyContinue
    if (-not $psql) { Write-Error 'Suba o Docker (run-local-stack.ps1) ou instale psql.' }
    $env:PGPASSWORD = $Pass
    try {
        & psql -h $PgHost -p $Port -U $PgUser -d $PgDb -f $applyFile -v ON_ERROR_STOP=1
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    } finally {
        Remove-Item env:PGPASSWORD -ErrorAction SilentlyContinue
    }
}

Write-Host "Seed aplicado: $Name ($City/$State) @ $Latitude , $Longitude (raio ${RadiusKm} km)" -ForegroundColor Green
Write-Host "No app: permita localização e puxe para atualizar a lista de territórios próximos (raio busca: 10 km)." -ForegroundColor DarkGray
