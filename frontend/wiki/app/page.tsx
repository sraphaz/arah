import Link from "next/link";
import { readdir, readFile } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";
import { Header } from "../components/layout/Header";
import { Footer } from "../components/layout/Footer";

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
      <main className="flex-1 container-max py-12">
        {onboardingDoc && (
          <div className="glass-card animation-fade-in">
            <div className="glass-card__content">
              {/* Document Title */}
              <h1 className="text-5xl md:text-6xl font-bold text-forest-900 mb-8 leading-tight">
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

        {/* Quick Navigation */}
        <div className="mt-12 grid md:grid-cols-3 gap-6">
          <Link
            href="/docs/ONBOARDING_DEVELOPERS"
            className="glass-card group hover:scale-105 transition-transform duration-300"
          >
            <div className="glass-card__content text-center">
              <div className="text-4xl mb-4">👨‍💻</div>
              <h3 className="text-xl font-bold text-forest-900 mb-2">Desenvolvedores</h3>
              <p className="text-forest-600 text-sm">Comece a desenvolver com o Araponga</p>
            </div>
          </Link>

          <Link
            href="/docs/ONBOARDING_ANALISTAS_FUNCIONAIS"
            className="glass-card group hover:scale-105 transition-transform duration-300"
          >
            <div className="glass-card__content text-center">
              <div className="text-4xl mb-4">👁️</div>
              <h3 className="text-xl font-bold text-forest-900 mb-2">Analistas</h3>
              <p className="text-forest-600 text-sm">Observe territórios e proponha melhorias</p>
            </div>
          </Link>

          <Link
            href="/docs/00_INDEX"
            className="glass-card group hover:scale-105 transition-transform duration-300"
          >
            <div className="glass-card__content text-center">
              <div className="text-4xl mb-4">📚</div>
              <h3 className="text-xl font-bold text-forest-900 mb-2">Índice Completo</h3>
              <p className="text-forest-600 text-sm">Explore toda a documentação</p>
            </div>
          </Link>
        </div>
      </main>

      <Footer />
    </div>
  );
}
