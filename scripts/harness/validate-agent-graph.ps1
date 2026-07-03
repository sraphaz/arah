#Requires -Version 5.1
<#
.SYNOPSIS
  Valida a consistência do Agent Graph do Arah (coreografia + manifests + skills
  + specs), com erros para inconsistências críticas e warnings para o resto.
.DESCRIPTION
  Checagens:
    - toda skill referenciada em .agents/choreography.yaml existe em .skills/;
    - todo agente referenciado tem manifest em .agents/, .agents/domain/ ou
      .agents/specialists/;
    - toda rule tem >=1 path e >=1 agente;
    - rules críticas (identity-privacy, monetization, infra-deploy,
      core-control-plane, federation-handoff) têm gate/domain consult/agente
      operacional/skill coerente;
    - o artefato docs/_meta/agent-graph.generated.json está presente e coerente
      com o estado atual (warning se ausente/defasado).
  Sem dependências pesadas (parse regex, PS 5.1+). Idempotente e read-only.
.EXAMPLE
  ./validate-agent-graph.ps1
.EXAMPLE
  ./validate-agent-graph.ps1 -Json
.EXAMPLE
  ./validate-agent-graph.ps1 -Strict   # trata warnings como erros
#>
param(
    [switch]$Json,
    [switch]$Strict
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ChoreoPath = Join-Path $Root '.agents/choreography.yaml'
$AgentsDir = Join-Path $Root '.agents'
$SkillsDir = Join-Path $Root '.skills'
$GraphJson = Join-Path $Root 'docs/_meta/agent-graph.generated.json'

$CriticalRules = @('identity-privacy', 'monetization', 'infra-deploy', 'core-control-plane', 'federation-handoff')

$errors = @()
$warnings = @()

function Parse-ChoreographyRules {
    param([string]$Raw)
    $rules = @()
    $blocks = [regex]::Split($Raw, '(?m)^  - id: ')
    foreach ($block in $blocks) {
        # Ignora o preâmbulo/comentários; rule ids são lowercase-dashed seguidos de newline.
        if ($block -notmatch '^([a-z][\w-]*)\r?\n') { continue }
        $ruleId = $Matches[1].Trim()
        $paths = @()
        # (?m) sem Singleline: '.' não cruza linha, então a lista de paths para
        # na próxima chave irmã (agents:) e não engole entradas de agente.
        if ($block -match '(?m)^    paths:\s*\r?\n((?:      - [^\r\n]+\r?\n?)+)') {
            $paths = [regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') | ForEach-Object {
                $_.Groups[1].Value.Trim().Trim('"').Trim("'")
            }
        }
        $agents = @()
        if ($block -match '(?ms)^    agents:\s*\n((?:      - .+\r?\n?)+)') {
            $agentChunks = [regex]::Split($Matches[1], '(?m)^      - id: ')
            foreach ($chunk in $agentChunks) {
                if ($chunk -notmatch '^(\S+)') { continue }
                $aid = $Matches[1].Trim()
                $type = if ($chunk -match '(?m)^        type:\s+(\S+)') { $Matches[1] } else { 'operational' }
                $skills = @()
                if ($chunk -match '(?m)^        skills:\s*\[(.+)\]') {
                    $skills = $Matches[1] -split ',' | ForEach-Object { $_.Trim() }
                } elseif ($chunk -match '(?ms)^        skills:\s*\n((?:          - .+\r?\n?)+)') {
                    $skills = [regex]::Matches($Matches[1], '^\s+-\s+(\S+)', 'Multiline') | ForEach-Object { $_.Groups[1].Value }
                }
                $agents += @{ id = $aid; kind = $type; skills = @($skills) }
            }
        }
        $rules += @{ id = $ruleId; paths = @($paths); agents = @($agents) }
    }
    return $rules
}

function Test-AgentManifestExists {
    param([string]$AgentId)
    $candidates = @(
        (Join-Path $AgentsDir "$AgentId.agent.yaml"),
        (Join-Path $AgentsDir "domain/$AgentId.agent.yaml"),
        (Join-Path $AgentsDir "specialists/$AgentId.agent.yaml")
    )
    foreach ($c in $candidates) { if (Test-Path $c) { return $true } }
    return $false
}

# --- Fontes ------------------------------------------------------------------
if (-not (Test-Path $ChoreoPath)) {
    Write-Error "Missing .agents/choreography.yaml"
    exit 1
}
$rules = Parse-ChoreographyRules -Raw (Get-Content $ChoreoPath -Raw)

$skillIds = @(Get-ChildItem -Path $SkillsDir -Filter '*.skill.yaml' |
    ForEach-Object { $_.BaseName -replace '\.skill$', '' } | Select-Object -Unique)

# --- Checagens por rule ------------------------------------------------------
foreach ($rule in $rules) {
    if ($rule.paths.Count -eq 0) {
        $errors += "rule '$($rule.id)': has no paths"
    }
    if ($rule.agents.Count -eq 0) {
        $errors += "rule '$($rule.id)': has no agents"
    }

    foreach ($a in $rule.agents) {
        if (-not (Test-AgentManifestExists -AgentId $a.id)) {
            $errors += "rule '$($rule.id)': agent '$($a.id)' has no manifest in .agents/[domain|specialists]/"
        }
        foreach ($sk in $a.skills) {
            if ($skillIds -notcontains $sk) {
                $errors += "rule '$($rule.id)': skill '$sk' (agent '$($a.id)') not found in .skills/"
            }
        }
    }

    if ($CriticalRules -contains $rule.id) {
        $hasDomain = @($rule.agents | Where-Object { $_.kind -eq 'domain' }).Count -gt 0
        $hasOperational = @($rule.agents | Where-Object { $_.kind -ne 'domain' }).Count -gt 0
        $hasSkill = @($rule.agents | Where-Object { $_.skills.Count -gt 0 }).Count -gt 0
        if (-not ($hasDomain -or $hasOperational -or $hasSkill)) {
            $errors += "critical rule '$($rule.id)': needs a domain consult, operational agent or coherent skill"
        }
    }
}

# --- Rules críticas presentes ------------------------------------------------
$ruleIds = @($rules | ForEach-Object { $_.id })
foreach ($cr in $CriticalRules) {
    if ($ruleIds -notcontains $cr) {
        $warnings += "critical rule '$cr' not present in choreography (expected a gate for it)"
    }
}

# --- Consistência do artefato gerado -----------------------------------------
# Compara o conteúdo INTEIRO do grafo (nós + arestas), não só contagens: regenera
# em memória e confronta com o commitado, ignorando o campo volátil generated_at.
function Get-GraphFingerprint {
    param($Graph)
    if ($null -eq $Graph) { return $null }
    $Graph.generated_at = ''
    return ($Graph | ConvertTo-Json -Depth 20 -Compress)
}

$exportScript = Join-Path $Root 'scripts/agents/export-agent-graph.ps1'
if (-not (Test-Path $GraphJson)) {
    $warnings += "artefato ausente: docs/_meta/agent-graph.generated.json (rode scripts/agents/export-agent-graph.ps1)"
} elseif (-not (Test-Path $exportScript)) {
    $warnings += "export script ausente: scripts/agents/export-agent-graph.ps1 — não foi possível checar defasagem"
} else {
    try {
        $committed = Get-Content $GraphJson -Raw | ConvertFrom-Json
        $freshRaw = & $exportScript -Json
        $fresh = ($freshRaw -join "`n") | ConvertFrom-Json
        $committedFp = Get-GraphFingerprint -Graph $committed
        $freshFp = Get-GraphFingerprint -Graph $fresh
        if ($committedFp -ne $freshFp) {
            $warnings += "artefato defasado: docs/_meta/agent-graph.generated.json difere do grafo regenerado (nós/arestas mudaram) — rode scripts/agents/export-agent-graph.ps1"
        }
    } catch {
        $errors += "artefato inválido: falha ao ler/regenerar o grafo — $($_.Exception.Message)"
    }
}

# --- Resultado ---------------------------------------------------------------
$ok = ($errors.Count -eq 0) -and (-not ($Strict -and $warnings.Count -gt 0))

if ($Json) {
    [ordered]@{
        ok       = $ok
        rules    = $rules.Count
        skills   = $skillIds.Count
        errors   = @($errors)
        warnings = @($warnings)
    } | ConvertTo-Json -Depth 6
} else {
    Write-Host "Agent graph validation — rules=$($rules.Count) skills=$($skillIds.Count)"
    foreach ($w in $warnings) { Write-Warning $w }
    if ($errors.Count -gt 0) {
        foreach ($e in $errors) { Write-Error $e -ErrorAction Continue }
    } else {
        Write-Host "OK — no critical inconsistencies."
    }
}

if (-not $ok) { exit 1 }
exit 0
