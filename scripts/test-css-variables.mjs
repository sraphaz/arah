/**
 * Script para testar uso de variáveis CSS vs cores hardcoded
 * em regras de componente (ignora definições de tokens e efeitos visuais).
 */

import { readFile } from 'fs/promises';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const cssFiles = [
  'frontend/devportal/assets/css/devportal.css',
  'frontend/devportal/assets/css/sidebar-modern.css',
  'frontend/devportal/assets/css/content-typography.css',
  'frontend/devportal/assets/css/mobile-menu.css',
];

const hardcodedPatterns = [
  /#[0-9a-fA-F]{3,6}(?!['"])/g,
  /rgba?\([^)]+\)(?!['"])/g,
  /hsla?\([^)]+\)(?!['"])/g,
];

const allowedPropertyContexts = [
  /box-shadow\s*:/,
  /text-shadow\s*:/,
  /filter\s*:/,
  /backdrop-filter\s*:/,
  /-webkit-backdrop-filter\s*:/,
  /linear-gradient/,
  /radial-gradient/,
  /conic-gradient/,
  /url\(/,
];

function isInsideTokenDefinition(lines, lineIndex) {
  for (let i = lineIndex; i >= 0; i -= 1) {
    const line = lines[i].trim();
    if (line.startsWith('--')) {
      return true;
    }
    if (line === ':root {' || line === ':root.dark {' || line.startsWith('@')) {
      return false;
    }
    if (line.includes('{') && !line.startsWith('--')) {
      return false;
    }
  }
  return false;
}

function isAllowedVisualEffect(line) {
  return allowedPropertyContexts.some((pattern) => pattern.test(line));
}

function extractHardcodedColors(content, filePath) {
  const issues = [];
  const lines = content.split('\n');

  lines.forEach((line, index) => {
    const codeWithoutComments = line.replace(/\/\*.*?\*\//g, '');
    if (codeWithoutComments.trim().startsWith('/*') || codeWithoutComments.trim().startsWith('*')) {
      return;
    }

    if (codeWithoutComments.trim().startsWith('--')) {
      return;
    }

    if (isInsideTokenDefinition(lines, index)) {
      return;
    }

    if (isAllowedVisualEffect(codeWithoutComments)) {
      return;
    }

    hardcodedPatterns.forEach((pattern) => {
      const matches = codeWithoutComments.match(pattern);
      if (!matches) {
        return;
      }

      matches.forEach((match) => {
        if (match.includes('var(--') || match.includes('var( --')) {
          return;
        }

        issues.push({
          file: filePath,
          line: index + 1,
          color: match,
          code: line.trim(),
        });
      });
    });
  });

  return issues;
}

async function testCSSVariables() {
  console.log('🧪 Testando uso de variáveis CSS vs cores hardcoded...\n');

  let totalIssues = 0;

  for (const cssFile of cssFiles) {
    try {
      const filePath = join(__dirname, '..', cssFile);
      const content = await readFile(filePath, 'utf8');
      const issues = extractHardcodedColors(content, cssFile);

      if (issues.length > 0) {
        console.log(`⚠️  ${cssFile}: ${issues.length} cores hardcoded encontradas`);
        issues.forEach((issue) => {
          console.log(`   Linha ${issue.line}: ${issue.color}`);
          console.log(`   ${issue.code.substring(0, 80)}...`);
        });
        console.log('');
        totalIssues += issues.length;
      } else {
        console.log(`✅ ${cssFile}: Nenhuma cor hardcoded encontrada`);
      }
    } catch (error) {
      console.error(`❌ Erro ao ler ${cssFile}:`, error.message);
    }
  }

  console.log(`\n📊 Resultado: ${totalIssues} cores hardcoded encontradas`);

  if (totalIssues > 0) {
    console.log('\n💡 Recomendação: Substitua por variáveis CSS (ex: var(--text), var(--accent))');
    process.exit(1);
  }

  console.log('\n✅ Todos os arquivos usam variáveis CSS!');
  process.exit(0);
}

testCSSVariables().catch((error) => {
  console.error('❌ Erro ao executar testes:', error);
  process.exit(1);
});
