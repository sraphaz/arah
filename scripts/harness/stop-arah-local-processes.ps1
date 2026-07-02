#Requires -Version 5.1
<#
.SYNOPSIS
  Encerra processos locais Arah.Bff / Arah.Api que travam dotnet build (MSB3027).

  Usado pelo harness antes de comandos dotnet build/test na solution completa.
  Fail-open: erros ao encerrar processos não bloqueiam o harness.

.EXAMPLE
  ./stop-arah-local-processes.ps1
  ./stop-arah-local-processes.ps1 -Json
#>
param(
    [switch]$Json
)

$ErrorActionPreference = 'SilentlyContinue'
$stopped = @()

foreach ($name in @('Arah.Bff', 'Arah.Api')) {
    Get-Process -Name $name -ErrorAction SilentlyContinue | ForEach-Object {
        try {
            Stop-Process -Id $_.Id -Force -ErrorAction Stop
            $stopped += @{ name = $name; pid = $_.Id }
        } catch {}
    }
}

if ($stopped.Count -gt 0) {
    Start-Sleep -Milliseconds 500
}

if ($Json) {
    @{ stopped = $stopped; count = $stopped.Count } | ConvertTo-Json -Compress
} elseif ($stopped.Count -gt 0) {
    $ids = ($stopped | ForEach-Object { "$($_.name) ($($_.pid))" }) -join ', '
    Write-Host "Stopped local stack processes: $ids"
}
