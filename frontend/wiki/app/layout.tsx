import "./globals.css";
import type { Metadata } from "next";

const siteUrl = "https://wiki.araponga.app";

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
    <html lang="pt-BR">
      <body>{children}</body>
    </html>
  );
}
