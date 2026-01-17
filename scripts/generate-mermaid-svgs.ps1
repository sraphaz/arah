# Script PowerShell para gerar SVGs dos diagramas Mermaid usando mermaid-cli
# Pré-requisito: npm install -g @mermaid-js/mermaid-cli

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$diagramsDir = Join-Path $scriptDir "..\backend\Araponga.Api\wwwroot\devportal\assets\images\diagrams"

Write-Host "📊 Gerando SVGs dos diagramas Mermaid..." -ForegroundColor Cyan

# Verificar se mermaid-cli está instalado
try {
    $null = Get-Command mmdc -ErrorAction Stop
    Write-Host "✅ mermaid-cli encontrado" -ForegroundColor Green
} catch {
    Write-Host "❌ mermaid-cli não encontrado. Instale com: npm install -g @mermaid-js/mermaid-cli" -ForegroundColor Red
    exit 1
}

# Gerar SVGs para cada arquivo .mmd
$mmdFiles = Get-ChildItem -Path $diagramsDir -Filter "*.mmd"
$generated = 0

foreach ($file in $mmdFiles) {
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    $outputFile = Join-Path $diagramsDir "$baseName.svg"
    
    Write-Host "   Gerando: $baseName.svg..." -NoNewline
    
    & mmdc -i $file.FullName -o $outputFile -e svg -t dark -b transparent 2>&1 | Out-Null
    
    if (Test-Path $outputFile) {
        Write-Host " ✅" -ForegroundColor Green
        $generated++
    } else {
        Write-Host " ❌" -ForegroundColor Red
    }
}

Write-Host "`n✅ $generated/$($mmdFiles.Count) diagramas gerados com sucesso!" -ForegroundColor Green
Write-Host "📁 SVGs salvos em: $diagramsDir" -ForegroundColor Cyan