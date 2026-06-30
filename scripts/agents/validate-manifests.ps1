#Requires -Version 5.1
$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

$required = @('id', 'name')
$errors = @()

Get-ChildItem -Path (Join-Path $Root '.agents') -Recurse -Filter '*.agent.yaml' | ForEach-Object {
    $raw = Get-Content $_.FullName -Raw
    foreach ($field in $required) {
        if ($raw -notmatch "(?m)^$field\s*:") {
            $errors += "$($_.FullName): missing '$field'"
        }
    }
}

Get-ChildItem -Path (Join-Path $Root '.skills') -Filter '*.skill.yaml' | ForEach-Object {
    $raw = Get-Content $_.FullName -Raw
    if ($raw -notmatch '(?m)^id\s*:') {
        $errors += "$($_.FullName): missing 'id'"
    }
}

if (-not (Test-Path (Join-Path $Root 'AGENTS.md'))) {
    $errors += 'AGENTS.md missing at repo root'
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "Validated agent/skill manifests — OK ($((Get-ChildItem (Join-Path $Root '.agents') -Recurse -Filter '*.agent.yaml').Count) agents, $((Get-ChildItem (Join-Path $Root '.skills') -Filter '*.skill.yaml').Count) skills)."
