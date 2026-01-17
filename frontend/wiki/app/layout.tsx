import "./globals.css";
import type { Metadata } from "next";

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
    <html lang="pt-BR" className="scroll-smooth" suppressHydrationWarning>
      <body className="antialiased">
        <script
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
