#Requires -Version 5.1
<#
.SYNOPSIS
  Gate leve de regressão de IA do app Flutter (APP-DS-13).
.DESCRIPTION
  Verifica contratos estruturais do shell alinhados ao ADR-021 / UI kit:
  - MainShell com aba Serviços (não Notificações na 4ª posição)
  - ArahTopBar presente e ações de chat/notificações
  - Hub Serviços e JourneyShell existem
  - Primária premium floresta (canopy) ativa
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

function Assert-FileMatches {
    param(
        [string]$RelPath,
        [string]$Pattern,
        [string]$Hint
    )
    $full = Join-Path $Root $RelPath
    if (-not (Test-Path $full)) {
        $script:failures += "ausente: $RelPath ($Hint)"
        return
    }
    $text = Get-Content -Raw -LiteralPath $full
    if ($text -notmatch $Pattern) {
        $script:failures += "${RelPath}: esperado /$Pattern/ — $Hint"
    }
}

# 4ª aba do shell: IndexedStack alimentado por _buildScreens com ServicesHubScreen
Assert-FileMatches `
    -RelPath "$AppRoot/lib/features/home/presentation/screens/main_shell_screen.dart" `
    -Pattern '(?s)IndexedStack\([\s\S]*?children:\s*_buildScreens\(\)' `
    -Hint 'MainShell deve usar IndexedStack com _buildScreens()'

Assert-FileMatches `
    -RelPath "$AppRoot/lib/features/home/presentation/screens/main_shell_screen.dart" `
    -Pattern '(?s)_buildScreens\(\)[\s\S]*?const\s+ServicesHubScreen\(\)' `
    -Hint 'ServicesHubScreen deve estar em _buildScreens (ADR-021)'

Assert-FileMatches `
    -RelPath "$AppRoot/lib/features/home/presentation/screens/main_shell_screen.dart" `
    -Pattern '(?s)NavigationDestination\([\s\S]*?label:\s*l10n\.services' `
    -Hint 'label Serviços (l10n.services) na NavigationBar'

Assert-FileMatches `
    -RelPath "$AppRoot/lib/features/home/presentation/screens/main_shell_screen.dart" `
    -Pattern '(?m)^\s*const\s+ArahTopBar\(\),?\s*$' `
    -Hint 'ArahTopBar instanciada no body do MainShell'

# TopBar: ações ligadas a rotas (não só strings soltas em comentários)
Assert-FileMatches `
    -RelPath "$AppRoot/lib/core/widgets/arah_top_bar.dart" `
    -Pattern "context\.push\(\s*'/notifications'\s*\)" `
    -Hint 'TopBar deve navegar para /notifications'

Assert-FileMatches `
    -RelPath "$AppRoot/lib/core/widgets/arah_top_bar.dart" `
    -Pattern "context\.push\(\s*'/chat'\s*\)" `
    -Hint 'TopBar deve navegar para /chat'

Assert-FileMatches `
    -RelPath "$AppRoot/lib/features/services/presentation/screens/services_hub_screen.dart" `
    -Pattern '(?m)^\s*class\s+ServicesHubScreen\b' `
    -Hint 'ServicesHubScreen deve existir'

Assert-FileMatches `
    -RelPath "$AppRoot/lib/features/services/presentation/screens/services_hub_screen.dart" `
    -Pattern '\b(statusLive|statusSoon)\b' `
    -Hint 'Hub Serviços com selos live/soon'

Assert-FileMatches `
    -RelPath "$AppRoot/lib/core/widgets/arah_journey_shell.dart" `
    -Pattern '(?m)^\s*class\s+ArahJourneyShell\s+extends\b' `
    -Hint 'ArahJourneyShell deve ser uma classe Widget'

# Primária ativa = canopy (não só menção em comentário)
Assert-FileMatches `
    -RelPath "$AppRoot/lib/core/theme/app_design_tokens.dart" `
    -Pattern '(?m)^\s*static\s+const\s+Color\s+primary\s*=\s*Color\(0xFFA6D6B9\)' `
    -Hint 'primária premium floresta (canopy) como Color primary ativa'

if ($failures.Count -gt 0) {
    Write-Warning "design-ia-gate-check: $($failures.Count) falha(s) de contrato de IA:"
    $failures | ForEach-Object { Write-Warning "  $_" }
    Write-Error 'design-ia-gate-check FALHOU — shell/IA divergiu do ADR-021'
    exit 1
}

Write-Output 'design-ia-gate-check — OK (contratos de IA do app Flutter)'
exit 0
