/**
 * Utilitários para processamento de Markdown
 * Centraliza funções compartilhadas entre páginas da wiki
 */

import sanitizeHtml from "sanitize-html";

/**
 * Extrai texto puro de HTML de forma segura (remove tags)
 */
export function getTextContent(html: string): string {
  return sanitizeHtml(html, {
    allowedTags: [],
    allowedAttributes: {},
  });
}

/**
 * Gera ID único a partir de texto (slug)
 * Garante unicidade usando um Map de IDs já usados
 */
export function generateUniqueId(
  text: string,
  usedIds: Map<string, number>
): string {
  const baseId = text
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "") // Remove acentos
    .replace(/[^a-z0-9]+/g, "-") // Replace non-alphanumeric with dash
    .replace(/^-+|-+$/g, ""); // Remove leading/trailing dashes

  if (usedIds.has(baseId)) {
    const count = (usedIds.get(baseId) || 0) + 1;
    usedIds.set(baseId, count);
    return `${baseId}-${count}`;
  } else {
    usedIds.set(baseId, 0);
    return baseId;
  }
}

/**
 * Processa links no HTML renderizado para incluir basePath
 */
export function processMarkdownLinks(html: string, basePath: string = "/wiki"): string {
  return html.replace(
    /<a\s+([^>]*\s+)?href=["']([^"']+)["']([^>]*)>/gi,
    (match, before, href, after) => {
      // Ignora se já começa com basePath, é link externo, anchor ou mailto
      if (
        href.startsWith(basePath) ||
        href.startsWith("http") ||
        href.startsWith("#") ||
        href.startsWith("mailto:")
      ) {
        return match;
      }

      // Se termina com .md, converte para rota /docs
      if (href.endsWith(".md")) {
        const slug = href.replace(/^\.\/|\.md$/g, "");
        const normalizedSlug = slug.replace(/\\/g, "/"); // Normaliza separadores de caminho
        const newHref = `${basePath}/docs/${normalizedSlug}`;
        return `<a ${before || ""}href="${newHref}"${after || ""}>`;
      }

      // Se começa com /, adiciona basePath
      if (href.startsWith("/")) {
        const newHref = `${basePath}${href}`;
        return `<a ${before || ""}href="${newHref}"${after || ""}>`;
      }

      return match;
    }
  );
}


/**
 * Extrai e remove o primeiro H1 do markdown
 * Retorna o título extraído e o HTML sem o H1
 */
export function extractFirstH1(htmlContent: string): {
  title: string | null;
  content: string;
} {
  let firstH1Title: string | null = null;

  const processedContent = htmlContent.replace(
    /<h1[^>]*>(.*?)<\/h1>/gi,
    (match, text) => {
      if (firstH1Title === null) {
        firstH1Title = getTextContent(text).trim();
      }
      return ""; // Remove o H1 do conteúdo
    }
  );

  return {
    title: firstH1Title,
    content: processedContent,
  };
}

/**
 * Adiciona IDs únicos aos headings (h2-h4)
 */
export function addHeadingIds(htmlContent: string): string {
  const usedIds = new Map<string, number>();

  return htmlContent.replace(
    /<h([2-4])>(.*?)<\/h\1>/gi,
    (match, level, text) => {
      const cleanText = getTextContent(text);
      const id = generateUniqueId(cleanText, usedIds);
      return `<h${level} id="${id}">${text}</h${level}>`;
    }
  );
}
