#Requires -Version 5.1
<#
.SYNOPSIS
  Gate leve de regressão de IA do app Flutter (APP-DS-13).
.DESCRIPTION
  Verifica contratos estruturais do shell alinhados ao ADR-021 / UI kit:
  - MainShell com aba Serviços (não Notificações na 4ª posição)
  - ArahTopBar presente
  - Hub Serviços e JourneyShell existem
  Falha o PR se algum contrato quebrar.
.EXAMPLE
  ./design-ia-gate-check.ps1
#>
param(
    [string]$AppRoot = 'frontend/arah.app'
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$failures = @()

function Assert-FileContains {
    param([string]$RelPath, [string]$Pattern, [string]$Hint)
    $full = Join-Path $Root $RelPath
    if (-not (Test-Path $full)) {
        $script:failures += "ausente: $RelPath ($Hint)"
        return
    }
    $text = Get-Content -Raw $full
    if ($text -notmatch $Pattern) {
        $script:failures += "${RelPath}: esperado /$Pattern/ — $Hint"
    }
}

Assert-FileContains `
    -RelPath "$AppRoot/lib/features/home/presentation/screens/main_shell_screen.dart" `
    -Pattern 'ServicesHubScreen' `
    -Hint '4ª aba do shell deve ser Serviços (ADR-021)'

Assert-FileContains `
    -RelPath "$AppRoot/lib/features/home/presentation/screens/main_shell_screen.dart" `
    -Pattern 'ArahTopBar' `
    -Hint 'TopBar território-primeiro obrigatória no MainShell'

Assert-FileContains `
    -RelPath "$AppRoot/lib/features/home/presentation/screens/main_shell_screen.dart" `
    -Pattern 'l10n\.services' `
    -Hint 'label Serviços na NavigationBar'

Assert-FileContains `
    -RelPath "$AppRoot/lib/core/widgets/arah_top_bar.dart" `
    -Pattern "/notifications" `
    -Hint 'TopBar deve abrir notificações'

Assert-FileContains `
    -RelPath "$AppRoot/lib/core/widgets/arah_top_bar.dart" `
    -Pattern "/chat" `
    -Hint 'TopBar deve abrir mensagens'

Assert-FileContains `
    -RelPath "$AppRoot/lib/features/services/presentation/screens/services_hub_screen.dart" `
    -Pattern 'statusLive|statusSoon' `
    -Hint 'Hub Serviços com selos live/soon'

Assert-FileContains `
    -RelPath "$AppRoot/lib/core/widgets/arah_journey_shell.dart" `
    -Pattern 'class ArahJourneyShell' `
    -Hint 'JourneyShell deve existir'

Assert-FileContains `
    -RelPath "$AppRoot/lib/core/theme/app_design_tokens.dart" `
    -Pattern '0xFFA6D6B9|A6D6B9' `
    -Hint 'primária premium floresta (canopy)'

if ($failures.Count -gt 0) {
    Write-Warning "design-ia-gate-check: $($failures.Count) falha(s) de contrato de IA:"
    $failures | ForEach-Object { Write-Warning "  $_" }
    Write-Error 'design-ia-gate-check FALHOU — shell/IA divergiu do ADR-021'
    exit 1
}

Write-Output 'design-ia-gate-check — OK (contratos de IA do app Flutter)'
exit 0
