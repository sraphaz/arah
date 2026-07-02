#Requires -Version 5.1
<#
.SYNOPSIS
  Gate de design (DSG-08): bloqueia cores hardcoded em arquivos de frontend alterados.
.DESCRIPTION
  Aplica LIC-001/LIC-002: cores devem vir de tokens (design-tokens.css, tailwind.config,
  AppDesignTokens). Verifica apenas os arquivos alterados no PR; arquivos de definição
  de tokens são permitidos (é onde os literais vivem).
.EXAMPLE
  ./design-gate-check.ps1 -ChangedFiles frontend/wiki/app/page.tsx
#>
param(
    [string[]]$ChangedFiles = @(),
    [string]$BaseRef = 'origin/main'
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

if ($ChangedFiles.Count -eq 0) {
    $ChangedFiles = @(git diff --name-only "$BaseRef...HEAD" 2>$null)
}

# Arquivos onde literais de cor são a fonte de verdade (definição de tokens/tema).
$tokenFileGlobs = @(
    'frontend/shared/styles/*',
    'frontend/*/tailwind.config.ts',
    'frontend/arah.app/lib/core/theme/*',
    'frontend/wiki/lib/mermaid-theme.ts'
)

function Test-IsTokenFile {
    param([string]$Path)
    foreach ($glob in $tokenFileGlobs) {
        if ($Path -like $glob) { return $true }
    }
    return $false
}

$violations = @()

foreach ($file in $ChangedFiles) {
    $normalized = $file.Replace('\', '/').Trim()
    if (-not $normalized.StartsWith('frontend/')) { continue }
    if (Test-IsTokenFile $normalized) { continue }
    $full = Join-Path $Root $normalized
    if (-not (Test-Path $full)) { continue }

    $ext = [IO.Path]::GetExtension($normalized).ToLowerInvariant()
    $lines = Get-Content $full

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        $lineNo = $i + 1

        switch -Regex ($ext) {
            '^\.(tsx|ts|jsx|js|html)$' {
                # Tailwind arbitrárias com cor: bg-[#4dd4a8], text-[rgb(...)]
                if ($line -match '\[#[0-9a-fA-F]{3,8}\]' -or $line -match '\[rgba?\(') {
                    $violations += "${normalized}:${lineNo}: Tailwind arbitrária com cor — use classe configurada ou var(--...)"
                }
                # style inline com cor hex
                if ($line -match '(color|background)\s*:\s*[''"]?#[0-9a-fA-F]{3,8}') {
                    $violations += "${normalized}:${lineNo}: cor hex inline — use var(--...) ou token"
                }
            }
            '^\.css$' {
                # Hex/rgb permitidos apenas ao definir custom properties (--foo: ...).
                if ($line -notmatch '^\s*--[\w-]+\s*:' -and ($line -match '#[0-9a-fA-F]{3,8}\b' -or $line -match '\brgba?\(')) {
                    $violations += "${normalized}:${lineNo}: cor literal em CSS fora de definição de token — use var(--...)"
                }
            }
            '^\.dart$' {
                # Cores diretas fora do tema: Color(0x...) e paleta Material (Colors.*).
                if ($line -match 'Color\(0x[0-9a-fA-F]{8}\)') {
                    $violations += "${normalized}:${lineNo}: Color(0x...) fora do tema — use AppDesignTokens/AppColors"
                }
                if ($line -match '\bColors\.(?!transparent\b)\w+') {
                    $violations += "${normalized}:${lineNo}: Colors.* fora do tema — use AppDesignTokens/AppColors"
                }
            }
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Warning "design-gate-check: $($violations.Count) violação(ões) de cor hardcoded (LIC-001/LIC-002):"
    $violations | ForEach-Object { Write-Warning "  $_" }
    Write-Error 'design-gate-check FALHOU — cores devem vir de tokens (docs/CURSOR_DESIGN_RULES.md)'
    exit 1
}

Write-Output "design-gate-check — OK (nenhuma cor hardcoded nos arquivos de frontend alterados)"
exit 0
