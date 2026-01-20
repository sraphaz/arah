/**
 * Inicializador do Menu Mobile para DevPortal
 * Versão: 20250120-04
 */
(function() {
  'use strict';
  const DevPortalMobileMenuInit = { version: '20250120-04', namespace: 'DevPortalMobileMenuInit' };
  
  DevPortalMobileMenuInit._waitForModuleAndInit = function() {
    if (window.MobileMenu && window.MobileMenu.init) {
      
      window.MobileMenu.init({ toggleButtonId: 'mobile-menu-toggle', sidebarSelector: '.sidebar-container', desktopBreakpoint: 1024 });
    } else {
      setTimeout(DevPortalMobileMenuInit._waitForModuleAndInit, 100);
    }
  };
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', DevPortalMobileMenuInit._waitForModuleAndInit);
  } else {
    setTimeout(DevPortalMobileMenuInit._waitForModuleAndInit, 0);
  }
  window.DevPortalMobileMenuInit = DevPortalMobileMenuInit;
})();

