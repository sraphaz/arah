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

async function getYamlContent(filePath: string) {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", filePath);
    const fileContents = await readFile(docsPath, "utf8");
    
    // Helper para remover prefixos numéricos (00_, 01_, etc.) do nome do arquivo
    function removeNumericPrefix(text: string): string {
      return text.replace(/^\d+_/, "");
    }

    const fileName = filePath.split('/').pop() || '';
    const fileNameWithoutExt = fileName.replace(/\.(yaml|yml)$/, "");
    const titleWithoutPrefix = removeNumericPrefix(fileNameWithoutExt);
    const fallbackTitle = titleWithoutPrefix.replace(/_/g, " ");

    // Não precisa fazer escape manual - React já faz escape automaticamente ao renderizar
    // O conteúdo será renderizado dentro de <code> que React escapa automaticamente

    return {
      content: fileContents,
      title: fallbackTitle,
      isYaml: true,
      fileName: fileName,
    };
  } catch (error) {
    console.error(`Error reading YAML file ${filePath}:`, error);
    return null;
  }
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

    // Adiciona IDs aos headings para navegação (garantindo unicidade)
    let htmlContent = processedContent.toString();

    // Extrai o primeiro H1 do markdown para usar como título se não houver frontmatter title
    let firstH1Title: string | null = null;
    htmlContent = htmlContent.replace(
      /<h1[^>]*>(.*?)<\/h1>/gi,
      (match, text) => {
        // Se ainda não capturamos o primeiro H1, usa-o como título
        if (firstH1Title === null) {
          firstH1Title = getTextContent(text).trim();
        }
        // Remove o H1 do conteúdo (não renderiza, evita duplicação)
        return '';
      }
    );

    const usedIds = new Map<string, number>(); // Rastreia IDs já usados e seus contadores

    htmlContent = htmlContent.replace(
      /<h([2-4])>(.*?)<\/h\1>/gi,
      (match, level, text) => {
        // Usa sanitize-html para remover HTML de forma segura
        const cleanText = getTextContent(text);
        let baseId = cleanText
          .toLowerCase()
          .normalize('NFD')
          .replace(/[\u0300-\u036f]/g, '') // Remove acentos
          .replace(/[^a-z0-9]+/g, '-') // Replace non-alphanumeric with dash
          .replace(/^-+|-+$/g, ''); // Remove leading/trailing dashes

        // Garante ID único: se já existe, adiciona sufixo numérico
        let id = baseId;
        if (usedIds.has(baseId)) {
          const count = (usedIds.get(baseId) || 0) + 1;
          usedIds.set(baseId, count);
          id = `${baseId}-${count}`;
        } else {
          usedIds.set(baseId, 0);
        }

        return `<h${level} id="${id}">${text}</h${level}>`;
      }
    );

    // Processa links no HTML renderizado para incluir basePath
    htmlContent = processMarkdownLinks(htmlContent, '/wiki');

    // Helper para remover prefixos numéricos (00_, 01_, etc.) do nome do arquivo
    function removeNumericPrefix(text: string): string {
      return text.replace(/^\d+_/, "");
    }

    // Gera título: usa frontmatter title, ou primeiro H1 do markdown, ou nome do arquivo
    const fileName = filePath.split('/').pop() || '';
    const fileNameWithoutExt = fileName.replace(".md", "");
    const titleWithoutPrefix = removeNumericPrefix(fileNameWithoutExt);
    const fallbackTitle = titleWithoutPrefix.replace(/_/g, " ");

    return {
      content: htmlContent,
      frontMatter: data,
      title: data.title || firstH1Title || fallbackTitle,
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
      } else if (entry.isFile() && (entry.name.endsWith('.md') || entry.name.endsWith('.yaml') || entry.name.endsWith('.yml'))) {
        // Adiciona arquivo (sem extensão para o slug)
        const relativePath = fullPath.replace(/\.(md|yaml|yml)$/, '');
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
  // Decodifica cada segmento do slug (remove URL encoding) e constrói o caminho
  const decodedSlug = slug.map((segment: string) => {
    try {
      return decodeURIComponent(segment);
    } catch {
      return segment;
    }
  });
  
  // Tenta primeiro como .md, depois como .yaml/.yml
  let filePath = `${decodedSlug.join('/')}.md`.replace(/\\/g, '/');
  let doc = await getDocContent(filePath);
  let yamlDoc = null;
  let yamlFilePath = '';
  
  // Se não encontrou como .md, tenta como YAML
  if (!doc) {
    yamlFilePath = `${decodedSlug.join('/')}.yaml`.replace(/\\/g, '/');
    yamlDoc = await getYamlContent(yamlFilePath);
    
    // Se não encontrou .yaml, tenta .yml
    if (!yamlDoc) {
      yamlFilePath = `${decodedSlug.join('/')}.yml`.replace(/\\/g, '/');
      yamlDoc = await getYamlContent(yamlFilePath);
    }
  } else {
    yamlFilePath = filePath;
  }

  if (!doc && !yamlDoc) {
    notFound();
  }

  return (
        <main className="flex-1 py-4 lg:py-6 px-4 md:px-6 lg:px-8">
      <div className="w-full mx-auto grid grid-cols-1 lg:grid-cols-[1fr_240px] xl:grid-cols-[1fr_260px] 2xl:grid-cols-[1fr_280px] gap-4 lg:gap-6 xl:gap-8">
        {/* Main Content Column */}
        <div>
          <div className="glass-card animation-fade-in">
            <div className="glass-card__content">
              {/* Breadcrumb Refinado */}
              <nav className="breadcrumb mb-8">
                <Link href="/wiki">Boas-Vindas</Link>
                <span>›</span>
                <Link href="/wiki/docs">Documentação</Link>
                {(slug[0]?.startsWith('ONBOARDING_') || slug.some(s => s?.startsWith('ONBOARDING_'))) && (
                  <>
                    <span>›</span>
                    <Link href="/wiki/docs">Onboarding</Link>
                  </>
                )}
                <span>›</span>
                <span className="text-forest-900 font-medium">{doc?.title || yamlDoc?.title}</span>
              </nav>

              {/* Document Title - Hero */}
              <h1 className="text-2xl md:text-3xl lg:text-4xl font-bold text-forest-900 dark:text-forest-50 mb-6 leading-tight">
                {doc?.title || yamlDoc?.title}
              </h1>

              {/* Botão de Download para YAML */}
              {yamlDoc && (
                <div className="mb-6 flex flex-wrap gap-4 items-center">
                  <a
                    href={`/wiki/api/yaml-download?file=${encodeURIComponent(yamlFilePath)}`}
                    download={yamlDoc.fileName}
                    className="btn-secondary inline-flex items-center gap-2"
                  >
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                    </svg>
                    Baixar {yamlDoc.fileName}
                  </a>
                  <span className="text-sm text-forest-600 dark:text-forest-400">
                    Arquivo YAML - Contrato OpenAPI
                  </span>
                </div>
              )}

              {/* Document Content - Refinado com Progressive Disclosure */}
              {doc && <ContentSections htmlContent={doc.content} />}
              
              {/* YAML Content com syntax highlighting */}
              {yamlDoc && (
                <div className="markdown-content">
                  <pre className="bg-forest-900 dark:bg-dark-bg text-forest-50 p-6 rounded-2xl overflow-x-auto mb-10 border border-forest-800 dark:border-dark-border shadow-lg">
                    <code className="language-yaml">{yamlDoc.content}</code>
                  </pre>
                </div>
              )}
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
