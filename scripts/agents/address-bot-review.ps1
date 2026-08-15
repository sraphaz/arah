#Requires -Version 5.1
<#
.SYNOPSIS
  Audita apontamentos de bots em um PR (só o que bloqueia merge).

  Conta como pendente:
  - Threads inline NÃO resolvidas de bots de review (CodeRabbit, Bugbot, Codex, etc.)
  - Checks CI falhando

  NÃO conta (ruído de sinalização Arah):
  - Comentários arah-domain-consult / arah-agent-activity / arah-orchestrator
  - Templates QA/Security/Steward (arah-qa-gate, arah-security-gate, arah-pr-steward)
  - arah-pr-graph / arah-bot-response / respostas de consumo (Pareceres endereçados)
  - Autores github-actions em comentários de issue (publicação passiva)

.EXAMPLE
  ./address-bot-review.ps1 -PrNumber 297 -Json
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

function Test-ArahSignalComment {
    param([string]$Body)
    if ([string]::IsNullOrWhiteSpace($Body)) { return $false }
    return [bool]($Body -match '(?i)(<!--\s*arah-(domain-consult|agent-activity|orchestrator|qa-gate|security-gate|pr-steward|pr-graph|bot-response|agent-reply))') `
        -or [bool]($Body -match '(?i)(##\s*PR Steward|##\s*QA Agent — Checklist|##\s*Security Agent — Relatório|##\s*Resposta aos bots|##\s*Pareceres endereçados|##\s*Agent Graph \(PR\))')
}

function Test-ReviewBotAuthor {
    param([string]$Author)
    # Bots de review inline — não incluir github-actions (sinalização Arah)
    return [bool]($Author -match '(?i)(coderabbit|bugbot|chatgpt-codex|codex|cursor|trivy|codeql|dependabot)')
}

Push-Location $Root
try {
    $repo = gh repo view --json nameWithOwner -q .nameWithOwner
    $owner, $name = $repo -split '/'
    $pr = gh pr view $PrNumber --json title,state,headRefName,baseRefName,statusCheckRollup | ConvertFrom-Json

    $botItems = @()
    $ignoredSignal = 0

    # --- Threads inline não resolvidas (fonte autoritativa) ---
    $gq = @'
query($owner:String!,$name:String!,$n:Int!){
  repository(owner:$owner,name:$name){
    pullRequest(number:$n){
      reviewThreads(first:100){
        nodes{
          isResolved
          comments(first:1){
            nodes{ author{ login } body path url: url }
          }
        }
      }
    }
  }
}
'@
    $threadsJson = gh api graphql -f query=$gq -F owner=$owner -F name=$name -F n=$PrNumber 2>$null
    if ($threadsJson) {
        $threadsData = $threadsJson | ConvertFrom-Json
        $nodes = @($threadsData.data.repository.pullRequest.reviewThreads.nodes)
        foreach ($t in $nodes) {
            if ($t.isResolved) { continue }
            $c0 = $t.comments.nodes | Select-Object -First 1
            if (-not $c0) { continue }
            $author = [string]$c0.author.login
            if (-not (Test-ReviewBotAuthor -Author $author)) { continue }
            $body = [string]$c0.body
            if (Test-ArahSignalComment -Body $body) { $ignoredSignal++; continue }
            if ($body -match '(?i)(already resolved|fixed in|endereçado|resolvido|arah-bot-response)') { continue }
            $botItems += [ordered]@{
                kind   = 'unresolved_thread'
                author = $author
                path   = $c0.path
                line   = $null
                body   = $(if ($body.Length -gt 0) { ($body -replace '\s+', ' ').Substring(0, [Math]::Min(200, $body.Length)) } else { '' })
                url    = $c0.url
            }
        }
    }

    # Fallback: review comments da API REST se GraphQL falhar / vazio e ainda houver inline
    if ($botItems.Count -eq 0 -and -not $threadsJson) {
        $reviewComments = gh api "repos/$repo/pulls/$PrNumber/comments" --paginate 2>$null | ConvertFrom-Json
        if (-not $reviewComments) { $reviewComments = @() }
        if ($reviewComments -isnot [array]) { $reviewComments = @($reviewComments) }
        foreach ($c in $reviewComments) {
            $author = [string]$c.user.login
            if (-not (Test-ReviewBotAuthor -Author $author)) { continue }
            if (Test-ArahSignalComment -Body ([string]$c.body)) { $ignoredSignal++; continue }
            $botItems += [ordered]@{
                kind   = 'review_comment'
                author = $author
                path   = $c.path
                line   = $c.line
                body   = $(if ($c.body -and $c.body.Length -gt 0) { ($c.body -replace '\s+', ' ').Substring(0, [Math]::Min(200, $c.body.Length)) } else { '' })
                url    = $c.html_url
            }
        }
    }

    # Issue comments: só Dependabot/CodeRabbit com alerta acionável — nunca github-actions Arah
    $issueComments = gh api "repos/$repo/issues/$PrNumber/comments" --paginate 2>$null | ConvertFrom-Json
    if (-not $issueComments) { $issueComments = @() }
    if ($issueComments -isnot [array]) { $issueComments = @($issueComments) }
    foreach ($c in $issueComments) {
        $author = [string]$c.user.login
        $body = [string]$c.body
        if (Test-ArahSignalComment -Body $body) { $ignoredSignal++; continue }
        if ($author -match '(?i)^github-actions') { $ignoredSignal++; continue }
        # Resumos CodeRabbit "Review skipped" / draft — não bloqueiam
        if ($body -match '(?i)(review skipped|draft detected|summary by coderabbit)') { continue }
        if ($author -match '(?i)(dependabot)' -and $body -match '(?i)(security|vulnerabilit|CVE)') {
            $botItems += [ordered]@{
                kind   = 'issue_comment'
                author = $author
                path   = $null
                line   = $null
                body   = $(if ($body.Length -gt 0) { ($body -replace '\s+', ' ').Substring(0, [Math]::Min(200, $body.Length)) } else { '' })
                url    = $c.html_url
            }
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
        } catch { }
    }

    $ciReady = ($failedChecks.Count -eq 0)
    $botsPending = ($botItems.Count -gt 0)

    $result = [ordered]@{
        pr                 = $PrNumber
        title              = $pr.title
        state              = $pr.state
        bot_comments       = $botItems.Count
        bot_items          = $botItems
        ignored_signal     = $ignoredSignal
        failed_checks      = $failedChecks
        ci_ready           = $ciReady
        bots_pending       = $botsPending
        ready              = ($ciReady -and -not $botsPending)
        message            = if (-not $ciReady) {
            "CI/checks falhando: $($failedChecks -join ', ')"
        } elseif ($botsPending) {
            "$($botItems.Count) thread(s)/alerta(s) de bot de review — resolver/responder antes do merge (ignorados $ignoredSignal sinalizações Arah)"
        } else {
            "CI OK; sem threads de review pendentes (ignoradas $ignoredSignal sinalizações Arah). Validar reviews humanas."
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
