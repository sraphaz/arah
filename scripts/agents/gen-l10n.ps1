#Requires -Version 5.1
$ErrorActionPreference = 'Stop'
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$app = Join-Path $Root 'frontend/arah.app'
$l10nSrc = Join-Path $app '.dart_tool/flutter_gen/gen_l10n'
$l10nDst = Join-Path $app 'lib/l10n'

Push-Location $app
try {
    flutter gen-l10n
    if (Test-Path $l10nSrc) {
        Copy-Item -Path (Join-Path $l10nSrc '*') -Destination $l10nDst -Force -ErrorAction SilentlyContinue
    }
    Write-Host "gen-l10n: OK ($l10nDst)"
} finally {
    Pop-Location
}
