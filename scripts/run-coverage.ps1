#!/usr/bin/env pwsh
# Run tests with code coverage (matches CI). Output: backend/Araponga.Tests/TestResults/<guid>/coverage.cobertura.xml

$ErrorActionPreference = "Stop"
$resultsDir = "backend/Araponga.Tests/TestResults"

dotnet test backend/Araponga.Tests/Araponga.Tests.csproj `
  --configuration Release `
  --verbosity minimal `
  --collect:"XPlat Code Coverage" `
  --results-directory:$resultsDir `
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

$xml = Get-ChildItem -Path $resultsDir -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue | Select-Object -First 1
if ($xml) {
  $cov = [xml](Get-Content $xml.FullName -Raw)
  $lineRate = [double]$cov.coverage.GetAttribute("line-rate") * 100
  $branchRate = [double]$cov.coverage.GetAttribute("branch-rate") * 100
  Write-Host ""
  Write-Host "## Coverage summary" -ForegroundColor Cyan
  Write-Host "  Lines:    $([math]::Round($lineRate, 1))%"
  Write-Host "  Branches: $([math]::Round($branchRate, 1))%"
  Write-Host "  Report:   $($xml.FullName)"
}
