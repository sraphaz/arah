// screens8.jsx — Serviços hub: full feature directory by category + status badges.
// The gateway to the entire backlog, visually previewed.

function ServicesHubScreen({ territory, onOpen, role }) {
  const cats = window.ARAH.serviceCategories;
  return (
    <div style={{ padding: '0 18px 14px' }}>
      {/* intro */}
      <p style={{ margin: '0 0 18px', fontFamily: T.sans, fontSize: 14, lineHeight: 1.5, color: T.fg2 }}>
        Tudo que a vida em <strong style={{ color: T.green, fontWeight: 600 }}>{territory.name}</strong> precisa — economia, serviços, governança e cuidado com o lugar.
      </p>

      {cats.map(cat => (
        <div key={cat.id} style={{ marginBottom: 24 }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 13 }}>
            <div style={{ width: 34, height: 34, borderRadius: 10, background: `${cat.color}1f`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <Icon name={cat.icon} size={19} color={cat.color} fill={1} />
            </div>
            <div>
              <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg, letterSpacing: -0.2 }}>{cat.label}</div>
              <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3 }}>{cat.desc}</div>
            </div>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10 }}>
            {cat.items.map(it => (
              <Card key={it.id} onClick={() => onOpen(it.id)} style={{ padding: 13, position: 'relative' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 9, marginBottom: 9 }}>
                  <div style={{ width: 34, height: 34, borderRadius: 10, background: it.status === 'live' ? T.greenDim : 'rgba(255,255,255,0.05)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                    <Icon name={it.icon} size={19} color={it.status === 'live' ? T.green : T.fg2} fill={it.status === 'live' ? 1 : 0} />
                  </div>
                  {it.status === 'live'
                    ? <span style={statusPill(T.green, T.greenDim)}><Icon name="check" size={11} />No ar</span>
                    : <span style={statusPill(T.amber || '#E8A06A', T.alertDim)}>Em breve</span>}
                </div>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 13.5, color: T.fg, letterSpacing: -0.2, lineHeight: 1.2 }}>{it.label}</div>
                <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 2 }}>{it.sub}</div>
                {it.phase && <div style={{ fontFamily: T.sans, fontSize: 10, color: T.fg3, marginTop: 6, opacity: 0.8 }}>{it.phase}</div>}
              </Card>
            ))}
          </div>
        </div>
      ))}

      <Card style={{ padding: 15, display: 'flex', alignItems: 'center', gap: 11, background: T.cardHiGrad, marginTop: 4 }}>
        <Icon name="map" size={20} color={T.green} fill={1} />
        <span style={{ flex: 1, fontFamily: T.sans, fontSize: 12.5, color: T.fg2, lineHeight: 1.4 }}>
          Roadmap aberto: 16 de 48 fases entregues. As funcionalidades chegam em ondas, no ritmo da comunidade.
        </span>
      </Card>
    </div>
  );
}
function statusPill(color, bg) {
  return { marginLeft: 'auto', display: 'inline-flex', alignItems: 'center', gap: 3, padding: '3px 8px', borderRadius: 999,
    background: bg, color, fontFamily: T.sans, fontSize: 10, fontWeight: 700, letterSpacing: 0.2 };
}

// Reusable "preview / roadmap" banner shown atop soon-features
function SoonBanner({ phase, children }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 9, marginBottom: 16, padding: '10px 13px', borderRadius: 13, background: T.alertDim }}>
      <Icon name="auto_awesome" size={16} color={T.amber || '#E8A06A'} fill={1} />
      <span style={{ flex: 1, fontFamily: T.sans, fontSize: 12, color: T.fg2, lineHeight: 1.4 }}>{children}</span>
      {phase && <span style={{ fontFamily: T.sans, fontSize: 10, fontWeight: 700, color: T.amber || '#E8A06A', background: 'rgba(232,160,106,0.16)', padding: '3px 8px', borderRadius: 999, flexShrink: 0 }}>{phase}</span>}
    </div>
  );
}

// Generic section sub-header reused across feature screens
function SubHead({ children, icon }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 8, margin: '4px 0 12px' }}>
      {icon && <Icon name={icon} size={18} color={T.green} fill={1} />}
      <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15.5, color: T.fg, letterSpacing: -0.2 }}>{children}</span>
    </div>
  );
}

Object.assign(window, { ServicesHubScreen, SoonBanner, SubHead });
