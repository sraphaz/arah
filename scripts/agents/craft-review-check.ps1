#Requires -Version 5.1
<#
.SYNOPSIS
  Craft review (Uncle Bob) — lembrete e heurísticas soft para o ciclo
  projetar -> desenvolver -> testar. Complementa architecture-review e
  code-review; não substitui linter/formatter nem os testes.
.DESCRIPTION
  Soft por padrão: emite avisos e o checklist por fase, sempre exit 0.
  Com -Strict, avisos viram erro (exit 1) — para promover a gate duro depois.
.EXAMPLE
  ./craft-review-check.ps1
.EXAMPLE
  ./craft-review-check.ps1 -ChangedFiles backend/Arah.Domain/Foo.cs -Strict
#>
param(
    [string[]]$ChangedFiles = @(),
    [string]$BaseRef = 'origin/main',
    [switch]$Strict
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

function Get-ChangedFiles {
    param([string[]]$Provided, [string]$BaseRef, [string]$RepoRoot)
    if ($Provided.Count -gt 0) { return @($Provided | Where-Object { $_ }) }
    Push-Location $RepoRoot
    try {
        git fetch origin main --depth=1 2>$null
        return @(git diff --name-only "$BaseRef...HEAD" 2>$null | Where-Object { $_ })
    } finally { Pop-Location }
}

# Inclui o tooling do próprio Ará (PowerShell/Node), não só app code — senão a
# própria skill de craft ficaria cega para o que a implementa (dogfooding).
function Test-IsCodeFile {
    param([string]$Path)
    return $Path -match '\.(cs|dart|ts|tsx|js|jsx|mjs|cjs|ps1|psm1|py)$'
}

function Test-IsTestFile {
    param([string]$Path)
    $p = $Path.Replace('\', '/')
    return ($p -match '(?i)(/tests?/|/__tests__/|\.tests?\.|_test\.|\.spec\.|\.test\.)')
}

# Mudança de comportamento sem teste na mesma leva é o smell mais barato de pegar.
function Get-BehaviorWithoutTestWarning {
    param([string[]]$Files)
    $code = @($Files | Where-Object { (Test-IsCodeFile $_) -and -not (Test-IsTestFile $_) })
    $tests = @($Files | Where-Object { Test-IsTestFile $_ })
    if ($code.Count -gt 0 -and $tests.Count -eq 0) {
        return "mudança de código sem teste na mesma leva ($($code.Count) arquivo(s)) — tests_for_behavior_change (soft)"
    }
    return $null
}

function Write-Phase {
    param([string]$Title, [string]$Intent, [string[]]$Items)
    Write-Host ""
    Write-Host "  $Title — $Intent"
    foreach ($i in $Items) { Write-Host "    - $i" }
}

# Dot-sourced (ex.: testes) apenas define funções; execução real fica no bloco
# abaixo. InvocationName '.' indica dot-source.
if ($MyInvocation.InvocationName -eq '.') { return }

$files = Get-ChangedFiles -Provided $ChangedFiles -BaseRef $BaseRef -RepoRoot $Root
$warnings = @()

$behaviorWarning = Get-BehaviorWithoutTestWarning -Files $files
if ($behaviorWarning) { $warnings += $behaviorWarning }

Write-Host "Craft review (Uncle Bob) — projetar -> desenvolver -> testar"
Write-Phase 'PROJETAR' 'fronteiras, responsabilidade e simplicidade' @(
    'Dependency rule: dependências apontam para dentro (Api -> Infra -> App -> Domain).',
    'Single Responsibility: uma razão para mudar por unidade.',
    'Padrão só na 3a duplicação / 2o eixo de mudança (sem cargo cult).',
    'YAGNI/KISS: sem abstração especulativa.'
)
Write-Phase 'DESENVOLVER' 'funções pequenas, nomes que revelam intenção' @(
    'Funções curtas, um nível de abstração.',
    'Nomes revelam intenção (territory/items/membership).',
    'Result<T> em vez de null; async com CancellationToken.',
    'DRY: extrair duplicação; sem comentários que narram o óbvio.'
)
Write-Phase 'TESTAR' 'teste como requisito, não como sobra' @(
    'Toda mudança de comportamento tem teste (TDD quando aplicável).',
    'Cobertura > 90% na área alterada.',
    'AC da spec ligado a covered_by (rastreabilidade fina).'
)

Write-Host ""
Write-Host "Refs: docs/governance/21_CODE_REVIEW.md, 22_COHESION_AND_TESTS.md, DEFINITION_OF_DONE.md"

foreach ($w in $warnings) { Write-Warning $w }

if ($Strict -and $warnings.Count -gt 0) {
    Write-Error "craft-review: $($warnings.Count) aviso(s) em modo -Strict"
    exit 1
}
Write-Host "craft-review-check: OK ($($warnings.Count) aviso(s), soft)"
exit 0
