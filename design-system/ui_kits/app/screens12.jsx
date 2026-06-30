// screens12.jsx — Painel de métricas, Banco de sementes, Learning Hub.

function MetricsScreen({ territory }) {
  const m = window.ARAH.metrics;
  const max = Math.max(...m.bars.map(b => b.v));
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 25">Painel de métricas — o pulso vivo do território. Visão de produto.</SoonBanner>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10, marginBottom: 14 }}>
        {[['Engajamento', m.engagement + '%', 'trending_up', T.green], ['Posts no mês', m.posts, 'article', T.blue], ['Eventos', m.events, 'event', T.amber || '#E8A06A'], ['Novos membros', '+' + m.newMembers, 'group_add', T.green]].map(([k, v, ic, c]) => (
          <Card key={k} style={{ padding: 15 }}>
            <Icon name={ic} size={20} color={c} fill={1} />
            <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 24, color: T.fg, marginTop: 8, letterSpacing: -0.5 }}>{v}</div>
            <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 1 }}>{k}</div>
          </Card>
        ))}
      </div>
      <Card style={{ padding: 16 }}>
        <SubHead icon="bar_chart">Atividade na semana</SubHead>
        <div style={{ display: 'flex', alignItems: 'flex-end', gap: 8, height: 130, marginTop: 4 }}>
          {m.bars.map(b => (
            <div key={b.label} style={{ flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6 }}>
              <div style={{ width: '100%', height: `${b.v / max * 100}%`, minHeight: 6, borderRadius: '7px 7px 3px 3px', background: T.greenGrad }} />
              <span style={{ fontFamily: T.sans, fontSize: 10.5, color: T.fg3 }}>{b.label}</span>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}

function SeedsScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 48">Banco de sementes e mudas — regenerar o território, juntos. Visão de produto.</SoonBanner>
      <Card style={{ padding: 16, marginBottom: 16, display: 'flex', alignItems: 'center', gap: 13, background: T.cardHiGrad }}>
        <div style={{ width: 46, height: 46, borderRadius: 13, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Icon name="forest" size={24} color={T.green} fill={1} />
        </div>
        <div style={{ flex: 1 }}>
          <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 18, color: T.fg }}>43 espécies</div>
          <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3 }}>disponíveis para troca em Camburi</div>
        </div>
        <Btn kind="primary" size="sm" icon="add" onClick={() => window.openJourney('create', window.JOURNEY_PRESETS.semente)}>Doar</Btn>
      </Card>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 11 }}>
        {window.ARAH.seeds.map(s => (
          <Card key={s.id} style={{ padding: 15 }}>
            <div style={{ width: 42, height: 42, borderRadius: 12, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: 11 }}>
              <Icon name={s.icon} size={22} color={T.green} fill={1} />
            </div>
            <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{s.name}</div>
            <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 2 }}>{s.qty}</div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 9 }}>
              <span style={{ fontFamily: T.sans, fontSize: 10.5, color: T.green, background: T.greenDim, padding: '2px 8px', borderRadius: 999 }}>{s.tag}</span>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 10, fontFamily: T.sans, fontSize: 11.5, color: T.fg3 }}>
              <Avatar color="#4F956F" name={s.who} size={16} /> {s.who}
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}

function LearningScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 45">Aprendizado — saberes e ofícios que vivem no território. Visão de produto.</SoonBanner>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
        {window.ARAH.courses.map(c => (
          <Card key={c.id} glow onClick={() => window.openJourney('course', c)} style={{ padding: 0, overflow: 'hidden' }}>
            <div style={{ display: 'flex' }}>
              <div style={{ width: 92, flexShrink: 0, background: `linear-gradient(150deg, ${c.avatar}40, ${c.avatar}12)`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Icon name={c.icon} size={34} color={T.fg} fill={0} style={{ opacity: 0.85 }} />
              </div>
              <div style={{ flex: 1, minWidth: 0, padding: 15 }}>
                <span style={{ fontFamily: T.sans, fontSize: 10, fontWeight: 700, letterSpacing: 0.6, textTransform: 'uppercase', color: T.green }}>{c.tag}</span>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15.5, color: T.fg, marginTop: 3, letterSpacing: -0.2, lineHeight: 1.25 }}>{c.title}</div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 8, fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>
                  <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}><Icon name="play_lesson" size={14} /> {c.lessons} aulas</span>
                  <span>· {c.level}</span>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 9 }}>
                  <Avatar color={c.avatar} name={c.teacher} size={18} resident /><span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg2 }}>{c.teacher}</span>
                </div>
              </div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}

Object.assign(window, { MetricsScreen, SeedsScreen, LearningScreen });
