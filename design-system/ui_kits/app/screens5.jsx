// screens5.jsx — Mercado (marketplace + compra coletiva) + Mensagens (chat). NEW (proposed).

function MarketScreen({ onJoinGroup, joinedGroups }) {
  const [tab, setTab] = React.useState('mercado');
  return (
    <div style={{ padding: '0 18px 12px' }}>
      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <SegTab active={tab === 'mercado'} onClick={() => setTab('mercado')} icon="storefront">Mercado local</SegTab>
        <SegTab active={tab === 'coletiva'} onClick={() => setTab('coletiva')} icon="groups">Compra coletiva</SegTab>
      </div>

      {/* proposed feature note */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 14, padding: '9px 12px', borderRadius: 12, background: T.blueDim }}>
        <Icon name="auto_awesome" size={16} color={T.blue} fill={1} />
        <span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg2, lineHeight: 1.4 }}>Novo na Arah — economia local do território.</span>
      </div>

      {tab === 'mercado' ? (
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 11 }}>
          {window.ARAH.market.map(m => (
            <Card key={m.id} glow onClick={() => window.openJourney('checkout', m)} style={{ padding: 0, overflow: 'hidden' }}>
              <div style={{ height: 84, background: `linear-gradient(150deg, ${m.avatar}66, ${m.avatar}22)`, display: 'flex', alignItems: 'center', justifyContent: 'center', position: 'relative' }}>
                <Icon name={m.icon} size={34} color={T.fg} fill={0} style={{ opacity: 0.9 }} />
                <span style={{ position: 'absolute', top: 8, left: 8, fontFamily: T.sans, fontSize: 10, fontWeight: 600, color: T.fg, background: 'rgba(11,12,10,0.5)', backdropFilter: 'blur(4px)', padding: '3px 8px', borderRadius: 999 }}>{m.tag}</span>
              </div>
              <div style={{ padding: 12 }}>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14, color: T.fg, lineHeight: 1.25, letterSpacing: -0.2 }}>{m.title}</div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 5, marginTop: 6, color: T.fg3, fontFamily: T.sans, fontSize: 11.5 }}>
                  <Avatar color={m.avatar} name={m.seller} size={16} /> {m.seller}
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 10 }}>
                  <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 15, color: T.green }}>{m.price}</span>
                  <button onClick={(e) => { e.stopPropagation(); window.openJourney('checkout', m); }} style={{ width: 32, height: 32, borderRadius: 10, border: 'none', background: T.greenDim, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <Icon name="add_shopping_cart" size={17} color={T.green} />
                  </button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
          {window.ARAH.groupBuys.map(g => {
            const joined = joinedGroups[g.id];
            const count = g.joined + (joined ? 1 : 0);
            const pct = Math.min(100, Math.round(count / g.goal * 100));
            return (
              <Card key={g.id} glow style={{ padding: 16 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 13 }}>
                  <div style={{ width: 46, height: 46, borderRadius: 13, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                    <Icon name={g.icon} size={24} color={T.green} fill={1} />
                  </div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15.5, color: T.fg, letterSpacing: -0.2, lineHeight: 1.25 }}>{g.title}</div>
                    <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 2 }}>Organizado por {g.org} · {g.deadline}</div>
                  </div>
                </div>
                <div style={{ height: 8, borderRadius: 999, background: T.cardHi, overflow: 'hidden' }}>
                  <div style={{ width: `${pct}%`, height: '100%', borderRadius: 999, background: T.greenGrad, transition: 'width .4s var(--ease)' }} />
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 9 }}>
                  <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2 }}><strong style={{ color: T.fg, fontWeight: 600 }}>{count}</strong> de {g.goal} {g.unit} · {pct}%</span>
                  <Btn kind={joined ? 'dark' : 'primary'} size="sm" icon={joined ? 'check' : 'group_add'} onClick={() => window.openJourney('groupBuy', { ...window.ARAH.groupBuyDetail, title: g.title, icon: g.icon, onConfirm: () => onJoinGroup(g.id) })}>
                    {joined ? 'Participando' : 'Participar'}
                  </Btn>
                </div>
              </Card>
            );
          })}
        </div>
      )}
    </div>
  );
}

function SegTab({ children, active, onClick, icon }) {
  return (
    <button onClick={onClick} style={{
      flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 7, padding: '11px',
      borderRadius: 13, cursor: 'pointer', WebkitTapHighlightColor: 'transparent',
      background: active ? T.greenDim : T.cardFlat, color: active ? T.green : T.fg2,
      border: `1px solid ${active ? 'transparent' : T.line}`,
      fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, transition: 'all .15s',
    }}>
      <Icon name={icon} size={17} fill={active ? 1 : 0} /> {children}
    </button>
  );
}

// Chat list
function ChatListScreen({ onOpen }) {
  return (
    <div style={{ padding: '0 18px 12px' }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
        {window.ARAH.chats.map(c => (
          <button key={c.id} onClick={() => onOpen(c.id)} style={{
            display: 'flex', alignItems: 'center', gap: 13, padding: '12px 6px', textAlign: 'left', cursor: 'pointer',
            background: 'none', border: 'none', borderBottom: `1px solid ${T.line}`, WebkitTapHighlightColor: 'transparent',
          }}>
            <Avatar color={c.avatar} name={c.name} size={48} resident={c.role === 'morador'} />
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
                <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg, letterSpacing: -0.2 }}>{c.name}</span>
                <span style={{ marginLeft: 'auto', fontFamily: T.sans, fontSize: 11.5, color: c.unread ? T.green : T.fg3 }}>{c.time}</span>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 2 }}>
                <span style={{ flex: 1, fontFamily: T.sans, fontSize: 13, color: c.unread ? T.fg : T.fg3, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap', fontWeight: c.unread ? 500 : 400 }}>{c.last}</span>
                {c.unread > 0 && <span style={{ minWidth: 18, height: 18, padding: '0 5px', borderRadius: 999, background: T.greenSolid, color: '#0C1B10', fontFamily: T.sans, fontSize: 11, fontWeight: 700, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>{c.unread}</span>}
              </div>
            </div>
          </button>
        ))}
      </div>
    </div>
  );
}

// Chat thread
function ChatThreadScreen({ chatId }) {
  const chat = window.ARAH.chats.find(c => c.id === chatId);
  const [msgs, setMsgs] = React.useState(chat.thread);
  const [draft, setDraft] = React.useState('');
  const scrollRef = React.useRef(null);
  const send = () => {
    if (!draft.trim()) return;
    setMsgs(m => [...m, { me: true, body: draft.trim(), time: 'agora' }]);
    setDraft('');
  };
  React.useEffect(() => { if (scrollRef.current) scrollRef.current.scrollTop = scrollRef.current.scrollHeight; }, [msgs]);
  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <div ref={scrollRef} className="appscroll" style={{ flex: 1, overflowY: 'auto', padding: '4px 16px 12px', display: 'flex', flexDirection: 'column', gap: 9 }}>
        {msgs.map((m, i) => (
          <div key={i} style={{ alignSelf: m.me ? 'flex-end' : 'flex-start', maxWidth: '78%' }}>
            <div style={{
              padding: '10px 14px', borderRadius: m.me ? '16px 16px 4px 16px' : '16px 16px 16px 4px',
              background: m.me ? T.greenGrad : T.cardGrad, color: m.me ? '#0C1B10' : T.fg,
              border: m.me ? 'none' : `1px solid ${T.line}`,
              fontFamily: T.sans, fontSize: 14.5, lineHeight: 1.45,
            }}>{m.body}</div>
            <div style={{ fontFamily: T.sans, fontSize: 10.5, color: T.fg3, marginTop: 3, textAlign: m.me ? 'right' : 'left', padding: '0 4px' }}>{m.time}</div>
          </div>
        ))}
      </div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 9, padding: '12px 16px 16px', borderTop: `1px solid ${T.line}`, background: T.bg2 }}>
        <button style={{ ...iconBtn, width: 42, height: 42, borderRadius: '50%' }}><Icon name="add" size={22} color={T.fg2} /></button>
        <input value={draft} onChange={e => setDraft(e.target.value)} onKeyDown={e => e.key === 'Enter' && send()}
          placeholder="Mensagem…" style={{ flex: 1, background: T.cardFlat, color: T.fg, border: `1px solid ${T.line}`, borderRadius: 999, padding: '11px 16px', fontFamily: T.sans, fontSize: 14, outline: 'none' }} />
        <button onClick={send} style={{ width: 42, height: 42, borderRadius: '50%', border: 'none', background: T.greenGrad, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: T.greenGlow, flexShrink: 0 }}>
          <Icon name="send" size={19} color="#0C1B10" fill={1} />
        </button>
      </div>
    </div>
  );
}

Object.assign(window, { MarketScreen, ChatListScreen, ChatThreadScreen });
