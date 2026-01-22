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

  // Limpar código: remover entidades HTML e tags XML de forma segura
  // Usar uma função de desescape que processa todas as entidades de uma vez
  // Isso evita problemas de double escaping
  function unescapeHtmlEntities(text) {
    // Primeiro, remover tags XML/HTML antes de processar entidades
    // Isso evita que tags contenham entidades que seriam processadas incorretamente
    let result = text.replace(/<\/?[a-z][^>]*>/gi, '');
    result = result.replace(/<\/[a-zA-Z][^>]*$/gm, '');
    
    // Agora processar entidades HTML de forma segura usando uma única regex
    // com callback para evitar double unescaping
    // A regex captura todas as entidades conhecidas de uma vez
    result = result.replace(/&(?:amp|lt|gt|quot|#x27|#39|#x2F|#x60|#x3D);/g, (match) => {
      switch (match) {
        case '&amp;': return '&';
        case '&lt;': return '<';
        case '&gt;': return '>';
        case '&quot;': return '"';
        case '&#x27;': case '&#39;': return "'";
        case '&#x2F;': return '/';
        case '&#x60;': return '`';
        case '&#x3D;': return '=';
        default: return match; // Se não reconhecer, mantém original para evitar corrupção
      }
    });
    
    return result;
  }

  const cleanedCode = unescapeHtmlEntities(diagramCode).trim();

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
