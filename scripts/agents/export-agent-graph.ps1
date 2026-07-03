#Requires -Version 5.1
<#
.SYNOPSIS
  Exporta o Agent Graph do Arah: formaliza as relações já existentes entre
  agentes, skills, rules de coreografia, paths, domínios, specs, harnesses,
  guardrails e workflows num único artefato auditável (JSON estável).
.DESCRIPTION
  Fonte inicial de verdade: .agents/choreography.yaml, cruzada com
  .agents/**/*.agent.yaml, .skills/*.skill.yaml, docs/specs/**/*.spec.yaml e
  .github/workflows/*.yml. Sem dependências pesadas (parse regex, PS 5.1+).
  Saída ordenada/estável para diffs limpos e idempotência.
.EXAMPLE
  ./export-agent-graph.ps1
.EXAMPLE
  ./export-agent-graph.ps1 -Json          # imprime JSON no stdout, não grava arquivo
.EXAMPLE
  ./export-agent-graph.ps1 -Mermaid       # imprime diagrama Mermaid no stdout
#>
param(
    [string]$OutFile = '',
    [string]$MermaidOut = '',
    [switch]$Json,
    [switch]$Mermaid
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ChoreoPath = Join-Path $Root '.agents/choreography.yaml'
$AgentsDir = Join-Path $Root '.agents'
$SkillsDir = Join-Path $Root '.skills'
$SpecsDir = Join-Path $Root 'docs/specs'
$WorkflowsDir = Join-Path $Root '.github/workflows'
if (-not $OutFile) { $OutFile = Join-Path $Root 'docs/_meta/agent-graph.generated.json' }
if (-not $MermaidOut) { $MermaidOut = Join-Path $Root 'docs/_meta/agent-graph.generated.mmd' }

. (Join-Path $PSScriptRoot 'choreography-parser.ps1')
. (Join-Path $PSScriptRoot 'yaml-lite.ps1')

function Get-RelPath {
    param([string]$FullPath)
    return $FullPath.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/')
}

function Get-ScopePaths {
    param([string]$Raw)
    $scope = Get-TopLevelBlock -Raw $Raw -Key 'scope'
    if (-not $scope) { return @() }
    return Get-ListUnderKey -Raw $scope -Key 'paths' -Indent 2
}

# Extrai os acceptance criteria de uma spec: id, status, covered_by (filtro de
# teste) e evidence. Mesmo padrão de validate-specs.ps1 (sem Singleline).
function Get-SpecAcceptance {
    param([string]$Raw)
    $items = @()
    if ($Raw -notmatch '(?m)^acceptance:[ \t]*\r?\n((?:[ \t]+\S.*\r?\n?)+)') { return $items }
    $block = $Matches[1]
    foreach ($chunk in [regex]::Split($block, '(?m)^  - id:\s*')) {
        if ($chunk -notmatch '^(\S+)') { continue }
        $acId = $Matches[1].Trim()
        $status = if ($chunk -match '(?m)^    status:\s*(\S+)') { $Matches[1].Trim() } else { $null }
        $coveredBy = if ($chunk -match '(?m)^    covered_by:\s*(.+)$') { $Matches[1].Trim().Trim('"').Trim("'") } else { $null }
        $evidence = if ($chunk -match '(?m)^    evidence:\s*(.+)$') { $Matches[1].Trim().Trim('"').Trim("'") } else { $null }
        $items += [ordered]@{ id = $acId; status = $status; covered_by = $coveredBy; evidence = $evidence }
    }
    return $items
}

# --- 1) Agents ---------------------------------------------------------------
$agents = @()
Get-ChildItem -Path $AgentsDir -Recurse -Filter '*.agent.yaml' | Sort-Object FullName | ForEach-Object {
    $raw = Get-Content $_.FullName -Raw
    $id = Get-ScalarField -Raw $raw -Field 'id'
    if (-not $id) { return }
    $rel = Get-RelPath $_.FullName
    $kind = if ($rel -match '/domain/') { 'domain' } elseif ($rel -match '/specialists/') { 'specialist' } else { 'operational' }

    $skills = Get-ListUnderKey -Raw $raw -Key 'skills' -Indent 0
    $consultDomain = Get-ListUnderKey -Raw $raw -Key 'domain' -Indent 2
    $consultSpec = Get-ListUnderKey -Raw $raw -Key 'specialists' -Indent 2
    $paths = Get-ScopePaths -Raw $raw

    $guardrails = [ordered]@{}
    $grBlock = Get-TopLevelBlock -Raw $raw -Key 'guardrails'
    if ($grBlock) {
        foreach ($m in [regex]::Matches($grBlock, '^\s+(\w+):\s+(\S+)', 'Multiline')) {
            $guardrails[$m.Groups[1].Value] = $m.Groups[2].Value
        }
    }

    $agents += [ordered]@{
        id         = $id
        name       = (Get-ScalarField -Raw $raw -Field 'name')
        kind       = $kind
        manifest   = $rel
        skills     = @($skills)
        consults   = [ordered]@{ domain = @($consultDomain); specialists = @($consultSpec) }
        paths      = @($paths)
        guardrails = $guardrails
    }
}

# --- 2) Skills ---------------------------------------------------------------
$skills = @()
Get-ChildItem -Path $SkillsDir -Filter '*.skill.yaml' | Sort-Object Name -Unique | ForEach-Object {
    $raw = Get-Content $_.FullName -Raw
    $id = Get-ScalarField -Raw $raw -Field 'id'
    if (-not $id) { return }
    $skills += [ordered]@{
        id          = $id
        description = (Get-ScalarField -Raw $raw -Field 'description')
        manifest    = (Get-RelPath $_.FullName)
    }
}
$skills = @($skills | Sort-Object { $_.id } -Unique)
$skillIds = @($skills | ForEach-Object { $_.id })

# --- 3) Rules (coreografia) --------------------------------------------------
$rules = @()
if (Test-Path $ChoreoPath) {
    $rules = Parse-ChoreographyRules -Raw (Get-Content $ChoreoPath -Raw)
}

# --- 4) PathPatterns (rules + scope.paths dos manifests) ---------------------
$pathMap = @{}
foreach ($rule in $rules) {
    foreach ($p in $rule.paths) {
        if (-not $pathMap.ContainsKey($p)) { $pathMap[$p] = @{ rules = @(); agents = @() } }
        if ($pathMap[$p].rules -notcontains $rule.id) { $pathMap[$p].rules += $rule.id }
    }
}
foreach ($a in $agents) {
    foreach ($p in $a.paths) {
        if (-not $pathMap.ContainsKey($p)) { $pathMap[$p] = @{ rules = @(); agents = @() } }
        if ($pathMap[$p].agents -notcontains $a.id) { $pathMap[$p].agents += $a.id }
    }
}
$paths = @($pathMap.GetEnumerator() | Sort-Object Name | ForEach-Object {
    [ordered]@{ pattern = $_.Key; rules = @($_.Value.rules); agents = @($_.Value.agents) }
})

# --- 5) Domains (agentes de domínio + rules que os acionam) ------------------
$domainMap = @{}
foreach ($rule in $rules) {
    foreach ($a in $rule.agents) {
        if ($a.kind -eq 'domain') {
            if (-not $domainMap.ContainsKey($a.id)) { $domainMap[$a.id] = @() }
            if ($domainMap[$a.id] -notcontains $rule.id) { $domainMap[$a.id] += $rule.id }
        }
    }
}
$domains = @($domainMap.GetEnumerator() | Sort-Object Name | ForEach-Object {
    [ordered]@{ id = $_.Key; agent = $_.Key; rules = @($_.Value) }
})

# --- 6) Specs + Harnesses ----------------------------------------------------
$specs = @()
$harnesses = @()
if (Test-Path $SpecsDir) {
    Get-ChildItem -Path $SpecsDir -Recurse -Filter '*.spec.yaml' |
        Where-Object { $_.Name -ne '_template.spec.yaml' } |
        Sort-Object FullName -Unique |
        ForEach-Object {
            $raw = Get-Content $_.FullName -Raw
            $id = Get-ScalarField -Raw $raw -Field 'id'
            if (-not $id) { return }
            $status = Get-ScalarField -Raw $raw -Field 'status'
            $harnessBlock = Get-TopLevelBlock -Raw $raw -Key 'harness'
            $harnessAgents = @()
            if ($harnessBlock) {
                $harnessAgents = Get-ListUnderKey -Raw $harnessBlock -Key 'agents' -Indent 2
            }
            $grs = Get-ListUnderKey -Raw $raw -Key 'guardrails' -Indent 0
            $acceptance = Get-SpecAcceptance -Raw $raw
            $rel = Get-RelPath $_.FullName
            $specs += [ordered]@{
                id             = $id
                status         = $status
                manifest       = $rel
                harness_agents = @($harnessAgents)
                guardrails     = @($grs)
                acceptance     = @($acceptance)
            }
            $harnesses += [ordered]@{ id = $id; spec = $id; agents = @($harnessAgents) }
        }
}
$specs = @($specs | Sort-Object { $_.id } -Unique)
$harnesses = @($harnesses | Sort-Object { $_.id } -Unique)

# --- 6b) Tests (filtros covered_by das acceptance criteria) ------------------
# Rastreabilidade fina AC <-> teste: cada covered_by vira um nó de teste,
# referenciado pelas specs/ACs que o cobrem.
$testMap = @{}
foreach ($s in $specs) {
    foreach ($ac in $s.acceptance) {
        if ($ac.covered_by) {
            if (-not $testMap.ContainsKey($ac.covered_by)) { $testMap[$ac.covered_by] = @() }
            $ref = "$($s.id):$($ac.id)"
            if ($testMap[$ac.covered_by] -notcontains $ref) { $testMap[$ac.covered_by] += $ref }
        }
    }
}
$tests = @($testMap.GetEnumerator() | Sort-Object Name | ForEach-Object {
    [ordered]@{ id = $_.Key; covers = @($_.Value) }
})

# --- 7) Guardrails (declarados + executáveis no harness) ---------------------
# Guardrails com verificação executável em run-harness.ps1 (Test-Guardrail).
$executableGuardrails = @(
    'no-secrets-in-repo', 'production-gate-human', 'clean-architecture',
    'territory-data-stays-on-instance', 'no-merge-automatic', 'sync-docs-on-behavior-change'
)
$guardrailSet = @{}
foreach ($g in $executableGuardrails) { $guardrailSet[$g] = $true }
foreach ($a in $agents) { foreach ($k in $a.guardrails.Keys) { if (-not $guardrailSet.ContainsKey($k)) { $guardrailSet[$k] = $false } } }
foreach ($s in $specs) { foreach ($g in $s.guardrails) { if (-not $guardrailSet.ContainsKey($g)) { $guardrailSet[$g] = $false } } }
$guardrails = @($guardrailSet.GetEnumerator() | Sort-Object Name | ForEach-Object {
    [ordered]@{ id = $_.Key; executable = [bool]$_.Value }
})

# --- 8) Workflows (gates de CI relevantes) -----------------------------------
$workflowRoles = @{
    'agents-validate.yml'   = 'Valida manifests de agentes/skills'
    'spec-harness.yml'      = 'Valida specs + roda harness SDD'
    'agents-gates.yml'      = 'Gates qa/security/release'
    'agents-pr-steward.yml' = 'PR Steward: bots + ready-for-merge'
    'agents.yml'            = 'Orquestração + pareceres de domínio'
    'ci.yml'                = 'Build + testes'
}
$workflows = @()
if (Test-Path $WorkflowsDir) {
    Get-ChildItem -Path $WorkflowsDir -Filter '*.yml' | Sort-Object Name | ForEach-Object {
        $role = if ($workflowRoles.ContainsKey($_.Name)) { $workflowRoles[$_.Name] } else { $null }
        $workflows += [ordered]@{ id = $_.Name; path = (Get-RelPath $_.FullName); role = $role }
    }
}

# --- 9) ReviewGates ----------------------------------------------------------
$reviewGates = @(
    [ordered]@{ id = 'ci';           kind = 'automated'; description = 'CI verde obrigatório (ci.yml + agents-gates.yml)' },
    [ordered]@{ id = 'qa';           kind = 'automated'; description = 'QA ativado pela rule pr-always' },
    [ordered]@{ id = 'bot-review';   kind = 'bot';       description = 'Apontamentos de bots resolvidos (CodeRabbit, CodeQL, ...)' },
    [ordered]@{ id = 'pr-steward';   kind = 'automated'; description = 'Aplica ready-for-merge só com gates OK' },
    [ordered]@{ id = 'human-review'; kind = 'human';     description = 'Merge humano obrigatório (guardrail no_merge)' }
)

# --- Edges (relações) --------------------------------------------------------
$edges = @()
function Add-Edge {
    param([string]$From, [string]$To, [string]$Type, [string]$Via = $null)
    $e = [ordered]@{ from = $From; to = $To; type = $Type }
    if ($Via) { $e.via = $Via }
    $script:edges += $e
}

foreach ($rule in $rules) {
    foreach ($p in $rule.paths) { Add-Edge "path:$p" "rule:$($rule.id)" 'matches_rule' }
    foreach ($a in $rule.agents) {
        if ($a.kind -eq 'domain') {
            Add-Edge "rule:$($rule.id)" "agent:$($a.id)" 'consults_domain_agent'
        } else {
            Add-Edge "rule:$($rule.id)" "agent:$($a.id)" 'activates_agent'
        }
        foreach ($sk in $a.skills) {
            Add-Edge "rule:$($rule.id)" "skill:$sk" 'requires_skill' $rule.id
            Add-Edge "agent:$($a.id)" "skill:$sk" 'may_invoke_skill' $rule.id
        }
    }
}

# specs-sdd rule => requires_spec (paths SDD exigem spec válida)
$sddRule = $rules | Where-Object { $_.id -eq 'specs-sdd' } | Select-Object -First 1
if ($sddRule) {
    foreach ($s in $specs) { Add-Edge "rule:specs-sdd" "spec:$($s.id)" 'requires_spec' }
}

foreach ($a in $agents) {
    foreach ($p in $a.paths) { Add-Edge "path:$p" "agent:$($a.id)" 'scopes_agent' 'manifest' }
    foreach ($sk in $a.skills) {
        if ($skillIds -contains $sk) { Add-Edge "agent:$($a.id)" "skill:$sk" 'may_invoke_skill' 'manifest' }
    }
    foreach ($d in $a.consults.domain) { Add-Edge "agent:$($a.id)" "agent:$d" 'consults_domain_agent' 'manifest' }
    foreach ($g in $a.guardrails.Keys) {
        if ($a.guardrails[$g] -eq 'true') { Add-Edge "agent:$($a.id)" "guardrail:$g" 'blocked_by_guardrail' 'manifest' }
    }
}

foreach ($s in $specs) {
    Add-Edge "spec:$($s.id)" "harness:$($s.id)" 'requires_harness'
    foreach ($ha in $s.harness_agents) { Add-Edge "harness:$($s.id)" "agent:$ha" 'validated_by' }
    foreach ($g in $s.guardrails) { Add-Edge "spec:$($s.id)" "guardrail:$g" 'blocked_by_guardrail' }
    foreach ($ac in $s.acceptance) {
        if ($ac.covered_by) { Add-Edge "spec:$($s.id)" "test:$($ac.covered_by)" 'verified_by_test' $ac.id }
    }
}

# Guardrails executáveis são impostos pelo harness (spec-harness.yml)
foreach ($g in $guardrails) {
    if ($g.executable) { Add-Edge "guardrail:$($g.id)" "workflow:spec-harness.yml" 'enforced_by_workflow' }
}

# Cadeia de review gates terminando em revisão humana
Add-Edge "gate:ci" "gate:pr-steward" 'requires_human_review'
Add-Edge "gate:qa" "gate:pr-steward" 'requires_human_review'
Add-Edge "gate:bot-review" "gate:pr-steward" 'requires_human_review'
Add-Edge "gate:pr-steward" "gate:human-review" 'requires_human_review'

$edges = @($edges | Sort-Object { "$($_.type)|$($_.from)|$($_.to)" })

# --- Diagrama Mermaid (overview) ---------------------------------------------
# Visão focada e legível: coreografia (rules -> agentes operacionais/domínio) +
# cadeia de review gates. Paths/skills/specs/tests ficam só no JSON completo.
function ConvertTo-MermaidId {
    param([string]$Raw)
    return ($Raw -replace '[^A-Za-z0-9]', '_')
}

function Build-Mermaid {
    param($Rules, $Agents, $ReviewGates, $Edges)
    $sb = New-Object System.Text.StringBuilder
    # Arquivo .mmd puro (sem cercas markdown) — consumidores/Mermaid Live esperam
    # corpo 'flowchart' cru. Para embutir em .md, adicione as cercas no destino.
    [void]$sb.AppendLine('flowchart LR')
    [void]$sb.AppendLine('  %% Gerado por scripts/agents/export-agent-graph.ps1 — não editar à mão')

    $agentKind = @{}
    foreach ($a in $Agents) { $agentKind[$a.id] = $a.kind }

    # Rules
    [void]$sb.AppendLine('  subgraph CHOREOGRAPHY["Coreografia (paths -> rules)"]')
    foreach ($r in ($Rules | Sort-Object { $_.id })) {
        $rid = 'rule_' + (ConvertTo-MermaidId $r.id)
        [void]$sb.AppendLine("    $rid[""$($r.id)""]")
    }
    [void]$sb.AppendLine('  end')

    # Agentes usados nas rules
    $usedAgents = @{}
    foreach ($e in $Edges) {
        if ($e.type -in @('activates_agent', 'consults_domain_agent') -and $e.from -like 'rule:*') {
            $aid = $e.to -replace '^agent:', ''
            $usedAgents[$aid] = $true
        }
    }
    [void]$sb.AppendLine('  subgraph OPERATIONAL["Agentes operacionais"]')
    foreach ($aid in ($usedAgents.Keys | Where-Object { $agentKind[$_] -ne 'domain' } | Sort-Object)) {
        $nid = 'agent_' + (ConvertTo-MermaidId $aid)
        [void]$sb.AppendLine("    $nid([""$aid""])")
    }
    [void]$sb.AppendLine('  end')
    [void]$sb.AppendLine('  subgraph DOMAIN["Agentes de domínio (consultivos)"]')
    foreach ($aid in ($usedAgents.Keys | Where-Object { $agentKind[$_] -eq 'domain' } | Sort-Object)) {
        $nid = 'agent_' + (ConvertTo-MermaidId $aid)
        [void]$sb.AppendLine("    $nid{{""$aid""}}")
    }
    [void]$sb.AppendLine('  end')

    # Arestas rule -> agente
    foreach ($e in ($Edges | Where-Object { $_.from -like 'rule:*' -and $_.type -in @('activates_agent', 'consults_domain_agent') } | Sort-Object { "$($_.from)|$($_.to)" })) {
        $rid = 'rule_' + (ConvertTo-MermaidId ($e.from -replace '^rule:', ''))
        $nid = 'agent_' + (ConvertTo-MermaidId ($e.to -replace '^agent:', ''))
        if ($e.type -eq 'consults_domain_agent') {
            [void]$sb.AppendLine("  $rid -. consulta .-> $nid")
        } else {
            [void]$sb.AppendLine("  $rid --> $nid")
        }
    }

    # Cadeia de review gates
    [void]$sb.AppendLine('  subgraph REVIEW["Pipeline de revisão (termina em humano)"]')
    foreach ($g in $ReviewGates) {
        $gid = 'gate_' + (ConvertTo-MermaidId $g.id)
        $shape = if ($g.kind -eq 'human') { "[[""$($g.id)""]]" } else { "[""$($g.id)""]" }
        [void]$sb.AppendLine("    $gid$shape")
    }
    [void]$sb.AppendLine('  end')
    foreach ($e in ($Edges | Where-Object { $_.type -eq 'requires_human_review' })) {
        $from = 'gate_' + (ConvertTo-MermaidId ($e.from -replace '^gate:', ''))
        $to = 'gate_' + (ConvertTo-MermaidId ($e.to -replace '^gate:', ''))
        [void]$sb.AppendLine("  $from --> $to")
    }
    return $sb.ToString()
}

# --- Montagem do grafo -------------------------------------------------------
$graph = [ordered]@{
    schema     = '.agents/agent-graph.schema.yaml'
    version    = 1
    generator  = 'scripts/agents/export-agent-graph.ps1'
    generated_at = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    sources    = @(
        '.agents/choreography.yaml',
        '.agents/**/*.agent.yaml',
        '.skills/*.skill.yaml',
        'docs/specs/**/*.spec.yaml',
        '.github/workflows/*.yml'
    )
    stats      = [ordered]@{
        agents      = $agents.Count
        skills      = $skills.Count
        rules       = $rules.Count
        paths       = $paths.Count
        domains     = $domains.Count
        specs       = $specs.Count
        harnesses   = $harnesses.Count
        tests       = $tests.Count
        guardrails  = $guardrails.Count
        workflows   = $workflows.Count
        review_gates = $reviewGates.Count
        edges       = $edges.Count
    }
    nodes = [ordered]@{
        agents       = @($agents)
        skills       = @($skills)
        rules        = @($rules)
        paths        = @($paths)
        domains      = @($domains)
        specs        = @($specs)
        harnesses    = @($harnesses)
        tests        = @($tests)
        guardrails   = @($guardrails)
        workflows    = @($workflows)
        review_gates = @($reviewGates)
    }
    edges = @($edges)
}

$jsonText = $graph | ConvertTo-Json -Depth 12
$mermaidText = Build-Mermaid -Rules $rules -Agents $agents -ReviewGates $reviewGates -Edges $edges

if ($Mermaid) {
    $mermaidText
} elseif ($Json) {
    $jsonText
} else {
    $outDir = Split-Path $OutFile -Parent
    if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir -Force | Out-Null }
    [System.IO.File]::WriteAllText($OutFile, $jsonText + "`n", [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::WriteAllText($MermaidOut, $mermaidText, [System.Text.UTF8Encoding]::new($false))
    Write-Host "Agent graph exported: $(Get-RelPath $OutFile)"
    Write-Host "  diagram: $(Get-RelPath $MermaidOut)"
    Write-Host "  agents=$($agents.Count) skills=$($skills.Count) rules=$($rules.Count) specs=$($specs.Count) tests=$($tests.Count) edges=$($edges.Count)"
}
exit 0
