/**
 * Script para verificar IDs duplicados no DevPortal
 */

const fs = require('fs');
const path = require('path');

const htmlPath = path.join(__dirname, '..', 'frontend', 'devportal', 'index.html');

try {
  const html = fs.readFileSync(htmlPath, 'utf8');
  const idRegex = /id=['"]([^'"]+)['"]/g;
  const ids = [];
  let match;
  
  while ((match = idRegex.exec(html)) !== null) {
    ids.push(match[1]);
  }
  
  const idMap = {};
  ids.forEach(id => {
    idMap[id] = (idMap[id] || 0) + 1;
  });
  
  const duplicates = Object.entries(idMap)
    .filter(([id, count]) => count > 1)
    .sort((a, b) => b[1] - a[1]);
  
  if (duplicates.length > 0) {
    console.log('❌ IDs duplicados encontrados:');
    duplicates.forEach(([id, count]) => {
      console.log(`  - ${id}: ${count} ocorrências`);
    });
    process.exit(1);
  } else {
    console.log('✅ Nenhum ID duplicado encontrado');
    process.exit(0);
  }
} catch (error) {
  console.error('❌ Erro ao verificar IDs:', error.message);
  process.exit(1);
}
