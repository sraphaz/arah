import Link from "next/link";
// Header, Sidebar e Footer agora estão no layout.tsx raiz
import { FeatureCard } from "../components/ui/FeatureCard";
import { ApiDomainDiagram } from "../components/content/ApiDomainDiagram";
import { AppBanner } from "../components/content/AppBanner";
import { ContentSectionsProgressive } from "./docs/[slug]/content-sections-progressive";
import { TableOfContents } from "../components/layout/TableOfContents";
import { JourneyCard } from "../components/ui/JourneyCard";
import { getAllJourneys } from "../lib/journeys";
import { brand } from "../../shared/config/brand";
import { processMarkdownContent } from "../lib/document";

// Função para extrair o primeiro parágrafo/texto introdutório (antes de listas ou headings)
function extractFirstIntroParagraph(html: string): { firstParagraph: string; remainingContent: string } {
  // Remove espaços em branco no início
  const trimmed = html.trim();

  // Encontra o primeiro <ul>, <ol>, <h2>, ou <hr>
  const firstListOrHeadingMatch = trimmed.match(/<(ul|ol|h[2-6]|hr)[\s>]/i);

  if (!firstListOrHeadingMatch) {
    // Se não encontrar, retorna o conteúdo completo como firstParagraph
    return { firstParagraph: trimmed, remainingContent: '' };
  }

  const splitIndex = firstListOrHeadingMatch.index || 0;
  const firstParagraph = trimmed.substring(0, splitIndex).trim();
  const remainingContent = trimmed.substring(splitIndex).trim();

  return { firstParagraph, remainingContent };
}

// Força geração estática no build time - homepage pré-renderizada
export const dynamic = 'force-static';
export const revalidate = false; // Página totalmente estática, sem revalidação

export default async function HomePage() {
  // Carregar ONBOARDING_PUBLICO como landing
  const onboardingDoc = await processMarkdownContent("ONBOARDING_PUBLICO.md");
  // Título próprio da homepage: usa frontmatter, senão "Boas-Vindas"
  const homeTitle = (onboardingDoc?.frontMatter.title as string) || "Boas-Vindas";

  // Se não conseguir carregar, mostra fallback ao invés de 404
  if (!onboardingDoc) {
    return (
      <main className="container-max py-4 lg:py-6 px-4 md:px-6 lg:px-8">
        <div className="glass-card animation-fade-in">
          <div className="glass-card__content markdown-content">
            <h1 className="text-2xl md:text-3xl lg:text-4xl font-bold text-forest-900 dark:text-forest-50 mb-6 leading-tight tracking-tight">
              Boas-Vindas ao {brand.name}
            </h1>
            <p className="text-lg text-forest-700 dark:text-forest-300 mb-8">
              Bem-vindo à documentação completa do {brand.name}.
            </p>
            <div className="mt-8">
              <Link href="/docs" prefetch={false} className="btn-primary">
                Ver Documentação
              </Link>
            </div>
          </div>
        </div>
      </main>
    );
  }

  // Extrai primeiro parágrafo e conteúdo restante
  const { firstParagraph, remainingContent } = extractFirstIntroParagraph(onboardingDoc.content);

  return (
    <main className="flex-1 py-4 px-1 md:px-1.5 lg:px-2">
      {onboardingDoc && (
        <div className="max-w-7xl xl:max-w-8xl 2xl:max-w-full mx-auto grid grid-cols-1 lg:grid-cols-[1fr_280px] xl:grid-cols-[1fr_300px] 2xl:grid-cols-[1fr_320px] gap-1.5 lg:gap-2 xl:gap-2.5">
          {/* Main Content Column */}
          <div>
            <div className="glass-card animation-fade-in">
              <div className="glass-card__content markdown-content">
                {/* Document Title - H1 para SEO, título principal da página - Tipografia Enterprise */}
                <h1 className="text-3xl md:text-4xl lg:text-5xl font-bold text-forest-900 dark:text-forest-50 mb-4 leading-tight">
                  {homeTitle}
                </h1>

                {/* Elemento visual decorado - HR após o título */}
                <hr />

                {/* Primeiro parágrafo - Dentro do card, logo após o HR */}
                {firstParagraph && (
                  <div className="mb-6" dangerouslySetInnerHTML={{ __html: firstParagraph }} />
                )}

                {/* Document Content - Com Progressive Disclosure (mesmo da página de onboarding) */}
                {remainingContent && (
                  <ContentSectionsProgressive htmlContent={remainingContent} useProgressive={true} />
                )}

                {/* Diagrama do Domínio API - Visual Explicativo */}
                <ApiDomainDiagram />
              </div>
            </div>

            {/* App Banner - Call to Action para Lançamento */}
            <AppBanner />

            {/* Jornadas Guiadas - Sistema de Navegação por Perfil */}
            <div className="mt-12 mb-8">
              <h2 className="text-2xl md:text-3xl font-bold text-forest-900 dark:text-forest-50 mb-4">
                Escolha seu Caminho
              </h2>
              <p className="text-base text-forest-600 dark:text-forest-400 mb-8 max-w-3xl">
                Navegue pela documentação seguindo um caminho guiado recomendado para seu perfil:
              </p>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {getAllJourneys().map((journey) => (
                  <JourneyCard key={journey.title} journey={journey} />
                ))}
              </div>
            </div>

            {/* Quick Navigation - Grid horizontal enterprise-level */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-8">
              <FeatureCard
                icon="👨‍💻"
                title="Desenvolvedores"
                description={`Comece a desenvolver com o ${brand.name}`}
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
          </div>

          {/* TOC Column - Sticky - Aparece na homepage também */}
          <aside className="hidden lg:block">
            <div className="sticky top-24">
              <TableOfContents />
            </div>
          </aside>
        </div>
      )}
    </main>
  );
}
