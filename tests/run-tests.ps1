<#
    run-tests.ps1
    -------------
    Ejecuta las 3 suites (Unit, Integration, E2E) del proyecto MauiApp1 y
    deja los resultados en tests\TestResults\.

    Uso:
       .\run-tests.ps1                 # Unit + Integration (rápido)
       .\run-tests.ps1 -IncludeE2E     # Incluye E2E (requiere Appium + emulador)
       .\run-tests.ps1 -Live           # Activa smoke tests contra backend real
#>
param(
    [switch]$IncludeE2E,
    [switch]$Live
)

$ErrorActionPreference = 'Stop'
$root        = $PSScriptRoot
$resultsDir  = Join-Path $root 'TestResults'
$unitProj    = Join-Path $root 'MauiApp1.UnitTests\MauiApp1.UnitTests.csproj'
$intProj     = Join-Path $root 'MauiApp1.IntegrationTests\MauiApp1.IntegrationTests.csproj'
$e2eProj     = Join-Path $root 'MauiApp1.E2ETests\MauiApp1.E2ETests.csproj'

New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
if ($Live) { $env:RUN_LIVE_TESTS = '1' }

function Invoke-Test($proj, $label) {
    Write-Host "`n=== Ejecutando $label ===" -ForegroundColor Cyan
    dotnet test $proj `
        --logger "trx;LogFileName=$label.trx" `
        --logger "console;verbosity=normal" `
        --results-directory $resultsDir `
        --collect:"XPlat Code Coverage"
}

Invoke-Test $unitProj 'UnitTests'
Invoke-Test $intProj  'IntegrationTests'
if ($IncludeE2E) { Invoke-Test $e2eProj 'E2ETests' }

Write-Host "`nResultados: $resultsDir" -ForegroundColor Green
