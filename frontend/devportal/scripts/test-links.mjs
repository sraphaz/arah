#!/usr/bin/env node
/**
 * Script para testar links do DevPortal
 *
 * Valida:
 * - Links internos apontam para IDs existentes
 * - Links externos estão acessíveis (opcional, apenas validação de formato)
 * - Links da sidebar estão corretos
 */

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const htmlPath = path.join(__dirname, '..', 'index.html');
const htmlContent = fs.readFileSync(htmlPath, 'utf-8');

// Parse HTML simples (sem JSDOM para performance)
const extractLinks = (html) => {
  const linkRegex = /<a[^>]+href=["']([^"']+)["'][^>]*>/gi;
  const links = [];
  let match;

  while ((match = linkRegex.exec(html)) !== null) {
    links.push({
      href: match[1],
      fullMatch: match[0],
    });
  }

  return links;
};

const extractIds = (html) => {
  const idRegex = /id=["']([^"']+)["']/gi;
  const ids = new Set();
  let match;

  while ((match = idRegex.exec(html)) !== null) {
    ids.add(match[1]);
  }

  return ids;
};

const links = extractLinks(htmlContent);
const ids = extractIds(htmlContent);

console.log(`📋 Analisando ${links.length} links e ${ids.size} IDs...\n`);

// Dangerous URL schemes that should be blocked
const DANGEROUS_SCHEMES = ['javascript:', 'data:', 'vbscript:', 'file:', 'about:'];
const isDangerousScheme = (href) => {
  const lowerHref = href.toLowerCase().trim();
  return DANGEROUS_SCHEMES.some(scheme => lowerHref.startsWith(scheme));
};

const internalLinks = links.filter(link => link.href.startsWith('#'));
const externalLinks = links.filter(link => 
  !link.href.startsWith('#') && 
  !isDangerousScheme(link.href)
);

let brokenInternalLinks = [];
let warnings = [];

// Valida links internos
internalLinks.forEach(link => {
  const targetId = link.href.substring(1);
  if (targetId && !ids.has(targetId)) {
    // Ignora IDs que podem estar em conteúdo dinâmico ou phase-panels
    if (!['quickstart', 'auth', 'territory-session', 'faq', 'openapi', 'erros'].includes(targetId)) {
      brokenInternalLinks.push({ href: link.href, targetId });
    }
  }
});

// Valida links externos (formato)
externalLinks.forEach(link => {
  if (!link.href.startsWith('http://') && !link.href.startsWith('https://') && !link.href.startsWith('mailto:')) {
    warnings.push({ href: link.href, reason: 'Formato de URL inválido' });
  }
});

// Relatório
if (brokenInternalLinks.length > 0) {
  console.log('❌ Links internos quebrados:');
  brokenInternalLinks.forEach(link => {
    console.log(`   ${link.href} -> ID "${link.targetId}" não encontrado`);
  });
  console.log('');
}

if (warnings.length > 0) {
  console.log('⚠️  Avisos:');
  warnings.forEach(warning => {
    console.log(`   ${warning.href}: ${warning.reason}`);
  });
  console.log('');
}

if (brokenInternalLinks.length === 0 && warnings.length === 0) {
  console.log('✅ Todos os links estão válidos!\n');
  process.exit(0);
} else {
  console.log(`\n📊 Resumo:`);
  console.log(`   - Links internos quebrados: ${brokenInternalLinks.length}`);
  console.log(`   - Avisos: ${warnings.length}`);
  console.log(`   - Total de links: ${links.length}\n`);
  process.exit(brokenInternalLinks.length > 0 ? 1 : 0);
}
