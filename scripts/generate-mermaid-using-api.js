/**
 * Script para gerar SVGs dos diagramas Mermaid usando API online (mermaid.ink)
 * Alternativa quando mermaid-cli não está disponível
 */

const fs = require('fs');
const path = require('path');
const https = require('https');

const DIAGRAMS_DIR = path.join(__dirname, '../backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams');

// Função para codificar diagrama em base64 (para mermaid.ink)
function encodeMermaid(diagramCode) {
  return Buffer.from(diagramCode).toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
}

// Função para fazer download do SVG
function downloadSVG(diagramCode, outputPath) {
  return new Promise((resolve, reject) => {
    const encoded = encodeMermaid(diagramCode);
    const url = `https://mermaid.ink/svg/${encoded}`;
    
    const file = fs.createWriteStream(outputPath);
    
    https.get(url, (response) => {
      if (response.statusCode !== 200) {
        reject(new Error(`Erro ao baixar SVG: ${response.statusCode}`));
        return;
      }
      
      response.pipe(file);
      file.on('finish', () => {
        file.close();
        resolve();
      });
    }).on('error', (err) => {
      fs.unlink(outputPath, () => {}); // Remove arquivo parcial
      reject(err);
    });
  });
}

// Ler todos os arquivos .mmd
const mmdFiles = fs.readdirSync(DIAGRAMS_DIR).filter(f => f.endsWith('.mmd'));

console.log(`📊 Gerando ${mmdFiles.length} SVGs usando API online (mermaid.ink)...`);

let generated = 0;
let failed = 0;

async function generateAll() {
  for (const mmdFile of mmdFiles) {
    const mmdPath = path.join(DIAGRAMS_DIR, mmdFile);
    const diagramCode = fs.readFileSync(mmdPath, 'utf8');
    const svgPath = mmdPath.replace('.mmd', '.svg');
    const diagramId = path.basename(mmdFile, '.mmd');
    
    process.stdout.write(`   ${diagramId}... `);
    
    try {
      await downloadSVG(diagramCode, svgPath);
      console.log('✅');
      generated++;
      
      // Aguardar um pouco para não sobrecarregar a API
      await new Promise(resolve => setTimeout(resolve, 500));
    } catch (err) {
      console.log(`❌ ${err.message}`);
      failed++;
    }
  }
  
  console.log(`\n✅ ${generated}/${mmdFiles.length} SVGs gerados com sucesso!`);
  if (failed > 0) {
    console.log(`⚠️  ${failed} falharam. Use mermaid-cli como alternativa.`);
  }
}

generateAll().catch(console.error);