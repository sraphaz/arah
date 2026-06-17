import "./globals.css";
import type { Metadata } from "next";
import SiteHeader from "@/components/layout/SiteHeader";
import { brand } from "../../shared/config/brand";

function getSiteUrl(): string {
  if (process.env.NEXT_PUBLIC_SITE_URL) {
    return process.env.NEXT_PUBLIC_SITE_URL;
  }

  if (process.env.NODE_ENV === "development") {
    const port = process.env.PORT || 3000;
    return `http://localhost:${port}`;
  }

  return brand.urls.site;
}

const siteUrl = getSiteUrl();
const pageTitle = `${brand.name} — Território-Primeiro & Comunidade-Primeiro`;
const pageDescription =
  "Plataforma orientada ao território para organização comunitária local. Território primeiro, comunidade primeiro.";

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: pageTitle,
  description: pageDescription,
  openGraph: {
    type: "website",
    url: siteUrl,
    title: pageTitle,
    description: pageDescription,
    images: [
      {
        url: "/og.png",
        width: 1200,
        height: 630,
        alt: brand.name,
      },
    ],
  },
  twitter: {
    card: "summary_large_image",
    title: pageTitle,
    description: pageDescription,
    images: ["/og.png"],
  },
  icons: {
    icon: "/favicon.png",
    apple: "/icon.png"
  }
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="pt-BR">
      <body>
        <SiteHeader />
        {children}
      </body>
    </html>
  );
}
