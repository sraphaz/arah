/**
 * Script para substituir divs .mermaid por tags <img> após gerar SVGs
 * Executa após: node scripts/generate-mermaid-diagrams.js && [gerar SVGs]
 */

const fs = require('fs');
const path = require('path');

const DEVPORTAL_PATH = path.join(__dirname, '../backend/Araponga.Api/wwwroot/devportal');
const INDEX_HTML = path.join(DEVPORTAL_PATH, 'index.html');
const DIAGRAMS_DIR = path.join(DEVPORTAL_PATH, 'assets/images/diagrams');

// Ler o HTML
let htmlContent = fs.readFileSync(INDEX_HTML, 'utf8');

// Substituir cada div .mermaid por uma img
const mermaidDivRegex = /<div class="mermaid"[^>]*data-diagram="([^"]+)"[^>]*>[\s\S]*?<\/div>/g;

htmlContent = htmlContent.replace(mermaidDivRegex, (match, diagramId) => {
  const svgPath = `./assets/images/diagrams/${diagramId}.svg`;
  return `<img src="${svgPath}" alt="Diagrama de sequência: ${diagramId}" class="mermaid-diagram" data-diagram="${diagramId}" style="max-width: 100%; height: auto; display: block; margin: 1rem 0;" />`;
});

// Salvar HTML atualizado
fs.writeFileSync(INDEX_HTML, htmlContent, 'utf8');

console.log('✅ HTML atualizado: divs .mermaid substituídas por <img>');
console.log(`📁 SVGs devem estar em: ${DIAGRAMS_DIR}`);
console.log(`\n💡 Lembre-se de gerar os SVGs primeiro usando mermaid-cli ou outro método!`);