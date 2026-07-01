#Requires -Version 5.1
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$Out = Join-Path $Root 'docs/INDEX.generated.md'
$now = Get-Date -Format 'yyyy-MM-dd'

$lines = @(
    '# Índice gerado — docs',
    '',
    "> Gerado em **$now** por ``scripts/agents/generate-docs-index.ps1``. Não editar manualmente.",
    "> Exclui stubs ``status: deprecated`` na raiz (ver ``_archive/``).",
    '',
    '| Documento | Owner | Updated |',
    '|-----------|-------|---------|'
)

$count = 0
Get-ChildItem -Path (Join-Path $Root 'docs') -Recurse -Filter '*.md' |
    Where-Object { $_.FullName -notmatch 'INDEX\.generated' } |
    Sort-Object FullName |
    ForEach-Object {
        $rawHead = (Get-Content $_.FullName -TotalCount 20 -ErrorAction SilentlyContinue) -join "`n"
        if ($rawHead -match '(?m)^status:\s*deprecated\s*$') { return }

        $title = $_.BaseName
        $owner = '-'
        $updated = '-'
        if ($rawHead -match '(?m)^title:\s*(.+)$') { $title = $Matches[1].Trim().Trim('"') }
        if ($rawHead -match '(?m)^owner:\s*(.+)$') { $owner = $Matches[1].Trim() }
        if ($rawHead -match '(?m)^updated:\s*(.+)$') { $updated = $Matches[1].Trim() }
        $rel = $_.FullName.Substring($Root.Length + 1).Replace('\', '/')
        $lines += "| [$title]($rel) | $owner | $updated |"
        $count++
    }

$lines | Set-Content -Path $Out -Encoding UTF8
Write-Host "Wrote $Out ($count entries)"
