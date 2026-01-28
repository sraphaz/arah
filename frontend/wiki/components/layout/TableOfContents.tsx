"use client";

import { useEffect, useState, useRef } from "react";

interface TOCItem {
  id: string;
  text: string;
  level: number;
}

export function TableOfContents() {
  const [toc, setToc] = useState<TOCItem[]>([]);
  const [activeId, setActiveId] = useState<string>("");
  const scrollTimeoutRef = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    // Extract headings from markdown content - busca em todo o documento
    const extractHeadings = () => {
      // Tenta múltiplos seletores para encontrar o conteúdo
      const content = document.querySelector(".markdown-content") ||
                      document.querySelector("main") ||
                      document.body;

      if (!content) return [];

      // Busca headings em todo o documento, não apenas dentro de .markdown-content
      const headings = content.querySelectorAll("h2[id], h3[id], h4[id]");
      const items: TOCItem[] = [];

      headings.forEach((heading) => {
        const id = heading.id || heading.textContent?.toLowerCase().replace(/\s+/g, "-") || "";
        const level = parseInt(heading.tagName.charAt(1));
        const text = heading.textContent?.trim() || "";

        // Only include h2, h3, h4 (skip h1 which is usually the title)
        if (level >= 2 && level <= 4 && id && text) {
          items.push({ id, text, level });
        }
      });

      return items;
    };

    // Aguarda o conteúdo carregar - tenta múltiplas vezes
    let attempts = 0;
    const maxAttempts = 10;

    const tryExtract = () => {
      const headings = extractHeadings();
      if (headings.length > 0 || attempts >= maxAttempts) {
        setToc(headings);
      } else {
        attempts++;
        setTimeout(tryExtract, 200);
      }
    };

    // Inicia imediatamente e também após um delay
    tryExtract();
    const timer = setTimeout(tryExtract, 500);

    return () => clearTimeout(timer);
  }, []);

  useEffect(() => {
    // Track scroll position to highlight active heading using IntersectionObserver
    if (toc.length === 0) return;

    // Função para fazer scroll do item ativo no TOC para dentro da área visível
    // Usa debounce para evitar scrolls muito frequentes
    const scrollActiveItemIntoView = (id: string) => {
      if (scrollTimeoutRef.current) {
        clearTimeout(scrollTimeoutRef.current);
      }
      
      scrollTimeoutRef.current = setTimeout(() => {
        const tocContainer = document.querySelector('.toc-container');
        if (!tocContainer) return;

        const activeItem = tocContainer.querySelector(`[data-toc-id="${id}"]`) as HTMLElement;
        if (!activeItem) return;

        const containerRect = tocContainer.getBoundingClientRect();
        const itemRect = activeItem.getBoundingClientRect();

        // Verifica se o item está fora da área visível do container (com margem de segurança)
        const margin = 20; // Margem de segurança em pixels
        const isAboveView = itemRect.top < containerRect.top + margin;
        const isBelowView = itemRect.bottom > containerRect.bottom - margin;

        if (isAboveView || isBelowView) {
          // Scroll suave para trazer o item para o centro visível
          const scrollOffset = itemRect.top - containerRect.top + tocContainer.scrollTop - (containerRect.height / 2) + (itemRect.height / 2);
          tocContainer.scrollTo({
            top: Math.max(0, scrollOffset), // Garante que não seja negativo
            behavior: 'smooth',
          });
        }
      }, 150); // Debounce de 150ms
    };

    const observerOptions = {
      root: null,
      rootMargin: "-140px 0px -60% 0px", // Considera heading ativo quando está próximo do topo (ajustado para sticky header)
      threshold: [0, 0.1, 0.25, 0.5, 0.75, 1],
    };

    const observer = new IntersectionObserver((entries) => {
      // Encontra o heading mais próximo do topo que está visível ou que passou do topo
      const headingsWithPosition = entries
        .map(entry => ({
          id: entry.target.id,
          top: entry.boundingClientRect.top,
          isIntersecting: entry.isIntersecting,
          intersectionRatio: entry.intersectionRatio,
        }))
        .sort((a, b) => a.top - b.top);

      // Primeiro, tenta encontrar um heading visível
      const visibleHeading = headingsWithPosition.find(h => h.isIntersecting && h.top >= 140);

      if (visibleHeading) {
        setActiveId(visibleHeading.id);
        // Scroll automático do item ativo no TOC se estiver fora da área visível
        scrollActiveItemIntoView(visibleHeading.id);
        return;
      }

      // Se não há heading visível, pega o último que passou do topo (mais próximo do topo)
      const pastTop = headingsWithPosition.filter(h => h.top < 140);
      if (pastTop.length > 0) {
        const activeId = pastTop[pastTop.length - 1].id;
        setActiveId(activeId);
        // Scroll automático do item ativo no TOC se estiver fora da área visível
        scrollActiveItemIntoView(activeId);
      }
    }, observerOptions);

    // Observa todos os headings
    toc.forEach((item) => {
      const element = document.getElementById(item.id);
      if (element) {
        observer.observe(element);
      }
    });

    // Fallback: também usa scroll listener para garantir detecção mais precisa
    const handleScroll = () => {
      const headings = toc.map((item) => document.getElementById(item.id)).filter(Boolean) as HTMLElement[];

      if (headings.length === 0) return;

      const offset = 140; // Offset para considerar sticky header
      let current = "";

      // Procura o heading que está mais próximo do topo mas ainda não passou muito
      for (let i = headings.length - 1; i >= 0; i--) {
        const heading = headings[i];
        const rect = heading.getBoundingClientRect();

        // Se o heading está acima ou próximo do offset, é o ativo
        if (rect.top <= offset + 50) {
          current = heading.id;
          break;
        }
      }

      if (current && current !== activeId) {
        setActiveId(current);
        // Scroll automático do item ativo no TOC se estiver fora da área visível
        scrollActiveItemIntoView(current);
      }
    };

    window.addEventListener("scroll", handleScroll, { passive: true });
    handleScroll(); // Initial check

    return () => {
      observer.disconnect();
      window.removeEventListener("scroll", handleScroll);
      if (scrollTimeoutRef.current) {
        clearTimeout(scrollTimeoutRef.current);
        scrollTimeoutRef.current = null;
      }
    };
  }, [toc]);

  if (toc.length === 0) {
    return null;
  }

  const scrollToHeading = (id: string) => {
    const element = document.getElementById(id);
    if (element) {
      const headerOffset = 100;
      const elementPosition = element.getBoundingClientRect().top;
      const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

      window.scrollTo({
        top: offsetPosition,
        behavior: "smooth",
      });
    }
  };

  return (
    <nav className="toc-container" aria-label="Índice do documento">
      <div className="toc-header">
        <h3 className="toc-title">Neste Documento</h3>
      </div>
      <ul className="toc-list">
        {toc.map((item, index) => {
          // Determina se é o início de um novo grupo (nível 2 ou quando muda de nível 2 para 3)
          const prevItem = index > 0 ? toc[index - 1] : null;
          const isGroupStart = item.level === 2 || (prevItem && prevItem.level === 2 && item.level > 2);
          const isGroupEnd = index === toc.length - 1 || (toc[index + 1] && toc[index + 1].level <= item.level);

          return (
            <li
              key={item.id}
              data-toc-id={item.id}
              className={`toc-item toc-item-level-${item.level} ${
                activeId === item.id ? "toc-item-active" : ""
              } ${isGroupStart ? "toc-group-start" : ""} ${isGroupEnd ? "toc-group-end" : ""}`}
            >
              <button
                onClick={() => scrollToHeading(item.id)}
                className="toc-link"
                aria-current={activeId === item.id ? "location" : undefined}
              >
                {item.text}
              </button>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
