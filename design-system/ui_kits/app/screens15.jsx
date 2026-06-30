// screens15.jsx — Booking journeys: Reserva (hospedagem), Babá, Bem-estar, Checkout mercado.

function ReservaJourney({ ctx, onClose }) {
  const h = ctx || window.ARAH.hosting[0];
  const [step, setStep] = React.useState(0);
  const [date, setDate] = React.useState('Sex, 13/jun');
  const [nights, setNights] = React.useState(2);
  const [guests, setGuests] = React.useState(2);
  const [done, setDone] = React.useState(false);
  const total = `R$ ${parseInt(h.price.replace(/\D/g, '')) * nights}`;
  if (done) return <SuccessView title="Reserva confirmada!" msg={`Sua estadia em "${h.title}" está reservada. ${h.host} já foi avisado e vai te receber.`} primaryLabel="Ver minhas reservas" onPrimary={onClose} secondaryLabel="Voltar" onSecondary={onClose} />;
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'lock' : 'arrow_forward'} onClick={next}>{step === 2 ? `Pagar ${total}` : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Reservar estadia" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub={`${h.title} · ${h.host}`}>Quando você vem?</JStepTitle>
        <JField label="Data de entrada"><PillSelect cols={2} value={date} onChange={setDate} options={['Sex, 13/jun', 'Sáb, 14/jun', 'Sex, 20/jun', 'Sáb, 21/jun']} /></JField>
        <JField label="Noites"><Counter value={nights} onChange={setNights} min={1} max={30} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle sub={`Até ${h.guests} hóspedes nesta acomodação`}>Quantas pessoas?</JStepTitle>
        <JField label="Hóspedes"><Counter value={guests} onChange={setGuests} min={1} max={h.guests} /></JField>
        <Card style={{ padding: 14, display: 'flex', gap: 11, alignItems: 'center', background: T.cardHiGrad }}>
          <Avatar color={h.avatar} name={h.host} size={40} resident />
          <div><div style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, color: T.fg }}>Anfitrião: {h.host}</div><div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>Morador verificado · {h.reviews} avaliações</div></div>
        </Card>
      </>}
      {step === 2 && <>
        <JStepTitle>Revise e pague</JStepTitle>
        <Card style={{ padding: '4px 16px', marginBottom: 18 }}>
          <ReviewRow icon="cottage" label={h.title} value={h.tag} />
          <ReviewRow icon="calendar_month" label="Entrada" value={date} />
          <ReviewRow icon="bedtime" label="Noites" value={`${nights} × ${h.price}`} />
          <ReviewRow icon="group" label="Hóspedes" value={guests} />
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

function SitterJourney({ ctx, onClose }) {
  const s = ctx || window.ARAH.sitters[0];
  const [step, setStep] = React.useState(0);
  const [day, setDay] = React.useState('Sáb, 14/jun');
  const [time, setTime] = React.useState('14:00');
  const [hours, setHours] = React.useState(4);
  const [kids, setKids] = React.useState(1);
  const [done, setDone] = React.useState(false);
  const total = `R$ ${parseInt(s.price.replace(/\D/g, '')) * hours}`;
  if (done) return <SuccessView title="Solicitação enviada!" msg={`${s.name} recebeu seu pedido e vai confirmar em instantes. Você pode combinar os detalhes pelo chat.`} primaryLabel="Abrir conversa" onPrimary={onClose} secondaryLabel="Voltar" onSecondary={onClose} />;
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'send' : 'arrow_forward'} onClick={next}>{step === 2 ? 'Enviar solicitação' : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Contratar babá" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <div style={{ display: 'flex', gap: 12, alignItems: 'center', marginBottom: 20 }}>
          <Avatar color={s.avatar} name={s.name} size={52} resident />
          <div><div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 17, color: T.fg }}>{s.name}</div>
            <div style={{ display: 'inline-flex', alignItems: 'center', gap: 4, fontFamily: T.sans, fontSize: 12.5, color: T.fg2 }}><Icon name="star" size={14} color="#F5C451" fill={1} /> {s.rating} · {s.price} · {s.ages}</div></div>
        </div>
        <JStepTitle>Quando você precisa?</JStepTitle>
        <JField label="Dia"><PillSelect cols={2} value={day} onChange={setDay} options={['Sex, 13/jun', 'Sáb, 14/jun', 'Dom, 15/jun', 'Seg, 16/jun']} /></JField>
        <JField label="Horário de início"><PillSelect value={time} onChange={setTime} options={['08:00', '14:00', '18:00', '20:00']} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle sub="Para a babá se preparar melhor">Detalhes do cuidado</JStepTitle>
        <JField label="Duração (horas)"><Counter value={hours} onChange={setHours} min={1} max={12} suffix="h" /></JField>
        <JField label="Quantas crianças"><Counter value={kids} onChange={setKids} min={1} max={6} /></JField>
        <JField label="Observações"><textarea placeholder="Idades, rotina, alergias, necessidades especiais…" rows={3} style={{ ...jInput, resize: 'none', lineHeight: 1.5 }} /></JField>
      </>}
      {step === 2 && <>
        <JStepTitle>Revise o pedido</JStepTitle>
        <Card style={{ padding: '4px 16px', marginBottom: 16 }}>
          <ReviewRow icon="person" label="Babá" value={s.name} />
          <ReviewRow icon="calendar_month" label="Quando" value={`${day} · ${time}`} />
          <ReviewRow icon="schedule" label="Duração" value={`${hours}h`} />
          <ReviewRow icon="child_care" label="Crianças" value={kids} />
          <div style={{ display: 'flex', alignItems: 'center', padding: '14px 0 4px' }}>
            <span style={{ flex: 1, fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg }}>Estimativa</span>
            <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 19, color: T.green }}>{total}</span>
          </div>
        </Card>
        <Card style={{ padding: 13, display: 'flex', gap: 10, alignItems: 'center', background: 'rgba(166,214,185,0.08)' }}>
          <Icon name="verified_user" size={19} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2, lineHeight: 1.4 }}>Pagamento liberado só após o serviço, pela plataforma.</span>
        </Card>
      </>}
    </JourneyShell>
  );
}

function WellnessJourney({ ctx, onClose }) {
  const [step, setStep] = React.useState(0);
  const [service, setService] = React.useState('Yoga em grupo');
  const [slot, setSlot] = React.useState('Qua · 07:00');
  const [done, setDone] = React.useState(false);
  if (done) return <SuccessView title="Sessão agendada!" msg="Seu horário está reservado. Você recebeu a confirmação e um lembrete será enviado no dia." primaryLabel="Ver agenda" onPrimary={onClose} secondaryLabel="Voltar" onSecondary={onClose} />;
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 1 ? 'event_available' : 'arrow_forward'} onClick={next}>{step === 1 ? 'Confirmar agendamento' : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Agendar bem-estar" step={step} steps={2} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub={ctx?.name || 'Espaço Maré Yoga'}>Escolha o serviço</JStepTitle>
        <JField><PillSelect cols={2} value={service} onChange={setService} options={['Yoga em grupo', 'Yoga individual', 'Terapia sonora', 'Meditação']} /></JField>
        <JField label="Horários disponíveis"><PillSelect cols={2} value={slot} onChange={setSlot} options={['Qua · 07:00', 'Qua · 18:00', 'Sex · 07:00', 'Sáb · 09:00']} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle>Confirme</JStepTitle>
        <Card style={{ padding: '4px 16px' }}>
          <ReviewRow icon="self_improvement" label="Serviço" value={service} />
          <ReviewRow icon="spa" label="Local" value={ctx?.name || 'Espaço Maré Yoga'} />
          <ReviewRow icon="schedule" label="Horário" value={slot} />
          <ReviewRow icon="payments" label="Valor" value="R$ 60" accent={T.green} />
        </Card>
      </>}
    </JourneyShell>
  );
}

function CheckoutJourney({ ctx, onClose }) {
  const item = ctx || { title: 'Robalo fresco (kg)', price: 'R$ 42', seller: 'Peixaria da Marta', avatar: '#C9962B', icon: 'set_meal' };
  const [step, setStep] = React.useState(0);
  const [qty, setQty] = React.useState(1);
  const [fulfil, setFulfil] = React.useState('Retirar no local');
  const [done, setDone] = React.useState(false);
  const unit = parseInt(item.price.replace(/\D/g, ''));
  const delivery = fulfil === 'Entrega local' ? 8 : 0;
  const total = `R$ ${unit * qty + delivery}`;
  if (done) return <SuccessView title="Pedido confirmado!" msg={`${item.seller} recebeu seu pedido. ${fulfil === 'Entrega local' ? 'Um entregador da vila vai levar até você.' : 'É só retirar no local quando quiser.'}`} primaryLabel="Acompanhar pedido" onPrimary={onClose} secondaryLabel="Voltar ao mercado" onSecondary={onClose} />;
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'lock' : 'arrow_forward'} onClick={next}>{step === 2 ? `Pagar ${total}` : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Finalizar compra" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle>Sua sacola</JStepTitle>
        <Card style={{ padding: 14, display: 'flex', gap: 12, alignItems: 'center', marginBottom: 18 }}>
          <div style={{ width: 48, height: 48, borderRadius: 12, background: `${item.avatar}22`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}><Icon name={item.icon || 'shopping_basket'} size={24} color={T.green} /></div>
          <div style={{ flex: 1, minWidth: 0 }}><div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{item.title}</div><div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3 }}>{item.seller} · {item.price}</div></div>
        </Card>
        <JField label="Quantidade"><Counter value={qty} onChange={setQty} min={1} max={20} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle>Como você quer receber?</JStepTitle>
        <JField><PillSelect cols={1} value={fulfil} onChange={setFulfil} options={['Retirar no local', 'Entrega local']} /></JField>
        {fulfil === 'Entrega local' && <JField label="Endereço"><input defaultValue="Rua das Conchas, 12 — Camburi" style={jInput} /></JField>}
        <Card style={{ padding: 13, display: 'flex', gap: 10, alignItems: 'center', background: 'rgba(166,214,185,0.08)' }}>
          <Icon name={fulfil === 'Entrega local' ? 'local_shipping' : 'storefront'} size={19} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2 }}>{fulfil === 'Entrega local' ? 'Entrega por um vizinho · R$ 8' : 'Retirada gratuita com o vendedor'}</span>
        </Card>
      </>}
      {step === 2 && <>
        <JStepTitle>Pagamento</JStepTitle>
        <Card style={{ padding: '4px 16px', marginBottom: 18 }}>
          <ReviewRow label={`${item.title} × ${qty}`} value={`R$ ${unit * qty}`} />
          <ReviewRow label="Entrega" value={delivery ? `R$ ${delivery}` : 'Grátis'} />
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

Object.assign(window, { ReservaJourney, SitterJourney, WellnessJourney, CheckoutJourney });
