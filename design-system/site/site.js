// site.js — Arah site interactions: nav scroll state, hero theme, reveal-on-scroll, form.
(function () {
  'use strict';

  var nav = document.getElementById('nav');
  var hero = document.getElementById('top');

  // Nav background on scroll
  function onScroll() {
    var y = window.scrollY || window.pageYOffset;
    if (y > 24) nav.classList.add('scrolled');
    else nav.classList.remove('scrolled');
    // hero theme: nav transparent while over hero
    if (hero) {
      var h = hero.offsetHeight - 90;
      if (y < h) nav.classList.add('on-hero');
      else nav.classList.remove('on-hero');
    }
  }
  window.addEventListener('scroll', onScroll, { passive: true });
  onScroll();

  // Reveal on scroll — opt-in only when JS runs, so content is never stuck hidden.
  document.documentElement.classList.add('reveal-on');
  var revealEls = [].slice.call(document.querySelectorAll('.reveal'));
  if ('IntersectionObserver' in window && revealEls.length) {
    var io = new IntersectionObserver(function (entries) {
      entries.forEach(function (e) {
        if (e.isIntersecting) { e.target.classList.add('in'); io.unobserve(e.target); }
      });
    }, { threshold: 0.12, rootMargin: '0px 0px -8% 0px' });
    revealEls.forEach(function (el) { io.observe(el); });
    // Safety: reveal anything still hidden after 2.4s (e.g. if observer misfires)
    setTimeout(function () { revealEls.forEach(function (el) { el.classList.add('in'); }); }, 2400);
  } else {
    revealEls.forEach(function (el) { el.classList.add('in'); });
  }

  // Smooth-scroll for in-page anchors with fixed-nav offset (respects reduced motion)
  var reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  document.querySelectorAll('a[href^="#"]').forEach(function (a) {
    a.addEventListener('click', function (ev) {
      var id = a.getAttribute('href');
      if (id === '#' || id.length < 2) return;
      var target = document.querySelector(id);
      if (!target) return;
      ev.preventDefault();
      var navH = nav ? nav.offsetHeight : 0;
      var y = target.getBoundingClientRect().top + (window.scrollY || window.pageYOffset) - navH - 8;
      window.scrollTo({ top: id === '#top' ? 0 : y, behavior: reduce ? 'auto' : 'smooth' });
      history.replaceState(null, '', id);
    });
  });

  // Community form (demo — no backend)
  window.arahSubmit = function (e) {
    e.preventDefault();
    var form = e.target;
    var note = document.getElementById('formNote');
    var nome = (form.querySelector('[name="nome"]') || {}).value || '';
    var first = nome.trim().split(' ')[0];
    note.innerHTML = '\u2713 Recebido' + (first ? ', ' + first : '') + '. Entraremos em contato para iniciar a implementa\u00e7\u00e3o no seu territ\u00f3rio. \uD83C\uDF31';
    note.style.color = '#A6D6B9';
    form.reset();
    return false;
  };
})();
