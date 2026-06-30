// persist.jsx — lightweight localStorage persistence for the Arah app demo.
// Survives reload: feed posts, store products, wallet, chats, and session prefs.
// Loads right after data.jsx so window.ARAH is hydrated before any screen renders.

(function () {
  const KEY = 'arah_app_v1';

  window.arahLoad = function () {
    try { return JSON.parse(localStorage.getItem(KEY)) || {}; } catch (e) { return {}; }
  };
  window.arahSave = function (patch) {
    try {
      const cur = window.arahLoad();
      localStorage.setItem(KEY, JSON.stringify(Object.assign({}, cur, patch)));
    } catch (e) { /* quota / private mode — ignore */ }
  };

  // Snapshot the mutable slices of window.ARAH (no functions — content/wallet/store/chats are plain)
  window.arahSnapshot = function () {
    const A = window.ARAH; if (!A) return;
    window.arahSave({
      data: {
        content: A.content,   // per-territory feed/events (t1–t3)
        myStore: A.myStore,
        wallet: A.wallet,
        chats: A.chats,
      },
    });
  };

  // Apply saved slices back onto window.ARAH
  window.arahHydrate = function () {
    const saved = window.arahLoad();
    const d = saved && saved.data;
    if (!d || !window.ARAH) return;
    if (d.content) {
      // merge per-territory (keep territories that weren't saved)
      Object.keys(d.content).forEach(function (k) { window.ARAH.content[k] = d.content[k]; });
    }
    if (d.myStore) window.ARAH.myStore = d.myStore;
    if (d.wallet) window.ARAH.wallet = d.wallet;
    if (d.chats) window.ARAH.chats = d.chats;
  };

  // Save / read the React-session slice (prefs that live in App state)
  window.arahSaveApp = function (appSlice) { window.arahSave({ app: appSlice }); };
  window.arahLoadApp = function () { return window.arahLoad().app || {}; };

  // Clear everything (used by a "limpar dados" affordance / logout-reset if desired)
  window.arahReset = function () { try { localStorage.removeItem(KEY); } catch (e) {} };

  // Hydrate immediately at load (data.jsx already ran)
  window.arahHydrate();
})();
