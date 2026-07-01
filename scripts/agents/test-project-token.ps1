#Requires -Version 5.1
<#
.SYNOPSIS
  Diagnóstico do GH_PROJECT_TOKEN — scopes e acesso a Projects v2.
.EXAMPLE
  $env:GH_TOKEN = 'ghp_...'; ./test-project-token.ps1
#>
param([switch]$Json)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'GitHub-ProjectApi.ps1')

$token = Get-GhProjectToken
if (-not $token) {
    Write-Error 'GH_PROJECT_TOKEN / GH_TOKEN ausente'
    exit 1
}

$headers = gh api -i user 2>&1 | Out-String
$scopes = ''
if ($headers -match 'x-oauth-scopes:\s*(.+)') {
    $scopes = $Matches[1].Trim()
}

$isClassic = $scopes -ne ''
$hasProjectWrite = $scopes -match '\bproject\b'
$hasRepo = $scopes -match '\brepo\b'

$viewerOk = $false
$createOk = $false
$viewerError = $null
$createError = $null

try {
    $r = Invoke-GhGraphql -Query 'query { viewer { login projectsV2(first: 1) { nodes { number title } } } }' -AllowPartialErrors
    $viewerOk = $true
    $login = $r.data.viewer.login
} catch {
    $viewerError = $_.Exception.Message
    $login = $null
}

$report = [ordered]@{
    token_type       = if ($isClassic) { 'classic' } else { 'fine-grained-or-unknown' }
    oauth_scopes     = if ($scopes) { $scopes } else { '(fine-grained — sem x-oauth-scopes)' }
    has_project      = $hasProjectWrite
    has_repo         = $hasRepo
    viewer_login     = $login
    viewer_query_ok  = $viewerOk
    viewer_error     = $viewerError
    recommendation   = $null
}

if (-not $isClassic) {
    $report.recommendation = @'
Use PAT CLASSIC (não fine-grained) para Projects do usuário sraphaz.

GitHub → Settings → Developer settings → Personal access tokens
→ Tokens (classic) → Generate new token (classic)
Marcar: project + repo
'@
} elseif (-not $hasProjectWrite) {
    $report.recommendation = 'PAT classic sem scope project. Recrie o token marcando "project" (não só read:project).'
} elseif (-not $viewerOk) {
    $report.recommendation = 'Token tem scope project mas query falhou — autorize SSO em github.com/settings/tokens se aplicável.'
} else {
    $report.recommendation = 'Token OK para bootstrap. Rode: ./scripts/agents/github-project.ps1 bootstrap'
}

if ($Json) { $report | ConvertTo-Json -Depth 4 } else {
    $report | Format-List
    if ($report.recommendation) { Write-Host "`n$($report.recommendation)" -ForegroundColor Yellow }
}

if (-not $viewerOk -or (-not $isClassic) -or (-not $hasProjectWrite)) { exit 1 }
exit 0
