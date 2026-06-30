// screens18.jsx — Missing journeys: voto, compra coletiva, produto, carteira (receber/adicionar),
// participar de evento, abrir eleição (curador). Reuses the JourneyShell framework.

// ---- Votar em eleição ----
function VoteJourney({ ctx, onClose }) {
  const c = ctx?.candidate || { name: 'Dona Marta', avatar: '#C9962B', pitch: 'Pesca artesanal e feira' };
  const el = ctx?.election || { title: 'Conselho de Moradores 2026' };
  const [step, setStep] = React.useState(0);
  const [done, setDone] = React.useState(false);
  const protocolo = 'ARH-' + Math.random().toString(36).slice(2, 8).toUpperCase();
  if (done) return <SuccessView icon="how_to_vote" title="Voto registrado!" msg={`Seu voto em ${c.name} foi computado de forma secreta e auditável. Protocolo ${protocolo}.`} primaryLabel="Ver resultados parciais" onPrimary={() => { ctx?.onConfirm && ctx.onConfirm(); onClose(); }} secondaryLabel="Fechar" onSecondary={onClose} />;
  const footer = <Btn kind="primary" full size="lg" icon={step === 1 ? 'how_to_vote' : 'arrow_forward'} onClick={() => step < 1 ? setStep(1) : setDone(true)}>{step === 1 ? 'Confirmar voto' : 'Revisar voto'}</Btn>;
  return (
    <JourneyShell title="Votar" step={step} steps={2} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub={el.title}>Confira seu candidato</JStepTitle>
        <Card glow style={{ padding: 18, display: 'flex', gap: 14, alignItems: 'center', marginBottom: 16 }}>
          <Avatar color={c.avatar} name={c.name} size={56} resident />
          <div><div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 17, color: T.fg }}>{c.name}</div>
            <div style={{ fontFamily: T.sans, fontSize: 13, color: T.fg2, marginTop: 2 }}>{c.pitch}</div></div>
        </Card>
        <Card style={{ padding: 13, display: 'flex', gap: 10, alignItems: 'center', background: 'rgba(166,214,185,0.08)' }}>
          <Icon name="lock" size={19} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2, lineHeight: 1.4 }}>Seu voto é secreto. Cada morador vota uma vez por eleição.</span>
        </Card>
      </>}
      {step === 1 && <>
        <JStepTitle sub="Esta ação não pode ser desfeita.">Confirmar voto</JStepTitle>
        <Card style={{ padding: '4px 16px' }}>
          <ReviewRow icon="how_to_vote" label="Eleição" value={el.title} />
          <ReviewRow icon="person" label="Candidato" value={c.name} accent={T.green} />
          <ReviewRow icon="verified" label="Validação" value="1 morador · 1 voto" />
          <ReviewRow icon="lock" label="Sigilo" value="Voto secreto" />
        </Card>
      </>}
    </JourneyShell>
  );
}

// ---- Entrar na compra coletiva (com pagamento) ----
function GroupBuyJoinJourney({ ctx, onClose }) {
  const g = ctx || window.ARAH.groupBuyDetail || { title: 'Compra coletiva', icon: 'groups_2', price: 'R$ 2.400', saved: 'R$ 1.100' };
  const [step, setStep] = React.useState(0);
  const [qty, setQty] = React.useState(1);
  const [done, setDone] = React.useState(false);
  const unit = parseInt((g.price || 'R$ 2400').replace(/\D/g, '')) || 2400;
  const total = `R$ ${(unit * qty).toLocaleString('pt-BR')}`;
  if (done) return <SuccessView icon="groups_2" title="Você entrou!" msg={`Sua reserva na "${g.title}" está confirmada. Quando a meta for atingida, o pedido é fechado e você é avisado.`} primaryLabel="Acompanhar" onPrimary={() => { ctx?.onConfirm && ctx.onConfirm(); onClose(); }} secondaryLabel="Fechar" onSecondary={onClose} />;
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'lock' : 'arrow_forward'} onClick={next}>{step === 2 ? `Reservar ${total}` : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Entrar na compra coletiva" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub={g.title}>Quantas unidades?</JStepTitle>
        <JField label="Sua reserva"><Counter value={qty} onChange={setQty} min={1} max={5} /></JField>
        <Card style={{ padding: 13, display: 'flex', gap: 10, alignItems: 'center', background: 'rgba(166,214,185,0.08)' }}>
          <Icon name="savings" size={19} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2 }}>Economia estimada de <strong style={{ color: T.green, fontWeight: 700 }}>{g.saved}</strong> por unidade na meta.</span>
        </Card>
      </>}
      {step === 1 && <>
        <JStepTitle sub="Como funciona">Pagamento garantido</JStepTitle>
        <Card style={{ padding: 16 }}>
          {['O valor fica reservado até a meta ser atingida.', 'Se a meta não for batida no prazo, você é reembolsado.', 'Atingida a meta, o pedido é fechado com o fornecedor.'].map((t, i) => (
            <div key={i} style={{ display: 'flex', gap: 11, alignItems: 'flex-start', padding: '9px 0' }}>
              <Icon name="check_circle" size={18} color={T.green} fill={1} style={{ marginTop: 1 }} />
              <span style={{ fontFamily: T.sans, fontSize: 14, color: T.fg2, lineHeight: 1.45 }}>{t}</span>
            </div>
          ))}
        </Card>
      </>}
      {step === 2 && <>
        <JStepTitle>Reserve sua vaga</JStepTitle>
        <Card style={{ padding: '4px 16px', marginBottom: 18 }}>
          <ReviewRow label={`Unidades`} value={qty} />
          <ReviewRow label="Preço na meta" value={g.price} />
          <div style={{ display: 'flex', alignItems: 'center', padding: '14px 0 4px' }}>
            <span style={{ flex: 1, fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg }}>Total reservado</span>
            <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 19, color: T.green }}>{total}</span>
          </div>
        </Card>
        <PixPay amount={total} />
      </>}
    </JourneyShell>
  );
}

// ---- Adicionar produto à loja ----
function AddProductJourney({ ctx, onClose }) {
  const [step, setStep] = React.useState(0);
  const [name, setName] = React.useState('');
  const [price, setPrice] = React.useState('');
  const [cat, setCat] = React.useState('Alimento');
  const [avail, setAvail] = React.useState('Disponível');
  const [photo, setPhoto] = React.useState(false);
  const [done, setDone] = React.useState(false);
  const iconFor = { Alimento: 'lunch_dining', Serviço: 'handyman', Artesanato: 'redeem', Aluguel: 'category' };
  const publish = () => {
    const prod = { id: 'sp' + Date.now(), title: name || 'Novo produto', price: price ? (price.startsWith('R$') ? price : 'R$ ' + price) : 'R$ 0', tag: cat, icon: iconFor[cat] || 'sell', active: true };
    ctx?.onConfirm && ctx.onConfirm(prod);
    setDone(true);
  };
  if (done) return <SuccessView icon="storefront" title="Produto publicado!" msg="Seu item já aparece no mercado do território. Vizinhos podem comprar e falar com você pelo chat." primaryLabel="Ver minha loja" onPrimary={onClose} secondaryLabel="Adicionar outro" onSecondary={() => { setStep(0); setDone(false); setPhoto(false); setName(''); setPrice(''); }} />;
  const next = () => step < 2 ? setStep(step + 1) : publish();
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'publish' : 'arrow_forward'} onClick={next}>{step === 2 ? 'Publicar produto' : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Novo produto" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub="O que você quer vender no território?">Detalhes</JStepTitle>
        <JField label="Nome do produto"><input value={name} onChange={e => setName(e.target.value)} placeholder="Ex.: Doce de banana caseiro" style={jInput} /></JField>
        <JField label="Categoria"><PillSelect cols={2} value={cat} onChange={setCat} options={['Alimento', 'Serviço', 'Artesanato', 'Aluguel']} /></JField>
        <JField label="Preço"><input value={price} onChange={e => setPrice(e.target.value)} placeholder="R$ 0,00" inputMode="numeric" style={jInput} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle>Foto e descrição</JStepTitle>
        <JField label="Foto do produto">
          <button onClick={() => setPhoto(true)} style={{ width: '100%', padding: '26px', borderRadius: 16, cursor: 'pointer', background: T.cardFlat, border: `1.5px dashed ${photo ? T.green : T.lineHi}`, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 9, WebkitTapHighlightColor: 'transparent' }}>
            <Icon name={photo ? 'check_circle' : 'add_a_photo'} size={30} color={photo ? T.green : T.fg2} fill={photo ? 1 : 0} />
            <span style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 500, color: photo ? T.green : T.fg2 }}>{photo ? 'foto-produto.jpg' : 'Adicionar foto'}</span>
          </button>
        </JField>
        <JField label="Descrição"><textarea placeholder="Ingredientes, tamanho, prazo de entrega…" rows={3} style={{ ...jInput, resize: 'none', lineHeight: 1.5 }} /></JField>
        <JField label="Disponibilidade"><PillSelect value={avail} onChange={setAvail} options={['Disponível', 'Sob encomenda']} /></JField>
      </>}
      {step === 2 && <>
        <JStepTitle sub="Confira antes de publicar.">Tudo certo?</JStepTitle>
        <Card style={{ padding: '4px 16px' }}>
          <ReviewRow icon="sell" label="Produto" value={name || '—'} accent={T.green} />
          <ReviewRow icon="category" label="Categoria" value={cat} />
          <ReviewRow icon="payments" label="Preço" value={price ? (price.startsWith('R$') ? price : 'R$ ' + price) : '—'} />
          <ReviewRow icon="image" label="Foto" value={photo ? 'Anexada' : 'Sem foto'} accent={photo ? T.green : T.fg3} />
          <ReviewRow icon="inventory" label="Disponibilidade" value={avail} />
        </Card>
      </>}
    </JourneyShell>
  );
}

// ---- Receber Aratá (QR + chave) ----
function WalletReceiveJourney({ onClose }) {
  const w = window.ARAH.wallet;
  return (
    <JourneyShell title="Receber Aratá" step={0} steps={1} onClose={onClose} footer={<Btn kind="primary" full size="lg" icon="ios_share" onClick={onClose}>Compartilhar cobrança</Btn>}>
      <JStepTitle sub="Mostre o código para quem vai te pagar">Sua cobrança</JStepTitle>
      <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', padding: '22px', borderRadius: 16, background: '#fff', marginBottom: 16 }}>
        <Icon name="qr_code_2" size={150} color="#0E2117" fill={0} />
      </div>
      <Card style={{ padding: '4px 16px' }}>
        <ReviewRow icon="account_circle" label="Recebedor" value="Ana Ribeiro" />
        <ReviewRow icon="tag" label="Carteira" value={`${w.name} · Camburi`} />
        <ReviewRow icon="account_balance_wallet" label="Saldo atual" value={w.balance} accent={T.green} />
      </Card>
    </JourneyShell>
  );
}

// ---- Adicionar Aratá (top-up) ----
function WalletTopupJourney({ onClose }) {
  const [step, setStep] = React.useState(0);
  const [amount, setAmount] = React.useState('100');
  const [done, setDone] = React.useState(false);
  const confirm = () => {
    window.arahMutate(() => {
      const w = window.ARAH.wallet;
      const cur = parseInt(String(w.balance).replace(/\D/g, '')) || 0;
      const n = cur + parseInt(amount);
      w.balance = 'A̧ ' + n; w.brl = 'R$ ' + n + ',00';
      w.transactions.unshift({ id: 'wt' + Date.now(), label: 'Recarga de Aratá', sub: 'via PIX', val: '+ A̧ ' + amount, icon: 'add', neg: false });
    });
    setDone(true);
  };
  if (done) return <SuccessView title="Aratá adicionado!" msg={`A̧ ${amount} entraram na sua carteira. A moeda circula só dentro do território de Camburi.`} primaryLabel="Ver carteira" onPrimary={onClose} accent={T.green} />;
  const footer = <Btn kind="primary" full size="lg" icon={step === 1 ? 'lock' : 'arrow_forward'} onClick={() => step < 1 ? setStep(1) : confirm()}>{step === 1 ? `Pagar R$ ${amount}` : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Adicionar Aratá" step={step} steps={2} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub="1 Aratá = R$ 1,00 · lastreado pelo fundo comunitário">Quanto adicionar?</JStepTitle>
        <div style={{ textAlign: 'center', padding: '18px 0' }}>
          <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 52, color: T.fg, letterSpacing: -2 }}>A̧ {amount || '0'}</div>
        </div>
        <PillSelect cols={4} value={amount} onChange={setAmount} options={['50', '100', '200', '500']} />
      </>}
      {step === 1 && <><JStepTitle sub="Pague via PIX para creditar sua carteira.">Pagamento</JStepTitle><PixPay amount={`R$ ${amount},00`} /></>}
    </JourneyShell>
  );
}

// ---- Participar de evento ----
function JoinEventJourney({ ctx, onClose }) {
  const e = ctx || { title: 'Mutirão de limpeza', time: 'Sáb · 08:00', place: 'Praça da Capela' };
  const [going, setGoing] = React.useState('1');
  const [cal, setCal] = React.useState(true);
  const [done, setDone] = React.useState(false);
  if (done) return <SuccessView icon="event_available" title="Presença confirmada!" msg={`Te esperamos no "${e.title}". ${cal ? 'O evento foi adicionado à sua agenda e ' : ''}você receberá um lembrete no dia.`} primaryLabel="Concluir" onPrimary={() => { ctx?.onConfirm && ctx.onConfirm(); onClose(); }} secondaryLabel="Fechar" onSecondary={onClose} />;
  return (
    <JourneyShell title="Participar do evento" step={0} steps={1} onClose={onClose} footer={<Btn kind="primary" full size="lg" icon="event_available" onClick={() => setDone(true)}>Confirmar presença</Btn>}>
      <JStepTitle sub={`${e.time} · ${e.place}`}>{e.title}</JStepTitle>
      <JField label="Quantas pessoas vão com você?"><PillSelect cols={4} value={going} onChange={setGoing} options={['Só eu', '1', '2', '3+']} /></JField>
      <button onClick={() => setCal(!cal)} style={{ width: '100%', display: 'flex', alignItems: 'center', gap: 12, padding: '14px 15px', borderRadius: 14, cursor: 'pointer', background: T.cardFlat, border: `1px solid ${T.line}`, WebkitTapHighlightColor: 'transparent' }}>
        <Icon name="calendar_add_on" size={20} color={T.green} fill={1} />
        <span style={{ flex: 1, textAlign: 'left', fontFamily: T.sans, fontSize: 14, color: T.fg }}>Adicionar à minha agenda</span>
        <div style={{ width: 44, height: 26, borderRadius: 999, background: cal ? T.greenSolid : T.cardHi, position: 'relative', transition: 'background .2s' }}>
          <span style={{ position: 'absolute', top: 3, left: cal ? 21 : 3, width: 20, height: 20, borderRadius: '50%', background: '#fff', transition: 'left .2s' }} />
        </div>
      </button>
    </JourneyShell>
  );
}

// ---- Abrir nova eleição (curador) ----
function CreateElectionJourney({ onClose }) {
  const [step, setStep] = React.useState(0);
  const [type, setType] = React.useState('Conselho');
  const [deadline, setDeadline] = React.useState('7 dias');
  const [done, setDone] = React.useState(false);
  if (done) return <SuccessView icon="how_to_vote" title="Eleição aberta!" msg="A votação foi publicada para todos os moradores de Camburi. Você acompanha os resultados em tempo real na gestão." primaryLabel="Concluir" onPrimary={onClose} accent={T.alert} />;
  const next = () => step < 2 ? setStep(step + 1) : setDone(true);
  const footer = <Btn kind="primary" full size="lg" icon={step === 2 ? 'how_to_vote' : 'arrow_forward'} onClick={next}>{step === 2 ? 'Abrir votação' : 'Continuar'}</Btn>;
  return (
    <JourneyShell title="Abrir eleição" step={step} steps={3} onBack={() => setStep(step - 1)} onClose={onClose} footer={footer}>
      {step === 0 && <>
        <JStepTitle sub="Governança comunitária do território">Tipo de votação</JStepTitle>
        <JField><PillSelect cols={1} value={type} onChange={setType} options={['Conselho', 'Representante de tema', 'Consulta pública']} /></JField>
        <JField label="Título"><input placeholder="Ex.: Conselho de Moradores 2026" style={jInput} /></JField>
      </>}
      {step === 1 && <>
        <JStepTitle sub="Quem pode se candidatar e por quanto tempo.">Regras</JStepTitle>
        <JField label="Prazo de votação"><PillSelect cols={3} value={deadline} onChange={setDeadline} options={['3 dias', '7 dias', '15 dias']} /></JField>
        <JField label="Candidatos" hint="No app real, moradores se inscrevem ou são indicados.">
          <Card style={{ padding: 12 }}>
            {['Dona Marta', 'Seu Tião', 'Bruno Caiçara'].map(n => (
              <div key={n} style={{ display: 'flex', alignItems: 'center', gap: 10, padding: '7px 0' }}>
                <Avatar color="#4F956F" name={n} size={30} resident /><span style={{ flex: 1, fontFamily: T.sans, fontSize: 14, color: T.fg }}>{n}</span>
                <Icon name="check_circle" size={18} color={T.green} fill={1} />
              </div>
            ))}
          </Card>
        </JField>
      </>}
      {step === 2 && <>
        <JStepTitle>Revise e abra</JStepTitle>
        <Card style={{ padding: '4px 16px' }}>
          <ReviewRow icon="how_to_vote" label="Tipo" value={type} accent={T.alert} />
          <ReviewRow icon="schedule" label="Prazo" value={deadline} />
          <ReviewRow icon="groups" label="Candidatos" value="3 moradores" />
          <ReviewRow icon="visibility" label="Quem vota" value="Moradores de Camburi" />
        </Card>
      </>}
    </JourneyShell>
  );
}

Object.assign(window, { VoteJourney, GroupBuyJoinJourney, AddProductJourney, WalletReceiveJourney, WalletTopupJourney, JoinEventJourney, CreateElectionJourney });
