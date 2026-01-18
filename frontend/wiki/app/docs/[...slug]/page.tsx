import Link from "next/link";
import { notFound } from "next/navigation";
import { readdir, readFile, stat } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";
import sanitizeHtml from "sanitize-html";
import { TableOfContents } from "../../../components/layout/TableOfContents";
import { ContentSections } from "../[slug]/content-sections";

// Helper function para extrair texto de HTML de forma segura
function getTextContent(html: string): string {
  return sanitizeHtml(html, {
    allowedTags: [],
    allowedAttributes: {},
  });
}

interface PageProps {
  params: Promise<{ slug: string[] }>;
}

function processMarkdownLinks(html: string, basePath: string = '/wiki'): string {
  // Processa links <a href="/docs/..."> para <a href="/wiki/docs/...">
  // Também processa links relativos que terminam com .md
  return html.replace(
    /<a\s+([^>]*\s+)?href=["']([^"']+)["']([^>]*)>/gi,
    (match, before, href, after) => {
      // Ignora se já começa com basePath ou é link externo
      if (href.startsWith(basePath) || href.startsWith('http') || href.startsWith('#') || href.startsWith('mailto:')) {
        return match;
      }

      // Se é link relativo que termina com .md, converte para /wiki/docs/... (sem .md)
      if (href.endsWith('.md')) {
        const slug = href.replace(/^\.\/|\.md$/g, '');
        const newHref = `${basePath}/docs/${slug}`;
        return `<a ${before || ''}href="${newHref}"${after || ''}>`;
      }

      // Se começa com /, adiciona basePath
      if (href.startsWith('/')) {
        const newHref = `${basePath}${href}`;
        return `<a ${before || ''}href="${newHref}"${after || ''}>`;
      }

      // Links relativos sem .md - mantém como está
      return match;
    }
  );
}

async function getDocContent(filePath: string) {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", filePath);
    const fileContents = await readFile(docsPath, "utf8");
    const { content, data } = matter(fileContents);

    const processedContent = await remark()
      .use(remarkHtml)
      .use(remarkGfm)
      .process(content);

    // Adiciona IDs aos headings para navegação
    let htmlContent = processedContent.toString();
    htmlContent = htmlContent.replace(
      /<h([2-4])>(.*?)<\/h\1>/gi,
      (match, level, text) => {
        // Usa sanitize-html para remover HTML de forma segura
        const cleanText = getTextContent(text);
        const id = cleanText
          .toLowerCase()
          .normalize('NFD')
          .replace(/[\u0300-\u036f]/g, '') // Remove acentos
          .replace(/[^a-z0-9]+/g, '-') // Replace non-alphanumeric with dash
          .replace(/^-+|-+$/g, ''); // Remove leading/trailing dashes
        return `<h${level} id="${id}">${text}</h${level}>`;
      }
    );

    // Processa links no HTML renderizado para incluir basePath
    htmlContent = processMarkdownLinks(htmlContent, '/wiki');

    // Helper para remover prefixos numéricos (00_, 01_, etc.) do nome do arquivo
    function removeNumericPrefix(text: string): string {
      return text.replace(/^\d+_/, "");
    }

    // Gera título: usa frontmatter title, ou remove prefixo numérico e substitui _ por espaços
    const fileName = filePath.split('/').pop() || '';
    const fileNameWithoutExt = fileName.replace(".md", "");
    const titleWithoutPrefix = removeNumericPrefix(fileNameWithoutExt);
    const fallbackTitle = titleWithoutPrefix.replace(/_/g, " ");

    return {
      content: htmlContent,
      frontMatter: data,
      title: data.title || fallbackTitle,
    };
  } catch (error) {
    console.error(`Error reading ${filePath}:`, error);
    return null;
  }
}

async function getAllDocsRecursive(basePath: string = ''): Promise<string[]> {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", basePath);
    const files: string[] = [];
    const entries = await readdir(docsPath, { withFileTypes: true });

    for (const entry of entries) {
      // Normaliza separadores de caminho para '/' (funciona em todos os sistemas)
      const fullPath = basePath 
        ? `${basePath}/${entry.name}`.replace(/\\/g, '/')
        : entry.name;
      
      if (entry.isDirectory()) {
        // Recursivamente busca em subdiretórios
        const subFiles = await getAllDocsRecursive(fullPath);
        files.push(...subFiles);
      } else if (entry.isFile() && entry.name.endsWith('.md')) {
        // Adiciona arquivo (sem extensão .md para o slug)
        const relativePath = fullPath.replace(/\.md$/, '');
        files.push(relativePath);
      }
    }

    return files;
  } catch (error) {
    console.error(`Error reading docs directory ${basePath}:`, error);
    return [];
  }
}

export async function generateStaticParams() {
  const docs = await getAllDocsRecursive('');
  return docs.map((doc) => ({
    slug: doc.split('/'),
  }));
}

export default async function DocPage({ params }: PageProps) {
  const { slug } = await params;
  
  // slug é um array, ex: ['backlog-api', 'README']
  // Constrói o caminho do arquivo normalizando separadores
  const filePath = `${slug.join('/')}.md`.replace(/\\/g, '/');
  
  const doc = await getDocContent(filePath);

  if (!doc) {
    notFound();
  }

  return (
    <main className="flex-1 py-12 px-4 md:px-8 lg:px-12">
      <div className="max-w-6xl xl:max-w-7xl 2xl:max-w-[90rem] mx-auto grid grid-cols-1 lg:grid-cols-[1fr_280px] xl:grid-cols-[1fr_300px] 2xl:grid-cols-[1fr_320px] gap-6 lg:gap-8 xl:gap-10">
        {/* Main Content Column */}
        <div>
          <div className="glass-card animation-fade-in">
            <div className="glass-card__content">
              {/* Breadcrumb Refinado */}
              <nav className="breadcrumb mb-8">
                <Link href="/wiki">Boas-Vindas</Link>
                <span>›</span>
                <Link href="/wiki/docs">Documentação</Link>
                {slug.map((segment, index) => (
                  <span key={index}>
                    <span>›</span>
                    {index === slug.length - 1 ? (
                      <span className="text-forest-900 font-medium">{doc.title}</span>
                    ) : (
                      <span>{segment}</span>
                    )}
                  </span>
                ))}
              </nav>

              {/* Document Title - Hero */}
              <h1 className="text-5xl md:text-6xl font-bold text-forest-900 dark:text-forest-50 mb-8 leading-tight">
                {doc.title}
              </h1>

              {/* Document Metadata - Badges */}
              {doc.frontMatter && (doc.frontMatter.version || doc.frontMatter.date || doc.frontMatter.status) && (
                <div className="mb-12 pb-6 border-b-2 border-forest-200/80 dark:border-forest-800/80 flex flex-wrap gap-3">
                  {doc.frontMatter.version && (
                    <span className="metadata-badge">
                      <span className="mr-2">📌</span>
                      Versão: {doc.frontMatter.version}
                    </span>
                  )}
                  {doc.frontMatter.date && (
                    <span className="metadata-badge">
                      <span className="mr-2">📅</span>
                      {doc.frontMatter.date}
                    </span>
                  )}
                  {doc.frontMatter.status && (
                    <span className="metadata-badge">
                      <span className="mr-2">✓</span>
                      {doc.frontMatter.status}
                    </span>
                  )}
                </div>
              )}

              {/* Document Content - Refinado com Progressive Disclosure */}
              <ContentSections htmlContent={doc.content} />
            </div>
          </div>

          {/* Navigation Links - Refinado */}
          <div className="mt-12 flex flex-col sm:flex-row justify-between gap-4">
            <Link
              href="/wiki"
              className="btn-secondary text-center"
            >
              ← Voltar às Boas-Vindas
            </Link>
            <Link
              href="/wiki/docs"
              className="btn-secondary text-center"
            >
              Ver Todos os Docs →
            </Link>
          </div>
        </div>

        {/* TOC Column - Sticky */}
        <aside className="hidden lg:block">
          <div className="sticky top-24">
            <TableOfContents />
          </div>
        </aside>
      </div>
    </main>
  );
}
