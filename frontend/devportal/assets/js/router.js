/**
 * DevPortal Router - Enterprise-Level Client-Side Routing
 *
 * Single Page Application (SPA) router com:
 * - Hash-based routing (#/comecando, #/api-pratica)
 * - Componentes compartilhados (header, sidebar fixos)
 * - Lazy loading de conteúdo
 * - URLs amigáveis e compartilháveis
 * - Performance otimizada
 * - Acessibilidade (ARIA, keyboard navigation)
 *
 * @module DevPortalRouter
 */

(function() {
  'use strict';

  // ============================================================
  // CONFIGURAÇÃO DE ROTAS
  // ============================================================

  const ROUTES = {
    '': 'home', // Homepage
    'comecando': 'comecando',
    'fundamentos': 'fundamentos',
    'api-pratica': 'api-pratica',
    'funcionalidades': 'funcionalidades',
    'avancado': 'avancado'
  };

  const DEFAULT_ROUTE = 'comecando';

  // ============================================================
  // ROUTER CORE
  // ============================================================

  const Router = {
    currentRoute: null,
    contentContainer: null,
    init: function() {
      // Aguarda DOM estar pronto
      if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', this._onDOMReady.bind(this));
      } else {
        this._onDOMReady();
      }
    },

    _onDOMReady: function() {
      // Encontra container de conteúdo
      this.contentContainer = document.getElementById('page-content');

      if (!this.contentContainer) {
        console.warn('Router: Container #page-content não encontrado, usando fallback');
        // Fallback: usa main como container (sistema antigo continua ativo)
        return;
      }

      // Esconde phase-panels quando router está ativo
      this._hidePhasePanels();

      // Mostra container do router
      this.contentContainer.style.display = 'block';

      // Inicializa navegação
      this._initNavigation();

      // Escuta mudanças de hash
      window.addEventListener('hashchange', this._onHashChange.bind(this));

      // Carrega rota inicial (aguarda um pouco para garantir que DOM está pronto)
      setTimeout(function() {
        this._onHashChange();
      }.bind(this), 100);
    },

    _hidePhasePanels: function() {
      // Esconde phase-panels quando router está ativo
      const phasePanels = document.querySelector('.phase-panels');
      const phaseNavigation = document.querySelector('.phase-navigation');
      if (phasePanels) {
        phasePanels.style.display = 'none';
      }
      // Mantém tabs visíveis para navegação
      // phaseNavigation pode ficar visível ou ser escondido conforme preferência
    },

    _showPhasePanels: function() {
      // Mostra phase-panels (fallback)
      const phasePanels = document.querySelector('.phase-panels');
      if (phasePanels) {
        phasePanels.style.display = 'block';
      }
    },

    _initNavigation: function() {
      // Atualiza links da sidebar para usar hash routing
      const sidebarLinks = document.querySelectorAll('.sidebar-link[href^="#"]');
      sidebarLinks.forEach(function(link) {
        const href = link.getAttribute('href');
        // Se for link para seção dentro da página (#quickstart), mantém como está
        // Se for link para página principal, converte para hash routing
        if (href === '#quickstart' || href === '#auth' || href === '#territory-session') {
          // Links internos mantêm como está
          return;
        }
      });

      // Atualiza tabs de fase para usar hash routing
      const phaseTabs = document.querySelectorAll('.phase-tab');
      phaseTabs.forEach(function(tab) {
        tab.addEventListener('click', function(e) {
          const phase = tab.getAttribute('data-phase');
          if (phase && ROUTES[phase]) {
            e.preventDefault();
            window.location.hash = '#/' + phase;
          }
        });
      });
    },

    _onHashChange: function() {
      const hash = window.location.hash.replace('#/', '').replace('#', '');
      // Suporta sub-rotas: funcionalidades/marketplace
      const routeParts = hash.split('/');
      const mainRoute = routeParts[0];
      const subRoute = routeParts[1];

      // Mapeia rota principal
      const route = mainRoute && ROUTES[mainRoute] ? ROUTES[mainRoute] : (mainRoute ? mainRoute : DEFAULT_ROUTE);

      // Se houver sub-rota, constrói caminho completo
      const fullRoute = subRoute ? route + '/' + subRoute : route;

      if (fullRoute === this.currentRoute) {
        return; // Já está na rota atual
      }

      this.currentRoute = fullRoute;
      this._loadRoute(fullRoute, mainRoute);
    },

    _loadRoute: function(route, mainRoute) {
      // Mostra loading state
      this._showLoading();

      // Carrega conteúdo da rota
      this._fetchContent(route, mainRoute)
        .then(function(html) {
          this._renderContent(html);
          this._updateActiveNavigation(mainRoute || route);
          this._initPageScripts();
          this._scrollToTop();
        }.bind(this))
        .catch(function(error) {
          console.error('Router: Erro ao carregar rota', route, error);
          this._showError('Erro ao carregar página. Tente novamente.');
        }.bind(this));
    },

    _fetchContent: function(route, mainRoute) {
      // Se for homepage ou rota especial, renderiza conteúdo inline
      if (route === 'home' || route === '') {
        return Promise.resolve(this._getHomeContent());
      }

      // Constrói caminho do arquivo
      // Se route tem sub-rota (ex: funcionalidades/marketplace), usa route completo
      // Senão, usa mainRoute para index.html
      const filePath = route.includes('/')
        ? 'pages/' + route + '.html'
        : 'pages/' + route + '/index.html';

      // Tenta carregar arquivo HTML primeiro
      return this._fetchHTML(filePath)
        .catch(function(error) {
          // Se falhar (CORS com file:// ou arquivo não existe), usa fallback inline
          console.log('Router: Fallback para conteúdo inline para rota', route);
          return Promise.resolve(this._getInlineContent(route));
        }.bind(this));
    },

    _fetchHTML: function(filePath) {
      // Tenta fazer fetch do arquivo HTML
      return fetch(filePath)
        .then(function(response) {
          if (!response.ok) {
            throw new Error('HTTP ' + response.status);
          }
          return response.text();
        });
    },

    _getHomeContent: function() {
      // Homepage: mostra hero + navegação rápida
      return `
        <div class="page-hero">
          <span class="eyebrow">Bem-vindo</span>
          <h1>Developer Portal da API Arah</h1>
          <p class="hero-intro">
            Documentação completa da API orientada a território.
            Explore os conceitos, fluxos e funcionalidades para começar a integrar.
          </p>
        </div>

        <div class="quick-nav-grid">
          <a href="#/comecando" class="nav-card">
            <h3>🚀 Começando</h3>
            <p>Quickstart, autenticação e primeiros passos</p>
          </a>

          <a href="#/fundamentos" class="nav-card">
            <h3>📚 Fundamentos</h3>
            <p>Visão geral, conceitos e territórios</p>
          </a>

          <a href="#/api-pratica" class="nav-card">
            <h3>🔧 API Prática</h3>
            <p>Fluxos, casos de uso e exemplos</p>
          </a>

          <a href="#/funcionalidades" class="nav-card">
            <h3>⚙️ Funcionalidades</h3>
            <p>Marketplace, eventos e admin</p>
          </a>

          <a href="#/avancado" class="nav-card">
            <h3>🎓 Avançado</h3>
            <p>FAQ, capacidades técnicas e roadmap</p>
          </a>
        </div>
      `;
    },

    _getInlineContent: function(route) {
      // Fallback: extrai conteúdo do phase-panel correspondente
      // Remove sub-rota se houver (ex: "funcionalidades/marketplace" -> "funcionalidades")
      const mainRoute = route.split('/')[0];

      // Mostra phase-panels temporariamente para extrair conteúdo
      this._showPhasePanels();

      const panel = document.querySelector('[data-phase-panel="' + mainRoute + '"]');
      if (panel) {
        // Se houver sub-rota, tenta encontrar seção específica
        if (route.includes('/')) {
          const subRoute = route.split('/')[1];
          const section = panel.querySelector('#' + subRoute);
          if (section) {
            // Retorna hero + seção específica
            const content = this._buildPageWithHero(subRoute, section.outerHTML);
            this._hidePhasePanels(); // Esconde novamente após extrair
            return content;
          }
        }
        // Retorna conteúdo completo do panel
        const content = panel.innerHTML;
        this._hidePhasePanels(); // Esconde novamente após extrair
        return content;
      }

      this._hidePhasePanels(); // Esconde novamente
      return '<div class="error">Conteúdo não encontrado para rota: ' + route + '</div>';
    },

    _buildPageWithHero: function(title, content) {
      // Constrói página com hero section básico
      const titleFormatted = title.replace(/-/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
      return `
        <section class="page-hero">
          <div class="hero-content">
            <h1>${titleFormatted}</h1>
          </div>
        </section>
        ${content}
      `;
    },

    _renderContent: function(html) {
      if (!this.contentContainer) return;

      // Esconde phase-panels
      this._hidePhasePanels();

      // Mostra container do router
      this.contentContainer.style.display = 'block';

      // Limpa conteúdo anterior
      this.contentContainer.innerHTML = '';

      // Renderiza novo conteúdo
      this.contentContainer.insertAdjacentHTML('beforeend', html);

      // Anima transição
      this.contentContainer.style.opacity = '0';
      setTimeout(function() {
        this.contentContainer.style.transition = 'opacity 0.3s ease';
        this.contentContainer.style.opacity = '1';
      }.bind(this), 10);
    },

    _updateActiveNavigation: function(route) {
      // Atualiza sidebar - marca página ativa
      const sidebarLinks = document.querySelectorAll('.sidebar-link');
      sidebarLinks.forEach(function(link) {
        link.classList.remove('sidebar-link-active', 'active');
      });

      // Atualiza tabs - marca tab ativo
      const phaseTabs = document.querySelectorAll('.phase-tab');
      phaseTabs.forEach(function(tab) {
        const tabPhase = tab.getAttribute('data-phase');
        if (tabPhase === route) {
          tab.classList.add('active');
        } else {
          tab.classList.remove('active');
        }
      });

      // Atualiza título da página
      const routeTitles = {
        'home': 'Home',
        'comecando': 'Começando',
        'fundamentos': 'Fundamentos',
        'api-pratica': 'API Prática',
        'funcionalidades': 'Funcionalidades',
        'avancado': 'Avançado'
      };
      const title = routeTitles[route] || 'DevPortal';
      document.title = title + ' • Arah API • Developer Portal';
    },

    _initPageScripts: function() {
      // Reinicializa scripts da página (accordions, copy buttons, etc.)
      // Os scripts globais já devem estar ativos, mas podemos re-inicializar se necessário

      // Trigger event para scripts que precisam saber que conteúdo mudou
      window.dispatchEvent(new CustomEvent('page:loaded', {
        detail: { route: this.currentRoute }
      }));
    },

    _showLoading: function() {
      if (this.contentContainer) {
        this.contentContainer.innerHTML = '<div class="loading-spinner">Carregando...</div>';
      }
    },

    _showError: function(message) {
      if (this.contentContainer) {
        this.contentContainer.innerHTML = '<div class="error-message">' + message + '</div>';
      }
    },

    _scrollToTop: function() {
      // Scroll suave para o topo, respeitando header fixo
      const headerHeight = document.querySelector('.header')?.offsetHeight || 100;
      window.scrollTo({
        top: headerHeight,
        behavior: 'smooth'
      });
    }
  };

  // ============================================================
  // INICIALIZAÇÃO
  // ============================================================

  // Exporta para uso global
  window.DevPortalRouter = Router;

  // Inicializa router - agora suporta carregamento de arquivos HTML
  // Phase-panels servem como fallback se arquivos não estiverem disponíveis
  Router.init();

})();
