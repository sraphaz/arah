/**
 * Script para gerar imagens SVG dos diagramas Mermaid
 * Executa: node scripts/generate-mermaid-diagrams.js
 */

const fs = require('fs');
const path = require('path');

const DEVPORTAL_PATH = path.join(__dirname, '../backend/Araponga.Api/wwwroot/devportal');
const INDEX_HTML = path.join(DEVPORTAL_PATH, 'index.html');
const OUTPUT_DIR = path.join(DEVPORTAL_PATH, 'assets/images/diagrams');

// Garantir que o diretório existe
if (!fs.existsSync(OUTPUT_DIR)) {
  fs.mkdirSync(OUTPUT_DIR, { recursive: true });
}

// Ler o HTML
const htmlContent = fs.readFileSync(INDEX_HTML, 'utf8');

// Extrair todos os diagramas Mermaid
const mermaidRegex = /<div class="mermaid"[^>]*data-diagram="([^"]+)"[^>]*>([\s\S]*?)<\/div>/g;
const diagrams = [];
let match;

while ((match = mermaidRegex.exec(htmlContent)) !== null) {
  const diagramId = match[1];
  const diagramCode = match[2].trim();
  
  // Limpar código: remover entidades HTML e tags XML
  const cleanedCode = diagramCode
    .replace(/&lt;/g, '<')
    .replace(/&gt;/g, '>')
    .replace(/&quot;/g, '"')
    .replace(/&amp;/g, '&')
    .replace(/&#x27;/g, "'")
    .replace(/&#39;/g, "'")
    .replace(/<\/?[a-z][^>]*>/gi, '') // Remove tags XML/HTML
    .replace(/<\/[a-zA-Z][^>]*$/gm, '') // Remove tags incompletas no final da linha
    .trim();
  
  diagrams.push({
    id: diagramId,
    code: cleanedCode,
    outputPath: path.join(OUTPUT_DIR, `${diagramId}.mmd`)
  });
}

// Salvar arquivos .mmd temporários para usar com mermaid-cli
console.log(`📊 Encontrados ${diagrams.length} diagramas Mermaid:`);
diagrams.forEach(diagram => {
  console.log(`   - ${diagram.id}`);
  fs.writeFileSync(diagram.outputPath, diagram.code, 'utf8');
});

console.log(`\n✅ Arquivos .mmd gerados em: ${OUTPUT_DIR}`);
console.log(`\n📝 Próximos passos:`);
console.log(`   1. Instalar mermaid-cli: npm install -g @mermaid-js/mermaid-cli`);
console.log(`   2. Executar: mmdc -i "${OUTPUT_DIR}/*.mmd" -o "${OUTPUT_DIR}" -e svg`);
console.log(`   3. Ou usar um script automatizado para gerar todos os SVGs`);