import Link from "next/link";
import Image from "next/image";
import { ThemeToggle } from "../ui/ThemeToggle";

export function Header() {
  return (
    <header className="border-b border-forest-200/80 dark:border-forest-900/80 bg-white/90 dark:bg-forest-950/90 backdrop-blur-xl sticky top-0 z-50 shadow-sm dark:shadow-forest-900/20 transition-all duration-300">
      <div className="container-max py-6">
        <div className="flex items-center justify-between">
          <Link href="/" className="flex items-center space-x-4 group transition-transform duration-300 hover:scale-[1.02]">
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
          <nav className="flex items-center space-x-3">
            <Link href="/" className="nav-link">
              Boas-Vindas
            </Link>
            <Link href="/docs" className="nav-link">
              Todos os Docs
            </Link>
            <a
              href="https://araponga.app"
              target="_blank"
              rel="noopener noreferrer"
              className="nav-link"
            >
              Site Principal
            </a>
            <div className="ml-2 pl-3 border-l border-forest-200 dark:border-forest-800">
              <ThemeToggle />
            </div>
          </nav>
        </div>
      </div>
    </header>
  );
}
