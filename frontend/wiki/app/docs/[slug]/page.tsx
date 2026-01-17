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
import { Sidebar } from "../../../components/layout/Sidebar";
import { MobileSidebar } from "../../../components/layout/MobileSidebar";
import { TableOfContents } from "../../../components/layout/TableOfContents";
import { ContentSections } from "./content-sections";

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

async function getDocContent(fileName: string) {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", fileName);
    const fileContents = await readFile(docsPath, "utf8");
    const { content, data } = matter(fileContents);

    const processedContent = await remark()
      .use(remarkHtml)
      .use(remarkGfm)
      .process(content);

    // Adiciona IDs aos headings para navegação
    let htmlContent = processedContent.toString();
    htmlContent = htmlContent.replace(
      /<h([2-4])>(.*?)<\/h\1>/gi,
      (match, level, text) => {
        const id = text
          .replace(/<[^>]*>/g, '') // Remove HTML tags
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
      <MobileSidebar />

      {/* Main Content with Sidebar */}
      <div className="flex-1 flex">
        <Sidebar />
        
        <main className="flex-1 py-12 px-4 md:px-8 lg:px-12">
          <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-[1fr_280px] gap-8">
            {/* Main Content Column */}
            <div>
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
                  <h1 className="text-5xl md:text-6xl font-bold text-forest-900 dark:text-forest-50 mb-8 leading-tight">
                    {doc.title}
                  </h1>

                  {/* Document Metadata - Badges */}
                  {doc.frontMatter && (doc.frontMatter.version || doc.frontMatter.date || doc.frontMatter.status) && (
                    <div className="mb-12 pb-6 border-b-2 border-forest-200/80 dark:border-forest-800/80 flex flex-wrap gap-3">
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

                  {/* Document Content - Refinado com Progressive Disclosure */}
                  <ContentSections htmlContent={doc.content} />
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
            </div>

            {/* TOC Column - Sticky */}
            <aside className="hidden lg:block">
              <div className="sticky top-24">
                <TableOfContents />
              </div>
            </aside>
          </div>
        </main>
      </div>

      <Footer />
    </div>
  );
}
