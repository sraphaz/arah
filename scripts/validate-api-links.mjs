/**
 * Script para validar links da API após subdivisão
 * Verifica se todos os links internos funcionam
 */

import { readFile } from 'fs/promises';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const docsPath = join(__dirname, '..', 'docs');

async function validateLinks() {
  const brokenLinks = [];
  const filesToCheck = [
    'api/60_API_LÓGICA_NEGÓCIO_INDEX.md',
    '60_API_LÓGICA_NEGÓCIO.md',
    '00_INDEX.md',
    'ONBOARDING_DEVELOPERS.md',
    'ONBOARDING_ANALISTAS_FUNCIONAIS.md',
  ];

  for (const file of filesToCheck) {
    try {
      const filePath = join(docsPath, file);
      const content = await readFile(filePath, 'utf8');
      
      // Busca links internos (formato [text](./path) ou [text](../path))
      const linkRegex = /\[([^\]]+)\]\((\.\/|\.\.\/)?([^)]+\.md)\)/g;
      let match;
      
      while ((match = linkRegex.exec(content)) !== null) {
        const [, text, prefix, targetPath] = match;
        
        // Resolve caminho relativo
        const currentDir = dirname(filePath);
        const targetFile = join(currentDir, prefix || './', targetPath);
        
        try {
          await readFile(targetFile, 'utf8');
        } catch (error) {
          brokenLinks.push({
            file,
            link: match[0],
            target: targetPath,
            text,
            error: error.message,
          });
        }
      }
    } catch (error) {
      console.error(`Erro ao ler ${file}:`, error.message);
    }
  }

  if (brokenLinks.length > 0) {
    console.log('❌ Links quebrados encontrados:');
    brokenLinks.forEach(({ file, link, target, error }) => {
      console.log(`  - ${file}: ${link} → ${target}`);
      console.log(`    Erro: ${error}`);
    });
    process.exit(1);
  } else {
    console.log('✅ Todos os links estão funcionando');
    process.exit(0);
  }
}

validateLinks().catch(error => {
  console.error('❌ Erro ao validar links:', error);
  process.exit(1);
});
