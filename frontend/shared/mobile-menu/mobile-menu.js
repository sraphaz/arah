/**
 * Sistema de Menu Mobile Compartilhado
 * Lógica unificada para Wiki e DevPortal
 */

(function() {
  'use strict';

  function initMobileMenu(config) {
    const {
      toggleButtonId = 'mobile-menu-toggle',
      sidebarSelector = '.sidebar-container',
      desktopBreakpoint = 1024,
      onLinkClick = null
    } = config || {};

    const toggleButton = document.getElementById(toggleButtonId);
    const sidebar = document.querySelector(sidebarSelector);
    const body = document.body;

    if (!toggleButton || !sidebar) {
      return null;
    }

    let isOpen = false;

    function isMobile() {
      return window.innerWidth < desktopBreakpoint;
    }

    function removeOverlay() {
      const staticOverlays = document.querySelectorAll('.mobile-menu-overlay');
      staticOverlays.forEach(staticOverlay => {
        if (staticOverlay.parentNode) {
          staticOverlay.parentNode.removeChild(staticOverlay);
        }
      });
    }

    function handleDocumentClick(e) {
      const clickedButton = toggleButton && (toggleButton.contains(e.target) || toggleButton === e.target);
      const clickedSidebar = sidebar && (sidebar.contains(e.target) || sidebar === e.target);
      
      if (clickedButton || clickedSidebar || !isOpen) {
        return;
      }

      closeMenu();
    }

    function openMenu() {
      if (!sidebar || !toggleButton) return;

      isOpen = true;

      sidebar.classList.add('sidebar-mobile-open');
      body.classList.add('mobile-menu-open');
      toggleButton.classList.add('active');
      toggleButton.setAttribute('aria-expanded', 'true');
      sidebar.setAttribute('aria-hidden', 'false');

      removeOverlay();

      body.style.overflow = 'hidden';

      const firstLink = sidebar.querySelector('a[href]');
      if (firstLink) {
        setTimeout(() => firstLink.focus(), 100);
      }
    }

    function closeMenu() {
      if (!sidebar || !toggleButton) return;

      isOpen = false;

      body.classList.remove('mobile-menu-open');
      sidebar.classList.remove('sidebar-mobile-open');
      toggleButton.classList.remove('active');
      toggleButton.setAttribute('aria-expanded', 'false');

      if (isMobile()) {
        sidebar.setAttribute('aria-hidden', 'true');
      } else {
        sidebar.removeAttribute('aria-hidden');
      }

      removeOverlay();

      body.style.overflow = '';
    }

    function toggleMenu() {
      if (isOpen) {
        closeMenu();
      } else {
        openMenu();
      }
    }

    function handleSidebarClick(e) {
      
      

      const isButton = e.target.tagName === 'BUTTON' || e.target.closest('button');
      if (isButton) {
        return;
      }

      const link = e.target.closest('a[href]');
      if (link && link.closest(sidebarSelector) && isMobile() && isOpen) {
        if (onLinkClick) {
          onLinkClick(link, closeMenu);
        } else {
          setTimeout(() => closeMenu(), 150);
        }
      }
    }

    function handleEscapeKey(e) {
      if (e.key === 'Escape' && isOpen) {
        closeMenu();
        if (toggleButton) {
          toggleButton.focus();
        }
      }
    }

    function handleResize() {
      if (window.innerWidth >= desktopBreakpoint && isOpen) {
        closeMenu();
      }
    }

    function handleHashChange() {
      if (isMobile() && isOpen) {
        closeMenu();
      }
    }

    if (isMobile()) {
      body.classList.remove('mobile-menu-open');
      body.style.overflow = '';
      removeOverlay();
      sidebar.setAttribute('aria-hidden', 'true');
    }

    toggleButton.addEventListener('click', function(e) {
      e.preventDefault();
      
      
      toggleMenu();
    });

    sidebar.addEventListener('click', handleSidebarClick);

    document.addEventListener('click', handleDocumentClick);
    document.addEventListener('keydown', handleEscapeKey);

    let resizeTimeout;
    window.addEventListener('resize', function() {
      clearTimeout(resizeTimeout);
      resizeTimeout = setTimeout(handleResize, 150);
    });

    window.addEventListener('hashchange', handleHashChange);

    return {
      open: openMenu,
      close: closeMenu,
      toggle: toggleMenu,
      isOpen: () => isOpen
    };
  }

  window.MobileMenu = {
    init: initMobileMenu,
    version: '20250120-03'
  };
})();

