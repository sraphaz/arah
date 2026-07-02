#Requires -Version 5.1
<#
.SYNOPSIS
  Cria git worktrees isolados para tentativas paralelas de uma mesma tarefa
  (best-of-N): cada tentativa tem branch e diretório próprios; o melhor PR vence.
.EXAMPLE
  ./parallel-attempt.ps1 -Name fase56-revenue -Count 2
  ./parallel-attempt.ps1 -Name fase56-revenue -Cleanup
#>
param(
    [Parameter(Mandatory)]
    [string]$Name,
    [int]$Count = 2,
    [string]$BaseRef = 'main',
    [switch]$Cleanup,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$WorktreesDir = Join-Path (Split-Path $Root -Parent) 'arah-worktrees'

Push-Location $Root
try {
    $results = @()

    if ($Cleanup) {
        foreach ($wt in (git worktree list --porcelain | Select-String '^worktree (.+)' | ForEach-Object { $_.Matches[0].Groups[1].Value })) {
            if ((Split-Path $wt -Leaf) -like "$Name-attempt-*") {
                git worktree remove $wt --force
                $results += @{ worktree = $wt; action = 'removed' }
            }
        }
    } else {
        if ($Count -lt 2 -or $Count -gt 5) {
            Write-Error 'Count deve estar entre 2 e 5 (best-of-N pragmático)'
            exit 1
        }
        New-Item -ItemType Directory -Path $WorktreesDir -Force | Out-Null
        for ($i = 1; $i -le $Count; $i++) {
            $branch = "attempt/$Name-$i"
            $path = Join-Path $WorktreesDir "$Name-attempt-$i"
            if (Test-Path $path) {
                $results += @{ worktree = $path; branch = $branch; action = 'exists' }
                continue
            }
            git worktree add -b $branch $path $BaseRef
            $results += @{ worktree = $path; branch = $branch; action = 'created' }
        }
    }

    if ($Json) { @{ name = $Name; attempts = $results } | ConvertTo-Json -Depth 4 }
    else { $results | ForEach-Object { Write-Host "$($_.action): $($_.worktree)" } }
} finally { Pop-Location }
exit 0
