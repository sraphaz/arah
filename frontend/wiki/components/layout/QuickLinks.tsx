"use client";

import Link from "next/link";

interface QuickLink {
  label: string;
  href: string;
  icon: string;
  external?: boolean;
  description?: string;
}

const quickLinks: QuickLink[] = [
  {
    label: "Discord",
    href: "https://discord.gg/auwqN8Yjgw",
    icon: "💬",
    external: true,
    description: "Conecte-se com o time",
  },
  {
    label: "GitHub",
    href: "https://github.com/sraphaz/araponga",
    icon: "🔗",
    external: true,
    description: "Repositório do projeto",
  },
  {
    label: "Site Principal",
    href: "https://araponga.app",
    icon: "🌐",
    external: true,
    description: "Visite o site oficial",
  },
  {
    label: "Dev Portal",
    href: "https://devportal.araponga.app",
    icon: "⚡",
    external: true,
    description: "Documentação técnica da API",
  },
];

export function QuickLinks() {
  return (
    <nav className="quick-links-container" aria-label="Links rápidos">
      <div className="quick-links-header">
        <span className="quick-links-icon">⚡</span>
        <h3 className="quick-links-title">Links Úteis</h3>
      </div>
      <ul className="quick-links-list">
        {quickLinks.map((link) => (
          <li key={link.href} className="quick-links-item">
            {link.external ? (
              <a
                href={link.href}
                target="_blank"
                rel="noopener noreferrer"
                className="quick-link group"
              >
                <span className="quick-link-icon">{link.icon}</span>
                <div className="quick-link-content">
                  <span className="quick-link-label">{link.label}</span>
                  {link.description && (
                    <span className="quick-link-description">{link.description}</span>
                  )}
                </div>
                <svg
                  className="quick-link-external-icon"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                  strokeWidth={2}
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"
                  />
                </svg>
              </a>
            ) : (
              <Link href={link.href} className="quick-link">
                <span className="quick-link-icon">{link.icon}</span>
                <div className="quick-link-content">
                  <span className="quick-link-label">{link.label}</span>
                  {link.description && (
                    <span className="quick-link-description">{link.description}</span>
                  )}
                </div>
              </Link>
            )}
          </li>
        ))}
      </ul>
    </nav>
  );
}
