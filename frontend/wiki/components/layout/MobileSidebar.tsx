"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";

interface MobileSidebarSection {
  title: string;
  items: {
    title: string;
    href: string;
    description?: string;
  }[];
}

const sidebarSections: MobileSidebarSection[] = [
  {
    title: "Início",
    items: [
      { title: "Boas-Vindas", href: "/", description: "Página inicial e apresentação" },
    ],
  },
  {
    title: "Onboarding",
    items: [
      { title: "Desenvolvedores", href: "/docs/ONBOARDING_DEVELOPERS", description: "Comece a desenvolver" },
      { title: "Analistas Funcionais", href: "/docs/ONBOARDING_ANALISTAS_FUNCIONAIS", description: "Análise funcional e territorial" },
    ],
  },
  {
    title: "Documentação",
    items: [
      { title: "Índice Completo", href: "/docs/00_INDEX", description: "Todos os documentos" },
      { title: "Lista de Documentos", href: "/docs", description: "Navegar todos os docs" },
    ],
  },
];

export function MobileSidebar() {
  const [isOpen, setIsOpen] = useState(false);
  const pathname = usePathname();

  // Fecha menu ao redimensionar para desktop
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 1024 && isOpen) {
        setIsOpen(false);
      }
    };

    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, [isOpen]);

  // Previne scroll do body quando menu está aberto
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }

    return () => {
      document.body.style.overflow = "";
    };
  }, [isOpen]);

  // Fecha menu ao pressionar ESC
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape" && isOpen) {
        setIsOpen(false);
      }
    };

    document.addEventListener("keydown", handleEscape);
    return () => document.removeEventListener("keydown", handleEscape);
  }, [isOpen]);

  return (
    <>
      {/* Mobile Menu Button - Mobile-first: visível até 1023px */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className={`fixed top-24 right-4 z-50 p-3 rounded-xl text-white shadow-lg hover:shadow-xl transition-all duration-300 lg:hidden ${
          isOpen
            ? "bg-forest-600 dark:bg-[#4dd4a8]"
            : "bg-forest-600/90 dark:bg-[#4dd4a8]/90 hover:bg-forest-600 dark:hover:bg-[#4dd4a8]"
        }`}
        aria-label={isOpen ? "Fechar menu de navegação" : "Abrir menu de navegação"}
        aria-expanded={isOpen}
        aria-controls="mobile-sidebar"
      >
        <svg
          className={`w-6 h-6 transition-transform duration-300 ${isOpen ? "rotate-90" : ""}`}
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          strokeWidth={2}
        >
          {isOpen ? (
            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
          ) : (
            <path strokeLinecap="round" strokeLinejoin="round" d="M4 6h16M4 12h16M4 18h16" />
          )}
        </svg>
      </button>

      {/* Mobile Menu Overlay - Mobile-first: até 1023px */}
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/50 dark:bg-black/70 backdrop-blur-sm z-40 lg:hidden transition-opacity duration-300"
          onClick={() => setIsOpen(false)}
          aria-hidden={!isOpen}
        />
      )}

      {/* Mobile Menu Panel - Mobile-first: oculto acima de 1024px - Modernizado */}
      <aside
        id="mobile-sidebar"
        className={`fixed top-0 left-0 h-full w-[280px] sm:w-[320px] max-w-[90vw] z-50 bg-white/95 dark:bg-forest-950/95 backdrop-blur-xl border-r border-forest-200/80 dark:border-forest-800/80 transform transition-transform duration-300 ease-in-out flex flex-col lg:hidden ${
          isOpen ? "translate-x-0" : "-translate-x-full"
        }`}
        style={{
          width: 'clamp(280px, 85vw, 320px)'
        }}
        aria-label="Menu de navegação mobile"
        aria-hidden={!isOpen}
      >
        <div className="p-6 border-b border-forest-200/80 dark:border-forest-800/80">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-bold text-forest-900 dark:text-forest-50">Navegação</h2>
            <button
              onClick={() => setIsOpen(false)}
              className="p-2 rounded-lg hover:bg-forest-100/80 dark:hover:bg-forest-900/80 transition-colors"
              aria-label="Fechar menu"
            >
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          {/* Links rápidos do header (acessíveis no menu mobile) */}
          <div className="flex flex-col space-y-2 pt-2 border-t border-forest-200/80 dark:border-forest-800/80">
            <Link
              href="/docs"
              onClick={() => setIsOpen(false)}
              className="px-3 py-2 rounded-lg text-sm text-forest-700 dark:text-forest-300 hover:bg-forest-100/80 dark:hover:bg-forest-900/60 transition-colors"
            >
              Todos os Docs
            </Link>
            <a
              href="/"
              onClick={() => setIsOpen(false)}
              className="px-3 py-2 rounded-lg text-sm text-forest-700 dark:text-forest-300 hover:bg-forest-100/80 dark:hover:bg-forest-900/60 transition-colors"
              title="DevPortal - Referência de API"
            >
              DevPortal
            </a>
            <a
              href="https://araponga.app"
              target="_blank"
              rel="noopener noreferrer"
              onClick={() => setIsOpen(false)}
              className="px-3 py-2 rounded-lg text-sm text-forest-700 dark:text-forest-300 hover:bg-forest-100/80 dark:hover:bg-forest-900/60 transition-colors"
            >
              Site Institucional
            </a>
          </div>
        </div>

        <nav className="p-4 space-y-2 overflow-y-auto flex-1">
          {sidebarSections.map((section) => (
            <div key={section.title} className="mb-4">
              <h3 className="px-3 py-2 text-sm font-semibold text-forest-600 dark:text-forest-400 uppercase tracking-wide">
                {section.title}
              </h3>
              <ul className="space-y-1">
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
                        onClick={() => setIsOpen(false)}
                        className={`block px-3 py-2.5 rounded-lg text-sm transition-all duration-300 ${
                          isActive
                            ? "bg-forest-100 dark:bg-forest-900/80 text-forest-900 dark:text-forest-50 font-medium"
                            : "text-forest-700 dark:text-forest-300 hover:bg-forest-100/80 dark:hover:bg-forest-900/60"
                        }`}
                      >
                        <span className="block">{item.title}</span>
                        {item.description && (
                          <span className="block text-xs mt-1 text-forest-500 dark:text-forest-400 opacity-75">
                            {item.description}
                          </span>
                        )}
                      </Link>
                    </li>
                  );
                })}
              </ul>
            </div>
          ))}
        </nav>
      </aside>
    </>
  );
}

