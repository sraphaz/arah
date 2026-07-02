#Requires -Version 5.1
<#
.SYNOPSIS
  Publica parecer consultivo de agente de domínio (negócio) no PR/issue.
#>
param(
    [Parameter(Mandatory)]
    [string]$DomainId,
    [int]$IssueNumber = 0,
    [int]$PrNumber = 0,
    [string]$Trigger = 'choreography',
    [string[]]$ChangedFiles = @(),
    [switch]$PostComment,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$AgentsDir = Join-Path $Root '.agents'

$manifestPath = Join-Path $AgentsDir "domain/$DomainId.agent.yaml"
if (-not (Test-Path $manifestPath)) {
    $found = Get-ChildItem -Path $AgentsDir -Recurse -Filter "$DomainId.agent.yaml" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) { $manifestPath = $found.FullName }
}
if (-not (Test-Path $manifestPath)) {
    Write-Error "Domain agent not found: $DomainId"
    exit 1
}

$raw = Get-Content $manifestPath -Raw
$name = if ($raw -match '(?m)^name:\s*(.+)$') { $Matches[1].Trim() } else { $DomainId }
$enrich = if ($raw -match '(?m)^enrich:[ \t]*\|[ \t]*\r?\n((?:[ \t]{2}.+\r?\n)+)') { ($Matches[1] -replace '(?m)^  ', '').Trim() }
          elseif ($raw -match '(?m)^enrich:[ \t]+(.+)$') { $Matches[1].Trim() } else { '' }
$validate = if ($raw -match '(?m)^validate:[ \t]*\|[ \t]*\r?\n((?:[ \t]{2}.+\r?\n)+)') { ($Matches[1] -replace '(?m)^  ', '').Trim() }
            elseif ($raw -match '(?m)^validate:[ \t]+(.+)$') { $Matches[1].Trim() } else { '' }
$refs = @()
if ($raw -match '(?m)^references:[ \t]*\r?\n((?:[ \t]+-[ \t].+\r?\n)+)') {
    $refs = [regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') | ForEach-Object { $_.Groups[1].Value.Trim() }
}

$filesList = if ($ChangedFiles.Count -gt 0) {
    ($ChangedFiles | Select-Object -First 8 | ForEach-Object { "- ``$_``" }) -join "`n"
} else { '_nenhum arquivo listado_' }

$refList = if ($refs.Count -gt 0) {
    ($refs | ForEach-Object { "- ``$_``" }) -join "`n"
} else { '_sem referências_' }

$timestamp = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
$marker = "<!-- arah-domain-consult:$DomainId -->"
$targetNumber = if ($PrNumber -gt 0) { $PrNumber } else { $IssueNumber }

$body = @"
$marker
## Parecer de domínio: $name

**ID:** ``$DomainId`` | **Quando:** $timestamp UTC | **Gatilho:** $Trigger

### Enriquecimento (negócio)
$enrich

### Validar no PR
$validate

### Arquivos relacionados
$filesList

### Referências
$refList

---
_Autonomia via coreografia (``.agents/choreography.yaml``). Agente consultivo — não altera código._
"@

$result = [ordered]@{
    domain_id   = $DomainId
    domain_name = $name
    marker      = $marker
    timestamp   = $timestamp
}

if ($PostComment -and $targetNumber -gt 0) {
    if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
        Write-Error 'gh CLI required for -PostComment'
        exit 1
    }
    Push-Location $Root
    try {
        $repo = gh repo view --json nameWithOwner -q .nameWithOwner
        $existing = gh api "repos/$repo/issues/$targetNumber/comments" --paginate 2>$null | ConvertFrom-Json
        if ($existing -isnot [array]) { $existing = @($existing) }
        $match = $existing | Where-Object { $_.body -and $_.body -match [regex]::Escape($marker) } | Select-Object -First 1
        if ($match) {
            gh api -X PATCH "repos/$repo/issues/comments/$($match.id)" -f body="$body" | Out-Null
            $result.comment_action = 'updated'
        } else {
            gh issue comment $targetNumber --body "$body" | Out-Null
            $result.comment_action = 'created'
        }
    } finally { Pop-Location }
}

if ($Json) { $result | ConvertTo-Json -Depth 4 } else { Write-Output $body }
exit 0
