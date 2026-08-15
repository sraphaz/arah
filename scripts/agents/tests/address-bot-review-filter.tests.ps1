#Requires -Version 5.1
# Testes leves do filtro de ruído Arah (address-bot-review helpers).
$ErrorActionPreference = 'Stop'
$failed = 0

function Assert-True($cond, $msg) {
    if (-not $cond) { Write-Host "FAIL: $msg"; $script:failed++ } else { Write-Host "OK: $msg" }
}

# Inline copies of predicates (keep in sync with address-bot-review.ps1)
function Test-ArahSignalComment([string]$Body) {
    if ([string]::IsNullOrWhiteSpace($Body)) { return $false }
    return [bool]($Body -match '(?i)(<!--\s*arah-(domain-consult|agent-activity|orchestrator|qa-gate|security-gate|pr-steward|pr-graph|bot-response|agent-reply))') `
        -or [bool]($Body -match '(?i)(##\s*PR Steward|##\s*QA Agent — Checklist|##\s*Security Agent — Relatório|##\s*Resposta aos bots|##\s*Pareceres endereçados|##\s*Agent Graph \(PR\))')
}
function Test-ReviewBotAuthor([string]$Author) {
    return [bool]($Author -match '(?i)(coderabbit|bugbot|chatgpt-codex|codex|cursor|trivy|codeql|dependabot)')
}

Assert-True (Test-ArahSignalComment '<!-- arah-domain-consult:monetizacao-split -->') 'domain-consult ignored'
Assert-True (Test-ArahSignalComment '<!-- arah-agent-activity:backend -->') 'agent-activity ignored'
Assert-True (Test-ArahSignalComment "## PR Steward — Apontamentos`n<!-- arah-pr-steward -->") 'steward ignored'
Assert-True (-not (Test-ArahSignalComment 'Critical: null ref in Foo.cs')) 'real finding not ignored'
Assert-True (Test-ReviewBotAuthor 'coderabbitai') 'coderabbit is review bot'
Assert-True (Test-ReviewBotAuthor 'chatgpt-codex-connector') 'codex is review bot'
Assert-True (-not (Test-ReviewBotAuthor 'github-actions[bot]')) 'github-actions not review bot for pending'

if ($failed -gt 0) { Write-Host "`n$failed failure(s)"; exit 1 }
Write-Host "`nAll filter tests passed"
exit 0
