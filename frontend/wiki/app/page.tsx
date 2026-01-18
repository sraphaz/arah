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
    // Caminho: de frontend/wiki para docs/ na raiz (2 níveis acima)
    // process.cwd() pode variar em dev vs build - usa __dirname como fallback
    let basePath = process.cwd();

    // Se estiver em .next (build), ajusta o caminho
    if (basePath.includes('.next')) {
      basePath = join(basePath, '..', '..', '..', '..');
    } else {
      // Em dev, frontend/wiki - vai 2 níveis acima
      basePath = join(basePath, '..', '..');
    }

    const docsPath = join(basePath, "docs", filePath).replace(/\\/g, '/');
    const fileContents = await readFile(docsPath, "utf8");
    const { content, data } = matter(fileContents);

    const processedContent = await remark()
      .use(remarkHtml)
      .use(remarkGfm)
      .process(content);

    // Adiciona IDs aos headings para navegação e progressive disclosure
    // IMPORTANTE: Remove H1 do markdown completamente, pois já temos H1 próprio na página
    let htmlContent = processedContent.toString();

    // Remove apenas o H1 (título) - mantém todo conteúdo após (incluindo parágrafos introdutórios)
    // O H1 do markdown é removido mas o conteúdo após ele é preservado
    htmlContent = htmlContent.replace(
      /<h1[^>]*>(.*?)<\/h1>/gi,
      (match, text) => {
        // Remove apenas o H1 - conteúdo após será preservado
        return '';
      }
    );

    // Adiciona IDs aos headings garantindo unicidade
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

    return {
      content: htmlContent,
      frontMatter: data,
      title: data.title || "Boas-Vindas",
    };
  } catch (error) {
    console.error(`Error reading ${filePath}:`, error);
    const attemptedPath = join(process.cwd(), "..", "..", "docs", filePath).replace(/\\/g, '/');
    console.error(`Attempted path: ${attemptedPath}`);
    console.error(`Current working directory: ${process.cwd()}`);
    console.error(`Error details:`, error);
    // Não retorna null - sempre retorna algo para evitar 404
    // Retorna um objeto vazio que será tratado pelo fallback
    return null;
  }
}

export default async function HomePage() {
  // Carregar ONBOARDING_PUBLICO como landing
  const onboardingDoc = await getDocContent("ONBOARDING_PUBLICO.md");

  // Se não conseguir carregar, mostra fallback ao invés de 404
  if (!onboardingDoc) {
    return (
      <main className="container-max py-12 lg:py-16 px-4 md:px-6 lg:px-8">
        <div className="glass-card animation-fade-in">
          <div className="glass-card__content markdown-content">
            <h1 className="text-3xl md:text-4xl lg:text-5xl font-bold text-forest-900 dark:text-forest-50 mb-6 leading-tight tracking-tight">
              Boas-Vindas ao Araponga
            </h1>
            <p className="text-lg text-forest-700 dark:text-forest-300 mb-8">
              Bem-vindo à documentação completa do Araponga.
            </p>
            <div className="mt-8">
              <Link href="/docs" className="btn-primary">
                Ver Documentação
              </Link>
            </div>
          </div>
        </div>
      </main>
    );
  }

  return (
    <main className="container-max py-12 lg:py-16 px-4 md:px-6 lg:px-8">
        {onboardingDoc && (
          <div className="w-full mx-auto grid grid-cols-1 lg:grid-cols-[1fr_240px] xl:grid-cols-[1fr_260px] 2xl:grid-cols-[1fr_280px] gap-4 lg:gap-6 xl:gap-8">
            {/* Main Content Column */}
            <div>
              <div className="glass-card animation-fade-in">
                <div className="glass-card__content markdown-content">
                  {/* Document Title - H1 para SEO, título principal da página */}
                  <h1 className="text-3xl md:text-4xl lg:text-5xl font-bold text-forest-900 dark:text-forest-50 mb-6 leading-tight tracking-tight">
                    {onboardingDoc.title}
                  </h1>

                  {/* Document Metadata - Padrão internacional discreto */}
              {onboardingDoc.frontMatter && (onboardingDoc.frontMatter.version || onboardingDoc.frontMatter.date) && (
                <div className="document-metadata">
                  {onboardingDoc.frontMatter.version && (
                    <span>v{onboardingDoc.frontMatter.version}</span>
                  )}
                  {onboardingDoc.frontMatter.version && onboardingDoc.frontMatter.date && (
                    <span className="separator">·</span>
                  )}
                  {onboardingDoc.frontMatter.date && (
                    <time dateTime={onboardingDoc.frontMatter.date}>{onboardingDoc.frontMatter.date}</time>
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

        {/* Section Divider - sem bordas (removido) */}

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

        {/* Section Divider - sem bordas (removido) */}

        {/* Quick Links Section - Página Inicial */}
        <div className="glass-card animation-fade-in">
          <div className="glass-card__content">
            <QuickLinks />
          </div>
        </div>
    </main>
  );
}
