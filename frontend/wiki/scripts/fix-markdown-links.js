/**
 * Script para processar links no markdown renderizado
 * Garante que links relativos no markdown usem o basePath correto
 */

function processMarkdownLinks(html, basePath = '/wiki') {
  // Processa links <a href="/docs/..."> para <a href="/wiki/docs/...">
  // Mas NÃO processa links externos ou absolutos
  const processedHTML = html.replace(
    /<a\s+([^>]*\s+)?href=["'](\/[^"']+)["']([^>]*)>/gi,
    (match, before, href, after) => {
      // Ignora se já começa com basePath ou é link externo
      if (href.startsWith(basePath) || href.startsWith('http')) {
        return match;
      }
      
      // Adiciona basePath a links relativos que começam com /
      const newHref = `${basePath}${href}`;
      return `<a ${before || ''}href="${newHref}"${after || ''}>`;
    }
  );

  return processedHTML;
}

module.exports = { processMarkdownLinks };
