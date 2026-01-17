"use client";

import { useMemo } from "react";
import { Accordion } from "../../../components/ui/Accordion";

interface Section {
  title: string;
  content: string;
  isLong: boolean;
  level: number;
  id?: string;
}

function processContentIntoSections(html: string): Section[] {
  const sections: Section[] = [];
  
  // Regex para encontrar headings com IDs
  const headingRegex = /<h([2-4])([^>]*?)id="([^"]*)"([^>]*?)>(.*?)<\/h\1>/gi;
  
  const matches: Array<{ index: number; level: number; id: string; title: string; fullMatch: string }> = [];
  let match;
  
  // Coleta todos os matches primeiro
  while ((match = headingRegex.exec(html)) !== null) {
    matches.push({
      index: match.index,
      level: parseInt(match[1]),
      id: match[3],
      title: match[5].replace(/<[^>]*>/g, '').trim(),
      fullMatch: match[0],
    });
  }
  
  // Se não há headings, retorna todo o conteúdo como uma seção
  if (matches.length === 0) {
    const textLength = html.replace(/<[^>]*>/g, '').trim().length;
    return [{
      title: '',
      content: html,
      isLong: textLength > 500,
      level: 1,
    }];
  }
  
  // Processa conteúdo antes do primeiro heading
  if (matches[0].index > 0) {
    const beforeContent = html.substring(0, matches[0].index);
    if (beforeContent.trim()) {
      const textLength = beforeContent.replace(/<[^>]*>/g, '').trim().length;
      sections.push({
        title: '',
        content: beforeContent,
        isLong: textLength > 500,
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
    
    const textLength = sectionContent.replace(/<[^>]*>/g, '').trim().length;
    
    sections.push({
      title: currentMatch.title,
      content: sectionContent,
      isLong: textLength > 500,
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
        
        // Seção longa - extrai título e usa Accordion
        return (
          <section key={index} id={section.id} className="mb-6">
            <Accordion title={section.title || 'Conteúdo'} defaultOpen={index <= 1}>
              <div 
                className="markdown-content"
                dangerouslySetInnerHTML={{ __html: section.content }} 
              />
            </Accordion>
          </section>
        );
      })}
    </div>
  );
}
