import Link from "next/link";
import Image from "next/image";
import { brand } from "../../../shared/config/brand";

export default function SiteHeader() {
  return (
    <header className="sticky top-0 z-50 border-b border-forest-200/80 bg-white/90 backdrop-blur-xl" role="banner">
      <div className="container-max flex items-center justify-between py-4">
        <Link href="/" className="flex items-center gap-3" aria-label={`${brand.name} — página inicial`}>
          <Image src="/Logo_1.png" alt={`${brand.name} logo`} width={40} height={40} className="h-10 w-10 object-contain" />
          <span className="text-xl font-bold tracking-tight text-forest-950">{brand.name}</span>
        </Link>
        <nav className="flex items-center gap-4 text-sm font-medium text-forest-800" aria-label="Navegação principal">
          <a href={brand.urls.wiki} target="_blank" rel="noopener noreferrer" className="hover:text-forest-950" aria-label="Abrir a Wiki do Arah em nova aba">
            Wiki
          </a>
          <a href={brand.urls.devportal} target="_blank" rel="noopener noreferrer" className="hover:text-forest-950" aria-label="Abrir o DevPortal da API em nova aba">
            DevPortal
          </a>
          <a href={brand.urls.github} target="_blank" rel="noopener noreferrer" className="hover:text-forest-950" aria-label="Abrir o repositório no GitHub em nova aba">
            GitHub
          </a>
        </nav>
      </div>
    </header>
  );
}
