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
    // Extract headings from markdown content
    const extractHeadings = () => {
      const content = document.querySelector(".markdown-content");
      if (!content) return [];

      const headings = content.querySelectorAll("h1, h2, h3, h4");
      const items: TOCItem[] = [];

      headings.forEach((heading) => {
        const id = heading.id || heading.textContent?.toLowerCase().replace(/\s+/g, "-") || "";
        const level = parseInt(heading.tagName.charAt(1));
        const text = heading.textContent || "";

        // Only include h2, h3, h4 (skip h1 which is usually the title)
        if (level >= 2 && level <= 4 && id && text) {
          items.push({ id, text, level });
        }
      });

      return items;
    };

    // Wait for content to load
    const timer = setTimeout(() => {
      const headings = extractHeadings();
      setToc(headings);
    }, 100);

    return () => clearTimeout(timer);
  }, []);

  useEffect(() => {
    // Track scroll position to highlight active heading
    const handleScroll = () => {
      const headings = toc.map((item) => document.getElementById(item.id)).filter(Boolean) as HTMLElement[];
      
      if (headings.length === 0) return;

      let current = "";
      for (const heading of headings) {
        const rect = heading.getBoundingClientRect();
        if (rect.top <= 100) {
          current = heading.id;
        } else {
          break;
        }
      }

      if (current) {
        setActiveId(current);
      }
    };

    if (toc.length > 0) {
      window.addEventListener("scroll", handleScroll, { passive: true });
      handleScroll(); // Initial check
      return () => window.removeEventListener("scroll", handleScroll);
    }
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
