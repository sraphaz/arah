#!/usr/bin/env node
/**
 * Script para calcular dinamicamente o número total de fases do projeto
 *
 * Conta arquivos FASE*.md no diretório docs/backlog-api/ (excluindo subdiretórios)
 *
 * Uso:
 *   node scripts/get-phase-count.mjs
 *   ou
 *   npm run get-phase-count (se adicionado ao package.json)
 */

import { readdir } from 'fs/promises';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);
const projectRoot = join(__dirname, '..');

async function getPhaseCount() {
  try {
    const backlogPath = join(projectRoot, 'docs', 'backlog-api');
    const files = await readdir(backlogPath, { withFileTypes: true });

    // Filtra apenas arquivos FASE*.md na raiz do diretório (não em subdiretórios)
    const phaseFiles = files
      .filter(dirent => dirent.isFile())
      .map(dirent => dirent.name)
      .filter(filename => /^FASE\d+\.md$/.test(filename));

    const count = phaseFiles.length;

    // Output formatado para uso em scripts ou CI/CD
    if (process.argv.includes('--json')) {
      console.log(JSON.stringify({ totalPhases: count }));
    } else {
      console.log(count);
    }

    return count;
  } catch (error) {
    console.error('Erro ao contar fases:', error);
    process.exit(1);
  }
}

getPhaseCount();
