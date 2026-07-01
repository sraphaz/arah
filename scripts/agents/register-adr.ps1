#Requires -Version 5.1
<#
.SYNOPSIS
  Registra ADR (ADR-020+): cria arquivo, atualiza registry e índice em 10_ARCHITECTURE_DECISIONS.md.
.EXAMPLE
  ./register-adr.ps1 -Title "Arah Core control plane" -Status proposed `
    -Context "..." -Decision "..." -Consequences "..." -SpecId FASE53-arah-core
#>
param(
    [Parameter(Mandatory)]
    [string]$Title,
    [ValidateSet('proposed', 'accepted', 'deprecated', 'superseded')]
    [string]$Status = 'proposed',
    [string]$Context = '',
    [string]$Decision = '',
    [string]$Consequences = '',
    [string]$Alternatives = '1. _(preencher alternativas rejeitadas)_',
    [string]$SpecId = '',
    [string]$LikeC4View = '',
    [string]$DiagramPath = '',
    [switch]$DryRun,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$AdrsDir = Join-Path $Root 'docs/architecture/adrs'
$RegistryPath = Join-Path $Root 'docs/architecture/ADR-REGISTRY.yaml'
$IndexPath = Join-Path $Root 'docs/architecture/10_ARCHITECTURE_DECISIONS.md'
$TemplatePath = Join-Path $AdrsDir '_template.md'

function ConvertTo-Slug {
    param([string]$Text)
    $s = $Text.ToLowerInvariant()
    $s = $s -replace '[^a-z0-9\s-]', ''
    $s = $s -replace '\s+', '-'
    $s = $s.Trim('-')
    if ($s.Length -gt 60) { $s = $s.Substring(0, 60).TrimEnd('-') }
    return $s
}

function Get-NextAdrNumber {
    param([string]$RegistryRaw)
    if ($RegistryRaw -match '(?m)^next_number:\s*(\d+)') {
        return [int]$Matches[1]
    }
    $max = 19
    foreach ($m in [regex]::Matches($RegistryRaw, 'id:\s*ADR-(\d+)')) {
        $n = [int]$m.Groups[1].Value
        if ($n -gt $max) { $max = $n }
    }
    return $max + 1
}

if (-not (Test-Path $TemplatePath)) {
    Write-Error "Template not found: $TemplatePath"
    exit 1
}

$registryRaw = if (Test-Path $RegistryPath) { Get-Content $RegistryPath -Raw } else { "next_number: 20`n" }
$num = Get-NextAdrNumber -RegistryRaw $registryRaw
$adrId = "ADR-{0:D3}" -f $num
$slug = ConvertTo-Slug -Text $Title
$fileName = "$adrId-$slug.md"
$fileRel = "docs/architecture/adrs/$fileName"
$filePath = Join-Path $AdrsDir $fileName

if (Test-Path $filePath) {
    Write-Error "ADR file already exists: $fileRel"
    exit 1
}

$date = (Get-Date).ToString('yyyy-MM-dd')
$specLine = if ($SpecId) { $SpecId } else { '_nenhuma_' }
$viewLine = if ($LikeC4View) { $LikeC4View } else { '_nenhuma_' }
$diagramBlock = if ($DiagramPath) {
    "Ver diagrama: ``$DiagramPath``"
} else {
    '_Opcional: export LikeC4 → docs/architecture/diagrams/_'
}

$body = (Get-Content $TemplatePath -Raw) `
    -replace '\{\{TITLE\}\}', $Title `
    -replace '\{\{STATUS\}\}', $Status `
    -replace '\{\{DATE\}\}', $date `
    -replace '\{\{SPEC_ID\}\}', $specLine `
    -replace '\{\{LIKEC4_VIEW\}\}', $viewLine `
    -replace '\{\{CONTEXT\}\}', $(if ($Context) { $Context } else { '_Preencher contexto._' }) `
    -replace '\{\{DECISION\}\}', $(if ($Decision) { $Decision } else { '_Preencher decisão._' }) `
    -replace '\{\{CONSEQUENCES\}\}', $(if ($Consequences) { $Consequences } else { '_Preencher consequências._' }) `
    -replace '\{\{ALTERNATIVES\}\}', $Alternatives `
    -replace '\{\{DIAGRAM\}\}', $diagramBlock

$statusLabel = switch ($Status) {
    'proposed' { 'Proposto' }
    'accepted' { 'Aceito' }
    'deprecated' { 'Depreciado' }
    'superseded' { 'Substituído' }
    default { $Status }
}

$indexEntry = @"

## $adrId`: $Title

**Status**: $statusLabel  
**Data**: $date  
**Arquivo**: [$fileRel](./adrs/$fileName)  
$(if ($SpecId) { "**Spec-Id**: $SpecId  " })$(if ($LikeC4View) { "**LikeC4**: $LikeC4View  " })

Resumo: $(if ($Decision) { ($Decision -replace "`n", ' ').Substring(0, [Math]::Min(200, ($Decision -replace "`n", ' ').Length)) } else { '_Ver arquivo completo._' })

---

"@

$registryEntry = @"
  - id: $adrId
    title: "$($Title -replace '"', '\"')"
    status: $Status
    file: $fileRel
    spec_id: $(if ($SpecId) { $SpecId } else { 'null' })
    date: $date
"@

$result = [ordered]@{
    id       = $adrId
    title    = $Title
    status   = $Status
    file     = $fileRel
    spec_id  = $SpecId
    dry_run  = [bool]$DryRun
}

if ($DryRun) {
    $result.preview = $body.Substring(0, [Math]::Min(500, $body.Length))
    if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4 }
    exit 0
}

New-Item -Path $AdrsDir -ItemType Directory -Force | Out-Null
Set-Content -Path $filePath -Value $body -Encoding UTF8

# Update registry
$newNext = $num + 1
$updatedRegistry = $registryRaw
if ($updatedRegistry -match '(?m)^next_number:\s*\d+') {
    $updatedRegistry = $updatedRegistry -replace '(?m)^next_number:\s*\d+', "next_number: $newNext"
} else {
    $updatedRegistry = "next_number: $newNext`n" + $updatedRegistry
}
if ($updatedRegistry -match '(?ms)^adrs:\s*\n') {
    $updatedRegistry = $updatedRegistry.TrimEnd() + "`n$registryEntry`n"
} else {
    $updatedRegistry += "`nadrs:`n$registryEntry`n"
}
$updatedRegistry = $updatedRegistry -replace '(?m)^updated:\s*.+$', "updated: $date"
Set-Content -Path $RegistryPath -Value $updatedRegistry -Encoding UTF8

# Insert index entry before ## Referências
if (Test-Path $IndexPath) {
    $indexRaw = Get-Content $IndexPath -Raw
    if ($indexRaw -match '(?ms)^## Referências\s*\n') {
        $indexRaw = $indexRaw -replace '(?ms)^## Referências\s*\n', "$indexEntry`n## Referências`n"
    } else {
        $indexRaw += "`n$indexEntry"
    }
    Set-Content -Path $IndexPath -Value $indexRaw -Encoding UTF8
}

Write-Host "Registered $adrId → $fileRel"
$result.created = $true
if ($Json) { $result | ConvertTo-Json -Depth 4 } else { $result | ConvertTo-Json -Depth 4 }
exit 0
