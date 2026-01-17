import Link from "next/link";
import { readdir, readFile } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";
import { Header } from "../components/layout/Header";
import { Footer } from "../components/layout/Footer";
import { FeatureCard } from "../components/ui/FeatureCard";
import { QuickLinks } from "../components/layout/QuickLinks";

function processMarkdownLinks(html: string, basePath: string = '/wiki'): string {
  // Processa links <a href="/docs/..."> para <a href="/wiki/docs/...">
  // Mas NÃO processa links externos ou absolutos
  return html.replace(
    /<a\s+([^>]*\s+)?href=["'](\/[^"']+)["']([^>]*)>/gi,
    (match, before, href, after) => {
      // Ignora se já começa com basePath ou é link externo
      if (href.startsWith(basePath) || href.startsWith('http')) {
        return match;
      }
      
      // Adiciona basePath a links relativos que começam com /
      const newHref = `${basePath}${href}`;
      return `<a ${before || ''}href="${newHref}"${after || ''}>`;
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

    // Processa links no HTML renderizado para incluir basePath
    const htmlContent = processMarkdownLinks(processedContent.toString(), '/wiki');

    return {
      content: htmlContent,
      frontMatter: data,
      title: data.title || "Bem-Vind@ à Wiki Araponga",
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
    <div className="min-h-screen flex flex-col">
      <Header />

      {/* Main Content */}
      <main className="flex-1 container-max py-16 md:py-20">
        {onboardingDoc && (
          <div className="glass-card animation-fade-in">
            <div className="glass-card__content">
              {/* Document Title */}
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

              {/* Document Content */}
              <div
                className="markdown-content prose-headings:first:mt-0"
                dangerouslySetInnerHTML={{ __html: onboardingDoc.content }}
              />
            </div>
          </div>
        )}

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

      <Footer />
    </div>
  );
}
