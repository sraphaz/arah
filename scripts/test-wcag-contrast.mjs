/**
 * Script para validar contraste WCAG AA
 * Calcula ratios de contraste e valida conformidade
 */

/**
 * Converte hex para RGB
 */
function hexToRgb(hex) {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
  return result
    ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16),
      }
    : null;
}

/**
 * Calcula luminância relativa
 */
function getLuminance(r, g, b) {
  const [rs, gs, bs] = [r, g, b].map(val => {
    val = val / 255;
    return val <= 0.03928 ? val / 12.92 : Math.pow((val + 0.055) / 1.055, 2.4);
  });
  return 0.2126 * rs + 0.7152 * gs + 0.0722 * bs;
}

/**
 * Calcula ratio de contraste WCAG
 */
function getContrastRatio(color1, color2) {
  const rgb1 = hexToRgb(color1);
  const rgb2 = hexToRgb(color2);
  
  if (!rgb1 || !rgb2) {
    return 0;
  }
  
  const lum1 = getLuminance(rgb1.r, rgb1.g, rgb1.b);
  const lum2 = getLuminance(rgb2.r, rgb2.g, rgb2.b);
  
  const lighter = Math.max(lum1, lum2);
  const darker = Math.min(lum1, lum2);
  
  return (lighter + 0.05) / (darker + 0.05);
}

/**
 * Valida se contraste atende WCAG AA
 */
function meetsWCAGAA(ratio, isLargeText = false) {
  // WCAG AA: 4.5:1 para texto normal, 3:1 para texto grande (18px+ ou 14px+ bold)
  const threshold = isLargeText ? 3 : 4.5;
  return ratio >= threshold;
}

/**
 * Testes de contraste do DevPortal
 */
function testDevPortalContrast() {
  console.log('🧪 Testando contraste WCAG AA do DevPortal...\n');
  
  const tests = [
    // Light Mode
    {
      name: 'Light: Text sobre bg',
      fg: '#1a3d2e',
      bg: '#f1f8f4',
      expected: 7.2,
      isLargeText: false,
    },
    {
      name: 'Light: Text-muted sobre bg',
      fg: '#2d4a3f',
      bg: '#f1f8f4',
      expected: 5.8,
      isLargeText: false,
    },
    {
      name: 'Light: Text-subtle sobre bg',
      fg: '#4a6b5f',
      bg: '#f1f8f4',
      expected: 4.6,
      isLargeText: false,
    },
    {
      name: 'Light: Link sobre bg',
      fg: '#0066cc',
      bg: '#f1f8f4',
      expected: 4.8,
      isLargeText: false,
    },
    // Dark Mode
    {
      name: 'Dark: Text sobre bg',
      fg: '#e8edf2',
      bg: '#0a0e12',
      expected: 13.5,
      isLargeText: false,
    },
    {
      name: 'Dark: Text-muted sobre bg',
      fg: '#c5d1de',
      bg: '#0a0e12',
      expected: 10.2,
      isLargeText: false,
    },
    {
      name: 'Dark: Text-subtle sobre bg',
      fg: '#a0afbc',
      bg: '#0a0e12',
      expected: 7.8,
      isLargeText: false,
    },
    {
      name: 'Dark: Link sobre bg',
      fg: '#7dd3ff',
      bg: '#0a0e12',
      expected: 6.8,
      isLargeText: false,
    },
  ];
  
  let passed = 0;
  let failed = 0;
  
  tests.forEach(test => {
    const ratio = getContrastRatio(test.fg, test.bg);
    const meetsAA = meetsWCAGAA(ratio, test.isLargeText);
    const deviation = Math.abs(ratio - test.expected);
    // Aceita se passa WCAG AA e está dentro de margem razoável (20% de diferença)
    const tolerance = test.expected * 0.2;
    
    if (meetsAA && deviation < tolerance) {
      console.log(`✅ ${test.name}`);
      console.log(`   Ratio: ${ratio.toFixed(2)}:1 (esperado: ~${test.expected}:1)`);
      console.log(`   WCAG AA: ${meetsAA ? '✅ Passa' : '❌ Falha'}\n`);
      passed++;
    } else {
      console.log(`❌ ${test.name}`);
      console.log(`   Ratio: ${ratio.toFixed(2)}:1 (esperado: ~${test.expected}:1)`);
      console.log(`   WCAG AA: ${meetsAA ? '✅ Passa' : '❌ Falha'}\n`);
      failed++;
    }
  });
  
  console.log(`\n📊 Resultado: ${passed} passaram, ${failed} falharam`);
  
  if (failed > 0) {
    process.exit(1);
  }
  
  return { passed, failed };
}

// Executa testes
try {
  testDevPortalContrast();
  console.log('\n✅ Todos os testes de contraste passaram!');
  process.exit(0);
} catch (error) {
  console.error('\n❌ Erro ao executar testes:', error);
  process.exit(1);
}
