import Link from "next/link";
import Image from "next/image";
import { ThemeToggle } from "../ui/ThemeToggle";
import { SearchTrigger } from "../search/SearchTrigger";

export function Header() {
  return (
    <header className="border-b border-forest-200/80 dark:border-forest-900/80 bg-white/90 dark:bg-forest-950/90 backdrop-blur-xl sticky top-0 z-50 shadow-sm dark:shadow-forest-900/20 transition-all duration-300">
      <div className="container-max py-6">
        <div className="flex items-center justify-between">
          <Link href="/" prefetch={false} className="flex items-center space-x-4 group transition-transform duration-300 hover:scale-[1.02]">
            <div className="relative w-14 h-14 md:w-16 md:h-16 flex-shrink-0 transition-transform duration-300 group-hover:scale-110 group-hover:rotate-3">
              <Image
                src="/wiki/icon.png"
                alt="Araponga Logo"
                width={64}
                height={64}
                className="object-contain dark:brightness-110 dark:contrast-105"
                priority
                loading="eager"
                style={{ opacity: 1 }}
              />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-forest-900 dark:text-forest-50 group-hover:text-forest-700 dark:group-hover:text-forest-200 transition-colors leading-tight">
                Wiki Araponga
              </h1>
              <p className="text-xs text-forest-500 dark:text-forest-400 font-medium tracking-wide">Documentação Completa</p>
            </div>
          </Link>
          <nav className="flex items-center space-x-2 lg:space-x-3">
            <SearchTrigger />
            {/* Links de navegação - apenas ícones, ocultos em mobile, visíveis em desktop */}
            <div className="hidden lg:flex items-center gap-3">
              <Link
                href="/docs"
                prefetch={false}
                className="header-link-icon"
                aria-label="Todos os Docs"
                title="Todos os Docs"
              >
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </Link>
              <a
                href="https://devportal.araponga.app"
                target="_blank"
                rel="noopener noreferrer"
                className="header-link-icon"
                aria-label="DevPortal - Referência de API"
                title="DevPortal - Referência de API"
              >
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
                </svg>
              </a>
              <a
                href="https://araponga.app"
                target="_blank"
                rel="noopener noreferrer"
                className="header-link-icon"
                aria-label="Site Institucional"
                title="Site Institucional"
              >
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                </svg>
              </a>
            </div>
            <div className="ml-0 lg:ml-2 lg:pl-3 lg:border-l border-forest-200 dark:border-forest-800">
              <ThemeToggle />
            </div>
          </nav>
        </div>
      </div>
    </header>
  );
}
