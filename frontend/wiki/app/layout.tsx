import "./globals.css";
import type { Metadata } from "next";
import Script from "next/script";
import { Inter } from "next/font/google";
import { JetBrains_Mono } from "next/font/google";

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
    icon: "/favicon.png",
    apple: "/icon.png"
  }
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="pt-BR" className={`scroll-smooth ${inter.variable} ${jetbrainsMono.variable}`} suppressHydrationWarning>
      <body className="antialiased font-sans">
        <Script
          id="theme-init"
          strategy="beforeInteractive"
          dangerouslySetInnerHTML={{
            __html: `
              (function() {
                try {
                  const savedTheme = localStorage.getItem('wiki-theme');
                  const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
                  const theme = savedTheme || (systemPrefersDark ? 'dark' : 'light');
                  if (theme === 'dark') {
                    document.documentElement.classList.add('dark');
                  }
                } catch (e) {}
              })();
            `,
          }}
        />
        {children}
      </body>
    </html>
  );
}
