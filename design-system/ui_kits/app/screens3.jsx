// screens3.jsx — Profile hub (role switcher), Events (join/cancel), Settings, Território Health.

function ProfileScreen({ role, onSetRole, onOpen, interests, onToggleInterest }) {
  const p = window.ARAH.profile;
  const roleLabel = { visitante: 'Visitante', morador: 'Morador', curador: 'Curador' };
  const tools = [
    { id: 'store', icon: 'storefront', label: 'Minha loja', color: T.green, badge: 'Novo' },
    { id: 'chat', icon: 'forum', label: 'Mensagens', color: T.green },
    { id: 'saved', icon: 'bookmark', label: 'Salvos', color: T.blue },
    { id: 'settings', icon: 'settings', label: 'Configurações', color: T.fg2 },
  ];
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 15, marginBottom: 16 }}>
        <Avatar color={p.avatar} name={p.name} size={68} resident={role !== 'visitante'} />
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 21, color: T.fg, letterSpacing: -0.4 }}>{p.name}</div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 7, marginTop: 4 }}>
            <RoleBadge role={role === 'visitante' ? 'visitante' : 'morador'} size="md" />
            {role === 'curador' && (
              <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4, padding: '4px 10px', borderRadius: 999, background: T.alertDim, color: T.alert, fontFamily: T.sans, fontSize: 12, fontWeight: 600 }}>
                <Icon name="shield_person" size={14} fill={1} /> Curador
              </span>
            )}
          </div>
        </div>
      </div>
      <p style={{ margin: '0 0 14px', fontFamily: T.sans, fontSize: 14, lineHeight: 1.55, color: T.fg2 }}>{p.bio}</p>

      {/* Persona / journey switcher — demonstrates the role-based journeys */}
      <Card style={{ padding: 14, marginBottom: 16, background: T.cardHiGrad }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 7, marginBottom: 10 }}>
          <Icon name="switch_account" size={17} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, fontWeight: 600, color: T.fg }}>Ver como (demonstração de jornada)</span>
        </div>
        <div style={{ display: 'flex', gap: 8 }}>
          {['visitante', 'morador', 'curador'].map(r => (
            <button key={r} onClick={() => onSetRole(r)} style={{
              flex: 1, padding: '9px 6px', borderRadius: 11, cursor: 'pointer', WebkitTapHighlightColor: 'transparent',
              background: role === r ? T.greenDim : T.cardFlat, color: role === r ? T.green : T.fg2,
              border: `1px solid ${role === r ? 'transparent' : T.line}`,
              fontFamily: T.sans, fontSize: 12.5, fontWeight: 600, transition: 'all .15s',
            }}>{roleLabel[r]}</button>
          ))}
        </div>
        {role === 'visitante' && (
          <Btn kind="primary" full size="sm" icon="how_to_reg" style={{ marginTop: 11 }} onClick={() => window.openJourney('residencia')}>Confirmar residência</Btn>
        )}
      </Card>

      <div style={{ display: 'flex', gap: 10, marginBottom: 18 }}>
        {[['Posts', p.posts], ['Territórios', p.territories], ['Desde', p.since]].map(([k, v]) => (
          <Card key={k} style={{ flex: 1, padding: '13px 10px', textAlign: 'center' }}>
            <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 19, color: T.fg }}>{v}</div>
            <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 2, letterSpacing: 0.2 }}>{k}</div>
          </Card>
        ))}
      </div>

      {/* user tools — territory tools moved to Explorar */}
      <SectionLabel icon="account_circle">Minha conta</SectionLabel>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10, marginTop: 11, marginBottom: 22 }}>
        {tools.map(t => (
          <Card key={t.id} onClick={() => onOpen(t.id)} style={{ display: 'flex', alignItems: 'center', gap: 11, padding: 14 }}>
            <div style={{ width: 38, height: 38, borderRadius: 11, background: `${t.color}1f`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
              <Icon name={t.icon} size={20} color={t.color} fill={1} />
            </div>
            <span style={{ flex: 1, fontFamily: T.sans, fontSize: 13.5, fontWeight: 500, color: T.fg, lineHeight: 1.2 }}>{t.label}</span>
            {t.badge && <span style={{ fontFamily: T.sans, fontSize: 9.5, fontWeight: 700, color: t.color, background: `${t.color}1f`, padding: '2px 6px', borderRadius: 999, letterSpacing: 0.3 }}>{t.badge}</span>}
          </Card>
        ))}
      </div>

      <SectionLabel icon="interests">Meus interesses</SectionLabel>
      <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3, margin: '0 0 11px' }}>Interesses personalizam o feed do território.</div>
      <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8, marginBottom: 22 }}>
        {window.ARAH.interestPool.map(i => (
          <Chip key={i} active={interests.includes(i)} onClick={() => onToggleInterest(i)} icon={interests.includes(i) ? 'check' : 'add'}>{i}</Chip>
        ))}
      </div>

      <Btn kind="dark" full icon="logout" onClick={() => onOpen('logout')} style={{ color: T.alert, borderColor: 'rgba(232,160,106,0.2)' }}>Sair da conta</Btn>
    </div>
  );
}

function SectionLabel({ children, icon }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
      <Icon name={icon} size={18} color={T.green} fill={1} />
      <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg, letterSpacing: -0.2 }}>{children}</span>
    </div>
  );
}

function EventsScreen({ territory, content, role }) {
  const [status, setStatus] = React.useState(Object.fromEntries(content.events.map(e => [e.id, e.status])));
  const toggle = (id) => setStatus(s => ({ ...s, [id]: s[id] === 'confirmed' ? null : 'confirmed' }));
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <p style={{ margin: '0 0 16px', fontFamily: T.sans, fontSize: 14, color: T.fg2, lineHeight: 1.5 }}>
        Acontecendo em <strong style={{ color: T.green, fontWeight: 600 }}>{territory.name}</strong>
      </p>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 11 }}>
        {content.events.map(e => {
          const confirmed = status[e.id] === 'confirmed';
          return (
            <Card key={e.id} glow style={{ display: 'flex', gap: 14, padding: 14 }}>
              <div style={{ width: 52, flexShrink: 0, borderRadius: 13, background: T.greenDim, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', padding: '8px 0' }}>
                <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 21, color: T.green, lineHeight: 1 }}>{e.day}</span>
                <span style={{ fontFamily: T.sans, fontSize: 10.5, fontWeight: 600, color: T.green, letterSpacing: 1, marginTop: 3 }}>{e.mon}</span>
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'inline-block', fontFamily: T.sans, fontSize: 10.5, fontWeight: 600, color: T.fg3, letterSpacing: 0.8, textTransform: 'uppercase', marginBottom: 3 }}>{e.tag}</div>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15.5, color: T.fg, letterSpacing: -0.2, lineHeight: 1.3 }}>{e.title}</div>
                <p style={{ margin: '5px 0 0', fontFamily: T.sans, fontSize: 13, color: T.fg2, lineHeight: 1.45 }}>{e.desc}</p>
                <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginTop: 8, color: T.fg3, fontFamily: T.sans, fontSize: 12.5 }}>
                  <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}><Icon name="schedule" size={14} /> {e.time}</span>
                  <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}><Icon name="place" size={14} /> {e.place}</span>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginTop: 11 }}>
                  <Btn kind={confirmed ? 'dark' : 'primary'} size="sm" icon={confirmed ? 'check_circle' : 'event_available'} onClick={() => { if (!confirmed) window.openJourney('joinEvent', { ...e, onConfirm: () => toggle(e.id) }); }}
                    style={confirmed ? { color: T.green, borderColor: 'rgba(166,214,185,0.3)' } : {}}>
                    {confirmed ? 'Presença confirmada' : 'Participar'}
                  </Btn>
                  {confirmed && <button onClick={() => toggle(e.id)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: T.fg3, fontFamily: T.sans, fontSize: 12.5, fontWeight: 500 }}>Cancelar</button>}
                  <span style={{ marginLeft: 'auto', fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>{e.going + (confirmed ? 1 : 0)} vão</span>
                </div>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}

function HealthScreen({ territory, content }) {
  const h = content.health;
  const meters = [
    { label: 'Água potável', val: h.agua, icon: 'water_drop', color: T.water, unit: '%' },
    { label: 'Árvores nativas', val: h.nativas, icon: 'park', color: T.green, unit: '%' },
  ];
  const counts = [
    { label: 'Nascentes', val: h.nascentes, icon: 'water', color: T.water },
    { label: 'Mirantes', val: h.mirantes, icon: 'landscape', color: T.green },
    { label: 'Santuários', val: h.santuarios, icon: 'forest', color: T.green },
  ];
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <p style={{ margin: '0 0 16px', fontFamily: T.sans, fontSize: 14, color: T.fg2, lineHeight: 1.5 }}>
        Indicadores vivos de <strong style={{ color: T.green, fontWeight: 600 }}>{territory.name}</strong>, mantidos pela comunidade.
      </p>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 11, marginBottom: 14 }}>
        {meters.map(m => (
          <Card key={m.label} style={{ padding: 16 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 11 }}>
              <Icon name={m.icon} size={20} color={m.color} fill={1} />
              <span style={{ flex: 1, fontFamily: T.sans, fontSize: 14, fontWeight: 500, color: T.fg }}>{m.label}</span>
              <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 18, color: m.color }}>{m.val}{m.unit}</span>
            </div>
            <div style={{ height: 8, borderRadius: 999, background: T.cardHi, overflow: 'hidden' }}>
              <div style={{ width: `${m.val}%`, height: '100%', borderRadius: 999, background: `linear-gradient(90deg, ${m.color}aa, ${m.color})` }} />
            </div>
          </Card>
        ))}
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 10 }}>
        {counts.map(c => (
          <Card key={c.label} style={{ padding: '16px 8px', textAlign: 'center' }}>
            <Icon name={c.icon} size={24} color={c.color} fill={1} />
            <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 22, color: T.fg, marginTop: 7 }}>{c.val}</div>
            <div style={{ fontFamily: T.sans, fontSize: 11, color: T.fg3, marginTop: 1 }}>{c.label}</div>
          </Card>
        ))}
      </div>
    </div>
  );
}

function SettingsScreen() {
  const groups = [
    { title: 'Conta', rows: [
      { icon: 'person', label: 'Editar perfil', detail: '' },
      { icon: 'verified_user', label: 'Confirmar residência', detail: 'Visitante', green: true },
    ] },
    { title: 'Preferências', rows: [
      { icon: 'notifications', label: 'Notificações', detail: 'Posts · Eventos · Alertas' },
      { icon: 'public', label: 'Idioma', detail: 'Português' },
      { icon: 'my_location', label: 'Localização', detail: 'Ativa', green: true },
      { icon: 'dark_mode', label: 'Tema', detail: 'Escuro' },
    ] },
    { title: 'Privacidade', rows: [
      { icon: 'shield', label: 'Privacidade e dados', detail: '' },
      { icon: 'block', label: 'Contas bloqueadas', detail: '' },
    ] },
  ];
  return (
    <div style={{ padding: '0 18px 16px' }}>
      {groups.map(g => (
        <div key={g.title} style={{ marginBottom: 18 }}>
          <div style={{ fontFamily: T.sans, fontSize: 11.5, fontWeight: 600, letterSpacing: 0.8, textTransform: 'uppercase', color: T.fg3, margin: '0 2px 9px' }}>{g.title}</div>
          <Card style={{ overflow: 'hidden' }}>
            {g.rows.map((r, i) => (
              <div key={r.label} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: '14px 15px', borderBottom: i === g.rows.length - 1 ? 'none' : `1px solid ${T.line}`, cursor: 'pointer' }}>
                <Icon name={r.icon} size={20} color={T.fg2} />
                <span style={{ flex: 1, fontFamily: T.sans, fontSize: 14.5, color: T.fg }}>{r.label}</span>
                {r.detail && <span style={{ fontFamily: T.sans, fontSize: 13, color: r.green ? T.green : T.fg3 }}>{r.detail}</span>}
                <Icon name="chevron_right" size={19} color={T.fg3} />
              </div>
            ))}
          </Card>
        </div>
      ))}
    </div>
  );
}

Object.assign(window, { ProfileScreen, EventsScreen, HealthScreen, SettingsScreen, SectionLabel });

// Saved / bookmarked content (posts + market items)
function SavedScreen() {
  const [tab, setTab] = React.useState('posts');
  const posts = window.ARAH.content.t1.feed.slice(0, 2);
  const items = window.ARAH.market.slice(0, 2);
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <SegTab active={tab === 'posts'} onClick={() => setTab('posts')} icon="article">Posts</SegTab>
        <SegTab active={tab === 'items'} onClick={() => setTab('items')} icon="storefront">Mercado</SegTab>
      </div>
      {tab === 'posts' ? (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          {posts.map(p => (
            <Card key={p.id} style={{ padding: 14, display: 'flex', gap: 12, alignItems: 'flex-start' }}>
              <Avatar color={p.avatar} name={p.author} size={38} resident={p.role === 'morador'} />
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14.5, color: T.fg, letterSpacing: -0.2, lineHeight: 1.3 }}>{p.title}</div>
                <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 3 }}>{p.author} · {p.time}</div>
              </div>
              <Icon name="bookmark" size={20} color={T.green} fill={1} />
            </Card>
          ))}
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          {items.map(m => (
            <Card key={m.id} style={{ padding: 14, display: 'flex', gap: 12, alignItems: 'center' }}>
              <div style={{ width: 44, height: 44, borderRadius: 12, background: `${m.avatar}22`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}><Icon name={m.icon} size={22} color={T.green} /></div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14.5, color: T.fg, letterSpacing: -0.2 }}>{m.title}</div>
                <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 2 }}>{m.seller} · {m.price}</div>
              </div>
              <Icon name="bookmark" size={20} color={T.green} fill={1} />
            </Card>
          ))}
        </div>
      )}
      <div style={{ textAlign: 'center', color: T.fg3, fontFamily: T.sans, fontSize: 12.5, padding: '18px 0 4px' }}>
        Toque no marcador em qualquer post ou item para salvar aqui.
      </div>
    </div>
  );
}

Object.assign(window, { SavedScreen });
