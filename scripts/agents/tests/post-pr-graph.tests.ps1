#Requires -Version 5.1
# Testes leves de post-pr-graph (parsing de route.json com choreography).
$ErrorActionPreference = 'Stop'
$failed = 0
$here = $PSScriptRoot
$root = (Resolve-Path (Join-Path $here '../../..')).Path

function Assert-True($cond, $msg) {
    if (-not $cond) { Write-Host "FAIL: $msg"; $script:failed++ } else { Write-Host "OK: $msg" }
}

$tmpRoute = Join-Path ([System.IO.Path]::GetTempPath()) "arah-route-test-$([guid]::NewGuid()).json"
$route = @{
    agent      = 'backend'
    agent_name = 'Backend'
    skills     = @('run-tests', 'sync-docs')
    co_agents  = @('qa')
    path_agents = @(
        @{ agent = 'docs-steward'; files = @('docs/CHANGELOG.md') }
    )
    choreography = @{
        matched_rules   = @('backend-paths')
        domain_consults = @('monetizacao-split', 'carteira-arata')
        operational     = @('backend', 'qa')
    }
} | ConvertTo-Json -Depth 6
Set-Content -Path $tmpRoute -Value $route -Encoding utf8

$out = & (Join-Path $root 'scripts/agents/post-pr-graph.ps1') `
    -PrNumber 1 `
    -RouteFile $tmpRoute `
    -ChangedFiles @('backend/Arah.Api/Program.cs', 'docs/CHANGELOG.md') `
    -Json 2>&1 | Out-String

Remove-Item $tmpRoute -Force -ErrorAction SilentlyContinue

Assert-True ($out -match '"primary"\s*:\s*"backend"') 'primary agent from route.agent'
Assert-True ($out -match 'monetizacao-split') 'domain from choreography.domain_consults'
Assert-True ($out -match 'carteira-arata') 'second domain'
Assert-True ($out -match 'run-tests') 'skills from route'
Assert-True ($out -match '"posted"\s*:\s*false') 'DryRun / no PostComment'

if ($failed -gt 0) { Write-Host "`n$failed failure(s)"; exit 1 }
Write-Host "`nAll post-pr-graph tests passed"
exit 0
