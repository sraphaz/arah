import Link from "next/link";
import { notFound } from "next/navigation";
import { readdir, readFile } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";

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
    // Caminho relativo para a pasta docs na raiz do projeto
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
      title: data.title || fileName.replace(".md", ""),
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
    <div className="min-h-screen">
      {/* Header */}
      <header className="border-b border-forest-200 bg-white/50 backdrop-blur-sm sticky top-0 z-50">
        <div className="container-max py-4">
          <div className="flex items-center justify-between">
            <Link href="/" className="flex items-center space-x-3">
              <h1 className="text-2xl font-bold text-forest-900">🐦 Wiki Araponga</h1>
            </Link>
            <nav className="flex items-center space-x-4">
              <Link href="/" className="nav-link">Início</Link>
              <Link href="/docs" className="nav-link">Todos os Docs</Link>
              <a
                href="https://araponga.app"
                target="_blank"
                rel="noopener noreferrer"
                className="nav-link"
              >
                Site Principal
              </a>
            </nav>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="container-max py-8">
        <div className="glass-card">
          <div className="glass-card__content">
            {/* Breadcrumb */}
            <nav className="mb-6 text-sm text-forest-600">
              <Link href="/" className="hover:underline">
                Início
              </Link>
              <span className="mx-2">/</span>
              <Link href="/docs" className="hover:underline">
                Documentação
              </Link>
              <span className="mx-2">/</span>
              <span className="text-forest-900 font-medium">{doc.title}</span>
            </nav>

            {/* Document Title */}
            <h1 className="text-4xl font-bold text-forest-900 mb-6">{doc.title}</h1>

            {/* Document Metadata */}
            {doc.frontMatter && (
              <div className="mb-6 pb-6 border-b border-forest-200">
                {doc.frontMatter.version && (
                  <p className="text-sm text-forest-600">
                    <span className="font-semibold">Versão:</span> {doc.frontMatter.version}
                  </p>
                )}
                {doc.frontMatter.date && (
                  <p className="text-sm text-forest-600">
                    <span className="font-semibold">Data:</span> {doc.frontMatter.date}
                  </p>
                )}
                {doc.frontMatter.status && (
                  <p className="text-sm text-forest-600">
                    <span className="font-semibold">Status:</span> {doc.frontMatter.status}
                  </p>
                )}
              </div>
            )}

            {/* Document Content */}
            <div
              className="markdown-content"
              dangerouslySetInnerHTML={{ __html: doc.content }}
            />
          </div>
        </div>

        {/* Navigation Links */}
        <div className="mt-8 flex justify-between">
          <Link
            href="/"
            className="nav-link bg-forest-100 px-6 py-3 rounded-lg hover:bg-forest-200 transition-colors"
          >
            ← Voltar ao Início
          </Link>
          <Link
            href="/docs"
            className="nav-link bg-forest-100 px-6 py-3 rounded-lg hover:bg-forest-200 transition-colors"
          >
            Ver Todos os Docs →
          </Link>
        </div>
      </main>

      {/* Footer */}
      <footer className="border-t border-forest-200 bg-white/50 backdrop-blur-sm mt-16">
        <div className="container-max py-8">
          <div className="text-center text-forest-600">
            <p>
              Wiki Araponga — Documentação completa da plataforma digital comunitária
              orientada ao território
            </p>
            <p className="mt-2 text-sm">
              <a
                href="https://github.com/sraphaz/araponga"
                target="_blank"
                rel="noopener noreferrer"
                className="hover:underline"
              >
                Contribuir no GitHub
              </a>
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}
