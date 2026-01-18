"use client";

import { useEffect, useState } from "react";

interface TOCItem {
  id: string;
  text: string;
  level: number;
}

export function TableOfContents() {
  const [toc, setToc] = useState<TOCItem[]>([]);
  const [activeId, setActiveId] = useState<string>("");

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
        return;
      }

      // Se não há heading visível, pega o último que passou do topo (mais próximo do topo)
      const pastTop = headingsWithPosition.filter(h => h.top < 140);
      if (pastTop.length > 0) {
        setActiveId(pastTop[pastTop.length - 1].id);
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
      }
    };

    window.addEventListener("scroll", handleScroll, { passive: true });
    handleScroll(); // Initial check

    return () => {
      observer.disconnect();
      window.removeEventListener("scroll", handleScroll);
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
        <span className="toc-icon">📑</span>
        <h3 className="toc-title">Neste Documento</h3>
      </div>
      <ul className="toc-list">
        {toc.map((item) => (
          <li
            key={item.id}
            className={`toc-item toc-item-level-${item.level} ${
              activeId === item.id ? "toc-item-active" : ""
            }`}
          >
            <button
              onClick={() => scrollToHeading(item.id)}
              className="toc-link"
              aria-current={activeId === item.id ? "location" : undefined}
            >
              {item.text}
            </button>
          </li>
        ))}
      </ul>
    </nav>
  );
}
