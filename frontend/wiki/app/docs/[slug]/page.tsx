import Link from "next/link";
import { notFound } from "next/navigation";
import { readdir, readFile } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";
import sanitizeHtml from "sanitize-html";
// Header, Sidebar e Footer agora estão no layout.tsx raiz
import { TableOfContents } from "../../../components/layout/TableOfContents";
import { ContentSections } from "./content-sections";
import { ContentSectionsProgressive } from "./content-sections-progressive";

// Helper function para extrair texto de HTML de forma segura
function getTextContent(html: string): string {
  return sanitizeHtml(html, {
    allowedTags: [],
    allowedAttributes: {},
  });
}

interface PageProps {
  params: Promise<{ slug: string }>;
}

// Mapeamento de slugs para nomes de arquivo
const slugToFile: Record<string, string> = {
  "00_INDEX": "00_INDEX.md",
  "01_PRODUCT_VISION": "01_PRODUCT_VISION.md",
  "02_ROADMAP": "02_ROADMAP.md",
  "03_BACKLOG": "03_BACKLOG.md",
  "04_USER_STORIES": "04_USER_STORIES.md",
  "05_GLOSSARY": "05_GLOSSARY.md",
  "10_ARCHITECTURE_DECISIONS": "10_ARCHITECTURE_DECISIONS.md",
  "11_ARCHITECTURE_SERVICES": "11_ARCHITECTURE_SERVICES.md",
  "12_DOMAIN_MODEL": "12_DOMAIN_MODEL.md",
  "13_DOMAIN_ROUTING": "13_DOMAIN_ROUTING.md",
  "20_IMPLEMENTATION_PLAN": "20_IMPLEMENTATION_PLAN.md",
  "21_CODE_REVIEW": "21_CODE_REVIEW.md",
  "22_COHESION_AND_TESTS": "22_COHESION_AND_TESTS.md",
  "23_IMPLEMENTATION_RECOMMENDATIONS": "23_IMPLEMENTATION_RECOMMENDATIONS.md",
  "ONBOARDING_PUBLICO": "ONBOARDING_PUBLICO.md",
  "ONBOARDING_DEVELOPERS": "ONBOARDING_DEVELOPERS.md",
  "ONBOARDING_ANALISTAS_FUNCIONAIS": "ONBOARDING_ANALISTAS_FUNCIONAIS.md",
  "CARTILHA_COMPLETA": "CARTILHA_COMPLETA.md",
  "ONBOARDING_FAQ": "ONBOARDING_FAQ.md",
  "MENTORIA": "MENTORIA.md",
  "PRIORIZACAO_PROPOSTAS": "PRIORIZACAO_PROPOSTAS.md",
  "PROJECT_STRUCTURE": "PROJECT_STRUCTURE.md",
  "SECURITY_CONFIGURATION": "SECURITY_CONFIGURATION.md",
  "SECURITY_AUDIT": "SECURITY_AUDIT.md",
  "40_CHANGELOG": "40_CHANGELOG.md",
  "41_CONTRIBUTING": "41_CONTRIBUTING.md",
  "60_API_LÓGICA_NEGÓCIO": "60_API_LÓGICA_NEGÓCIO.md",
};

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

async function getDocContent(fileName: string) {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", fileName);
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
    const fileNameWithoutExt = fileName.replace(".md", "");
    const titleWithoutPrefix = removeNumericPrefix(fileNameWithoutExt);
    const fallbackTitle = titleWithoutPrefix.replace(/_/g, " ");

    return {
      content: htmlContent,
      frontMatter: data,
      title: data.title || firstH1Title || fallbackTitle,
    };
  } catch (error) {
    console.error(`Error reading ${fileName}:`, error);
    return null;
  }
}

async function getAllDocs() {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs");
    const files = await readdir(docsPath);
    return files.filter((file) => file.endsWith(".md")).map((file) => file.replace(".md", ""));
  } catch (error) {
    console.error("Error reading docs directory:", error);
    return [];
  }
}

// Força geração estática no build time - páginas são pré-renderizadas
export const dynamic = 'force-static';
export const dynamicParams = false; // Retorna 404 para slugs não conhecidos em vez de gerar dinamicamente
export const revalidate = false; // Páginas totalmente estáticas, sem revalidação

export async function generateStaticParams() {
  const docs = await getAllDocs();
  return docs.map((doc) => ({
    slug: doc,
  }));
}

export default async function DocPage({ params }: PageProps) {
  const { slug } = await params;
  // Decodifica o slug caso tenha sido URL-encoded (ex: %C3%93 -> Ó)
  const decodedSlug = decodeURIComponent(slug);
  const fileName = slugToFile[decodedSlug] || slugToFile[slug] || `${decodedSlug}.md`;
  const doc = await getDocContent(fileName);

  if (!doc) {
    notFound();
  }

  return (
    <main className="flex-1 py-4 px-4 md:px-6 lg:px-8">
          <div className="max-w-6xl xl:max-w-7xl 2xl:max-w-[90rem] mx-auto grid grid-cols-1 lg:grid-cols-[1fr_280px] xl:grid-cols-[1fr_300px] 2xl:grid-cols-[1fr_320px] gap-6 lg:gap-8 xl:gap-10">
            {/* Main Content Column */}
            <div>
              <div className="glass-card animation-fade-in">
                <div className="glass-card__content">
                  {/* Breadcrumb Refinado */}
                  <nav className="breadcrumb mb-4">
                    <Link href="/" prefetch={false}>Boas-Vindas</Link>
                    <span>›</span>
                    <Link href="/docs" prefetch={false}>Documentação</Link>
                    <span>›</span>
                    <span className="text-forest-900 font-medium">{doc.title}</span>
                  </nav>

                  {/* Document Title - Hero */}
                  <h1 className="text-xl md:text-2xl lg:text-3xl font-bold text-forest-900 dark:text-forest-50 mb-4 leading-tight">
                    {doc.title}
                  </h1>


                  {/* Document Content - Progressive Disclosure para documentos longos */}
                  <ContentSectionsProgressive htmlContent={doc.content} useProgressive={true} />
                </div>
              </div>

              {/* Navigation Links - Refinado */}
              <div className="mt-12 flex flex-col sm:flex-row justify-between gap-4">
                <Link
                  href="/"
                  prefetch={false}
                  className="btn-secondary text-center"
                >
                  ← Voltar às Boas-Vindas
                </Link>
                <Link
                  href="/docs"
                  prefetch={false}
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
