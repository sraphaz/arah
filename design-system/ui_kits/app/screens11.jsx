// screens11.jsx — Serviços pessoais: Babás, Bem-estar (espaços + prestadores).

function SittersScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Épico 10">Babás verificadas do território — confiança entre vizinhos. Visão de produto.</SoonBanner>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
        {window.ARAH.sitters.map(s => (
          <Card key={s.id} glow style={{ padding: 15 }}>
            <div style={{ display: 'flex', gap: 13 }}>
              <Avatar color={s.avatar} name={s.name} size={56} resident={s.verified} />
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg, letterSpacing: -0.2 }}>{s.name}</span>
                  {s.verified && <Icon name="verified" size={15} color={T.green} fill={1} />}
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 9, marginTop: 3, fontFamily: T.sans, fontSize: 12.5, color: T.fg3 }}>
                  <span style={{ display: 'inline-flex', alignItems: 'center', gap: 3, color: T.fg2 }}><Icon name="star" size={14} color="#F5C451" fill={1} />{s.rating} <span style={{ color: T.fg3 }}>({s.reviews})</span></span>
                  <span>· {s.exp}</span>
                  <span>· {s.dist}</span>
                </div>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6, marginTop: 9 }}>
                  {s.badges.map(b => (
                    <span key={b} style={{ display: 'inline-flex', alignItems: 'center', gap: 4, fontFamily: T.sans, fontSize: 11, color: T.green, background: T.greenDim, padding: '3px 9px', borderRadius: 999 }}>
                      <Icon name="check" size={12} />{b}
                    </span>
                  ))}
                  <span style={{ fontFamily: T.sans, fontSize: 11, color: T.fg2, background: T.cardHi, padding: '3px 9px', borderRadius: 999 }}>{s.ages}</span>
                </div>
              </div>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 14, paddingTop: 13, borderTop: `1px solid ${T.line}` }}>
              <span><strong style={{ fontFamily: T.display, fontWeight: 700, fontSize: 17, color: T.fg }}>{s.price}</strong></span>
              <div style={{ display: 'flex', gap: 9 }}>
                <Btn kind="dark" size="sm" icon="chat" onClick={() => window.appNav.openChat(s.name, s.avatar)}>Conversar</Btn>
                <Btn kind="primary" size="sm" icon="event_available" onClick={() => window.openJourney('sitter', s)}>Solicitar</Btn>
              </div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}

function WellnessScreen() {
  const [tab, setTab] = React.useState('espacos');
  const w = window.ARAH.wellness;
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Épico 10">Bem-estar local — espaços e profissionais com agenda compartilhada. Visão de produto.</SoonBanner>
      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <SegTab active={tab === 'espacos'} onClick={() => setTab('espacos')} icon="spa">Espaços</SegTab>
        <SegTab active={tab === 'prestadores'} onClick={() => setTab('prestadores')} icon="self_improvement">Profissionais</SegTab>
      </div>
      {tab === 'espacos' ? (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
          {w.spaces.map(s => (
            <Card key={s.id} glow style={{ padding: 0, overflow: 'hidden' }}>
              <div style={{ height: 104, background: `linear-gradient(150deg, ${s.avatar}40, ${s.avatar}12)`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Icon name={s.icon} size={44} color={T.fg} fill={0} style={{ opacity: 0.85 }} />
              </div>
              <div style={{ padding: 15 }}>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg, letterSpacing: -0.2 }}>{s.name}</div>
                <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3, marginTop: 2 }}>{s.type} · {s.cap}</div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 13 }}>
                  <span style={{ display: 'inline-flex', alignItems: 'center', gap: 5, fontFamily: T.sans, fontSize: 12.5, color: T.green }}>
                    <Icon name="event_available" size={15} fill={1} /> {s.slots}
                  </span>
                  <Btn kind="primary" size="sm" icon="calendar_month" onClick={() => window.openJourney('wellness', s)}>Ver agenda</Btn>
                </div>
              </div>
            </Card>
          ))}
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 11 }}>
          {w.providers.map(p => (
            <Card key={p.id} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: 14 }}>
              <Avatar color={p.avatar} name={p.name} size={48} resident={p.verified} />
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{p.name}</span>
                  {p.verified && <Icon name="verified" size={14} color={T.green} fill={1} />}
                </div>
                <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3, marginTop: 2 }}>{p.spec}</div>
                <div style={{ display: 'inline-flex', alignItems: 'center', gap: 4, marginTop: 5, fontFamily: T.sans, fontSize: 12, color: T.fg2 }}>
                  <Icon name="star" size={13} color="#F5C451" fill={1} /> {p.rating} · a partir de <strong style={{ color: T.fg, fontWeight: 600 }}>{p.price}</strong>
                </div>
              </div>
              <Btn kind="dark" size="sm" icon="calendar_month" style={{ flexShrink: 0 }} onClick={() => window.openJourney('wellness', p)}>Agendar</Btn>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}

Object.assign(window, { SittersScreen, WellnessScreen });
