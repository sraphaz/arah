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
  ./export-agent-graph.ps1 -Json          # imprime no stdout, não grava arquivo
#>
param(
    [string]$OutFile = '',
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ChoreoPath = Join-Path $Root '.agents/choreography.yaml'
$AgentsDir = Join-Path $Root '.agents'
$SkillsDir = Join-Path $Root '.skills'
$SpecsDir = Join-Path $Root 'docs/specs'
$WorkflowsDir = Join-Path $Root '.github/workflows'
if (-not $OutFile) { $OutFile = Join-Path $Root 'docs/_meta/agent-graph.generated.json' }

function Get-RelPath {
    param([string]$FullPath)
    return $FullPath.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/')
}

# --- Parser da coreografia (mesmo estilo de choreograph-agents.ps1) ----------
function Parse-ChoreographyRules {
    param([string]$Raw)
    $rules = @()
    $blocks = [regex]::Split($Raw, '(?m)^  - id: ')
    foreach ($block in $blocks) {
        if ($block -notmatch '^(\S+)') { continue }
        $ruleId = $Matches[1].Trim()
        $paths = @()
        if ($block -match '(?ms)^    paths:\s*\n((?:      - .+\r?\n?)+)') {
            $paths = [regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') | ForEach-Object {
                $_.Groups[1].Value.Trim().Trim('"').Trim("'")
            }
        }
        $when = if ($block -match '(?m)^    when:\s+(\S+)') { $Matches[1].Trim() } else { $null }
        $agents = @()
        if ($block -match '(?ms)^    agents:\s*\n((?:      - .+\r?\n?)+)') {
            $agentSection = $Matches[1]
            $agentChunks = [regex]::Split($agentSection, '(?m)^      - id: ')
            foreach ($chunk in $agentChunks) {
                if ($chunk -notmatch '^(\S+)') { continue }
                $aid = $Matches[1].Trim()
                $sub = $chunk
                $type = if ($sub -match '(?m)^        type:\s+(\S+)') { $Matches[1] } else { 'operational' }
                $autonomy = @()
                if ($sub -match '(?m)^        autonomy:\s*\[(.+)\]') {
                    $autonomy = $Matches[1] -split ',' | ForEach-Object { $_.Trim() }
                }
                $skills = @()
                if ($sub -match '(?m)^        skills:\s*\[(.+)\]') {
                    $skills = $Matches[1] -split ',' | ForEach-Object { $_.Trim() }
                } elseif ($sub -match '(?ms)^        skills:\s*\n((?:          - .+\r?\n?)+)') {
                    $skills = [regex]::Matches($Matches[1], '^\s+-\s+(\S+)', 'Multiline') | ForEach-Object { $_.Groups[1].Value }
                }
                $agents += [ordered]@{
                    id       = $aid
                    kind     = $type
                    autonomy = @($autonomy)
                    skills   = @($skills)
                }
            }
        }
        if ($paths.Count -gt 0) {
            $rules += [ordered]@{ id = $ruleId; when = $when; paths = @($paths); agents = @($agents) }
        }
    }
    return $rules
}

# --- Helpers de YAML simples (regex) ----------------------------------------
function Get-ScalarField {
    param([string]$Raw, [string]$Field)
    if ($Raw -match "(?m)^$Field\s*:\s*(.+)$") { return $Matches[1].Trim().Trim('"').Trim("'") }
    return $null
}

function Get-ListUnderKey {
    param([string]$Raw, [string]$Key, [int]$Indent)
    $prefix = ' ' * $Indent
    $itemPrefix = ' ' * ($Indent + 2)
    $pattern = '(?m)^{0}{1}:[ \t]*\r?\n((?:{2}-[ \t]+[^\r\n]+\r?\n?)+)' -f $prefix, [regex]::Escape($Key), $itemPrefix
    if ($Raw -match $pattern) {
        return @([regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') |
            ForEach-Object { $_.Groups[1].Value.Trim().Trim('"').Trim("'") })
    }
    return @()
}

# Extrai um bloco top-level YAML até a próxima chave de nível 0. Usa `.*?` lazy
# com lookahead ancorado (padrão seguro de run-harness.ps1) — sem quantificador
# aninhado, evitando backtracking catastrófico.
function Get-TopLevelBlock {
    param([string]$Raw, [string]$Key)
    if ($Raw -match "(?ms)^$([regex]::Escape($Key))\s*:\s*\r?\n(.*?)(?=^[A-Za-z_][\w-]*\s*:|\z)") {
        return $Matches[1]
    }
    return ''
}

function Get-ScopePaths {
    param([string]$Raw)
    $scope = Get-TopLevelBlock -Raw $Raw -Key 'scope'
    if (-not $scope) { return @() }
    return Get-ListUnderKey -Raw $scope -Key 'paths' -Indent 2
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

# --- 4) PathPatterns (agregados das rules) -----------------------------------
$pathMap = @{}
foreach ($rule in $rules) {
    foreach ($p in $rule.paths) {
        if (-not $pathMap.ContainsKey($p)) { $pathMap[$p] = @() }
        if ($pathMap[$p] -notcontains $rule.id) { $pathMap[$p] += $rule.id }
    }
}
$paths = @($pathMap.GetEnumerator() | Sort-Object Name | ForEach-Object {
    [ordered]@{ pattern = $_.Key; rules = @($_.Value) }
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
            $rel = Get-RelPath $_.FullName
            $specs += [ordered]@{
                id             = $id
                status         = $status
                manifest       = $rel
                harness_agents = @($harnessAgents)
                guardrails     = @($grs)
            }
            $harnesses += [ordered]@{ id = $id; spec = $id; agents = @($harnessAgents) }
        }
}
$specs = @($specs | Sort-Object { $_.id } -Unique)
$harnesses = @($harnesses | Sort-Object { $_.id } -Unique)

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
        } elseif ($a.autonomy -contains 'activate') {
            Add-Edge "rule:$($rule.id)" "agent:$($a.id)" 'activates_agent'
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
        guardrails   = @($guardrails)
        workflows    = @($workflows)
        review_gates = @($reviewGates)
    }
    edges = @($edges)
}

$jsonText = $graph | ConvertTo-Json -Depth 12

if ($Json) {
    $jsonText
} else {
    $outDir = Split-Path $OutFile -Parent
    if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir -Force | Out-Null }
    [System.IO.File]::WriteAllText($OutFile, $jsonText + "`n", [System.Text.UTF8Encoding]::new($false))
    Write-Host "Agent graph exported: $(Get-RelPath $OutFile)"
    Write-Host "  agents=$($agents.Count) skills=$($skills.Count) rules=$($rules.Count) specs=$($specs.Count) edges=$($edges.Count)"
}
exit 0
