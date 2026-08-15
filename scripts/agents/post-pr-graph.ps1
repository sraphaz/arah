#Requires -Version 5.1
<#
.SYNOPSIS
  Publica comentário <!-- arah-pr-graph --> com agentes/skills/domínios do roteamento do PR.
.EXAMPLE
  ./post-pr-graph.ps1 -PrNumber 469 -RouteFile route.json -ChangedFiles @('backend/...') -PostComment
#>
param(
    [Parameter(Mandatory)]
    [int]$PrNumber,
    [string]$RouteFile = 'route.json',
    [string[]]$ChangedFiles = @(),
    [switch]$PostComment,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

function Get-StringList {
    param($Value)
    if ($null -eq $Value) { return @() }
    return @($Value | ForEach-Object { "$_" } | Where-Object { $_ -and $_.Trim() })
}

Push-Location $Root
try {
    $route = $null
    if (Test-Path $RouteFile) {
        $raw = Get-Content $RouteFile -Raw
        try { $route = $raw | ConvertFrom-Json } catch { $route = $null }
    }

    $choreo = $null
    if ($route -and $route.choreography) { $choreo = $route.choreography }

    $primary = if ($route -and $route.agent) { [string]$route.agent }
               elseif ($route -and $route.primary_agent) { [string]$route.primary_agent }
               else { '_não resolvido_' }

    $agents = @()
    if ($route) {
        $agents += (Get-StringList $route.co_agents)
        foreach ($key in @('agents', 'activated', 'secondary_agents', 'operational')) {
            if ($route.PSObject.Properties.Name -contains $key) {
                $agents += (Get-StringList $route.$key)
            }
        }
        if ($route.path_agents) {
            foreach ($pa in @($route.path_agents)) {
                if ($pa.agent) { $agents += [string]$pa.agent }
            }
        }
    }
    if ($choreo) {
        $agents += (Get-StringList $choreo.operational)
    }
    $agents = @($agents | Where-Object { $_ -and $_ -ne 'orchestrator' } | Select-Object -Unique)
    if ($agents.Count -eq 0 -and $primary -ne '_não resolvido_') { $agents = @($primary) }

    $skills = @()
    if ($route -and $route.skills) { $skills = Get-StringList $route.skills }
    elseif ($route -and $route.suggested_skills) { $skills = Get-StringList $route.suggested_skills }

    $domains = @()
    if ($choreo -and $choreo.domain_consults) { $domains = Get-StringList $choreo.domain_consults }
    elseif ($route -and $route.domain_agents) { $domains = Get-StringList $route.domain_agents }
    elseif ($route -and $route.domains) { $domains = Get-StringList $route.domains }

    $matchedRules = @()
    if ($choreo -and $choreo.matched_rules) { $matchedRules = Get-StringList $choreo.matched_rules }

    # Heurística de domínio pelos paths se route não trouxe
    if ($domains.Count -eq 0 -and $ChangedFiles.Count -gt 0) {
        $joined = ($ChangedFiles -join ' ').ToLowerInvariant()
        if ($joined -match 'marketplace|store|cart|checkout|item') { $domains += 'mercado-economia' }
        if ($joined -match 'feesplit|subscription|payout|wallet|financial|merchant') {
            $domains += 'monetizacao-split'
            $domains += 'carteira-arata'
        }
        if ($joined -match 'intelligence|world-monitor|ti0|ti-') { $domains += 'signal-scout' }
        if ($joined -match 'territor') { $domains += 'territorio-membership' }
        $domains = @($domains | Select-Object -Unique)
    }

    $filesPreview = if ($ChangedFiles.Count -gt 0) {
        ($ChangedFiles | Select-Object -First 12 | ForEach-Object { "- ``$_``" }) -join "`n"
    } else { '_paths não informados_' }

    $agentsList = if ($agents.Count -gt 0) { ($agents | ForEach-Object { "- ``$_``" }) -join "`n" } else { '- _(nenhum)_' }
    $skillsList = if ($skills.Count -gt 0) { ($skills | ForEach-Object { "- ``$_``" }) -join "`n" } else { '- _(sugerir via orquestrador)_' }
    $domainsList = if ($domains.Count -gt 0) {
        ($domains | ForEach-Object { "- ``$_`` — parecer consultivo (não executa checklist sozinho)" }) -join "`n"
    } else { '- _(nenhum domínio mapeado)_' }
    $rulesList = if ($matchedRules.Count -gt 0) {
        ($matchedRules | ForEach-Object { "- ``$_``" }) -join "`n"
    } else { '- _(nenhuma)_' }

    $marker = '<!-- arah-pr-graph -->'
    $body = @"
$marker
## Agent Graph (PR)

**Agente principal:** ``$primary``

### Agentes no roteamento
$agentsList

### Skills sugeridas
$skillsList

### Domínios a consumir (pareceres)
$domainsList

### Regras de coreografia
$rulesList

### Paths (amostra)
$filesPreview

### Como usar
1. Ler cada ``arah-domain-consult:*`` aplicável ao diff.
2. Cumprir / justificar itens de **Validar no PR**.
3. Preencher no corpo do PR a seção **Pareceres endereçados**.
4. Grafo estático do repo: ``./scripts/agents/arah-agents.ps1 export-graph`` → ``docs/_meta/agent-graph.generated.json``
5. Diagnóstico do fluxo: [AGENT_PR_FLOW_INTEGRITY.md](https://github.com/sraphaz/arah/blob/main/docs/ops/AGENT_PR_FLOW_INTEGRITY.md)

---
_Automático via ``post-pr-graph.ps1`` — descritivo; não executa agentes._
"@

    $result = [ordered]@{
        pr             = $PrNumber
        primary        = $primary
        agents         = @($agents)
        skills         = @($skills)
        domains        = @($domains)
        matched_rules  = @($matchedRules)
        posted         = $false
    }

    if ($PostComment) {
        if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
            Write-Error 'gh CLI required for -PostComment'
            exit 1
        }
        $repo = gh repo view --json nameWithOwner -q .nameWithOwner
        $comments = gh api "repos/$repo/issues/$PrNumber/comments" --paginate 2>$null | ConvertFrom-Json
        if (-not $comments) { $comments = @() }
        if ($comments -isnot [array]) { $comments = @($comments) }
        $existing = $comments | Where-Object { $_.body -and $_.body.Contains($marker) } | Select-Object -First 1
        $tmp = Join-Path ([System.IO.Path]::GetTempPath()) "arah-pr-graph-$PrNumber.md"
        # UTF8 sem BOM para corpo de comentário GitHub
        [System.IO.File]::WriteAllText($tmp, $body)
        if ($existing) {
            gh api -X PATCH "repos/$repo/issues/comments/$($existing.id)" -F body="$(Get-Content $tmp -Raw)" | Out-Null
        } else {
            gh pr comment $PrNumber --body-file $tmp | Out-Null
        }
        $result.posted = $true
    }

    if ($Json) { $result | ConvertTo-Json -Depth 4 }
    else {
        Write-Host $body
        if ($result.posted) { Write-Host "Posted/updated arah-pr-graph on PR #$PrNumber" }
    }
    exit 0
} finally {
    Pop-Location
}
