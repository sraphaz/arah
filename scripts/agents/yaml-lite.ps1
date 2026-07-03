#Requires -Version 5.1
<#
.SYNOPSIS
  Helpers de leitura "YAML-lite" por regex (PS 5.1+), fonte única para
  export-agent-graph.ps1 e validate-agent-graph.ps1.
.DESCRIPTION
  Assume a formatação atual dos manifests (2 espaços de indentação, sem YAML
  avançado). Extraído para evitar duplicação (DRY) entre export e validate — a
  mesma leitura frágil deve ter um só dono.
#>

# Valor escalar de uma chave top-level: "key: valor".
function Get-ScalarField {
    param([string]$Raw, [string]$Field)
    if ($Raw -match "(?m)^$Field\s*:\s*(.+)$") { return $Matches[1].Trim().Trim('"').Trim("'") }
    return $null
}

# Itens de uma lista sob uma chave, no nível de indentação informado.
# Ex.: Get-ListUnderKey -Raw $y -Key 'skills' -Indent 0  -> @('run-tests', ...)
function Get-ListUnderKey {
    param([string]$Raw, [string]$Key, [int]$Indent)
    $prefix = ' ' * $Indent
    $itemPrefix = ' ' * ($Indent + 2)
    $pattern = '(?m)^{0}{1}:[ \t]*\r?\n((?:{2}-[ \t]+[^\r\n]+\r?\n?)+)' -f $prefix, [regex]::Escape($Key), $itemPrefix
    if ($Raw -match $pattern) {
        return @([regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') |
            ForEach-Object { $_.Groups[1].Value.Trim().Trim('"').Trim("'") })
    }
    return @()
}

# Bloco YAML de uma chave top-level até a próxima chave de nível 0. Usa lazy
# `.*?` com lookahead ancorado — sem quantificador aninhado (evita backtracking
# catastrófico).
function Get-TopLevelBlock {
    param([string]$Raw, [string]$Key)
    if ($Raw -match "(?ms)^$([regex]::Escape($Key))\s*:\s*\r?\n(.*?)(?=^[A-Za-z_][\w-]*\s*:|\z)") {
        return $Matches[1]
    }
    return ''
}
