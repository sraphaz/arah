import Link from "next/link";
import { readdir, readFile } from "fs/promises";
import { join } from "path";
import matter from "gray-matter";
import { remark } from "remark";
import remarkHtml from "remark-html";
import remarkGfm from "remark-gfm";

// Estrutura hierárquica da documentação
const docStructure = {
  "Visão e Produto": [
    { name: "Visão do Produto", path: "01_PRODUCT_VISION.md" },
    { name: "Roadmap", path: "02_ROADMAP.md" },
    { name: "Backlog", path: "03_BACKLOG.md" },
    { name: "User Stories", path: "04_USER_STORIES.md" },
    { name: "Glossário", path: "05_GLOSSARY.md" },
  ],
  "Arquitetura e Design": [
    { name: "Decisões Arquiteturais", path: "10_ARCHITECTURE_DECISIONS.md" },
    { name: "Arquitetura de Services", path: "11_ARCHITECTURE_SERVICES.md" },
    { name: "Modelo de Domínio", path: "12_DOMAIN_MODEL.md" },
    { name: "Domain Routing", path: "13_DOMAIN_ROUTING.md" },
  ],
  "Onboarding e Comunidade": [
    { name: "Onboarding Público", path: "ONBOARDING_PUBLICO.md" },
    { name: "Onboarding para Desenvolvedores", path: "ONBOARDING_DEVELOPERS.md" },
    { name: "Onboarding para Analistas", path: "ONBOARDING_ANALISTAS_FUNCIONAIS.md" },
    { name: "Cartilha Completa", path: "CARTILHA_COMPLETA.md" },
    { name: "FAQ", path: "ONBOARDING_FAQ.md" },
    { name: "Mentoria", path: "MENTORIA.md" },
    { name: "Priorização de Propostas", path: "PRIORIZACAO_PROPOSTAS.md" },
  ],
  "Desenvolvimento": [
    { name: "Plano de Implementação", path: "20_IMPLEMENTATION_PLAN.md" },
    { name: "Revisão de Código", path: "21_CODE_REVIEW.md" },
    { name: "Coesão e Testes", path: "22_COHESION_AND_TESTS.md" },
    { name: "Implementação de Recomendações", path: "23_IMPLEMENTATION_RECOMMENDATIONS.md" },
    { name: "Estrutura do Projeto", path: "PROJECT_STRUCTURE.md" },
  ],
  "Segurança": [
    { name: "Configuração de Segurança", path: "SECURITY_CONFIGURATION.md" },
    { name: "Security Audit", path: "SECURITY_AUDIT.md" },
  ],
  "Referência": [
    { name: "Índice Completo", path: "00_INDEX.md" },
    { name: "Changelog", path: "40_CHANGELOG.md" },
    { name: "Contribuindo", path: "41_CONTRIBUTING.md" },
  ],
};

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
    };
  } catch (error) {
    console.error(`Error reading ${filePath}:`, error);
    return null;
  }
}

export default async function HomePage() {
  // Carregar conteúdo do índice principal
  const indexDoc = await getDocContent("00_INDEX.md");

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
        {/* Hero Section */}
        <div className="glass-card mb-8">
          <div className="glass-card__content">
            <h1 className="text-4xl font-bold text-forest-900 mb-4">
              Documentação Completa do Araponga
            </h1>
            <p className="text-xl text-forest-700 mb-6">
              Explore a documentação completa da plataforma: visão do produto, arquitetura,
              desenvolvimento, onboarding e muito mais.
            </p>
            <div className="flex flex-wrap gap-3">
              <Link
                href="/docs/ONBOARDING_PUBLICO"
                className="bg-forest-600 text-white px-6 py-3 rounded-lg hover:bg-forest-700 transition-colors"
              >
                Começar Aqui
              </Link>
              <Link
                href="/docs/00_INDEX"
                className="bg-forest-100 text-forest-800 px-6 py-3 rounded-lg hover:bg-forest-200 transition-colors"
              >
                Ver Índice Completo
              </Link>
            </div>
          </div>
        </div>

        {/* Categories Grid */}
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          {Object.entries(docStructure).map(([category, docs]) => (
            <div key={category} className="glass-card">
              <div className="glass-card__content">
                <h2 className="text-2xl font-semibold text-forest-900 mb-4">
                  {category}
                </h2>
                <ul className="space-y-2">
                  {docs.map((doc) => {
                    const docSlug = doc.path.replace(".md", "");
                    return (
                      <li key={doc.path}>
                        <Link
                          href={`/docs/${docSlug}`}
                          className="text-forest-700 hover:text-forest-900 hover:underline flex items-center space-x-2"
                        >
                          <span>→</span>
                          <span>{doc.name}</span>
                        </Link>
                      </li>
                    );
                  })}
                </ul>
              </div>
            </div>
          ))}
        </div>

        {/* Índice Completo (se disponível) */}
        {indexDoc && (
          <div className="glass-card mt-8">
            <div className="glass-card__content">
              <div
                className="markdown-content"
                dangerouslySetInnerHTML={{ __html: indexDoc.content }}
              />
            </div>
          </div>
        )}
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
