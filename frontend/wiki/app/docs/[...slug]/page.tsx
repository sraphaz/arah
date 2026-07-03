import Link from "next/link";
import { notFound } from "next/navigation";
import { readdir } from "fs/promises";
import { join } from "path";
import { TableOfContents } from "../../../components/layout/TableOfContents";
import { YamlDownloadButton } from "../../../components/YamlDownloadButton";
import { MermaidContent } from "../../../components/content/MermaidContent";
import { processMarkdownContent, getYamlContent } from "../../../lib/document";

interface PageProps {
  params: Promise<{ slug: string[] }>;
}

async function getAllDocsRecursive(basePath: string = ''): Promise<string[]> {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs", basePath);
    const files: string[] = [];
    const entries = await readdir(docsPath, { withFileTypes: true });

    for (const entry of entries) {
      // Normaliza separadores de caminho para '/' (funciona em todos os sistemas)
      const fullPath = basePath
        ? `${basePath}/${entry.name}`.replace(/\\/g, '/')
        : entry.name;

      if (entry.isDirectory()) {
        // Recursivamente busca em subdiretórios
        const subFiles = await getAllDocsRecursive(fullPath);
        files.push(...subFiles);
      } else if (entry.isFile() && (entry.name.endsWith('.md') || entry.name.endsWith('.yaml') || entry.name.endsWith('.yml'))) {
        // Adiciona arquivo (sem extensão para o slug)
        const relativePath = fullPath.replace(/\.(md|yaml|yml)$/, '');
        files.push(relativePath);
      }
    }

    return files;
  } catch (error) {
    console.error(`Error reading docs directory ${basePath}:`, error);
    return [];
  }
}

export async function generateStaticParams() {
  const docs = await getAllDocsRecursive('');
  return docs.map((doc) => ({
    slug: doc.split('/'),
  }));
}

export default async function DocPage({ params }: PageProps) {
  const { slug } = await params;

  // slug é um array, ex: ['backlog-api', 'README']
  // Decodifica cada segmento do slug (remove URL encoding) e constrói o caminho
  const decodedSlug = slug.map((segment: string) => {
    try {
      return decodeURIComponent(segment);
    } catch {
      return segment;
    }
  });
  
  // Tenta primeiro como .md, depois como .yaml/.yml
  let filePath = `${decodedSlug.join('/')}.md`.replace(/\\/g, '/');
  let doc = await processMarkdownContent(filePath, { processMermaid: true });
  let yamlDoc = null;
  
  // Se não encontrou como .md, tenta como YAML
  if (!doc) {
    const yamlFilePath = `${decodedSlug.join('/')}.yaml`.replace(/\\/g, '/');
    yamlDoc = await getYamlContent(yamlFilePath);
    
    // Se não encontrou .yaml, tenta .yml
    if (!yamlDoc) {
      const ymlFilePath = `${decodedSlug.join('/')}.yml`.replace(/\\/g, '/');
      yamlDoc = await getYamlContent(ymlFilePath);
    }
  }

  if (!doc && !yamlDoc) {
    notFound();
  }

  return (
        <main className="flex-1 py-4 lg:py-6 px-4 md:px-6 lg:px-8">
      <div className="w-full mx-auto grid grid-cols-1 lg:grid-cols-[1fr_240px] xl:grid-cols-[1fr_260px] 2xl:grid-cols-[1fr_280px] gap-4 lg:gap-6 xl:gap-8">
        {/* Main Content Column */}
        <div>
          <div className="glass-card animation-fade-in">
            <div className="glass-card__content">
              {/* Breadcrumb Refinado */}
              <nav className="breadcrumb mb-8">
                <Link href="/" prefetch={false}>Boas-Vindas</Link>
                <span>›</span>
                <Link href="/docs" prefetch={false}>Documentação</Link>
                {(slug[0]?.startsWith('ONBOARDING_') || slug.some(s => s?.startsWith('ONBOARDING_'))) && (
                  <span className="inline-flex items-center gap-1">
                    <span>›</span>
                    <Link href="/docs" prefetch={false}>Onboarding</Link>
                  </span>
                )}
                <span>›</span>
                <span className="text-forest-900 font-medium">{doc?.title || yamlDoc?.title}</span>
              </nav>

              {/* Document Title - Hero */}
              <h1 className="text-2xl md:text-3xl lg:text-4xl font-bold text-forest-900 dark:text-forest-50 mb-6 leading-tight">
                {doc?.title || yamlDoc?.title}
              </h1>

              {/* Botão de Download para YAML */}
              {yamlDoc && (
                <YamlDownloadButton fileName={yamlDoc.fileName} content={yamlDoc.content} />
              )}

              {/* Document Content - Refinado com Progressive Disclosure e suporte a Mermaid */}
              {doc && <MermaidContent htmlContent={doc.content} />}
              
              {/* YAML Content com syntax highlighting */}
              {yamlDoc && (
                <div className="markdown-content">
                  <pre className="bg-forest-900 dark:bg-dark-bg text-forest-50 p-6 rounded-2xl overflow-x-auto mb-10 border border-forest-800 dark:border-dark-border shadow-lg">
                    <code className="language-yaml">{yamlDoc.content}</code>
                  </pre>
                </div>
              )}
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
