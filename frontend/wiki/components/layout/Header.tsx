import Link from "next/link";
import { ThemeToggle } from "../ui/ThemeToggle";

export function Header() {
  return (
    <header className="border-b border-forest-200/80 dark:border-forest-900/80 bg-white/80 dark:bg-forest-950/80 backdrop-blur-xl sticky top-0 z-50 shadow-sm transition-colors duration-300">
      <div className="container-max py-5">
        <div className="flex items-center justify-between">
          <Link href="/" className="flex items-center space-x-3 group">
            <span className="text-3xl">🐦</span>
            <div>
              <h1 className="text-2xl font-bold text-forest-900 dark:text-forest-50 group-hover:text-forest-700 dark:group-hover:text-forest-200 transition-colors">
                Wiki Araponga
              </h1>
              <p className="text-xs text-forest-500 dark:text-forest-400 font-medium">Documentação Completa</p>
            </div>
          </Link>
          <nav className="flex items-center space-x-2">
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
            <ThemeToggle />
          </nav>
        </div>
      </div>
    </header>
  );
}
