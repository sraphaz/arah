import Link from "next/link";
import { readdir } from "fs/promises";
import { join } from "path";
import { Header } from "../../components/layout/Header";
import { Footer } from "../../components/layout/Footer";
import { CategoryCard } from "../../components/ui/CategoryCard";

// Estrutura hierárquica da documentação
const docStructure = {
  "🎯 Visão e Produto": [
    { name: "Visão do Produto", path: "01_PRODUCT_VISION.md" },
    { name: "Roadmap", path: "02_ROADMAP.md" },
    { name: "Backlog", path: "03_BACKLOG.md" },
    { name: "User Stories", path: "04_USER_STORIES.md" },
    { name: "Glossário", path: "05_GLOSSARY.md" },
  ],
  "🏗️ Arquitetura e Design": [
    { name: "Decisões Arquiteturais", path: "10_ARCHITECTURE_DECISIONS.md" },
    { name: "Arquitetura de Services", path: "11_ARCHITECTURE_SERVICES.md" },
    { name: "Modelo de Domínio", path: "12_DOMAIN_MODEL.md" },
    { name: "Domain Routing", path: "13_DOMAIN_ROUTING.md" },
  ],
  "🌱 Onboarding e Comunidade": [
    { name: "Onboarding Público", path: "ONBOARDING_PUBLICO.md" },
    { name: "Onboarding para Desenvolvedores", path: "ONBOARDING_DEVELOPERS.md" },
    { name: "Onboarding para Analistas", path: "ONBOARDING_ANALISTAS_FUNCIONAIS.md" },
    { name: "Cartilha Completa", path: "CARTILHA_COMPLETA.md" },
    { name: "FAQ", path: "ONBOARDING_FAQ.md" },
    { name: "Mentoria", path: "MENTORIA.md" },
    { name: "Priorização de Propostas", path: "PRIORIZACAO_PROPOSTAS.md" },
  ],
  "🔧 Desenvolvimento": [
    { name: "Plano de Implementação", path: "20_IMPLEMENTATION_PLAN.md" },
    { name: "Revisão de Código", path: "21_CODE_REVIEW.md" },
    { name: "Coesão e Testes", path: "22_COHESION_AND_TESTS.md" },
    { name: "Implementação de Recomendações", path: "23_IMPLEMENTATION_RECOMMENDATIONS.md" },
    { name: "Estrutura do Projeto", path: "PROJECT_STRUCTURE.md" },
  ],
  "🛡️ Segurança": [
    { name: "Configuração de Segurança", path: "SECURITY_CONFIGURATION.md" },
    { name: "Security Audit", path: "SECURITY_AUDIT.md" },
  ],
  "📚 Referência": [
    { name: "Índice Completo", path: "00_INDEX.md" },
    { name: "Changelog", path: "40_CHANGELOG.md" },
    { name: "Contribuindo", path: "41_CONTRIBUTING.md" },
  ],
};

async function getAllDocs() {
  try {
    const docsPath = join(process.cwd(), "..", "..", "docs");
    const files = await readdir(docsPath);
    return files.filter((file) => file.endsWith(".md"));
  } catch (error) {
    console.error("Error reading docs directory:", error);
    return [];
  }
}

export default async function DocsPage() {
  const allDocs = await getAllDocs();

  return (
    <div className="min-h-screen flex flex-col">
      <Header />

      {/* Main Content */}
      <main className="flex-1 container-max py-12">
        {/* Hero Section */}
        <div className="glass-card mb-12 animation-fade-in">
          <div className="glass-card__content text-center">
            <h1 className="hero-title text-balance mb-4">
              Todos os Documentos
            </h1>
            <p className="hero-subtitle text-balance max-w-2xl mx-auto">
              Explore toda a documentação disponível na Wiki do Araponga.
            </p>
          </div>
        </div>

        {/* Categories Grid */}
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8 mb-12">
          {Object.entries(docStructure).map(([category, docs], index) => (
            <div
              key={category}
              className="animation-slide-up"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              <CategoryCard category={category} docs={docs} />
            </div>
          ))}
        </div>

        {/* All Docs List - Alfabética */}
        <div className="glass-card animation-fade-in">
          <div className="glass-card__content">
            <h2 className="text-3xl font-bold text-forest-900 mb-6">
              Lista Completa ({allDocs.length} documentos)
            </h2>
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-3">
              {allDocs
                .sort()
                .map((doc) => {
                  const docSlug = doc.replace(".md", "");
                  const docName = doc.replace(".md", "").replace(/_/g, " ");
                  return (
                    <Link
                      key={doc}
                      href={`/docs/${docSlug}`}
                      className="doc-link group"
                    >
                      <span>→</span>
                      <span className="flex-1">{docName}</span>
                    </Link>
                  );
                })}
            </div>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  );
}
