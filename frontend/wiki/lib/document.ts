/**
 * Utilitários para processamento de documentos Markdown
 * Centraliza lógica de leitura e processamento de documentos
 */

import { readFile, stat } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";
import {
  getTextContent,
  processMarkdownLinks,
  extractFirstH1,
  addHeadingIds,
} from "./markdown";
import { getTitleFromFileName } from "./file-utils";

export interface DocumentContent {
  content: string;
  frontMatter: Record<string, unknown>;
  title: string;
}

export interface YamlContent {
  content: string;
  title: string;
  isYaml: true;
  fileName: string;
}

export interface ProcessMarkdownOptions {
  /** Base path para reescrita de links (padrão: "/wiki"). */
  basePath?: string;
  /**
   * Substitui blocos `<pre><code class="language-mermaid">` por placeholders
   * `<div data-mermaid-code="...">` renderizados por `MermaidContent`.
   */
  processMermaid?: boolean;
}

/** Resolve o caminho absoluto de um arquivo dentro de `docs/`. */
function resolveDocPath(filePath: string): string {
  return join(process.cwd(), "..", "..", "docs", filePath);
}

/** Verifica se o arquivo existe (evita logs de erro para caminhos ausentes). */
async function fileExists(absolutePath: string): Promise<boolean> {
  try {
    await stat(absolutePath);
    return true;
  } catch {
    return false;
  }
}

/**
 * Substitui blocos Mermaid por placeholders para renderização no cliente.
 */
function processMermaidBlocks(html: string): string {
  return html.replace(
    /<pre><code class="language-mermaid">([\s\S]*?)<\/code><\/pre>/gi,
    (_match, code) => {
      const encodedCode = encodeURIComponent(code.trim());
      return `<div data-mermaid-code="${encodedCode}"></div>`;
    }
  );
}

/**
 * Processa conteúdo Markdown e retorna HTML processado.
 * Retorna `null` silenciosamente quando o arquivo não existe.
 */
export async function processMarkdownContent(
  filePath: string,
  options: ProcessMarkdownOptions = {}
): Promise<DocumentContent | null> {
  const { basePath = "/wiki", processMermaid = false } = options;
  const docsPath = resolveDocPath(filePath);

  if (!(await fileExists(docsPath))) {
    return null;
  }

  try {
    const fileContents = await readFile(docsPath, "utf8");
    const { content, data } = matter(fileContents);

    // Processa markdown para HTML
    const processedContent = await remark()
      .use(remarkHtml)
      .use(remarkGfm)
      .process(content);

    let htmlContent = processedContent.toString();

    // Extrai primeiro H1 para usar como título e remove-o do conteúdo
    const { title: firstH1Title, content: contentWithoutH1 } = extractFirstH1(htmlContent);
    htmlContent = contentWithoutH1;

    // Adiciona IDs únicos aos headings (h2-h4)
    htmlContent = addHeadingIds(htmlContent);

    // Processa links no HTML renderizado para incluir basePath
    htmlContent = processMarkdownLinks(htmlContent, basePath);

    // Processa blocos Mermaid quando solicitado
    if (processMermaid) {
      htmlContent = processMermaidBlocks(htmlContent);
    }

    // Gera título: frontmatter > primeiro H1 > nome do arquivo
    const fileName = filePath.split("/").pop() || "";
    const fallbackTitle = getTitleFromFileName(fileName);
    const title = (data.title as string) || firstH1Title || fallbackTitle;

    return {
      content: htmlContent,
      frontMatter: data,
      title,
    };
  } catch (error) {
    console.error(`Error reading ${filePath}:`, error);
    return null;
  }
}

/**
 * Lê um arquivo YAML de `docs/` e retorna seu conteúdo bruto e título.
 * Retorna `null` silenciosamente quando o arquivo não existe.
 */
export async function getYamlContent(filePath: string): Promise<YamlContent | null> {
  const docsPath = resolveDocPath(filePath);

  if (!(await fileExists(docsPath))) {
    return null;
  }

  try {
    const fileContents = await readFile(docsPath, "utf8");
    const fileName = filePath.split("/").pop() || "";
    const fileNameWithoutExt = fileName.replace(/\.(yaml|yml)$/, "");
    const title = getTitleFromFileName(fileNameWithoutExt);

    return {
      content: fileContents,
      title,
      isYaml: true,
      fileName,
    };
  } catch (error) {
    console.error(`Error reading YAML file ${filePath}:`, error);
    return null;
  }
}
