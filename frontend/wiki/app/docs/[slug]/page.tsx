import Link from "next/link";
import { notFound } from "next/navigation";
import { readdir } from "fs/promises";
import { join } from "path";
// Header, Sidebar e Footer agora estão no layout.tsx raiz
import { TableOfContents } from "../../../components/layout/TableOfContents";
import { ContentSectionsProgressive } from "./content-sections-progressive";
import { processMarkdownContent } from "../../../lib/document";

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
  const doc = await processMarkdownContent(fileName);

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
