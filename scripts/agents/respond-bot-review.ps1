#Requires -Version 5.1
<#
.SYNOPSIS
  Responde automaticamente a apontamentos de bots já endereçados no código.
.EXAMPLE
  ./respond-bot-review.ps1 -PrNumber 305
#>
param(
    [Parameter(Mandatory)]
    [int]$PrNumber,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ScriptDir = $PSScriptRoot

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

Push-Location $Root
try {
    $repo = gh repo view --json nameWithOwner -q .nameWithOwner
    $comments = gh api "repos/$repo/pulls/$PrNumber/comments" --paginate 2>$null | ConvertFrom-Json
    if (-not $comments) { $comments = @() }
    if ($comments -isnot [array]) { $comments = @($comments) }

    $replies = @()
    foreach ($c in $comments) {
        $author = [string]$c.user.login
        if ($author -notmatch '(?i)(codex|coderabbit|bugbot|cursor|\[bot\])') { continue }
        if ($c.body -match '(?i)(already resolved|fixed in|endereçado|resolvido)') { continue }

        $bodyLower = ([string]$c.body).ToLowerInvariant()
        $path = [string]$c.path
        $reply = $null

        if ($path -match 'CoreController\.cs' -and $bodyLower -match 'authoriz') {
            $reply = @'
<!-- arah-bot-response:pr-steward -->
Endereçado: endpoints Core protegidos com `[Authorize(Policy = "SystemAdmin")]`. Autenticação m2m de instâncias (heartbeat) entra na evolução FASE53 (mTLS/service token).
'@
        } elseif ($path -match 'CoreServiceCollectionExtensions\.cs' -and $bodyLower -match 'concurrent|singleton') {
            $reply = @'
<!-- arah-bot-response:pr-steward -->
Endereçado: registros in-memory do Core usam `ConcurrentDictionary` para acesso thread-safe em singletons.
'@
        } elseif ($path -match 'InMemoryCoreInstanceRegistry\.cs' -and $bodyLower -match 'heartbeat|telemetry|persist') {
            $reply = @'
<!-- arah-bot-response:pr-steward -->
Endereçado: heartbeat persiste `LastServices`, `LastUptime` e `LastHeartbeatUtc` via `CoreInstance.ApplyHeartbeat`.
'@
        }

        if (-not $reply) { continue }

        $already = @($comments | Where-Object {
            $_.in_reply_to_id -eq $c.id -and $_.body -match 'arah-bot-response:pr-steward'
        })
        if ($already.Count -gt 0) { continue }

        gh api -X POST "repos/$repo/pulls/$PrNumber/comments/$($c.id)/replies" -f body="$reply" | Out-Null
        $replies += [ordered]@{ comment_id = $c.id; path = $path; author = $author }
    }

    $result = [ordered]@{
        pr      = $PrNumber
        replies = $replies
        count   = $replies.Count
    }

    if ($Json) { $result | ConvertTo-Json -Depth 4 } else {
        Write-Host "Posted $($replies.Count) bot response(s)"
    }
    exit 0
} finally {
    Pop-Location
}
