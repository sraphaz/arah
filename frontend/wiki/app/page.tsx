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

async function getDocContent(filePath: string) {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", filePath);
    const fileContents = await readFile(docsPath, "utf8");
    const { content, data } = matter(fileContents);

    const processedContent = await remark()
      .use(remarkHtml)
      .use(remarkGfm)
      .process(content);

    return {
      content: processedContent.toString(),
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
              <h1 className="text-5xl md:text-6xl lg:text-7xl font-bold text-forest-900 dark:text-forest-50 mb-10 leading-tight tracking-tight">
                {onboardingDoc.title}
              </h1>

              {/* Document Metadata */}
              {onboardingDoc.frontMatter && (onboardingDoc.frontMatter.version || onboardingDoc.frontMatter.date) && (
                <div className="mb-10 pb-8 border-b border-forest-200/60 flex flex-wrap gap-3">
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

        {/* Quick Navigation - Dynamic design with FeatureCards */}
        <div className="mt-16 grid md:grid-cols-3 gap-6">
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
            color="blue"
            href="/docs/ONBOARDING_ANALISTAS_FUNCIONAIS"
          />
          <FeatureCard
            icon="📚"
            title="Índice Completo"
            description="Explore toda a documentação"
            color="purple"
            href="/docs/00_INDEX"
          />
        </div>
      </main>

      <Footer />
    </div>
  );
}
