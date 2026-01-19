/**
 * Sistema de busca para DevPortal
 * Busca instantânea em seções do DevPortal usando Fuse.js
 */

// Índice de seções do DevPortal
function generateDevPortalIndex() {
  const sections = Array.from(document.querySelectorAll('.section'));
  const index = [];

  sections.forEach((section) => {
    const id = section.id;
    if (!id) return;

    const h2 = section.querySelector('h2');
    const title = h2 ? h2.textContent.trim() : '';

    // Extrai texto de toda a seção
    const content = section.textContent || '';

    // Determina categoria baseado na estrutura
    const category = getCategoryFromSection(section);

    index.push({
      id,
      title,
      content: content.substring(0, 500), // Primeiros 500 caracteres
      url: `#${id}`,
      category,
    });
  });

  return index;
}

/**
 * Determina categoria da seção baseado na estrutura
 */
function getCategoryFromSection(section) {
  // Verifica se está dentro de uma phase-panel
  const phasePanel = section.closest('.phase-panel');
  if (phasePanel) {
    const phaseTitle = phasePanel.querySelector('.phase-title');
    if (phaseTitle) {
      return phaseTitle.textContent.trim();
    }
  }

  // Verifica seção especial
  if (section.id.startsWith('operacao-')) return 'Operações';
  if (section.id.startsWith('cenario-')) return 'Cenários';
  if (section.id.startsWith('guia-producao-')) return 'Guia de Produção';
  if (section.id === 'modelo-dominio') return 'Fundamentos';
  if (section.id === 'fluxos') return 'Fluxos';
  if (section.id === 'casos-de-uso') return 'Casos de Uso';

  return 'Outros';
}

/**
 * Inicializa busca no DevPortal
 */
function initSearch() {
  // Carrega Fuse.js dinamicamente
  if (typeof Fuse === 'undefined') {
    const script = document.createElement('script');
    script.src = 'https://cdn.jsdelivr.net/npm/fuse.js@7.0.0/dist/fuse.min.js';
    script.onload = () => setupSearch();
    document.head.appendChild(script);
  } else {
    setupSearch();
  }
}

/**
 * Configura busca com Fuse.js
 */
function setupSearch() {
  const index = generateDevPortalIndex();
  let fuse = null;

  // Cria dialog de busca
  const searchDialog = createSearchDialog();

  // Cria trigger de busca
  const searchTrigger = createSearchTrigger();

  // Busca quando query muda
  let searchTimeout;
  const searchInput = searchDialog.querySelector('#search-input');

  searchInput.addEventListener('input', (e) => {
    const query = e.target.value.trim();

    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
      if (!query) {
        showResults([]);
        return;
      }

      if (!fuse) {
        fuse = new Fuse(index, {
          keys: [
            { name: 'title', weight: 0.7 },
            { name: 'content', weight: 0.3 },
          ],
          threshold: 0.4,
          distance: 100,
          minMatchCharLength: 2,
        });
      }

      const results = fuse.search(query).slice(0, 10).map(r => r.item);
      showResults(results);
    }, 150);
  });

  // Atalho Cmd/Ctrl + K
  document.addEventListener('keydown', (e) => {
    if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
      e.preventDefault();
      openSearch();
    }
    if (e.key === 'Escape' && searchDialog.classList.contains('open')) {
      closeSearch();
    }
  });
}

/**
 * Cria dialog de busca (XSS-safe)
 * Usa DOM APIs ao invés de innerHTML para evitar vulnerabilidades XSS
 */
function createSearchDialog() {
  const dialog = document.createElement('div');
  dialog.id = 'search-dialog';
  dialog.className = 'search-dialog';

  // Overlay
  const overlay = document.createElement('div');
  overlay.className = 'search-dialog-overlay';
  overlay.addEventListener('click', closeSearch);
  dialog.appendChild(overlay);

  // Content container
  const content = document.createElement('div');
  content.className = 'search-dialog-content';

  // Header
  const header = document.createElement('div');
  header.className = 'search-dialog-header';

  const input = document.createElement('input');
  input.id = 'search-input';
  input.type = 'text';
  input.placeholder = 'Buscar no DevPortal... (Cmd/Ctrl + K)';
  input.setAttribute('autocomplete', 'off');
  header.appendChild(input);

  const closeBtn = document.createElement('button');
  closeBtn.className = 'search-dialog-close';
  closeBtn.setAttribute('aria-label', 'Fechar busca');
  closeBtn.addEventListener('click', closeSearch);

  // SVG de fechar (criado de forma segura)
  const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
  svg.setAttribute('width', '20');
  svg.setAttribute('height', '20');
  svg.setAttribute('viewBox', '0 0 20 20');
  svg.setAttribute('fill', 'none');

  const path1 = document.createElementNS('http://www.w3.org/2000/svg', 'path');
  path1.setAttribute('d', 'M15 5L5 15');
  path1.setAttribute('stroke', 'currentColor');
  path1.setAttribute('stroke-width', '2');
  path1.setAttribute('stroke-linecap', 'round');
  svg.appendChild(path1);

  const path2 = document.createElementNS('http://www.w3.org/2000/svg', 'path');
  path2.setAttribute('d', 'M5 5l10 10');
  path2.setAttribute('stroke', 'currentColor');
  path2.setAttribute('stroke-width', '2');
  path2.setAttribute('stroke-linecap', 'round');
  svg.appendChild(path2);

  closeBtn.appendChild(svg);
  header.appendChild(closeBtn);

  content.appendChild(header);

  // Results container
  const resultsContainer = document.createElement('div');
  resultsContainer.id = 'search-results';
  resultsContainer.className = 'search-dialog-results';
  content.appendChild(resultsContainer);

  dialog.appendChild(content);
  document.body.appendChild(dialog);

  return dialog;
}

/**
 * Cria trigger de busca (XSS-safe)
 * Usa DOM APIs ao invés de innerHTML para evitar vulnerabilidades XSS
 */
function createSearchTrigger() {
  const header = document.querySelector('header');
  if (!header) return;

  const trigger = document.createElement('button');
  trigger.className = 'search-trigger';
  trigger.addEventListener('click', openSearch);

  // SVG de busca (criado de forma segura)
  const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
  svg.setAttribute('width', '16');
  svg.setAttribute('height', '16');
  svg.setAttribute('viewBox', '0 0 16 16');
  svg.setAttribute('fill', 'none');

  const path1 = document.createElementNS('http://www.w3.org/2000/svg', 'path');
  path1.setAttribute('d', 'M7 12A5 5 0 1 0 7 2a5 5 0 0 0 0 10z');
  path1.setAttribute('stroke', 'currentColor');
  path1.setAttribute('stroke-width', '2');
  path1.setAttribute('stroke-linecap', 'round');
  svg.appendChild(path1);

  const path2 = document.createElementNS('http://www.w3.org/2000/svg', 'path');
  path2.setAttribute('d', 'M13 13l-3-3');
  path2.setAttribute('stroke', 'currentColor');
  path2.setAttribute('stroke-width', '2');
  path2.setAttribute('stroke-linecap', 'round');
  svg.appendChild(path2);

  trigger.appendChild(svg);

  // Span de texto
  const span = document.createElement('span');
  span.textContent = 'Buscar';
  trigger.appendChild(span);

  // KBD
  const kbd = document.createElement('kbd');
  kbd.textContent = '⌘K';
  trigger.appendChild(kbd);

  const nav = header.querySelector('nav');
  if (nav) {
    nav.insertBefore(trigger, nav.firstChild);
  }

  return trigger;
}

/**
 * Abre dialog de busca
 */
function openSearch() {
  const dialog = document.getElementById('search-dialog');
  if (dialog) {
    dialog.classList.add('open');
    const input = dialog.querySelector('#search-input');
    if (input) {
      input.focus();
    }
  }
}

/**
 * Fecha dialog de busca
 */
function closeSearch() {
  const dialog = document.getElementById('search-dialog');
  if (dialog) {
    dialog.classList.remove('open');
    const input = dialog.querySelector('#search-input');
    if (input) {
      input.value = '';
      showResults([]);
    }
  }
}

/**
 * Mostra resultados da busca (XSS-safe)
 * Usa DOM APIs ao invés de innerHTML para evitar vulnerabilidades XSS
 */
function showResults(results) {
  const resultsContainer = document.getElementById('search-results');
  if (!resultsContainer) return;

  // Limpar container de forma segura
  while (resultsContainer.firstChild) {
    resultsContainer.removeChild(resultsContainer.firstChild);
  }

  if (results.length === 0) {
    const emptyDiv = document.createElement('div');
    emptyDiv.className = 'search-empty';
    emptyDiv.textContent = 'Digite para buscar...';
    resultsContainer.appendChild(emptyDiv);
    return;
  }

  // Criar elementos DOM de forma segura (XSS-safe)
  results.forEach((result) => {
    const link = document.createElement('a');
    link.href = result.url;
    link.className = 'search-result';

    // Ícone
    const iconDiv = document.createElement('div');
    iconDiv.className = 'search-result-icon';
    iconDiv.textContent = '📄';
    link.appendChild(iconDiv);

    // Conteúdo
    const contentDiv = document.createElement('div');
    contentDiv.className = 'search-result-content';

    const h3 = document.createElement('h3');
    h3.textContent = result.title; // textContent é XSS-safe
    contentDiv.appendChild(h3);

    if (result.category) {
      const categorySpan = document.createElement('span');
      categorySpan.className = 'search-result-category';
      categorySpan.textContent = result.category; // textContent é XSS-safe
      contentDiv.appendChild(categorySpan);
    }

    link.appendChild(contentDiv);

    // Seta
    const arrowDiv = document.createElement('div');
    arrowDiv.className = 'search-result-arrow';
    arrowDiv.textContent = '→';
    link.appendChild(arrowDiv);

    resultsContainer.appendChild(link);
  });
}

// Inicializa busca quando DOM estiver pronto
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initSearch);
} else {
  initSearch();
}
