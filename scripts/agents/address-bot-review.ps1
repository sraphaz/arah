#Requires -Version 5.1
<#
.SYNOPSIS
  Audita apontamentos de bots em um PR e lista threads não resolvidas.
.EXAMPLE
  ./address-bot-review.ps1 -PrNumber 297
#>
param(
    [Parameter(Mandatory)]
    [int]$PrNumber,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error 'gh CLI required'
    exit 1
}

Push-Location $Root
try {
    $repo = gh repo view --json nameWithOwner -q .nameWithOwner
    $pr = gh pr view $PrNumber --json title,state,headRefName,baseRefName,statusCheckRollup | ConvertFrom-Json

    $botPattern = '(?i)(coderabbit|dependabot|github-actions|cursor|bugbot|codeql|codex|trivy|\[bot\])'
    $ignoreBodyPattern = '(?i)(review limit reached|rate limited|about codex in github|thanks for using \[coderabbit\])'

    $reviewComments = gh api "repos/$repo/pulls/$PrNumber/comments" --paginate 2>$null | ConvertFrom-Json
    if (-not $reviewComments) { $reviewComments = @() }
    if ($reviewComments -isnot [array]) { $reviewComments = @($reviewComments) }

    $issueComments = gh api "repos/$repo/issues/$PrNumber/comments" --paginate 2>$null | ConvertFrom-Json
    if (-not $issueComments) { $issueComments = @() }
    if ($issueComments -isnot [array]) { $issueComments = @($issueComments) }

    $reviews = gh api "repos/$repo/pulls/$PrNumber/reviews" --paginate 2>$null | ConvertFrom-Json
    if (-not $reviews) { $reviews = @() }
    if ($reviews -isnot [array]) { $reviews = @($reviews) }

    $botItems = @()
    foreach ($c in $reviewComments) {
        $author = [string]$c.user.login
        if ($author -notmatch $botPattern) { continue }
        if ($c.body -and $c.body -match $ignoreBodyPattern) { continue }
        $botItems += [ordered]@{
            kind   = 'review_comment'
            author = $author
            path   = $c.path
            line   = $c.line
            body   = $(if ($c.body -and $c.body.Length -gt 0) { ($c.body -replace '\s+', ' ').Substring(0, [Math]::Min(200, $c.body.Length)) } else { '' })
            url    = $c.html_url
        }
    }
    foreach ($c in $issueComments) {
        $author = [string]$c.user.login
        if ($author -notmatch $botPattern) { continue }
        if ($c.body -and $c.body -match $ignoreBodyPattern) { continue }
        $botItems += [ordered]@{
            kind   = 'issue_comment'
            author = $author
            path   = $null
            line   = $null
            body   = $(if ($c.body -and $c.body.Length -gt 0) { ($c.body -replace '\s+', ' ').Substring(0, [Math]::Min(200, $c.body.Length)) } else { '' })
            url    = $c.html_url
        }
    }

    $failedChecks = @()
    if ($pr.statusCheckRollup) {
        foreach ($check in $pr.statusCheckRollup) {
            if ($check.conclusion -and $check.conclusion -notin @('SUCCESS', 'NEUTRAL', 'SKIPPED')) {
                $failedChecks += $check.name
            } elseif ($check.state -and $check.state -notin @('SUCCESS', 'SKIPPED')) {
                if ($check.state -eq 'FAILURE' -or $check.state -eq 'ERROR') {
                    $failedChecks += $check.context
                }
            }
        }
    }
    if ($failedChecks.Count -eq 0) {
        try {
            $checksOut = gh pr checks $PrNumber 2>&1
            if ($LASTEXITCODE -ne 0 -and $checksOut) {
                foreach ($line in ($checksOut -split "`n")) {
                    if ($line -match '\sfail\s*$') {
                        $name = ($line -split '\s+')[0]
                        if ($name) { $failedChecks += $name }
                    }
                }
            }
        } catch {
            # gh pr checks unavailable — rely on statusCheckRollup only
        }
    }

    $ciReady = ($failedChecks.Count -eq 0)
    $botsPending = ($botItems.Count -gt 0)

    $result = [ordered]@{
        pr            = $PrNumber
        title         = $pr.title
        state         = $pr.state
        bot_comments  = $botItems.Count
        bot_items     = $botItems
        failed_checks = $failedChecks
        ci_ready      = $ciReady
        bots_pending  = $botsPending
        ready         = ($ciReady -and -not $botsPending)
        message       = if (-not $ciReady) {
            "CI/checks falhando: $($failedChecks -join ', ')"
        } elseif ($botsPending) {
            "$($botItems.Count) apontamento(s) de bot — revisar e resolver/responder antes do merge"
        } else {
            'CI OK e sem apontamentos de bot pendentes; validar reviews humanas.'
        }
    }

    if ($Json) {
        $result | ConvertTo-Json -Depth 6
    } else {
        $result | ConvertTo-Json -Depth 6
        Write-Host $result.message
    }

    if ($failedChecks.Count -gt 0) { exit 2 }
    if ($botsPending) { exit 3 }
    exit 0
} finally {
    Pop-Location
}
