#Requires -Version 5.1
<#
.SYNOPSIS
  Hook do Cursor (evento `stop`): gera os pareceres dos agentes de domínio em
  .cursor/domain-review.md de forma PASSIVA (comunicação por arquivo).

  Não injeta followup_message: o parecer é consumido pela rule escopada
  (.cursor/rules/domain-agents-autonomy.mdc) quando arquivos de domínio estão
  em contexto, e pelo CI (agents.yml) que publica comentários no PR.
  Isso evita turnos extras de modelo a cada interação (custo de API).

  Fail-open: qualquer erro resulta em '{}' (não bloqueia nem interrompe o fluxo).
  Idempotente por conjunto de mudanças (sentinela em .cursor/.domain-review.state).
#>

$ErrorActionPreference = 'SilentlyContinue'

try { $null = [Console]::In.ReadToEnd() } catch {}

try {
    $root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
    $script = Join-Path $root 'scripts/agents/domain-autoreview.ps1'
    if (Test-Path $script) {
        $null = & $script -Json 2>$null
    }
} catch {}

'{}'
exit 0
