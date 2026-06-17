"use client";

import Link from "next/link";
import { SocialIcon } from "../ui/SocialIcon";
import { brand } from "../../../shared/config/brand";

interface QuickLink {
  label: string;
  href: string;
  icon?: string;
  socialPlatform?: "github" | "discord";
  external?: boolean;
  description?: string;
}

const quickLinks: QuickLink[] = [
  {
    label: "Discord",
    href: brand.urls.discord,
    socialPlatform: "discord",
    external: true,
    description: "Conecte-se com o time",
  },
  {
    label: "GitHub",
    href: brand.urls.github,
    socialPlatform: "github",
    external: true,
    description: "Repositório do projeto",
  },
  {
    label: "Site Institucional",
    href: brand.urls.site,
    icon: "🌐",
    external: true,
    description: "Visite o site oficial",
  },
  {
    label: "Dev Portal",
    href: brand.urls.devportal,
    external: true,
    description: "Documentação técnica da API",
  },
];

export function QuickLinks() {
  return (
    <nav className="quick-links-container" aria-label="Links rápidos">
      <div className="quick-links-header">
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
                <span className="quick-link-icon">
                  {link.socialPlatform ? (
                    <SocialIcon platform={link.socialPlatform} size={24} className="opacity-90 group-hover:opacity-100 transition-opacity" />
                  ) : (
                    link.icon
                  )}
                </span>
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
              <Link href={link.href} prefetch={false} className="quick-link group">
                <span className="quick-link-icon">
                  {link.socialPlatform ? (
                    <SocialIcon platform={link.socialPlatform} size={24} className="opacity-90 group-hover:opacity-100 transition-opacity" />
                  ) : (
                    link.icon
                  )}
                </span>
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
