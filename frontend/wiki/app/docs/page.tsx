import Link from "next/link";
import { readdir } from "fs/promises";
import { join } from "path";
// Header, Sidebar e Footer agora estão no layout.tsx raiz
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
    <main className="flex-1 container-max py-12 xl:py-16">
        {/* Hero Section - Assertivo e direto */}
        <div className="mb-16 animation-fade-in">
          <h1 className="text-5xl md:text-6xl lg:text-7xl font-bold text-forest-900 dark:text-forest-50 mb-6 leading-tight tracking-tight">
            Documentação
          </h1>
          <p className="text-xl md:text-2xl text-forest-600 dark:text-forest-400 max-w-3xl leading-relaxed">
            Documentação técnica, arquitetural e funcional da plataforma Araponga, organizada por categoria.
          </p>
        </div>

        {/* Categories Grid - Responsivo e profissional */}
        <div className="grid md:grid-cols-2 xl:grid-cols-3 gap-6 lg:gap-8">
          {Object.entries(docStructure).map(([category, docs], index) => (
            <div
              key={category}
              className="animation-slide-up"
              style={{ animationDelay: `${index * 0.08}s` }}
            >
              <CategoryCard category={category} docs={docs} />
            </div>
          ))}
        </div>
    </main>
  );
}
