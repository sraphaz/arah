// screens16.jsx — Transaction/create journeys + JourneyHost router.

function WalletSendJourney({ onClose }) {
  const [step, setStep] = React.useState(0);
  const [amount, setAmount] = React.useState('80');
  const [to, setTo] = React.useState(null);
  const [done, setDone] = React.useState(false);
  const people = [{ n: 'Dona Marta', a: '#C9962B' }, { n: 'Bruno Caiçara', a: '#377B57' }, { n: 'Seu Tião', a: '#4F956F' }, { n: 'Coletivo Maré', a: '#2A6FDB' }];
  const confirm = () => {
    window.arahMutate && window.arahMutate(() => {
      const w = window.ARAH.wallet;
      const cur = parseInt(String(w.balance).replace(/\D/g, '')) || 0;
      const n = Math.max(0, cur - parseInt(amount));
      w.balance = 'A̧ ' + n; w.brl = 'R$ ' + n + ',00';
      w.transactions.unshift({ id: 'ws' + Date.now(), label: to?.n || 'Envio', sub: 'Transferência enviada', val: '- A̧ ' + amount, icon: 'north_east', neg: true });
    });
    setDone(true);
  };
  if (done) return <SuccessView title="Aratá enviado!" msg={`A̧ ${amount} foram transferidos para ${to?.n}. A moeda continua circulando em Camburi.`} primaryLabel="Ver carteira" onPrimary={onClose} />;
  const next = () => step < 2 ? setStep(step + 1) : confirm();
  const canNext = step === 1 ? !!to : true;
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'north_east' : 'arrow_forward'} onClick={() => canNext && next()} style={{ opacity: canNext ? 1 : 0.5 }}>{step === 2 ? 'Confirmar envio' : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Enviar Aratá" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub={`Saldo disponível: ${window.ARAH.wallet.balance}`}>Quanto enviar?</JStepTitle>
        <div style={{ textAlign: 'center', padding: '20px 0' }}>
          <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 52, color: T.fg, letterSpacing: -2 }}>A̧ {amount || '0'}</div>
        </div>
        <PillSelect cols={4} value={amount} onChange={setAmount} options={['20', '50', '80', '120']} />
      </>}
      {step === 1 && <>
        <JStepTitle>Para quem?</JStepTitle>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 9 }}>
          {people.map(p => {
            const on = to?.n === p.n;
            return (
              <Card key={p.n} onClick={() => setTo(p)} hi={on} style={{ display: 'flex', alignItems: 'center', gap: 12, padding: 12, border: `1px solid ${on ? T.green : T.line}` }}>
                <Avatar color={p.a} name={p.n} size={40} resident />
                <span style={{ flex: 1, fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{p.n}</span>
                <Icon name={on ? 'check_circle' : 'radio_button_unchecked'} size={21} color={on ? T.green : T.fg3} fill={on ? 1 : 0} />
              </Card>
            );
          })}
        </div>
      </>}
      {step === 2 && <>
        <JStepTitle>Confirme o envio</JStepTitle>
        <Card style={{ padding: 22, textAlign: 'center', marginBottom: 16 }}>
          <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 40, color: T.green, letterSpacing: -1 }}>A̧ {amount}</div>
          <div style={{ fontFamily: T.sans, fontSize: 13, color: T.fg3, margin: '6px 0 16px' }}>≈ R$ {amount},00</div>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 9 }}>
            <Avatar color={to?.a} name={to?.n || ''} size={32} resident />
            <span style={{ fontFamily: T.sans, fontSize: 14, color: T.fg }}>para <strong style={{ fontWeight: 600 }}>{to?.n}</strong></span>
          </div>
        </Card>
      </>}
    </JourneyShell>
  );
}

function SubscriptionJourney({ ctx, onClose }) {
  const tier = ctx || window.ARAH.subscriptionTiers[1];
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  if (done) return <SuccessView icon="favorite" title="Você é Guardião!" msg={`Obrigado por sustentar Camburi. Sua assinatura ${tier.name} mantém a infraestrutura comunitária viva e independente.`} primaryLabel="Concluir" onPrimary={onClose} />;
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 1 ? 'lock' : 'arrow_forward'} onClick={next}>{step === 1 ? `Assinar · ${tier.price}${tier.period}` : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Apoiar o território" step={step} steps={2} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub="Apoio recorrente ao território">Plano {tier.name}</JStepTitle>
        <Card glow style={{ padding: 18 }}>
          <div style={{ display: 'flex', alignItems: 'baseline' }}>
            <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 18, color: T.fg }}>{tier.name}</span>
            <span style={{ marginLeft: 'auto', fontFamily: T.display, fontWeight: 700, fontSize: 24, color: T.green }}>{tier.price}</span>
            <span style={{ fontFamily: T.sans, fontSize: 13, color: T.fg3 }}>{tier.period}</span>
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 9, marginTop: 15 }}>
            {tier.perks.map(p => <div key={p} style={{ display: 'flex', alignItems: 'center', gap: 9, fontFamily: T.sans, fontSize: 13.5, color: T.fg2 }}><Icon name="check_circle" size={17} color={T.green} fill={1} /> {p}</div>)}
          </div>
        </Card>
      </>}
      {step === 1 && <><JStepTitle>Pagamento</JStepTitle><PixPay amount={`${tier.price}${tier.period}`} /></>}
    </JourneyShell>
  );
}

function ResidenciaJourney({ onClose, onApprove }) {
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  const [proof, setProof] = React.useState(false);
  if (done) return <SuccessView icon="how_to_reg" title="Solicitação enviada!" msg="A curadoria do território vai analisar seu comprovante. Você será avisado assim que sua residência for confirmada." primaryLabel="Entendi" onPrimary={() => { onApprove && onApprove(); onClose(); }} />;
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'send' : 'arrow_forward'} onClick={next}>{step === 2 ? 'Enviar solicitação' : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Confirmar residência" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub="Moradores acessam conteúdo restrito, votam e participam da gestão.">Torne-se morador de Camburi</JStepTitle>
        <Card style={{ padding: 14, display: 'flex', gap: 11, alignItems: 'center', marginBottom: 11, background: T.cardHiGrad }}>
          <Icon name="my_location" size={20} color={T.green} fill={1} />
          <div style={{ flex: 1 }}><div style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, color: T.fg }}>Presença no território</div><div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>Confirmada por GPS · privada</div></div>
          <Icon name="check_circle" size={20} color={T.green} fill={1} />
        </Card>
      </>}
      {step === 1 && <>
        <JStepTitle sub="Conta de luz, água, contrato ou declaração da associação.">Envie um comprovante</JStepTitle>
        <button onClick={() => setProof(true)} style={{ width: '100%', padding: '30px', borderRadius: 16, cursor: 'pointer', background: T.cardFlat, border: `1.5px dashed ${proof ? T.green : T.lineHi}`, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 10, WebkitTapHighlightColor: 'transparent' }}>
          <Icon name={proof ? 'check_circle' : 'upload_file'} size={34} color={proof ? T.green : T.fg2} fill={proof ? 1 : 0} />
          <span style={{ fontFamily: T.sans, fontSize: 14, fontWeight: 500, color: proof ? T.green : T.fg2 }}>{proof ? 'comprovante-residencia.pdf' : 'Anexar comprovante'}</span>
        </button>
      </>}
      {step === 2 && <>
        <JStepTitle>Revise</JStepTitle>
        <Card style={{ padding: '4px 16px' }}>
          <ReviewRow icon="forest" label="Território" value="Camburi" />
          <ReviewRow icon="my_location" label="Presença GPS" value="Confirmada" accent={T.green} />
          <ReviewRow icon="description" label="Comprovante" value="Anexado" accent={T.green} />
          <ReviewRow icon="shield_person" label="Análise" value="Curadoria local" />
        </Card>
      </>}
    </JourneyShell>
  );
}

// Generic "create / propose" journey (demanda, oferta, semente, troca)
function CreateJourney({ ctx, onClose }) {
  const cfg = ctx || {};
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  const [kind, setKind] = React.useState(cfg.kinds ? cfg.kinds[0] : null);
  if (done) return <SuccessView title={cfg.successTitle} msg={cfg.successMsg} primaryLabel="Ver no território" onPrimary={onClose} secondaryLabel="Voltar" onSecondary={onClose} />;
  const next = () => step < 1 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 1 ? 'send' : 'arrow_forward'} onClick={next}>{step === 1 ? cfg.cta : 'Continuar'}</Btn>;
  return (
    <JourneyShell title={cfg.title} step={step} steps={2} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub={cfg.sub}>{cfg.heading}</JStepTitle>
        {cfg.kinds && <JField label={cfg.kindLabel}><PillSelect cols={cfg.kinds.length} value={kind} onChange={setKind} options={cfg.kinds} /></JField>}
        <JField label={cfg.f1}><input placeholder={cfg.f1ph} style={jInput} /></JField>
        {cfg.f2 && <JField label={cfg.f2}><input placeholder={cfg.f2ph} style={jInput} /></JField>}
        <JField label="Descrição"><textarea placeholder={cfg.descPh} rows={3} style={{ ...jInput, resize: 'none', lineHeight: 1.5 }} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle sub="Confira antes de publicar no território.">Tudo certo?</JStepTitle>
        <Card style={{ padding: '4px 16px' }}>
          {kind && <ReviewRow icon={cfg.icon} label={cfg.kindLabel} value={kind} accent={T.green} />}
          <ReviewRow icon="forest" label="Território" value="Camburi" />
          <ReviewRow icon="visibility" label="Visível para" value="Moradores e visitantes" />
        </Card>
        <Card style={{ padding: 13, display: 'flex', gap: 10, alignItems: 'center', background: 'rgba(166,214,185,0.08)', marginTop: 14 }}>
          <Icon name="groups" size={19} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2, lineHeight: 1.4 }}>{cfg.note}</span>
        </Card>
      </>}
    </JourneyShell>
  );
}

// Course enrollment (single confirm)
function CourseJourney({ ctx, onClose }) {
  const c = ctx || window.ARAH.courses[0];
  const [done, setDone] = React.useState(false);
  if (done) return <SuccessView icon="school" title="Matrícula feita!" msg={`Você entrou em "${c.title}". ${c.teacher} vai te avisar quando a próxima oficina começar.`} primaryLabel="Concluir" onPrimary={onClose} />;
  return (
    <JourneyShell title="Participar da oficina" step={0} steps={1} onClose={onClose} footer={<Btn kind="primary" full size="lg" icon="how_to_reg" onClick={() => setDone(true)}>Confirmar participação</Btn>}>
      <JStepTitle sub={c.tag}>{c.title}</JStepTitle>
      <Card style={{ padding: '4px 16px' }}>
        <ReviewRow icon="person" label="Quem ensina" value={c.teacher} />
        <ReviewRow icon="play_lesson" label="Aulas" value={`${c.lessons} encontros`} />
        <ReviewRow icon="signal_cellular_alt" label="Nível" value={c.level} />
        <ReviewRow icon="payments" label="Valor" value="Gratuito · saber comunitário" accent={T.green} />
      </Card>
    </JourneyShell>
  );
}

// ---- Router ----
function JourneyHost({ journey, onClose, onApprove }) {
  if (!journey) return null;
  const { id, ctx } = journey;
  switch (id) {
    case 'reserva': return <ReservaJourney ctx={ctx} onClose={onClose} />;
    case 'sitter': return <SitterJourney ctx={ctx} onClose={onClose} />;
    case 'wellness': return <WellnessJourney ctx={ctx} onClose={onClose} />;
    case 'checkout': return <CheckoutJourney ctx={ctx} onClose={onClose} />;
    case 'walletSend': return <WalletSendJourney onClose={onClose} />;
    case 'subscribe': return <SubscriptionJourney ctx={ctx} onClose={onClose} />;
    case 'residencia': return <ResidenciaJourney onClose={onClose} onApprove={onApprove} />;
    case 'course': return <CourseJourney ctx={ctx} onClose={onClose} />;
    case 'rental': return <RentalJourney ctx={ctx} onClose={onClose} />;
    case 'deliveryReq': return <DeliveryRequestJourney onClose={onClose} />;
    case 'vote': return <VoteJourney ctx={ctx} onClose={onClose} />;
    case 'groupBuy': return <GroupBuyJoinJourney ctx={ctx} onClose={onClose} />;
    case 'addProduct': return <AddProductJourney ctx={ctx} onClose={onClose} />;
    case 'walletReceive': return <WalletReceiveJourney onClose={onClose} />;
    case 'walletTopup': return <WalletTopupJourney onClose={onClose} />;
    case 'joinEvent': return <JoinEventJourney ctx={ctx} onClose={onClose} />;
    case 'createElection': return <CreateElectionJourney onClose={onClose} />;
    case 'create': return <CreateJourney ctx={ctx} onClose={onClose} />;
    default: return null;
  }
}

// Context presets for the generic create journey
const JOURNEY_PRESETS = {
  demanda: { title: 'Nova demanda ou oferta', heading: 'O que você quer divulgar?', sub: 'Peça uma mão ou ofereça o que você faz.', icon: 'swap_horiz', kindLabel: 'Tipo', kinds: ['Demanda', 'Oferta'], f1: 'Título', f1ph: 'Ex.: Preciso de pedreiro', f2: 'Categoria', f2ph: 'Construção, transporte…', descPh: 'Detalhe o que precisa ou oferece…', cta: 'Publicar', successTitle: 'Publicado!', successMsg: 'Sua demanda está no território. Vizinhos podem responder pelo chat.', note: 'Quem puder ajudar fala com você pelo chat do território.' },
  semente: { title: 'Doar muda ou semente', heading: 'O que você quer compartilhar?', sub: 'Fortaleça a regeneração de Camburi.', icon: 'potted_plant', f1: 'Espécie', f1ph: 'Ex.: Ipê-amarelo', f2: 'Quantidade', f2ph: 'Ex.: 20 sementes', descPh: 'Origem, época de plantio, cuidados…', cta: 'Doar ao banco', successTitle: 'Doação registrada!', successMsg: 'Sua espécie entrou no banco de sementes do território. Outros moradores já podem solicitar.', note: 'A muda fica disponível para troca com outros moradores.' },
  troca: { title: 'Propor troca', heading: 'O que você oferece?', sub: 'Escambo e doações entre vizinhos.', icon: 'handshake', f1: 'Ofereço', f1ph: 'Ex.: Mudas de ipê', f2: 'Quero em troca', f2ph: 'Ex.: Adubo orgânico', descPh: 'Detalhes da troca…', cta: 'Publicar troca', successTitle: 'Troca publicada!', successMsg: 'Sua proposta está no território. Quem topar fala com você pelo chat.', note: 'Combine os detalhes da troca diretamente pelo chat.' },
  digital: { title: 'Contratar serviço digital', heading: 'O que você precisa?', sub: 'Talentos digitais do território.', icon: 'apps', f1: 'Serviço', f1ph: 'Ex.: Logo para minha loja', f2: 'Prazo', f2ph: 'Ex.: 2 semanas', descPh: 'Descreva o que você precisa…', cta: 'Enviar pedido', successTitle: 'Pedido enviado!', successMsg: 'O profissional do território vai responder com uma proposta pelo chat.', note: 'A proposta e o pagamento são combinados pelo chat do território.' },
};
window.JOURNEY_PRESETS = JOURNEY_PRESETS;

Object.assign(window, { WalletSendJourney, SubscriptionJourney, ResidenciaJourney, CreateJourney, CourseJourney, JourneyHost });
