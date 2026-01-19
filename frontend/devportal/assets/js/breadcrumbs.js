/**
 * Sistema de breadcrumbs para DevPortal
 * Gera breadcrumbs baseado na estrutura de navegação
 */

/**
 * Estrutura de navegação do DevPortal
 */
const navigationStructure = {
  '#introducao': { label: 'Introdução', parent: null },
  '#visao-geral': { label: 'Visão Geral', parent: '#introducao' },
  '#como-funciona': { label: 'Como Funciona', parent: '#introducao' },
  '#territorios': { label: 'Territórios', parent: '#como-funciona' },
  '#conceitos': { label: 'Conceitos Fundamentais', parent: '#como-funciona' },
  '#marketplace': { label: 'Marketplace', parent: null },
  '#eventos': { label: 'Eventos', parent: null },
  '#payout-gestao-financeira': { label: 'Gestão Financeira', parent: '#marketplace' },
  '#admin': { label: 'Admin e Filas', parent: null },
  '#operacao-auth': { label: 'Autenticação', parent: null },
  '#operacao-territory-discovery': { label: 'Descoberta de Território', parent: null },
  '#operacao-marketplace-checkout': { label: 'Checkout Marketplace', parent: null },
  '#capacidades-tecnicas': { label: 'Capacidades Técnicas', parent: null },
  '#roadmap': { label: 'Roadmap', parent: null },
  '#contribuir': { label: 'Contribuir', parent: null },
  '#versoes': { label: 'Versões', parent: null },
  '#configure-ambiente': { label: 'Configure seu Ambiente', parent: null },
  '#modelo-dominio': { label: 'Modelo de Domínio', parent: null },
  '#fluxos': { label: 'Fluxos', parent: null },
  '#casos-de-uso': { label: 'Casos de Uso', parent: null },
  '#cenario-onboarding-usuario': { label: 'Onboarding de Usuário', parent: '#casos-de-uso' },
  '#cenario-publicar-midias': { label: 'Publicar Mídias', parent: '#casos-de-uso' },
  '#cenario-assets': { label: 'Assets Territoriais', parent: '#casos-de-uso' },
  '#cenario-chat': { label: 'Chat', parent: '#casos-de-uso' },
  '#cenario-marketplace': { label: 'Marketplace', parent: '#casos-de-uso' },
  '#cenario-eventos': { label: 'Eventos', parent: '#casos-de-uso' },
  '#casos-uso-comuns': { label: 'Casos de Uso Comuns', parent: '#casos-de-uso' },
  '#pontos-atencao': { label: 'Pontos de Atenção', parent: null },
  '#guia-producao-passo-1': { label: 'Passo 1: Configuração', parent: null },
  '#guia-producao-passo-2': { label: 'Passo 2: Deploy', parent: null },
  '#guia-producao-passo-3': { label: 'Passo 3: Monitoramento', parent: null },
  '#guia-producao-passo-4': { label: 'Passo 4: Manutenção', parent: null },
  '#auth': { label: 'Autenticação', parent: null },
  '#territory-session': { label: 'Territory Session', parent: null },
  '#openapi': { label: 'OpenAPI Explorer', parent: null },
  '#erros': { label: 'Tratamento de Erros', parent: null },
  '#onboarding-analistas': { label: 'Onboarding Analistas', parent: null },
  '#onboarding-developers': { label: 'Onboarding Developers', parent: null },
};

/**
 * Gera breadcrumbs baseado na seção atual
 */
function generateBreadcrumbs() {
  const hash = window.location.hash || '#introducao';
  const breadcrumbs = [];
  let current = hash;

  // Constroi caminho do breadcrumb
  while (current && navigationStructure[current]) {
    const item = navigationStructure[current];
    breadcrumbs.unshift({
      label: item.label,
      href: current === '#introducao' ? '#' : current,
    });
    current = item.parent;
  }

  // Sempre adiciona Home no início
  breadcrumbs.unshift({
    label: 'Home',
    href: '#',
  });

  return breadcrumbs;
}

/**
 * Cria componente de breadcrumbs (XSS-safe)
 * Usa DOM APIs ao invés de innerHTML para evitar vulnerabilidades XSS
 */
function createBreadcrumbs() {
  const breadcrumbs = generateBreadcrumbs();
  const container = document.getElementById('breadcrumbs-container');

  if (!container || breadcrumbs.length <= 1) {
    // Sem breadcrumbs se só tem Home ou não há container
    if (container) {
      // Limpar container de forma segura
      while (container.firstChild) {
        container.removeChild(container.firstChild);
      }
    }
    return;
  }

  // Limpar container de forma segura
  while (container.firstChild) {
    container.removeChild(container.firstChild);
  }

  // Criar elementos DOM de forma segura (XSS-safe)
  const nav = document.createElement('nav');
  nav.className = 'breadcrumbs';
  nav.setAttribute('aria-label', 'Breadcrumb');

  const ol = document.createElement('ol');
  ol.className = 'breadcrumbs-list';

  breadcrumbs.forEach((crumb, index) => {
    const li = document.createElement('li');
    li.className = 'breadcrumbs-item';

    if (index < breadcrumbs.length - 1) {
      // Criar link de forma segura
      const link = document.createElement('a');
      link.href = crumb.href;
      link.className = 'breadcrumbs-link';
      link.textContent = crumb.label; // textContent é XSS-safe
      li.appendChild(link);
    } else {
      // Criar span atual de forma segura
      const span = document.createElement('span');
      span.className = 'breadcrumbs-current';
      span.setAttribute('aria-current', 'page');
      span.textContent = crumb.label; // textContent é XSS-safe
      li.appendChild(span);
    }

    ol.appendChild(li);
  });

  nav.appendChild(ol);
  container.appendChild(nav);
}

/**
 * Inicializa breadcrumbs
 */
function initBreadcrumbs() {
  // Usa container já existente no HTML ou cria se não existir
  let breadcrumbsContainer = document.getElementById('breadcrumbs-container');
  
  if (!breadcrumbsContainer) {
    // Fallback: cria container dinamicamente se não estiver no HTML
    const contentContainer = document.getElementById('page-content') || document.querySelector('.container');
    if (!contentContainer) return;

    breadcrumbsContainer = document.createElement('div');
    breadcrumbsContainer.id = 'breadcrumbs-container';
    breadcrumbsContainer.className = 'breadcrumbs-container';
    contentContainer.insertBefore(breadcrumbsContainer, contentContainer.firstChild);
  }

  // Gera breadcrumbs
  createBreadcrumbs();
}

/**
 * Setup de breadcrumbs - inicializa e configura listeners
 */
function setupBreadcrumbs() {
  // Inicializa quando DOM estiver pronto
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initBreadcrumbs);
  } else {
    initBreadcrumbs();
  }

  // Atualiza breadcrumbs quando hash muda (navegação entre seções)
  window.addEventListener('hashchange', initBreadcrumbs);
  window.addEventListener('load', initBreadcrumbs);
}

// Executa setup
setupBreadcrumbs();
