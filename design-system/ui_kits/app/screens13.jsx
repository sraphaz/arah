// screens13.jsx — Assistente IA, Conquistas/Gamificação, Assinaturas.

function AIScreen({ territory }) {
  const [msgs, setMsgs] = React.useState([
    { me: false, body: `Oi! Sou o assistente de ${territory.name}. Posso te ajudar a navegar o território, encontrar serviços e resumir o que está acontecendo. O que você precisa?` },
  ]);
  const [draft, setDraft] = React.useState('');
  const scrollRef = React.useRef(null);
  const ask = (text) => {
    const q = (text || draft).trim();
    if (!q) return;
    setMsgs(m => [...m, { me: true, body: q }]);
    setDraft('');
    setTimeout(() => {
      setMsgs(m => [...m, { me: false, body: 'Aqui está o que encontrei no território — em breve, com respostas reais conectadas aos dados de Camburi. 🌿', typing: false }]);
    }, 600);
  };
  React.useEffect(() => { if (scrollRef.current) scrollRef.current.scrollTop = scrollRef.current.scrollHeight; }, [msgs]);
  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <div style={{ padding: '0 16px' }}><SoonBanner phase="Fase 27">Assistente IA do território — visão de produto.</SoonBanner></div>
      <div ref={scrollRef} className="appscroll" style={{ flex: 1, overflowY: 'auto', padding: '0 16px 12px', display: 'flex', flexDirection: 'column', gap: 11 }}>
        {msgs.map((m, i) => (
          <div key={i} style={{ alignSelf: m.me ? 'flex-end' : 'flex-start', maxWidth: '84%', display: 'flex', gap: 9, flexDirection: m.me ? 'row-reverse' : 'row' }}>
            {!m.me && <div style={{ width: 32, height: 32, borderRadius: 10, background: T.greenGrad, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}><Icon name="auto_awesome" size={17} color="#0C1B10" fill={1} /></div>}
            <div style={{ padding: '11px 14px', borderRadius: m.me ? '16px 16px 4px 16px' : '16px 16px 16px 4px',
              background: m.me ? T.greenGrad : T.cardGrad, color: m.me ? '#0C1B10' : T.fg, border: m.me ? 'none' : `1px solid ${T.line}`,
              fontFamily: T.sans, fontSize: 14.5, lineHeight: 1.5 }}>{m.body}</div>
          </div>
        ))}
        {/* suggested prompts */}
        {msgs.length <= 1 && (
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8, marginTop: 6 }}>
            {window.ARAH.aiPrompts.map(p => (
              <button key={p} onClick={() => ask(p)} style={{ textAlign: 'left', padding: '9px 13px', borderRadius: 13, cursor: 'pointer',
                background: T.cardFlat, border: `1px solid ${T.line}`, color: T.fg2, fontFamily: T.sans, fontSize: 13, WebkitTapHighlightColor: 'transparent' }}>{p}</button>
            ))}
          </div>
        )}
      </div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 9, padding: '12px 16px 16px', borderTop: `1px solid ${T.line}`, background: T.bg2 }}>
        <input value={draft} onChange={e => setDraft(e.target.value)} onKeyDown={e => e.key === 'Enter' && ask()}
          placeholder="Pergunte sobre o território…" style={{ flex: 1, background: T.cardFlat, color: T.fg, border: `1px solid ${T.line}`, borderRadius: 999, padding: '11px 16px', fontFamily: T.sans, fontSize: 14, outline: 'none' }} />
        <button onClick={() => ask()} style={{ width: 42, height: 42, borderRadius: '50%', border: 'none', background: T.greenGrad, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: T.greenGlow, flexShrink: 0 }}>
          <Icon name="arrow_upward" size={20} color="#0C1B10" fill={1} />
        </button>
      </div>
    </div>
  );
}

function AchievementsScreen() {
  const list = window.ARAH.achievements;
  const got = list.filter(a => a.got).length;
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 42">Conquistas — reconhecimento do cuidado com o território. Visão de produto.</SoonBanner>
      <Card glow style={{ padding: 18, marginBottom: 16, textAlign: 'center', background: T.cardHiGrad }}>
        <div style={{ width: 60, height: 60, borderRadius: 18, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 12px' }}>
          <Icon name="workspace_premium" size={32} color={T.green} fill={1} />
        </div>
        <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 20, color: T.fg }}>{got} de {list.length} conquistas</div>
        <div style={{ fontFamily: T.sans, fontSize: 13, color: T.fg3, marginTop: 3 }}>Guardiã ativa de Camburi</div>
      </Card>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 11 }}>
        {list.map(a => (
          <Card key={a.id} style={{ padding: 15, textAlign: 'center', opacity: a.got ? 1 : 0.72 }}>
            <div style={{ width: 48, height: 48, borderRadius: 14, background: a.got ? T.greenDim : T.cardHi, display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 11px', position: 'relative' }}>
              <Icon name={a.icon} size={24} color={a.got ? T.green : T.fg3} fill={a.got ? 1 : 0} />
              {a.got && <div style={{ position: 'absolute', bottom: -3, right: -3, width: 20, height: 20, borderRadius: '50%', background: T.greenSolid, border: `2px solid ${T.cardFlat}`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}><Icon name="check" size={12} color="#0C1B10" /></div>}
            </div>
            <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 13.5, color: T.fg, lineHeight: 1.2 }}>{a.label}</div>
            <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 3 }}>{a.sub}</div>
            {!a.got && a.progress > 0 && (
              <div style={{ height: 5, borderRadius: 999, background: T.cardHi, overflow: 'hidden', marginTop: 9 }}>
                <div style={{ width: `${a.progress}%`, height: '100%', borderRadius: 999, background: T.greenGrad }} />
              </div>
            )}
          </Card>
        ))}
      </div>
    </div>
  );
}

function SubscriptionsScreen() {
  const [sel, setSel] = React.useState('sub2');
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 15">Assinaturas — sustente o território que te sustenta. Visão de produto.</SoonBanner>
      <p style={{ margin: '0 0 16px', fontFamily: T.sans, fontSize: 14, lineHeight: 1.5, color: T.fg2 }}>
        Apoio recorrente que mantém a infraestrutura comunitária viva e independente.
      </p>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
        {window.ARAH.subscriptionTiers.map(t => {
          const on = sel === t.id;
          return (
            <Card key={t.id} onClick={() => setSel(t.id)} glow={on} style={{ padding: 18, position: 'relative', border: `1.5px solid ${on ? T.green : T.line}` }}>
              {t.popular && <span style={{ position: 'absolute', top: -10, right: 16, fontFamily: T.sans, fontSize: 10.5, fontWeight: 700, letterSpacing: 0.4, color: '#0C1B10', background: T.greenSolid, padding: '4px 11px', borderRadius: 999 }}>MAIS ESCOLHIDO</span>}
              <div style={{ display: 'flex', alignItems: 'baseline', gap: 8 }}>
                <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 18, color: T.fg }}>{t.name}</span>
                <span style={{ marginLeft: 'auto', fontFamily: T.display, fontWeight: 700, fontSize: 22, color: on ? T.green : T.fg }}>{t.price}</span>
                <span style={{ fontFamily: T.sans, fontSize: 13, color: T.fg3 }}>{t.period}</span>
              </div>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 8, marginTop: 13 }}>
                {t.perks.map(p => (
                  <div key={p} style={{ display: 'flex', alignItems: 'center', gap: 9, fontFamily: T.sans, fontSize: 13.5, color: T.fg2 }}>
                    <Icon name="check_circle" size={17} color={T.green} fill={1} /> {p}
                  </div>
                ))}
              </div>
            </Card>
          );
        })}
      </div>
      <Btn kind="primary" full size="lg" icon="favorite" style={{ marginTop: 18 }} onClick={() => window.openJourney('subscribe', window.ARAH.subscriptionTiers.find(t => t.id === sel))}>Apoiar o território</Btn>
    </div>
  );
}

Object.assign(window, { AIScreen, AchievementsScreen, SubscriptionsScreen });
