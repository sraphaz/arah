"use client";

import { useMemo } from "react";
import sanitizeHtml from "sanitize-html";

interface Section {
  title: string;
  content: string;
  isLong: boolean;
  level: number;
  id?: string;
}

// Helper function para extrair texto de HTML de forma segura
function getTextContent(html: string): string {
  return sanitizeHtml(html, {
    allowedTags: [],
    allowedAttributes: {},
  });
}

function processContentIntoSections(html: string): Section[] {
  const sections: Section[] = [];

  // Regex para encontrar headings - procura por qualquer h2, h3, h4 (com ou sem ID)
  const headingRegex = /<h([2-4])([^>]*)>(.*?)<\/h\1>/gi;

  const matches: Array<{ index: number; level: number; id: string; title: string; fullMatch: string }> = [];
  let match;

  // Coleta todos os matches primeiro
  while ((match = headingRegex.exec(html)) !== null) {
    const attrs = match[2] || '';
    const idMatch = attrs.match(/id=["']([^"']+)["']/i);
    const id = idMatch ? idMatch[1] : '';
    // Usa sanitize-html para extrair texto do título de forma segura
    const title = getTextContent(match[3] || '').trim();

    // Gera ID se não existir
    const generatedId = id || title
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '') // Remove acentos
      .replace(/[^a-z0-9]+/g, '-') // Replace non-alphanumeric with dash
      .replace(/^-+|-+$/g, ''); // Remove leading/trailing dashes

    matches.push({
      index: match.index,
      level: parseInt(match[1]),
      id: generatedId,
      title: title,
      fullMatch: match[0],
    });
  }

  // Se não há headings, retorna todo o conteúdo como uma seção
  if (matches.length === 0) {
    const textLength = getTextContent(html).trim().length;
    return [{
      title: '',
      content: html,
      isLong: textLength > 300, // Reduzido de 500 para 300 para ativar progressive disclosure mais cedo
      level: 1,
    }];
  }

  // Processa conteúdo antes do primeiro heading
  if (matches[0].index > 0) {
    const beforeContent = html.substring(0, matches[0].index);
    if (beforeContent.trim()) {
      const textLength = getTextContent(beforeContent).trim().length;
      sections.push({
        title: '',
        content: beforeContent,
        isLong: textLength > 300, // Reduzido de 500 para 300
        level: 1,
      });
    }
  }

  // Processa cada seção de heading
  for (let i = 0; i < matches.length; i++) {
    const currentMatch = matches[i];
    const nextMatch = matches[i + 1];

    // Conteúdo desta seção: do heading atual até o próximo (ou fim)
    const contentStart = currentMatch.index;
    const contentEnd = nextMatch ? nextMatch.index : html.length;
    const sectionContent = html.substring(contentStart, contentEnd);

    const textLength = getTextContent(sectionContent).trim().length;

    sections.push({
      title: currentMatch.title,
      content: sectionContent,
      isLong: textLength > 300, // Reduzido de 500 para 300 para ativar progressive disclosure mais cedo
      level: currentMatch.level,
      id: currentMatch.id,
    });
  }

  return sections;
}

interface ContentSectionsProps {
  htmlContent: string;
}

export function ContentSections({ htmlContent }: ContentSectionsProps) {
  const sections = useMemo(() => processContentIntoSections(htmlContent), [htmlContent]);

  return (
    <div className="markdown-content prose-headings:first:mt-0">
      {sections.map((section, index) => {
        if (!section.title && !section.isLong) {
          // Conteúdo sem título e curto - renderiza diretamente
          return (
            <div
              key={index}
              className="mb-6"
              dangerouslySetInnerHTML={{ __html: section.content }}
            />
          );
        }

        if (!section.isLong) {
          // Seção com título mas curta - renderiza normalmente (heading já está no content)
          return (
            <section key={index} id={section.id} className="mb-8">
              <div dangerouslySetInnerHTML={{ __html: section.content }} />
            </section>
          );
        }

        // Seção longa - renderiza como texto simples (sem accordion)
        // Extrai título se existir, senão renderiza conteúdo diretamente
        if (section.title) {
          // Seção com título - heading já está no content
          return (
            <section key={index} id={section.id} className="mb-8">
              <div dangerouslySetInnerHTML={{ __html: section.content }} />
            </section>
          );
        }

        // Seção sem título - renderiza diretamente como texto
        return (
          <div
            key={index}
            className="mb-8"
            dangerouslySetInnerHTML={{ __html: section.content }}
          />
        );
      })}
    </div>
  );
}
