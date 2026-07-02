#Requires -Version 5.1
<#
.SYNOPSIS
  Revisão autônoma de agentes de domínio sobre as mudanças da árvore de trabalho.
  Calcula arquivos alterados (git), resolve a coreografia, gera os pareceres de
  domínio e grava a evidência em .cursor/domain-review.md.

  Projetado para ser chamado por um hook do Cursor (evento `stop`) — a cada
  interação os agentes de domínio agem sem acionamento manual.

.PARAMETER Force
  Regenera os pareceres mesmo que o conjunto de mudanças já tenha sido revisado.

.EXAMPLE
  pwsh -File scripts/agents/domain-autoreview.ps1 -Json
#>
param(
    [switch]$Force,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ScriptDir = $PSScriptRoot
$CursorDir = Join-Path $Root '.cursor'
$OutFile = Join-Path $CursorDir 'domain-review.md'
$StateFile = Join-Path $CursorDir '.domain-review.state'

function Write-ResultJson {
    param([hashtable]$Data)
    ([ordered]@{
        domains        = @($Data.domains)
        changed_count  = [int]$Data.changed_count
        already_reviewed = [bool]$Data.already_reviewed
        review_file    = $Data.review_file
        hash           = $Data.hash
    }) | ConvertTo-Json -Depth 4 -Compress
}

function Get-ChangedFiles {
    $lines = & git -C $Root status --porcelain 2>$null
    if (-not $lines) { return @() }
    $files = @()
    foreach ($line in $lines) {
        if ([string]::IsNullOrWhiteSpace($line)) { continue }
        $path = $line.Substring(3).Trim()
        if ($path -match '->') { $path = ($path -split '->')[-1].Trim() }
        $path = $path.Trim('"').Replace('\', '/')
        if ($path) { $files += $path }
    }
    return @($files | Select-Object -Unique)
}

function Get-StringHash {
    param([string]$Text)
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
        return (($sha.ComputeHash($bytes) | ForEach-Object { $_.ToString('x2') }) -join '')
    } finally { $sha.Dispose() }
}

try {
    if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
        Write-ResultJson @{ domains = @(); changed_count = 0; already_reviewed = $false; review_file = $null; hash = $null }
        exit 0
    }

    $files = Get-ChangedFiles
    if ($files.Count -eq 0) {
        Write-ResultJson @{ domains = @(); changed_count = 0; already_reviewed = $false; review_file = $null; hash = $null }
        exit 0
    }

    $chRaw = & (Join-Path $ScriptDir 'choreograph-agents.ps1') -ChangedFiles $files -Trigger 'local' -Json
    $ch = $chRaw | ConvertFrom-Json
    $domains = @($ch.domain_consults)

    if ($domains.Count -eq 0) {
        Write-ResultJson @{ domains = @(); changed_count = $files.Count; already_reviewed = $false; review_file = $null; hash = $null }
        exit 0
    }

    $hashInput = (($domains | Sort-Object) -join ',') + '|' + (($files | Sort-Object) -join ',')
    $hash = Get-StringHash -Text $hashInput

    if (-not $Force -and (Test-Path $StateFile)) {
        $prev = (Get-Content $StateFile -Raw -ErrorAction SilentlyContinue).Trim()
        if ($prev -eq $hash) {
            Write-ResultJson @{ domains = $domains; changed_count = $files.Count; already_reviewed = $true; review_file = $OutFile.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/'); hash = $hash }
            exit 0
        }
    }

    if (-not (Test-Path $CursorDir)) { New-Item -ItemType Directory -Path $CursorDir -Force | Out-Null }

    $timestamp = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    $sections = @()
    foreach ($d in $domains) {
        try {
            $body = & (Join-Path $ScriptDir 'post-domain-consult.ps1') -DomainId $d -ChangedFiles $files -Trigger 'local-autoreview' | Out-String
            $sections += $body.Trim()
        } catch {
            $sections += "## Parecer de domínio: $d`n_falha ao gerar: $($_.Exception.Message)_"
        }
    }

    $matched = (@($ch.matched_rules) -join ', ')
    $fileList = ($files | ForEach-Object { "- ``$_``" }) -join "`n"
    $header = @"
<!-- arah-domain-autoreview -->
# Revisão autônoma de domínio

**Quando:** $timestamp UTC | **Regras casadas:** $matched
**Agentes de domínio acionados:** $(( $domains | ForEach-Object { "``$_``" }) -join ', ')

## Arquivos revisados ($($files.Count))
$fileList

---
"@

    Set-Content -Path $OutFile -Value ($header + "`n" + ($sections -join "`n`n---`n`n")) -Encoding utf8
    Set-Content -Path $StateFile -Value $hash -Encoding utf8

    Write-ResultJson @{ domains = $domains; changed_count = $files.Count; already_reviewed = $false; review_file = $OutFile.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/'); hash = $hash }
    exit 0
}
catch {
    # Fail open: nunca bloquear o fluxo por erro do autoreview.
    Write-ResultJson @{ domains = @(); changed_count = 0; already_reviewed = $false; review_file = $null; hash = $null }
    exit 0
}
