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

Write-Host 'choreography (repo)'
$rootDir = Split-Path -Parent (Split-Path -Parent $agentsDir)
$repoChoreo = Get-Content (Join-Path $rootDir '.agents/choreography.yaml') -Raw
$repoRules = @(Parse-ChoreographyRules -Raw $repoChoreo)
$craftBackendRule = @($repoRules | Where-Object { $_.id -eq 'craft-backend' })[0]
Assert-True ($null -ne $craftBackendRule) 'craft-backend existe na coreografia'
Assert-True (@($craftBackendRule.paths) -contains 'backend/Arah.Infrastructure.Shared/**') 'craft-backend cobre Infrastructure.Shared'

Write-Host 'export-agent-graph'
$specDir = Join-Path $rootDir 'docs/specs/tests'
$tempSpec = Join-Path $specDir '__tmp_agent_graph_blank_lines.spec.yaml'
New-Item -ItemType Directory -Path $specDir -Force | Out-Null
@"
id: tmp-agent-graph-blank-lines
title: "Spec temporaria de regressao"
owner: qa
status: draft

acceptance:
  - id: AC-TMP-1
    then: "primeiro criterio"
    status: covered
    covered_by: "FullyQualifiedName~TmpOne"

  - id: AC-TMP-2
    then: "segundo criterio apos linha em branco"
    status: covered
    covered_by: "FullyQualifiedName~TmpTwo"

harness:
  agents:
    - qa
"@ | Set-Content -Path $tempSpec -Encoding UTF8

try {
    $exportScript = Join-Path $agentsDir 'export-agent-graph.ps1'
    $graph = ((& $exportScript -Json) -join "`n") | ConvertFrom-Json
    $tmpSpec = @($graph.nodes.specs | Where-Object { $_.id -eq 'tmp-agent-graph-blank-lines' })[0]
    Assert-True ($null -ne $tmpSpec) 'export inclui spec temporaria'
    Assert-Equal 2 @($tmpSpec.acceptance).Count 'export preserva ACs apos linha em branco'
    $testIds = @($graph.nodes.tests | ForEach-Object { $_.id })
    Assert-True ($testIds -contains 'FullyQualifiedName~TmpOne') 'export cria no de teste para AC antes da linha em branco'
    Assert-True ($testIds -contains 'FullyQualifiedName~TmpTwo') 'export cria no de teste para AC apos a linha em branco'
} finally {
    if (Test-Path $tempSpec) { Remove-Item $tempSpec -Force }
    if ((Test-Path $specDir) -and -not (Get-ChildItem -Path $specDir -Force)) { Remove-Item $specDir -Force }
}

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

Write-Host ''
Write-Host "Resultado: $pass ok, $fail falha(s)"
if ($fail -gt 0) { exit 1 }
exit 0
