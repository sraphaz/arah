// app.jsx — Arah orchestrator. Stages, tabs, overlay stack, roles, smooth transitions.

function App() {
  const _saved = (window.arahLoadApp && window.arahLoadApp()) || {};
  const [stage, setStage] = React.useState(_saved.stage || 'login');     // login | onboarding | app
  const [tab, setTab] = React.useState('feed');           // feed|explore|post|market|profile
  const [stack, setStack] = React.useState([]);           // overlay stack: {type, param}
  const [sheet, setSheet] = React.useState(false);
  const [territoryId, setTerritoryId] = React.useState(_saved.territoryId || 't1');
  const [role, setRole] = React.useState(_saved.role || 'visitante');    // visitante | morador | curador
  const [likes, setLikes] = React.useState(_saved.likes || { p1: true });
  const [interests, setInterests] = React.useState(_saved.interests || [...window.ARAH.profile.interests]);
  const [notifs, setNotifs] = React.useState(() => {
    const reads = _saved.notifReads || [];
    return window.ARAH.notifications.map(n => reads.includes(n.id) ? { ...n, read: true } : n);
  });
  const [joinedGroups, setJoinedGroups] = React.useState(_saved.joinedGroups || {});
  const [journey, setJourney] = React.useState(null); // {id, ctx}
  const [toast, setToast] = React.useState(null);
  const [, forceTick] = React.useReducer(x => x + 1, 0);

  // Global bridges — any deep screen can navigate or mutate session data
  React.useEffect(() => {
    window.openJourney = (id, ctx) => setJourney({ id, ctx });
    window.arahMutate = (fn) => { try { fn && fn(); } finally { window.arahSnapshot && window.arahSnapshot(); forceTick(); } };
    window.arahToast = (msg, icon) => { setToast({ msg, icon: icon || 'check_circle' }); setTimeout(() => setToast(null), 2200); };
    window.appNav = {
      goTab: (t) => { setStack([]); setTab(t); },
      push: (type, param) => setStack(s => [...s, { type, param }]),
      openChat: (name, avatar) => {
        let c = window.ARAH.chats.find(x => x.name === name);
        if (!c) { c = { id: 'ch' + Date.now(), name, avatar: avatar || '#4F956F', role: 'morador', last: '', time: 'agora', unread: 0, thread: [] }; window.ARAH.chats.unshift(c); }
        setStack(s => [...s, { type: 'chat', param: c.id }]);
      },
    };
    return () => { delete window.openJourney; delete window.arahMutate; delete window.arahToast; delete window.appNav; };
  }, []);

  // Persist session prefs whenever they change
  React.useEffect(() => {
    if (stage === 'login') return;
    window.arahSaveApp && window.arahSaveApp({
      stage, territoryId, role, likes, interests, joinedGroups,
      notifReads: notifs.filter(n => n.read).map(n => n.id),
    });
  }, [stage, territoryId, role, likes, interests, joinedGroups, notifs]);

  const territory = window.ARAH.territories.find(t => t.id === territoryId);
  const content = window.ARAH.getContent(territoryId);
  const unread = notifs.filter(n => !n.read).length;
  const chatUnread = window.ARAH.chats.reduce((s, c) => s + c.unread, 0);
  const top = stack[stack.length - 1] || null;

  const showToast = (msg, icon = 'check_circle') => { setToast({ msg, icon }); setTimeout(() => setToast(null), 2200); };
  const push = (type, param) => setStack(s => [...s, { type, param }]);
  const pop = () => setStack(s => s.slice(0, -1));
  const goTab = (t) => { setStack([]); setTab(t); };
  const toggleLike = (id) => setLikes(l => ({ ...l, [id]: !l[id] }));
  const toggleInterest = (i) => setInterests(s => s.includes(i) ? s.filter(x => x !== i) : [...s, i]);
  const readNotif = (id) => setNotifs(ns => ns.map(n => n.id === id ? { ...n, read: true } : n));
  const enterTerritory = (id) => { setTerritoryId(id); goTab('feed'); showToast(`Você está em ${window.ARAH.territories.find(t => t.id === id).name}`, 'forest'); };
  const setRoleAndToast = (r) => { setRole(r); const m = { visitante: 'Vendo como visitante', morador: 'Você confirmou residência', curador: 'Vendo como curador' }; showToast(m[r], r === 'morador' ? 'verified' : 'switch_account'); };

  const openLink = (link) => {
    if (!link) return;
    if (link.startsWith('post:')) push('post', link.slice(5));
    else if (link.startsWith('map:')) push('map', link.slice(4));
    else if (link === 'events') push('events');
    else if (link === 'profile') goTab('profile');
  };
  const openTool = (id) => {
    if (id === 'logout') { setStage('login'); setTab('feed'); setStack([]); setRole('visitante'); return; }
    if (id === 'chat') { push('chats'); return; }
    if (id === 'market') { push('market'); return; }
    push(id);
  };

  // ---- Stage screens ----
  if (stage === 'login')
    return <Device viewKey="login"><LoginScreen onLogin={() => setStage('onboarding')} /></Device>;
  if (stage === 'onboarding')
    return <Device viewKey="onb" scroll header={{ kind: 'back', title: 'Entrar no território', onBack: () => setStage('login') }}>
      <OnboardingScreen onContinue={(id) => { setTerritoryId(id); setStage('app'); setRole('visitante'); showToast('Bem-vinda a Camburi', 'forest'); }} />
    </Device>;

  // ---- Overlay (pushed) screens ----
  if (top) {
    const titles = { map: 'Mapa', events: 'Eventos', health: 'Saúde do território', settings: 'Configurações',
      elections: 'Eleições & votação', manage: 'Gestão do território', chats: 'Mensagens', post: 'Publicação', notifications: 'Notificações', store: 'Minha loja', saved: 'Salvos',
      market: 'Mercado', groupbuy: 'Compra coletiva', hosting: 'Hospedagem', demands: 'Demandas & ofertas', trades: 'Trocas comunitárias', delivery: 'Entregas', wallet: 'Carteira territorial',
      sitters: 'Babás', wellness: 'Bem-estar', rental: 'Aluguéis', digital: 'Hub digital', moderation: 'Moderação', subscriptions: 'Assinaturas',
      metrics: 'Painel de métricas', seeds: 'Banco de sementes', learning: 'Aprendizado', ai: 'Assistente IA', achievements: 'Conquistas' };
    const chat = top.type === 'chat' ? window.ARAH.chats.find(c => c.id === top.param) : null;
    const title = chat ? chat.name : titles[top.type];
    const noPad = top.type === 'map' || top.type === 'post' || top.type === 'chat' || top.type === 'ai';
    let inner;
    if (top.type === 'map') inner = <MapScreen territory={territory} content={content} focusId={top.param} />;
    if (top.type === 'events') inner = <EventsScreen territory={territory} content={content} role={role} />;
    if (top.type === 'health') inner = <HealthScreen territory={territory} content={content} />;
    if (top.type === 'settings') inner = <SettingsScreen />;
    if (top.type === 'store') inner = <StoreScreen />;
    if (top.type === 'saved') inner = <SavedScreen />;
    if (top.type === 'elections') inner = <ElectionsScreen role={role} />;
    if (top.type === 'manage') inner = <ManageScreen territory={territory} />;
    if (top.type === 'moderation') inner = <ManageScreen territory={territory} />;
    if (top.type === 'chats') inner = <ChatListScreen onOpen={(id) => push('chat', id)} />;
    if (top.type === 'chat') inner = <ChatThreadScreen chatId={top.param} />;
    if (top.type === 'post') inner = <PostDetailScreen postId={top.param} likes={likes} onLike={toggleLike} />;
    if (top.type === 'notifications') inner = <NotificationsScreen items={notifs} onRead={readNotif} onOpen={openLink} />;
    if (top.type === 'market') inner = <MarketScreen joinedGroups={joinedGroups} onJoinGroup={(id) => setJoinedGroups(g => ({ ...g, [id]: !g[id] }))} />;
    if (top.type === 'groupbuy') inner = <GroupBuyScreen />;
    if (top.type === 'hosting') inner = <HostingScreen />;
    if (top.type === 'demands') inner = <DemandsScreen />;
    if (top.type === 'trades') inner = <TradesScreen />;
    if (top.type === 'delivery') inner = <DeliveryScreen />;
    if (top.type === 'wallet') inner = <WalletScreen />;
    if (top.type === 'sitters') inner = <SittersScreen />;
    if (top.type === 'wellness') inner = <WellnessScreen />;
    if (top.type === 'rental') inner = <RentalScreen />;
    if (top.type === 'digital') inner = <DigitalScreen />;
    if (top.type === 'metrics') inner = <MetricsScreen territory={territory} />;
    if (top.type === 'seeds') inner = <SeedsScreen />;
    if (top.type === 'learning') inner = <LearningScreen />;
    if (top.type === 'ai') inner = <AIScreen territory={territory} />;
    if (top.type === 'achievements') inner = <AchievementsScreen />;
    if (top.type === 'subscriptions') inner = <SubscriptionsScreen />;
    const action = top.type === 'notifications' && unread ? { icon: 'done_all', onClick: () => setNotifs(ns => ns.map(n => ({ ...n, read: true }))) } : null;
    return <Device viewKey={'ov' + stack.length + top.type + (top.param || '')} scroll={!noPad} noPad={noPad}
      journey={journey} onCloseJourney={() => setJourney(null)} onApproveResidencia={() => setRoleAndToast('morador')}
      header={{ kind: 'back', title, onBack: pop, action }}>{inner}</Device>;
  }

  // ---- Main shell tabs ----
  const titles = { explore: 'Explorar', post: 'Publicar', services: 'Serviços do território', profile: 'Perfil' };
  let body, header;
  if (tab === 'feed') {
    header = { kind: 'territory' };
    body = <FeedScreen territory={territory} content={content} role={role} likes={likes} onLike={toggleLike} onOpen={(id) => push('post', id)} />;
  } else {
    header = { kind: 'title', title: titles[tab] };
    if (tab === 'explore') body = <ExploreScreen territory={territory} activeId={territoryId} role={role} onEnter={enterTerritory} onMap={() => push('map')} onTool={openTool} />;
    if (tab === 'post') body = <CreatePostScreen territory={territory} onPublish={(data) => {
      const c = window.ARAH.content[territoryId];
      if (c) c.feed.unshift({ id: 'np' + Date.now(), author: window.ARAH.profile.name, handle: window.ARAH.profile.handle, avatar: window.ARAH.profile.avatar, role: role === 'visitante' ? 'visitante' : 'morador', time: 'agora', type: data.type, visibility: data.vis, title: data.title || 'Sem título', body: data.body || '', photo: data.photo, likes: 0, comments: 0 });
      window.arahSnapshot && window.arahSnapshot();
      goTab('feed'); showToast('Post publicado no território');
    }} />;
    if (tab === 'services') body = <ServicesHubScreen territory={territory} role={role} onOpen={openTool} />;
    if (tab === 'profile') body = <ProfileScreen role={role} onSetRole={setRoleAndToast} onOpen={openTool} interests={interests} onToggleInterest={toggleInterest} />;
  }

  return (
    <Device
      viewKey={'tab-' + tab}
      scroll
      header={header}
      territory={territory}
      onTerritoryTap={() => setSheet(true)}
      onChat={() => push('chats')}
      onNotif={() => push('notifications')}
      unread={unread} chatUnread={chatUnread}
      bottom={<BottomNav active={tab} onNav={goTab} />}
      toast={toast}
      journey={journey} onCloseJourney={() => setJourney(null)} onApproveResidencia={() => setRoleAndToast('morador')}
    >
      {body}
      {sheet && <TerritorySheet activeId={territoryId} onPick={(id) => { setSheet(false); enterTerritory(id); }} onClose={() => setSheet(false)} />}
    </Device>
  );
}

// Device wrapper with smooth crossfade between views (keyed on viewKey).
function Device({ children, viewKey, header, bottom, scroll, noPad, territory, onTerritoryTap, onChat, onNotif, unread, chatUnread, toast, journey, onCloseJourney, onApproveResidencia }) {
  return (
    <IOSDevice dark width={390} height={844}>
      <div style={{ position: 'absolute', inset: 0, background: `radial-gradient(130% 80% at 50% -8%, #15201A 0%, ${T.bg} 58%)`, display: 'flex', flexDirection: 'column' }}>
        <div style={{ height: 56, flexShrink: 0 }} />
        {header && <Header {...header} territory={territory} onTerritoryTap={onTerritoryTap} onChat={onChat} onNotif={onNotif} unread={unread} chatUnread={chatUnread} />}
        {/* keyed fade region */}
        <div key={viewKey} className="screen-fade" style={{ flex: 1, minHeight: 0, display: 'flex', flexDirection: 'column' }}>
          {noPad ? (
            <div style={{ flex: 1, minHeight: 0, overflow: 'hidden', position: 'relative' }}>{children}</div>
          ) : (
            <div className="appscroll" style={{ flex: 1, overflowY: scroll ? 'auto' : 'hidden', overflowX: 'hidden', position: 'relative' }}>
              {children}
              {scroll && bottom && <div style={{ height: 96 }} />}
            </div>
          )}
        </div>
        {bottom && <div style={{ position: 'absolute', left: 0, right: 0, bottom: 0, zIndex: 30 }}>{bottom}</div>}
        {toast && (
          <div style={{
            position: 'absolute', left: 18, right: 18, bottom: bottom ? 108 : 40, zIndex: 200,
            display: 'flex', alignItems: 'center', gap: 10, padding: '13px 16px',
            background: T.glassGrad, backdropFilter: 'blur(14px)', WebkitBackdropFilter: 'blur(14px)',
            borderRadius: 15, border: `1px solid ${T.lineHi}`, boxShadow: '0 12px 30px rgba(0,0,0,0.45)',
            animation: 'toastIn .3s cubic-bezier(.16,1,.3,1)',
          }}>
            <Icon name={toast.icon} size={20} color={T.green} fill={1} />
            <span style={{ fontFamily: T.sans, fontSize: 14, color: T.fg, fontWeight: 500 }}>{toast.msg}</span>
          </div>
        )}
        {/* Journey overlay (multi-step flows) */}
        {journey && <JourneyHost journey={journey} onClose={onCloseJourney} onApprove={onApproveResidencia} />}
      </div>
    </IOSDevice>
  );
}

function Header({ kind, title, action, onBack, territory, onTerritoryTap, onChat, onNotif, unread, chatUnread }) {
  if (kind === 'territory')
    return <TopBar territory={territory} onTerritoryTap={onTerritoryTap} onChat={onChat} onNotif={onNotif} unread={unread} chatUnread={chatUnread} />;
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '6px 16px 14px' }}>
      {kind === 'back' && <button onClick={onBack} style={iconBtn}><Icon name="arrow_back" size={22} color={T.fg} /></button>}
      <h2 style={{ margin: 0, flex: 1, fontFamily: T.display, fontWeight: 600, fontSize: 21, color: T.fg, letterSpacing: -0.4, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{title}</h2>
      {action && <button onClick={action.onClick} style={iconBtn}><Icon name={action.icon} size={21} color={T.green} /></button>}
    </div>
  );
}

ReactDOM.createRoot(document.getElementById('root')).render(<App />);
