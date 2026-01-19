import Link from "next/link";
import { readdir } from "fs/promises";
import { join } from "path";
// Header, Sidebar e Footer agora estão no layout.tsx raiz
import { CategoryCard } from "../../components/ui/CategoryCard";

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
  "Onboarding": [
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

  // Agrupar categorias por áreas semânticas para quebras visuais
  const coreCategories = [
    "Visão e Produto",
    "Arquitetura e Design",
  ];

  const communityCategories = [
    "Onboarding",
    "Desenvolvimento",
  ];

  const referenceCategories = [
    "Segurança",
    "Referência",
  ];

  const categoryGroups = [
    { title: "Fundamentos", categories: coreCategories },
    { title: "Desenvolvimento", categories: communityCategories },
    { title: "Referência", categories: referenceCategories },
  ];

  return (
    <main className="flex-1 container-max py-4 lg:py-6 px-4 md:px-6 lg:px-8">
      <div className="w-full mx-auto grid grid-cols-1 lg:grid-cols-[1fr_200px] xl:grid-cols-[1fr_220px] 2xl:grid-cols-[1fr_240px] gap-4 lg:gap-5 xl:gap-6">
        {/* Main Content Column */}
        <div>
        {/* Hero Section - Assertivo e direto */}
        <div className="mb-12 sm:mb-16 lg:mb-20 animation-fade-in">
          <h1 className="text-2xl sm:text-3xl md:text-4xl lg:text-5xl font-bold text-forest-900 dark:text-forest-50 mb-4 sm:mb-6 leading-tight tracking-tight">
            Documentação
          </h1>
          <p className="text-lg sm:text-xl md:text-2xl text-forest-600 dark:text-forest-400 max-w-3xl leading-relaxed">
            Documentação técnica, arquitetural e funcional da plataforma Araponga, organizada por categoria.
          </p>
        </div>

        {/* Categories Grid - Estrutura hierárquica enterprise-level com dois retângulos agrupadores */}
        <div className="space-y-10 sm:space-y-12 lg:space-y-16">
          {categoryGroups.map((group, groupIndex) => {
            const groupCategories = Object.entries(docStructure).filter(([category]) =>
              group.categories.includes(category)
            );

            return (
              <section key={group.title} id={group.title.toLowerCase().replace(/\s+/g, '-')} className="animation-fade-in scroll-mt-24">
                {/* Título de seção - Tipografia enterprise-level */}
                <h2 className="text-2xl sm:text-3xl md:text-4xl font-bold text-forest-900 dark:text-forest-50 mb-8 sm:mb-10 lg:mb-12 pb-4 border-b border-forest-200/60 dark:border-forest-800/60">
                  {group.title}
                </h2>

                {/* Dois retângulos agrupadores lado a lado - aproveita espaço horizontal */}
                {/* Mobile: 1 col, Desktop: 2 cols (dois retângulos lado a lado) */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 lg:gap-8 xl:gap-10">
                  {groupCategories.map(([category, docs], index) => (
                    <div
                      key={category}
                      className="category-group-card animation-slide-up"
                      style={{ animationDelay: `${(groupIndex * 2 + index) * 0.1}s` }}
                    >
                      <CategoryCard category={category} docs={docs} />
                    </div>
                  ))}
                </div>
              </section>
            );
          })}
        </div>
        </div>

        {/* TOC Column - Sticky */}
        <aside className="hidden lg:block">
          <div className="sticky top-24">
            <nav className="space-y-2" aria-label="Navegação de seções">
              <div className="text-sm font-semibold text-forest-700 dark:text-forest-300 mb-4 pb-2 border-b border-forest-200 dark:border-forest-800">
                Seções
              </div>
              {categoryGroups.map((group) => (
                <a
                  key={group.title}
                  href={`#${group.title.toLowerCase().replace(/\s+/g, '-')}`}
                  className="block text-sm text-forest-600 dark:text-forest-400 hover:text-forest-900 dark:hover:text-forest-50 py-1.5 transition-colors"
                >
                  {group.title}
                </a>
              ))}
            </nav>
          </div>
        </aside>
      </div>
    </main>
  );
}
