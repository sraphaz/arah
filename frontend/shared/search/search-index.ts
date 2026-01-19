/**
 * Índice de busca compartilhado para Wiki e DevPortal
 * Centraliza lógica de indexação de documentos
 */

export interface SearchItem {
  id: string;
  title: string;
  content: string;
  url: string;
  category: 'wiki' | 'devportal';
  type: 'doc' | 'section' | 'endpoint';
}

/**
 * Índice de documentos da Wiki
 */
export async function getWikiIndex(): Promise<SearchItem[]> {
  // Em runtime (client-side), os dados serão carregados via API route
  // Este arquivo serve como interface para o índice
  return [];
}

/**
 * Índice de seções do DevPortal
 */
export async function getDevPortalIndex(): Promise<SearchItem[]> {
  // Em runtime (client-side), os dados serão carregados via JavaScript
  // Este arquivo serve como interface para o índice
  return [];
}

/**
 * Configuração Fuse.js para busca
 */
export const fuseConfig = {
  keys: [
    { name: 'title', weight: 0.7 },
    { name: 'content', weight: 0.3 },
  ],
  threshold: 0.4,
  distance: 100,
  minMatchCharLength: 2,
  includeScore: true,
  includeMatches: true,
};
