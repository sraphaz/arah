"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";

interface SidebarSection {
  title: string;
  items: {
    title: string;
    href: string;
    description?: string;
  }[];
}

const sidebarSections: SidebarSection[] = [
  {
    title: "Início",
    items: [
      { title: "Boas-Vindas", href: "/", description: "Página inicial e apresentação" },
    ],
  },
  {
    title: "Onboarding",
    items: [
      { title: "Analistas Funcionais", href: "/docs/ONBOARDING_ANALISTAS_FUNCIONAIS", description: "Análise funcional e territorial" },
      { title: "Desenvolvedores", href: "/docs/ONBOARDING_DEVELOPERS", description: "Comece a desenvolver" },
    ],
  },
  {
    title: "Documentação",
    items: [
      { title: "Índice Completo", href: "/docs/00_INDEX", description: "Todos os documentos" },
      { title: "Lista de Documentos", href: "/docs", description: "Navegar todos os docs" },
      { title: "Documentação Funcional", href: "/docs/funcional/00_PLATAFORMA_ARAPONGA", description: "Visão funcional completa da plataforma" },
    ],
  },
  {
    title: "Projeto",
    items: [
      { title: "Visão do Produto", href: "/docs/01_PRODUCT_VISION", description: "Visão e objetivos" },
      { title: "Roadmap", href: "/docs/02_ROADMAP", description: "Planejamento e fases" },
      { title: "Backlog", href: "/docs/backlog-api/README", description: "Backlog completo de fases" },
    ],
  },
  {
    title: "Arquitetura",
    items: [
      { title: "Decisões Arquiteturais", href: "/docs/10_ARCHITECTURE_DECISIONS", description: "ADRs e decisões técnicas" },
      { title: "Modelo de Domínio", href: "/docs/12_DOMAIN_MODEL", description: "Entidades e relações" },
      { title: "Serviços", href: "/docs/11_ARCHITECTURE_SERVICES", description: "Serviços da aplicação" },
    ],
  },
  {
    title: "Comunidade",
    items: [
      { title: "Discord", href: "/docs/DISCORD_SETUP", description: "Configuração e estrutura" },
      { title: "Contribuindo", href: "/docs/41_CONTRIBUTING", description: "Como contribuir" },
    ],
  },
];

export function Sidebar() {
  const pathname = usePathname();
  const [openSections, setOpenSections] = useState<Set<string>>(new Set(["Início"]));

  const toggleSection = (title: string) => {
    const newOpen = new Set(openSections);
    if (newOpen.has(title)) {
      newOpen.delete(title);
    } else {
      newOpen.add(title);
    }
    setOpenSections(newOpen);
  };

  // Auto-open section if current path matches (normalizado)
  const normalizedPathname = (pathname || '/').replace(/\/$/, '') || '/';
  const currentSection = sidebarSections.find((section) =>
    section.items.some((item) => {
      const normalizedHref = item.href.replace(/\/$/, '') || '/';
      return normalizedPathname === normalizedHref ||
             (normalizedHref !== '/' && normalizedPathname.startsWith(normalizedHref + '/'));
    })
  );
  if (currentSection && !openSections.has(currentSection.title)) {
    setOpenSections(new Set([...openSections, currentSection.title]));
  }

  return (
    <aside className="sidebar-container">
      <nav className="sidebar-nav" aria-label="Navegação principal">
        {sidebarSections.map((section) => {
          const isOpen = openSections.has(section.title);
          return (
            <div key={section.title} className="sidebar-section">
              <button
                onClick={() => toggleSection(section.title)}
                className="sidebar-section-toggle"
                aria-expanded={isOpen}
              >
                <span className="sidebar-section-title">{section.title}</span>
                <svg
                  className={`sidebar-chevron ${isOpen ? "sidebar-chevron-open" : ""}`}
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                  strokeWidth={2}
                >
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                </svg>
              </button>

              {isOpen && (
                <ul className="sidebar-items">
                  {section.items.map((item) => {
                    // Normalizar pathname e href para comparação (remover trailing slash)
                    const normalizedPathname = pathname.replace(/\/$/, '') || '/';
                    const normalizedHref = item.href.replace(/\/$/, '') || '/';
                    // Lógica mais precisa: /docs só é ativo em /docs exatamente, não em /docs/qualquer-coisa
                    // Para outras rotas, permite sub-rotas (ex: /docs/ONBOARDING_DEVELOPERS)
                    const isActive = normalizedPathname === normalizedHref ||
                                   (normalizedHref !== '/' &&
                                    normalizedHref !== '/docs' && // Exceção: /docs só é ativo em /docs
                                    normalizedPathname.startsWith(normalizedHref + '/'));
                    return (
                      <li key={item.href}>
                        <Link
                          href={item.href}
                          className={`sidebar-link ${isActive ? "sidebar-link-active" : ""}`}
                        >
                          <span className="sidebar-link-title">{item.title}</span>
                          {item.description && (
                            <span className="sidebar-link-description">{item.description}</span>
                          )}
                        </Link>
                      </li>
                    );
                  })}
                </ul>
              )}
            </div>
          );
        })}
      </nav>
    </aside>
  );
}

