// wiki.js — Arah wiki: hash router, sidebar active state, mobile toggle, search filter.
(function () {
  'use strict';
  var docs = [].slice.call(document.querySelectorAll('.doc'));
  var links = [].slice.call(document.querySelectorAll('.nav a[data-doc]'));
  var side = document.getElementById('side');
  var scrim = document.getElementById('scrim');

  function show(id) {
    var found = false;
    docs.forEach(function (d) {
      var on = d.id === 'doc-' + id;
      d.classList.toggle('active', on);
      if (on) found = true;
    });
    if (!found && docs.length) { docs[0].classList.add('active'); id = docs[0].id.replace('doc-', ''); }
    links.forEach(function (a) { a.classList.toggle('active', a.getAttribute('data-doc') === id); });
    window.scrollTo({ top: 0, behavior: 'auto' });
    closeSide();
  }

  function openSide() { side.classList.add('open'); scrim.classList.add('show'); }
  function closeSide() { side.classList.remove('open'); scrim.classList.remove('show'); }

  // routing
  function onHash() {
    var id = (location.hash || '#visao-geral').replace('#', '');
    show(id);
  }
  window.addEventListener('hashchange', onHash);

  // mobile toggle
  var burger = document.getElementById('burger');
  if (burger) burger.addEventListener('click', openSide);
  if (scrim) scrim.addEventListener('click', closeSide);

  // search filter (filters sidebar links by text)
  var search = document.getElementById('search');
  if (search) {
    search.addEventListener('input', function () {
      var q = search.value.trim().toLowerCase();
      links.forEach(function (a) {
        var t = a.textContent.toLowerCase();
        a.classList.toggle('hidden', q && t.indexOf(q) === -1);
      });
      // hide empty groups
      document.querySelectorAll('.nav .grp').forEach(function (g) {
        var next = g.nextElementSibling, anyVisible = false;
        while (next && !next.classList.contains('grp')) {
          if (next.matches('a') && !next.classList.contains('hidden')) anyVisible = true;
          next = next.nextElementSibling;
        }
        g.style.display = (q && !anyVisible) ? 'none' : '';
      });
    });
  }

  onHash();
})();
