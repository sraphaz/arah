"use client";

import Image from "next/image";
import Link from "next/link";

export function AppBanner() {
  return (
    <section className="app-banner-container my-20">
      <div className="glass-card animation-fade-in app-banner-card">
        <div className="glass-card__content">
          <div className="app-banner-grid">
            {/* Conteúdo Textual */}
            <div className="app-banner-content">
              <div className="app-banner-badge">
                <span className="app-banner-badge-icon">🚀</span>
                <span className="app-banner-badge-text">Em Breve</span>
              </div>

              <p className="app-banner-description">
                Uma plataforma digital comunitária que valoriza o território, fortalece vínculos locais e respeita a autonomia das comunidades.
              </p>

              <div className="app-banner-features">
                <div className="app-banner-feature">
                  <span className="app-banner-feature-icon">📍</span>
                  <span>Organizado por território</span>
                </div>
                <div className="app-banner-feature">
                  <span className="app-banner-feature-icon">👥</span>
                  <span>Governança comunitária</span>
                </div>
                <div className="app-banner-feature">
                  <span className="app-banner-feature-icon">🌱</span>
                  <span>Autonomia local</span>
                </div>
              </div>

              <div className="app-banner-cta">
                <a
                  href="https://araponga.app"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="app-banner-button-primary"
                >
                  <span>Conheça mais</span>
                  <svg
                    className="app-banner-button-icon"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M13 7l5 5m0 0l-5 5m5-5H6"
                    />
                  </svg>
                </a>
                <a
                  href="https://discord.gg/auwqN8Yjgw"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="app-banner-button-secondary"
                >
                  <span>Junte-se à comunidade</span>
                </a>
              </div>
            </div>

            {/* Banner Visual */}
            <div className="app-banner-visual">
              <div className="app-banner-image-wrapper">
                <Image
                  src="/wiki/app_banner.png"
                  alt="Araponga App - Plataforma Digital Comunitária"
                  width={800}
                  height={600}
                  className="app-banner-image"
                  priority
                  unoptimized={true}
                />
                {/* Overlay sutil para destaque */}
                <div className="app-banner-overlay"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
