import "./globals.css";
import type { Metadata } from "next";
import Script from "next/script";
import { Inter } from "next/font/google";
import { JetBrains_Mono } from "next/font/google";
import { Header } from "../components/layout/Header";
import { Sidebar } from "../components/layout/Sidebar";
import { MobileSidebar } from "../components/layout/MobileSidebar";
import { Footer } from "../components/layout/Footer";

// Inter - Fonte moderna e sóbria para corpo de texto
// Usada por Vercel, Stripe, Linear - transmite profissionalismo e modernidade
const inter = Inter({
  subsets: ["latin"],
  variable: "--font-inter",
  display: "swap",
  weight: ["400", "500", "600", "700"],
  preload: true,
});

// JetBrains Mono - Fonte técnica e sóbria para código
// Excelente legibilidade, usada em documentação técnica enterprise
const jetbrainsMono = JetBrains_Mono({
  subsets: ["latin"],
  variable: "--font-mono",
  display: "swap",
  weight: ["400", "500", "600", "700"],
  preload: true,
});

const siteUrl = "https://devportal.araponga.app/wiki";

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: "Wiki Araponga — Documentação Completa",
  description:
    "Documentação completa do Araponga: visão do produto, arquitetura, desenvolvimento, onboarding e mais.",
  openGraph: {
    type: "website",
    url: siteUrl,
    title: "Wiki Araponga — Documentação Completa",
    description:
      "Documentação completa do Araponga: visão do produto, arquitetura, desenvolvimento, onboarding e mais.",
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
    <html lang="pt-BR" className={`scroll-smooth dark ${inter.variable} ${jetbrainsMono.variable}`} suppressHydrationWarning>
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
