/**
 * Script para gerar índice de busca no build time
 * Gera search-index.json como asset estático para uso em export estático
 *
 * Implementação standalone para não depender de TypeScript
 */

import { writeFile, readdir, readFile, mkdir } from 'fs/promises';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Importa dependências necessárias (devem estar no package.json)
import matter from 'gray-matter';
import { remark } from 'remark';
import remarkHtml from 'remark-html';
import remarkGfm from 'remark-gfm';

// Função para extrair texto de HTML
function getTextContent(html) {
  return html
    .replace(/<[^>]*>/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();
}

// Função para buscar recursivamente todos os arquivos .md
async function getAllMarkdownFiles(dir, basePath = '') {
  try {
    const files = [];
    const entries = await readdir(dir, { withFileTypes: true });

    for (const entry of entries) {
      const fullPath = basePath
        ? `${basePath}/${entry.name}`.replace(/\\/g, '/')
        : entry.name;

      if (entry.isDirectory()) {
        const subDir = join(dir, entry.name);
        const subFiles = await getAllMarkdownFiles(subDir, fullPath);
        files.push(...subFiles);
      } else if (entry.isFile() && entry.name.endsWith('.md')) {
        const relativePath = fullPath.replace(/\.md$/, '');
        files.push(relativePath);
      }
    }

    return files;
  } catch (error) {
    console.error(`Error reading directory ${dir}:`, error);
    return [];
  }
}

// Função para determinar categoria
function getCategoryFromPath(filePath) {
  if (filePath.startsWith('api/')) {
    return 'API';
  }

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

// Função para gerar título do arquivo
function getTitleFromFileName(fileName) {
  const fileNameWithoutExt = fileName.replace('.md', '');
  const titleWithoutPrefix = fileNameWithoutExt.replace(/^\d+_/, '');
  return titleWithoutPrefix.replace(/_/g, ' ');
}

// Função principal para gerar índice
async function generateWikiIndex() {
  const docsPath = join(__dirname, '..', '..', '..', 'docs');
  const index = [];

  try {
    const allMdFiles = await getAllMarkdownFiles(docsPath);

    for (const relativePath of allMdFiles) {
      try {
        const filePath = join(docsPath, relativePath + '.md');
        const fileContents = await readFile(filePath, 'utf8');
        const { content, data } = matter(fileContents);

        const processedContent = await remark()
          .use(remarkHtml)
          .use(remarkGfm)
          .process(content);

        const htmlContent = processedContent.toString();
        const textContent = getTextContent(htmlContent);

        const category = getCategoryFromPath(relativePath);
        const slug = relativePath.replace(/\\/g, '/');

        index.push({
          id: slug,
          title: (data.title) || getTitleFromFileName(relativePath.split(/[/\\]/).pop() || ''),
          content: textContent.substring(0, 500),
          url: `/wiki/docs/${slug}`,
          category,
          description: data.description,
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

async function main() {
  try {
    console.log('🔍 Gerando índice de busca...');

    const index = await generateWikiIndex();

    console.log(`✅ ${index.length} documentos indexados`);

    // Criar pasta public se não existir
    const publicDir = join(__dirname, '..', 'public');
    try {
      await readdir(publicDir);
    } catch {
      await mkdir(publicDir, { recursive: true });
    }

    // Salvar como JSON estático na pasta public
    const outputPath = join(publicDir, 'search-index.json');
    await writeFile(outputPath, JSON.stringify({ index }, null, 2), 'utf8');

    console.log(`✅ Índice salvo em: ${outputPath}`);
    console.log(`📊 Categorias: ${[...new Set(index.map(item => item.category))].join(', ')}`);
  } catch (error) {
    console.error('❌ Erro ao gerar índice de busca:', error);
    process.exit(1);
  }
}

main();
