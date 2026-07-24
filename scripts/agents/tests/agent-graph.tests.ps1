#Requires -Version 5.1
<#
.SYNOPSIS
  Testes do tooling do Agent Graph (yaml-lite, choreography-parser, craft-review).
.DESCRIPTION
  Sem dependência de Pester: asserts simples, exit 1 no primeiro grupo com falha.
  Fecha a lacuna de "tooling sem teste" apontada pela própria skill craft-review.
.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File scripts/agents/tests/agent-graph.tests.ps1
#>
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'

$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$agentsDir = Split-Path -Parent $here
. (Join-Path $agentsDir 'yaml-lite.ps1')
. (Join-Path $agentsDir 'choreography-parser.ps1')

$script:pass = 0
$script:fail = 0

function Assert-Equal {
    param($Expected, $Actual, [string]$Name)
    $e = ($Expected | Out-String).Trim()
    $a = ($Actual | Out-String).Trim()
    if ($e -eq $a) { $script:pass++; Write-Host "  [ok] $Name" }
    else { $script:fail++; Write-Host "  [FAIL] $Name`n     esperado: $e`n     obtido:   $a" }
}

function Assert-True {
    param([bool]$Condition, [string]$Name)
    if ($Condition) { $script:pass++; Write-Host "  [ok] $Name" }
    else { $script:fail++; Write-Host "  [FAIL] $Name" }
}

Write-Host 'yaml-lite'
$yaml = @"
id: sample-agent
skills:
  - run-tests
  - open-pr
scope:
  paths:
    - backend/**
    - docs/**
"@
Assert-Equal 'sample-agent' (Get-ScalarField -Raw $yaml -Field 'id') 'Get-ScalarField lê escalar top-level'
Assert-Equal $null (Get-ScalarField -Raw $yaml -Field 'missing') 'Get-ScalarField retorna null se ausente'
Assert-Equal @('run-tests', 'open-pr') (Get-ListUnderKey -Raw $yaml -Key 'skills' -Indent 0) 'Get-ListUnderKey nível 0'
Assert-Equal @('backend/**', 'docs/**') (Get-ListUnderKey -Raw $yaml -Key 'paths' -Indent 2) 'Get-ListUnderKey aninhado (indent 2)'
Assert-Equal @() (Get-ListUnderKey -Raw $yaml -Key 'nope' -Indent 0) 'Get-ListUnderKey vazio se chave ausente'

Write-Host 'choreography-parser'
$choreo = @"
rules:
  - id: sample-rule
    when: on_change
    paths:
      - backend/**
    agents:
      - id: backend
        type: operational
        autonomy: [invoke_skill]
        skills: [run-tests, craft-review]
  - id: no-paths-rule
    agents:
      - id: qa
"@
$rules = @(Parse-ChoreographyRules -Raw $choreo)
Assert-Equal 2 $rules.Count 'regra sem paths é mantida (validador flagra depois)'
Assert-Equal 'sample-rule' $rules[0].id 'id da regra'
Assert-Equal @('backend/**') $rules[0].paths 'paths da regra'
Assert-Equal 'backend' @($rules[0].agents)[0].id 'agente da regra'
Assert-Equal @('run-tests', 'craft-review') @($rules[0].agents)[0].skills 'skills inline da regra'
Assert-Equal 'no-paths-rule' $rules[1].id 'regra sem paths presente'
Assert-Equal 0 @($rules[1].paths).Count 'regra sem paths tem paths vazio'

Write-Host 'craft-review-check (detecção de arquivo)'
. (Join-Path $agentsDir 'craft-review-check.ps1')
if (Get-Command Test-IsCodeFile -ErrorAction SilentlyContinue) {
    Assert-True (Test-IsCodeFile 'scripts/agents/export-agent-graph.ps1') '.ps1 conta como código'
    Assert-True (Test-IsCodeFile 'scripts/agents/agent-graph-mcp.mjs') '.mjs conta como código'
    Assert-True (-not (Test-IsCodeFile 'docs/ops/AGENT_GRAPH.md')) '.md não conta como código'
    Assert-True (Test-IsTestFile 'scripts/agents/tests/agent-graph.tests.ps1') '*.tests.ps1 conta como teste'
} else {
    Write-Host '  [skip] Test-IsCodeFile não exposto (craft-review-check executa direto)'
}

Write-Host 'next-phase eligibility (TI track)'
. (Join-Path $agentsDir 'Get-PhaseQueue.ps1')
Assert-True (Test-PhaseQueueItemEligibleForNextPhase -Item @{ id = 'FASE55'; kind = $null }) 'FASE55 elegível'
Assert-True (-not (Test-PhaseQueueItemEligibleForNextPhase -Item @{ id = 'TI-0'; kind = 'track' })) 'TI-0 track não elegível'
Assert-True (-not (Test-PhaseQueueItemEligibleForNextPhase -Item @{ id = 'TI-1' })) 'TI-1 por id não elegível'
Assert-True (-not (Test-PhaseQueueItemEligibleForNextPhase -Item @{ id = 'dod-retrofit'; kind = 'maintenance' })) 'maintenance não elegível'
Assert-True (-not (Test-PhaseQueueItemEligibleForNextPhase -Item @{ id = 'doc-deflation-wave2' })) 'doc-* não elegível'

Write-Host ''
Write-Host "Resultado: $pass ok, $fail falha(s)"
if ($fail -gt 0) { exit 1 }
exit 0
