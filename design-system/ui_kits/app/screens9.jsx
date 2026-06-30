// screens9.jsx — Economia: Compra Coletiva, Hospedagem, Demandas & Ofertas.

function GroupBuyScreen() {
  const g = window.ARAH.groupBuyDetail;
  const [joined, setJoined] = React.useState(false);
  const count = g.joined + (joined ? 1 : 0);
  const pct = Math.min(100, Math.round(count / g.goal * 100));
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 17">Compra coletiva — quanto mais gente, menor o preço. Visão de produto.</SoonBanner>
      <Card glow style={{ padding: 0, overflow: 'hidden', marginBottom: 16 }}>
        <div style={{ height: 130, background: 'linear-gradient(150deg, rgba(166,214,185,0.25), rgba(129,199,132,0.08))', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Icon name={g.icon} size={56} color={T.green} fill={1} style={{ opacity: 0.9 }} />
        </div>
        <div style={{ padding: 18 }}>
          <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 19, color: T.fg, letterSpacing: -0.4, lineHeight: 1.2 }}>{g.title}</div>
          <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3, marginTop: 4 }}>Organizado por {g.org} · {g.deadline}</div>
          <p style={{ margin: '12px 0 0', fontFamily: T.sans, fontSize: 14, lineHeight: 1.5, color: T.fg2 }}>{g.desc}</p>

          {/* progress */}
          <div style={{ marginTop: 16, height: 10, borderRadius: 999, background: T.cardHi, overflow: 'hidden' }}>
            <div style={{ width: `${pct}%`, height: '100%', borderRadius: 999, background: T.greenGrad, transition: 'width .4s' }} />
          </div>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 9 }}>
            <span style={{ fontFamily: T.sans, fontSize: 13, color: T.fg2 }}><strong style={{ color: T.fg, fontWeight: 700 }}>{count}</strong>/{g.goal} {g.unit}</span>
            <span style={{ display: 'flex', alignItems: 'center' }}>
              {g.participants.map((c, i) => <span key={i} style={{ marginLeft: i ? -8 : 0 }}><Avatar color={c} name="A B" size={24} /></span>)}
              <span style={{ marginLeft: 6, fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>+{count - g.participants.length}</span>
            </span>
          </div>

          {/* price tiers */}
          <div style={{ display: 'flex', gap: 8, marginTop: 16 }}>
            {g.tiers.map((t, i) => {
              const active = count >= t.n;
              return (
                <div key={i} style={{ flex: 1, padding: '11px 8px', borderRadius: 13, textAlign: 'center',
                  background: active ? T.greenDim : T.cardFlat, border: `1px solid ${active ? 'rgba(166,214,185,0.35)' : T.line}` }}>
                  <div style={{ fontFamily: T.sans, fontSize: 10.5, color: T.fg3 }}>{t.n}+ famílias</div>
                  <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 15, color: active ? T.green : T.fg, marginTop: 3 }}>{t.price}</div>
                </div>
              );
            })}
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 14, padding: '10px 13px', borderRadius: 12, background: 'rgba(166,214,185,0.1)' }}>
            <Icon name="savings" size={18} color={T.green} fill={1} />
            <span style={{ fontFamily: T.sans, fontSize: 13, color: T.fg2 }}>Economia estimada de <strong style={{ color: T.green, fontWeight: 700 }}>{g.saved}</strong> por família.</span>
          </div>
          <Btn kind={joined ? 'dark' : 'primary'} full size="lg" icon={joined ? 'check_circle' : 'group_add'} style={{ marginTop: 16 }} onClick={() => setJoined(j => !j)}>
            {joined ? 'Você está participando' : 'Entrar na compra coletiva'}
          </Btn>
        </div>
      </Card>
    </div>
  );
}

function HostingScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 18">Hospedagem no território — estadias com quem é da vila. Visão de produto.</SoonBanner>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 13 }}>
        {window.ARAH.hosting.map(h => (
          <Card key={h.id} glow style={{ padding: 0, overflow: 'hidden' }}>
            <div style={{ height: 128, background: `linear-gradient(150deg, ${h.avatar}40, ${h.avatar}12)`, display: 'flex', alignItems: 'center', justifyContent: 'center', position: 'relative' }}>
              <Icon name={h.icon} size={48} color={T.fg} fill={0} style={{ opacity: 0.85 }} />
              <span style={{ position: 'absolute', top: 10, left: 10, fontFamily: T.sans, fontSize: 11, fontWeight: 600, color: T.fg, background: 'rgba(11,12,10,0.55)', backdropFilter: 'blur(4px)', padding: '4px 10px', borderRadius: 999 }}>{h.tag}</span>
              <span style={{ position: 'absolute', top: 10, right: 10, display: 'inline-flex', alignItems: 'center', gap: 3, fontFamily: T.sans, fontSize: 11.5, fontWeight: 600, color: T.fg, background: 'rgba(11,12,10,0.55)', backdropFilter: 'blur(4px)', padding: '4px 10px', borderRadius: 999 }}>
                <Icon name="star" size={13} color="#F5C451" fill={1} /> {h.rating}
              </span>
            </div>
            <div style={{ padding: 15 }}>
              <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg, letterSpacing: -0.3 }}>{h.title}</div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 7, marginTop: 7 }}>
                <Avatar color={h.avatar} name={h.host} size={22} resident />
                <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3 }}>{h.host} · {h.guests} hóspedes · {h.reviews} avaliações</span>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 13 }}>
                <span><strong style={{ fontFamily: T.display, fontWeight: 700, fontSize: 18, color: T.fg }}>{h.price}</strong><span style={{ fontFamily: T.sans, fontSize: 13, color: T.fg3 }}> / {h.unit}</span></span>
                <Btn kind="primary" size="sm" icon="calendar_month" onClick={() => window.openJourney('reserva', h)}>Reservar</Btn>
              </div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}

function DemandsScreen() {
  const [tab, setTab] = React.useState('todos');
  let list = window.ARAH.demands;
  if (tab !== 'todos') list = list.filter(d => d.kind === tab);
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 19">Demandas & ofertas — quem precisa encontra quem oferece. Visão de produto.</SoonBanner>
      <Btn kind="primary" full size="md" icon="add" style={{ marginBottom: 14 }} onClick={() => window.openJourney('create', window.JOURNEY_PRESETS.demanda)}>Publicar demanda ou oferta</Btn>
      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <SegTab active={tab === 'todos'} onClick={() => setTab('todos')} icon="swap_horiz">Tudo</SegTab>
        <SegTab active={tab === 'demanda'} onClick={() => setTab('demanda')} icon="pan_tool">Demandas</SegTab>
        <SegTab active={tab === 'oferta'} onClick={() => setTab('oferta')} icon="handshake">Ofertas</SegTab>
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {list.map(d => {
          const isDemand = d.kind === 'demanda';
          const c = isDemand ? T.blue : T.green;
          return (
            <Card key={d.id} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: 14 }}>
              <div style={{ width: 44, height: 44, borderRadius: 12, background: `${c}1f`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <Icon name={d.icon} size={22} color={c} fill={1} />
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
                  <span style={{ fontFamily: T.sans, fontSize: 9.5, fontWeight: 700, letterSpacing: 0.6, textTransform: 'uppercase', color: c }}>{isDemand ? 'Demanda' : 'Oferta'}</span>
                  <span style={{ fontFamily: T.sans, fontSize: 11, color: T.fg3 }}>· {d.tag} · {d.time}</span>
                </div>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14.5, color: T.fg, marginTop: 2, letterSpacing: -0.2 }}>{d.title}</div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 5 }}>
                  <Avatar color={d.avatar} name={d.who} size={18} /><span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>{d.who}</span>
                </div>
              </div>
              <button onClick={() => window.openJourney('create', window.JOURNEY_PRESETS.demanda)} style={{ width: 38, height: 38, borderRadius: 11, border: `1px solid ${T.line}`, background: T.cardFlat, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <Icon name="chat" size={18} color={T.green} />
              </button>
            </Card>
          );
        })}
      </div>
    </div>
  );
}

Object.assign(window, { GroupBuyScreen, HostingScreen, DemandsScreen });
