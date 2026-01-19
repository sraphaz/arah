/**
 * Script para testar uso de variáveis CSS vs cores hardcoded
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
];

// Padrões para detectar cores hardcoded
const hardcodedPatterns = [
  /#[0-9a-fA-F]{3,6}(?!['"])/g, // Hex colors (não em strings)
  /rgba?\([^)]+\)(?!['"])/g, // RGB/RGBA (não em strings)
  /hsla?\([^)]+\)(?!['"])/g, // HSL/HSLA (não em strings)
];

// Cores permitidas (valores especiais ou em comentários)
const allowedPatterns = [
  /\/\*.*?#[0-9a-fA-F]{3,6}.*?\*\//g, // Comentários
  /url\([^)]+\)/g, // URLs
  /var\(--[^)]+\)/g, // Variáveis CSS
];

function extractHardcodedColors(content, filePath) {
  const issues = [];
  const lines = content.split('\n');
  
  lines.forEach((line, index) => {
    // Pula comentários completos
    if (line.trim().startsWith('/*') || line.trim().startsWith('*')) {
      return;
    }
    
    // Ignora definições de variáveis CSS (são esperadas)
    if (line.trim().startsWith('--')) {
      return;
    }
    
    // Verifica cada padrão
    hardcodedPatterns.forEach(pattern => {
      const matches = line.match(pattern);
      if (matches) {
        matches.forEach(match => {
          // Verifica se está em contexto permitido
          const isAllowed = allowedPatterns.some(allowed => {
            const allowedMatch = line.match(allowed);
            return allowedMatch && allowedMatch.some(a => line.indexOf(match) >= line.indexOf(a) && 
              line.indexOf(match) <= line.indexOf(a) + a.length);
          });
          
          // Verifica se é variável CSS
          if (match.includes('var(--') || match.includes('var( --')) {
            return; // É variável, não é hardcoded
          }
          
          // Ignora se está em comentário inline
          if (line.indexOf(match) < line.indexOf('/*')) {
            return;
          }
          
          // Ignora se está definindo uma variável CSS (ex: --color: #fff)
          if (line.includes('--') && line.indexOf('--') < line.indexOf(match)) {
            return;
          }
          
          if (!isAllowed) {
            issues.push({
              file: filePath,
              line: index + 1,
              color: match,
              code: line.trim(),
            });
          }
        });
      }
    });
  });
  
  return issues;
}

async function testCSSVariables() {
  console.log('🧪 Testando uso de variáveis CSS vs cores hardcoded...\n');
  
  let totalIssues = 0;
  const allIssues = [];
  
  for (const cssFile of cssFiles) {
    try {
      const filePath = join(__dirname, '..', cssFile);
      const content = await readFile(filePath, 'utf8');
      const issues = extractHardcodedColors(content, cssFile);
      
      if (issues.length > 0) {
        console.log(`⚠️  ${cssFile}: ${issues.length} cores hardcoded encontradas`);
        issues.forEach(issue => {
          console.log(`   Linha ${issue.line}: ${issue.color}`);
          console.log(`   ${issue.code.substring(0, 80)}...`);
        });
        console.log('');
        totalIssues += issues.length;
        allIssues.push(...issues);
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

testCSSVariables().catch(error => {
  console.error('❌ Erro ao executar testes:', error);
  process.exit(1);
});
