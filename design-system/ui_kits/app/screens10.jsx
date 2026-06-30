// screens10.jsx — Trocas, Entregas, Carteira / Moeda Territorial.

function TradesScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 20">Trocas comunitárias — escambo e doações sem dinheiro. Visão de produto.</SoonBanner>
      <Btn kind="primary" full size="md" icon="add" style={{ marginBottom: 14 }} onClick={() => window.openJourney('create', window.JOURNEY_PRESETS.troca)}>Propor uma troca</Btn>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {window.ARAH.trades.map(t => (
          <Card key={t.id} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: 14 }}>
            <div style={{ width: 44, height: 44, borderRadius: 12, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
              <Icon name={t.icon} size={22} color={T.green} fill={1} />
            </div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14.5, color: T.fg, letterSpacing: -0.2 }}>{t.title}</div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 5 }}>
                <Avatar color={t.avatar} name={t.who} size={18} />
                <span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>{t.who}</span>
                <span style={{ fontFamily: T.sans, fontSize: 11, color: T.green, background: T.greenDim, padding: '2px 8px', borderRadius: 999 }}>quer: {t.want}</span>
              </div>
            </div>
            <Btn kind="dark" size="sm" icon="swap_horiz" onClick={() => window.openJourney('create', window.JOURNEY_PRESETS.troca)}>Propor</Btn>
          </Card>
        ))}
      </div>
    </div>
  );
}

function DeliveryScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 21">Entregas territoriais — logística local entre vizinhos. Visão de produto.</SoonBanner>
      <Btn kind="primary" full size="md" icon="add" style={{ marginBottom: 14 }} onClick={() => window.openJourney('deliveryReq')}>Solicitar entrega</Btn>
      {/* live map snippet */}
      <Card style={{ padding: 0, overflow: 'hidden', marginBottom: 16 }}>
        <div style={{ position: 'relative', height: 150 }}>
          <MapCanvas height={150} mini tile={window.ARAH.content.t1.tile} />
          <div style={{ position: 'absolute', left: '34%', top: '52%', transform: 'translate(-50%,-50%)' }}>
            <div style={{ width: 30, height: 30, borderRadius: '50%', background: T.green, display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: T.greenGlow }}>
              <Icon name="two_wheeler" size={17} color="#0C1B10" fill={1} />
            </div>
          </div>
        </div>
      </Card>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {window.ARAH.deliveries.map(d => {
          const done = d.status === 'entregue';
          return (
            <Card key={d.id} style={{ padding: 14 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 11 }}>
                <div style={{ width: 42, height: 42, borderRadius: 12, background: done ? T.greenDim : T.blueDim, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                  <Icon name={d.icon} size={21} color={done ? T.green : T.blue} fill={1} />
                </div>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14, color: T.fg }}>{d.from} → {d.to}</div>
                  <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 2 }}>Entregador: {d.courier}</div>
                </div>
                <div style={{ textAlign: 'right', flexShrink: 0 }}>
                  <div style={{ display: 'inline-flex', alignItems: 'center', gap: 4, fontFamily: T.sans, fontSize: 11.5, fontWeight: 600, color: done ? T.green : T.blue, background: done ? T.greenDim : T.blueDim, padding: '4px 10px', borderRadius: 999 }}>
                    <Icon name={done ? 'check' : 'schedule'} size={13} fill={1} /> {d.status}
                  </div>
                  <div style={{ fontFamily: T.sans, fontSize: 11, color: T.fg3, marginTop: 4 }}>{d.eta}</div>
                </div>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}

function WalletScreen() {
  const w = window.ARAH.wallet;
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 22">Moeda territorial — créditos que circulam dentro da comunidade. Visão de produto.</SoonBanner>
      {/* balance card */}
      <Card glow style={{ padding: 20, marginBottom: 16, background: 'linear-gradient(155deg, #1E3A2A, #14241A)', border: '1px solid rgba(166,214,185,0.2)' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 14 }}>
          <div style={{ width: 30, height: 30, borderRadius: 9, background: T.greenSolid, display: 'flex', alignItems: 'center', justifyContent: 'center', fontFamily: T.display, fontWeight: 700, fontSize: 16, color: '#0C1B10' }}>{w.symbol}</div>
          <span style={{ fontFamily: T.sans, fontSize: 13, color: T.green, fontWeight: 600 }}>{w.name} · moeda de Camburi</span>
        </div>
        <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 38, color: '#fff', letterSpacing: -1 }}>{w.balance}</div>
        <div style={{ fontFamily: T.sans, fontSize: 13, color: 'rgba(255,255,255,0.6)', marginTop: 2 }}>≈ {w.brl}</div>
        <div style={{ display: 'flex', gap: 9, marginTop: 18 }}>
          <Btn kind="primary" size="sm" icon="north_east" style={{ flex: 1, background: T.greenSolid, color: '#0C1B10' }} onClick={() => window.openJourney('walletSend')}>Enviar</Btn>
          <Btn kind="dark" size="sm" icon="south_west" style={{ flex: 1, color: '#fff', borderColor: 'rgba(255,255,255,0.18)' }} onClick={() => window.openJourney('walletReceive')}>Receber</Btn>
          <Btn kind="dark" size="sm" icon="add" style={{ flex: 1, color: '#fff', borderColor: 'rgba(255,255,255,0.18)' }} onClick={() => window.openJourney('walletTopup')}>Adicionar</Btn>
        </div>
      </Card>
      <SubHead icon="receipt_long">Movimentações</SubHead>
      <Card style={{ overflow: 'hidden' }}>
        {w.transactions.map((tx, i) => (
          <div key={tx.id} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: '13px 15px', borderBottom: i === w.transactions.length - 1 ? 'none' : `1px solid ${T.line}` }}>
            <div style={{ width: 38, height: 38, borderRadius: 11, background: T.cardHi, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
              <Icon name={tx.icon} size={19} color={tx.neg ? T.fg2 : T.green} fill={tx.neg ? 0 : 1} />
            </div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontFamily: T.sans, fontSize: 14, fontWeight: 500, color: T.fg }}>{tx.label}</div>
              <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 1 }}>{tx.sub}</div>
            </div>
            <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 14.5, color: tx.neg ? T.fg2 : T.green }}>{tx.val}</span>
          </div>
        ))}
      </Card>
    </div>
  );
}

Object.assign(window, { TradesScreen, DeliveryScreen, WalletScreen });
