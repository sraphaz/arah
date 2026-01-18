#!/bin/bash
# Verificação de conformidade de design
# Uso: ./scripts/check-design-compliance.sh

set -e

echo "🔍 Verificando conformidade de design..."
echo ""

ERRORS=0
WARNINGS=0

# Cores hardcoded em Tailwind arbitrárias
echo "📋 Verificando Tailwind arbitrárias com cores..."
HARDCODED_ARBITRARIES=$(grep -rn "dark:bg-\[#" frontend/wiki/app/globals.css frontend/wiki/components/ 2>/dev/null || true)
if [ -n "$HARDCODED_ARBITRARIES" ]; then
  echo "❌ Cores hardcoded encontradas (dark:bg-[#...]):"
  echo "$HARDCODED_ARBITRARIES"
  ERRORS=$((ERRORS + 1))
else
  echo "✅ Nenhuma Tailwind arbitrária com cores encontrada"
fi

TEXT_ARBITRARIES=$(grep -rn "text-\[#" frontend/wiki/app/globals.css frontend/wiki/components/ 2>/dev/null || true)
if [ -n "$TEXT_ARBITRARIES" ]; then
  echo "❌ Cores hardcoded encontradas (text-[#...]):"
  echo "$TEXT_ARBITRARIES"
  ERRORS=$((ERRORS + 1))
fi

echo ""

# Valores hex/rgb diretos em CSS
echo "📋 Verificando valores hex/rgb diretos..."
HEX_VALUES=$(grep -rnE ":\s*#[0-9a-fA-F]{6}" frontend/wiki/app/globals.css 2>/dev/null | grep -v "^\s*--" | grep -v "^[^:]*:\s*/\*" || true)
if [ -n "$HEX_VALUES" ]; then
  echo "⚠️  Valores hex encontrados (verificar se são em definição de variáveis):"
  echo "$HEX_VALUES" | head -5
  WARNINGS=$((WARNINGS + 1))
fi

RGB_VALUES=$(grep -rnE ":\s*rgba?\([0-9]+," frontend/wiki/app/globals.css 2>/dev/null | grep -v "^\s*--" | grep -v "^[^:]*:\s*/\*" || true)
if [ -n "$RGB_VALUES" ]; then
  echo "⚠️  Valores rgb/rgba encontrados (verificar se são em definição de variáveis):"
  echo "$RGB_VALUES" | head -5
  WARNINGS=$((WARNINGS + 1))
fi

echo ""

# Resumo
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📊 Resumo:"
echo "   Erros: $ERRORS"
echo "   Avisos: $WARNINGS"

if [ $ERRORS -gt 0 ]; then
  echo ""
  echo "❌ Falhas de conformidade encontradas!"
  echo "   Consulte docs/CURSOR_DESIGN_RULES.md para diretrizes"
  exit 1
elif [ $WARNINGS -gt 0 ]; then
  echo ""
  echo "⚠️  Avisos encontrados (verificar se são válidos)"
  exit 0
else
  echo ""
  echo "✅ Conformidade de design OK!"
  exit 0
fi
