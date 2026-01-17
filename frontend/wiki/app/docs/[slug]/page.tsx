import Link from "next/link";
import { notFound } from "next/navigation";
import { readdir, readFile } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";
import { Header } from "../../../components/layout/Header";
import { Footer } from "../../../components/layout/Footer";

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
};

async function getDocContent(fileName: string) {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", fileName);
    const fileContents = await readFile(docsPath, "utf8");
    const { content, data } = matter(fileContents);

    const processedContent = await remark()
      .use(remarkHtml)
      .use(remarkGfm)
      .process(content);

    return {
      content: processedContent.toString(),
      frontMatter: data,
      title: data.title || fileName.replace(".md", "").replace(/_/g, " "),
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

export async function generateStaticParams() {
  const docs = await getAllDocs();
  return docs.map((doc) => ({
    slug: doc,
  }));
}

export default async function DocPage({ params }: PageProps) {
  const { slug } = await params;
  const fileName = slugToFile[slug] || `${slug}.md`;
  const doc = await getDocContent(fileName);

  if (!doc) {
    notFound();
  }

  return (
    <div className="min-h-screen flex flex-col">
      <Header />

      {/* Main Content */}
      <main className="flex-1 container-max py-12">
        <div className="glass-card animation-fade-in">
          <div className="glass-card__content">
            {/* Breadcrumb Refinado */}
            <nav className="breadcrumb mb-8">
              <Link href="/">Boas-Vindas</Link>
              <span>›</span>
              <Link href="/docs">Documentação</Link>
              <span>›</span>
              <span className="text-forest-900 font-medium">{doc.title}</span>
            </nav>

            {/* Document Title - Hero */}
            <h1 className="text-5xl md:text-6xl font-bold text-forest-900 mb-8 leading-tight">
              {doc.title}
            </h1>

            {/* Document Metadata - Badges */}
            {doc.frontMatter && (doc.frontMatter.version || doc.frontMatter.date || doc.frontMatter.status) && (
              <div className="mb-10 pb-8 border-b border-forest-200/60 flex flex-wrap gap-3">
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

            {/* Document Content - Refinado */}
            <div
              className="markdown-content prose-headings:first:mt-0"
              dangerouslySetInnerHTML={{ __html: doc.content }}
            />
          </div>
        </div>

        {/* Navigation Links - Refinado */}
        <div className="mt-12 flex flex-col sm:flex-row justify-between gap-4">
          <Link
            href="/"
            className="btn-secondary text-center"
          >
            ← Voltar às Boas-Vindas
          </Link>
          <Link
            href="/docs"
            className="btn-secondary text-center"
          >
            Ver Todos os Docs →
          </Link>
        </div>
      </main>

      <Footer />
    </div>
  );
}
