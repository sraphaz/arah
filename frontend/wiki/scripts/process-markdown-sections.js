/**
 * Processador de Markdown para adicionar progressive disclosure
 * Converte seções longas em componentes colapsáveis
 */

const sanitizeHtml = require('sanitize-html');

function getTextContent(html) {
  return sanitizeHtml(html, {
    allowedTags: [],
    allowedAttributes: {},
  });
}

function processMarkdownForProgressiveDisclosure(html, options = {}) {
  const {
    minLength = 500, // Mínimo de caracteres para colapsar
    headingsToCollapse = ['h2', 'h3'], // Headings para considerar colapsáveis
    maxSections = 10, // Máximo de seções a processar por vez
  } = options;

  // Encontra seções baseadas em headings
  const headingRegex = new RegExp(`<(${headingsToCollapse.join('|')})([^>]*)>([^<]+)</\\1>`, 'gi');
  
  let processedHtml = html;
  let sectionCount = 0;

  // Processa cada heading
  processedHtml = processedHtml.replace(headingRegex, (match, tag, attrs, text) => {
    if (sectionCount >= maxSections) return match;

    // Encontra o próximo heading ou fim do documento
    const afterMatch = processedHtml.indexOf(match) + match.length;
    const remainingHtml = processedHtml.substring(afterMatch);
    
    // Encontra a próxima seção ou parágrafo final
    const nextHeadingMatch = remainingHtml.match(new RegExp(`<(${headingsToCollapse.join('|')})`, 'i'));
    const nextHeadingIndex = nextHeadingMatch ? nextHeadingMatch.index : remainingHtml.length;
    
    const sectionContent = remainingHtml.substring(0, nextHeadingIndex);
    const sectionLength = getTextContent(sectionContent).trim().length;

    // Se a seção é longa o suficiente, adiciona atributos para colapsar
    if (sectionLength > minLength) {
      sectionCount++;
      const id = attrs.match(/id="([^"]+)"/)?.[1] || text.toLowerCase().replace(/\s+/g, '-');
      return `<${tag}${attrs} data-collapsible="true" data-section-id="${id}">${text}</${tag}>`;
    }

    return match;
  });

  return processedHtml;
}

module.exports = { processMarkdownForProgressiveDisclosure };
