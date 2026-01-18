import "./globals.css";
import type { Metadata } from "next";

// Detecta URL base do site baseado no ambiente
function getSiteUrl(): string {
  // Em produção, usa variável de ambiente ou URL padrão
  if (process.env.NEXT_PUBLIC_SITE_URL) {
    return process.env.NEXT_PUBLIC_SITE_URL;
  }

  // Em desenvolvimento, detecta automaticamente
  if (process.env.NODE_ENV === "development") {
    const port = process.env.PORT || 3000;
    return `http://localhost:${port}`;
  }

  // Fallback para produção
  return "https://araponga.app";
}

const siteUrl = getSiteUrl();

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: "ARAPONGA — Território-Primeiro & Comunidade-Primeiro",
  description:
    "Plataforma orientada ao território para organização comunitária local. Território primeiro, comunidade primeiro.",
  openGraph: {
    type: "website",
    url: siteUrl,
    title: "ARAPONGA — Território-Primeiro & Comunidade-Primeiro",
    description:
      "Plataforma orientada ao território para organização comunitária local. Território primeiro, comunidade primeiro.",
    images: [
      {
        url: "/og.png",
        width: 1200,
        height: 630,
        alt: "ARAPONGA"
      }
    ]
  },
  twitter: {
    card: "summary_large_image",
    title: "ARAPONGA — Território-Primeiro & Comunidade-Primeiro",
    description:
      "Plataforma orientada ao território para organização comunitária local.",
    images: ["/og.png"]
  },
  icons: {
    icon: "/favicon.png",
    apple: "/icon.png"
  }
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="pt-BR">
      <body>{children}</body>
    </html>
  );
}
