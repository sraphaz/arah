"use client";

import { useMemo } from "react";
import { MermaidDiagram } from "./MermaidDiagram";
import sanitizeHtml from "sanitize-html";

interface MermaidContentProps {
  htmlContent: string;
}

interface ContentPart {
  type: "html" | "mermaid";
  content: string;
  id?: string;
}

export function MermaidContent({ htmlContent }: MermaidContentProps) {
  const parts = useMemo(() => {
    const result: ContentPart[] = [];
    let remaining = htmlContent;
    let mermaidIndex = 0;

    // Processa o HTML procurando por divs com data-mermaid-code
    // Suporta tanto com data-mermaid-block quanto sem (formato atual)
    const mermaidRegex = /<div\s+(?:data-mermaid-block="[^"]*"\s+)?data-mermaid-code="([^"]*)"[^>]*><\/div>/gi;
    let match;
    let lastIndex = 0;

    while ((match = mermaidRegex.exec(htmlContent)) !== null) {
      // Adiciona HTML antes do bloco Mermaid
      if (match.index > lastIndex) {
        const htmlPart = htmlContent.substring(lastIndex, match.index);
        if (htmlPart.trim()) {
          result.push({ type: "html", content: htmlPart });
        }
      }

      // Adiciona bloco Mermaid
      const encodedCode = match[1];
      const code = decodeURIComponent(encodedCode);
      const id = `mermaid-${mermaidIndex++}`;
      result.push({ type: "mermaid", content: code, id });

      lastIndex = match.index + match[0].length;
    }

    // Adiciona HTML restante
    if (lastIndex < htmlContent.length) {
      const htmlPart = htmlContent.substring(lastIndex);
      if (htmlPart.trim()) {
        result.push({ type: "html", content: htmlPart });
      }
    }

    // Se não encontrou nenhum bloco Mermaid, retorna todo o conteúdo como HTML
    if (result.length === 0) {
      result.push({ type: "html", content: htmlContent });
    }

    return result;
  }, [htmlContent]);

  return (
    <div className="markdown-content">
      {parts.map((part, index) => {
        if (part.type === "mermaid") {
          return <MermaidDiagram key={part.id || `mermaid-${index}`} code={part.content} id={part.id} />;
        }
        
        // Renderiza HTML de forma segura
        const sanitized = sanitizeHtml(part.content, {
          allowedTags: sanitizeHtml.defaults.allowedTags.concat(['div', 'span']),
          allowedAttributes: {
            ...sanitizeHtml.defaults.allowedAttributes,
            div: ['class', 'id', 'data-*'],
            span: ['class', 'id'],
          },
        });

        return (
          <div
            key={`html-${index}`}
            dangerouslySetInnerHTML={{ __html: sanitized }}
          />
        );
      })}
    </div>
  );
}
