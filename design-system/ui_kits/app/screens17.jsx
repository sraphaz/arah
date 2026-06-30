// screens17.jsx — Aluguéis, Hub digital (screens) + Rental/Delivery journeys.

function RentalScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 46">Aluguéis — equipamentos e espaços entre vizinhos. Visão de produto.</SoonBanner>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 11 }}>
        {window.ARAH.rentals.map(r => (
          <Card key={r.id} glow onClick={() => window.openJourney('rental', r)} style={{ padding: 0, overflow: 'hidden' }}>
            <div style={{ height: 84, background: `linear-gradient(150deg, ${r.avatar}40, ${r.avatar}12)`, display: 'flex', alignItems: 'center', justifyContent: 'center', position: 'relative' }}>
              <Icon name={r.icon} size={32} color={T.fg} fill={0} style={{ opacity: 0.9 }} />
              <span style={{ position: 'absolute', top: 8, left: 8, fontFamily: T.sans, fontSize: 10, fontWeight: 600, color: T.fg, background: 'rgba(11,12,10,0.5)', backdropFilter: 'blur(4px)', padding: '3px 8px', borderRadius: 999 }}>{r.tag}</span>
            </div>
            <div style={{ padding: 12 }}>
              <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14, color: T.fg, lineHeight: 1.25, letterSpacing: -0.2 }}>{r.title}</div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 5, marginTop: 6, color: T.fg3, fontFamily: T.sans, fontSize: 11.5 }}>
                <Avatar color={r.avatar} name={r.owner} size={16} resident /> {r.owner}
              </div>
              <div style={{ marginTop: 9 }}>
                <strong style={{ fontFamily: T.display, fontWeight: 700, fontSize: 15, color: T.green }}>{r.price}</strong>
                <span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}> / {r.unit}</span>
              </div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}

function DigitalScreen() {
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <SoonBanner phase="Fase 26">Hub de serviços digitais — talentos do território para o território. Visão de produto.</SoonBanner>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 11 }}>
        {window.ARAH.digitalServices.map(s => (
          <Card key={s.id} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: 14 }}>
            <div style={{ width: 46, height: 46, borderRadius: 13, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
              <Icon name={s.icon} size={23} color={T.green} fill={1} />
            </div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <span style={{ fontFamily: T.sans, fontSize: 10, fontWeight: 700, letterSpacing: 0.6, textTransform: 'uppercase', color: T.green }}>{s.tag}</span>
              <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14.5, color: T.fg, marginTop: 2, letterSpacing: -0.2, lineHeight: 1.2 }}>{s.title}</div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 5 }}>
                <Avatar color={s.avatar} name={s.who} size={18} /><span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>{s.who} · {s.price}</span>
              </div>
            </div>
            <Btn kind="dark" size="sm" icon="bookmark_add" style={{ flexShrink: 0 }} onClick={() => window.openJourney('create', window.JOURNEY_PRESETS.digital)}>Contratar</Btn>
          </Card>
        ))}
      </div>
    </div>
  );
}

// ---- Rental journey: item → período → PIX → success ----
function RentalJourney({ ctx, onClose }) {
  const r = ctx || window.ARAH.rentals[0];
  const [step, setStep] = React.useState(0);
  const [from, setFrom] = React.useState('Sex, 13/jun');
  const [days, setDays] = React.useState(2);
  const [done, setDone] = React.useState(false);
  const unit = parseInt(r.price.replace(/\D/g, ''));
  const total = `R$ ${unit * days}`;
  if (done) return <SuccessView title="Aluguel confirmado!" msg={`Você reservou "${r.title}" com ${r.owner}. Combine a retirada pelo chat do território.`} primaryLabel="Abrir conversa" onPrimary={onClose} secondaryLabel="Voltar" onSecondary={onClose} />;
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'lock' : 'arrow_forward'} onClick={next}>{step === 2 ? `Pagar ${total}` : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Alugar item" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <Card style={{ padding: 14, display: 'flex', gap: 12, alignItems: 'center', marginBottom: 18 }}>
          <div style={{ width: 48, height: 48, borderRadius: 12, background: `${r.avatar}22`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}><Icon name={r.icon} size={24} color={T.green} /></div>
          <div style={{ flex: 1, minWidth: 0 }}><div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{r.title}</div><div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3 }}>{r.owner} · {r.price}/{r.unit}</div></div>
        </Card>
        <JStepTitle>Quando você precisa?</JStepTitle>
        <JField label="Início"><PillSelect cols={2} value={from} onChange={setFrom} options={['Sex, 13/jun', 'Sáb, 14/jun', 'Dom, 15/jun', 'Seg, 16/jun']} /></JField>
        <JField label="Dias"><Counter value={days} onChange={setDays} min={1} max={30} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle sub="Combine a retirada com o dono pelo chat.">Retirada</JStepTitle>
        <JField><PillSelect cols={1} value={'Retirar com o dono'} onChange={() => {}} options={['Retirar com o dono']} /></JField>
        <Card style={{ padding: 13, display: 'flex', gap: 10, alignItems: 'center', background: 'rgba(166,214,185,0.08)' }}>
          <Icon name="verified_user" size={19} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2, lineHeight: 1.4 }}>Caução opcional combinada entre vizinhos do território.</span>
        </Card>
      </>}
      {step === 2 && <>
        <JStepTitle>Revise e pague</JStepTitle>
        <Card style={{ padding: '4px 16px', marginBottom: 18 }}>
          <ReviewRow icon="category" label={r.title} value={r.tag} />
          <ReviewRow icon="calendar_month" label="Início" value={from} />
          <ReviewRow icon="schedule" label="Período" value={`${days} ${days > 1 ? 'dias' : 'dia'} × ${r.price}`} />
          <div style={{ display: 'flex', alignItems: 'center', padding: '14px 0 4px' }}>
            <span style={{ flex: 1, fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg }}>Total</span>
            <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 19, color: T.green }}>{total}</span>
          </div>
        </Card>
        <PixPay amount={total} />
      </>}
    </JourneyShell>
  );
}

// ---- Delivery request journey: item → endereços → confirmar → success ----
function DeliveryRequestJourney({ onClose }) {
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  if (done) return <SuccessView icon="local_shipping" title="Entrega solicitada!" msg="Um entregador da vila vai aceitar e levar seu pedido. Você acompanha o trajeto em tempo real." primaryLabel="Acompanhar" onPrimary={onClose} secondaryLabel="Voltar" onSecondary={onClose} />;
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 1 ? 'send' : 'arrow_forward'} onClick={next}>{step === 1 ? 'Solicitar entrega' : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Solicitar entrega" step={step} steps={2} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub="Um vizinho leva pra você.">O que vai ser entregue?</JStepTitle>
        <JField label="Item / pedido"><input placeholder="Ex.: 2kg de peixe da Peixaria da Marta" style={jInput} /></JField>
        <JField label="Retirar em"><input defaultValue="Peixaria da Marta" style={jInput} /></JField>
        <JField label="Entregar em"><input defaultValue="Rua das Conchas, 12 — Camburi" style={jInput} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle>Confirme</JStepTitle>
        <Card style={{ padding: '4px 16px', marginBottom: 16 }}>
          <ReviewRow icon="inventory_2" label="Pedido" value="Peixe fresco" />
          <ReviewRow icon="storefront" label="Retirada" value="Peixaria da Marta" />
          <ReviewRow icon="home" label="Entrega" value="Rua das Conchas, 12" />
          <ReviewRow icon="payments" label="Frete local" value="R$ 8" accent={T.green} />
        </Card>
        <Card style={{ padding: 13, display: 'flex', gap: 10, alignItems: 'center', background: 'rgba(166,214,185,0.08)' }}>
          <Icon name="two_wheeler" size={19} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2, lineHeight: 1.4 }}>Entregadores são moradores do território — renda que fica na vila.</span>
        </Card>
      </>}
    </JourneyShell>
  );
}

Object.assign(window, { RentalScreen, DigitalScreen, RentalJourney, DeliveryRequestJourney });
