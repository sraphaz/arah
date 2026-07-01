#Requires -Version 5.1
<#
.SYNOPSIS
  Valida specs YAML em docs/specs/ (SDD).
.EXAMPLE
  ./validate-specs.ps1
  ./validate-specs.ps1 -SpecId FASE53-arah-core
#>
param(
    [string]$SpecId = '',
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$SpecsRoot = Join-Path $Root 'docs/specs'

$requiredRoot = @('id', 'version', 'title', 'owner', 'status', 'acceptance')
$errors = @()
$validated = @()

function Get-SimpleYamlField {
    param([string]$Raw, [string]$Field)
    if ($Raw -match "(?m)^$Field\s*:\s*(.+)$") {
        return $Matches[1].Trim().Trim('"')
    }
    return $null
}

function Test-SpecFile {
    param([string]$Path)

    $raw = Get-Content $Path -Raw
    $rel = $Path.Replace($Root + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/')
    $fileErrors = @()

    foreach ($field in $requiredRoot) {
        if ($raw -notmatch "(?m)^$field\s*:") {
            $fileErrors += "$rel missing '$field'"
        }
    }

    $id = Get-SimpleYamlField -Raw $raw -Field 'id'
    if ($SpecId -and $id -ne $SpecId) { return $null }

    if ($raw -notmatch '(?ms)^acceptance:\s*\n(?:\s+-\s+id:)') {
        $fileErrors += "$rel acceptance must list items with id"
    }

    $status = Get-SimpleYamlField -Raw $raw -Field 'status'
    if ($status -and $status -notin @('draft', 'active', 'deprecated')) {
        $fileErrors += "$rel invalid status: $status"
    }

    if ($fileErrors.Count -gt 0) {
        return @{ path = $rel; id = $id; ok = $false; errors = $fileErrors }
    }

    return @{ path = $rel; id = $id; ok = $true; status = $status; errors = @() }
}

Get-ChildItem -Path $SpecsRoot -Recurse -Filter '*.spec.yaml' |
    Where-Object { $_.Name -ne '_template.spec.yaml' } |
    ForEach-Object {
        $result = Test-SpecFile -Path $_.FullName
        if ($result) {
            $validated += $result
            if (-not $result.ok) { $errors += $result.errors }
        }
    }

if ($SpecId -and ($validated | Where-Object { $_.id -eq $SpecId }).Count -eq 0) {
    $errors += "Spec not found: $SpecId"
}

$summary = [ordered]@{
    specs   = @($validated)
    count   = $validated.Count
    errors  = $errors
    ok      = ($errors.Count -eq 0)
}

if ($Json) { $summary | ConvertTo-Json -Depth 6 } else {
    Write-Host "Validated $($validated.Count) spec(s)"
    if ($errors.Count -gt 0) { $errors | ForEach-Object { Write-Error $_ } }
}

if ($errors.Count -gt 0) { exit 1 }
exit 0
