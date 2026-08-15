import "./globals.css";
import type { Metadata } from "next";
import Script from "next/script";
import { Header } from "../components/layout/Header";
import { Sidebar } from "../components/layout/Sidebar";
import { MobileSidebar } from "../components/layout/MobileSidebar";
import { Footer } from "../components/layout/Footer";
import { brand } from "../../shared/config/brand";

// Fontes via CSS (system stack) — evita falha flaky do CI ao baixar Google Fonts (fonts.gstatic.com).
// Tokens --font-inter / --font-mono continuam definidos em styles/variables.css.

const siteUrl = brand.urls.wiki;

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: `${brand.wikiTitle} — ${brand.tagline}`,
  description: brand.description,
  openGraph: {
    type: "website",
    url: siteUrl,
    title: `${brand.wikiTitle} — ${brand.tagline}`,
    description: brand.description,
  },
  icons: {
    icon: "/wiki/favicon.png",
    apple: "/wiki/icon.png"
  }
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="pt-BR" className="scroll-smooth dark" suppressHydrationWarning>
      <body className="antialiased font-sans">
        <Script
          id="theme-init"
          strategy="beforeInteractive"
          dangerouslySetInnerHTML={{
            __html: `
              (function() {
                try {
                  // Garantia adicional: verifica novamente após DOM carregar
                  const savedTheme = localStorage.getItem('wiki-theme');
                  const theme = savedTheme || 'dark';
                  
                  // Aplica imediatamente antes do render para evitar flash
                  if (theme === 'dark') {
                    document.documentElement.classList.add('dark');
                  } else {
                    document.documentElement.classList.remove('dark');
                  }
                  
                  // Garante que a classe persiste mesmo após navegação
                  document.documentElement.setAttribute('data-theme', theme);
                  
                  // Log para debug (apenas em desenvolvimento)
                  if (typeof console !== 'undefined' && console.log) {
                    console.log('[Theme Init] Theme:', theme, 'Saved:', savedTheme);
                  }
                } catch (e) {
                  // Fallback: aplica dark mode em caso de erro
                  try {
                    document.documentElement.classList.add('dark');
                    document.documentElement.setAttribute('data-theme', 'dark');
                    if (typeof console !== 'undefined' && console.error) {
                      console.error('[Theme Init] Error:', e);
                    }
                  } catch (fallbackError) {
                    // Ignora se nem o fallback funcionar
                  }
                }
              })();
            `,
          }}
        />
        <div className="min-h-screen flex flex-col">
          <Header />
          <MobileSidebar />
          <div className="flex-1 flex">
            <Sidebar />
            <div className="flex-1">
              {children}
            </div>
          </div>
          <Footer />
        </div>
      </body>
    </html>
  );
}
