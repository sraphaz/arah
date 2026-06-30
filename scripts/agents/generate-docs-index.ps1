#Requires -Version 5.1
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$Out = Join-Path $Root 'docs/INDEX.generated.md'
$now = Get-Date -Format 'yyyy-MM-dd'

$lines = @(
    '# Índice gerado — docs',
    '',
    "> Gerado em **$now** por `scripts/agents/generate-docs-index.ps1`. Não editar manualmente.",
    '',
    '| Documento | Owner | Updated |',
    '|-----------|-------|---------|'
)

Get-ChildItem -Path (Join-Path $Root 'docs') -Recurse -Filter '*.md' |
    Where-Object { $_.FullName -notmatch '_archive|INDEX\.generated' } |
    Sort-Object FullName |
    ForEach-Object {
        $head = Get-Content $_.FullName -TotalCount 15 -ErrorAction SilentlyContinue
        $title = $_.BaseName
        $owner = '-'
        $updated = '-'
        if ($head -match '^title:\s*(.+)$') { $title = $Matches[1].Trim() }
        if ($head -match '^owner:\s*(.+)$') { $owner = $Matches[1].Trim() }
        if ($head -match '^updated:\s*(.+)$') { $updated = $Matches[1].Trim() }
        $rel = $_.FullName.Substring($Root.Length + 1).Replace('\', '/')
        $lines += "| [$title]($rel) | $owner | $updated |"
    }

$lines | Set-Content -Path $Out -Encoding UTF8
Write-Host "Wrote $Out ($(( $lines.Count - 4 )) entries)"
