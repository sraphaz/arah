/**
 * Índice de busca para Wiki
 * Indexa documentos Markdown da pasta docs/
 */

import { readFile } from 'fs/promises';
import { join } from 'path';
import matter from 'gray-matter';
import { remark } from 'remark';
import remarkHtml from 'remark-html';
import remarkGfm from 'remark-gfm';
import { getTextContent } from './markdown';
import { getTitleFromFileName, getAllMarkdownFiles } from './file-utils';

export interface WikiSearchItem {
  id: string;
  title: string;
  content: string;
  url: string;
  category: string;
  description?: string;
}

/**
 * Gera índice de busca para todos os documentos da Wiki (recursivo)
 * Indexa arquivos .md em docs/ e subpastas (ex: docs/api/)
 */
export async function generateWikiIndex(): Promise<WikiSearchItem[]> {
  const docsPath = join(process.cwd(), '..', '..', 'docs');
  const index: WikiSearchItem[] = [];

  try {
    // Usa getAllMarkdownFiles para indexar recursivamente
    const allMdFiles = await getAllMarkdownFiles(docsPath);

    for (const relativePath of allMdFiles) {
      try {
        // relativePath já está sem .md, precisa adicionar de volta para ler
        const filePath = join(docsPath, relativePath + '.md');
        const fileContents = await readFile(filePath, 'utf8');
        const { content, data } = matter(fileContents);

        // Processa markdown para extrair texto
        const processedContent = await remark()
          .use(remarkHtml)
          .use(remarkGfm)
          .process(content);

        const htmlContent = processedContent.toString();
        const textContent = getTextContent(htmlContent);

        // Determina categoria baseado no caminho do arquivo
        const category = getCategoryFromPath(relativePath);
        // Slug é o caminho relativo sem .md (já está sem .md em relativePath)
        const slug = relativePath.replace(/\\/g, '/'); // Normaliza separadores de caminho

        index.push({
          id: slug,
          title: (data.title as string) || getTitleFromFileName(relativePath.split(/[/\\]/).pop() || ''),
          content: textContent.substring(0, 500), // Primeiros 500 caracteres para busca
          url: `/wiki/docs/${slug}`,
          category,
          description: data.description as string | undefined,
        });
      } catch (error) {
        console.error(`Error indexing ${relativePath}:`, error);
      }
    }
  } catch (error) {
    console.error('Error reading docs directory:', error);
  }

  return index;
}

/**
 * Determina categoria do documento baseado no caminho do arquivo
 */
function getCategoryFromPath(filePath: string): string {
  // Se está em subpasta api/, é API
  if (filePath.startsWith('api/')) {
    return 'API';
  }

  // Extrai nome do arquivo
  const fileName = filePath.split(/[/\\]/).pop() || '';

  if (fileName.startsWith('00_')) return 'Referência';
  if (fileName.startsWith('01_') || fileName.startsWith('02_') || fileName.startsWith('03_') || fileName.startsWith('04_') || fileName.startsWith('05_')) {
    return 'Visão e Produto';
  }
  if (fileName.startsWith('10_') || fileName.startsWith('11_') || fileName.startsWith('12_') || fileName.startsWith('13_')) {
    return 'Arquitetura';
  }
  if (fileName.startsWith('20_') || fileName.startsWith('21_') || fileName.startsWith('22_') || fileName.startsWith('23_')) {
    return 'Desenvolvimento';
  }
  if (fileName.startsWith('ONBOARDING_') || fileName === 'MENTORIA.md' || fileName === 'PRIORIZACAO_PROPOSTAS.md') {
    return 'Onboarding';
  }
  if (fileName.startsWith('SECURITY_')) {
    return 'Segurança';
  }
  if (fileName === 'PROJECT_STRUCTURE.md') {
    return 'Desenvolvimento';
  }
  if (fileName === 'CARTILHA_COMPLETA.md') {
    return 'Onboarding';
  }
  return 'Outros';
}
