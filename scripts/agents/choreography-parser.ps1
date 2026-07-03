#Requires -Version 5.1
<#
.SYNOPSIS
  Parser compartilhado de .agents/choreography.yaml (regex, PS 5.1+).
.DESCRIPTION
  Fonte única usada por choreograph-agents.ps1, export-agent-graph.ps1 e
  validate-agent-graph.ps1 para evitar deriva silenciosa entre runtime, export
  e validação.
#>
function Parse-ChoreographyRules {
    param([string]$Raw)
    $rules = @()
    $blocks = [regex]::Split($Raw, '(?m)^  - id: ')
    foreach ($block in $blocks) {
        # Ignora preâmbulo/comentários; rule ids são lowercase-dashed + newline.
        if ($block -notmatch '^([a-z][\w-]*)\r?\n') { continue }
        $ruleId = $Matches[1].Trim()
        $paths = @()
        if ($block -match '(?m)^    paths:\s*\r?\n((?:      - [^\r\n]+\r?\n?)+)') {
            $paths = [regex]::Matches($Matches[1], '^\s+-\s+(.+)$', 'Multiline') | ForEach-Object {
                $_.Groups[1].Value.Trim().Trim('"').Trim("'")
            }
        }
        $when = if ($block -match '(?m)^    when:\s+(\S+)') { $Matches[1].Trim() } else { $null }
        $agents = @()
        if ($block -match '(?ms)^    agents:\s*\n((?:      - .+\r?\n?)+)') {
            $agentSection = $Matches[1]
            $agentChunks = [regex]::Split($agentSection, '(?m)^      - id: ')
            foreach ($chunk in $agentChunks) {
                if ($chunk -notmatch '^(\S+)') { continue }
                $aid = $Matches[1].Trim()
                $type = if ($chunk -match '(?m)^        type:\s+(\S+)') { $Matches[1] } else { 'operational' }
                $autonomy = @()
                if ($chunk -match '(?m)^        autonomy:\s*\[(.+)\]') {
                    $autonomy = $Matches[1] -split ',' | ForEach-Object { $_.Trim() }
                }
                $skills = @()
                if ($chunk -match '(?m)^        skills:\s*\[(.+)\]') {
                    $skills = $Matches[1] -split ',' | ForEach-Object { $_.Trim() }
                } elseif ($chunk -match '(?ms)^        skills:\s*\n((?:          - .+\r?\n?)+)') {
                    $skills = [regex]::Matches($Matches[1], '^\s+-\s+(\S+)', 'Multiline') | ForEach-Object { $_.Groups[1].Value }
                }
                $agents += @{
                    id       = $aid
                    type     = $type
                    kind     = $type
                    autonomy = @($autonomy)
                    skills   = @($skills)
                }
            }
        }
        if ($paths.Count -gt 0) {
            $rules += @{ id = $ruleId; when = $when; paths = @($paths); agents = @($agents) }
        }
    }
    return $rules
}
