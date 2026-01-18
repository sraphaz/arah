import Link from "next/link";
import { readdir, readFile } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";
import sanitizeHtml from "sanitize-html";
// Header, Sidebar e Footer agora estão no layout.tsx raiz
import { FeatureCard } from "../components/ui/FeatureCard";
import { QuickLinks } from "../components/layout/QuickLinks";
import { ApiDomainDiagram } from "../components/content/ApiDomainDiagram";
import { AppBanner } from "../components/content/AppBanner";
import { ContentSections } from "./docs/[slug]/content-sections";
import { TableOfContents } from "../components/layout/TableOfContents";

// Helper function para extrair texto de HTML de forma segura
function getTextContent(html: string): string {
  return sanitizeHtml(html, {
    allowedTags: [],
    allowedAttributes: {},
  });
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

    // Adiciona IDs aos headings para navegação e progressive disclosure
    // IMPORTANTE: Também processa H1 para garantir que não haja duplicação
    let htmlContent = processedContent.toString();
    
    // Remove ou transforma H1 em H2 se necessário para evitar duplicação com título da página
    htmlContent = htmlContent.replace(
      /<h1>(.*?)<\/h1>/gi,
      (match, text) => {
        // Se o markdown tem H1, transforma em H2 para evitar duplicação
        const cleanText = getTextContent(text);
        const id = cleanText
          .toLowerCase()
          .normalize('NFD')
          .replace(/[\u0300-\u036f]/g, '')
          .replace(/[^a-z0-9]+/g, '-')
          .replace(/^-+|-+$/g, '');
        return `<h2 id="${id}">${text}</h2>`;
      }
    );
    
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

    return {
      content: htmlContent,
      frontMatter: data,
      title: data.title || "Araponga",
    };
  } catch (error) {
    console.error(`Error reading ${filePath}:`, error);
    return null;
  }
}

export default async function HomePage() {
  // Carregar ONBOARDING_PUBLICO como landing
  const onboardingDoc = await getDocContent("ONBOARDING_PUBLICO.md");

  return (
    <main className="container-max py-16 md:py-20">
        {onboardingDoc && (
          <div className="max-w-6xl xl:max-w-7xl 2xl:max-w-[90rem] mx-auto grid grid-cols-1 lg:grid-cols-[1fr_280px] xl:grid-cols-[1fr_300px] 2xl:grid-cols-[1fr_320px] gap-6 lg:gap-8 xl:gap-10">
            {/* Main Content Column */}
            <div>
              <div className="glass-card animation-fade-in">
                <div className="glass-card__content markdown-content">
                  {/* Document Title - H1 para SEO, título principal da página */}
                  <h1 className="text-5xl md:text-6xl lg:text-7xl font-bold text-forest-900 dark:text-forest-50 mb-8 leading-tight tracking-tight">
                    {onboardingDoc.title}
                  </h1>

                  {/* Document Metadata */}
              {onboardingDoc.frontMatter && (onboardingDoc.frontMatter.version || onboardingDoc.frontMatter.date) && (
                <div className="mb-12 pb-6 border-b-2 border-forest-200/80 dark:border-forest-800/80 flex flex-wrap gap-3">
                  {onboardingDoc.frontMatter.version && (
                    <span className="metadata-badge">
                      <span className="mr-2">📌</span>
                      Versão: {onboardingDoc.frontMatter.version}
                    </span>
                  )}
                  {onboardingDoc.frontMatter.date && (
                    <span className="metadata-badge">
                      <span className="mr-2">📅</span>
                      {onboardingDoc.frontMatter.date}
                    </span>
                  )}
                </div>
              )}

              {/* Document Content - Com Progressive Disclosure */}
              <ContentSections htmlContent={onboardingDoc.content} />

              {/* Diagrama do Domínio API - Visual Explicativo */}
              <ApiDomainDiagram />
                </div>
              </div>
            </div>

            {/* TOC Column - Sticky - Aparece na homepage também */}
            <aside className="hidden lg:block">
              <div className="sticky top-24">
                <TableOfContents />
              </div>
            </aside>
          </div>
        )}

        {/* App Banner - Call to Action para Lançamento */}
        <AppBanner />

        {/* Section Divider */}
        <div className="mt-20 mb-8 relative">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t-2 border-forest-200/60 dark:border-forest-800/60"></div>
          </div>
          <div className="relative flex justify-center text-sm">
            <span className="px-4 bg-forest-50 dark:bg-forest-950 text-forest-500 dark:text-forest-400 font-medium">
              Explorar Documentação
            </span>
          </div>
        </div>

        {/* Quick Navigation - Harmonizado com paleta Araponga */}
        <div className="grid md:grid-cols-3 gap-6">
          <FeatureCard
            icon="👨‍💻"
            title="Desenvolvedores"
            description="Comece a desenvolver com o Araponga"
            color="forest"
            href="/docs/ONBOARDING_DEVELOPERS"
          />
          <FeatureCard
            icon="👁️"
            title="Analistas"
            description="Observe territórios e proponha melhorias"
            color="accent"
            href="/docs/ONBOARDING_ANALISTAS_FUNCIONAIS"
          />
          <FeatureCard
            icon="📚"
            title="Índice Completo"
            description="Explore toda a documentação"
            color="link"
            href="/docs/00_INDEX"
          />
        </div>

        {/* Section Divider - Links Úteis */}
        <div className="mt-16 mb-8 relative">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t-2 border-forest-200/60 dark:border-forest-800/60"></div>
          </div>
          <div className="relative flex justify-center text-sm">
            <span className="px-4 bg-forest-50 dark:bg-forest-950 text-forest-500 dark:text-forest-400 font-medium">
              Links Úteis
            </span>
          </div>
        </div>

        {/* Quick Links Section - Página Inicial */}
        <div className="glass-card animation-fade-in">
          <div className="glass-card__content">
            <QuickLinks />
          </div>
        </div>
    </main>
  );
}
