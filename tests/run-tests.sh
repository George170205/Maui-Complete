#!/usr/bin/env bash
# run-tests.sh — equivalente a run-tests.ps1 para Linux / macOS / WSL.
#
#   ./run-tests.sh             # Unit + Integration
#   ./run-tests.sh --e2e       # Incluye E2E (requiere Appium + emulador)
#   ./run-tests.sh --live      # Activa smoke tests contra backend real

set -euo pipefail
cd "$(dirname "$0")"

INCLUDE_E2E=0
LIVE=0
for a in "$@"; do
  case "$a" in
    --e2e|-e) INCLUDE_E2E=1 ;;
    --live|-l) LIVE=1 ;;
  esac
done

mkdir -p TestResults
[[ $LIVE -eq 1 ]] && export RUN_LIVE_TESTS=1

run_suite() {
  local proj=$1 label=$2
  echo
  echo "=== Ejecutando $label ==="
  dotnet test "$proj" \
    --logger "trx;LogFileName=${label}.trx" \
    --logger "console;verbosity=normal" \
    --results-directory "TestResults" \
    --collect:"XPlat Code Coverage"
}

run_suite MauiApp1.UnitTests/MauiApp1.UnitTests.csproj               UnitTests
run_suite MauiApp1.IntegrationTests/MauiApp1.IntegrationTests.csproj IntegrationTests
[[ $INCLUDE_E2E -eq 1 ]] && run_suite MauiApp1.E2ETests/MauiApp1.E2ETests.csproj E2ETests

echo
echo "Resultados: $(pwd)/TestResults"
