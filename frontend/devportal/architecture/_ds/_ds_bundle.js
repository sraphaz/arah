/* @ds-bundle: {"format":3,"namespace":"ArahDesignSystem_c7fa51","components":[],"sourceHashes":{"site/site.js":"8354e657b587","ui_kits/app/app.jsx":"9225df8294fe","ui_kits/app/chrome.jsx":"2f67336b329c","ui_kits/app/components.jsx":"fa71d352ad20","ui_kits/app/data.jsx":"5b21881ad46a","ui_kits/app/ios-frame.jsx":"be3343be4b51","ui_kits/app/persist.jsx":"1dd533b834b0","ui_kits/app/screens1.jsx":"82911d280b8c","ui_kits/app/screens10.jsx":"53821cdd9596","ui_kits/app/screens11.jsx":"bc529e1d5b99","ui_kits/app/screens12.jsx":"cd1ce13f4819","ui_kits/app/screens13.jsx":"7cdd990cfb9a","ui_kits/app/screens14.jsx":"017cd6964218","ui_kits/app/screens15.jsx":"48b8a7ed87ae","ui_kits/app/screens16.jsx":"160f890ba039","ui_kits/app/screens17.jsx":"6c6915ebef83","ui_kits/app/screens18.jsx":"e0cc8d218035","ui_kits/app/screens2.jsx":"7c3377358571","ui_kits/app/screens3.jsx":"76cc7e29119e","ui_kits/app/screens4.jsx":"ec22bc55dc3b","ui_kits/app/screens5.jsx":"7b96f8e713f6","ui_kits/app/screens6.jsx":"e548b2c34ec6","ui_kits/app/screens7.jsx":"45f3084f27f5","ui_kits/app/screens8.jsx":"9b72c4d61414","ui_kits/app/screens9.jsx":"1b653fbf9fc8","wiki/wiki.js":"7779f5f1fe26"},"inlinedExternals":[],"unexposedExports":[]} */

(() => {

const __ds_ns = (window.ArahDesignSystem_c7fa51 = window.ArahDesignSystem_c7fa51 || {});

const __ds_scope = {};

(__ds_ns.__errors = __ds_ns.__errors || []);

// site/site.js
try { (() => {
// site.js — Arah site interactions: nav scroll state, hero theme, reveal-on-scroll, form.
(function () {
  'use strict';

  var nav = document.getElementById('nav');
  var hero = document.getElementById('top');

  // Nav background on scroll
  function onScroll() {
    var y = window.scrollY || window.pageYOffset;
    if (y > 24) nav.classList.add('scrolled');else nav.classList.remove('scrolled');
    // hero theme: nav transparent while over hero
    if (hero) {
      var h = hero.offsetHeight - 90;
      if (y < h) nav.classList.add('on-hero');else nav.classList.remove('on-hero');
    }
  }
  window.addEventListener('scroll', onScroll, {
    passive: true
  });
  onScroll();

  // Reveal on scroll — opt-in only when JS runs, so content is never stuck hidden.
  document.documentElement.classList.add('reveal-on');
  var revealEls = [].slice.call(document.querySelectorAll('.reveal'));
  if ('IntersectionObserver' in window && revealEls.length) {
    var io = new IntersectionObserver(function (entries) {
      entries.forEach(function (e) {
        if (e.isIntersecting) {
          e.target.classList.add('in');
          io.unobserve(e.target);
        }
      });
    }, {
      threshold: 0.12,
      rootMargin: '0px 0px -8% 0px'
    });
    revealEls.forEach(function (el) {
      io.observe(el);
    });
    // Safety: reveal anything still hidden after 2.4s (e.g. if observer misfires)
    setTimeout(function () {
      revealEls.forEach(function (el) {
        el.classList.add('in');
      });
    }, 2400);
  } else {
    revealEls.forEach(function (el) {
      el.classList.add('in');
    });
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
      window.scrollTo({
        top: id === '#top' ? 0 : y,
        behavior: reduce ? 'auto' : 'smooth'
      });
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
})(); } catch (e) { __ds_ns.__errors.push({ path: "site/site.js", error: String((e && e.message) || e) }); }

// ui_kits/app/app.jsx
try { (() => {
function _extends() { return _extends = Object.assign ? Object.assign.bind() : function (n) { for (var e = 1; e < arguments.length; e++) { var t = arguments[e]; for (var r in t) ({}).hasOwnProperty.call(t, r) && (n[r] = t[r]); } return n; }, _extends.apply(null, arguments); }
// app.jsx — Arah orchestrator. Stages, tabs, overlay stack, roles, smooth transitions.

function App() {
  const _saved = window.arahLoadApp && window.arahLoadApp() || {};
  const [stage, setStage] = React.useState(_saved.stage || 'login'); // login | onboarding | app
  const [tab, setTab] = React.useState('feed'); // feed|explore|post|market|profile
  const [stack, setStack] = React.useState([]); // overlay stack: {type, param}
  const [sheet, setSheet] = React.useState(false);
  const [territoryId, setTerritoryId] = React.useState(_saved.territoryId || 't1');
  const [role, setRole] = React.useState(_saved.role || 'visitante'); // visitante | morador | curador
  const [likes, setLikes] = React.useState(_saved.likes || {
    p1: true
  });
  const [interests, setInterests] = React.useState(_saved.interests || [...window.ARAH.profile.interests]);
  const [notifs, setNotifs] = React.useState(() => {
    const reads = _saved.notifReads || [];
    return window.ARAH.notifications.map(n => reads.includes(n.id) ? {
      ...n,
      read: true
    } : n);
  });
  const [joinedGroups, setJoinedGroups] = React.useState(_saved.joinedGroups || {});
  const [journey, setJourney] = React.useState(null); // {id, ctx}
  const [toast, setToast] = React.useState(null);
  const [, forceTick] = React.useReducer(x => x + 1, 0);

  // Global bridges — any deep screen can navigate or mutate session data
  React.useEffect(() => {
    window.openJourney = (id, ctx) => setJourney({
      id,
      ctx
    });
    window.arahMutate = fn => {
      try {
        fn && fn();
      } finally {
        window.arahSnapshot && window.arahSnapshot();
        forceTick();
      }
    };
    window.arahToast = (msg, icon) => {
      setToast({
        msg,
        icon: icon || 'check_circle'
      });
      setTimeout(() => setToast(null), 2200);
    };
    window.appNav = {
      goTab: t => {
        setStack([]);
        setTab(t);
      },
      push: (type, param) => setStack(s => [...s, {
        type,
        param
      }]),
      openChat: (name, avatar) => {
        let c = window.ARAH.chats.find(x => x.name === name);
        if (!c) {
          c = {
            id: 'ch' + Date.now(),
            name,
            avatar: avatar || '#4F956F',
            role: 'morador',
            last: '',
            time: 'agora',
            unread: 0,
            thread: []
          };
          window.ARAH.chats.unshift(c);
        }
        setStack(s => [...s, {
          type: 'chat',
          param: c.id
        }]);
      }
    };
    return () => {
      delete window.openJourney;
      delete window.arahMutate;
      delete window.arahToast;
      delete window.appNav;
    };
  }, []);

  // Persist session prefs whenever they change
  React.useEffect(() => {
    if (stage === 'login') return;
    window.arahSaveApp && window.arahSaveApp({
      stage,
      territoryId,
      role,
      likes,
      interests,
      joinedGroups,
      notifReads: notifs.filter(n => n.read).map(n => n.id)
    });
  }, [stage, territoryId, role, likes, interests, joinedGroups, notifs]);
  const territory = window.ARAH.territories.find(t => t.id === territoryId);
  const content = window.ARAH.getContent(territoryId);
  const unread = notifs.filter(n => !n.read).length;
  const chatUnread = window.ARAH.chats.reduce((s, c) => s + c.unread, 0);
  const top = stack[stack.length - 1] || null;
  const showToast = (msg, icon = 'check_circle') => {
    setToast({
      msg,
      icon
    });
    setTimeout(() => setToast(null), 2200);
  };
  const push = (type, param) => setStack(s => [...s, {
    type,
    param
  }]);
  const pop = () => setStack(s => s.slice(0, -1));
  const goTab = t => {
    setStack([]);
    setTab(t);
  };
  const toggleLike = id => setLikes(l => ({
    ...l,
    [id]: !l[id]
  }));
  const toggleInterest = i => setInterests(s => s.includes(i) ? s.filter(x => x !== i) : [...s, i]);
  const readNotif = id => setNotifs(ns => ns.map(n => n.id === id ? {
    ...n,
    read: true
  } : n));
  const enterTerritory = id => {
    setTerritoryId(id);
    goTab('feed');
    showToast(`Você está em ${window.ARAH.territories.find(t => t.id === id).name}`, 'forest');
  };
  const setRoleAndToast = r => {
    setRole(r);
    const m = {
      visitante: 'Vendo como visitante',
      morador: 'Você confirmou residência',
      curador: 'Vendo como curador'
    };
    showToast(m[r], r === 'morador' ? 'verified' : 'switch_account');
  };
  const openLink = link => {
    if (!link) return;
    if (link.startsWith('post:')) push('post', link.slice(5));else if (link.startsWith('map:')) push('map', link.slice(4));else if (link === 'events') push('events');else if (link === 'profile') goTab('profile');
  };
  const openTool = id => {
    if (id === 'logout') {
      setStage('login');
      setTab('feed');
      setStack([]);
      setRole('visitante');
      return;
    }
    if (id === 'chat') {
      push('chats');
      return;
    }
    if (id === 'market') {
      push('market');
      return;
    }
    push(id);
  };

  // ---- Stage screens ----
  if (stage === 'login') return /*#__PURE__*/React.createElement(Device, {
    viewKey: "login"
  }, /*#__PURE__*/React.createElement(LoginScreen, {
    onLogin: () => setStage('onboarding')
  }));
  if (stage === 'onboarding') return /*#__PURE__*/React.createElement(Device, {
    viewKey: "onb",
    scroll: true,
    header: {
      kind: 'back',
      title: 'Entrar no território',
      onBack: () => setStage('login')
    }
  }, /*#__PURE__*/React.createElement(OnboardingScreen, {
    onContinue: id => {
      setTerritoryId(id);
      setStage('app');
      setRole('visitante');
      showToast('Bem-vinda a Camburi', 'forest');
    }
  }));

  // ---- Overlay (pushed) screens ----
  if (top) {
    const titles = {
      map: 'Mapa',
      events: 'Eventos',
      health: 'Saúde do território',
      settings: 'Configurações',
      elections: 'Eleições & votação',
      manage: 'Gestão do território',
      chats: 'Mensagens',
      post: 'Publicação',
      notifications: 'Notificações',
      store: 'Minha loja',
      saved: 'Salvos',
      market: 'Mercado',
      groupbuy: 'Compra coletiva',
      hosting: 'Hospedagem',
      demands: 'Demandas & ofertas',
      trades: 'Trocas comunitárias',
      delivery: 'Entregas',
      wallet: 'Carteira territorial',
      sitters: 'Babás',
      wellness: 'Bem-estar',
      rental: 'Aluguéis',
      digital: 'Hub digital',
      moderation: 'Moderação',
      subscriptions: 'Assinaturas',
      metrics: 'Painel de métricas',
      seeds: 'Banco de sementes',
      learning: 'Aprendizado',
      ai: 'Assistente IA',
      achievements: 'Conquistas'
    };
    const chat = top.type === 'chat' ? window.ARAH.chats.find(c => c.id === top.param) : null;
    const title = chat ? chat.name : titles[top.type];
    const noPad = top.type === 'map' || top.type === 'post' || top.type === 'chat' || top.type === 'ai';
    let inner;
    if (top.type === 'map') inner = /*#__PURE__*/React.createElement(MapScreen, {
      territory: territory,
      content: content,
      focusId: top.param
    });
    if (top.type === 'events') inner = /*#__PURE__*/React.createElement(EventsScreen, {
      territory: territory,
      content: content,
      role: role
    });
    if (top.type === 'health') inner = /*#__PURE__*/React.createElement(HealthScreen, {
      territory: territory,
      content: content
    });
    if (top.type === 'settings') inner = /*#__PURE__*/React.createElement(SettingsScreen, null);
    if (top.type === 'store') inner = /*#__PURE__*/React.createElement(StoreScreen, null);
    if (top.type === 'saved') inner = /*#__PURE__*/React.createElement(SavedScreen, null);
    if (top.type === 'elections') inner = /*#__PURE__*/React.createElement(ElectionsScreen, {
      role: role
    });
    if (top.type === 'manage') inner = /*#__PURE__*/React.createElement(ManageScreen, {
      territory: territory
    });
    if (top.type === 'moderation') inner = /*#__PURE__*/React.createElement(ManageScreen, {
      territory: territory
    });
    if (top.type === 'chats') inner = /*#__PURE__*/React.createElement(ChatListScreen, {
      onOpen: id => push('chat', id)
    });
    if (top.type === 'chat') inner = /*#__PURE__*/React.createElement(ChatThreadScreen, {
      chatId: top.param
    });
    if (top.type === 'post') inner = /*#__PURE__*/React.createElement(PostDetailScreen, {
      postId: top.param,
      likes: likes,
      onLike: toggleLike
    });
    if (top.type === 'notifications') inner = /*#__PURE__*/React.createElement(NotificationsScreen, {
      items: notifs,
      onRead: readNotif,
      onOpen: openLink
    });
    if (top.type === 'market') inner = /*#__PURE__*/React.createElement(MarketScreen, {
      joinedGroups: joinedGroups,
      onJoinGroup: id => setJoinedGroups(g => ({
        ...g,
        [id]: !g[id]
      }))
    });
    if (top.type === 'groupbuy') inner = /*#__PURE__*/React.createElement(GroupBuyScreen, null);
    if (top.type === 'hosting') inner = /*#__PURE__*/React.createElement(HostingScreen, null);
    if (top.type === 'demands') inner = /*#__PURE__*/React.createElement(DemandsScreen, null);
    if (top.type === 'trades') inner = /*#__PURE__*/React.createElement(TradesScreen, null);
    if (top.type === 'delivery') inner = /*#__PURE__*/React.createElement(DeliveryScreen, null);
    if (top.type === 'wallet') inner = /*#__PURE__*/React.createElement(WalletScreen, null);
    if (top.type === 'sitters') inner = /*#__PURE__*/React.createElement(SittersScreen, null);
    if (top.type === 'wellness') inner = /*#__PURE__*/React.createElement(WellnessScreen, null);
    if (top.type === 'rental') inner = /*#__PURE__*/React.createElement(RentalScreen, null);
    if (top.type === 'digital') inner = /*#__PURE__*/React.createElement(DigitalScreen, null);
    if (top.type === 'metrics') inner = /*#__PURE__*/React.createElement(MetricsScreen, {
      territory: territory
    });
    if (top.type === 'seeds') inner = /*#__PURE__*/React.createElement(SeedsScreen, null);
    if (top.type === 'learning') inner = /*#__PURE__*/React.createElement(LearningScreen, null);
    if (top.type === 'ai') inner = /*#__PURE__*/React.createElement(AIScreen, {
      territory: territory
    });
    if (top.type === 'achievements') inner = /*#__PURE__*/React.createElement(AchievementsScreen, null);
    if (top.type === 'subscriptions') inner = /*#__PURE__*/React.createElement(SubscriptionsScreen, null);
    const action = top.type === 'notifications' && unread ? {
      icon: 'done_all',
      onClick: () => setNotifs(ns => ns.map(n => ({
        ...n,
        read: true
      })))
    } : null;
    return /*#__PURE__*/React.createElement(Device, {
      viewKey: 'ov' + stack.length + top.type + (top.param || ''),
      scroll: !noPad,
      noPad: noPad,
      journey: journey,
      onCloseJourney: () => setJourney(null),
      onApproveResidencia: () => setRoleAndToast('morador'),
      header: {
        kind: 'back',
        title,
        onBack: pop,
        action
      }
    }, inner);
  }

  // ---- Main shell tabs ----
  const titles = {
    explore: 'Explorar',
    post: 'Publicar',
    services: 'Serviços do território',
    profile: 'Perfil'
  };
  let body, header;
  if (tab === 'feed') {
    header = {
      kind: 'territory'
    };
    body = /*#__PURE__*/React.createElement(FeedScreen, {
      territory: territory,
      content: content,
      role: role,
      likes: likes,
      onLike: toggleLike,
      onOpen: id => push('post', id)
    });
  } else {
    header = {
      kind: 'title',
      title: titles[tab]
    };
    if (tab === 'explore') body = /*#__PURE__*/React.createElement(ExploreScreen, {
      territory: territory,
      activeId: territoryId,
      role: role,
      onEnter: enterTerritory,
      onMap: () => push('map'),
      onTool: openTool
    });
    if (tab === 'post') body = /*#__PURE__*/React.createElement(CreatePostScreen, {
      territory: territory,
      onPublish: data => {
        const c = window.ARAH.content[territoryId];
        if (c) c.feed.unshift({
          id: 'np' + Date.now(),
          author: window.ARAH.profile.name,
          handle: window.ARAH.profile.handle,
          avatar: window.ARAH.profile.avatar,
          role: role === 'visitante' ? 'visitante' : 'morador',
          time: 'agora',
          type: data.type,
          visibility: data.vis,
          title: data.title || 'Sem título',
          body: data.body || '',
          photo: data.photo,
          likes: 0,
          comments: 0
        });
        window.arahSnapshot && window.arahSnapshot();
        goTab('feed');
        showToast('Post publicado no território');
      }
    });
    if (tab === 'services') body = /*#__PURE__*/React.createElement(ServicesHubScreen, {
      territory: territory,
      role: role,
      onOpen: openTool
    });
    if (tab === 'profile') body = /*#__PURE__*/React.createElement(ProfileScreen, {
      role: role,
      onSetRole: setRoleAndToast,
      onOpen: openTool,
      interests: interests,
      onToggleInterest: toggleInterest
    });
  }
  return /*#__PURE__*/React.createElement(Device, {
    viewKey: 'tab-' + tab,
    scroll: true,
    header: header,
    territory: territory,
    onTerritoryTap: () => setSheet(true),
    onChat: () => push('chats'),
    onNotif: () => push('notifications'),
    unread: unread,
    chatUnread: chatUnread,
    bottom: /*#__PURE__*/React.createElement(BottomNav, {
      active: tab,
      onNav: goTab
    }),
    toast: toast,
    journey: journey,
    onCloseJourney: () => setJourney(null),
    onApproveResidencia: () => setRoleAndToast('morador')
  }, body, sheet && /*#__PURE__*/React.createElement(TerritorySheet, {
    activeId: territoryId,
    onPick: id => {
      setSheet(false);
      enterTerritory(id);
    },
    onClose: () => setSheet(false)
  }));
}

// Device wrapper with smooth crossfade between views (keyed on viewKey).
function Device({
  children,
  viewKey,
  header,
  bottom,
  scroll,
  noPad,
  territory,
  onTerritoryTap,
  onChat,
  onNotif,
  unread,
  chatUnread,
  toast,
  journey,
  onCloseJourney,
  onApproveResidencia
}) {
  return /*#__PURE__*/React.createElement(IOSDevice, {
    dark: true,
    width: 390,
    height: 844
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      background: `radial-gradient(130% 80% at 50% -8%, #15201A 0%, ${T.bg} 58%)`,
      display: 'flex',
      flexDirection: 'column'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: 56,
      flexShrink: 0
    }
  }), header && /*#__PURE__*/React.createElement(Header, _extends({}, header, {
    territory: territory,
    onTerritoryTap: onTerritoryTap,
    onChat: onChat,
    onNotif: onNotif,
    unread: unread,
    chatUnread: chatUnread
  })), /*#__PURE__*/React.createElement("div", {
    key: viewKey,
    className: "screen-fade",
    style: {
      flex: 1,
      minHeight: 0,
      display: 'flex',
      flexDirection: 'column'
    }
  }, noPad ? /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minHeight: 0,
      overflow: 'hidden',
      position: 'relative'
    }
  }, children) : /*#__PURE__*/React.createElement("div", {
    className: "appscroll",
    style: {
      flex: 1,
      overflowY: scroll ? 'auto' : 'hidden',
      overflowX: 'hidden',
      position: 'relative'
    }
  }, children, scroll && bottom && /*#__PURE__*/React.createElement("div", {
    style: {
      height: 96
    }
  }))), bottom && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
      zIndex: 30
    }
  }, bottom), toast && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 18,
      right: 18,
      bottom: bottom ? 108 : 40,
      zIndex: 200,
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      padding: '13px 16px',
      background: T.glassGrad,
      backdropFilter: 'blur(14px)',
      WebkitBackdropFilter: 'blur(14px)',
      borderRadius: 15,
      border: `1px solid ${T.lineHi}`,
      boxShadow: '0 12px 30px rgba(0,0,0,0.45)',
      animation: 'toastIn .3s cubic-bezier(.16,1,.3,1)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: toast.icon,
    size: 20,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg,
      fontWeight: 500
    }
  }, toast.msg)), journey && /*#__PURE__*/React.createElement(JourneyHost, {
    journey: journey,
    onClose: onCloseJourney,
    onApprove: onApproveResidencia
  })));
}
function Header({
  kind,
  title,
  action,
  onBack,
  territory,
  onTerritoryTap,
  onChat,
  onNotif,
  unread,
  chatUnread
}) {
  if (kind === 'territory') return /*#__PURE__*/React.createElement(TopBar, {
    territory: territory,
    onTerritoryTap: onTerritoryTap,
    onChat: onChat,
    onNotif: onNotif,
    unread: unread,
    chatUnread: chatUnread
  });
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 12,
      padding: '6px 16px 14px'
    }
  }, kind === 'back' && /*#__PURE__*/React.createElement("button", {
    onClick: onBack,
    style: iconBtn
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "arrow_back",
    size: 22,
    color: T.fg
  })), /*#__PURE__*/React.createElement("h2", {
    style: {
      margin: 0,
      flex: 1,
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 21,
      color: T.fg,
      letterSpacing: -0.4,
      whiteSpace: 'nowrap',
      overflow: 'hidden',
      textOverflow: 'ellipsis'
    }
  }, title), action && /*#__PURE__*/React.createElement("button", {
    onClick: action.onClick,
    style: iconBtn
  }, /*#__PURE__*/React.createElement(Icon, {
    name: action.icon,
    size: 21,
    color: T.green
  })));
}
ReactDOM.createRoot(document.getElementById('root')).render(/*#__PURE__*/React.createElement(App, null));
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/app.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/chrome.jsx
try { (() => {
// chrome.jsx — Arah app shell: territory top bar (logo + chat + notifications) + bottom nav.

function TopBar({
  territory,
  onTerritoryTap,
  onChat,
  onNotif,
  unread = 0,
  chatUnread = 0
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      padding: '6px 16px 14px',
      position: 'relative',
      zIndex: 5
    }
  }, /*#__PURE__*/React.createElement("button", {
    onClick: onTerritoryTap,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      flex: 1,
      minWidth: 0,
      background: 'transparent',
      border: 'none',
      cursor: 'pointer',
      padding: 0,
      textAlign: 'left',
      WebkitTapHighlightColor: 'transparent'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 40,
      height: 40,
      borderRadius: 13,
      flexShrink: 0,
      overflow: 'hidden',
      background: 'linear-gradient(150deg, #1F2A22, #141A15)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      border: `1px solid ${T.lineHi}`,
      boxShadow: T.shadowSoft
    }
  }, /*#__PURE__*/React.createElement(Logo, {
    size: 28,
    mark: "png"
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 10.5,
      fontWeight: 600,
      letterSpacing: 1.2,
      textTransform: 'uppercase',
      color: T.fg3,
      marginBottom: 1
    }
  }, "Territ\xF3rio"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 5
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontSize: 19,
      fontWeight: 600,
      color: T.fg,
      letterSpacing: -0.3,
      whiteSpace: 'nowrap',
      overflow: 'hidden',
      textOverflow: 'ellipsis'
    }
  }, territory.name), /*#__PURE__*/React.createElement(Icon, {
    name: "unfold_more",
    size: 17,
    color: T.fg3
  })))), /*#__PURE__*/React.createElement("button", {
    onClick: onChat,
    style: topIconBtn
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "forum",
    size: 21,
    color: T.fg2
  }), chatUnread > 0 && /*#__PURE__*/React.createElement(Dot, null)), /*#__PURE__*/React.createElement("button", {
    onClick: onNotif,
    style: topIconBtn
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "notifications",
    size: 21,
    color: T.fg2
  }), unread > 0 && /*#__PURE__*/React.createElement(Dot, null)));
}
function Dot() {
  return /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: 7,
      right: 8,
      width: 8,
      height: 8,
      borderRadius: '50%',
      background: T.alert,
      border: `1.5px solid ${T.bg2}`
    }
  });
}
const topIconBtn = {
  width: 40,
  height: 40,
  borderRadius: 12,
  flexShrink: 0,
  position: 'relative',
  background: 'transparent',
  border: `1px solid ${T.line}`,
  cursor: 'pointer',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  WebkitTapHighlightColor: 'transparent'
};
const iconBtn = topIconBtn;
window.iconBtn = iconBtn;

// Bottom nav — 5 tabs. Center "Publicar" is an elevated gradient action.
function BottomNav({
  active,
  onNav
}) {
  const tabs = [{
    id: 'feed',
    icon: 'eco',
    label: 'Feed'
  }, {
    id: 'explore',
    icon: 'explore',
    label: 'Explorar'
  }, {
    id: 'post',
    icon: 'add',
    label: 'Publicar',
    center: true
  }, {
    id: 'services',
    icon: 'grid_view',
    label: 'Serviços'
  }, {
    id: 'profile',
    icon: 'person',
    label: 'Perfil'
  }];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'flex-end',
      justifyContent: 'space-around',
      padding: '10px 12px 30px',
      position: 'relative',
      background: 'linear-gradient(to top, #0B0C0A 64%, rgba(11,12,10,0))',
      borderTop: `1px solid ${T.line}`
    }
  }, tabs.map(t => {
    const on = active === t.id;
    if (t.center) {
      return /*#__PURE__*/React.createElement("button", {
        key: t.id,
        onClick: () => onNav(t.id),
        style: navBtn
      }, /*#__PURE__*/React.createElement("div", {
        style: {
          width: 54,
          height: 40,
          borderRadius: 16,
          background: T.greenGrad,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          boxShadow: T.greenGlow
        }
      }, /*#__PURE__*/React.createElement(Icon, {
        name: "add",
        size: 26,
        color: "#0C1B10",
        weight: 500
      })), /*#__PURE__*/React.createElement("span", {
        style: navLabel(false)
      }, t.label));
    }
    return /*#__PURE__*/React.createElement("button", {
      key: t.id,
      onClick: () => onNav(t.id),
      style: {
        ...navBtn,
        flex: 1
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: t.icon,
      size: 25,
      color: on ? T.green : T.fg3,
      fill: on ? 1 : 0,
      weight: on ? 500 : 400
    }), /*#__PURE__*/React.createElement("span", {
      style: navLabel(on)
    }, t.label));
  }));
}
const navBtn = {
  background: 'none',
  border: 'none',
  cursor: 'pointer',
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  gap: 5,
  WebkitTapHighlightColor: 'transparent'
};
const navLabel = on => ({
  fontFamily: T.sans,
  fontSize: 10.5,
  fontWeight: on ? 600 : 500,
  color: on ? T.green : T.fg3,
  letterSpacing: 0.1
});
Object.assign(window, {
  TopBar,
  BottomNav
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/chrome.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/components.jsx
try { (() => {
// components.jsx — Arah shared UI primitives. Premium minimalist, dark-default.
// v2: depth via subtle gradients, real araponga logo, richer surfaces.
// Material Symbols Rounded icons (the app's native icon system is Material Icons).

const T = {
  bg: '#0B0C0A',
  // deepest base
  bg2: '#0E100D',
  // app surface
  card: '#16181400',
  // (placeholder)
  // gradient surfaces (depth)
  cardGrad: 'linear-gradient(160deg, #1C201A 0%, #161814 100%)',
  cardHiGrad: 'linear-gradient(160deg, #242821 0%, #1C201A 100%)',
  cardFlat: '#191B17',
  cardHi: '#20231E',
  glassGrad: 'linear-gradient(160deg, rgba(32,36,30,0.92), rgba(20,23,18,0.88))',
  line: 'rgba(255,255,255,0.07)',
  lineHi: 'rgba(255,255,255,0.13)',
  fg: '#F2F4EE',
  fg2: '#A6AC9E',
  // muted
  fg3: '#6B7164',
  // subtle
  green: '#A6D6B9',
  // canopy accent (light, for dark UI)
  greenSolid: '#81C784',
  greenGrad: 'linear-gradient(150deg, #93D29C 0%, #6FB98C 100%)',
  // primary button depth
  greenDeep: '#2B6246',
  greenDim: 'rgba(129,199,132,0.13)',
  greenGlow: '0 8px 24px rgba(129,199,132,0.30)',
  // elevated depth shadows (modern, layered)
  shadowSoft: '0 1px 0 rgba(255,255,255,0.05) inset, 0 8px 22px rgba(0,0,0,0.30)',
  shadowCard: '0 1px 0 rgba(255,255,255,0.05) inset, 0 14px 34px rgba(0,0,0,0.40)',
  shadowFloat: '0 18px 44px rgba(0,0,0,0.50)',
  alert: '#E8A06A',
  // warm terracotta-amber
  alertDim: 'rgba(232,160,106,0.14)',
  blue: '#86AEEA',
  blueDim: 'rgba(134,174,234,0.14)',
  water: '#6FC5D6',
  // território health: água
  display: '"Sora", system-ui, sans-serif',
  sans: '"Geist", system-ui, sans-serif'
};
window.T = T;

// Material Symbols Rounded icon
function Icon({
  name,
  size = 24,
  color = 'currentColor',
  fill = 0,
  weight = 400,
  grade = 0,
  style = {}
}) {
  return /*#__PURE__*/React.createElement("span", {
    className: "msr",
    style: {
      fontSize: size,
      color,
      lineHeight: 1,
      userSelect: 'none',
      fontVariationSettings: `'FILL' ${fill}, 'wght' ${weight}, 'GRAD' ${grade}, 'opsz' 24`,
      display: 'inline-flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0,
      ...style
    }
  }, name);
}

// Premium brand mark — leaf-wing fusion (araponga in flight + território vivo).
// tone: 'canopy' (gradient) | 'ink' | 'white' (flat, for mono use).
let _logoSeq = 0;
function LogoMark({
  size = 32,
  variant = 'wingleaf',
  tone = 'canopy'
}) {
  const uid = React.useMemo(() => 'lg' + ++_logoSeq, []);
  const flat = tone === 'ink' ? '#0E1F12' : tone === 'white' ? '#F2F4EE' : null;
  const light = flat || '#A6D6B9';
  const deep = flat || '#5FB07F';
  const solid = flat || '#81C784';
  const bodyFill = flat || `url(#${uid}b)`;
  const barbCol = tone === 'ink' ? '#A6D6B9' : tone === 'white' ? '#1A2D20' : '#0E2117';
  const paths = {
    // leaf-wing fusion — a tilted leaf whose central vein sprouts feather barbs
    wingleaf: /*#__PURE__*/React.createElement(React.Fragment, null, !flat && /*#__PURE__*/React.createElement("defs", null, /*#__PURE__*/React.createElement("linearGradient", {
      id: uid + 'b',
      x1: "0.1",
      y1: "0.05",
      x2: "0.85",
      y2: "0.95"
    }, /*#__PURE__*/React.createElement("stop", {
      offset: "0",
      stopColor: "#D2EFDD"
    }), /*#__PURE__*/React.createElement("stop", {
      offset: "0.5",
      stopColor: "#93D2A8"
    }), /*#__PURE__*/React.createElement("stop", {
      offset: "1",
      stopColor: "#56A877"
    }))), /*#__PURE__*/React.createElement("path", {
      d: "M11.5 40.5 C6.5 26 16 11.5 40.5 7.5 C33 23 26 35.5 11.5 40.5 Z",
      fill: deep,
      opacity: flat ? 0.45 : 0.9
    }), /*#__PURE__*/React.createElement("path", {
      d: "M14.5 37.8 C12.4 27 19 16.3 35.4 11.2 C29.6 23 22.6 32.6 14.5 37.8 Z",
      fill: bodyFill
    }), !flat && /*#__PURE__*/React.createElement("path", {
      d: "M35.4 11.2 C24.5 14.5 17.8 21.5 14.9 30.5",
      stroke: "rgba(255,255,255,0.55)",
      strokeWidth: "1.1",
      strokeLinecap: "round",
      fill: "none",
      opacity: "0.7"
    }), /*#__PURE__*/React.createElement("g", {
      stroke: barbCol,
      strokeWidth: "1.5",
      strokeLinecap: "round",
      opacity: flat ? 0.5 : 0.42,
      fill: "none"
    }, /*#__PURE__*/React.createElement("path", {
      d: "M15.4 36.6 L33.2 13.4"
    }), /*#__PURE__*/React.createElement("path", {
      d: "M19.4 31.4 L27.6 28.6"
    }), /*#__PURE__*/React.createElement("path", {
      d: "M22.4 26.6 L30.4 23.8"
    }), /*#__PURE__*/React.createElement("path", {
      d: "M25.4 21.6 L32.4 18.8"
    }))),
    // two leaf-wings meeting at the base — reads as flight and canopy growth
    wing: /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement("path", {
      d: "M24 43 C12 35 7 21 13 8 C23 15 28 30 24 43 Z",
      fill: deep
    }), /*#__PURE__*/React.createElement("path", {
      d: "M24 43 C36 35 41 21 35 8 C25 15 20 30 24 43 Z",
      fill: light
    })),
    leaf: /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement("path", {
      d: "M24 5 C37 15 37 33 24 44 C11 33 11 15 24 5 Z",
      fill: light
    }), /*#__PURE__*/React.createElement("path", {
      d: "M24 11 L24 39",
      stroke: tone === 'ink' ? '#0E1F12' : '#13241A',
      strokeWidth: "2",
      strokeLinecap: "round",
      opacity: "0.5"
    })),
    pin: /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement("path", {
      d: "M24 4 C15 4 8 11 8 20 C8 31 24 44 24 44 C24 44 40 31 40 20 C40 11 33 4 24 4 Z",
      fill: solid
    }), /*#__PURE__*/React.createElement("path", {
      d: "M24 12 C30 16 30 25 24 30 C18 25 18 16 24 12 Z",
      fill: tone === 'ink' ? '#A6D6B9' : '#0E1F12'
    }))
  };
  return /*#__PURE__*/React.createElement("svg", {
    width: size,
    height: size,
    viewBox: "0 0 48 48",
    fill: "none",
    style: {
      display: 'block'
    },
    "aria-label": "Arah"
  }, paths[variant] || paths.wing);
}

// Brand logo. variant: 'mark' | 'lockup'. mark: 'wing' | 'leaf' | 'pin' | 'png'.
function Logo({
  size = 32,
  lockup = false,
  color = T.fg,
  mark = 'png'
}) {
  const glyph = mark === 'png' ? /*#__PURE__*/React.createElement("img", {
    src: "../../assets/arah-logo.png",
    alt: "Arah",
    style: {
      height: size,
      width: 'auto',
      display: 'block'
    }
  }) : /*#__PURE__*/React.createElement(LogoMark, {
    size: size,
    variant: mark
  });
  if (!lockup) return glyph;
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10
    }
  }, glyph, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: size * 0.7,
      color,
      letterSpacing: -0.6
    }
  }, "Arah"));
}

// Circular avatar with initials
function Avatar({
  color = T.greenDeep,
  name = '',
  size = 40,
  ring = false,
  resident = false
}) {
  const initials = name.split(/[ .]/).filter(Boolean).slice(0, 2).map(w => w[0]?.toUpperCase()).join('');
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: size,
      height: size,
      borderRadius: '50%',
      background: `linear-gradient(150deg, ${color}, ${color}bb)`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      color: '#fff',
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: size * 0.36,
      boxShadow: ring ? `0 0 0 2px ${T.bg2}, 0 0 0 3.5px ${T.green}` : 'inset 0 1px 0 rgba(255,255,255,0.15)',
      letterSpacing: 0.2
    }
  }, initials), resident && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      bottom: -1,
      right: -1,
      width: size * 0.4,
      height: size * 0.4,
      borderRadius: '50%',
      background: T.greenSolid,
      border: `2px solid ${T.bg2}`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "verified",
    size: size * 0.26,
    color: "#0E1F12",
    fill: 1
  })));
}

// Role pill: morador vs visitante
function RoleBadge({
  role,
  size = 'sm'
}) {
  const resident = role === 'morador';
  const fs = size === 'sm' ? 10.5 : 12;
  return /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4,
      padding: size === 'sm' ? '2px 7px' : '4px 10px',
      borderRadius: 999,
      fontFamily: T.sans,
      fontWeight: 600,
      fontSize: fs,
      letterSpacing: 0.2,
      background: resident ? T.greenDim : 'rgba(166,172,158,0.12)',
      color: resident ? T.green : T.fg2
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: resident ? 'cottage' : 'explore',
    size: fs + 2,
    fill: resident ? 1 : 0
  }), resident ? 'Morador' : 'Visitante');
}

// Primary / secondary / ghost button
function Btn({
  children,
  kind = 'primary',
  icon,
  iconEnd,
  onClick,
  full = false,
  size = 'md',
  style = {}
}) {
  const pad = size === 'lg' ? '15px 22px' : size === 'sm' ? '8px 14px' : '12px 18px';
  const fs = size === 'lg' ? 16 : size === 'sm' ? 13.5 : 15;
  const kinds = {
    primary: {
      background: T.greenGrad,
      color: '#0C1B10',
      border: 'none',
      boxShadow: T.greenGlow
    },
    secondary: {
      background: 'transparent',
      color: T.fg,
      border: `1px solid ${T.lineHi}`
    },
    ghost: {
      background: 'transparent',
      color: T.green,
      border: 'none'
    },
    dark: {
      background: T.cardFlat,
      color: T.fg,
      border: `1px solid ${T.line}`
    }
  };
  return /*#__PURE__*/React.createElement("button", {
    onClick: onClick,
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 8,
      padding: pad,
      borderRadius: 14,
      fontFamily: T.sans,
      fontWeight: 600,
      fontSize: fs,
      cursor: 'pointer',
      width: full ? '100%' : undefined,
      letterSpacing: -0.1,
      transition: 'transform .12s, filter .18s',
      WebkitTapHighlightColor: 'transparent',
      ...kinds[kind],
      ...style
    },
    onMouseDown: e => e.currentTarget.style.transform = 'scale(0.975)',
    onMouseUp: e => e.currentTarget.style.transform = 'scale(1)',
    onMouseLeave: e => e.currentTarget.style.transform = 'scale(1)'
  }, icon && /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: fs + 4,
    fill: kind === 'primary' ? 1 : 0,
    weight: 500
  }), children, iconEnd && /*#__PURE__*/React.createElement(Icon, {
    name: iconEnd,
    size: fs + 4,
    fill: kind === 'primary' ? 1 : 0,
    weight: 500
  }));
}

// Pill chip
function Chip({
  children,
  active = false,
  onClick,
  icon,
  tone = 'green'
}) {
  const toneColor = tone === 'alert' ? T.alert : T.green;
  const toneDim = tone === 'alert' ? T.alertDim : T.greenDim;
  return /*#__PURE__*/React.createElement("button", {
    onClick: onClick,
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 6,
      flexShrink: 0,
      padding: '7px 13px',
      borderRadius: 999,
      cursor: 'pointer',
      fontFamily: T.sans,
      fontWeight: 500,
      fontSize: 13,
      letterSpacing: -0.1,
      background: active ? toneDim : 'transparent',
      color: active ? toneColor : T.fg2,
      border: `1px solid ${active ? 'transparent' : T.line}`,
      transition: 'all .15s',
      WebkitTapHighlightColor: 'transparent'
    }
  }, icon && /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 15,
    fill: active ? 1 : 0
  }), children);
}
function Eyebrow({
  children,
  color = T.green,
  style = {}
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11,
      fontWeight: 600,
      letterSpacing: 1.4,
      textTransform: 'uppercase',
      color,
      ...style
    }
  }, children);
}

// Card surface with depth gradient + layered elevation
function Card({
  children,
  style = {},
  onClick,
  hi = false,
  glow = false
}) {
  return /*#__PURE__*/React.createElement("div", {
    onClick: onClick,
    style: {
      background: hi ? T.cardHiGrad : T.cardGrad,
      border: `1px solid ${T.line}`,
      borderRadius: 20,
      boxShadow: glow ? T.shadowCard : T.shadowSoft,
      cursor: onClick ? 'pointer' : undefined,
      WebkitTapHighlightColor: 'transparent',
      ...style
    }
  }, children);
}
Object.assign(window, {
  Icon,
  Logo,
  LogoMark,
  Avatar,
  RoleBadge,
  Btn,
  Chip,
  Eyebrow,
  Card
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/components.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/data.jsx
try { (() => {
// data.jsx — Arah mock content. Território-primeiro, comunidade-primeiro. pt-BR.
// v3: content scoped PER TERRITORY (feed, events, map entities, health, members),
//     comments, photo posts, marketplace, compra coletiva, chat, store, roles.

const ARAH = {
  me: {
    name: 'Ana Ribeiro',
    handle: 'ana.ribeiro',
    avatar: '#4F956F',
    role: 'visitante'
  },
  territories: [{
    id: 't1',
    name: 'Camburi',
    region: 'São Sebastião · SP',
    members: 1284,
    distance: 0.4,
    desc: 'Vila caiçara entre a mata e o mar.',
    active: true
  }, {
    id: 't2',
    name: 'Maresias',
    region: 'São Sebastião · SP',
    members: 2106,
    distance: 6.2,
    desc: 'Praia, surfe e comércio local.'
  }, {
    id: 't3',
    name: 'Boiçucanga',
    region: 'São Sebastião · SP',
    members: 1740,
    distance: 9.1,
    desc: 'Comunidade ativa e feira de domingo.'
  }, {
    id: 't4',
    name: 'Juquehy',
    region: 'São Sebastião · SP',
    members: 980,
    distance: 12.7,
    desc: 'Rio, mangue e trilhas.'
  }, {
    id: 't5',
    name: 'Barra do Una',
    region: 'São Sebastião · SP',
    members: 612,
    distance: 18.3,
    desc: 'Reserva e pesca artesanal.'
  }],
  // ---- Per-territory content ----
  content: {
    t1: {
      tile: {
        x: 3057,
        y: 4652
      },
      health: {
        agua: 92,
        nativas: 78,
        nascentes: 6,
        mirantes: 4,
        santuarios: 2
      },
      feed: [{
        id: 'p1',
        author: 'Conselho de Moradores',
        handle: 'conselho.camburi',
        avatar: '#4F956F',
        role: 'morador',
        time: '2h',
        type: 'Alert',
        visibility: 'Public',
        pinned: true,
        title: 'Mutirão de limpeza no costão neste sábado',
        body: 'Encontro às 8h na praça da capela. Traga luvas e água. Vamos cuidar do nosso território juntos — café da manhã comunitário ao final.',
        likes: 64,
        comments: 12
      }, {
        id: 'p2',
        author: 'Dona Marta',
        handle: 'marta.peixaria',
        avatar: '#C9962B',
        role: 'morador',
        time: '5h',
        type: 'General',
        visibility: 'Public',
        photo: '../../assets/cover-rainforest.png',
        title: 'Chegou robalo fresco na peixaria',
        body: 'Pescado da manhã pelo seu Tião. Quem quiser, é só passar até as 18h. Apoiem a pesca artesanal da vila 🐟',
        likes: 38,
        comments: 7
      }, {
        id: 'p3',
        author: 'Coletivo Maré',
        handle: 'coletivo.mare',
        avatar: '#2A6FDB',
        role: 'morador',
        time: '1d',
        type: 'General',
        visibility: 'ResidentsOnly',
        title: 'Roda de conversa: água e saneamento',
        body: 'Próxima terça, 19h, no centro comunitário. Vamos discutir a captação da nascente e os próximos passos com a associação.',
        likes: 91,
        comments: 28
      }, {
        id: 'p4',
        author: 'Bruno Caiçara',
        handle: 'bruno.trilhas',
        avatar: '#377B57',
        role: 'visitante',
        time: '2d',
        type: 'General',
        visibility: 'Public',
        title: 'Trilha da Cachoeira reaberta',
        body: 'Galho caído foi removido pelo mutirão. Caminho liberado, mas atenção no trecho de pedra após a chuva.',
        likes: 47,
        comments: 5
      }],
      events: [{
        id: 'e1',
        day: '07',
        mon: 'JUN',
        title: 'Mutirão de limpeza do costão',
        time: 'Sáb · 08:00',
        place: 'Praça da Capela',
        going: 42,
        tag: 'Comunidade',
        status: 'interested',
        desc: 'Limpeza coletiva do costão e da faixa de areia. Traga luvas e garrafa de água.'
      }, {
        id: 'e2',
        day: '10',
        mon: 'JUN',
        title: 'Roda de conversa: água e saneamento',
        time: 'Ter · 19:00',
        place: 'Centro Comunitário',
        going: 28,
        tag: 'Assembleia',
        status: 'confirmed',
        desc: 'Discussão aberta sobre a captação da nascente e o saneamento da vila.'
      }, {
        id: 'e3',
        day: '15',
        mon: 'JUN',
        title: 'Feira de produtores da vila',
        time: 'Dom · 09:00',
        place: 'Largo da Igreja',
        going: 130,
        tag: 'Feira',
        desc: 'Produtores locais, agroecologia e artesanato caiçara.'
      }],
      entities: [{
        id: 'm_peixaria',
        x: 38,
        y: 38,
        kind: 'place',
        icon: 'storefront',
        color: '#A6D6B9',
        label: 'Peixaria da Marta',
        sub: 'Comércio local · aberto',
        confirmed: true
      }, {
        id: 'm_evento',
        x: 60,
        y: 30,
        kind: 'event',
        icon: 'event',
        color: '#86AEEA',
        label: 'Mutirão do costão',
        sub: 'Sáb · 08:00',
        confirmed: true
      }, {
        id: 'm_alerta',
        x: 30,
        y: 62,
        kind: 'alert',
        icon: 'warning',
        color: '#E8A06A',
        label: 'Trecho escorregadio',
        sub: 'Trilha · após a chuva',
        confirmed: true
      }, {
        id: 'm_cachoeira',
        x: 70,
        y: 60,
        kind: 'waterfall',
        icon: 'water_drop',
        color: '#6FC5D6',
        label: 'Cachoeira do Bracuí',
        sub: 'Banho · trilha 40min',
        confirmed: true
      }, {
        id: 'm_nascente',
        x: 52,
        y: 72,
        kind: 'spring',
        icon: 'water',
        color: '#6FC5D6',
        label: 'Nascente do Bracuí',
        sub: 'Água potável · confirmada',
        confirmed: true
      }, {
        id: 'm_mirante',
        x: 46,
        y: 24,
        kind: 'viewpoint',
        icon: 'landscape',
        color: '#A6D6B9',
        label: 'Mirante da Pedra',
        sub: 'Vista 360° · trilha 20min',
        confirmed: false
      }]
    },
    t2: {
      tile: {
        x: 3059,
        y: 4653
      },
      health: {
        agua: 74,
        nativas: 52,
        nascentes: 3,
        mirantes: 5,
        santuarios: 1
      },
      feed: [{
        id: 'm2p1',
        author: 'Escola de Surfe Maresias',
        handle: 'surf.maresias',
        avatar: '#2A6FDB',
        role: 'morador',
        time: '1h',
        type: 'General',
        visibility: 'Public',
        photo: '../../assets/cover-rainforest.png',
        title: 'Aula aberta de surfe neste domingo',
        body: 'Aula gratuita pra moradores às 7h, antes do movimento. Pranchas emprestadas. Bora pegar onda com responsabilidade 🌊',
        likes: 112,
        comments: 19
      }, {
        id: 'm2p2',
        author: 'Defesa Civil',
        handle: 'defesa.maresias',
        avatar: '#E8A06A',
        role: 'morador',
        time: '3h',
        type: 'Alert',
        visibility: 'Public',
        pinned: true,
        title: 'Trânsito intenso na entrada da praia',
        body: 'Feriado prolongado lotou a Rio-Santos. Evitem a rua da praia entre 10h e 14h. Usem o estacionamento do largo.',
        likes: 58,
        comments: 8
      }, {
        id: 'm2p3',
        author: 'Restaurante da Praia',
        handle: 'restaurante.maresias',
        avatar: '#C9962B',
        role: 'morador',
        time: '6h',
        type: 'General',
        visibility: 'Public',
        title: 'Menu caiçara de inverno',
        body: 'Caldo de peixe, moqueca e pastel de camarão. Moradores têm 15% no almoço durante a semana 🍲',
        likes: 44,
        comments: 6
      }, {
        id: 'm2p4',
        author: 'Coletivo Praia Limpa',
        handle: 'praialimpa',
        avatar: '#377B57',
        role: 'morador',
        time: '1d',
        type: 'General',
        visibility: 'ResidentsOnly',
        title: 'Coleta de guimbas na areia',
        body: 'Sábado de manhã. Vamos mapear os pontos mais sujos e instalar cinzeiros ecológicos na orla.',
        likes: 73,
        comments: 14
      }],
      events: [{
        id: 'm2e1',
        day: '08',
        mon: 'JUN',
        title: 'Campeonato de surfe amador',
        time: 'Dom · 07:00',
        place: 'Praia de Maresias',
        going: 214,
        tag: 'Esporte',
        desc: 'Etapa local do circuito caiçara. Categorias mirim, amador e master.'
      }, {
        id: 'm2e2',
        day: '14',
        mon: 'JUN',
        title: 'Feira gastronômica da orla',
        time: 'Sáb · 17:00',
        place: 'Calçadão da Praia',
        going: 156,
        tag: 'Feira',
        status: 'interested',
        desc: 'Food trucks, produtores e música ao vivo.'
      }],
      entities: [{
        id: 'm2_pico',
        x: 44,
        y: 42,
        kind: 'place',
        icon: 'surfing',
        color: '#86AEEA',
        label: 'Pico do Surfe',
        sub: 'Ondas · nível intermediário',
        confirmed: true
      }, {
        id: 'm2_quiosque',
        x: 62,
        y: 36,
        kind: 'place',
        icon: 'storefront',
        color: '#A6D6B9',
        label: 'Quiosque do Zé',
        sub: 'Comércio · aberto',
        confirmed: true
      }, {
        id: 'm2_alerta',
        x: 34,
        y: 58,
        kind: 'alert',
        icon: 'warning',
        color: '#E8A06A',
        label: 'Trânsito intenso',
        sub: 'Entrada da praia · 10h–14h',
        confirmed: true
      }, {
        id: 'm2_mirante',
        x: 56,
        y: 66,
        kind: 'viewpoint',
        icon: 'landscape',
        color: '#A6D6B9',
        label: 'Mirante do Pontal',
        sub: 'Pôr do sol · trilha 15min',
        confirmed: true
      }]
    },
    t3: {
      tile: {
        x: 3060,
        y: 4654
      },
      health: {
        agua: 81,
        nativas: 69,
        nascentes: 4,
        mirantes: 2,
        santuarios: 3
      },
      feed: [{
        id: 'm3p1',
        author: 'Associação de Boiçucanga',
        handle: 'assoc.boicucanga',
        avatar: '#4F956F',
        role: 'morador',
        time: '2h',
        type: 'General',
        visibility: 'Public',
        pinned: true,
        title: 'Feira de domingo confirmada',
        body: 'Das 8h às 13h no largo. Hortaliças, peixe, pães e artesanato. Tragam suas sacolas retornáveis 🧺',
        likes: 88,
        comments: 21
      }, {
        id: 'm3p2',
        author: 'Escola da Vila',
        handle: 'escola.boicucanga',
        avatar: '#C9962B',
        role: 'morador',
        time: '7h',
        type: 'General',
        visibility: 'Public',
        title: 'Aulas de reforço abertas',
        body: 'Voluntários dão reforço de matemática e leitura às quartas e sextas. Inscrições na secretaria.',
        likes: 52,
        comments: 9
      }, {
        id: 'm3p3',
        author: 'Defesa Civil',
        handle: 'defesa.boicucanga',
        avatar: '#E8A06A',
        role: 'morador',
        time: '1d',
        type: 'Alert',
        visibility: 'Public',
        title: 'Rio cheio após a chuva',
        body: 'Nível do rio subiu. Evitem a travessia na ponte velha até segunda. Usem a passarela nova.',
        likes: 41,
        comments: 4
      }],
      events: [{
        id: 'm3e1',
        day: '09',
        mon: 'JUN',
        title: 'Feira de domingo',
        time: 'Dom · 08:00',
        place: 'Largo da Vila',
        going: 190,
        tag: 'Feira',
        status: 'confirmed',
        desc: 'Feira semanal de produtores e artesãos de Boiçucanga.'
      }, {
        id: 'm3e2',
        day: '16',
        mon: 'JUN',
        title: 'Plantio de mudas nativas',
        time: 'Sáb · 09:00',
        place: 'Margem do Rio',
        going: 64,
        tag: 'Comunidade',
        desc: 'Recuperação da mata ciliar com mudas do viveiro comunitário.'
      }],
      entities: [{
        id: 'm3_feira',
        x: 48,
        y: 40,
        kind: 'event',
        icon: 'storefront',
        color: '#A6D6B9',
        label: 'Feira da Vila',
        sub: 'Dom · 08h–13h',
        confirmed: true
      }, {
        id: 'm3_igreja',
        x: 60,
        y: 50,
        kind: 'place',
        icon: 'church',
        color: '#A6D6B9',
        label: 'Igreja Matriz',
        sub: 'Centro histórico',
        confirmed: true
      }, {
        id: 'm3_rio',
        x: 36,
        y: 64,
        kind: 'alert',
        icon: 'warning',
        color: '#E8A06A',
        label: 'Ponte velha',
        sub: 'Rio cheio · evitar',
        confirmed: true
      }, {
        id: 'm3_cachoeira',
        x: 68,
        y: 62,
        kind: 'waterfall',
        icon: 'water_drop',
        color: '#6FC5D6',
        label: 'Cachoeira do Sertão',
        sub: 'Trilha 50min',
        confirmed: false
      }]
    }
  },
  // Fallback content for territories without bespoke data (t4, t5)
  fallbackContent: t => ({
    tile: {
      x: 3058,
      y: 4655
    },
    health: {
      agua: 70,
      nativas: 60,
      nascentes: 2,
      mirantes: 1,
      santuarios: 1
    },
    feed: [{
      id: t.id + 'fp1',
      author: 'Associação Local',
      handle: 'assoc.' + t.name.toLowerCase().replace(/\s/g, ''),
      avatar: '#4F956F',
      role: 'morador',
      time: '4h',
      type: 'General',
      visibility: 'Public',
      title: `Bem-vindo a ${t.name}`,
      body: `O território de ${t.name} ainda está se organizando na Arah. Seja um dos primeiros a publicar e fortalecer a comunidade local.`,
      likes: 12,
      comments: 2
    }, {
      id: t.id + 'fp2',
      author: 'Moradores de ' + t.name,
      handle: 'moradores',
      avatar: '#377B57',
      role: 'morador',
      time: '2d',
      type: 'General',
      visibility: 'Public',
      title: 'Grupo de WhatsApp migrando pra cá',
      body: 'Estamos trazendo os avisos da vila pro app. Em breve mais conteúdo do território por aqui.',
      likes: 8,
      comments: 1
    }],
    events: [{
      id: t.id + 'fe1',
      day: '21',
      mon: 'JUN',
      title: 'Encontro de moradores',
      time: 'Sáb · 16:00',
      place: 'Centro da Vila',
      going: 18,
      tag: 'Comunidade',
      desc: `Primeiro encontro de moradores de ${t.name} para organizar o território na Arah.`
    }],
    entities: [{
      id: t.id + '_centro',
      x: 50,
      y: 46,
      kind: 'place',
      icon: 'place',
      color: '#A6D6B9',
      label: 'Centro da Vila',
      sub: 'Ponto de encontro',
      confirmed: true
    }, {
      id: t.id + '_praia',
      x: 60,
      y: 60,
      kind: 'place',
      icon: 'beach_access',
      color: '#86AEEA',
      label: 'Praia',
      sub: 'Acesso público',
      confirmed: false
    }]
  }),
  getContent(tid) {
    if (this.content[tid]) return this.content[tid];
    return this.fallbackContent(this.territories.find(t => t.id === tid));
  },
  comments: {
    p1: [{
      id: 'c1',
      author: 'Bruno Caiçara',
      avatar: '#377B57',
      role: 'visitante',
      time: '1h',
      body: 'Vou levar dois sacos extras e luvas. Bora!'
    }, {
      id: 'c2',
      author: 'Dona Marta',
      avatar: '#C9962B',
      role: 'morador',
      time: '1h',
      body: 'Levo o café e os bolos da padaria 🙌'
    }, {
      id: 'c3',
      author: 'Seu Tião',
      avatar: '#4F956F',
      role: 'morador',
      time: '40min',
      body: 'Confirmado. Levo o caminhão pra recolher o lixo pesado.'
    }]
  },
  notifications: [{
    id: 'n1',
    kind: 'alert',
    read: false,
    link: 'post:p1',
    title: 'Novo alerta no seu território',
    body: 'Conselho de Moradores publicou um aviso sobre o mutirão de sábado.',
    time: 'há 2h'
  }, {
    id: 'n2',
    kind: 'event',
    read: false,
    link: 'events',
    title: 'Lembrete de evento',
    body: 'Roda de conversa sobre água começa em 1 dia.',
    time: 'há 5h'
  }, {
    id: 'n3',
    kind: 'map',
    read: false,
    link: 'map:m_nascente',
    title: 'Nova entidade no mapa',
    body: 'Nascente do Bracuí foi confirmada pela curadoria. Ver no mapa.',
    time: 'há 8h'
  }, {
    id: 'n4',
    kind: 'connection',
    read: true,
    link: 'profile',
    title: 'Bruno Caiçara entrou em Camburi',
    body: 'Agora faz parte do seu território.',
    time: 'ontem'
  }, {
    id: 'n5',
    kind: 'post',
    read: true,
    link: 'post:p4',
    title: 'Seu post recebeu 12 comentários',
    body: '“Trilha da Cachoeira reaberta” está movimentando a vila.',
    time: 'há 2 dias'
  }],
  market: [{
    id: 'mk1',
    title: 'Robalo fresco (kg)',
    seller: 'Peixaria da Marta',
    price: 'R$ 42',
    tag: 'Alimento',
    avatar: '#C9962B',
    icon: 'set_meal'
  }, {
    id: 'mk2',
    title: 'Passeio de canoa no mangue',
    seller: 'Bruno Caiçara',
    price: 'R$ 80',
    tag: 'Serviço',
    avatar: '#377B57',
    icon: 'kayaking'
  }, {
    id: 'mk3',
    title: 'Cestas de orgânicos da feira',
    seller: 'Coletivo Maré',
    price: 'R$ 55',
    tag: 'Alimento',
    avatar: '#2A6FDB',
    icon: 'shopping_basket'
  }, {
    id: 'mk4',
    title: 'Artesanato em fibra de bananeira',
    seller: 'Dona Lurdes',
    price: 'R$ 35',
    tag: 'Artesanato',
    avatar: '#4F956F',
    icon: 'redeem'
  }],
  groupBuys: [{
    id: 'gb1',
    title: 'Compra coletiva de placas solares',
    org: 'Conselho de Moradores',
    goal: 30,
    joined: 22,
    unit: 'famílias',
    deadline: 'até 20/jun',
    icon: 'solar_power'
  }, {
    id: 'gb2',
    title: 'Cisternas de captação de chuva',
    org: 'Coletivo Maré',
    goal: 15,
    joined: 9,
    unit: 'casas',
    deadline: 'até 30/jun',
    icon: 'water_drop'
  }],
  // My store (net-new): seller products + payment config
  myStore: {
    open: false,
    name: 'Ateliê da Ana',
    products: [{
      id: 'sp1',
      title: 'Aulas de reforço (hora)',
      price: 'R$ 40',
      tag: 'Serviço',
      icon: 'school',
      active: true
    }, {
      id: 'sp2',
      title: 'Doce de banana caseiro',
      price: 'R$ 18',
      tag: 'Alimento',
      icon: 'lunch_dining',
      active: true
    }],
    sales: {
      month: 'R$ 1.240',
      orders: 31
    },
    payment: {
      pix: 'ana.ribeiro@email.com',
      methods: ['PIX', 'Cartão', 'Dinheiro'],
      fee: '0%',
      payout: 'Banco do Brasil ····2207'
    }
  },
  chats: [{
    id: 'ch1',
    name: 'Conselho de Moradores',
    avatar: '#4F956F',
    role: 'morador',
    last: 'Confirmado o caminhão pro mutirão.',
    time: '10:24',
    unread: 2,
    thread: [{
      me: false,
      body: 'Oi Ana! Você confirma presença no mutirão de sábado?',
      time: '10:20'
    }, {
      me: true,
      body: 'Confirmo sim! Posso levar luvas extras.',
      time: '10:22'
    }, {
      me: false,
      body: 'Perfeito. Confirmado o caminhão pro mutirão.',
      time: '10:24'
    }]
  }, {
    id: 'ch2',
    name: 'Dona Marta',
    avatar: '#C9962B',
    role: 'morador',
    last: 'Separei o robalo, viu?',
    time: 'ontem',
    unread: 0,
    thread: [{
      me: true,
      body: 'Dona Marta, ainda tem robalo pra hoje?',
      time: 'ontem'
    }, {
      me: false,
      body: 'Separei o robalo, viu? Pode passar até as 18h.',
      time: 'ontem'
    }]
  }, {
    id: 'ch3',
    name: 'Coletivo Maré',
    avatar: '#2A6FDB',
    role: 'morador',
    last: 'Te vejo na roda de conversa 💧',
    time: 'seg',
    unread: 0,
    thread: [{
      me: false,
      body: 'Te vejo na roda de conversa 💧',
      time: 'seg'
    }]
  }],
  profile: {
    name: 'Ana Ribeiro',
    handle: 'ana.ribeiro',
    avatar: '#4F956F',
    bio: 'Caiçara, professora e voluntária no coletivo de águas. Território é casa.',
    posts: 23,
    territories: 2,
    since: '2024',
    interests: ['Meio ambiente', 'Pesca artesanal', 'Educação', 'Cultura caiçara', 'Saneamento']
  },
  interestPool: ['Meio ambiente', 'Pesca artesanal', 'Educação', 'Cultura caiçara', 'Saneamento', 'Turismo de base', 'Agroecologia', 'Saúde', 'Esporte', 'Artesanato', 'Música', 'Trilhas'],
  // ============================================================
  // BACKLOG DOMAINS — visual preview of the full roadmap (48 fases / 10 ondas)
  // status: 'live' (implemented MVP) | 'soon' (planned roadmap)
  // ============================================================
  serviceCategories: [{
    id: 'economia',
    label: 'Economia local',
    icon: 'payments',
    color: '#A6D6B9',
    desc: 'Mercado, compras coletivas e renda no território.',
    items: [{
      id: 'market',
      icon: 'storefront',
      label: 'Mercado',
      sub: 'Lojas, itens e checkout',
      status: 'live'
    }, {
      id: 'groupbuy',
      icon: 'groups_2',
      label: 'Compra coletiva',
      sub: 'Unir pedidos da vila',
      status: 'soon',
      phase: 'Fase 17'
    }, {
      id: 'hosting',
      icon: 'cottage',
      label: 'Hospedagem',
      sub: 'Estadias no território',
      status: 'soon',
      phase: 'Fase 18'
    }, {
      id: 'demands',
      icon: 'swap_horiz',
      label: 'Demandas & ofertas',
      sub: 'Quem precisa, quem oferece',
      status: 'soon',
      phase: 'Fase 19'
    }, {
      id: 'trades',
      icon: 'handshake',
      label: 'Trocas comunitárias',
      sub: 'Escambo e doações',
      status: 'soon',
      phase: 'Fase 20'
    }, {
      id: 'delivery',
      icon: 'local_shipping',
      label: 'Entregas',
      sub: 'Logística local',
      status: 'soon',
      phase: 'Fase 21'
    }, {
      id: 'wallet',
      icon: 'account_balance_wallet',
      label: 'Moeda territorial',
      sub: 'Carteira e créditos',
      status: 'soon',
      phase: 'Fase 22'
    }]
  }, {
    id: 'servicos',
    label: 'Serviços territoriais',
    icon: 'concierge',
    color: '#86AEEA',
    desc: 'Profissionais e espaços de confiança local.',
    items: [{
      id: 'sitters',
      icon: 'child_care',
      label: 'Babás',
      sub: 'Cuidado infantil verificado',
      status: 'soon',
      phase: 'Épico 10'
    }, {
      id: 'wellness',
      icon: 'self_improvement',
      label: 'Bem-estar',
      sub: 'Yoga, terapias e espaços',
      status: 'soon',
      phase: 'Épico 10'
    }, {
      id: 'rental',
      icon: 'category',
      label: 'Aluguéis',
      sub: 'Equipamentos e espaços',
      status: 'soon',
      phase: 'Fase 46'
    }, {
      id: 'digital',
      icon: 'apps',
      label: 'Hub digital',
      sub: 'Serviços digitais locais',
      status: 'soon',
      phase: 'Fase 26'
    }]
  }, {
    id: 'governanca',
    label: 'Governança',
    icon: 'how_to_vote',
    color: '#E8A06A',
    desc: 'Decisões coletivas e cuidado comunitário.',
    items: [{
      id: 'elections',
      icon: 'how_to_vote',
      label: 'Eleições & votação',
      sub: 'Conselho e consultas',
      status: 'live'
    }, {
      id: 'manage',
      icon: 'shield_person',
      label: 'Gestão & curadoria',
      sub: 'Moderação comunitária',
      status: 'live'
    }, {
      id: 'moderation',
      icon: 'gavel',
      label: 'Moderação & denúncias',
      sub: 'Reports, bloqueios, sanções',
      status: 'live'
    }, {
      id: 'subscriptions',
      icon: 'card_membership',
      label: 'Assinaturas',
      sub: 'Apoio recorrente ao território',
      status: 'soon',
      phase: 'Fase 15'
    }]
  }, {
    id: 'territorio',
    label: 'Vida no território',
    icon: 'eco',
    color: '#A6D6B9',
    desc: 'Saúde, cultura e regeneração do lugar.',
    items: [{
      id: 'health',
      icon: 'water_drop',
      label: 'Saúde do território',
      sub: 'Indicadores vivos',
      status: 'soon',
      phase: 'Fase 24'
    }, {
      id: 'metrics',
      icon: 'monitoring',
      label: 'Painel de métricas',
      sub: 'Pulso da comunidade',
      status: 'soon',
      phase: 'Fase 25'
    }, {
      id: 'seeds',
      icon: 'potted_plant',
      label: 'Banco de sementes',
      sub: 'Mudas e troca de sementes',
      status: 'soon',
      phase: 'Fase 48'
    }, {
      id: 'learning',
      icon: 'school',
      label: 'Aprendizado',
      sub: 'Saberes e oficinas locais',
      status: 'soon',
      phase: 'Fase 45'
    }, {
      id: 'ai',
      icon: 'auto_awesome',
      label: 'Assistente IA',
      sub: 'Apoio inteligente do território',
      status: 'soon',
      phase: 'Fase 27'
    }, {
      id: 'achievements',
      icon: 'workspace_premium',
      label: 'Conquistas',
      sub: 'Reconhecimento comunitário',
      status: 'soon',
      phase: 'Fase 42'
    }]
  }],
  groupBuyDetail: {
    title: 'Compra coletiva de placas solares',
    org: 'Conselho de Moradores',
    icon: 'solar_power',
    goal: 30,
    joined: 22,
    unit: 'famílias',
    deadline: 'até 20/jun',
    price: 'R$ 2.400',
    saved: 'R$ 1.100',
    desc: 'Quanto mais famílias entram, menor o preço por placa. Negociação coletiva direto com o fornecedor regional.',
    tiers: [{
      n: 10,
      price: 'R$ 3.500'
    }, {
      n: 20,
      price: 'R$ 2.400'
    }, {
      n: 30,
      price: 'R$ 1.900'
    }],
    participants: ['#C9962B', '#377B57', '#2A6FDB', '#4F956F', '#E8A06A']
  },
  hosting: [{
    id: 'h1',
    title: 'Casa caiçara à beira-mar',
    host: 'Dona Marta',
    avatar: '#C9962B',
    price: 'R$ 180',
    unit: 'noite',
    rating: 4.9,
    reviews: 32,
    tag: 'Casa inteira',
    guests: 4,
    icon: 'cottage'
  }, {
    id: 'h2',
    title: 'Quarto na vila, perto da trilha',
    host: 'Bruno Caiçara',
    avatar: '#377B57',
    price: 'R$ 90',
    unit: 'noite',
    rating: 4.8,
    reviews: 18,
    tag: 'Quarto privativo',
    guests: 2,
    icon: 'bedroom_parent'
  }, {
    id: 'h3',
    title: 'Camping ecológico do rio',
    host: 'Coletivo Maré',
    avatar: '#2A6FDB',
    price: 'R$ 45',
    unit: 'noite',
    rating: 4.7,
    reviews: 41,
    tag: 'Camping',
    guests: 6,
    icon: 'camping'
  }],
  demands: [{
    id: 'd1',
    kind: 'demanda',
    title: 'Preciso de pedreiro para muro',
    who: 'Seu Tião',
    avatar: '#4F956F',
    tag: 'Construção',
    time: '2h',
    icon: 'construction'
  }, {
    id: 'd2',
    kind: 'oferta',
    title: 'Ofereço aulas de violão',
    who: 'Bruno Caiçara',
    avatar: '#377B57',
    tag: 'Educação',
    time: '4h',
    icon: 'music_note'
  }, {
    id: 'd3',
    kind: 'demanda',
    title: 'Procuro carona para São Sebastião',
    who: 'Ana Ribeiro',
    avatar: '#4F956F',
    tag: 'Transporte',
    time: '6h',
    icon: 'directions_car'
  }, {
    id: 'd4',
    kind: 'oferta',
    title: 'Conserto de redes de pesca',
    who: 'Dona Marta',
    avatar: '#C9962B',
    tag: 'Serviço',
    time: '1d',
    icon: 'phishing'
  }],
  trades: [{
    id: 'tr1',
    title: 'Troco mudas de ipê por adubo',
    who: 'Coletivo Maré',
    avatar: '#2A6FDB',
    want: 'Adubo orgânico',
    icon: 'potted_plant'
  }, {
    id: 'tr2',
    title: 'Doação: roupas infantis',
    who: 'Dona Lurdes',
    avatar: '#4F956F',
    want: 'Doação',
    icon: 'volunteer_activism'
  }, {
    id: 'tr3',
    title: 'Troco peixe por hortaliças',
    who: 'Seu Tião',
    avatar: '#4F956F',
    want: 'Hortaliças',
    icon: 'set_meal'
  }],
  deliveries: [{
    id: 'dl1',
    from: 'Peixaria da Marta',
    to: 'Praça da Capela',
    status: 'a caminho',
    courier: 'Bruno',
    eta: '15 min',
    icon: 'two_wheeler'
  }, {
    id: 'dl2',
    from: 'Feira da Vila',
    to: 'Rua das Conchas, 12',
    status: 'entregue',
    courier: 'Ana',
    eta: 'concluído',
    icon: 'check_circle'
  }],
  wallet: {
    balance: 'A̧ 340',
    brl: 'R$ 340,00',
    name: 'Aratá',
    symbol: 'A̧',
    transactions: [{
      id: 'w1',
      label: 'Feira da Vila',
      sub: 'Compra de orgânicos',
      val: '- A̧ 55',
      icon: 'shopping_basket',
      neg: true
    }, {
      id: 'w2',
      label: 'Mutirão do costão',
      sub: 'Crédito por participação',
      val: '+ A̧ 30',
      icon: 'volunteer_activism',
      neg: false
    }, {
      id: 'w3',
      label: 'Aulas de violão',
      sub: 'Recebido de Marina',
      val: '+ A̧ 80',
      icon: 'music_note',
      neg: false
    }, {
      id: 'w4',
      label: 'Peixaria da Marta',
      sub: 'Robalo fresco',
      val: '- A̧ 42',
      icon: 'set_meal',
      neg: true
    }]
  },
  sitters: [{
    id: 's1',
    name: 'Marina Souza',
    avatar: '#E8A06A',
    rating: 4.9,
    reviews: 27,
    price: 'R$ 35/h',
    exp: '8 anos',
    verified: true,
    badges: ['Primeiros socorros', 'Verificada'],
    dist: '0.6 km',
    ages: '0–6 anos'
  }, {
    id: 's2',
    name: 'Júlia Caiçara',
    avatar: '#377B57',
    rating: 4.8,
    reviews: 14,
    price: 'R$ 30/h',
    exp: '5 anos',
    verified: true,
    badges: ['Curso de babá'],
    dist: '1.2 km',
    ages: '2–10 anos'
  }, {
    id: 's3',
    name: 'Rosa Maria',
    avatar: '#C9962B',
    rating: 5.0,
    reviews: 41,
    price: 'R$ 40/h',
    exp: '12 anos',
    verified: true,
    badges: ['Primeiros socorros', 'Pedagogia'],
    dist: '2.0 km',
    ages: '0–12 anos'
  }],
  wellness: {
    spaces: [{
      id: 'ws1',
      name: 'Espaço Maré Yoga',
      type: 'Estúdio de yoga',
      icon: 'self_improvement',
      avatar: '#A6D6B9',
      slots: '6 horários livres',
      cap: '12 pessoas'
    }, {
      id: 'ws2',
      name: 'Casa Holística da Vila',
      type: 'Terapias integrativas',
      icon: 'spa',
      avatar: '#86AEEA',
      slots: '3 horários livres',
      cap: '8 pessoas'
    }],
    providers: [{
      id: 'wp1',
      name: 'Clara Ventos',
      spec: 'Professora de Yoga',
      avatar: '#377B57',
      rating: 4.9,
      price: 'R$ 60',
      icon: 'self_improvement',
      verified: true
    }, {
      id: 'wp2',
      name: 'Tomás Lua',
      spec: 'Terapia sonora',
      avatar: '#2A6FDB',
      rating: 4.8,
      price: 'R$ 90',
      icon: 'music_note',
      verified: true
    }]
  },
  courses: [{
    id: 'co1',
    title: 'Pesca artesanal sustentável',
    teacher: 'Seu Tião',
    avatar: '#4F956F',
    lessons: 6,
    level: 'Iniciante',
    tag: 'Saberes locais',
    icon: 'phishing'
  }, {
    id: 'co2',
    title: 'Agroecologia no quintal',
    teacher: 'Coletivo Maré',
    avatar: '#2A6FDB',
    lessons: 8,
    level: 'Todos',
    tag: 'Agroecologia',
    icon: 'eco'
  }, {
    id: 'co3',
    title: 'Construção com bioconstrução',
    teacher: 'Bruno Caiçara',
    avatar: '#377B57',
    lessons: 5,
    level: 'Intermediário',
    tag: 'Ofícios',
    icon: 'handyman'
  }],
  seeds: [{
    id: 'se1',
    name: 'Ipê-amarelo',
    who: 'Coletivo Maré',
    qty: '20 sementes',
    tag: 'Nativa',
    icon: 'park'
  }, {
    id: 'se2',
    name: 'Manjericão',
    who: 'Dona Lurdes',
    qty: '15 mudas',
    tag: 'Tempero',
    icon: 'potted_plant'
  }, {
    id: 'se3',
    name: 'Juçara (palmito)',
    who: 'Seu Tião',
    qty: '8 mudas',
    tag: 'Nativa',
    icon: 'forest'
  }, {
    id: 'se4',
    name: 'Hortelã',
    who: 'Ana Ribeiro',
    qty: '12 mudas',
    tag: 'Medicinal',
    icon: 'eco'
  }],
  achievements: [{
    id: 'a1',
    label: 'Guardião do território',
    sub: '5 mutirões',
    icon: 'shield',
    got: true
  }, {
    id: 'a2',
    label: 'Semeador',
    sub: '10 mudas doadas',
    icon: 'potted_plant',
    got: true
  }, {
    id: 'a3',
    label: 'Voz ativa',
    sub: 'Votou em 3 eleições',
    icon: 'how_to_vote',
    got: true
  }, {
    id: 'a4',
    label: 'Vizinho presente',
    sub: '20 eventos',
    icon: 'groups',
    got: false,
    progress: 70
  }, {
    id: 'a5',
    label: 'Comerciante local',
    sub: 'Loja com 50 vendas',
    icon: 'storefront',
    got: false,
    progress: 62
  }, {
    id: 'a6',
    label: 'Mestre de ofício',
    sub: 'Ensinou 1 curso',
    icon: 'school',
    got: false,
    progress: 0
  }],
  subscriptionTiers: [{
    id: 'sub1',
    name: 'Apoiador',
    price: 'R$ 9',
    period: '/mês',
    perks: ['Selo de apoiador', 'Acesso a relatórios do território'],
    popular: false
  }, {
    id: 'sub2',
    name: 'Guardião',
    price: 'R$ 29',
    period: '/mês',
    perks: ['Tudo do Apoiador', 'Prioridade em compras coletivas', 'Voz nas consultas'],
    popular: true
  }, {
    id: 'sub3',
    name: 'Mantenedor',
    price: 'R$ 79',
    period: '/mês',
    perks: ['Tudo do Guardião', 'Reconhecimento público', 'Reunião mensal com o conselho'],
    popular: false
  }],
  aiPrompts: ['O que está acontecendo em Camburi hoje?', 'Onde encontro peixe fresco agora?', 'Resumir a última assembleia', 'Quais trilhas estão abertas?'],
  metrics: {
    engagement: 78,
    posts: 142,
    events: 12,
    newMembers: 34,
    bars: [{
      label: 'Seg',
      v: 40
    }, {
      label: 'Ter',
      v: 65
    }, {
      label: 'Qua',
      v: 52
    }, {
      label: 'Qui',
      v: 80
    }, {
      label: 'Sex',
      v: 72
    }, {
      label: 'Sáb',
      v: 95
    }, {
      label: 'Dom',
      v: 88
    }]
  },
  rentals: [{
    id: 'rt1',
    title: 'Caiaque duplo',
    owner: 'Bruno Caiçara',
    avatar: '#377B57',
    price: 'R$ 40',
    unit: 'dia',
    tag: 'Lazer',
    icon: 'kayaking'
  }, {
    id: 'rt2',
    title: 'Furadeira + kit',
    owner: 'Seu Tião',
    avatar: '#4F956F',
    price: 'R$ 25',
    unit: 'dia',
    tag: 'Ferramenta',
    icon: 'handyman'
  }, {
    id: 'rt3',
    title: 'Barraca 4 pessoas',
    owner: 'Coletivo Maré',
    avatar: '#2A6FDB',
    price: 'R$ 35',
    unit: 'dia',
    tag: 'Camping',
    icon: 'camping'
  }, {
    id: 'rt4',
    title: 'Som + microfone',
    owner: 'Dona Marta',
    avatar: '#C9962B',
    price: 'R$ 60',
    unit: 'dia',
    tag: 'Eventos',
    icon: 'speaker'
  }],
  digitalServices: [{
    id: 'ds1',
    title: 'Criação de logo e identidade',
    who: 'Marina Designer',
    avatar: '#E8A06A',
    price: 'R$ 250',
    tag: 'Design',
    icon: 'palette'
  }, {
    id: 'ds2',
    title: 'Site para comércio local',
    who: 'Bruno Dev',
    avatar: '#377B57',
    price: 'R$ 800',
    tag: 'Web',
    icon: 'language'
  }, {
    id: 'ds3',
    title: 'Gestão de redes sociais',
    who: 'Coletivo Maré',
    avatar: '#2A6FDB',
    price: 'R$ 180/mês',
    tag: 'Marketing',
    icon: 'campaign'
  }, {
    id: 'ds4',
    title: 'Edição de vídeos',
    who: 'Júlia Filma',
    avatar: '#C9962B',
    price: 'R$ 120',
    tag: 'Vídeo',
    icon: 'movie'
  }]
};
window.ARAH = ARAH;
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/data.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/ios-frame.jsx
try { (() => {
// @ds-adherence-ignore -- omelette starter scaffold (raw elements/hex/px by design)

/* BEGIN USAGE */
// iOS.jsx — Simplified iOS 26 (Liquid Glass) device frame
// Based on the iOS 26 UI Kit + Figma status bar spec. No assets, no deps.
// Exports (to window): IOSDevice, IOSStatusBar, IOSNavBar, IOSGlassPill, IOSList, IOSListRow, IOSKeyboard
//
// Usage — wrap your screen content in <IOSDevice> to get the bezel, status bar
// and home indicator (props: title, dark, keyboard):
//
//   <IOSDevice title="Settings">
//     ...your screen content...
//   </IOSDevice>
//   <IOSDevice dark title="Search" keyboard>…</IOSDevice>
/* END USAGE */

// ─────────────────────────────────────────────────────────────
// Status bar
// ─────────────────────────────────────────────────────────────
function IOSStatusBar({
  dark = false,
  time = '9:41'
}) {
  const c = dark ? '#fff' : '#000';
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 154,
      alignItems: 'center',
      justifyContent: 'center',
      padding: '21px 24px 19px',
      boxSizing: 'border-box',
      position: 'relative',
      zIndex: 20,
      width: '100%'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      height: 22,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      paddingTop: 1.5
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: '-apple-system, "SF Pro", system-ui',
      fontWeight: 590,
      fontSize: 17,
      lineHeight: '22px',
      color: c
    }
  }, time)), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      height: 22,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 7,
      paddingTop: 1,
      paddingRight: 1
    }
  }, /*#__PURE__*/React.createElement("svg", {
    width: "19",
    height: "12",
    viewBox: "0 0 19 12"
  }, /*#__PURE__*/React.createElement("rect", {
    x: "0",
    y: "7.5",
    width: "3.2",
    height: "4.5",
    rx: "0.7",
    fill: c
  }), /*#__PURE__*/React.createElement("rect", {
    x: "4.8",
    y: "5",
    width: "3.2",
    height: "7",
    rx: "0.7",
    fill: c
  }), /*#__PURE__*/React.createElement("rect", {
    x: "9.6",
    y: "2.5",
    width: "3.2",
    height: "9.5",
    rx: "0.7",
    fill: c
  }), /*#__PURE__*/React.createElement("rect", {
    x: "14.4",
    y: "0",
    width: "3.2",
    height: "12",
    rx: "0.7",
    fill: c
  })), /*#__PURE__*/React.createElement("svg", {
    width: "17",
    height: "12",
    viewBox: "0 0 17 12"
  }, /*#__PURE__*/React.createElement("path", {
    d: "M8.5 3.2C10.8 3.2 12.9 4.1 14.4 5.6L15.5 4.5C13.7 2.7 11.2 1.5 8.5 1.5C5.8 1.5 3.3 2.7 1.5 4.5L2.6 5.6C4.1 4.1 6.2 3.2 8.5 3.2Z",
    fill: c
  }), /*#__PURE__*/React.createElement("path", {
    d: "M8.5 6.8C9.9 6.8 11.1 7.3 12 8.2L13.1 7.1C11.8 5.9 10.2 5.1 8.5 5.1C6.8 5.1 5.2 5.9 3.9 7.1L5 8.2C5.9 7.3 7.1 6.8 8.5 6.8Z",
    fill: c
  }), /*#__PURE__*/React.createElement("circle", {
    cx: "8.5",
    cy: "10.5",
    r: "1.5",
    fill: c
  })), /*#__PURE__*/React.createElement("svg", {
    width: "27",
    height: "13",
    viewBox: "0 0 27 13"
  }, /*#__PURE__*/React.createElement("rect", {
    x: "0.5",
    y: "0.5",
    width: "23",
    height: "12",
    rx: "3.5",
    stroke: c,
    strokeOpacity: "0.35",
    fill: "none"
  }), /*#__PURE__*/React.createElement("rect", {
    x: "2",
    y: "2",
    width: "20",
    height: "9",
    rx: "2",
    fill: c
  }), /*#__PURE__*/React.createElement("path", {
    d: "M25 4.5V8.5C25.8 8.2 26.5 7.2 26.5 6.5C26.5 5.8 25.8 4.8 25 4.5Z",
    fill: c,
    fillOpacity: "0.4"
  }))));
}

// ─────────────────────────────────────────────────────────────
// Liquid glass pill — blur + tint + shine
// ─────────────────────────────────────────────────────────────
function IOSGlassPill({
  children,
  dark = false,
  style = {}
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      height: 44,
      minWidth: 44,
      borderRadius: 9999,
      position: 'relative',
      overflow: 'hidden',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      boxShadow: dark ? '0 2px 6px rgba(0,0,0,0.35), 0 6px 16px rgba(0,0,0,0.2)' : '0 1px 3px rgba(0,0,0,0.07), 0 3px 10px rgba(0,0,0,0.06)',
      ...style
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      borderRadius: 9999,
      backdropFilter: 'blur(12px) saturate(180%)',
      WebkitBackdropFilter: 'blur(12px) saturate(180%)',
      background: dark ? 'rgba(120,120,128,0.28)' : 'rgba(255,255,255,0.5)'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      borderRadius: 9999,
      boxShadow: dark ? 'inset 1.5px 1.5px 1px rgba(255,255,255,0.15), inset -1px -1px 1px rgba(255,255,255,0.08)' : 'inset 1.5px 1.5px 1px rgba(255,255,255,0.7), inset -1px -1px 1px rgba(255,255,255,0.4)',
      border: dark ? '0.5px solid rgba(255,255,255,0.15)' : '0.5px solid rgba(0,0,0,0.06)'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      zIndex: 1,
      display: 'flex',
      alignItems: 'center',
      padding: '0 4px'
    }
  }, children));
}

// ─────────────────────────────────────────────────────────────
// Navigation bar — glass pills + large title
// ─────────────────────────────────────────────────────────────
function IOSNavBar({
  title = 'Title',
  dark = false,
  trailingIcon = true
}) {
  const muted = dark ? 'rgba(255,255,255,0.6)' : '#404040';
  const text = dark ? '#fff' : '#000';
  const pillIcon = content => /*#__PURE__*/React.createElement(IOSGlassPill, {
    dark: dark
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 36,
      height: 36,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, content));
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10,
      paddingTop: 62,
      paddingBottom: 10,
      position: 'relative',
      zIndex: 5
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      padding: '0 16px'
    }
  }, pillIcon(/*#__PURE__*/React.createElement("svg", {
    width: "12",
    height: "20",
    viewBox: "0 0 12 20",
    fill: "none",
    style: {
      marginLeft: -1
    }
  }, /*#__PURE__*/React.createElement("path", {
    d: "M10 2L2 10l8 8",
    stroke: muted,
    strokeWidth: "2.5",
    strokeLinecap: "round",
    strokeLinejoin: "round"
  }))), trailingIcon && pillIcon(/*#__PURE__*/React.createElement("svg", {
    width: "22",
    height: "6",
    viewBox: "0 0 22 6"
  }, /*#__PURE__*/React.createElement("circle", {
    cx: "3",
    cy: "3",
    r: "2.5",
    fill: muted
  }), /*#__PURE__*/React.createElement("circle", {
    cx: "11",
    cy: "3",
    r: "2.5",
    fill: muted
  }), /*#__PURE__*/React.createElement("circle", {
    cx: "19",
    cy: "3",
    r: "2.5",
    fill: muted
  })))), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 16px',
      fontFamily: '-apple-system, system-ui',
      fontSize: 34,
      fontWeight: 700,
      lineHeight: '41px',
      color: text,
      letterSpacing: 0.4
    }
  }, title));
}

// ─────────────────────────────────────────────────────────────
// Grouped list (inset card, r:26) + row (52px)
// ─────────────────────────────────────────────────────────────
function IOSListRow({
  title,
  detail,
  icon,
  chevron = true,
  isLast = false,
  dark = false
}) {
  const text = dark ? '#fff' : '#000';
  const sec = dark ? 'rgba(235,235,245,0.6)' : 'rgba(60,60,67,0.6)';
  const ter = dark ? 'rgba(235,235,245,0.3)' : 'rgba(60,60,67,0.3)';
  const sep = dark ? 'rgba(84,84,88,0.65)' : 'rgba(60,60,67,0.12)';
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      minHeight: 52,
      padding: '0 16px',
      position: 'relative',
      fontFamily: '-apple-system, system-ui',
      fontSize: 17,
      letterSpacing: -0.43
    }
  }, icon && /*#__PURE__*/React.createElement("div", {
    style: {
      width: 30,
      height: 30,
      borderRadius: 7,
      background: icon,
      marginRight: 12,
      flexShrink: 0
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      color: text
    }
  }, title), detail && /*#__PURE__*/React.createElement("span", {
    style: {
      color: sec,
      marginRight: 6
    }
  }, detail), chevron && /*#__PURE__*/React.createElement("svg", {
    width: "8",
    height: "14",
    viewBox: "0 0 8 14",
    style: {
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement("path", {
    d: "M1 1l6 6-6 6",
    stroke: ter,
    strokeWidth: "2",
    fill: "none",
    strokeLinecap: "round",
    strokeLinejoin: "round"
  })), !isLast && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      bottom: 0,
      right: 0,
      left: icon ? 58 : 16,
      height: 0.5,
      background: sep
    }
  }));
}
function IOSList({
  header,
  children,
  dark = false
}) {
  const hc = dark ? 'rgba(235,235,245,0.6)' : 'rgba(60,60,67,0.6)';
  const bg = dark ? '#1C1C1E' : '#fff';
  return /*#__PURE__*/React.createElement("div", null, header && /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: '-apple-system, system-ui',
      fontSize: 13,
      color: hc,
      textTransform: 'uppercase',
      padding: '8px 36px 6px',
      letterSpacing: -0.08
    }
  }, header), /*#__PURE__*/React.createElement("div", {
    style: {
      background: bg,
      borderRadius: 26,
      margin: '0 16px',
      overflow: 'hidden'
    }
  }, children));
}

// ─────────────────────────────────────────────────────────────
// Device frame
// ─────────────────────────────────────────────────────────────
function IOSDevice({
  children,
  width = 402,
  height = 874,
  dark = false,
  title,
  keyboard = false
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      width,
      height,
      borderRadius: 48,
      overflow: 'hidden',
      position: 'relative',
      background: dark ? '#000' : '#F2F2F7',
      boxShadow: '0 40px 80px rgba(0,0,0,0.18), 0 0 0 1px rgba(0,0,0,0.12)',
      fontFamily: '-apple-system, system-ui, sans-serif',
      WebkitFontSmoothing: 'antialiased'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      top: 11,
      left: '50%',
      transform: 'translateX(-50%)',
      width: 126,
      height: 37,
      borderRadius: 24,
      background: '#000',
      zIndex: 50
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      top: 0,
      left: 0,
      right: 0,
      zIndex: 10
    }
  }, /*#__PURE__*/React.createElement(IOSStatusBar, {
    dark: dark
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column'
    }
  }, title !== undefined && /*#__PURE__*/React.createElement(IOSNavBar, {
    title: title,
    dark: dark
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      overflow: 'auto'
    }
  }, children), keyboard && /*#__PURE__*/React.createElement(IOSKeyboard, {
    dark: dark
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      bottom: 0,
      left: 0,
      right: 0,
      zIndex: 60,
      height: 34,
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'flex-end',
      paddingBottom: 8,
      pointerEvents: 'none'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 139,
      height: 5,
      borderRadius: 100,
      background: dark ? 'rgba(255,255,255,0.7)' : 'rgba(0,0,0,0.25)'
    }
  })));
}

// ─────────────────────────────────────────────────────────────
// Keyboard — iOS 26 liquid glass
// ─────────────────────────────────────────────────────────────
function IOSKeyboard({
  dark = false
}) {
  const glyph = dark ? 'rgba(255,255,255,0.7)' : '#595959';
  const sugg = dark ? 'rgba(255,255,255,0.6)' : '#333';
  const keyBg = dark ? 'rgba(255,255,255,0.22)' : 'rgba(255,255,255,0.85)';

  // special-key icons
  const icons = {
    shift: /*#__PURE__*/React.createElement("svg", {
      width: "19",
      height: "17",
      viewBox: "0 0 19 17"
    }, /*#__PURE__*/React.createElement("path", {
      d: "M9.5 1L1 9.5h4.5V16h8V9.5H18L9.5 1z",
      fill: glyph
    })),
    del: /*#__PURE__*/React.createElement("svg", {
      width: "23",
      height: "17",
      viewBox: "0 0 23 17"
    }, /*#__PURE__*/React.createElement("path", {
      d: "M7 1h13a2 2 0 012 2v11a2 2 0 01-2 2H7l-6-7.5L7 1z",
      fill: "none",
      stroke: glyph,
      strokeWidth: "1.6",
      strokeLinejoin: "round"
    }), /*#__PURE__*/React.createElement("path", {
      d: "M10 5l7 7M17 5l-7 7",
      stroke: glyph,
      strokeWidth: "1.6",
      strokeLinecap: "round"
    })),
    ret: /*#__PURE__*/React.createElement("svg", {
      width: "20",
      height: "14",
      viewBox: "0 0 20 14"
    }, /*#__PURE__*/React.createElement("path", {
      d: "M18 1v6H4m0 0l4-4M4 7l4 4",
      fill: "none",
      stroke: "#fff",
      strokeWidth: "1.8",
      strokeLinecap: "round",
      strokeLinejoin: "round"
    }))
  };
  const key = (content, {
    w,
    flex,
    ret,
    fs = 25,
    k
  } = {}) => /*#__PURE__*/React.createElement("div", {
    key: k,
    style: {
      height: 42,
      borderRadius: 8.5,
      flex: flex ? 1 : undefined,
      width: w,
      minWidth: 0,
      background: ret ? '#08f' : keyBg,
      boxShadow: '0 1px 0 rgba(0,0,0,0.075)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      fontFamily: '-apple-system, "SF Compact", system-ui',
      fontSize: fs,
      fontWeight: 458,
      color: ret ? '#fff' : glyph
    }
  }, content);
  const row = (keys, pad = 0) => /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6.5,
      justifyContent: 'center',
      padding: `0 ${pad}px`
    }
  }, keys.map(l => key(l, {
    flex: true,
    k: l
  })));
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      zIndex: 15,
      borderRadius: 27,
      overflow: 'hidden',
      padding: '11px 0 2px',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      boxShadow: dark ? '0 -2px 20px rgba(0,0,0,0.09)' : '0 -1px 6px rgba(0,0,0,0.018), 0 -3px 20px rgba(0,0,0,0.012)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      borderRadius: 27,
      backdropFilter: 'blur(12px) saturate(180%)',
      WebkitBackdropFilter: 'blur(12px) saturate(180%)',
      background: dark ? 'rgba(120,120,128,0.14)' : 'rgba(255,255,255,0.25)'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      borderRadius: 27,
      boxShadow: dark ? 'inset 1.5px 1.5px 1px rgba(255,255,255,0.15)' : 'inset 1.5px 1.5px 1px rgba(255,255,255,0.7), inset -1px -1px 1px rgba(255,255,255,0.4)',
      border: dark ? '0.5px solid rgba(255,255,255,0.15)' : '0.5px solid rgba(0,0,0,0.06)',
      pointerEvents: 'none'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 20,
      alignItems: 'center',
      padding: '8px 22px 13px',
      width: '100%',
      boxSizing: 'border-box',
      position: 'relative'
    }
  }, ['"The"', 'the', 'to'].map((w, i) => /*#__PURE__*/React.createElement(React.Fragment, {
    key: i
  }, i > 0 && /*#__PURE__*/React.createElement("div", {
    style: {
      width: 1,
      height: 25,
      background: '#ccc',
      opacity: 0.3
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      textAlign: 'center',
      fontFamily: '-apple-system, system-ui',
      fontSize: 17,
      color: sugg,
      letterSpacing: -0.43,
      lineHeight: '22px'
    }
  }, w)))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 13,
      padding: '0 6.5px',
      width: '100%',
      boxSizing: 'border-box',
      position: 'relative'
    }
  }, row(['q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p']), row(['a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l'], 20), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 14.25,
      alignItems: 'center'
    }
  }, key(icons.shift, {
    w: 45,
    k: 'shift'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6.5,
      flex: 1
    }
  }, ['z', 'x', 'c', 'v', 'b', 'n', 'm'].map(l => key(l, {
    flex: true,
    k: l
  }))), key(icons.del, {
    w: 45,
    k: 'del'
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6,
      alignItems: 'center'
    }
  }, key('ABC', {
    w: 92.25,
    fs: 18,
    k: 'abc'
  }), key('', {
    flex: true,
    k: 'space'
  }), key(icons.ret, {
    w: 92.25,
    ret: true,
    k: 'ret'
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      height: 56,
      width: '100%',
      position: 'relative'
    }
  }));
}
Object.assign(window, {
  IOSDevice,
  IOSStatusBar,
  IOSNavBar,
  IOSGlassPill,
  IOSList,
  IOSListRow,
  IOSKeyboard
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/ios-frame.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/persist.jsx
try { (() => {
// persist.jsx — lightweight localStorage persistence for the Arah app demo.
// Survives reload: feed posts, store products, wallet, chats, and session prefs.
// Loads right after data.jsx so window.ARAH is hydrated before any screen renders.

(function () {
  const KEY = 'arah_app_v1';
  window.arahLoad = function () {
    try {
      return JSON.parse(localStorage.getItem(KEY)) || {};
    } catch (e) {
      return {};
    }
  };
  window.arahSave = function (patch) {
    try {
      const cur = window.arahLoad();
      localStorage.setItem(KEY, JSON.stringify(Object.assign({}, cur, patch)));
    } catch (e) {/* quota / private mode — ignore */}
  };

  // Snapshot the mutable slices of window.ARAH (no functions — content/wallet/store/chats are plain)
  window.arahSnapshot = function () {
    const A = window.ARAH;
    if (!A) return;
    window.arahSave({
      data: {
        content: A.content,
        // per-territory feed/events (t1–t3)
        myStore: A.myStore,
        wallet: A.wallet,
        chats: A.chats
      }
    });
  };

  // Apply saved slices back onto window.ARAH
  window.arahHydrate = function () {
    const saved = window.arahLoad();
    const d = saved && saved.data;
    if (!d || !window.ARAH) return;
    if (d.content) {
      // merge per-territory (keep territories that weren't saved)
      Object.keys(d.content).forEach(function (k) {
        window.ARAH.content[k] = d.content[k];
      });
    }
    if (d.myStore) window.ARAH.myStore = d.myStore;
    if (d.wallet) window.ARAH.wallet = d.wallet;
    if (d.chats) window.ARAH.chats = d.chats;
  };

  // Save / read the React-session slice (prefs that live in App state)
  window.arahSaveApp = function (appSlice) {
    window.arahSave({
      app: appSlice
    });
  };
  window.arahLoadApp = function () {
    return window.arahLoad().app || {};
  };

  // Clear everything (used by a "limpar dados" affordance / logout-reset if desired)
  window.arahReset = function () {
    try {
      localStorage.removeItem(KEY);
    } catch (e) {}
  };

  // Hydrate immediately at load (data.jsx already ran)
  window.arahHydrate();
})();
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/persist.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens1.jsx
try { (() => {
// screens1.jsx — Feed, PostCard, PostDetail (comments), Explore.

function PostCard({
  post,
  onLike,
  liked,
  onOpen
}) {
  const isAlert = post.type === 'Alert';
  const accent = isAlert ? T.alert : T.green;
  return /*#__PURE__*/React.createElement(Card, {
    glow: true,
    style: {
      padding: 16,
      marginBottom: 12,
      border: post.pinned ? `1px solid rgba(232,160,106,0.28)` : `1px solid ${T.line}`
    }
  }, post.pinned && /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 5,
      marginBottom: 11,
      color: T.alert,
      fontFamily: T.sans,
      fontSize: 11.5,
      fontWeight: 600,
      letterSpacing: 0.2
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "push_pin",
    size: 14,
    fill: 1
  }), " Fixado pelo conselho"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      marginBottom: 13
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: post.avatar,
    name: post.author,
    size: 42,
    resident: post.role === 'morador'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, post.author), /*#__PURE__*/React.createElement(RoleBadge, {
    role: post.role
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      color: T.fg3,
      fontFamily: T.sans,
      fontSize: 12.5,
      marginTop: 1
    }
  }, /*#__PURE__*/React.createElement("span", null, "@", post.handle), /*#__PURE__*/React.createElement("span", {
    style: {
      opacity: 0.5
    }
  }, "\xB7"), /*#__PURE__*/React.createElement("span", null, post.time), post.visibility === 'ResidentsOnly' && /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 3,
      color: T.green
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      opacity: 0.5,
      color: T.fg3
    }
  }, "\xB7"), /*#__PURE__*/React.createElement(Icon, {
    name: "lock",
    size: 12
  }), " s\xF3 moradores"))), isAlert && /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 5,
      padding: '5px 10px',
      borderRadius: 999,
      background: T.alertDim,
      color: T.alert,
      fontFamily: T.sans,
      fontSize: 11.5,
      fontWeight: 600
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "warning",
    size: 14,
    fill: 1
  }), " Alerta")), /*#__PURE__*/React.createElement("div", {
    onClick: onOpen,
    style: {
      cursor: 'pointer'
    }
  }, /*#__PURE__*/React.createElement("h3", {
    style: {
      margin: '0 0 6px',
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 17,
      color: T.fg,
      lineHeight: 1.3,
      letterSpacing: -0.3
    }
  }, post.title), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: 0,
      fontFamily: T.sans,
      fontSize: 14.5,
      lineHeight: 1.55,
      color: T.fg2,
      textWrap: 'pretty'
    }
  }, post.body), post.photo && /*#__PURE__*/React.createElement("div", {
    style: {
      marginTop: 13,
      borderRadius: 14,
      overflow: 'hidden',
      border: `1px solid ${T.line}`,
      position: 'relative'
    }
  }, /*#__PURE__*/React.createElement("img", {
    src: post.photo,
    alt: "",
    style: {
      width: '100%',
      height: 168,
      objectFit: 'cover',
      display: 'block'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      background: 'linear-gradient(to top, rgba(11,12,10,0.35), transparent 50%)'
    }
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 22,
      marginTop: 14
    }
  }, /*#__PURE__*/React.createElement("button", {
    onClick: onLike,
    style: postAction(liked ? accent : T.fg3)
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "favorite",
    size: 19,
    fill: liked ? 1 : 0,
    color: liked ? accent : T.fg3
  }), post.likes + (liked ? 1 : 0)), /*#__PURE__*/React.createElement("button", {
    onClick: onOpen,
    style: postAction(T.fg3)
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "mode_comment",
    size: 18
  }), " ", post.comments), /*#__PURE__*/React.createElement("button", {
    style: postAction(T.fg3)
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "ios_share",
    size: 18
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }), /*#__PURE__*/React.createElement("button", {
    style: {
      ...postAction(T.fg3),
      gap: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "bookmark_border",
    size: 19
  }))));
}
const postAction = color => ({
  display: 'inline-flex',
  alignItems: 'center',
  gap: 6,
  background: 'none',
  border: 'none',
  cursor: 'pointer',
  color,
  fontFamily: T.sans,
  fontSize: 13.5,
  fontWeight: 500,
  WebkitTapHighlightColor: 'transparent',
  padding: 0
});
function FeedScreen({
  territory,
  content,
  likes,
  onLike,
  onOpen,
  role
}) {
  const [filter, setFilter] = React.useState('Tudo');
  const filters = ['Tudo', 'Avisos', 'Vizinhança'];
  let posts = content.feed;
  if (filter === 'Avisos') posts = posts.filter(p => p.type === 'Alert');
  if (filter === 'Vizinhança') posts = posts.filter(p => p.visibility !== 'ResidentsOnly');
  return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '2px 18px 12px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      overflowX: 'auto',
      paddingBottom: 2
    },
    className: "noscroll"
  }, filters.map(f => /*#__PURE__*/React.createElement(Chip, {
    key: f,
    active: filter === f,
    onClick: () => setFilter(f)
  }, f)))), role === 'visitante' && /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 12px'
    }
  }, /*#__PURE__*/React.createElement(Card, {
    onClick: () => window.openJourney('residencia'),
    style: {
      padding: 14,
      display: 'flex',
      alignItems: 'center',
      gap: 12,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 38,
      height: 38,
      borderRadius: 11,
      background: T.greenDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "cottage",
    size: 20,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      color: T.fg
    }
  }, "Voc\xEA \xE9 visitante de ", territory.name), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      marginTop: 1
    }
  }, "Confirme resid\xEAncia para ver posts e votar.")), /*#__PURE__*/React.createElement(Icon, {
    name: "chevron_right",
    size: 20,
    color: T.fg3
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 8px'
    }
  }, posts.map(p => /*#__PURE__*/React.createElement(PostCard, {
    key: p.id,
    post: p,
    liked: !!likes[p.id],
    onLike: () => onLike(p.id),
    onOpen: () => onOpen(p.id)
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      textAlign: 'center',
      color: T.fg3,
      fontFamily: T.sans,
      fontSize: 12.5,
      padding: '14px 0 4px',
      letterSpacing: 0.2
    }
  }, "Voc\xEA viu tudo de ", territory.name, " \xB7 ", territory.members.toLocaleString('pt-BR'), " moradores")));
}

// Post detail with comments + composer
function PostDetailScreen({
  postId,
  likes,
  onLike
}) {
  const allPosts = Object.values(window.ARAH.content).flatMap(c => c.feed);
  const post = allPosts.find(p => p.id === postId) || window.ARAH.content.t1.feed[0];
  const [comments, setComments] = React.useState(window.ARAH.comments[postId] || []);
  const [draft, setDraft] = React.useState('');
  const send = () => {
    if (!draft.trim()) return;
    setComments(c => [...c, {
      id: 'cx' + Date.now(),
      author: 'Ana Ribeiro',
      avatar: '#4F956F',
      role: 'visitante',
      time: 'agora',
      body: draft.trim()
    }]);
    setDraft('');
  };
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%'
    }
  }, /*#__PURE__*/React.createElement("div", {
    className: "appscroll",
    style: {
      flex: 1,
      overflowY: 'auto',
      padding: '0 18px 12px'
    }
  }, /*#__PURE__*/React.createElement(PostCard, {
    post: post,
    liked: !!likes[postId],
    onLike: () => onLike(postId),
    onOpen: () => {}
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      fontWeight: 600,
      letterSpacing: 0.6,
      textTransform: 'uppercase',
      color: T.fg3,
      margin: '4px 2px 12px'
    }
  }, comments.length, " coment\xE1rios"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 14
    }
  }, comments.map(c => /*#__PURE__*/React.createElement("div", {
    key: c.id,
    style: {
      display: 'flex',
      gap: 11
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: c.avatar,
    name: c.author,
    size: 34,
    resident: c.role === 'morador'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 13.5,
      color: T.fg
    }
  }, c.author), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3
    }
  }, c.time)), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '3px 0 0',
      fontFamily: T.sans,
      fontSize: 14,
      lineHeight: 1.5,
      color: T.fg2
    }
  }, c.body)))))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      padding: '12px 16px 16px',
      borderTop: `1px solid ${T.line}`,
      background: T.bg2
    }
  }, /*#__PURE__*/React.createElement("input", {
    value: draft,
    onChange: e => setDraft(e.target.value),
    onKeyDown: e => e.key === 'Enter' && send(),
    placeholder: "Comentar como visitante\u2026",
    style: {
      flex: 1,
      background: T.cardFlat,
      color: T.fg,
      border: `1px solid ${T.line}`,
      borderRadius: 999,
      padding: '11px 16px',
      fontFamily: T.sans,
      fontSize: 14,
      outline: 'none'
    }
  }), /*#__PURE__*/React.createElement("button", {
    onClick: send,
    style: {
      width: 42,
      height: 42,
      borderRadius: '50%',
      border: 'none',
      background: T.greenGrad,
      cursor: 'pointer',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      boxShadow: T.greenGlow,
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "send",
    size: 19,
    color: "#0C1B10",
    fill: 1
  }))));
}
function ExploreScreen({
  territory,
  activeId,
  onEnter,
  onMap,
  onTool,
  role
}) {
  const list = window.ARAH.territories;
  const content = window.ARAH.getContent(activeId);
  const tools = [{
    id: 'map',
    icon: 'map',
    label: 'Mapa',
    color: T.green
  }, {
    id: 'events',
    icon: 'event',
    label: 'Eventos',
    color: T.blue
  }, {
    id: 'health',
    icon: 'eco',
    label: 'Saúde do território',
    color: T.green
  }, {
    id: 'elections',
    icon: 'how_to_vote',
    label: 'Eleições',
    color: T.alert,
    badge: 'Novo'
  }, ...(role === 'curador' ? [{
    id: 'manage',
    icon: 'shield_person',
    label: 'Gestão',
    color: T.alert,
    badge: 'Curador'
  }] : [])];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 12px'
    }
  }, /*#__PURE__*/React.createElement(Card, {
    onClick: onMap,
    hi: true,
    style: {
      padding: 0,
      overflow: 'hidden',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      height: 116
    }
  }, /*#__PURE__*/React.createElement(MapCanvas, {
    height: 116,
    mini: true,
    tile: content.tile
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      padding: 16,
      background: 'linear-gradient(to right, rgba(11,12,10,0.85), rgba(11,12,10,0.2))'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 42,
      height: 42,
      borderRadius: 12,
      background: T.greenDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "map",
    size: 22,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg
    }
  }, "Mapa do territ\xF3rio"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2
    }
  }, "Lugares, trilhas, nascentes e mirantes")), /*#__PURE__*/React.createElement(Icon, {
    name: "arrow_forward",
    size: 20,
    color: T.fg2
  })))), /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "apps"
  }, "Ferramentas de ", territory.name), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 10,
      marginTop: 11,
      marginBottom: 20
    }
  }, tools.map(t => /*#__PURE__*/React.createElement(Card, {
    key: t.id,
    onClick: () => onTool(t.id),
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      padding: 14
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 38,
      height: 38,
      borderRadius: 11,
      background: `${t.color}1f`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: t.icon,
    size: 20,
    color: t.color,
    fill: 1
  })), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 500,
      color: T.fg,
      lineHeight: 1.2
    }
  }, t.label), t.badge && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 9.5,
      fontWeight: 700,
      color: t.color,
      background: `${t.color}1f`,
      padding: '2px 6px',
      borderRadius: 999,
      letterSpacing: 0.3
    }
  }, t.badge)))), /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "travel_explore"
  }, "Trocar de territ\xF3rio"), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '11px 0 12px',
      fontFamily: T.sans,
      fontSize: 13.5,
      color: T.fg2
    }
  }, "Toque em um territ\xF3rio para ver o feed da regi\xE3o ou trocar de comunidade."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, list.map(t => {
    const on = t.id === activeId;
    return /*#__PURE__*/React.createElement(Card, {
      key: t.id,
      onClick: () => onEnter(t.id),
      hi: on,
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 13,
        padding: 14,
        border: `1px solid ${on ? 'rgba(166,214,185,0.32)' : T.line}`
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 46,
        height: 46,
        borderRadius: 14,
        flexShrink: 0,
        background: on ? T.greenGrad : T.cardHi,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: on ? 'location_on' : 'forest',
      size: 24,
      color: on ? '#0C1B10' : T.green,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 8
      }
    }, /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 16,
        color: T.fg,
        letterSpacing: -0.2
      }
    }, t.name), on && /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 10.5,
        fontWeight: 600,
        color: T.green,
        background: 'rgba(166,214,185,0.15)',
        padding: '2px 8px',
        borderRadius: 999
      }
    }, "ATIVO")), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 12.5,
        color: T.fg3,
        marginTop: 2
      }
    }, t.region, " \xB7 ", t.distance, " km"), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 13,
        color: T.fg2,
        marginTop: 5
      }
    }, t.desc)), /*#__PURE__*/React.createElement(Icon, {
      name: on ? 'check_circle' : 'chevron_right',
      size: on ? 22 : 20,
      color: on ? T.green : T.fg3,
      fill: on ? 1 : 0
    }));
  })));
}
Object.assign(window, {
  PostCard,
  FeedScreen,
  PostDetailScreen,
  ExploreScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens1.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens10.jsx
try { (() => {
// screens10.jsx — Trocas, Entregas, Carteira / Moeda Territorial.

function TradesScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 20"
  }, "Trocas comunit\xE1rias \u2014 escambo e doa\xE7\xF5es sem dinheiro. Vis\xE3o de produto."), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "md",
    icon: "add",
    style: {
      marginBottom: 14
    },
    onClick: () => window.openJourney('create', window.JOURNEY_PRESETS.troca)
  }, "Propor uma troca"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, window.ARAH.trades.map(t => /*#__PURE__*/React.createElement(Card, {
    key: t.id,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: 14
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 44,
      height: 44,
      borderRadius: 12,
      background: T.greenDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: t.icon,
    size: 22,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14.5,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, t.title), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      marginTop: 5
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: t.avatar,
    name: t.who,
    size: 18
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, t.who), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 11,
      color: T.green,
      background: T.greenDim,
      padding: '2px 8px',
      borderRadius: 999
    }
  }, "quer: ", t.want))), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "swap_horiz",
    onClick: () => window.openJourney('create', window.JOURNEY_PRESETS.troca)
  }, "Propor")))));
}
function DeliveryScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 21"
  }, "Entregas territoriais \u2014 log\xEDstica local entre vizinhos. Vis\xE3o de produto."), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "md",
    icon: "add",
    style: {
      marginBottom: 14
    },
    onClick: () => window.openJourney('deliveryReq')
  }, "Solicitar entrega"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 0,
      overflow: 'hidden',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      height: 150
    }
  }, /*#__PURE__*/React.createElement(MapCanvas, {
    height: 150,
    mini: true,
    tile: window.ARAH.content.t1.tile
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '34%',
      top: '52%',
      transform: 'translate(-50%,-50%)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 30,
      height: 30,
      borderRadius: '50%',
      background: T.green,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      boxShadow: T.greenGlow
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "two_wheeler",
    size: 17,
    color: "#0C1B10",
    fill: 1
  }))))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, window.ARAH.deliveries.map(d => {
    const done = d.status === 'entregue';
    return /*#__PURE__*/React.createElement(Card, {
      key: d.id,
      style: {
        padding: 14
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 11
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 42,
        height: 42,
        borderRadius: 12,
        background: done ? T.greenDim : T.blueDim,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexShrink: 0
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: d.icon,
      size: 21,
      color: done ? T.green : T.blue,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 14,
        color: T.fg
      }
    }, d.from, " \u2192 ", d.to), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 12,
        color: T.fg3,
        marginTop: 2
      }
    }, "Entregador: ", d.courier)), /*#__PURE__*/React.createElement("div", {
      style: {
        textAlign: 'right',
        flexShrink: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'inline-flex',
        alignItems: 'center',
        gap: 4,
        fontFamily: T.sans,
        fontSize: 11.5,
        fontWeight: 600,
        color: done ? T.green : T.blue,
        background: done ? T.greenDim : T.blueDim,
        padding: '4px 10px',
        borderRadius: 999
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: done ? 'check' : 'schedule',
      size: 13,
      fill: 1
    }), " ", d.status), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 11,
        color: T.fg3,
        marginTop: 4
      }
    }, d.eta))));
  })));
}
function WalletScreen() {
  const w = window.ARAH.wallet;
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 22"
  }, "Moeda territorial \u2014 cr\xE9ditos que circulam dentro da comunidade. Vis\xE3o de produto."), /*#__PURE__*/React.createElement(Card, {
    glow: true,
    style: {
      padding: 20,
      marginBottom: 16,
      background: 'linear-gradient(155deg, #1E3A2A, #14241A)',
      border: '1px solid rgba(166,214,185,0.2)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      marginBottom: 14
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 30,
      height: 30,
      borderRadius: 9,
      background: T.greenSolid,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 16,
      color: '#0C1B10'
    }
  }, w.symbol), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.green,
      fontWeight: 600
    }
  }, w.name, " \xB7 moeda de Camburi")), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 38,
      color: '#fff',
      letterSpacing: -1
    }
  }, w.balance), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: 'rgba(255,255,255,0.6)',
      marginTop: 2
    }
  }, "\u2248 ", w.brl), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 9,
      marginTop: 18
    }
  }, /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    size: "sm",
    icon: "north_east",
    style: {
      flex: 1,
      background: T.greenSolid,
      color: '#0C1B10'
    },
    onClick: () => window.openJourney('walletSend')
  }, "Enviar"), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "south_west",
    style: {
      flex: 1,
      color: '#fff',
      borderColor: 'rgba(255,255,255,0.18)'
    },
    onClick: () => window.openJourney('walletReceive')
  }, "Receber"), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "add",
    style: {
      flex: 1,
      color: '#fff',
      borderColor: 'rgba(255,255,255,0.18)'
    },
    onClick: () => window.openJourney('walletTopup')
  }, "Adicionar"))), /*#__PURE__*/React.createElement(SubHead, {
    icon: "receipt_long"
  }, "Movimenta\xE7\xF5es"), /*#__PURE__*/React.createElement(Card, {
    style: {
      overflow: 'hidden'
    }
  }, w.transactions.map((tx, i) => /*#__PURE__*/React.createElement("div", {
    key: tx.id,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: '13px 15px',
      borderBottom: i === w.transactions.length - 1 ? 'none' : `1px solid ${T.line}`
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 38,
      height: 38,
      borderRadius: 11,
      background: T.cardHi,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: tx.icon,
    size: 19,
    color: tx.neg ? T.fg2 : T.green,
    fill: tx.neg ? 0 : 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      fontWeight: 500,
      color: T.fg
    }
  }, tx.label), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      marginTop: 1
    }
  }, tx.sub)), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 14.5,
      color: tx.neg ? T.fg2 : T.green
    }
  }, tx.val)))));
}
Object.assign(window, {
  TradesScreen,
  DeliveryScreen,
  WalletScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens10.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens11.jsx
try { (() => {
// screens11.jsx — Serviços pessoais: Babás, Bem-estar (espaços + prestadores).

function SittersScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "\xC9pico 10"
  }, "Bab\xE1s verificadas do territ\xF3rio \u2014 confian\xE7a entre vizinhos. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 12
    }
  }, window.ARAH.sitters.map(s => /*#__PURE__*/React.createElement(Card, {
    key: s.id,
    glow: true,
    style: {
      padding: 15
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 13
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: s.avatar,
    name: s.name,
    size: 56,
    resident: s.verified
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, s.name), s.verified && /*#__PURE__*/React.createElement(Icon, {
    name: "verified",
    size: 15,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      marginTop: 3,
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 3,
      color: T.fg2
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "star",
    size: 14,
    color: "#F5C451",
    fill: 1
  }), s.rating, " ", /*#__PURE__*/React.createElement("span", {
    style: {
      color: T.fg3
    }
  }, "(", s.reviews, ")")), /*#__PURE__*/React.createElement("span", null, "\xB7 ", s.exp), /*#__PURE__*/React.createElement("span", null, "\xB7 ", s.dist)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexWrap: 'wrap',
      gap: 6,
      marginTop: 9
    }
  }, s.badges.map(b => /*#__PURE__*/React.createElement("span", {
    key: b,
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4,
      fontFamily: T.sans,
      fontSize: 11,
      color: T.green,
      background: T.greenDim,
      padding: '3px 9px',
      borderRadius: 999
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "check",
    size: 12
  }), b)), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 11,
      color: T.fg2,
      background: T.cardHi,
      padding: '3px 9px',
      borderRadius: 999
    }
  }, s.ages)))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      marginTop: 14,
      paddingTop: 13,
      borderTop: `1px solid ${T.line}`
    }
  }, /*#__PURE__*/React.createElement("span", null, /*#__PURE__*/React.createElement("strong", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 17,
      color: T.fg
    }
  }, s.price)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "chat",
    onClick: () => window.appNav.openChat(s.name, s.avatar)
  }, "Conversar"), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    size: "sm",
    icon: "event_available",
    onClick: () => window.openJourney('sitter', s)
  }, "Solicitar")))))));
}
function WellnessScreen() {
  const [tab, setTab] = React.useState('espacos');
  const w = window.ARAH.wellness;
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "\xC9pico 10"
  }, "Bem-estar local \u2014 espa\xE7os e profissionais com agenda compartilhada. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'espacos',
    onClick: () => setTab('espacos'),
    icon: "spa"
  }, "Espa\xE7os"), /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'prestadores',
    onClick: () => setTab('prestadores'),
    icon: "self_improvement"
  }, "Profissionais")), tab === 'espacos' ? /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 12
    }
  }, w.spaces.map(s => /*#__PURE__*/React.createElement(Card, {
    key: s.id,
    glow: true,
    style: {
      padding: 0,
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: 104,
      background: `linear-gradient(150deg, ${s.avatar}40, ${s.avatar}12)`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: s.icon,
    size: 44,
    color: T.fg,
    fill: 0,
    style: {
      opacity: 0.85
    }
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 15
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, s.name), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3,
      marginTop: 2
    }
  }, s.type, " \xB7 ", s.cap), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      marginTop: 13
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 5,
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.green
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "event_available",
    size: 15,
    fill: 1
  }), " ", s.slots), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    size: "sm",
    icon: "calendar_month",
    onClick: () => window.openJourney('wellness', s)
  }, "Ver agenda")))))) : /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 11
    }
  }, w.providers.map(p => /*#__PURE__*/React.createElement(Card, {
    key: p.id,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: 14
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: p.avatar,
    name: p.name,
    size: 48,
    resident: p.verified
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15,
      color: T.fg
    }
  }, p.name), p.verified && /*#__PURE__*/React.createElement(Icon, {
    name: "verified",
    size: 14,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3,
      marginTop: 2
    }
  }, p.spec), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4,
      marginTop: 5,
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg2
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "star",
    size: 13,
    color: "#F5C451",
    fill: 1
  }), " ", p.rating, " \xB7 a partir de ", /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.fg,
      fontWeight: 600
    }
  }, p.price))), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "calendar_month",
    style: {
      flexShrink: 0
    },
    onClick: () => window.openJourney('wellness', p)
  }, "Agendar")))));
}
Object.assign(window, {
  SittersScreen,
  WellnessScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens11.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens12.jsx
try { (() => {
// screens12.jsx — Painel de métricas, Banco de sementes, Learning Hub.

function MetricsScreen({
  territory
}) {
  const m = window.ARAH.metrics;
  const max = Math.max(...m.bars.map(b => b.v));
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 25"
  }, "Painel de m\xE9tricas \u2014 o pulso vivo do territ\xF3rio. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 10,
      marginBottom: 14
    }
  }, [['Engajamento', m.engagement + '%', 'trending_up', T.green], ['Posts no mês', m.posts, 'article', T.blue], ['Eventos', m.events, 'event', T.amber || '#E8A06A'], ['Novos membros', '+' + m.newMembers, 'group_add', T.green]].map(([k, v, ic, c]) => /*#__PURE__*/React.createElement(Card, {
    key: k,
    style: {
      padding: 15
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: ic,
    size: 20,
    color: c,
    fill: 1
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 24,
      color: T.fg,
      marginTop: 8,
      letterSpacing: -0.5
    }
  }, v), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 1
    }
  }, k)))), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 16
    }
  }, /*#__PURE__*/React.createElement(SubHead, {
    icon: "bar_chart"
  }, "Atividade na semana"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'flex-end',
      gap: 8,
      height: 130,
      marginTop: 4
    }
  }, m.bars.map(b => /*#__PURE__*/React.createElement("div", {
    key: b.label,
    style: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: '100%',
      height: `${b.v / max * 100}%`,
      minHeight: 6,
      borderRadius: '7px 7px 3px 3px',
      background: T.greenGrad
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 10.5,
      color: T.fg3
    }
  }, b.label))))));
}
function SeedsScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 48"
  }, "Banco de sementes e mudas \u2014 regenerar o territ\xF3rio, juntos. Vis\xE3o de produto."), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 16,
      marginBottom: 16,
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 46,
      height: 46,
      borderRadius: 13,
      background: T.greenDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "forest",
    size: 24,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 18,
      color: T.fg
    }
  }, "43 esp\xE9cies"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3
    }
  }, "dispon\xEDveis para troca em Camburi")), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    size: "sm",
    icon: "add",
    onClick: () => window.openJourney('create', window.JOURNEY_PRESETS.semente)
  }, "Doar")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 11
    }
  }, window.ARAH.seeds.map(s => /*#__PURE__*/React.createElement(Card, {
    key: s.id,
    style: {
      padding: 15
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 42,
      height: 42,
      borderRadius: 12,
      background: T.greenDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      marginBottom: 11
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: s.icon,
    size: 22,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15,
      color: T.fg
    }
  }, s.name), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 2
    }
  }, s.qty), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      marginTop: 9
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 10.5,
      color: T.green,
      background: T.greenDim,
      padding: '2px 8px',
      borderRadius: 999
    }
  }, s.tag)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      marginTop: 10,
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: "#4F956F",
    name: s.who,
    size: 16
  }), " ", s.who)))));
}
function LearningScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 45"
  }, "Aprendizado \u2014 saberes e of\xEDcios que vivem no territ\xF3rio. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 12
    }
  }, window.ARAH.courses.map(c => /*#__PURE__*/React.createElement(Card, {
    key: c.id,
    glow: true,
    onClick: () => window.openJourney('course', c),
    style: {
      padding: 0,
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 92,
      flexShrink: 0,
      background: `linear-gradient(150deg, ${c.avatar}40, ${c.avatar}12)`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: c.icon,
    size: 34,
    color: T.fg,
    fill: 0,
    style: {
      opacity: 0.85
    }
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0,
      padding: 15
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 10,
      fontWeight: 700,
      letterSpacing: 0.6,
      textTransform: 'uppercase',
      color: T.green
    }
  }, c.tag), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15.5,
      color: T.fg,
      marginTop: 3,
      letterSpacing: -0.2,
      lineHeight: 1.25
    }
  }, c.title), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      marginTop: 8,
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "play_lesson",
    size: 14
  }), " ", c.lessons, " aulas"), /*#__PURE__*/React.createElement("span", null, "\xB7 ", c.level)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      marginTop: 9
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: c.avatar,
    name: c.teacher,
    size: 18,
    resident: true
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg2
    }
  }, c.teacher))))))));
}
Object.assign(window, {
  MetricsScreen,
  SeedsScreen,
  LearningScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens12.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens13.jsx
try { (() => {
// screens13.jsx — Assistente IA, Conquistas/Gamificação, Assinaturas.

function AIScreen({
  territory
}) {
  const [msgs, setMsgs] = React.useState([{
    me: false,
    body: `Oi! Sou o assistente de ${territory.name}. Posso te ajudar a navegar o território, encontrar serviços e resumir o que está acontecendo. O que você precisa?`
  }]);
  const [draft, setDraft] = React.useState('');
  const scrollRef = React.useRef(null);
  const ask = text => {
    const q = (text || draft).trim();
    if (!q) return;
    setMsgs(m => [...m, {
      me: true,
      body: q
    }]);
    setDraft('');
    setTimeout(() => {
      setMsgs(m => [...m, {
        me: false,
        body: 'Aqui está o que encontrei no território — em breve, com respostas reais conectadas aos dados de Camburi. 🌿',
        typing: false
      }]);
    }, 600);
  };
  React.useEffect(() => {
    if (scrollRef.current) scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
  }, [msgs]);
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 27"
  }, "Assistente IA do territ\xF3rio \u2014 vis\xE3o de produto.")), /*#__PURE__*/React.createElement("div", {
    ref: scrollRef,
    className: "appscroll",
    style: {
      flex: 1,
      overflowY: 'auto',
      padding: '0 16px 12px',
      display: 'flex',
      flexDirection: 'column',
      gap: 11
    }
  }, msgs.map((m, i) => /*#__PURE__*/React.createElement("div", {
    key: i,
    style: {
      alignSelf: m.me ? 'flex-end' : 'flex-start',
      maxWidth: '84%',
      display: 'flex',
      gap: 9,
      flexDirection: m.me ? 'row-reverse' : 'row'
    }
  }, !m.me && /*#__PURE__*/React.createElement("div", {
    style: {
      width: 32,
      height: 32,
      borderRadius: 10,
      background: T.greenGrad,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "auto_awesome",
    size: 17,
    color: "#0C1B10",
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '11px 14px',
      borderRadius: m.me ? '16px 16px 4px 16px' : '16px 16px 16px 4px',
      background: m.me ? T.greenGrad : T.cardGrad,
      color: m.me ? '#0C1B10' : T.fg,
      border: m.me ? 'none' : `1px solid ${T.line}`,
      fontFamily: T.sans,
      fontSize: 14.5,
      lineHeight: 1.5
    }
  }, m.body))), msgs.length <= 1 && /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexWrap: 'wrap',
      gap: 8,
      marginTop: 6
    }
  }, window.ARAH.aiPrompts.map(p => /*#__PURE__*/React.createElement("button", {
    key: p,
    onClick: () => ask(p),
    style: {
      textAlign: 'left',
      padding: '9px 13px',
      borderRadius: 13,
      cursor: 'pointer',
      background: T.cardFlat,
      border: `1px solid ${T.line}`,
      color: T.fg2,
      fontFamily: T.sans,
      fontSize: 13,
      WebkitTapHighlightColor: 'transparent'
    }
  }, p)))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      padding: '12px 16px 16px',
      borderTop: `1px solid ${T.line}`,
      background: T.bg2
    }
  }, /*#__PURE__*/React.createElement("input", {
    value: draft,
    onChange: e => setDraft(e.target.value),
    onKeyDown: e => e.key === 'Enter' && ask(),
    placeholder: "Pergunte sobre o territ\xF3rio\u2026",
    style: {
      flex: 1,
      background: T.cardFlat,
      color: T.fg,
      border: `1px solid ${T.line}`,
      borderRadius: 999,
      padding: '11px 16px',
      fontFamily: T.sans,
      fontSize: 14,
      outline: 'none'
    }
  }), /*#__PURE__*/React.createElement("button", {
    onClick: () => ask(),
    style: {
      width: 42,
      height: 42,
      borderRadius: '50%',
      border: 'none',
      background: T.greenGrad,
      cursor: 'pointer',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      boxShadow: T.greenGlow,
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "arrow_upward",
    size: 20,
    color: "#0C1B10",
    fill: 1
  }))));
}
function AchievementsScreen() {
  const list = window.ARAH.achievements;
  const got = list.filter(a => a.got).length;
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 42"
  }, "Conquistas \u2014 reconhecimento do cuidado com o territ\xF3rio. Vis\xE3o de produto."), /*#__PURE__*/React.createElement(Card, {
    glow: true,
    style: {
      padding: 18,
      marginBottom: 16,
      textAlign: 'center',
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 60,
      height: 60,
      borderRadius: 18,
      background: T.greenDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      margin: '0 auto 12px'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "workspace_premium",
    size: 32,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 20,
      color: T.fg
    }
  }, got, " de ", list.length, " conquistas"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg3,
      marginTop: 3
    }
  }, "Guardi\xE3 ativa de Camburi")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 11
    }
  }, list.map(a => /*#__PURE__*/React.createElement(Card, {
    key: a.id,
    style: {
      padding: 15,
      textAlign: 'center',
      opacity: a.got ? 1 : 0.72
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 48,
      height: 48,
      borderRadius: 14,
      background: a.got ? T.greenDim : T.cardHi,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      margin: '0 auto 11px',
      position: 'relative'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: a.icon,
    size: 24,
    color: a.got ? T.green : T.fg3,
    fill: a.got ? 1 : 0
  }), a.got && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      bottom: -3,
      right: -3,
      width: 20,
      height: 20,
      borderRadius: '50%',
      background: T.greenSolid,
      border: `2px solid ${T.cardFlat}`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "check",
    size: 12,
    color: "#0C1B10"
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 13.5,
      color: T.fg,
      lineHeight: 1.2
    }
  }, a.label), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 3
    }
  }, a.sub), !a.got && a.progress > 0 && /*#__PURE__*/React.createElement("div", {
    style: {
      height: 5,
      borderRadius: 999,
      background: T.cardHi,
      overflow: 'hidden',
      marginTop: 9
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: `${a.progress}%`,
      height: '100%',
      borderRadius: 999,
      background: T.greenGrad
    }
  }))))));
}
function SubscriptionsScreen() {
  const [sel, setSel] = React.useState('sub2');
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 15"
  }, "Assinaturas \u2014 sustente o territ\xF3rio que te sustenta. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '0 0 16px',
      fontFamily: T.sans,
      fontSize: 14,
      lineHeight: 1.5,
      color: T.fg2
    }
  }, "Apoio recorrente que mant\xE9m a infraestrutura comunit\xE1ria viva e independente."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 12
    }
  }, window.ARAH.subscriptionTiers.map(t => {
    const on = sel === t.id;
    return /*#__PURE__*/React.createElement(Card, {
      key: t.id,
      onClick: () => setSel(t.id),
      glow: on,
      style: {
        padding: 18,
        position: 'relative',
        border: `1.5px solid ${on ? T.green : T.line}`
      }
    }, t.popular && /*#__PURE__*/React.createElement("span", {
      style: {
        position: 'absolute',
        top: -10,
        right: 16,
        fontFamily: T.sans,
        fontSize: 10.5,
        fontWeight: 700,
        letterSpacing: 0.4,
        color: '#0C1B10',
        background: T.greenSolid,
        padding: '4px 11px',
        borderRadius: 999
      }
    }, "MAIS ESCOLHIDO"), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'baseline',
        gap: 8
      }
    }, /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.display,
        fontWeight: 700,
        fontSize: 18,
        color: T.fg
      }
    }, t.name), /*#__PURE__*/React.createElement("span", {
      style: {
        marginLeft: 'auto',
        fontFamily: T.display,
        fontWeight: 700,
        fontSize: 22,
        color: on ? T.green : T.fg
      }
    }, t.price), /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 13,
        color: T.fg3
      }
    }, t.period)), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        flexDirection: 'column',
        gap: 8,
        marginTop: 13
      }
    }, t.perks.map(p => /*#__PURE__*/React.createElement("div", {
      key: p,
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 9,
        fontFamily: T.sans,
        fontSize: 13.5,
        color: T.fg2
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "check_circle",
      size: 17,
      color: T.green,
      fill: 1
    }), " ", p))));
  })), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: "favorite",
    style: {
      marginTop: 18
    },
    onClick: () => window.openJourney('subscribe', window.ARAH.subscriptionTiers.find(t => t.id === sel))
  }, "Apoiar o territ\xF3rio"));
}
Object.assign(window, {
  AIScreen,
  AchievementsScreen,
  SubscriptionsScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens13.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens14.jsx
try { (() => {
// screens14.jsx — Journey framework: full-screen multi-step flows + shared field helpers.

// Full-screen journey overlay with progress, header and sticky footer CTA.
function JourneyShell({
  title,
  step,
  steps,
  onBack,
  onClose,
  footer,
  children
}) {
  const pct = Math.round((step + 1) / steps * 100);
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      zIndex: 300,
      background: `radial-gradient(130% 80% at 50% -8%, #15201A 0%, ${T.bg} 58%)`,
      display: 'flex',
      flexDirection: 'column',
      animation: 'sheetUp .32s cubic-bezier(.16,1,.3,1)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: 56,
      flexShrink: 0
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 12,
      padding: '6px 16px 12px'
    }
  }, /*#__PURE__*/React.createElement("button", {
    onClick: step === 0 ? onClose : onBack,
    style: iconBtn
  }, /*#__PURE__*/React.createElement(Icon, {
    name: step === 0 ? 'close' : 'arrow_back',
    size: 22,
    color: T.fg
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 17,
      color: T.fg,
      letterSpacing: -0.3,
      whiteSpace: 'nowrap',
      overflow: 'hidden',
      textOverflow: 'ellipsis'
    }
  }, title), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 1
    }
  }, "Passo ", step + 1, " de ", steps))), /*#__PURE__*/React.createElement("div", {
    style: {
      height: 4,
      background: T.cardHi,
      margin: '0 16px',
      borderRadius: 999,
      overflow: 'hidden',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: `${pct}%`,
      height: '100%',
      background: T.greenGrad,
      borderRadius: 999,
      transition: 'width .35s var(--ease, ease)'
    }
  })), /*#__PURE__*/React.createElement("div", {
    key: step,
    className: "appscroll screen-fade",
    style: {
      flex: 1,
      overflowY: 'auto',
      padding: '20px 18px 16px'
    }
  }, children), footer && /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '12px 18px 30px',
      borderTop: `1px solid ${T.line}`,
      background: T.bg2,
      flexShrink: 0
    }
  }, footer));
}

// Success / confirmation state
function SuccessView({
  icon = 'check_circle',
  title,
  msg,
  primaryLabel = 'Concluir',
  onPrimary,
  secondaryLabel,
  onSecondary,
  accent = T.green
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      zIndex: 300,
      background: `radial-gradient(130% 80% at 50% -8%, #15201A 0%, ${T.bg} 58%)`,
      display: 'flex',
      flexDirection: 'column',
      animation: 'fadeIn .3s ease'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      textAlign: 'center',
      padding: '0 36px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 88,
      height: 88,
      borderRadius: '50%',
      background: `${accent}1f`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      marginBottom: 22,
      animation: 'pop .4s cubic-bezier(.16,1.4,.5,1)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 48,
    color: accent,
    fill: 1
  })), /*#__PURE__*/React.createElement("h2", {
    style: {
      margin: 0,
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 24,
      color: T.fg,
      letterSpacing: -0.5
    }
  }, title), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '12px 0 0',
      fontFamily: T.sans,
      fontSize: 15,
      lineHeight: 1.55,
      color: T.fg2,
      maxWidth: 300
    }
  }, msg)), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '12px 18px 32px',
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: "check",
    onClick: onPrimary
  }, primaryLabel), secondaryLabel && /*#__PURE__*/React.createElement(Btn, {
    kind: "secondary",
    full: true,
    size: "md",
    onClick: onSecondary
  }, secondaryLabel)));
}

// ---- shared field helpers ----
function JField({
  label,
  hint,
  children
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      marginBottom: 18
    }
  }, label && /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      fontWeight: 600,
      color: T.fg2,
      marginBottom: 9,
      letterSpacing: 0.1
    }
  }, label), children, hint && /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 7
    }
  }, hint));
}
const jInput = {
  width: '100%',
  boxSizing: 'border-box',
  background: T.cardFlat,
  color: T.fg,
  border: `1px solid ${T.line}`,
  borderRadius: 14,
  padding: '13px 15px',
  fontFamily: T.sans,
  fontSize: 15,
  outline: 'none'
};
window.jInput = jInput;

// horizontal selectable pills
function PillSelect({
  options,
  value,
  onChange,
  cols
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: cols ? 'grid' : 'flex',
      gridTemplateColumns: cols ? `repeat(${cols},1fr)` : undefined,
      gap: 8,
      flexWrap: 'wrap'
    }
  }, options.map(o => {
    const v = typeof o === 'string' ? o : o.value;
    const lbl = typeof o === 'string' ? o : o.label;
    const on = value === v;
    return /*#__PURE__*/React.createElement("button", {
      key: v,
      onClick: () => onChange(v),
      style: {
        padding: '11px 14px',
        borderRadius: 13,
        cursor: 'pointer',
        WebkitTapHighlightColor: 'transparent',
        background: on ? T.greenDim : T.cardFlat,
        color: on ? T.green : T.fg2,
        border: `1px solid ${on ? 'transparent' : T.line}`,
        fontFamily: T.sans,
        fontSize: 14,
        fontWeight: 500,
        textAlign: 'center',
        transition: 'all .15s'
      }
    }, lbl);
  }));
}

// − N + counter
function Counter({
  value,
  onChange,
  min = 0,
  max = 20,
  suffix = ''
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 16,
      background: T.cardFlat,
      border: `1px solid ${T.line}`,
      borderRadius: 14,
      padding: '8px 14px',
      width: 'fit-content'
    }
  }, /*#__PURE__*/React.createElement("button", {
    onClick: () => onChange(Math.max(min, value - 1)),
    style: counterBtn
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "remove",
    size: 20,
    color: T.fg
  })), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 17,
      color: T.fg,
      minWidth: 28,
      textAlign: 'center'
    }
  }, value, suffix), /*#__PURE__*/React.createElement("button", {
    onClick: () => onChange(Math.min(max, value + 1)),
    style: counterBtn
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "add",
    size: 20,
    color: T.fg
  })));
}
const counterBtn = {
  width: 34,
  height: 34,
  borderRadius: 10,
  border: `1px solid ${T.line}`,
  background: T.cardHi,
  cursor: 'pointer',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  WebkitTapHighlightColor: 'transparent'
};

// review summary rows
function ReviewRow({
  icon,
  label,
  value,
  accent
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 12,
      padding: '13px 0',
      borderBottom: `1px solid ${T.line}`
    }
  }, icon && /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 19,
    color: T.fg3
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg2
    }
  }, label), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      fontWeight: 600,
      color: accent || T.fg,
      textAlign: 'right'
    }
  }, value));
}

// PIX payment block
function PixPay({
  amount
}) {
  const [copied, setCopied] = React.useState(false);
  return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      padding: '13px 15px',
      borderRadius: 14,
      background: 'rgba(111,197,214,0.1)',
      border: '1px solid rgba(111,197,214,0.22)',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "pix",
    size: 22,
    color: T.water,
    fill: 1
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      color: T.fg
    }
  }, "Pague com PIX"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, "Aprova\xE7\xE3o na hora \xB7 sem taxa entre moradores"))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      padding: '22px',
      borderRadius: 16,
      background: '#fff',
      marginBottom: 14
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "qr_code_2",
    size: 132,
    color: "#0E2117",
    fill: 0
  })), /*#__PURE__*/React.createElement("button", {
    onClick: () => {
      setCopied(true);
      setTimeout(() => setCopied(false), 1600);
    },
    style: {
      width: '100%',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 8,
      padding: '13px',
      borderRadius: 14,
      cursor: 'pointer',
      background: T.cardFlat,
      border: `1px solid ${T.line}`,
      color: copied ? T.green : T.fg,
      fontFamily: T.sans,
      fontSize: 14,
      fontWeight: 600,
      WebkitTapHighlightColor: 'transparent'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: copied ? 'check' : 'content_copy',
    size: 17
  }), " ", copied ? 'Código PIX copiado' : 'Copiar código PIX'), /*#__PURE__*/React.createElement("div", {
    style: {
      textAlign: 'center',
      marginTop: 14,
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 22,
      color: T.fg
    }
  }, amount));
}
function JStepTitle({
  children,
  sub
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement("h2", {
    style: {
      margin: 0,
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 21,
      color: T.fg,
      letterSpacing: -0.4
    }
  }, children), sub && /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '7px 0 0',
      fontFamily: T.sans,
      fontSize: 14,
      lineHeight: 1.5,
      color: T.fg2
    }
  }, sub));
}
Object.assign(window, {
  JourneyShell,
  SuccessView,
  JField,
  PillSelect,
  Counter,
  ReviewRow,
  PixPay,
  JStepTitle
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens14.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens15.jsx
try { (() => {
// screens15.jsx — Booking journeys: Reserva (hospedagem), Babá, Bem-estar, Checkout mercado.

function ReservaJourney({
  ctx,
  onClose
}) {
  const h = ctx || window.ARAH.hosting[0];
  const [step, setStep] = React.useState(0);
  const [date, setDate] = React.useState('Sex, 13/jun');
  const [nights, setNights] = React.useState(2);
  const [guests, setGuests] = React.useState(2);
  const [done, setDone] = React.useState(false);
  const total = `R$ ${parseInt(h.price.replace(/\D/g, '')) * nights}`;
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: "Reserva confirmada!",
    msg: `Sua estadia em "${h.title}" está reservada. ${h.host} já foi avisado e vai te receber.`,
    primaryLabel: "Ver minhas reservas",
    onPrimary: onClose,
    secondaryLabel: "Voltar",
    onSecondary: onClose
  });
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'lock' : 'arrow_forward',
    onClick: next
  }, step === 2 ? `Pagar ${total}` : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Reservar estadia",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: `${h.title} · ${h.host}`
  }, "Quando voc\xEA vem?"), /*#__PURE__*/React.createElement(JField, {
    label: "Data de entrada"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 2,
    value: date,
    onChange: setDate,
    options: ['Sex, 13/jun', 'Sáb, 14/jun', 'Sex, 20/jun', 'Sáb, 21/jun']
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Noites"
  }, /*#__PURE__*/React.createElement(Counter, {
    value: nights,
    onChange: setNights,
    min: 1,
    max: 30
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: `Até ${h.guests} hóspedes nesta acomodação`
  }, "Quantas pessoas?"), /*#__PURE__*/React.createElement(JField, {
    label: "H\xF3spedes"
  }, /*#__PURE__*/React.createElement(Counter, {
    value: guests,
    onChange: setGuests,
    min: 1,
    max: h.guests
  })), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 14,
      display: 'flex',
      gap: 11,
      alignItems: 'center',
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: h.avatar,
    name: h.host,
    size: 40,
    resident: true
  }), /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      color: T.fg
    }
  }, "Anfitri\xE3o: ", h.host), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, "Morador verificado \xB7 ", h.reviews, " avalia\xE7\xF5es")))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Revise e pague"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px',
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "cottage",
    label: h.title,
    value: h.tag
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "calendar_month",
    label: "Entrada",
    value: date
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "bedtime",
    label: "Noites",
    value: `${nights} × ${h.price}`
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "group",
    label: "H\xF3spedes",
    value: guests
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      padding: '14px 0 4px'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg
    }
  }, "Total"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.green
    }
  }, total))), /*#__PURE__*/React.createElement(PixPay, {
    amount: total
  })));
}
function SitterJourney({
  ctx,
  onClose
}) {
  const s = ctx || window.ARAH.sitters[0];
  const [step, setStep] = React.useState(0);
  const [day, setDay] = React.useState('Sáb, 14/jun');
  const [time, setTime] = React.useState('14:00');
  const [hours, setHours] = React.useState(4);
  const [kids, setKids] = React.useState(1);
  const [done, setDone] = React.useState(false);
  const total = `R$ ${parseInt(s.price.replace(/\D/g, '')) * hours}`;
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: "Solicita\xE7\xE3o enviada!",
    msg: `${s.name} recebeu seu pedido e vai confirmar em instantes. Você pode combinar os detalhes pelo chat.`,
    primaryLabel: "Abrir conversa",
    onPrimary: onClose,
    secondaryLabel: "Voltar",
    onSecondary: onClose
  });
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'send' : 'arrow_forward',
    onClick: next
  }, step === 2 ? 'Enviar solicitação' : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Contratar bab\xE1",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 12,
      alignItems: 'center',
      marginBottom: 20
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: s.avatar,
    name: s.name,
    size: 52,
    resident: true
  }), /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 17,
      color: T.fg
    }
  }, s.name), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4,
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "star",
    size: 14,
    color: "#F5C451",
    fill: 1
  }), " ", s.rating, " \xB7 ", s.price, " \xB7 ", s.ages))), /*#__PURE__*/React.createElement(JStepTitle, null, "Quando voc\xEA precisa?"), /*#__PURE__*/React.createElement(JField, {
    label: "Dia"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 2,
    value: day,
    onChange: setDay,
    options: ['Sex, 13/jun', 'Sáb, 14/jun', 'Dom, 15/jun', 'Seg, 16/jun']
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Hor\xE1rio de in\xEDcio"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    value: time,
    onChange: setTime,
    options: ['08:00', '14:00', '18:00', '20:00']
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Para a bab\xE1 se preparar melhor"
  }, "Detalhes do cuidado"), /*#__PURE__*/React.createElement(JField, {
    label: "Dura\xE7\xE3o (horas)"
  }, /*#__PURE__*/React.createElement(Counter, {
    value: hours,
    onChange: setHours,
    min: 1,
    max: 12,
    suffix: "h"
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Quantas crian\xE7as"
  }, /*#__PURE__*/React.createElement(Counter, {
    value: kids,
    onChange: setKids,
    min: 1,
    max: 6
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Observa\xE7\xF5es"
  }, /*#__PURE__*/React.createElement("textarea", {
    placeholder: "Idades, rotina, alergias, necessidades especiais\u2026",
    rows: 3,
    style: {
      ...jInput,
      resize: 'none',
      lineHeight: 1.5
    }
  }))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Revise o pedido"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "person",
    label: "Bab\xE1",
    value: s.name
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "calendar_month",
    label: "Quando",
    value: `${day} · ${time}`
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "schedule",
    label: "Dura\xE7\xE3o",
    value: `${hours}h`
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "child_care",
    label: "Crian\xE7as",
    value: kids
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      padding: '14px 0 4px'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg
    }
  }, "Estimativa"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.green
    }
  }, total))), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      display: 'flex',
      gap: 10,
      alignItems: 'center',
      background: 'rgba(166,214,185,0.08)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "verified_user",
    size: 19,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, "Pagamento liberado s\xF3 ap\xF3s o servi\xE7o, pela plataforma."))));
}
function WellnessJourney({
  ctx,
  onClose
}) {
  const [step, setStep] = React.useState(0);
  const [service, setService] = React.useState('Yoga em grupo');
  const [slot, setSlot] = React.useState('Qua · 07:00');
  const [done, setDone] = React.useState(false);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: "Sess\xE3o agendada!",
    msg: "Seu hor\xE1rio est\xE1 reservado. Voc\xEA recebeu a confirma\xE7\xE3o e um lembrete ser\xE1 enviado no dia.",
    primaryLabel: "Ver agenda",
    onPrimary: onClose,
    secondaryLabel: "Voltar",
    onSecondary: onClose
  });
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 1 ? 'event_available' : 'arrow_forward',
    onClick: next
  }, step === 1 ? 'Confirmar agendamento' : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Agendar bem-estar",
    step: step,
    steps: 2,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: ctx?.name || 'Espaço Maré Yoga'
  }, "Escolha o servi\xE7o"), /*#__PURE__*/React.createElement(JField, null, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 2,
    value: service,
    onChange: setService,
    options: ['Yoga em grupo', 'Yoga individual', 'Terapia sonora', 'Meditação']
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Hor\xE1rios dispon\xEDveis"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 2,
    value: slot,
    onChange: setSlot,
    options: ['Qua · 07:00', 'Qua · 18:00', 'Sex · 07:00', 'Sáb · 09:00']
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Confirme"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "self_improvement",
    label: "Servi\xE7o",
    value: service
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "spa",
    label: "Local",
    value: ctx?.name || 'Espaço Maré Yoga'
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "schedule",
    label: "Hor\xE1rio",
    value: slot
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "payments",
    label: "Valor",
    value: "R$ 60",
    accent: T.green
  }))));
}
function CheckoutJourney({
  ctx,
  onClose
}) {
  const item = ctx || {
    title: 'Robalo fresco (kg)',
    price: 'R$ 42',
    seller: 'Peixaria da Marta',
    avatar: '#C9962B',
    icon: 'set_meal'
  };
  const [step, setStep] = React.useState(0);
  const [qty, setQty] = React.useState(1);
  const [fulfil, setFulfil] = React.useState('Retirar no local');
  const [done, setDone] = React.useState(false);
  const unit = parseInt(item.price.replace(/\D/g, ''));
  const delivery = fulfil === 'Entrega local' ? 8 : 0;
  const total = `R$ ${unit * qty + delivery}`;
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: "Pedido confirmado!",
    msg: `${item.seller} recebeu seu pedido. ${fulfil === 'Entrega local' ? 'Um entregador da vila vai levar até você.' : 'É só retirar no local quando quiser.'}`,
    primaryLabel: "Acompanhar pedido",
    onPrimary: onClose,
    secondaryLabel: "Voltar ao mercado",
    onSecondary: onClose
  });
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'lock' : 'arrow_forward',
    onClick: next
  }, step === 2 ? `Pagar ${total}` : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Finalizar compra",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Sua sacola"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 14,
      display: 'flex',
      gap: 12,
      alignItems: 'center',
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 48,
      height: 48,
      borderRadius: 12,
      background: `${item.avatar}22`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: item.icon || 'shopping_basket',
    size: 24,
    color: T.green
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15,
      color: T.fg
    }
  }, item.title), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3
    }
  }, item.seller, " \xB7 ", item.price))), /*#__PURE__*/React.createElement(JField, {
    label: "Quantidade"
  }, /*#__PURE__*/React.createElement(Counter, {
    value: qty,
    onChange: setQty,
    min: 1,
    max: 20
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Como voc\xEA quer receber?"), /*#__PURE__*/React.createElement(JField, null, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 1,
    value: fulfil,
    onChange: setFulfil,
    options: ['Retirar no local', 'Entrega local']
  })), fulfil === 'Entrega local' && /*#__PURE__*/React.createElement(JField, {
    label: "Endere\xE7o"
  }, /*#__PURE__*/React.createElement("input", {
    defaultValue: "Rua das Conchas, 12 \u2014 Camburi",
    style: jInput
  })), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      display: 'flex',
      gap: 10,
      alignItems: 'center',
      background: 'rgba(166,214,185,0.08)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: fulfil === 'Entrega local' ? 'local_shipping' : 'storefront',
    size: 19,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2
    }
  }, fulfil === 'Entrega local' ? 'Entrega por um vizinho · R$ 8' : 'Retirada gratuita com o vendedor'))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Pagamento"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px',
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    label: `${item.title} × ${qty}`,
    value: `R$ ${unit * qty}`
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    label: "Entrega",
    value: delivery ? `R$ ${delivery}` : 'Grátis'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      padding: '14px 0 4px'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg
    }
  }, "Total"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.green
    }
  }, total))), /*#__PURE__*/React.createElement(PixPay, {
    amount: total
  })));
}
Object.assign(window, {
  ReservaJourney,
  SitterJourney,
  WellnessJourney,
  CheckoutJourney
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens15.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens16.jsx
try { (() => {
// screens16.jsx — Transaction/create journeys + JourneyHost router.

function WalletSendJourney({
  onClose
}) {
  const [step, setStep] = React.useState(0);
  const [amount, setAmount] = React.useState('80');
  const [to, setTo] = React.useState(null);
  const [done, setDone] = React.useState(false);
  const people = [{
    n: 'Dona Marta',
    a: '#C9962B'
  }, {
    n: 'Bruno Caiçara',
    a: '#377B57'
  }, {
    n: 'Seu Tião',
    a: '#4F956F'
  }, {
    n: 'Coletivo Maré',
    a: '#2A6FDB'
  }];
  const confirm = () => {
    window.arahMutate && window.arahMutate(() => {
      const w = window.ARAH.wallet;
      const cur = parseInt(String(w.balance).replace(/\D/g, '')) || 0;
      const n = Math.max(0, cur - parseInt(amount));
      w.balance = 'A̧ ' + n;
      w.brl = 'R$ ' + n + ',00';
      w.transactions.unshift({
        id: 'ws' + Date.now(),
        label: to?.n || 'Envio',
        sub: 'Transferência enviada',
        val: '- A̧ ' + amount,
        icon: 'north_east',
        neg: true
      });
    });
    setDone(true);
  };
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: "Arat\xE1 enviado!",
    msg: `A̧ ${amount} foram transferidos para ${to?.n}. A moeda continua circulando em Camburi.`,
    primaryLabel: "Ver carteira",
    onPrimary: onClose
  });
  const next = () => step < 2 ? setStep(step + 1) : confirm();
  const canNext = step === 1 ? !!to : true;
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'north_east' : 'arrow_forward',
    onClick: () => canNext && next(),
    style: {
      opacity: canNext ? 1 : 0.5
    }
  }, step === 2 ? 'Confirmar envio' : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Enviar Arat\xE1",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: `Saldo disponível: ${window.ARAH.wallet.balance}`
  }, "Quanto enviar?"), /*#__PURE__*/React.createElement("div", {
    style: {
      textAlign: 'center',
      padding: '20px 0'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 52,
      color: T.fg,
      letterSpacing: -2
    }
  }, "A\u0327 ", amount || '0')), /*#__PURE__*/React.createElement(PillSelect, {
    cols: 4,
    value: amount,
    onChange: setAmount,
    options: ['20', '50', '80', '120']
  })), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Para quem?"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 9
    }
  }, people.map(p => {
    const on = to?.n === p.n;
    return /*#__PURE__*/React.createElement(Card, {
      key: p.n,
      onClick: () => setTo(p),
      hi: on,
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 12,
        padding: 12,
        border: `1px solid ${on ? T.green : T.line}`
      }
    }, /*#__PURE__*/React.createElement(Avatar, {
      color: p.a,
      name: p.n,
      size: 40,
      resident: true
    }), /*#__PURE__*/React.createElement("span", {
      style: {
        flex: 1,
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 15,
        color: T.fg
      }
    }, p.n), /*#__PURE__*/React.createElement(Icon, {
      name: on ? 'check_circle' : 'radio_button_unchecked',
      size: 21,
      color: on ? T.green : T.fg3,
      fill: on ? 1 : 0
    }));
  }))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Confirme o envio"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 22,
      textAlign: 'center',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 40,
      color: T.green,
      letterSpacing: -1
    }
  }, "A\u0327 ", amount), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg3,
      margin: '6px 0 16px'
    }
  }, "\u2248 R$ ", amount, ",00"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: to?.a,
    name: to?.n || '',
    size: 32,
    resident: true
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg
    }
  }, "para ", /*#__PURE__*/React.createElement("strong", {
    style: {
      fontWeight: 600
    }
  }, to?.n))))));
}
function SubscriptionJourney({
  ctx,
  onClose
}) {
  const tier = ctx || window.ARAH.subscriptionTiers[1];
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "favorite",
    title: "Voc\xEA \xE9 Guardi\xE3o!",
    msg: `Obrigado por sustentar Camburi. Sua assinatura ${tier.name} mantém a infraestrutura comunitária viva e independente.`,
    primaryLabel: "Concluir",
    onPrimary: onClose
  });
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 1 ? 'lock' : 'arrow_forward',
    onClick: next
  }, step === 1 ? `Assinar · ${tier.price}${tier.period}` : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Apoiar o territ\xF3rio",
    step: step,
    steps: 2,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Apoio recorrente ao territ\xF3rio"
  }, "Plano ", tier.name), /*#__PURE__*/React.createElement(Card, {
    glow: true,
    style: {
      padding: 18
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'baseline'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 18,
      color: T.fg
    }
  }, tier.name), /*#__PURE__*/React.createElement("span", {
    style: {
      marginLeft: 'auto',
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 24,
      color: T.green
    }
  }, tier.price), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg3
    }
  }, tier.period)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 9,
      marginTop: 15
    }
  }, tier.perks.map(p => /*#__PURE__*/React.createElement("div", {
    key: p,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      fontFamily: T.sans,
      fontSize: 13.5,
      color: T.fg2
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "check_circle",
    size: 17,
    color: T.green,
    fill: 1
  }), " ", p))))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Pagamento"), /*#__PURE__*/React.createElement(PixPay, {
    amount: `${tier.price}${tier.period}`
  })));
}
function ResidenciaJourney({
  onClose,
  onApprove
}) {
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  const [proof, setProof] = React.useState(false);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "how_to_reg",
    title: "Solicita\xE7\xE3o enviada!",
    msg: "A curadoria do territ\xF3rio vai analisar seu comprovante. Voc\xEA ser\xE1 avisado assim que sua resid\xEAncia for confirmada.",
    primaryLabel: "Entendi",
    onPrimary: () => {
      onApprove && onApprove();
      onClose();
    }
  });
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'send' : 'arrow_forward',
    onClick: next
  }, step === 2 ? 'Enviar solicitação' : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Confirmar resid\xEAncia",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Moradores acessam conte\xFAdo restrito, votam e participam da gest\xE3o."
  }, "Torne-se morador de Camburi"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 14,
      display: 'flex',
      gap: 11,
      alignItems: 'center',
      marginBottom: 11,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "my_location",
    size: 20,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      color: T.fg
    }
  }, "Presen\xE7a no territ\xF3rio"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, "Confirmada por GPS \xB7 privada")), /*#__PURE__*/React.createElement(Icon, {
    name: "check_circle",
    size: 20,
    color: T.green,
    fill: 1
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Conta de luz, \xE1gua, contrato ou declara\xE7\xE3o da associa\xE7\xE3o."
  }, "Envie um comprovante"), /*#__PURE__*/React.createElement("button", {
    onClick: () => setProof(true),
    style: {
      width: '100%',
      padding: '30px',
      borderRadius: 16,
      cursor: 'pointer',
      background: T.cardFlat,
      border: `1.5px dashed ${proof ? T.green : T.lineHi}`,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      gap: 10,
      WebkitTapHighlightColor: 'transparent'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: proof ? 'check_circle' : 'upload_file',
    size: 34,
    color: proof ? T.green : T.fg2,
    fill: proof ? 1 : 0
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      fontWeight: 500,
      color: proof ? T.green : T.fg2
    }
  }, proof ? 'comprovante-residencia.pdf' : 'Anexar comprovante'))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Revise"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "forest",
    label: "Territ\xF3rio",
    value: "Camburi"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "my_location",
    label: "Presen\xE7a GPS",
    value: "Confirmada",
    accent: T.green
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "description",
    label: "Comprovante",
    value: "Anexado",
    accent: T.green
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "shield_person",
    label: "An\xE1lise",
    value: "Curadoria local"
  }))));
}

// Generic "create / propose" journey (demanda, oferta, semente, troca)
function CreateJourney({
  ctx,
  onClose
}) {
  const cfg = ctx || {};
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  const [kind, setKind] = React.useState(cfg.kinds ? cfg.kinds[0] : null);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: cfg.successTitle,
    msg: cfg.successMsg,
    primaryLabel: "Ver no territ\xF3rio",
    onPrimary: onClose,
    secondaryLabel: "Voltar",
    onSecondary: onClose
  });
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 1 ? 'send' : 'arrow_forward',
    onClick: next
  }, step === 1 ? cfg.cta : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: cfg.title,
    step: step,
    steps: 2,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: cfg.sub
  }, cfg.heading), cfg.kinds && /*#__PURE__*/React.createElement(JField, {
    label: cfg.kindLabel
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: cfg.kinds.length,
    value: kind,
    onChange: setKind,
    options: cfg.kinds
  })), /*#__PURE__*/React.createElement(JField, {
    label: cfg.f1
  }, /*#__PURE__*/React.createElement("input", {
    placeholder: cfg.f1ph,
    style: jInput
  })), cfg.f2 && /*#__PURE__*/React.createElement(JField, {
    label: cfg.f2
  }, /*#__PURE__*/React.createElement("input", {
    placeholder: cfg.f2ph,
    style: jInput
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Descri\xE7\xE3o"
  }, /*#__PURE__*/React.createElement("textarea", {
    placeholder: cfg.descPh,
    rows: 3,
    style: {
      ...jInput,
      resize: 'none',
      lineHeight: 1.5
    }
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Confira antes de publicar no territ\xF3rio."
  }, "Tudo certo?"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, kind && /*#__PURE__*/React.createElement(ReviewRow, {
    icon: cfg.icon,
    label: cfg.kindLabel,
    value: kind,
    accent: T.green
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "forest",
    label: "Territ\xF3rio",
    value: "Camburi"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "visibility",
    label: "Vis\xEDvel para",
    value: "Moradores e visitantes"
  })), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      display: 'flex',
      gap: 10,
      alignItems: 'center',
      background: 'rgba(166,214,185,0.08)',
      marginTop: 14
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "groups",
    size: 19,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, cfg.note))));
}

// Course enrollment (single confirm)
function CourseJourney({
  ctx,
  onClose
}) {
  const c = ctx || window.ARAH.courses[0];
  const [done, setDone] = React.useState(false);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "school",
    title: "Matr\xEDcula feita!",
    msg: `Você entrou em "${c.title}". ${c.teacher} vai te avisar quando a próxima oficina começar.`,
    primaryLabel: "Concluir",
    onPrimary: onClose
  });
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Participar da oficina",
    step: 0,
    steps: 1,
    onClose: onClose,
    footer: /*#__PURE__*/React.createElement(Btn, {
      kind: "primary",
      full: true,
      size: "lg",
      icon: "how_to_reg",
      onClick: () => setDone(true)
    }, "Confirmar participa\xE7\xE3o")
  }, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: c.tag
  }, c.title), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "person",
    label: "Quem ensina",
    value: c.teacher
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "play_lesson",
    label: "Aulas",
    value: `${c.lessons} encontros`
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "signal_cellular_alt",
    label: "N\xEDvel",
    value: c.level
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "payments",
    label: "Valor",
    value: "Gratuito \xB7 saber comunit\xE1rio",
    accent: T.green
  })));
}

// ---- Router ----
function JourneyHost({
  journey,
  onClose,
  onApprove
}) {
  if (!journey) return null;
  const {
    id,
    ctx
  } = journey;
  switch (id) {
    case 'reserva':
      return /*#__PURE__*/React.createElement(ReservaJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'sitter':
      return /*#__PURE__*/React.createElement(SitterJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'wellness':
      return /*#__PURE__*/React.createElement(WellnessJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'checkout':
      return /*#__PURE__*/React.createElement(CheckoutJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'walletSend':
      return /*#__PURE__*/React.createElement(WalletSendJourney, {
        onClose: onClose
      });
    case 'subscribe':
      return /*#__PURE__*/React.createElement(SubscriptionJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'residencia':
      return /*#__PURE__*/React.createElement(ResidenciaJourney, {
        onClose: onClose,
        onApprove: onApprove
      });
    case 'course':
      return /*#__PURE__*/React.createElement(CourseJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'rental':
      return /*#__PURE__*/React.createElement(RentalJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'deliveryReq':
      return /*#__PURE__*/React.createElement(DeliveryRequestJourney, {
        onClose: onClose
      });
    case 'vote':
      return /*#__PURE__*/React.createElement(VoteJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'groupBuy':
      return /*#__PURE__*/React.createElement(GroupBuyJoinJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'addProduct':
      return /*#__PURE__*/React.createElement(AddProductJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'walletReceive':
      return /*#__PURE__*/React.createElement(WalletReceiveJourney, {
        onClose: onClose
      });
    case 'walletTopup':
      return /*#__PURE__*/React.createElement(WalletTopupJourney, {
        onClose: onClose
      });
    case 'joinEvent':
      return /*#__PURE__*/React.createElement(JoinEventJourney, {
        ctx: ctx,
        onClose: onClose
      });
    case 'createElection':
      return /*#__PURE__*/React.createElement(CreateElectionJourney, {
        onClose: onClose
      });
    case 'create':
      return /*#__PURE__*/React.createElement(CreateJourney, {
        ctx: ctx,
        onClose: onClose
      });
    default:
      return null;
  }
}

// Context presets for the generic create journey
const JOURNEY_PRESETS = {
  demanda: {
    title: 'Nova demanda ou oferta',
    heading: 'O que você quer divulgar?',
    sub: 'Peça uma mão ou ofereça o que você faz.',
    icon: 'swap_horiz',
    kindLabel: 'Tipo',
    kinds: ['Demanda', 'Oferta'],
    f1: 'Título',
    f1ph: 'Ex.: Preciso de pedreiro',
    f2: 'Categoria',
    f2ph: 'Construção, transporte…',
    descPh: 'Detalhe o que precisa ou oferece…',
    cta: 'Publicar',
    successTitle: 'Publicado!',
    successMsg: 'Sua demanda está no território. Vizinhos podem responder pelo chat.',
    note: 'Quem puder ajudar fala com você pelo chat do território.'
  },
  semente: {
    title: 'Doar muda ou semente',
    heading: 'O que você quer compartilhar?',
    sub: 'Fortaleça a regeneração de Camburi.',
    icon: 'potted_plant',
    f1: 'Espécie',
    f1ph: 'Ex.: Ipê-amarelo',
    f2: 'Quantidade',
    f2ph: 'Ex.: 20 sementes',
    descPh: 'Origem, época de plantio, cuidados…',
    cta: 'Doar ao banco',
    successTitle: 'Doação registrada!',
    successMsg: 'Sua espécie entrou no banco de sementes do território. Outros moradores já podem solicitar.',
    note: 'A muda fica disponível para troca com outros moradores.'
  },
  troca: {
    title: 'Propor troca',
    heading: 'O que você oferece?',
    sub: 'Escambo e doações entre vizinhos.',
    icon: 'handshake',
    f1: 'Ofereço',
    f1ph: 'Ex.: Mudas de ipê',
    f2: 'Quero em troca',
    f2ph: 'Ex.: Adubo orgânico',
    descPh: 'Detalhes da troca…',
    cta: 'Publicar troca',
    successTitle: 'Troca publicada!',
    successMsg: 'Sua proposta está no território. Quem topar fala com você pelo chat.',
    note: 'Combine os detalhes da troca diretamente pelo chat.'
  },
  digital: {
    title: 'Contratar serviço digital',
    heading: 'O que você precisa?',
    sub: 'Talentos digitais do território.',
    icon: 'apps',
    f1: 'Serviço',
    f1ph: 'Ex.: Logo para minha loja',
    f2: 'Prazo',
    f2ph: 'Ex.: 2 semanas',
    descPh: 'Descreva o que você precisa…',
    cta: 'Enviar pedido',
    successTitle: 'Pedido enviado!',
    successMsg: 'O profissional do território vai responder com uma proposta pelo chat.',
    note: 'A proposta e o pagamento são combinados pelo chat do território.'
  }
};
window.JOURNEY_PRESETS = JOURNEY_PRESETS;
Object.assign(window, {
  WalletSendJourney,
  SubscriptionJourney,
  ResidenciaJourney,
  CreateJourney,
  CourseJourney,
  JourneyHost
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens16.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens17.jsx
try { (() => {
// screens17.jsx — Aluguéis, Hub digital (screens) + Rental/Delivery journeys.

function RentalScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 46"
  }, "Alugu\xE9is \u2014 equipamentos e espa\xE7os entre vizinhos. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 11
    }
  }, window.ARAH.rentals.map(r => /*#__PURE__*/React.createElement(Card, {
    key: r.id,
    glow: true,
    onClick: () => window.openJourney('rental', r),
    style: {
      padding: 0,
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: 84,
      background: `linear-gradient(150deg, ${r.avatar}40, ${r.avatar}12)`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      position: 'relative'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: r.icon,
    size: 32,
    color: T.fg,
    fill: 0,
    style: {
      opacity: 0.9
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: 8,
      left: 8,
      fontFamily: T.sans,
      fontSize: 10,
      fontWeight: 600,
      color: T.fg,
      background: 'rgba(11,12,10,0.5)',
      backdropFilter: 'blur(4px)',
      padding: '3px 8px',
      borderRadius: 999
    }
  }, r.tag)), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 12
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14,
      color: T.fg,
      lineHeight: 1.25,
      letterSpacing: -0.2
    }
  }, r.title), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 5,
      marginTop: 6,
      color: T.fg3,
      fontFamily: T.sans,
      fontSize: 11.5
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: r.avatar,
    name: r.owner,
    size: 16,
    resident: true
  }), " ", r.owner), /*#__PURE__*/React.createElement("div", {
    style: {
      marginTop: 9
    }
  }, /*#__PURE__*/React.createElement("strong", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 15,
      color: T.green
    }
  }, r.price), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, " / ", r.unit)))))));
}
function DigitalScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 26"
  }, "Hub de servi\xE7os digitais \u2014 talentos do territ\xF3rio para o territ\xF3rio. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 11
    }
  }, window.ARAH.digitalServices.map(s => /*#__PURE__*/React.createElement(Card, {
    key: s.id,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: 14
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 46,
      height: 46,
      borderRadius: 13,
      background: T.greenDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: s.icon,
    size: 23,
    color: T.green,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 10,
      fontWeight: 700,
      letterSpacing: 0.6,
      textTransform: 'uppercase',
      color: T.green
    }
  }, s.tag), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14.5,
      color: T.fg,
      marginTop: 2,
      letterSpacing: -0.2,
      lineHeight: 1.2
    }
  }, s.title), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      marginTop: 5
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: s.avatar,
    name: s.who,
    size: 18
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, s.who, " \xB7 ", s.price))), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "bookmark_add",
    style: {
      flexShrink: 0
    },
    onClick: () => window.openJourney('create', window.JOURNEY_PRESETS.digital)
  }, "Contratar")))));
}

// ---- Rental journey: item → período → PIX → success ----
function RentalJourney({
  ctx,
  onClose
}) {
  const r = ctx || window.ARAH.rentals[0];
  const [step, setStep] = React.useState(0);
  const [from, setFrom] = React.useState('Sex, 13/jun');
  const [days, setDays] = React.useState(2);
  const [done, setDone] = React.useState(false);
  const unit = parseInt(r.price.replace(/\D/g, ''));
  const total = `R$ ${unit * days}`;
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: "Aluguel confirmado!",
    msg: `Você reservou "${r.title}" com ${r.owner}. Combine a retirada pelo chat do território.`,
    primaryLabel: "Abrir conversa",
    onPrimary: onClose,
    secondaryLabel: "Voltar",
    onSecondary: onClose
  });
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'lock' : 'arrow_forward',
    onClick: next
  }, step === 2 ? `Pagar ${total}` : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Alugar item",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 14,
      display: 'flex',
      gap: 12,
      alignItems: 'center',
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 48,
      height: 48,
      borderRadius: 12,
      background: `${r.avatar}22`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: r.icon,
    size: 24,
    color: T.green
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15,
      color: T.fg
    }
  }, r.title), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3
    }
  }, r.owner, " \xB7 ", r.price, "/", r.unit))), /*#__PURE__*/React.createElement(JStepTitle, null, "Quando voc\xEA precisa?"), /*#__PURE__*/React.createElement(JField, {
    label: "In\xEDcio"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 2,
    value: from,
    onChange: setFrom,
    options: ['Sex, 13/jun', 'Sáb, 14/jun', 'Dom, 15/jun', 'Seg, 16/jun']
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Dias"
  }, /*#__PURE__*/React.createElement(Counter, {
    value: days,
    onChange: setDays,
    min: 1,
    max: 30
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Combine a retirada com o dono pelo chat."
  }, "Retirada"), /*#__PURE__*/React.createElement(JField, null, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 1,
    value: 'Retirar com o dono',
    onChange: () => {},
    options: ['Retirar com o dono']
  })), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      display: 'flex',
      gap: 10,
      alignItems: 'center',
      background: 'rgba(166,214,185,0.08)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "verified_user",
    size: 19,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, "Cau\xE7\xE3o opcional combinada entre vizinhos do territ\xF3rio."))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Revise e pague"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px',
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "category",
    label: r.title,
    value: r.tag
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "calendar_month",
    label: "In\xEDcio",
    value: from
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "schedule",
    label: "Per\xEDodo",
    value: `${days} ${days > 1 ? 'dias' : 'dia'} × ${r.price}`
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      padding: '14px 0 4px'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg
    }
  }, "Total"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.green
    }
  }, total))), /*#__PURE__*/React.createElement(PixPay, {
    amount: total
  })));
}

// ---- Delivery request journey: item → endereços → confirmar → success ----
function DeliveryRequestJourney({
  onClose
}) {
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "local_shipping",
    title: "Entrega solicitada!",
    msg: "Um entregador da vila vai aceitar e levar seu pedido. Voc\xEA acompanha o trajeto em tempo real.",
    primaryLabel: "Acompanhar",
    onPrimary: onClose,
    secondaryLabel: "Voltar",
    onSecondary: onClose
  });
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 1 ? 'send' : 'arrow_forward',
    onClick: next
  }, step === 1 ? 'Solicitar entrega' : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Solicitar entrega",
    step: step,
    steps: 2,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Um vizinho leva pra voc\xEA."
  }, "O que vai ser entregue?"), /*#__PURE__*/React.createElement(JField, {
    label: "Item / pedido"
  }, /*#__PURE__*/React.createElement("input", {
    placeholder: "Ex.: 2kg de peixe da Peixaria da Marta",
    style: jInput
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Retirar em"
  }, /*#__PURE__*/React.createElement("input", {
    defaultValue: "Peixaria da Marta",
    style: jInput
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Entregar em"
  }, /*#__PURE__*/React.createElement("input", {
    defaultValue: "Rua das Conchas, 12 \u2014 Camburi",
    style: jInput
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Confirme"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "inventory_2",
    label: "Pedido",
    value: "Peixe fresco"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "storefront",
    label: "Retirada",
    value: "Peixaria da Marta"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "home",
    label: "Entrega",
    value: "Rua das Conchas, 12"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "payments",
    label: "Frete local",
    value: "R$ 8",
    accent: T.green
  })), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      display: 'flex',
      gap: 10,
      alignItems: 'center',
      background: 'rgba(166,214,185,0.08)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "two_wheeler",
    size: 19,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, "Entregadores s\xE3o moradores do territ\xF3rio \u2014 renda que fica na vila."))));
}
Object.assign(window, {
  RentalScreen,
  DigitalScreen,
  RentalJourney,
  DeliveryRequestJourney
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens17.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens18.jsx
try { (() => {
// screens18.jsx — Missing journeys: voto, compra coletiva, produto, carteira (receber/adicionar),
// participar de evento, abrir eleição (curador). Reuses the JourneyShell framework.

// ---- Votar em eleição ----
function VoteJourney({
  ctx,
  onClose
}) {
  const c = ctx?.candidate || {
    name: 'Dona Marta',
    avatar: '#C9962B',
    pitch: 'Pesca artesanal e feira'
  };
  const el = ctx?.election || {
    title: 'Conselho de Moradores 2026'
  };
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  const protocolo = 'ARH-' + Math.random().toString(36).slice(2, 8).toUpperCase();
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "how_to_vote",
    title: "Voto registrado!",
    msg: `Seu voto em ${c.name} foi computado de forma secreta e auditável. Protocolo ${protocolo}.`,
    primaryLabel: "Ver resultados parciais",
    onPrimary: () => {
      ctx?.onConfirm && ctx.onConfirm();
      onClose();
    },
    secondaryLabel: "Fechar",
    onSecondary: onClose
  });
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 1 ? 'how_to_vote' : 'arrow_forward',
    onClick: () => step < 1 ? setStep(1) : setDone(true)
  }, step === 1 ? 'Confirmar voto' : 'Revisar voto');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Votar",
    step: step,
    steps: 2,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: el.title
  }, "Confira seu candidato"), /*#__PURE__*/React.createElement(Card, {
    glow: true,
    style: {
      padding: 18,
      display: 'flex',
      gap: 14,
      alignItems: 'center',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: c.avatar,
    name: c.name,
    size: 56,
    resident: true
  }), /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 17,
      color: T.fg
    }
  }, c.name), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg2,
      marginTop: 2
    }
  }, c.pitch))), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      display: 'flex',
      gap: 10,
      alignItems: 'center',
      background: 'rgba(166,214,185,0.08)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "lock",
    size: 19,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, "Seu voto \xE9 secreto. Cada morador vota uma vez por elei\xE7\xE3o."))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Esta a\xE7\xE3o n\xE3o pode ser desfeita."
  }, "Confirmar voto"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "how_to_vote",
    label: "Elei\xE7\xE3o",
    value: el.title
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "person",
    label: "Candidato",
    value: c.name,
    accent: T.green
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "verified",
    label: "Valida\xE7\xE3o",
    value: "1 morador \xB7 1 voto"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "lock",
    label: "Sigilo",
    value: "Voto secreto"
  }))));
}

// ---- Entrar na compra coletiva (com pagamento) ----
function GroupBuyJoinJourney({
  ctx,
  onClose
}) {
  const g = ctx || window.ARAH.groupBuyDetail || {
    title: 'Compra coletiva',
    icon: 'groups_2',
    price: 'R$ 2.400',
    saved: 'R$ 1.100'
  };
  const [step, setStep] = React.useState(0);
  const [qty, setQty] = React.useState(1);
  const [done, setDone] = React.useState(false);
  const unit = parseInt((g.price || 'R$ 2400').replace(/\D/g, '')) || 2400;
  const total = `R$ ${(unit * qty).toLocaleString('pt-BR')}`;
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "groups_2",
    title: "Voc\xEA entrou!",
    msg: `Sua reserva na "${g.title}" está confirmada. Quando a meta for atingida, o pedido é fechado e você é avisado.`,
    primaryLabel: "Acompanhar",
    onPrimary: () => {
      ctx?.onConfirm && ctx.onConfirm();
      onClose();
    },
    secondaryLabel: "Fechar",
    onSecondary: onClose
  });
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'lock' : 'arrow_forward',
    onClick: next
  }, step === 2 ? `Reservar ${total}` : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Entrar na compra coletiva",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: g.title
  }, "Quantas unidades?"), /*#__PURE__*/React.createElement(JField, {
    label: "Sua reserva"
  }, /*#__PURE__*/React.createElement(Counter, {
    value: qty,
    onChange: setQty,
    min: 1,
    max: 5
  })), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      display: 'flex',
      gap: 10,
      alignItems: 'center',
      background: 'rgba(166,214,185,0.08)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "savings",
    size: 19,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2
    }
  }, "Economia estimada de ", /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.green,
      fontWeight: 700
    }
  }, g.saved), " por unidade na meta."))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Como funciona"
  }, "Pagamento garantido"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 16
    }
  }, ['O valor fica reservado até a meta ser atingida.', 'Se a meta não for batida no prazo, você é reembolsado.', 'Atingida a meta, o pedido é fechado com o fornecedor.'].map((t, i) => /*#__PURE__*/React.createElement("div", {
    key: i,
    style: {
      display: 'flex',
      gap: 11,
      alignItems: 'flex-start',
      padding: '9px 0'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "check_circle",
    size: 18,
    color: T.green,
    fill: 1,
    style: {
      marginTop: 1
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg2,
      lineHeight: 1.45
    }
  }, t))))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Reserve sua vaga"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px',
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    label: `Unidades`,
    value: qty
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    label: "Pre\xE7o na meta",
    value: g.price
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      padding: '14px 0 4px'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg
    }
  }, "Total reservado"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.green
    }
  }, total))), /*#__PURE__*/React.createElement(PixPay, {
    amount: total
  })));
}

// ---- Adicionar produto à loja ----
function AddProductJourney({
  ctx,
  onClose
}) {
  const [step, setStep] = React.useState(0);
  const [name, setName] = React.useState('');
  const [price, setPrice] = React.useState('');
  const [cat, setCat] = React.useState('Alimento');
  const [avail, setAvail] = React.useState('Disponível');
  const [photo, setPhoto] = React.useState(false);
  const [done, setDone] = React.useState(false);
  const iconFor = {
    Alimento: 'lunch_dining',
    Serviço: 'handyman',
    Artesanato: 'redeem',
    Aluguel: 'category'
  };
  const publish = () => {
    const prod = {
      id: 'sp' + Date.now(),
      title: name || 'Novo produto',
      price: price ? price.startsWith('R$') ? price : 'R$ ' + price : 'R$ 0',
      tag: cat,
      icon: iconFor[cat] || 'sell',
      active: true
    };
    ctx?.onConfirm && ctx.onConfirm(prod);
    setDone(true);
  };
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "storefront",
    title: "Produto publicado!",
    msg: "Seu item j\xE1 aparece no mercado do territ\xF3rio. Vizinhos podem comprar e falar com voc\xEA pelo chat.",
    primaryLabel: "Ver minha loja",
    onPrimary: onClose,
    secondaryLabel: "Adicionar outro",
    onSecondary: () => {
      setStep(0);
      setDone(false);
      setPhoto(false);
      setName('');
      setPrice('');
    }
  });
  const next = () => step < 2 ? setStep(step + 1) : publish();
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'publish' : 'arrow_forward',
    onClick: next
  }, step === 2 ? 'Publicar produto' : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Novo produto",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "O que voc\xEA quer vender no territ\xF3rio?"
  }, "Detalhes"), /*#__PURE__*/React.createElement(JField, {
    label: "Nome do produto"
  }, /*#__PURE__*/React.createElement("input", {
    value: name,
    onChange: e => setName(e.target.value),
    placeholder: "Ex.: Doce de banana caseiro",
    style: jInput
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Categoria"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 2,
    value: cat,
    onChange: setCat,
    options: ['Alimento', 'Serviço', 'Artesanato', 'Aluguel']
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Pre\xE7o"
  }, /*#__PURE__*/React.createElement("input", {
    value: price,
    onChange: e => setPrice(e.target.value),
    placeholder: "R$ 0,00",
    inputMode: "numeric",
    style: jInput
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Foto e descri\xE7\xE3o"), /*#__PURE__*/React.createElement(JField, {
    label: "Foto do produto"
  }, /*#__PURE__*/React.createElement("button", {
    onClick: () => setPhoto(true),
    style: {
      width: '100%',
      padding: '26px',
      borderRadius: 16,
      cursor: 'pointer',
      background: T.cardFlat,
      border: `1.5px dashed ${photo ? T.green : T.lineHi}`,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      gap: 9,
      WebkitTapHighlightColor: 'transparent'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: photo ? 'check_circle' : 'add_a_photo',
    size: 30,
    color: photo ? T.green : T.fg2,
    fill: photo ? 1 : 0
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 500,
      color: photo ? T.green : T.fg2
    }
  }, photo ? 'foto-produto.jpg' : 'Adicionar foto'))), /*#__PURE__*/React.createElement(JField, {
    label: "Descri\xE7\xE3o"
  }, /*#__PURE__*/React.createElement("textarea", {
    placeholder: "Ingredientes, tamanho, prazo de entrega\u2026",
    rows: 3,
    style: {
      ...jInput,
      resize: 'none',
      lineHeight: 1.5
    }
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Disponibilidade"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    value: avail,
    onChange: setAvail,
    options: ['Disponível', 'Sob encomenda']
  }))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Confira antes de publicar."
  }, "Tudo certo?"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "sell",
    label: "Produto",
    value: name || '—',
    accent: T.green
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "category",
    label: "Categoria",
    value: cat
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "payments",
    label: "Pre\xE7o",
    value: price ? price.startsWith('R$') ? price : 'R$ ' + price : '—'
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "image",
    label: "Foto",
    value: photo ? 'Anexada' : 'Sem foto',
    accent: photo ? T.green : T.fg3
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "inventory",
    label: "Disponibilidade",
    value: avail
  }))));
}

// ---- Receber Aratá (QR + chave) ----
function WalletReceiveJourney({
  onClose
}) {
  const w = window.ARAH.wallet;
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Receber Arat\xE1",
    step: 0,
    steps: 1,
    onClose: onClose,
    footer: /*#__PURE__*/React.createElement(Btn, {
      kind: "primary",
      full: true,
      size: "lg",
      icon: "ios_share",
      onClick: onClose
    }, "Compartilhar cobran\xE7a")
  }, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Mostre o c\xF3digo para quem vai te pagar"
  }, "Sua cobran\xE7a"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      padding: '22px',
      borderRadius: 16,
      background: '#fff',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "qr_code_2",
    size: 150,
    color: "#0E2117",
    fill: 0
  })), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "account_circle",
    label: "Recebedor",
    value: "Ana Ribeiro"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "tag",
    label: "Carteira",
    value: `${w.name} · Camburi`
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "account_balance_wallet",
    label: "Saldo atual",
    value: w.balance,
    accent: T.green
  })));
}

// ---- Adicionar Aratá (top-up) ----
function WalletTopupJourney({
  onClose
}) {
  const [step, setStep] = React.useState(0);
  const [amount, setAmount] = React.useState('100');
  const [done, setDone] = React.useState(false);
  const confirm = () => {
    window.arahMutate(() => {
      const w = window.ARAH.wallet;
      const cur = parseInt(String(w.balance).replace(/\D/g, '')) || 0;
      const n = cur + parseInt(amount);
      w.balance = 'A̧ ' + n;
      w.brl = 'R$ ' + n + ',00';
      w.transactions.unshift({
        id: 'wt' + Date.now(),
        label: 'Recarga de Aratá',
        sub: 'via PIX',
        val: '+ A̧ ' + amount,
        icon: 'add',
        neg: false
      });
    });
    setDone(true);
  };
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    title: "Arat\xE1 adicionado!",
    msg: `A̧ ${amount} entraram na sua carteira. A moeda circula só dentro do território de Camburi.`,
    primaryLabel: "Ver carteira",
    onPrimary: onClose,
    accent: T.green
  });
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 1 ? 'lock' : 'arrow_forward',
    onClick: () => step < 1 ? setStep(1) : confirm()
  }, step === 1 ? `Pagar R$ ${amount}` : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Adicionar Arat\xE1",
    step: step,
    steps: 2,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "1 Arat\xE1 = R$ 1,00 \xB7 lastreado pelo fundo comunit\xE1rio"
  }, "Quanto adicionar?"), /*#__PURE__*/React.createElement("div", {
    style: {
      textAlign: 'center',
      padding: '18px 0'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 52,
      color: T.fg,
      letterSpacing: -2
    }
  }, "A\u0327 ", amount || '0')), /*#__PURE__*/React.createElement(PillSelect, {
    cols: 4,
    value: amount,
    onChange: setAmount,
    options: ['50', '100', '200', '500']
  })), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Pague via PIX para creditar sua carteira."
  }, "Pagamento"), /*#__PURE__*/React.createElement(PixPay, {
    amount: `R$ ${amount},00`
  })));
}

// ---- Participar de evento ----
function JoinEventJourney({
  ctx,
  onClose
}) {
  const e = ctx || {
    title: 'Mutirão de limpeza',
    time: 'Sáb · 08:00',
    place: 'Praça da Capela'
  };
  const [going, setGoing] = React.useState('1');
  const [cal, setCal] = React.useState(true);
  const [done, setDone] = React.useState(false);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "event_available",
    title: "Presen\xE7a confirmada!",
    msg: `Te esperamos no "${e.title}". ${cal ? 'O evento foi adicionado à sua agenda e ' : ''}você receberá um lembrete no dia.`,
    primaryLabel: "Concluir",
    onPrimary: () => {
      ctx?.onConfirm && ctx.onConfirm();
      onClose();
    },
    secondaryLabel: "Fechar",
    onSecondary: onClose
  });
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Participar do evento",
    step: 0,
    steps: 1,
    onClose: onClose,
    footer: /*#__PURE__*/React.createElement(Btn, {
      kind: "primary",
      full: true,
      size: "lg",
      icon: "event_available",
      onClick: () => setDone(true)
    }, "Confirmar presen\xE7a")
  }, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: `${e.time} · ${e.place}`
  }, e.title), /*#__PURE__*/React.createElement(JField, {
    label: "Quantas pessoas v\xE3o com voc\xEA?"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 4,
    value: going,
    onChange: setGoing,
    options: ['Só eu', '1', '2', '3+']
  })), /*#__PURE__*/React.createElement("button", {
    onClick: () => setCal(!cal),
    style: {
      width: '100%',
      display: 'flex',
      alignItems: 'center',
      gap: 12,
      padding: '14px 15px',
      borderRadius: 14,
      cursor: 'pointer',
      background: T.cardFlat,
      border: `1px solid ${T.line}`,
      WebkitTapHighlightColor: 'transparent'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "calendar_add_on",
    size: 20,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      textAlign: 'left',
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg
    }
  }, "Adicionar \xE0 minha agenda"), /*#__PURE__*/React.createElement("div", {
    style: {
      width: 44,
      height: 26,
      borderRadius: 999,
      background: cal ? T.greenSolid : T.cardHi,
      position: 'relative',
      transition: 'background .2s'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: 3,
      left: cal ? 21 : 3,
      width: 20,
      height: 20,
      borderRadius: '50%',
      background: '#fff',
      transition: 'left .2s'
    }
  }))));
}

// ---- Abrir nova eleição (curador) ----
function CreateElectionJourney({
  onClose
}) {
  const [step, setStep] = React.useState(0);
  const [type, setType] = React.useState('Conselho');
  const [deadline, setDeadline] = React.useState('7 dias');
  const [done, setDone] = React.useState(false);
  if (done) return /*#__PURE__*/React.createElement(SuccessView, {
    icon: "how_to_vote",
    title: "Elei\xE7\xE3o aberta!",
    msg: "A vota\xE7\xE3o foi publicada para todos os moradores de Camburi. Voc\xEA acompanha os resultados em tempo real na gest\xE3o.",
    primaryLabel: "Concluir",
    onPrimary: onClose,
    accent: T.alert
  });
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: step === 2 ? 'how_to_vote' : 'arrow_forward',
    onClick: next
  }, step === 2 ? 'Abrir votação' : 'Continuar');
  return /*#__PURE__*/React.createElement(JourneyShell, {
    title: "Abrir elei\xE7\xE3o",
    step: step,
    steps: 3,
    onBack: () => setStep(step - 1),
    onClose: onClose,
    footer: footer
  }, step === 0 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Governan\xE7a comunit\xE1ria do territ\xF3rio"
  }, "Tipo de vota\xE7\xE3o"), /*#__PURE__*/React.createElement(JField, null, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 1,
    value: type,
    onChange: setType,
    options: ['Conselho', 'Representante de tema', 'Consulta pública']
  })), /*#__PURE__*/React.createElement(JField, {
    label: "T\xEDtulo"
  }, /*#__PURE__*/React.createElement("input", {
    placeholder: "Ex.: Conselho de Moradores 2026",
    style: jInput
  }))), step === 1 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, {
    sub: "Quem pode se candidatar e por quanto tempo."
  }, "Regras"), /*#__PURE__*/React.createElement(JField, {
    label: "Prazo de vota\xE7\xE3o"
  }, /*#__PURE__*/React.createElement(PillSelect, {
    cols: 3,
    value: deadline,
    onChange: setDeadline,
    options: ['3 dias', '7 dias', '15 dias']
  })), /*#__PURE__*/React.createElement(JField, {
    label: "Candidatos",
    hint: "No app real, moradores se inscrevem ou s\xE3o indicados."
  }, /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 12
    }
  }, ['Dona Marta', 'Seu Tião', 'Bruno Caiçara'].map(n => /*#__PURE__*/React.createElement("div", {
    key: n,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      padding: '7px 0'
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: "#4F956F",
    name: n,
    size: 30,
    resident: true
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg
    }
  }, n), /*#__PURE__*/React.createElement(Icon, {
    name: "check_circle",
    size: 18,
    color: T.green,
    fill: 1
  })))))), step === 2 && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(JStepTitle, null, "Revise e abra"), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: '4px 16px'
    }
  }, /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "how_to_vote",
    label: "Tipo",
    value: type,
    accent: T.alert
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "schedule",
    label: "Prazo",
    value: deadline
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "groups",
    label: "Candidatos",
    value: "3 moradores"
  }), /*#__PURE__*/React.createElement(ReviewRow, {
    icon: "visibility",
    label: "Quem vota",
    value: "Moradores de Camburi"
  }))));
}
Object.assign(window, {
  VoteJourney,
  GroupBuyJoinJourney,
  AddProductJourney,
  WalletReceiveJourney,
  WalletTopupJourney,
  JoinEventJourney,
  CreateElectionJourney
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens18.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens2.jsx
try { (() => {
// screens2.jsx — Create Post (with photo), Notifications.

function CreatePostScreen({
  territory,
  onPublish
}) {
  const [type, setType] = React.useState('General');
  const [vis, setVis] = React.useState('Public');
  const [title, setTitle] = React.useState('');
  const [body, setBody] = React.useState('');
  const [photo, setPhoto] = React.useState(null);
  const fileRef = React.useRef(null);
  const pickPhoto = e => {
    const f = e.target.files?.[0];
    if (f) setPhoto(URL.createObjectURL(f));
  };
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(Card, {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      padding: '11px 14px',
      marginBottom: 16,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement(Logo, {
    size: 22
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      color: T.fg
    }
  }, "Publicando em ", /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.green,
      fontWeight: 600
    }
  }, territory.name))), /*#__PURE__*/React.createElement(Field, {
    label: "T\xEDtulo"
  }, /*#__PURE__*/React.createElement("input", {
    value: title,
    onChange: e => setTitle(e.target.value),
    placeholder: "D\xEA um t\xEDtulo ao seu post",
    style: inputStyle
  })), /*#__PURE__*/React.createElement(Field, {
    label: "Conte\xFAdo"
  }, /*#__PURE__*/React.createElement("textarea", {
    value: body,
    onChange: e => setBody(e.target.value),
    rows: 4,
    placeholder: "O que voc\xEA quer compartilhar com a vila?",
    style: {
      ...inputStyle,
      resize: 'none',
      lineHeight: 1.5
    }
  })), /*#__PURE__*/React.createElement(Field, {
    label: "M\xEDdia"
  }, /*#__PURE__*/React.createElement("input", {
    ref: fileRef,
    type: "file",
    accept: "image/*",
    onChange: pickPhoto,
    style: {
      display: 'none'
    }
  }), photo ? /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      borderRadius: 14,
      overflow: 'hidden',
      border: `1px solid ${T.line}`
    }
  }, /*#__PURE__*/React.createElement("img", {
    src: photo,
    alt: "",
    style: {
      width: '100%',
      height: 170,
      objectFit: 'cover',
      display: 'block'
    }
  }), /*#__PURE__*/React.createElement("button", {
    onClick: () => setPhoto(null),
    style: {
      position: 'absolute',
      top: 10,
      right: 10,
      width: 34,
      height: 34,
      borderRadius: '50%',
      border: 'none',
      background: 'rgba(11,12,10,0.7)',
      backdropFilter: 'blur(6px)',
      cursor: 'pointer',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "close",
    size: 19,
    color: "#fff"
  }))) : /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement(MediaBtn, {
    icon: "image",
    label: "Foto",
    onClick: () => fileRef.current?.click()
  }), /*#__PURE__*/React.createElement(MediaBtn, {
    icon: "photo_camera",
    label: "C\xE2mera",
    onClick: () => fileRef.current?.click()
  }), /*#__PURE__*/React.createElement(MediaBtn, {
    icon: "location_on",
    label: "Local",
    onClick: () => {}
  }))), /*#__PURE__*/React.createElement(Field, {
    label: "Tipo"
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement(SegOption, {
    active: type === 'General',
    onClick: () => setType('General'),
    icon: "chat_bubble",
    tone: "green"
  }, "Geral"), /*#__PURE__*/React.createElement(SegOption, {
    active: type === 'Alert',
    onClick: () => setType('Alert'),
    icon: "warning",
    tone: "alert"
  }, "Alerta"))), /*#__PURE__*/React.createElement(Field, {
    label: "Visibilidade"
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement(SegOption, {
    active: vis === 'Public',
    onClick: () => setVis('Public'),
    icon: "public",
    tone: "green"
  }, "P\xFAblico"), /*#__PURE__*/React.createElement(SegOption, {
    active: vis === 'ResidentsOnly',
    onClick: () => setVis('ResidentsOnly'),
    icon: "lock",
    tone: "green"
  }, "S\xF3 moradores"))), /*#__PURE__*/React.createElement("div", {
    style: {
      marginTop: 22
    }
  }, /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: "send",
    onClick: () => onPublish({
      type,
      vis,
      title,
      body,
      photo
    })
  }, "Publicar no territ\xF3rio")));
}
function MediaBtn({
  icon,
  label,
  onClick
}) {
  return /*#__PURE__*/React.createElement("button", {
    onClick: onClick,
    style: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      gap: 7,
      padding: '16px 8px',
      borderRadius: 14,
      cursor: 'pointer',
      WebkitTapHighlightColor: 'transparent',
      background: T.cardFlat,
      border: `1px dashed ${T.lineHi}`,
      color: T.fg2,
      fontFamily: T.sans,
      fontSize: 12.5,
      fontWeight: 500
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 22,
    color: T.green
  }), " ", label);
}
function Field({
  label,
  children
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement("label", {
    style: {
      display: 'block',
      fontFamily: T.sans,
      fontSize: 12.5,
      fontWeight: 600,
      color: T.fg2,
      marginBottom: 8,
      letterSpacing: 0.1
    }
  }, label), children);
}
const inputStyle = {
  width: '100%',
  boxSizing: 'border-box',
  background: T.cardFlat,
  color: T.fg,
  border: `1px solid ${T.line}`,
  borderRadius: 14,
  padding: '13px 15px',
  fontFamily: T.sans,
  fontSize: 15,
  outline: 'none'
};
function SegOption({
  children,
  active,
  onClick,
  icon,
  tone
}) {
  const c = tone === 'alert' ? T.alert : T.green;
  const dim = tone === 'alert' ? T.alertDim : T.greenDim;
  return /*#__PURE__*/React.createElement("button", {
    onClick: onClick,
    style: {
      flex: 1,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 7,
      padding: '12px',
      borderRadius: 13,
      cursor: 'pointer',
      WebkitTapHighlightColor: 'transparent',
      background: active ? dim : T.cardFlat,
      color: active ? c : T.fg2,
      border: `1px solid ${active ? 'transparent' : T.line}`,
      fontFamily: T.sans,
      fontSize: 14,
      fontWeight: 500,
      transition: 'all .15s'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 17,
    fill: active ? 1 : 0
  }), " ", children);
}
function NotificationsScreen({
  items,
  onRead,
  onOpen
}) {
  const kindMeta = {
    alert: {
      icon: 'warning',
      color: T.alert,
      dim: T.alertDim
    },
    event: {
      icon: 'event',
      color: T.blue,
      dim: T.blueDim
    },
    map: {
      icon: 'map',
      color: T.water,
      dim: 'rgba(111,197,214,0.14)'
    },
    connection: {
      icon: 'group',
      color: T.green,
      dim: T.greenDim
    },
    post: {
      icon: 'article',
      color: T.green,
      dim: T.greenDim
    }
  };
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 12px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 8
    }
  }, items.map(n => {
    const m = kindMeta[n.kind] || kindMeta.post;
    return /*#__PURE__*/React.createElement(Card, {
      key: n.id,
      onClick: () => {
        onRead(n.id);
        onOpen(n.link);
      },
      style: {
        display: 'flex',
        gap: 13,
        padding: 14,
        position: 'relative',
        background: n.read ? 'transparent' : T.cardGrad,
        border: `1px solid ${n.read ? T.line : 'rgba(166,214,185,0.18)'}`
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 42,
        height: 42,
        borderRadius: 12,
        flexShrink: 0,
        background: m.dim,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: m.icon,
      size: 21,
      color: m.color,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: n.read ? 500 : 600,
        fontSize: 14.5,
        color: T.fg,
        letterSpacing: -0.2,
        lineHeight: 1.3
      }
    }, n.title), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 13,
        color: T.fg2,
        marginTop: 3,
        lineHeight: 1.45
      }
    }, n.body), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 11.5,
        color: T.fg3,
        marginTop: 6
      }
    }, n.time)), !n.read && /*#__PURE__*/React.createElement("div", {
      style: {
        width: 8,
        height: 8,
        borderRadius: '50%',
        background: T.green,
        flexShrink: 0,
        marginTop: 4
      }
    }));
  })));
}
Object.assign(window, {
  CreatePostScreen,
  NotificationsScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens2.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens3.jsx
try { (() => {
// screens3.jsx — Profile hub (role switcher), Events (join/cancel), Settings, Território Health.

function ProfileScreen({
  role,
  onSetRole,
  onOpen,
  interests,
  onToggleInterest
}) {
  const p = window.ARAH.profile;
  const roleLabel = {
    visitante: 'Visitante',
    morador: 'Morador',
    curador: 'Curador'
  };
  const tools = [{
    id: 'store',
    icon: 'storefront',
    label: 'Minha loja',
    color: T.green,
    badge: 'Novo'
  }, {
    id: 'chat',
    icon: 'forum',
    label: 'Mensagens',
    color: T.green
  }, {
    id: 'saved',
    icon: 'bookmark',
    label: 'Salvos',
    color: T.blue
  }, {
    id: 'settings',
    icon: 'settings',
    label: 'Configurações',
    color: T.fg2
  }];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 15,
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: p.avatar,
    name: p.name,
    size: 68,
    resident: role !== 'visitante'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 21,
      color: T.fg,
      letterSpacing: -0.4
    }
  }, p.name), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7,
      marginTop: 4
    }
  }, /*#__PURE__*/React.createElement(RoleBadge, {
    role: role === 'visitante' ? 'visitante' : 'morador',
    size: "md"
  }), role === 'curador' && /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4,
      padding: '4px 10px',
      borderRadius: 999,
      background: T.alertDim,
      color: T.alert,
      fontFamily: T.sans,
      fontSize: 12,
      fontWeight: 600
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "shield_person",
    size: 14,
    fill: 1
  }), " Curador")))), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '0 0 14px',
      fontFamily: T.sans,
      fontSize: 14,
      lineHeight: 1.55,
      color: T.fg2
    }
  }, p.bio), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 14,
      marginBottom: 16,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7,
      marginBottom: 10
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "switch_account",
    size: 17,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      fontWeight: 600,
      color: T.fg
    }
  }, "Ver como (demonstra\xE7\xE3o de jornada)")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8
    }
  }, ['visitante', 'morador', 'curador'].map(r => /*#__PURE__*/React.createElement("button", {
    key: r,
    onClick: () => onSetRole(r),
    style: {
      flex: 1,
      padding: '9px 6px',
      borderRadius: 11,
      cursor: 'pointer',
      WebkitTapHighlightColor: 'transparent',
      background: role === r ? T.greenDim : T.cardFlat,
      color: role === r ? T.green : T.fg2,
      border: `1px solid ${role === r ? 'transparent' : T.line}`,
      fontFamily: T.sans,
      fontSize: 12.5,
      fontWeight: 600,
      transition: 'all .15s'
    }
  }, roleLabel[r]))), role === 'visitante' && /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "sm",
    icon: "how_to_reg",
    style: {
      marginTop: 11
    },
    onClick: () => window.openJourney('residencia')
  }, "Confirmar resid\xEAncia")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 10,
      marginBottom: 18
    }
  }, [['Posts', p.posts], ['Territórios', p.territories], ['Desde', p.since]].map(([k, v]) => /*#__PURE__*/React.createElement(Card, {
    key: k,
    style: {
      flex: 1,
      padding: '13px 10px',
      textAlign: 'center'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.fg
    }
  }, v), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 2,
      letterSpacing: 0.2
    }
  }, k)))), /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "account_circle"
  }, "Minha conta"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 10,
      marginTop: 11,
      marginBottom: 22
    }
  }, tools.map(t => /*#__PURE__*/React.createElement(Card, {
    key: t.id,
    onClick: () => onOpen(t.id),
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      padding: 14
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 38,
      height: 38,
      borderRadius: 11,
      background: `${t.color}1f`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: t.icon,
    size: 20,
    color: t.color,
    fill: 1
  })), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 500,
      color: T.fg,
      lineHeight: 1.2
    }
  }, t.label), t.badge && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 9.5,
      fontWeight: 700,
      color: t.color,
      background: `${t.color}1f`,
      padding: '2px 6px',
      borderRadius: 999,
      letterSpacing: 0.3
    }
  }, t.badge)))), /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "interests"
  }, "Meus interesses"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3,
      margin: '0 0 11px'
    }
  }, "Interesses personalizam o feed do territ\xF3rio."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexWrap: 'wrap',
      gap: 8,
      marginBottom: 22
    }
  }, window.ARAH.interestPool.map(i => /*#__PURE__*/React.createElement(Chip, {
    key: i,
    active: interests.includes(i),
    onClick: () => onToggleInterest(i),
    icon: interests.includes(i) ? 'check' : 'add'
  }, i))), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    full: true,
    icon: "logout",
    onClick: () => onOpen('logout'),
    style: {
      color: T.alert,
      borderColor: 'rgba(232,160,106,0.2)'
    }
  }, "Sair da conta"));
}
function SectionLabel({
  children,
  icon
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 18,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, children));
}
function EventsScreen({
  territory,
  content,
  role
}) {
  const [status, setStatus] = React.useState(Object.fromEntries(content.events.map(e => [e.id, e.status])));
  const toggle = id => setStatus(s => ({
    ...s,
    [id]: s[id] === 'confirmed' ? null : 'confirmed'
  }));
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '0 0 16px',
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg2,
      lineHeight: 1.5
    }
  }, "Acontecendo em ", /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.green,
      fontWeight: 600
    }
  }, territory.name)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 11
    }
  }, content.events.map(e => {
    const confirmed = status[e.id] === 'confirmed';
    return /*#__PURE__*/React.createElement(Card, {
      key: e.id,
      glow: true,
      style: {
        display: 'flex',
        gap: 14,
        padding: 14
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 52,
        flexShrink: 0,
        borderRadius: 13,
        background: T.greenDim,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '8px 0'
      }
    }, /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.display,
        fontWeight: 700,
        fontSize: 21,
        color: T.green,
        lineHeight: 1
      }
    }, e.day), /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 10.5,
        fontWeight: 600,
        color: T.green,
        letterSpacing: 1,
        marginTop: 3
      }
    }, e.mon)), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'inline-block',
        fontFamily: T.sans,
        fontSize: 10.5,
        fontWeight: 600,
        color: T.fg3,
        letterSpacing: 0.8,
        textTransform: 'uppercase',
        marginBottom: 3
      }
    }, e.tag), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 15.5,
        color: T.fg,
        letterSpacing: -0.2,
        lineHeight: 1.3
      }
    }, e.title), /*#__PURE__*/React.createElement("p", {
      style: {
        margin: '5px 0 0',
        fontFamily: T.sans,
        fontSize: 13,
        color: T.fg2,
        lineHeight: 1.45
      }
    }, e.desc), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 10,
        marginTop: 8,
        color: T.fg3,
        fontFamily: T.sans,
        fontSize: 12.5
      }
    }, /*#__PURE__*/React.createElement("span", {
      style: {
        display: 'inline-flex',
        alignItems: 'center',
        gap: 4
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "schedule",
      size: 14
    }), " ", e.time), /*#__PURE__*/React.createElement("span", {
      style: {
        display: 'inline-flex',
        alignItems: 'center',
        gap: 4
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "place",
      size: 14
    }), " ", e.place)), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 10,
        marginTop: 11
      }
    }, /*#__PURE__*/React.createElement(Btn, {
      kind: confirmed ? 'dark' : 'primary',
      size: "sm",
      icon: confirmed ? 'check_circle' : 'event_available',
      onClick: () => {
        if (!confirmed) window.openJourney('joinEvent', {
          ...e,
          onConfirm: () => toggle(e.id)
        });
      },
      style: confirmed ? {
        color: T.green,
        borderColor: 'rgba(166,214,185,0.3)'
      } : {}
    }, confirmed ? 'Presença confirmada' : 'Participar'), confirmed && /*#__PURE__*/React.createElement("button", {
      onClick: () => toggle(e.id),
      style: {
        background: 'none',
        border: 'none',
        cursor: 'pointer',
        color: T.fg3,
        fontFamily: T.sans,
        fontSize: 12.5,
        fontWeight: 500
      }
    }, "Cancelar"), /*#__PURE__*/React.createElement("span", {
      style: {
        marginLeft: 'auto',
        fontFamily: T.sans,
        fontSize: 12,
        color: T.fg3
      }
    }, e.going + (confirmed ? 1 : 0), " v\xE3o"))));
  })));
}
function HealthScreen({
  territory,
  content
}) {
  const h = content.health;
  const meters = [{
    label: 'Água potável',
    val: h.agua,
    icon: 'water_drop',
    color: T.water,
    unit: '%'
  }, {
    label: 'Árvores nativas',
    val: h.nativas,
    icon: 'park',
    color: T.green,
    unit: '%'
  }];
  const counts = [{
    label: 'Nascentes',
    val: h.nascentes,
    icon: 'water',
    color: T.water
  }, {
    label: 'Mirantes',
    val: h.mirantes,
    icon: 'landscape',
    color: T.green
  }, {
    label: 'Santuários',
    val: h.santuarios,
    icon: 'forest',
    color: T.green
  }];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '0 0 16px',
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg2,
      lineHeight: 1.5
    }
  }, "Indicadores vivos de ", /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.green,
      fontWeight: 600
    }
  }, territory.name), ", mantidos pela comunidade."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 11,
      marginBottom: 14
    }
  }, meters.map(m => /*#__PURE__*/React.createElement(Card, {
    key: m.label,
    style: {
      padding: 16
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      marginBottom: 11
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: m.icon,
    size: 20,
    color: m.color,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 14,
      fontWeight: 500,
      color: T.fg
    }
  }, m.label), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 18,
      color: m.color
    }
  }, m.val, m.unit)), /*#__PURE__*/React.createElement("div", {
    style: {
      height: 8,
      borderRadius: 999,
      background: T.cardHi,
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: `${m.val}%`,
      height: '100%',
      borderRadius: 999,
      background: `linear-gradient(90deg, ${m.color}aa, ${m.color})`
    }
  }))))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr 1fr',
      gap: 10
    }
  }, counts.map(c => /*#__PURE__*/React.createElement(Card, {
    key: c.label,
    style: {
      padding: '16px 8px',
      textAlign: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: c.icon,
    size: 24,
    color: c.color,
    fill: 1
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 22,
      color: T.fg,
      marginTop: 7
    }
  }, c.val), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11,
      color: T.fg3,
      marginTop: 1
    }
  }, c.label)))));
}
function SettingsScreen() {
  const groups = [{
    title: 'Conta',
    rows: [{
      icon: 'person',
      label: 'Editar perfil',
      detail: ''
    }, {
      icon: 'verified_user',
      label: 'Confirmar residência',
      detail: 'Visitante',
      green: true
    }]
  }, {
    title: 'Preferências',
    rows: [{
      icon: 'notifications',
      label: 'Notificações',
      detail: 'Posts · Eventos · Alertas'
    }, {
      icon: 'public',
      label: 'Idioma',
      detail: 'Português'
    }, {
      icon: 'my_location',
      label: 'Localização',
      detail: 'Ativa',
      green: true
    }, {
      icon: 'dark_mode',
      label: 'Tema',
      detail: 'Escuro'
    }]
  }, {
    title: 'Privacidade',
    rows: [{
      icon: 'shield',
      label: 'Privacidade e dados',
      detail: ''
    }, {
      icon: 'block',
      label: 'Contas bloqueadas',
      detail: ''
    }]
  }];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, groups.map(g => /*#__PURE__*/React.createElement("div", {
    key: g.title,
    style: {
      marginBottom: 18
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      fontWeight: 600,
      letterSpacing: 0.8,
      textTransform: 'uppercase',
      color: T.fg3,
      margin: '0 2px 9px'
    }
  }, g.title), /*#__PURE__*/React.createElement(Card, {
    style: {
      overflow: 'hidden'
    }
  }, g.rows.map((r, i) => /*#__PURE__*/React.createElement("div", {
    key: r.label,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: '14px 15px',
      borderBottom: i === g.rows.length - 1 ? 'none' : `1px solid ${T.line}`,
      cursor: 'pointer'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: r.icon,
    size: 20,
    color: T.fg2
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 14.5,
      color: T.fg
    }
  }, r.label), r.detail && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: r.green ? T.green : T.fg3
    }
  }, r.detail), /*#__PURE__*/React.createElement(Icon, {
    name: "chevron_right",
    size: 19,
    color: T.fg3
  })))))));
}
Object.assign(window, {
  ProfileScreen,
  EventsScreen,
  HealthScreen,
  SettingsScreen,
  SectionLabel
});

// Saved / bookmarked content (posts + market items)
function SavedScreen() {
  const [tab, setTab] = React.useState('posts');
  const posts = window.ARAH.content.t1.feed.slice(0, 2);
  const items = window.ARAH.market.slice(0, 2);
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'posts',
    onClick: () => setTab('posts'),
    icon: "article"
  }, "Posts"), /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'items',
    onClick: () => setTab('items'),
    icon: "storefront"
  }, "Mercado")), tab === 'posts' ? /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, posts.map(p => /*#__PURE__*/React.createElement(Card, {
    key: p.id,
    style: {
      padding: 14,
      display: 'flex',
      gap: 12,
      alignItems: 'flex-start'
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: p.avatar,
    name: p.author,
    size: 38,
    resident: p.role === 'morador'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14.5,
      color: T.fg,
      letterSpacing: -0.2,
      lineHeight: 1.3
    }
  }, p.title), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      marginTop: 3
    }
  }, p.author, " \xB7 ", p.time)), /*#__PURE__*/React.createElement(Icon, {
    name: "bookmark",
    size: 20,
    color: T.green,
    fill: 1
  })))) : /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, items.map(m => /*#__PURE__*/React.createElement(Card, {
    key: m.id,
    style: {
      padding: 14,
      display: 'flex',
      gap: 12,
      alignItems: 'center'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 44,
      height: 44,
      borderRadius: 12,
      background: `${m.avatar}22`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: m.icon,
    size: 22,
    color: T.green
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14.5,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, m.title), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      marginTop: 2
    }
  }, m.seller, " \xB7 ", m.price)), /*#__PURE__*/React.createElement(Icon, {
    name: "bookmark",
    size: 20,
    color: T.green,
    fill: 1
  })))), /*#__PURE__*/React.createElement("div", {
    style: {
      textAlign: 'center',
      color: T.fg3,
      fontFamily: T.sans,
      fontSize: 12.5,
      padding: '18px 0 4px'
    }
  }, "Toque no marcador em qualquer post ou item para salvar aqui."));
}
Object.assign(window, {
  SavedScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens3.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens4.jsx
try { (() => {
// screens4.jsx — Map (geolocation + entities + health), Login, Onboarding, Territory sheet.

// Slippy-tile math: lat/lng → tile x/y at zoom z
function lngLatToTile(lng, lat, z) {
  const n = Math.pow(2, z);
  const x = Math.floor((lng + 180) / 360 * n);
  const latRad = lat * Math.PI / 180;
  const y = Math.floor((1 - Math.log(Math.tan(latRad) + 1 / Math.cos(latRad)) / Math.PI) / 2 * n);
  return {
    x,
    y
  };
}

// Dark-styled OpenStreetMap tile mosaic. coords (geo) override tile; else tile prop; else Camburi.
function MapCanvas({
  height = 220,
  coords = null,
  tile = null,
  mini = false,
  entities = [],
  onPin
}) {
  const z = 13;
  const center = coords ? lngLatToTile(coords.lng, coords.lat, z) : tile || {
    x: 3057,
    y: 4652
  };
  const tiles = [];
  for (let dy = -1; dy <= 1; dy++) for (let dx = -1; dx <= 1; dx++) tiles.push({
    url: `https://tile.openstreetmap.org/${z}/${center.x + dx}/${center.y + dy}.png`
  });
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      height,
      overflow: 'hidden',
      background: '#0a120d'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      width: 768,
      height: 768,
      left: '50%',
      top: '50%',
      transform: 'translate(-50%, -50%)',
      display: 'grid',
      gridTemplateColumns: 'repeat(3, 256px)',
      gridTemplateRows: 'repeat(3, 256px)',
      filter: 'invert(0.91) hue-rotate(165deg) saturate(0.55) brightness(0.92) contrast(0.98)'
    }
  }, tiles.map((t, i) => /*#__PURE__*/React.createElement("img", {
    key: i,
    src: t.url,
    width: 256,
    height: 256,
    alt: "",
    loading: "lazy",
    style: {
      display: 'block'
    },
    crossOrigin: "anonymous"
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      background: 'radial-gradient(120% 100% at 50% 50%, transparent 40%, rgba(11,18,13,0.55))',
      mixBlendMode: 'multiply'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      top: '50%',
      transform: 'translate(-50%,-50%)',
      width: height * 0.7,
      height: height * 0.7,
      borderRadius: '50%',
      border: `2px solid ${T.green}`,
      background: 'rgba(166,214,185,0.10)'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      top: '50%',
      transform: 'translate(-50%,-50%)',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 16,
      height: 16,
      borderRadius: '50%',
      background: T.alert,
      border: '3px solid #fff',
      boxShadow: '0 0 0 6px rgba(232,160,106,0.25)'
    }
  })), !mini && entities.map(e => /*#__PURE__*/React.createElement("button", {
    key: e.id,
    onClick: () => onPin && onPin(e),
    style: {
      position: 'absolute',
      left: `${e.x}%`,
      top: `${e.y}%`,
      transform: 'translate(-50%,-100%)',
      background: 'none',
      border: 'none',
      cursor: 'pointer',
      padding: 0,
      WebkitTapHighlightColor: 'transparent'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 34,
      height: 34,
      borderRadius: '50% 50% 50% 2px',
      transform: 'rotate(45deg)',
      background: 'rgba(16,20,16,0.85)',
      backdropFilter: 'blur(4px)',
      border: `1.5px solid ${e.color}`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      boxShadow: '0 6px 14px rgba(0,0,0,0.4)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: e.icon,
    size: 16,
    color: e.color,
    fill: 1,
    style: {
      transform: 'rotate(-45deg)'
    }
  })))));
}
function MapScreen({
  territory,
  content,
  focusId
}) {
  const ents = content.entities;
  const [coords, setCoords] = React.useState(null);
  const [geoState, setGeoState] = React.useState('idle'); // idle|loading|ok|denied
  const [sel, setSel] = React.useState(focusId ? ents.find(e => e.id === focusId) : null);
  const locate = () => {
    setGeoState('loading');
    if (!navigator.geolocation) {
      setGeoState('denied');
      return;
    }
    navigator.geolocation.getCurrentPosition(pos => {
      setCoords({
        lat: pos.coords.latitude,
        lng: pos.coords.longitude
      });
      setGeoState('ok');
    }, () => setGeoState('denied'), {
      timeout: 8000
    });
  };
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      height: '100%'
    }
  }, /*#__PURE__*/React.createElement(MapCanvas, {
    height: 760,
    coords: coords,
    tile: content.tile,
    entities: ents,
    onPin: setSel
  }), /*#__PURE__*/React.createElement("button", {
    onClick: locate,
    style: {
      position: 'absolute',
      top: 14,
      right: 16,
      width: 44,
      height: 44,
      borderRadius: 14,
      cursor: 'pointer',
      background: T.glassGrad,
      backdropFilter: 'blur(14px)',
      WebkitBackdropFilter: 'blur(14px)',
      border: `1px solid ${T.lineHi}`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: geoState === 'loading' ? 'sync' : 'my_location',
    size: 22,
    color: geoState === 'ok' ? T.green : T.fg,
    fill: geoState === 'ok' ? 1 : 0,
    style: geoState === 'loading' ? {
      animation: 'spin 1s linear infinite'
    } : {}
  })), geoState !== 'idle' && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      top: 16,
      left: 16,
      padding: '8px 13px',
      borderRadius: 999,
      background: T.glassGrad,
      backdropFilter: 'blur(14px)',
      border: `1px solid ${T.lineHi}`,
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg,
      display: 'flex',
      alignItems: 'center',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "place",
    size: 15,
    color: T.alert,
    fill: 1
  }), geoState === 'loading' ? 'Localizando…' : geoState === 'denied' ? 'Mostrando Camburi' : coords ? `${coords.lat.toFixed(3)}, ${coords.lng.toFixed(3)}` : ''), sel ? /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 16,
      right: 16,
      bottom: 16,
      padding: 16,
      background: T.glassGrad,
      backdropFilter: 'blur(16px)',
      WebkitBackdropFilter: 'blur(16px)',
      borderRadius: 20,
      border: `1px solid ${T.lineHi}`,
      boxShadow: '0 12px 30px rgba(0,0,0,0.45)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 12
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 44,
      height: 44,
      borderRadius: 13,
      background: `${sel.color}22`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: sel.icon,
    size: 23,
    color: sel.color,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg
    }
  }, sel.label), sel.confirmed ? /*#__PURE__*/React.createElement(Icon, {
    name: "verified",
    size: 16,
    color: T.green,
    fill: 1
  }) : /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 10.5,
      color: T.alert,
      background: T.alertDim,
      padding: '2px 7px',
      borderRadius: 999,
      fontWeight: 600
    }
  }, "em curadoria")), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg2,
      marginTop: 2
    }
  }, sel.sub)), /*#__PURE__*/React.createElement("button", {
    onClick: () => setSel(null),
    style: {
      ...iconBtn,
      width: 34,
      height: 34
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "close",
    size: 18,
    color: T.fg2
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 9,
      marginTop: 13
    }
  }, /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "directions",
    style: {
      flex: 1
    }
  }, "Rotas"), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: sel.confirmed ? 'thumb_up' : 'how_to_reg',
    style: {
      flex: 1
    }
  }, sel.confirmed ? 'Confirmar' : 'Validar'))) : /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 16,
      right: 16,
      bottom: 16,
      padding: 16,
      background: T.glassGrad,
      backdropFilter: 'blur(16px)',
      WebkitBackdropFilter: 'blur(16px)',
      borderRadius: 20,
      border: `1px solid ${T.lineHi}`,
      boxShadow: '0 12px 30px rgba(0,0,0,0.45)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      marginBottom: 11
    }
  }, /*#__PURE__*/React.createElement(Logo, {
    size: 18,
    mark: "png"
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15,
      color: T.fg
    }
  }, territory.name), /*#__PURE__*/React.createElement("span", {
    style: {
      marginLeft: 'auto',
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, ents.length, " pontos")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexWrap: 'wrap',
      gap: 12
    }
  }, [['storefront', 'Lugares', T.green], ['water', 'Nascentes', T.water], ['landscape', 'Mirantes', T.green], ['event', 'Eventos', T.blue], ['warning', 'Alertas', T.alert]].map(([ic, lb, c]) => /*#__PURE__*/React.createElement("div", {
    key: lb,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: ic,
    size: 16,
    color: c,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2
    }
  }, lb))))));
}
function LoginScreen({
  onLogin
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      padding: '0 26px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 92,
      marginBottom: 22
    }
  }, /*#__PURE__*/React.createElement(Logo, {
    size: 92,
    mark: "png"
  })), /*#__PURE__*/React.createElement(Eyebrow, null, "Territ\xF3rio-primeiro"), /*#__PURE__*/React.createElement("h1", {
    style: {
      margin: '10px 0 0',
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 44,
      lineHeight: 1.02,
      color: T.fg,
      letterSpacing: -1.2
    }
  }, "Arah"), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '14px 0 0',
      fontFamily: T.sans,
      fontSize: 16,
      lineHeight: 1.5,
      color: T.fg2,
      maxWidth: 300
    }
  }, "A vida da sua comunidade, organizada pelo territ\xF3rio. Conecte-se com quem est\xE1 perto.")), /*#__PURE__*/React.createElement("div", {
    style: {
      paddingBottom: 40,
      display: 'flex',
      flexDirection: 'column',
      gap: 11
    }
  }, /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: "login",
    onClick: onLogin
  }, "Entrar com Google"), /*#__PURE__*/React.createElement(Btn, {
    kind: "secondary",
    full: true,
    size: "lg",
    icon: "mail",
    onClick: onLogin
  }, "Entrar com e-mail"), /*#__PURE__*/React.createElement("div", {
    style: {
      textAlign: 'center',
      marginTop: 8,
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg3
    }
  }, "N\xE3o tem conta? ", /*#__PURE__*/React.createElement("span", {
    style: {
      color: T.green,
      fontWeight: 600
    }
  }, "Criar conta"))));
}
function OnboardingScreen({
  onContinue
}) {
  const [sel, setSel] = React.useState('t1');
  const [coords, setCoords] = React.useState(null);
  const [geo, setGeo] = React.useState('idle');
  const list = window.ARAH.territories;
  const selT = list.find(t => t.id === sel);
  React.useEffect(() => {
    if (!navigator.geolocation) {
      setGeo('denied');
      return;
    }
    setGeo('loading');
    navigator.geolocation.getCurrentPosition(pos => {
      setCoords({
        lat: pos.coords.latitude,
        lng: pos.coords.longitude
      });
      setGeo('ok');
    }, () => setGeo('denied'), {
      timeout: 8000
    });
  }, []);
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement("h1", {
    style: {
      margin: '4px 0 6px',
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 25,
      color: T.fg,
      letterSpacing: -0.5
    }
  }, "Escolha seu territ\xF3rio"), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '0 0 16px',
      fontFamily: T.sans,
      fontSize: 14,
      lineHeight: 1.5,
      color: T.fg2
    }
  }, "Para ver o feed e participar, escolha um territ\xF3rio pr\xF3ximo a voc\xEA."), /*#__PURE__*/React.createElement(Card, {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      padding: '12px 14px',
      marginBottom: 14
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: geo === 'ok' ? 'my_location' : geo === 'loading' ? 'sync' : 'location_off',
    size: 20,
    color: geo === 'denied' ? T.fg3 : T.green,
    fill: geo === 'ok' ? 1 : 0,
    style: geo === 'loading' ? {
      animation: 'spin 1s linear infinite'
    } : {}
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      color: T.fg
    }
  }, geo === 'ok' ? 'Localização ativa' : geo === 'loading' ? 'Buscando localização…' : 'Localização indisponível'), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 1
    }
  }, geo === 'ok' && coords ? `${coords.lat.toFixed(3)}, ${coords.lng.toFixed(3)} · privada` : 'Privada — não fica visível para outros.'))), /*#__PURE__*/React.createElement("div", {
    style: {
      borderRadius: 18,
      overflow: 'hidden',
      border: `1px solid ${T.line}`,
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(MapCanvas, {
    height: 180,
    coords: coords,
    tile: window.ARAH.content.t1.tile,
    entities: window.ARAH.content.t1.entities.slice(0, 3)
  })), /*#__PURE__*/React.createElement(Eyebrow, {
    style: {
      marginBottom: 10
    }
  }, "Pr\xF3ximos a voc\xEA"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 9,
      marginBottom: 18
    }
  }, list.slice(0, 3).map(t => {
    const on = t.id === sel;
    return /*#__PURE__*/React.createElement(Card, {
      key: t.id,
      onClick: () => setSel(t.id),
      hi: on,
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 12,
        padding: 13,
        border: `1.5px solid ${on ? T.green : T.line}`
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "place",
      size: 22,
      color: on ? T.green : T.fg3,
      fill: on ? 1 : 0
    }), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 15,
        color: T.fg
      }
    }, t.name), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 12,
        color: T.fg3,
        marginTop: 1
      }
    }, t.distance, " km de dist\xE2ncia")), /*#__PURE__*/React.createElement(Icon, {
      name: on ? 'check_circle' : 'radio_button_unchecked',
      size: 21,
      color: on ? T.green : T.fg3,
      fill: on ? 1 : 0
    }));
  })), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '0 0 12px',
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      lineHeight: 1.5
    }
  }, "Ao continuar, voc\xEA entrar\xE1 como visitante deste territ\xF3rio."), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "lg",
    icon: "arrow_forward",
    onClick: () => onContinue(sel)
  }, "Continuar com ", selT.name));
}
function TerritorySheet({
  activeId,
  onPick,
  onClose
}) {
  return /*#__PURE__*/React.createElement(Sheet, {
    onClose: onClose,
    title: "Trocar de territ\xF3rio",
    subtitle: "Voc\xEA participa como morador ou visitante."
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 9
    }
  }, window.ARAH.territories.map(t => {
    const on = t.id === activeId;
    return /*#__PURE__*/React.createElement(Card, {
      key: t.id,
      onClick: () => onPick(t.id),
      hi: on,
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 12,
        padding: 13,
        border: `1px solid ${on ? 'rgba(166,214,185,0.35)' : T.line}`
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 40,
        height: 40,
        borderRadius: 12,
        flexShrink: 0,
        background: on ? T.greenGrad : T.cardHi,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "forest",
      size: 20,
      color: on ? '#0C1B10' : T.green,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 15,
        color: T.fg
      }
    }, t.name), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 12,
        color: T.fg3
      }
    }, t.region)), on && /*#__PURE__*/React.createElement(Icon, {
      name: "check_circle",
      size: 21,
      color: T.green,
      fill: 1
    }));
  })));
}

// Reusable bottom sheet
function Sheet({
  children,
  title,
  subtitle,
  onClose
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      zIndex: 100
    }
  }, /*#__PURE__*/React.createElement("div", {
    onClick: onClose,
    style: {
      position: 'absolute',
      inset: 0,
      background: 'rgba(0,0,0,0.6)',
      backdropFilter: 'blur(2px)',
      animation: 'fadeIn .25s ease'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
      background: 'linear-gradient(180deg, #1A1D17, #131510)',
      borderRadius: '26px 26px 0 0',
      padding: '12px 18px 34px',
      maxHeight: '80%',
      overflow: 'auto',
      border: `1px solid ${T.lineHi}`,
      borderBottom: 'none',
      animation: 'sheetUp .32s cubic-bezier(.16,1,.3,1)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 38,
      height: 4,
      borderRadius: 999,
      background: T.lineHi,
      margin: '0 auto 16px'
    }
  }), title && /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 19,
      color: T.fg,
      marginBottom: 4,
      letterSpacing: -0.3
    }
  }, title), subtitle && /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg3,
      marginBottom: 16
    }
  }, subtitle), children));
}
Object.assign(window, {
  MapCanvas,
  MapScreen,
  LoginScreen,
  OnboardingScreen,
  TerritorySheet,
  Sheet
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens4.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens5.jsx
try { (() => {
// screens5.jsx — Mercado (marketplace + compra coletiva) + Mensagens (chat). NEW (proposed).

function MarketScreen({
  onJoinGroup,
  joinedGroups
}) {
  const [tab, setTab] = React.useState('mercado');
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 12px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'mercado',
    onClick: () => setTab('mercado'),
    icon: "storefront"
  }, "Mercado local"), /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'coletiva',
    onClick: () => setTab('coletiva'),
    icon: "groups"
  }, "Compra coletiva")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      marginBottom: 14,
      padding: '9px 12px',
      borderRadius: 12,
      background: T.blueDim
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "auto_awesome",
    size: 16,
    color: T.blue,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, "Novo na Arah \u2014 economia local do territ\xF3rio.")), tab === 'mercado' ? /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 11
    }
  }, window.ARAH.market.map(m => /*#__PURE__*/React.createElement(Card, {
    key: m.id,
    glow: true,
    onClick: () => window.openJourney('checkout', m),
    style: {
      padding: 0,
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: 84,
      background: `linear-gradient(150deg, ${m.avatar}66, ${m.avatar}22)`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      position: 'relative'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: m.icon,
    size: 34,
    color: T.fg,
    fill: 0,
    style: {
      opacity: 0.9
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: 8,
      left: 8,
      fontFamily: T.sans,
      fontSize: 10,
      fontWeight: 600,
      color: T.fg,
      background: 'rgba(11,12,10,0.5)',
      backdropFilter: 'blur(4px)',
      padding: '3px 8px',
      borderRadius: 999
    }
  }, m.tag)), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 12
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14,
      color: T.fg,
      lineHeight: 1.25,
      letterSpacing: -0.2
    }
  }, m.title), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 5,
      marginTop: 6,
      color: T.fg3,
      fontFamily: T.sans,
      fontSize: 11.5
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: m.avatar,
    name: m.seller,
    size: 16
  }), " ", m.seller), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      marginTop: 10
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 15,
      color: T.green
    }
  }, m.price), /*#__PURE__*/React.createElement("button", {
    onClick: e => {
      e.stopPropagation();
      window.openJourney('checkout', m);
    },
    style: {
      width: 32,
      height: 32,
      borderRadius: 10,
      border: 'none',
      background: T.greenDim,
      cursor: 'pointer',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "add_shopping_cart",
    size: 17,
    color: T.green
  }))))))) : /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 12
    }
  }, window.ARAH.groupBuys.map(g => {
    const joined = joinedGroups[g.id];
    const count = g.joined + (joined ? 1 : 0);
    const pct = Math.min(100, Math.round(count / g.goal * 100));
    return /*#__PURE__*/React.createElement(Card, {
      key: g.id,
      glow: true,
      style: {
        padding: 16
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 12,
        marginBottom: 13
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 46,
        height: 46,
        borderRadius: 13,
        background: T.greenDim,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexShrink: 0
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: g.icon,
      size: 24,
      color: T.green,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 15.5,
        color: T.fg,
        letterSpacing: -0.2,
        lineHeight: 1.25
      }
    }, g.title), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 12,
        color: T.fg3,
        marginTop: 2
      }
    }, "Organizado por ", g.org, " \xB7 ", g.deadline))), /*#__PURE__*/React.createElement("div", {
      style: {
        height: 8,
        borderRadius: 999,
        background: T.cardHi,
        overflow: 'hidden'
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: `${pct}%`,
        height: '100%',
        borderRadius: 999,
        background: T.greenGrad,
        transition: 'width .4s var(--ease)'
      }
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginTop: 9
      }
    }, /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 12.5,
        color: T.fg2
      }
    }, /*#__PURE__*/React.createElement("strong", {
      style: {
        color: T.fg,
        fontWeight: 600
      }
    }, count), " de ", g.goal, " ", g.unit, " \xB7 ", pct, "%"), /*#__PURE__*/React.createElement(Btn, {
      kind: joined ? 'dark' : 'primary',
      size: "sm",
      icon: joined ? 'check' : 'group_add',
      onClick: () => window.openJourney('groupBuy', {
        ...window.ARAH.groupBuyDetail,
        title: g.title,
        icon: g.icon,
        onConfirm: () => onJoinGroup(g.id)
      })
    }, joined ? 'Participando' : 'Participar')));
  })));
}
function SegTab({
  children,
  active,
  onClick,
  icon
}) {
  return /*#__PURE__*/React.createElement("button", {
    onClick: onClick,
    style: {
      flex: 1,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 7,
      padding: '11px',
      borderRadius: 13,
      cursor: 'pointer',
      WebkitTapHighlightColor: 'transparent',
      background: active ? T.greenDim : T.cardFlat,
      color: active ? T.green : T.fg2,
      border: `1px solid ${active ? 'transparent' : T.line}`,
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      transition: 'all .15s'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 17,
    fill: active ? 1 : 0
  }), " ", children);
}

// Chat list
function ChatListScreen({
  onOpen
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 12px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 4
    }
  }, window.ARAH.chats.map(c => /*#__PURE__*/React.createElement("button", {
    key: c.id,
    onClick: () => onOpen(c.id),
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: '12px 6px',
      textAlign: 'left',
      cursor: 'pointer',
      background: 'none',
      border: 'none',
      borderBottom: `1px solid ${T.line}`,
      WebkitTapHighlightColor: 'transparent'
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: c.avatar,
    name: c.name,
    size: 48,
    resident: c.role === 'morador'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, c.name), /*#__PURE__*/React.createElement("span", {
    style: {
      marginLeft: 'auto',
      fontFamily: T.sans,
      fontSize: 11.5,
      color: c.unread ? T.green : T.fg3
    }
  }, c.time)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      marginTop: 2
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 13,
      color: c.unread ? T.fg : T.fg3,
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap',
      fontWeight: c.unread ? 500 : 400
    }
  }, c.last), c.unread > 0 && /*#__PURE__*/React.createElement("span", {
    style: {
      minWidth: 18,
      height: 18,
      padding: '0 5px',
      borderRadius: 999,
      background: T.greenSolid,
      color: '#0C1B10',
      fontFamily: T.sans,
      fontSize: 11,
      fontWeight: 700,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, c.unread)))))));
}

// Chat thread
function ChatThreadScreen({
  chatId
}) {
  const chat = window.ARAH.chats.find(c => c.id === chatId);
  const [msgs, setMsgs] = React.useState(chat.thread);
  const [draft, setDraft] = React.useState('');
  const scrollRef = React.useRef(null);
  const send = () => {
    if (!draft.trim()) return;
    setMsgs(m => [...m, {
      me: true,
      body: draft.trim(),
      time: 'agora'
    }]);
    setDraft('');
  };
  React.useEffect(() => {
    if (scrollRef.current) scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
  }, [msgs]);
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%'
    }
  }, /*#__PURE__*/React.createElement("div", {
    ref: scrollRef,
    className: "appscroll",
    style: {
      flex: 1,
      overflowY: 'auto',
      padding: '4px 16px 12px',
      display: 'flex',
      flexDirection: 'column',
      gap: 9
    }
  }, msgs.map((m, i) => /*#__PURE__*/React.createElement("div", {
    key: i,
    style: {
      alignSelf: m.me ? 'flex-end' : 'flex-start',
      maxWidth: '78%'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '10px 14px',
      borderRadius: m.me ? '16px 16px 4px 16px' : '16px 16px 16px 4px',
      background: m.me ? T.greenGrad : T.cardGrad,
      color: m.me ? '#0C1B10' : T.fg,
      border: m.me ? 'none' : `1px solid ${T.line}`,
      fontFamily: T.sans,
      fontSize: 14.5,
      lineHeight: 1.45
    }
  }, m.body), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 10.5,
      color: T.fg3,
      marginTop: 3,
      textAlign: m.me ? 'right' : 'left',
      padding: '0 4px'
    }
  }, m.time)))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      padding: '12px 16px 16px',
      borderTop: `1px solid ${T.line}`,
      background: T.bg2
    }
  }, /*#__PURE__*/React.createElement("button", {
    style: {
      ...iconBtn,
      width: 42,
      height: 42,
      borderRadius: '50%'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "add",
    size: 22,
    color: T.fg2
  })), /*#__PURE__*/React.createElement("input", {
    value: draft,
    onChange: e => setDraft(e.target.value),
    onKeyDown: e => e.key === 'Enter' && send(),
    placeholder: "Mensagem\u2026",
    style: {
      flex: 1,
      background: T.cardFlat,
      color: T.fg,
      border: `1px solid ${T.line}`,
      borderRadius: 999,
      padding: '11px 16px',
      fontFamily: T.sans,
      fontSize: 14,
      outline: 'none'
    }
  }), /*#__PURE__*/React.createElement("button", {
    onClick: send,
    style: {
      width: 42,
      height: 42,
      borderRadius: '50%',
      border: 'none',
      background: T.greenGrad,
      cursor: 'pointer',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      boxShadow: T.greenGlow,
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "send",
    size: 19,
    color: "#0C1B10",
    fill: 1
  }))));
}
Object.assign(window, {
  MarketScreen,
  ChatListScreen,
  ChatThreadScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens5.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens6.jsx
try { (() => {
// screens6.jsx — Eleições/Votação (NEW, visual) + Gestão do território (curador journey).

function ElectionsScreen({
  role
}) {
  const [choice, setChoice] = React.useState({}); // selected candidate per election (not yet final)
  const [voted, setVoted] = React.useState({}); // confirmed votes per election
  const elections = [{
    id: 'el1',
    title: 'Conselho de Moradores 2026',
    status: 'open',
    ends: 'encerra em 3 dias',
    voters: 412,
    desc: 'Escolha 1 representante para o conselho do território.',
    candidates: [{
      id: 'c1',
      name: 'Dona Marta',
      avatar: '#C9962B',
      role: 'morador',
      pitch: 'Pesca artesanal e feira',
      pct: 38
    }, {
      id: 'c2',
      name: 'Seu Tião',
      avatar: '#4F956F',
      role: 'morador',
      pitch: 'Saneamento e águas',
      pct: 34
    }, {
      id: 'c3',
      name: 'Bruno Caiçara',
      avatar: '#377B57',
      role: 'morador',
      pitch: 'Trilhas e turismo de base',
      pct: 28
    }]
  }, {
    id: 'el2',
    title: 'Representante de águas',
    status: 'closed',
    ends: 'encerrada',
    voters: 388,
    desc: 'Resultado final da votação.',
    winner: 'Coletivo Maré',
    candidates: [{
      id: 'c4',
      name: 'Coletivo Maré',
      avatar: '#2A6FDB',
      role: 'morador',
      pitch: 'Captação da nascente',
      pct: 61,
      won: true
    }, {
      id: 'c5',
      name: 'Assoc. de Pescadores',
      avatar: '#4F956F',
      role: 'morador',
      pitch: 'Poços comunitários',
      pct: 39
    }]
  }];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      marginBottom: 14,
      padding: '9px 12px',
      borderRadius: 12,
      background: T.alertDim
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "auto_awesome",
    size: 16,
    color: T.alert,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, "Nova feature \u2014 governan\xE7a comunit\xE1ria do territ\xF3rio.")), role === 'visitante' && /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 13,
      marginBottom: 14,
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "lock",
    size: 18,
    color: T.fg3
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2
    }
  }, "S\xF3 moradores votam. Confirme resid\xEAncia para participar.")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 14
    }
  }, elections.map(el => {
    const myVote = choice[el.id];
    const hasVoted = !!voted[el.id];
    const showResults = el.status === 'closed' || hasVoted;
    const canVote = role !== 'visitante' && el.status === 'open';
    return /*#__PURE__*/React.createElement(Card, {
      key: el.id,
      glow: true,
      style: {
        padding: 16
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'flex-start',
        gap: 10,
        marginBottom: 4
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 16.5,
        color: T.fg,
        letterSpacing: -0.3
      }
    }, el.title), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 12.5,
        color: T.fg2,
        marginTop: 3
      }
    }, el.desc)), /*#__PURE__*/React.createElement("span", {
      style: {
        display: 'inline-flex',
        alignItems: 'center',
        gap: 4,
        padding: '4px 10px',
        borderRadius: 999,
        flexShrink: 0,
        background: el.status === 'open' ? T.greenDim : 'rgba(166,172,158,0.12)',
        color: el.status === 'open' ? T.green : T.fg3,
        fontFamily: T.sans,
        fontSize: 11,
        fontWeight: 600
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: el.status === 'open' ? 'how_to_vote' : 'lock',
      size: 13,
      fill: 1
    }), " ", el.status === 'open' ? 'Aberta' : 'Encerrada')), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 11.5,
        color: T.fg3,
        marginBottom: 13
      }
    }, el.ends, " \xB7 ", el.voters, " votos"), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        flexDirection: 'column',
        gap: 9
      }
    }, el.candidates.map(c => {
      const selected = myVote === c.id;
      return /*#__PURE__*/React.createElement("button", {
        key: c.id,
        disabled: !canVote || hasVoted,
        onClick: () => canVote && !hasVoted && setChoice(v => ({
          ...v,
          [el.id]: c.id
        })),
        style: {
          position: 'relative',
          display: 'flex',
          alignItems: 'center',
          gap: 11,
          padding: 11,
          textAlign: 'left',
          borderRadius: 13,
          cursor: canVote ? 'pointer' : 'default',
          WebkitTapHighlightColor: 'transparent',
          overflow: 'hidden',
          background: T.cardFlat,
          border: `1px solid ${selected || c.won ? c.won ? T.green : 'rgba(166,214,185,0.4)' : T.line}`
        }
      }, showResults && /*#__PURE__*/React.createElement("div", {
        style: {
          position: 'absolute',
          inset: 0,
          width: `${c.pct}%`,
          background: c.won ? 'rgba(166,214,185,0.16)' : 'rgba(166,214,185,0.08)'
        }
      }), /*#__PURE__*/React.createElement(Avatar, {
        color: c.avatar,
        name: c.name,
        size: 38,
        resident: true,
        style: {
          position: 'relative'
        }
      }), /*#__PURE__*/React.createElement("div", {
        style: {
          flex: 1,
          minWidth: 0,
          position: 'relative'
        }
      }, /*#__PURE__*/React.createElement("div", {
        style: {
          display: 'flex',
          alignItems: 'center',
          gap: 6
        }
      }, /*#__PURE__*/React.createElement("span", {
        style: {
          fontFamily: T.display,
          fontWeight: 600,
          fontSize: 14,
          color: T.fg
        }
      }, c.name), c.won && /*#__PURE__*/React.createElement(Icon, {
        name: "emoji_events",
        size: 15,
        color: T.green,
        fill: 1
      })), /*#__PURE__*/React.createElement("div", {
        style: {
          fontFamily: T.sans,
          fontSize: 12,
          color: T.fg3,
          marginTop: 1
        }
      }, c.pitch)), /*#__PURE__*/React.createElement("div", {
        style: {
          position: 'relative',
          display: 'flex',
          alignItems: 'center',
          gap: 8
        }
      }, showResults && /*#__PURE__*/React.createElement("span", {
        style: {
          fontFamily: T.display,
          fontWeight: 700,
          fontSize: 15,
          color: c.won ? T.green : T.fg2
        }
      }, c.pct, "%"), canVote && /*#__PURE__*/React.createElement(Icon, {
        name: selected ? 'radio_button_checked' : 'radio_button_unchecked',
        size: 22,
        color: selected ? T.green : T.fg3,
        fill: selected ? 1 : 0
      })));
    })), canVote && /*#__PURE__*/React.createElement(Btn, {
      kind: "primary",
      full: true,
      size: "md",
      icon: hasVoted ? 'check_circle' : 'how_to_vote',
      style: {
        marginTop: 13,
        opacity: hasVoted || myVote ? 1 : 0.5
      },
      onClick: () => {
        if (hasVoted || !myVote) return;
        const cand = el.candidates.find(x => x.id === myVote);
        window.openJourney('vote', {
          election: el,
          candidate: cand,
          onConfirm: () => setVoted(v => ({
            ...v,
            [el.id]: true
          }))
        });
      }
    }, hasVoted ? 'Voto registrado' : myVote ? 'Confirmar voto' : 'Escolha um candidato'));
  })));
}

// Território management — curador journey
function ManageScreen({
  territory
}) {
  const [done, setDone] = React.useState({});
  const queue = [{
    id: 'q1',
    kind: 'Entidade',
    icon: 'landscape',
    label: 'Mirante da Pedra',
    sub: 'Sugerido por Bruno · validar local',
    color: T.green
  }, {
    id: 'q2',
    kind: 'Post',
    icon: 'flag',
    label: 'Post sinalizado',
    sub: 'Possível spam · revisar',
    color: T.alert
  }, {
    id: 'q3',
    kind: 'Membro',
    icon: 'how_to_reg',
    label: 'Confirmar residência',
    sub: 'Ana Ribeiro · comprovante enviado',
    color: T.blue
  }];
  const stats = [{
    label: 'Moradores',
    val: '1.284',
    icon: 'groups'
  }, {
    label: 'Na curadoria',
    val: '3',
    icon: 'pending_actions'
  }, {
    label: 'Entidades',
    val: '6',
    icon: 'location_on'
  }];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(Card, {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      padding: 14,
      marginBottom: 16,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 40,
      height: 40,
      borderRadius: 12,
      background: T.alertDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "shield_person",
    size: 21,
    color: T.alert,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      color: T.fg
    }
  }, "Voc\xEA \xE9 curador de ", territory.name), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      marginTop: 1
    }
  }, "Cuide do feed, do mapa e da comunidade."))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr 1fr',
      gap: 10,
      marginBottom: 20
    }
  }, stats.map(s => /*#__PURE__*/React.createElement(Card, {
    key: s.label,
    style: {
      padding: '14px 8px',
      textAlign: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: s.icon,
    size: 20,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.fg,
      marginTop: 6
    }
  }, s.val), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 10.5,
      color: T.fg3,
      marginTop: 1
    }
  }, s.label)))), /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "rule"
  }, "Fila de curadoria"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10,
      marginTop: 11,
      marginBottom: 20
    }
  }, queue.filter(q => !done[q.id]).map(q => /*#__PURE__*/React.createElement(Card, {
    key: q.id,
    style: {
      padding: 14
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      marginBottom: 12
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 38,
      height: 38,
      borderRadius: 11,
      background: `${q.color}1f`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: q.icon,
    size: 20,
    color: q.color,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 10,
      fontWeight: 600,
      letterSpacing: 0.6,
      textTransform: 'uppercase',
      color: q.color
    }
  }, q.kind)), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14.5,
      color: T.fg,
      marginTop: 1
    }
  }, q.label), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      marginTop: 1
    }
  }, q.sub))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    size: "sm",
    icon: "check",
    style: {
      flex: 1
    },
    onClick: () => setDone(d => ({
      ...d,
      [q.id]: true
    }))
  }, "Aprovar"), /*#__PURE__*/React.createElement(Btn, {
    kind: "dark",
    size: "sm",
    icon: "close",
    style: {
      flex: 1,
      color: T.alert
    },
    onClick: () => setDone(d => ({
      ...d,
      [q.id]: true
    }))
  }, "Recusar")))), queue.every(q => done[q.id]) && /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 22,
      textAlign: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "task_alt",
    size: 28,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      color: T.fg2,
      marginTop: 8
    }
  }, "Fila de curadoria zerada. Bom trabalho!"))), /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "how_to_vote"
  }, "Elei\xE7\xF5es do territ\xF3rio"), /*#__PURE__*/React.createElement(Card, {
    onClick: () => window.openJourney('createElection'),
    style: {
      padding: 14,
      marginTop: 11,
      display: 'flex',
      alignItems: 'center',
      gap: 11
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 38,
      height: 38,
      borderRadius: 11,
      background: T.alertDim,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "add",
    size: 20,
    color: T.alert
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 13.5,
      fontWeight: 600,
      color: T.fg
    }
  }, "Abrir nova elei\xE7\xE3o"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, "Conselho \xB7 representantes \xB7 consultas")), /*#__PURE__*/React.createElement(Icon, {
    name: "chevron_right",
    size: 20,
    color: T.fg3
  })));
}
Object.assign(window, {
  ElectionsScreen,
  ManageScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens6.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens7.jsx
try { (() => {
// screens7.jsx — Minha loja: abrir loja, vender produtos, configuração de pagamento. NEW (proposed).

function StoreScreen() {
  const store = window.ARAH.myStore;
  const [open, setOpen] = React.useState(store.open);
  const [products, setProducts] = React.useState(store.products);
  const [pay, setPay] = React.useState(store.payment);
  if (!open) {
    return /*#__PURE__*/React.createElement("div", {
      style: {
        padding: '0 18px 16px'
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 8,
        marginBottom: 16,
        padding: '9px 12px',
        borderRadius: 12,
        background: T.greenDim
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "auto_awesome",
      size: 16,
      color: T.green,
      fill: 1
    }), /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 12,
        color: T.fg2,
        lineHeight: 1.4
      }
    }, "Novo na Arah \u2014 venda no mercado do seu territ\xF3rio.")), /*#__PURE__*/React.createElement(Card, {
      glow: true,
      style: {
        padding: 22,
        textAlign: 'center',
        marginBottom: 18
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 64,
        height: 64,
        borderRadius: 18,
        background: T.greenDim,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        margin: '0 auto 14px'
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "storefront",
      size: 32,
      color: T.green,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 700,
        fontSize: 21,
        color: T.fg,
        letterSpacing: -0.4
      }
    }, "Abra sua loja"), /*#__PURE__*/React.createElement("p", {
      style: {
        margin: '8px auto 0',
        fontFamily: T.sans,
        fontSize: 14,
        lineHeight: 1.5,
        color: T.fg2,
        maxWidth: 290
      }
    }, "Venda produtos e servi\xE7os direto para a sua comunidade. Sem intermedi\xE1rio, com pagamento via PIX.")), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        flexDirection: 'column',
        gap: 11,
        marginBottom: 22
      }
    }, [['sell', 'Anuncie produtos e serviços', 'Comida, artesanato, passeios, aulas.'], ['account_balance_wallet', 'Receba via PIX', 'Sem taxa entre moradores do território.'], ['verified_user', 'Confiança local', 'Compradores e vendedores do mesmo território.']].map(([ic, t, s]) => /*#__PURE__*/React.createElement("div", {
      key: t,
      style: {
        display: 'flex',
        gap: 13,
        alignItems: 'center'
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 40,
        height: 40,
        borderRadius: 12,
        background: T.cardHi,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexShrink: 0
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: ic,
      size: 20,
      color: T.green,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 14,
        fontWeight: 600,
        color: T.fg
      }
    }, t), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 12.5,
        color: T.fg3,
        marginTop: 1
      }
    }, s))))), /*#__PURE__*/React.createElement(Btn, {
      kind: "primary",
      full: true,
      size: "lg",
      icon: "add_business",
      onClick: () => setOpen(true)
    }, "Abrir minha loja"));
  }
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(Card, {
    glow: true,
    style: {
      padding: 16,
      marginBottom: 16,
      background: T.cardHiGrad
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 12
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 52,
      height: 52,
      borderRadius: 15,
      background: T.greenGrad,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "storefront",
    size: 26,
    color: "#0C1B10",
    fill: 1
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 17,
      color: T.fg,
      letterSpacing: -0.3
    }
  }, store.name), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4,
      marginTop: 3,
      fontFamily: T.sans,
      fontSize: 12,
      color: T.green
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "circle",
    size: 9,
    fill: 1
  }), " Loja ativa em Camburi"))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 10,
      marginTop: 14
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      background: 'rgba(0,0,0,0.18)',
      borderRadius: 13,
      padding: '11px 12px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 17,
      color: T.fg
    }
  }, store.sales.month), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11,
      color: T.fg3,
      marginTop: 1
    }
  }, "Vendas no m\xEAs")), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      background: 'rgba(0,0,0,0.18)',
      borderRadius: 13,
      padding: '11px 12px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 17,
      color: T.fg
    }
  }, store.sales.orders), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11,
      color: T.fg3,
      marginTop: 1
    }
  }, "Pedidos")))), /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "account_balance_wallet"
  }, "Pagamento"), /*#__PURE__*/React.createElement(Card, {
    style: {
      overflow: 'hidden',
      margin: '11px 0 22px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: '14px 15px',
      borderBottom: `1px solid ${T.line}`
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "pix",
    size: 20,
    color: T.water,
    fill: 1
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg
    }
  }, "Chave PIX"), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.mono || 'monospace',
      fontSize: 12,
      color: T.fg3,
      marginTop: 1
    }
  }, pay.pix)), /*#__PURE__*/React.createElement(Icon, {
    name: "edit",
    size: 18,
    color: T.fg3
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '14px 15px',
      borderBottom: `1px solid ${T.line}`
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg,
      marginBottom: 9
    }
  }, "Formas de pagamento"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      flexWrap: 'wrap'
    }
  }, ['PIX', 'Cartão', 'Dinheiro'].map(m => {
    const on = pay.methods.includes(m);
    return /*#__PURE__*/React.createElement("button", {
      key: m,
      onClick: () => setPay(p => ({
        ...p,
        methods: on ? p.methods.filter(x => x !== m) : [...p.methods, m]
      })),
      style: {
        display: 'inline-flex',
        alignItems: 'center',
        gap: 6,
        padding: '7px 13px',
        borderRadius: 999,
        cursor: 'pointer',
        background: on ? T.greenDim : T.cardFlat,
        color: on ? T.green : T.fg2,
        border: `1px solid ${on ? 'transparent' : T.line}`,
        fontFamily: T.sans,
        fontSize: 13,
        fontWeight: 500
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: on ? 'check' : 'add',
      size: 15
    }), " ", m);
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: '14px 15px',
      borderBottom: `1px solid ${T.line}`
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "percent",
    size: 20,
    color: T.fg2
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg
    }
  }, "Taxa entre moradores"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 14,
      color: T.green
    }
  }, pay.fee)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: '14px 15px'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "account_balance",
    size: 20,
    color: T.fg2
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 14,
      color: T.fg
    }
  }, "Conta de repasse"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3
    }
  }, pay.payout))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      marginBottom: 11
    }
  }, /*#__PURE__*/React.createElement(SectionLabel, {
    icon: "inventory_2"
  }, "Meus produtos"), /*#__PURE__*/React.createElement("button", {
    onClick: () => window.openJourney('addProduct', {
      onConfirm: prod => {
        setProducts(p => [prod, ...p]);
        window.arahMutate(() => {
          window.ARAH.myStore.products.unshift(prod);
        });
      }
    }),
    style: {
      marginLeft: 'auto',
      display: 'inline-flex',
      alignItems: 'center',
      gap: 5,
      background: 'none',
      border: 'none',
      cursor: 'pointer',
      color: T.green,
      fontFamily: T.sans,
      fontSize: 13,
      fontWeight: 600
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "add",
    size: 17
  }), " Adicionar")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, products.map(p => /*#__PURE__*/React.createElement(Card, {
    key: p.id,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 13,
      padding: 13
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 44,
      height: 44,
      borderRadius: 12,
      background: T.cardHi,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: p.icon,
    size: 22,
    color: T.green,
    fill: 0
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 14.5,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, p.title), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3,
      marginTop: 1
    }
  }, p.tag, " \xB7 ", p.price)), /*#__PURE__*/React.createElement("button", {
    onClick: () => setProducts(ps => ps.map(x => x.id === p.id ? {
      ...x,
      active: !x.active
    } : x)),
    style: {
      width: 46,
      height: 27,
      borderRadius: 999,
      border: 'none',
      cursor: 'pointer',
      position: 'relative',
      flexShrink: 0,
      background: p.active ? T.greenSolid : T.cardHi,
      transition: 'background .2s'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: 3,
      left: p.active ? 22 : 3,
      width: 21,
      height: 21,
      borderRadius: '50%',
      background: '#fff',
      transition: 'left .2s'
    }
  }))))));
}
Object.assign(window, {
  StoreScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens7.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens8.jsx
try { (() => {
// screens8.jsx — Serviços hub: full feature directory by category + status badges.
// The gateway to the entire backlog, visually previewed.

function ServicesHubScreen({
  territory,
  onOpen,
  role
}) {
  const cats = window.ARAH.serviceCategories;
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 14px'
    }
  }, /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '0 0 18px',
      fontFamily: T.sans,
      fontSize: 14,
      lineHeight: 1.5,
      color: T.fg2
    }
  }, "Tudo que a vida em ", /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.green,
      fontWeight: 600
    }
  }, territory.name), " precisa \u2014 economia, servi\xE7os, governan\xE7a e cuidado com o lugar."), cats.map(cat => /*#__PURE__*/React.createElement("div", {
    key: cat.id,
    style: {
      marginBottom: 24
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      marginBottom: 13
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 34,
      height: 34,
      borderRadius: 10,
      background: `${cat.color}1f`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: cat.icon,
    size: 19,
    color: cat.color,
    fill: 1
  })), /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, cat.label), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3
    }
  }, cat.desc))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 10
    }
  }, cat.items.map(it => /*#__PURE__*/React.createElement(Card, {
    key: it.id,
    onClick: () => onOpen(it.id),
    style: {
      padding: 13,
      position: 'relative'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      marginBottom: 9
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 34,
      height: 34,
      borderRadius: 10,
      background: it.status === 'live' ? T.greenDim : 'rgba(255,255,255,0.05)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: it.icon,
    size: 19,
    color: it.status === 'live' ? T.green : T.fg2,
    fill: it.status === 'live' ? 1 : 0
  })), it.status === 'live' ? /*#__PURE__*/React.createElement("span", {
    style: statusPill(T.green, T.greenDim)
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "check",
    size: 11
  }), "No ar") : /*#__PURE__*/React.createElement("span", {
    style: statusPill(T.amber || '#E8A06A', T.alertDim)
  }, "Em breve")), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 13.5,
      color: T.fg,
      letterSpacing: -0.2,
      lineHeight: 1.2
    }
  }, it.label), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 11.5,
      color: T.fg3,
      marginTop: 2
    }
  }, it.sub), it.phase && /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 10,
      color: T.fg3,
      marginTop: 6,
      opacity: 0.8
    }
  }, it.phase)))))), /*#__PURE__*/React.createElement(Card, {
    style: {
      padding: 15,
      display: 'flex',
      alignItems: 'center',
      gap: 11,
      background: T.cardHiGrad,
      marginTop: 4
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "map",
    size: 20,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, "Roadmap aberto: 16 de 48 fases entregues. As funcionalidades chegam em ondas, no ritmo da comunidade.")));
}
function statusPill(color, bg) {
  return {
    marginLeft: 'auto',
    display: 'inline-flex',
    alignItems: 'center',
    gap: 3,
    padding: '3px 8px',
    borderRadius: 999,
    background: bg,
    color,
    fontFamily: T.sans,
    fontSize: 10,
    fontWeight: 700,
    letterSpacing: 0.2
  };
}

// Reusable "preview / roadmap" banner shown atop soon-features
function SoonBanner({
  phase,
  children
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      marginBottom: 16,
      padding: '10px 13px',
      borderRadius: 13,
      background: T.alertDim
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "auto_awesome",
    size: 16,
    color: T.amber || '#E8A06A',
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg2,
      lineHeight: 1.4
    }
  }, children), phase && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 10,
      fontWeight: 700,
      color: T.amber || '#E8A06A',
      background: 'rgba(232,160,106,0.16)',
      padding: '3px 8px',
      borderRadius: 999,
      flexShrink: 0
    }
  }, phase));
}

// Generic section sub-header reused across feature screens
function SubHead({
  children,
  icon
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      margin: '4px 0 12px'
    }
  }, icon && /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: 18,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 15.5,
      color: T.fg,
      letterSpacing: -0.2
    }
  }, children));
}
Object.assign(window, {
  ServicesHubScreen,
  SoonBanner,
  SubHead
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens8.jsx", error: String((e && e.message) || e) }); }

// ui_kits/app/screens9.jsx
try { (() => {
// screens9.jsx — Economia: Compra Coletiva, Hospedagem, Demandas & Ofertas.

function GroupBuyScreen() {
  const g = window.ARAH.groupBuyDetail;
  const [joined, setJoined] = React.useState(false);
  const count = g.joined + (joined ? 1 : 0);
  const pct = Math.min(100, Math.round(count / g.goal * 100));
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 17"
  }, "Compra coletiva \u2014 quanto mais gente, menor o pre\xE7o. Vis\xE3o de produto."), /*#__PURE__*/React.createElement(Card, {
    glow: true,
    style: {
      padding: 0,
      overflow: 'hidden',
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: 130,
      background: 'linear-gradient(150deg, rgba(166,214,185,0.25), rgba(129,199,132,0.08))',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: g.icon,
    size: 56,
    color: T.green,
    fill: 1,
    style: {
      opacity: 0.9
    }
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 18
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 19,
      color: T.fg,
      letterSpacing: -0.4,
      lineHeight: 1.2
    }
  }, g.title), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3,
      marginTop: 4
    }
  }, "Organizado por ", g.org, " \xB7 ", g.deadline), /*#__PURE__*/React.createElement("p", {
    style: {
      margin: '12px 0 0',
      fontFamily: T.sans,
      fontSize: 14,
      lineHeight: 1.5,
      color: T.fg2
    }
  }, g.desc), /*#__PURE__*/React.createElement("div", {
    style: {
      marginTop: 16,
      height: 10,
      borderRadius: 999,
      background: T.cardHi,
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: `${pct}%`,
      height: '100%',
      borderRadius: 999,
      background: T.greenGrad,
      transition: 'width .4s'
    }
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      marginTop: 9
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg2
    }
  }, /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.fg,
      fontWeight: 700
    }
  }, count), "/", g.goal, " ", g.unit), /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'flex',
      alignItems: 'center'
    }
  }, g.participants.map((c, i) => /*#__PURE__*/React.createElement("span", {
    key: i,
    style: {
      marginLeft: i ? -8 : 0
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: c,
    name: "A B",
    size: 24
  }))), /*#__PURE__*/React.createElement("span", {
    style: {
      marginLeft: 6,
      fontFamily: T.sans,
      fontSize: 12,
      color: T.fg3
    }
  }, "+", count - g.participants.length))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      marginTop: 16
    }
  }, g.tiers.map((t, i) => {
    const active = count >= t.n;
    return /*#__PURE__*/React.createElement("div", {
      key: i,
      style: {
        flex: 1,
        padding: '11px 8px',
        borderRadius: 13,
        textAlign: 'center',
        background: active ? T.greenDim : T.cardFlat,
        border: `1px solid ${active ? 'rgba(166,214,185,0.35)' : T.line}`
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.sans,
        fontSize: 10.5,
        color: T.fg3
      }
    }, t.n, "+ fam\xEDlias"), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 700,
        fontSize: 15,
        color: active ? T.green : T.fg,
        marginTop: 3
      }
    }, t.price));
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      marginTop: 14,
      padding: '10px 13px',
      borderRadius: 12,
      background: 'rgba(166,214,185,0.1)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "savings",
    size: 18,
    color: T.green,
    fill: 1
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg2
    }
  }, "Economia estimada de ", /*#__PURE__*/React.createElement("strong", {
    style: {
      color: T.green,
      fontWeight: 700
    }
  }, g.saved), " por fam\xEDlia.")), /*#__PURE__*/React.createElement(Btn, {
    kind: joined ? 'dark' : 'primary',
    full: true,
    size: "lg",
    icon: joined ? 'check_circle' : 'group_add',
    style: {
      marginTop: 16
    },
    onClick: () => setJoined(j => !j)
  }, joined ? 'Você está participando' : 'Entrar na compra coletiva'))));
}
function HostingScreen() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 18"
  }, "Hospedagem no territ\xF3rio \u2014 estadias com quem \xE9 da vila. Vis\xE3o de produto."), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 13
    }
  }, window.ARAH.hosting.map(h => /*#__PURE__*/React.createElement(Card, {
    key: h.id,
    glow: true,
    style: {
      padding: 0,
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: 128,
      background: `linear-gradient(150deg, ${h.avatar}40, ${h.avatar}12)`,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      position: 'relative'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: h.icon,
    size: 48,
    color: T.fg,
    fill: 0,
    style: {
      opacity: 0.85
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: 10,
      left: 10,
      fontFamily: T.sans,
      fontSize: 11,
      fontWeight: 600,
      color: T.fg,
      background: 'rgba(11,12,10,0.55)',
      backdropFilter: 'blur(4px)',
      padding: '4px 10px',
      borderRadius: 999
    }
  }, h.tag), /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: 10,
      right: 10,
      display: 'inline-flex',
      alignItems: 'center',
      gap: 3,
      fontFamily: T.sans,
      fontSize: 11.5,
      fontWeight: 600,
      color: T.fg,
      background: 'rgba(11,12,10,0.55)',
      backdropFilter: 'blur(4px)',
      padding: '4px 10px',
      borderRadius: 999
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "star",
    size: 13,
    color: "#F5C451",
    fill: 1
  }), " ", h.rating)), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 15
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: T.display,
      fontWeight: 600,
      fontSize: 16,
      color: T.fg,
      letterSpacing: -0.3
    }
  }, h.title), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7,
      marginTop: 7
    }
  }, /*#__PURE__*/React.createElement(Avatar, {
    color: h.avatar,
    name: h.host,
    size: 22,
    resident: true
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 12.5,
      color: T.fg3
    }
  }, h.host, " \xB7 ", h.guests, " h\xF3spedes \xB7 ", h.reviews, " avalia\xE7\xF5es")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      marginTop: 13
    }
  }, /*#__PURE__*/React.createElement("span", null, /*#__PURE__*/React.createElement("strong", {
    style: {
      fontFamily: T.display,
      fontWeight: 700,
      fontSize: 18,
      color: T.fg
    }
  }, h.price), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: T.sans,
      fontSize: 13,
      color: T.fg3
    }
  }, " / ", h.unit)), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    size: "sm",
    icon: "calendar_month",
    onClick: () => window.openJourney('reserva', h)
  }, "Reservar")))))));
}
function DemandsScreen() {
  const [tab, setTab] = React.useState('todos');
  let list = window.ARAH.demands;
  if (tab !== 'todos') list = list.filter(d => d.kind === tab);
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '0 18px 16px'
    }
  }, /*#__PURE__*/React.createElement(SoonBanner, {
    phase: "Fase 19"
  }, "Demandas & ofertas \u2014 quem precisa encontra quem oferece. Vis\xE3o de produto."), /*#__PURE__*/React.createElement(Btn, {
    kind: "primary",
    full: true,
    size: "md",
    icon: "add",
    style: {
      marginBottom: 14
    },
    onClick: () => window.openJourney('create', window.JOURNEY_PRESETS.demanda)
  }, "Publicar demanda ou oferta"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      marginBottom: 16
    }
  }, /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'todos',
    onClick: () => setTab('todos'),
    icon: "swap_horiz"
  }, "Tudo"), /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'demanda',
    onClick: () => setTab('demanda'),
    icon: "pan_tool"
  }, "Demandas"), /*#__PURE__*/React.createElement(SegTab, {
    active: tab === 'oferta',
    onClick: () => setTab('oferta'),
    icon: "handshake"
  }, "Ofertas")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10
    }
  }, list.map(d => {
    const isDemand = d.kind === 'demanda';
    const c = isDemand ? T.blue : T.green;
    return /*#__PURE__*/React.createElement(Card, {
      key: d.id,
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 13,
        padding: 14
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        width: 44,
        height: 44,
        borderRadius: 12,
        background: `${c}1f`,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexShrink: 0
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: d.icon,
      size: 22,
      color: c,
      fill: 1
    })), /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 7
      }
    }, /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 9.5,
        fontWeight: 700,
        letterSpacing: 0.6,
        textTransform: 'uppercase',
        color: c
      }
    }, isDemand ? 'Demanda' : 'Oferta'), /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 11,
        color: T.fg3
      }
    }, "\xB7 ", d.tag, " \xB7 ", d.time)), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: T.display,
        fontWeight: 600,
        fontSize: 14.5,
        color: T.fg,
        marginTop: 2,
        letterSpacing: -0.2
      }
    }, d.title), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        alignItems: 'center',
        gap: 6,
        marginTop: 5
      }
    }, /*#__PURE__*/React.createElement(Avatar, {
      color: d.avatar,
      name: d.who,
      size: 18
    }), /*#__PURE__*/React.createElement("span", {
      style: {
        fontFamily: T.sans,
        fontSize: 12,
        color: T.fg3
      }
    }, d.who))), /*#__PURE__*/React.createElement("button", {
      onClick: () => window.openJourney('create', window.JOURNEY_PRESETS.demanda),
      style: {
        width: 38,
        height: 38,
        borderRadius: 11,
        border: `1px solid ${T.line}`,
        background: T.cardFlat,
        cursor: 'pointer',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexShrink: 0
      }
    }, /*#__PURE__*/React.createElement(Icon, {
      name: "chat",
      size: 18,
      color: T.green
    })));
  })));
}
Object.assign(window, {
  GroupBuyScreen,
  HostingScreen,
  DemandsScreen
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/app/screens9.jsx", error: String((e && e.message) || e) }); }

// wiki/wiki.js
try { (() => {
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
    if (!found && docs.length) {
      docs[0].classList.add('active');
      id = docs[0].id.replace('doc-', '');
    }
    links.forEach(function (a) {
      a.classList.toggle('active', a.getAttribute('data-doc') === id);
    });
    window.scrollTo({
      top: 0,
      behavior: 'auto'
    });
    closeSide();
  }
  function openSide() {
    side.classList.add('open');
    scrim.classList.add('show');
  }
  function closeSide() {
    side.classList.remove('open');
    scrim.classList.remove('show');
  }

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
        var next = g.nextElementSibling,
          anyVisible = false;
        while (next && !next.classList.contains('grp')) {
          if (next.matches('a') && !next.classList.contains('hidden')) anyVisible = true;
          next = next.nextElementSibling;
        }
        g.style.display = q && !anyVisible ? 'none' : '';
      });
    });
  }
  onHash();
})();
})(); } catch (e) { __ds_ns.__errors.push({ path: "wiki/wiki.js", error: String((e && e.message) || e) }); }

})();
