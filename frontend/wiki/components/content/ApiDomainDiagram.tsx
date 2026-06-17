"use client";

import Image from "next/image";
import { useState } from "react";
import { brand } from "../../../shared/config/brand";

export function ApiDomainDiagram() {
  const [isLightboxOpen, setIsLightboxOpen] = useState(false);

  return (
    <div className="api-domain-diagram-container my-16">
      <div className="glass-card animation-fade-in">
        <div className="glass-card__content">
          <div className="text-center mb-8">
            <h2 className="text-2xl md:text-3xl font-bold text-forest-900 dark:text-forest-50 mb-4">
              Visão Geral do Sistema {brand.name}
            </h2>
            <p className="text-lg text-forest-700 dark:text-forest-300 max-w-2xl mx-auto">
              Diagrama isométrico mostrando como os diferentes módulos se conectam ao território como referência central
            </p>
          </div>

          <div
            className="relative w-full max-w-5xl mx-auto rounded-2xl overflow-hidden shadow-2xl cursor-pointer"
            onClick={() => setIsLightboxOpen(true)}
            role="button"
            tabIndex={0}
            onKeyDown={(e) => {
              if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                setIsLightboxOpen(true);
              }
            }}
            aria-label="Clique para ampliar o diagrama"
          >
            <Image
              src="/wiki/araponga-api-domain-diagram.png"
              alt={`Diagrama isométrico do Domínio API ${brand.name} - mostrando TERRITÓRIO no centro com conexões para FEED, MAP, HEALTH, FEATURES, MEMBERSHIP & GOVERNANCE, e AUTENTICAÇÃO`}
              width={1200}
              height={800}
              className="w-full h-auto object-contain transition-transform duration-300 hover:scale-[1.02]"
              priority
              unoptimized={true}
              onError={(e) => {
                // Fallback se imagem não existir
                const target = e.target as HTMLImageElement;
                target.style.display = 'none';
                const fallback = target.nextElementSibling as HTMLElement;
                if (fallback) fallback.style.display = 'block';
              }}
            />
            <div
              className="hidden p-8 bg-forest-100 dark:bg-forest-900/50 rounded-xl text-center"
              style={{ display: 'none' }}
            >
              <p className="text-forest-600 dark:text-forest-400 mb-4">
                Diagrama do Domínio API {brand.name}
              </p>
              <p className="text-sm text-forest-500 dark:text-forest-500">
                A imagem será carregada quando disponível em <code>/public/araponga-api-domain-diagram.png</code>
              </p>
            </div>
          </div>

          <div className="mt-8 grid md:grid-cols-3 gap-4 text-sm text-forest-600 dark:text-forest-400">
            <div className="text-center">
              <span className="block text-2xl mb-2">🗺️</span>
              <p className="font-medium">Território Central</p>
              <p className="text-xs mt-1">Referência geográfica para todos os módulos</p>
            </div>
            <div className="text-center">
              <span className="block text-2xl mb-2">🔗</span>
              <p className="font-medium">Conexões Contextuais</p>
              <p className="text-xs mt-1">Cada módulo se conecta ao território ativo</p>
            </div>
            <div className="text-center">
              <span className="block text-2xl mb-2">👥</span>
              <p className="font-medium">Pessoas no Centro</p>
              <p className="text-xs mt-1">Governança e autonomia comunitária</p>
            </div>
          </div>
        </div>
      </div>

      {/* Lightbox Modal - Imagem em resolução original */}
      {isLightboxOpen && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center p-4"
          onClick={() => setIsLightboxOpen(false)}
          role="dialog"
          aria-modal="true"
          aria-label="Diagrama ampliado"
        >
          {/* Overlay - Ofusca levemente o fundo */}
          <div className="absolute inset-0 bg-black/60 backdrop-blur-sm transition-opacity" />

          {/* Imagem em tamanho original */}
          <div className="relative z-10 max-w-full max-h-full" onClick={(e) => e.stopPropagation()}>
            <Image
              src="/wiki/araponga-api-domain-diagram.png"
              alt={`Diagrama isométrico do Domínio API ${brand.name} - versão ampliada`}
              width={2400}
              height={1600}
              className="max-w-full max-h-[90vh] w-auto h-auto object-contain rounded-lg shadow-2xl"
              unoptimized={true}
            />
            {/* Botão de fechar */}
            <button
              onClick={(e) => {
                e.stopPropagation();
                setIsLightboxOpen(false);
              }}
              className="absolute top-4 right-4 p-2 bg-white/90 dark:bg-forest-900/90 rounded-full shadow-lg hover:bg-white dark:hover:bg-forest-900 transition-colors"
              aria-label="Fechar diagrama ampliado"
            >
              <svg className="w-6 h-6 text-forest-900 dark:text-forest-50" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
