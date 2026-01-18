/**
 * Configuração dinâmica do projeto
 * Centraliza informações que podem mudar com o planejamento
 *
 * IMPORTANTE: Este arquivo deve ser atualizado quando houver mudanças no planejamento.
 * Para o número de fases, use getTotalPhases() que calcula dinamicamente.
 */

import { readdir } from 'fs/promises';
import { join } from 'path';

let phaseCountCache: number | null = null;

/**
 * Calcula o número total de fases do projeto contando arquivos FASE*.md
 *
 * @returns Número total de fases
 */
export async function getTotalPhases(): Promise<number> {
  if (phaseCountCache !== null) {
    return phaseCountCache;
  }

  try {
    // A partir de frontend/wiki/lib, precisamos subir 3 níveis para chegar à raiz
    const projectRoot = join(process.cwd(), '..', '..', '..');
    const backlogPath = join(projectRoot, 'docs', 'backlog-api');
    const files = await readdir(backlogPath, { withFileTypes: true });

    // Filtra apenas arquivos FASE*.md na raiz do diretório (não em subdiretórios)
    const phaseFiles = files
      .filter(dirent => dirent.isFile())
      .map(dirent => dirent.name)
      .filter(filename => /^FASE\d+\.md$/.test(filename));

    phaseCountCache = phaseFiles.length;
    return phaseCountCache;
  } catch (error) {
    console.error('Erro ao contar fases:', error);
    // Fallback: retorna um valor padrão se não conseguir calcular
    return 29; // Valor atual conhecido, mas deve ser atualizado se mudar
  }
}

/**
 * Retorna uma string formatada para exibição do número de fases
 * Usado quando você não pode usar getTotalPhases() assíncrono
 *
 * @param count Número de fases (opcional, calcula se não fornecido)
 * @returns String formatada
 */
export function getPhasesDescription(count?: number): string {
  if (count !== undefined) {
    return `${count} fases`;
  }
  // Se não for fornecido, retorna descrição genérica
  return "fases do backlog";
}
