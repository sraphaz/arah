# Busca o contorno oficial do município (malha IBGE) e gera/aplica seed no Postgres local.
#
# Fonte: IBGE — Malhas Territoriais v3 (limites legais, SIRGAS 2000).
#   https://servicodados.ibge.gov.br/api/docs/malhas?versao=3
#   https://servicodados.ibge.gov.br/api/v1/localidades/municipios/{codigo}
#
# Uso:
#   .\scripts\seed\fetch-ibge-municipality-boundary.ps1 -City Socorro -State SP
#   .\scripts\seed\fetch-ibge-municipality-boundary.ps1 -IbgeCode 3552106 -Apply
#   .\scripts\seed\fetch-ibge-municipality-boundary.ps1 -City Socorro -State SP -OutputSql scripts\seed\seed-socorro.sql
#
param(
    [string]$City,
    [string]$Uf,
    [int]$IbgeCode = 0,
    [ValidateRange(0, 5)]
    [int]$Resolucao = 3,
    [ValidateSet('minima', 'intermediaria', 'maxima')]
    [string]$Qualidade = 'intermediaria',
    [string]$TerritoryName = '',
    [string]$Description = '',
    [string]$OutputSql = '',
    [string]$ContainerName = 'arah-postgres',
    [switch]$Apply,
    [switch]$Help
)

$ErrorActionPreference = 'Stop'
$ScriptDir = $PSScriptRoot
$RepoRoot = (Get-Item $ScriptDir).Parent.Parent.FullName

function Show-Help {
    Write-Host @"

=== Contorno municipal IBGE → seed Arah ===

Resolve código IBGE (API Localidades), baixa GeoJSON (API Malhas v3),
converte para BoundaryPolygonJson do domínio Arah e opcionalmente aplica no Postgres.

Exemplos:
  .\scripts\seed\fetch-ibge-municipality-boundary.ps1 -City Socorro -Uf SP -Apply
  .\scripts\seed\fetch-ibge-municipality-boundary.ps1 -IbgeCode 3552106 -OutputSql scripts\seed\seed-socorro.sql

"@
}

if ($Help) { Show-Help; exit 0 }

function Resolve-IbgeMunicipalityCode {
    param([string]$CityName, [string]$UfSigla)
    $ufCode = $UfSigla.Trim().ToUpper()
    if ($ufCode.Length -ne 2) { throw 'Uf deve ser sigla (ex.: SP).' }
    $states = Invoke-RestMethod -Uri 'https://servicodados.ibge.gov.br/api/v1/localidades/estados' -TimeoutSec 30
    $state = $states | Where-Object { $_.sigla -eq $ufCode } | Select-Object -First 1
    if (-not $state) { throw "UF não encontrada: $ufCode" }
    $municipios = Invoke-RestMethod -Uri "https://servicodados.ibge.gov.br/api/v1/localidades/estados/$($state.id)/municipios" -TimeoutSec 60
    $match = $municipios | Where-Object { $_.nome -ieq $CityName.Trim() } | Select-Object -First 1
    if (-not $match) { throw "Município '$CityName' não encontrado em $ufCode (IBGE)." }
    return [PSCustomObject]@{
        Code = [int]$match.id
        Name = $match.nome
        State = $ufCode
    }
}

function Get-LargestOuterRing {
    param($Geometry)
    if ($Geometry.type -eq 'Polygon') {
        return $Geometry.coordinates[0]
    }
    if ($Geometry.type -eq 'MultiPolygon') {
        $largest = $null
        $max = 0
        foreach ($poly in $Geometry.coordinates) {
            $candidate = $poly[0]
            if ($candidate.Count -gt $max) { $max = $candidate.Count; $largest = $candidate }
        }
        return $largest
    }
    throw "Geometria não suportada: $($Geometry.type)"
}

function Convert-RingToArahPoints {
    param($Ring)
    $points = New-Object System.Collections.Generic.List[object]
    foreach ($coord in $Ring) {
        $lng = [double]$coord[0]
        $lat = [double]$coord[1]
        $points.Add([PSCustomObject]@{ Latitude = [math]::Round($lat, 6); Longitude = [math]::Round($lng, 6) })
    }
    return $points
}

function Get-Centroid {
    param($Points)
    $sumLat = 0.0; $sumLng = 0.0
    foreach ($p in $Points) {
        $sumLat += $p.Latitude
        $sumLng += $p.Longitude
    }
    $n = [math]::Max(1, $Points.Count)
    return [PSCustomObject]@{
        Latitude = [math]::Round($sumLat / $n, 6)
        Longitude = [math]::Round($sumLng / $n, 6)
    }
}

if ($IbgeCode -le 0) {
    if ([string]::IsNullOrWhiteSpace($City) -or [string]::IsNullOrWhiteSpace($Uf)) {
        throw 'Informe -IbgeCode ou -City e -Uf.'
    }
    $resolved = Resolve-IbgeMunicipalityCode -CityName $City -UfSigla $Uf
    $IbgeCode = $resolved.Code
    if ([string]::IsNullOrWhiteSpace($TerritoryName)) { $TerritoryName = $resolved.Name }
    if ([string]::IsNullOrWhiteSpace($City)) { $City = $resolved.Name }
    if ([string]::IsNullOrWhiteSpace($Uf)) { $Uf = $resolved.State }
} else {
    $meta = Invoke-RestMethod -Uri "https://servicodados.ibge.gov.br/api/v1/localidades/municipios/$IbgeCode" -TimeoutSec 30
    if ([string]::IsNullOrWhiteSpace($TerritoryName)) { $TerritoryName = $meta.nome }
    if ([string]::IsNullOrWhiteSpace($City)) { $City = $meta.nome }
    if ([string]::IsNullOrWhiteSpace($Uf)) { $Uf = $meta.microrregiao.mesorregiao.UF.sigla }
}

$malhaUrl = "https://servicodados.ibge.gov.br/api/v3/malhas/municipios/$IbgeCode" +
    "?formato=application/vnd.geo+json&qualidade=$Qualidade&resolucao=$Resolucao"
Write-Host "IBGE $TerritoryName ($City/$Uf) código $IbgeCode" -ForegroundColor Cyan
Write-Host "Malha: $malhaUrl" -ForegroundColor DarkGray

$geo = Invoke-RestMethod -Uri $malhaUrl -Headers @{ Accept = 'application/vnd.geo+json' } -TimeoutSec 120
$feature = if ($geo.type -eq 'FeatureCollection') { $geo.features[0] } else { $geo }
$ring = Get-LargestOuterRing -Geometry $feature.geometry
if ($ring.Count -lt 3) { throw 'Polígono IBGE inválido (menos de 3 pontos).' }

$arahPoints = Convert-RingToArahPoints -Ring $ring
$centroid = Get-Centroid -Points $arahPoints
$polygonJson = ($arahPoints | ForEach-Object {
    "{`"Latitude`":$($_.Latitude),`"Longitude`":$($_.Longitude)}"
}) -join ','

if ([string]::IsNullOrWhiteSpace($Description)) {
    $Description = "Perímetro oficial do município (malha IBGE $IbgeCode, qualidade $Qualidade, resolução $Resolucao). SIRGAS 2000."
}

$territoryId = [guid]::NewGuid().ToString()
$userId = [guid]::NewGuid().ToString()
$membershipId = [guid]::NewGuid().ToString()
$postId = [guid]::NewGuid().ToString()

$safeName = $TerritoryName.Replace("'", "''")
$safeCity = $City.Replace("'", "''")
$safeState = $Uf.Replace("'", "''")
$safeDesc = $Description.Replace("'", "''")

$sql = @"
-- Território: $TerritoryName — $City/$State (IBGE $IbgeCode)
-- Fonte: IBGE Malhas v3 — limites legais do município (SIRGAS 2000)
-- Gerado por: scripts/seed/fetch-ibge-municipality-boundary.ps1
SET client_encoding = 'UTF8';

INSERT INTO territories (
    "Id", "ParentTerritoryId", "Name", "Description", "Status", "City", "State",
    "Latitude", "Longitude", "CreatedAtUtc", "RadiusKm", "BoundaryPolygonJson"
)
SELECT
    '$territoryId'::uuid,
    NULL,
    '$safeName',
    '$safeDesc',
    2,
    '$safeCity',
    '$safeState',
    $($centroid.Latitude),
    $($centroid.Longitude),
    NOW() AT TIME ZONE 'UTC',
    NULL,
    '[$polygonJson]'::jsonb
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
    'seed-ibge-$IbgeCode@arah.local',
    'seed',
    'seed-ibge-$IbgeCode',
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

INSERT INTO community_posts (
    "Id", "TerritoryId", "AuthorUserId", "Title", "Content", "Type", "Visibility", "Status",
    "MapEntityId", "ReferenceType", "ReferenceId", "CreatedAtUtc", "EditedAtUtc", "EditCount", "TagsJson", "RowVersion"
)
SELECT
    '$postId'::uuid,
    t."Id",
    '$userId'::uuid,
    'Bem-vindo a $safeName',
    'Território com contorno oficial IBGE ($IbgeCode). Use para validar mapa, onboarding e feed.',
    1,
    1,
    0,
    NULL,
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC',
    NULL,
    0,
    '["ibge","oficial"]'::jsonb,
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = '$safeName' AND t."City" = '$safeCity' AND t."State" = '$safeState'
  AND NOT EXISTS (SELECT 1 FROM community_posts p WHERE p."TerritoryId" = t."Id" LIMIT 1);
"@

if ([string]::IsNullOrWhiteSpace($OutputSql)) {
    $slug = ($TerritoryName -replace '\s+', '-').ToLower()
    $OutputSql = Join-Path $ScriptDir "seed-$slug-$Uf-ibge.sql"
}

$sql | Set-Content -Path $OutputSql -Encoding UTF8
Write-Host "SQL gerado: $OutputSql ($($arahPoints.Count) pontos, centro $($centroid.Latitude), $($centroid.Longitude))" -ForegroundColor Green

if (-not $Apply) {
    Write-Host "Use -Apply para gravar no Postgres ou execute o SQL manualmente." -ForegroundColor Yellow
    exit 0
}

$envFile = Join-Path $RepoRoot '.env'
if (Test-Path -LiteralPath $envFile) {
    Get-Content $envFile -Encoding UTF8 | ForEach-Object {
        if ($_ -match '^\s*POSTGRES_(\w+)=(.*)$') {
            Set-Item -Path "env:POSTGRES_$($matches[1])" -Value $matches[2].Trim().Trim('"').Trim("'") -ErrorAction SilentlyContinue
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
    $containerPath = '/tmp/arah-seed-ibge.sql'
    docker cp $OutputSql "${ContainerName}:${containerPath}"
    docker exec -e PGCLIENTENCODING=UTF8 $ContainerName psql -U $PgUser -d $PgDb -f $containerPath -v ON_ERROR_STOP=1
    docker exec $ContainerName rm -f $containerPath 2>$null
} else {
    Write-Error "Container $ContainerName não encontrado. Suba run-local-stack.ps1 ou aplique: psql -f $OutputSql"
}

Write-Host "Território $TerritoryName aplicado com polígono IBGE." -ForegroundColor Green
