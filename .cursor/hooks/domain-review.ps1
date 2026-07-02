#Requires -Version 5.1
<#
.SYNOPSIS
  Hook do Cursor (evento `stop`): aciona os agentes de domínio automaticamente
  sobre as mudanças da árvore de trabalho, sem acionamento manual.

  Fail-open: qualquer erro resulta em '{}' (não bloqueia nem interrompe o fluxo).
  Idempotente por conjunto de mudanças (sentinela em .cursor/.domain-review.state),
  então só devolve followup quando há mudança de domínio ainda não revisada.
#>

$ErrorActionPreference = 'SilentlyContinue'

try { $null = [Console]::In.ReadToEnd() } catch {}

try {
    $root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
    $script = Join-Path $root 'scripts/agents/domain-autoreview.ps1'
    if (-not (Test-Path $script)) { '{}'; exit 0 }

    $json = & $script -Json 2>$null | Out-String
    $res = $json | ConvertFrom-Json
    if ($res.domains -and $res.domains.Count -gt 0 -and -not $res.already_reviewed) {
        $names = ($res.domains -join ', ')
        $msg = "Agentes de dominio ($names) revisaram $($res.changed_count) arquivo(s) alterado(s). " +
               "Leia o parecer em $($res.review_file), enderece cada item de 'Validar no PR' antes de concluir " +
               "e confirme a Definition of Done (docs/governance/DEFINITION_OF_DONE.md)."
        ([ordered]@{ followup_message = $msg }) | ConvertTo-Json -Compress
        exit 0
    }
} catch {}

'{}'
exit 0
