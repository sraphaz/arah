// screens6.jsx — Eleições/Votação (NEW, visual) + Gestão do território (curador journey).

function ElectionsScreen({ role }) {
  const [choice, setChoice] = React.useState({}); // selected candidate per election (not yet final)
  const [voted, setVoted] = React.useState({});   // confirmed votes per election
  const elections = [
    {
      id: 'el1', title: 'Conselho de Moradores 2026', status: 'open', ends: 'encerra em 3 dias', voters: 412,
      desc: 'Escolha 1 representante para o conselho do território.',
      candidates: [
        { id: 'c1', name: 'Dona Marta', avatar: '#C9962B', role: 'morador', pitch: 'Pesca artesanal e feira', pct: 38 },
        { id: 'c2', name: 'Seu Tião', avatar: '#4F956F', role: 'morador', pitch: 'Saneamento e águas', pct: 34 },
        { id: 'c3', name: 'Bruno Caiçara', avatar: '#377B57', role: 'morador', pitch: 'Trilhas e turismo de base', pct: 28 },
      ],
    },
    {
      id: 'el2', title: 'Representante de águas', status: 'closed', ends: 'encerrada', voters: 388,
      desc: 'Resultado final da votação.',
      winner: 'Coletivo Maré',
      candidates: [
        { id: 'c4', name: 'Coletivo Maré', avatar: '#2A6FDB', role: 'morador', pitch: 'Captação da nascente', pct: 61, won: true },
        { id: 'c5', name: 'Assoc. de Pescadores', avatar: '#4F956F', role: 'morador', pitch: 'Poços comunitários', pct: 39 },
      ],
    },
  ];
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 14, padding: '9px 12px', borderRadius: 12, background: T.alertDim }}>
        <Icon name="auto_awesome" size={16} color={T.alert} fill={1} />
        <span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg2, lineHeight: 1.4 }}>Nova feature — governança comunitária do território.</span>
      </div>
      {role === 'visitante' && (
        <Card style={{ padding: 13, marginBottom: 14, display: 'flex', alignItems: 'center', gap: 10, background: T.cardHiGrad }}>
          <Icon name="lock" size={18} color={T.fg3} />
          <span style={{ flex: 1, fontFamily: T.sans, fontSize: 12.5, color: T.fg2 }}>Só moradores votam. Confirme residência para participar.</span>
        </Card>
      )}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
        {elections.map(el => {
          const myVote = choice[el.id];
          const hasVoted = !!voted[el.id];
          const showResults = el.status === 'closed' || hasVoted;
          const canVote = role !== 'visitante' && el.status === 'open';
          return (
            <Card key={el.id} glow style={{ padding: 16 }}>
              <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10, marginBottom: 4 }}>
                <div style={{ flex: 1 }}>
                  <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16.5, color: T.fg, letterSpacing: -0.3 }}>{el.title}</div>
                  <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2, marginTop: 3 }}>{el.desc}</div>
                </div>
                <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4, padding: '4px 10px', borderRadius: 999, flexShrink: 0,
                  background: el.status === 'open' ? T.greenDim : 'rgba(166,172,158,0.12)', color: el.status === 'open' ? T.green : T.fg3,
                  fontFamily: T.sans, fontSize: 11, fontWeight: 600 }}>
                  <Icon name={el.status === 'open' ? 'how_to_vote' : 'lock'} size={13} fill={1} /> {el.status === 'open' ? 'Aberta' : 'Encerrada'}
                </span>
              </div>
              <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginBottom: 13 }}>{el.ends} · {el.voters} votos</div>

              <div style={{ display: 'flex', flexDirection: 'column', gap: 9 }}>
                {el.candidates.map(c => {
                  const selected = myVote === c.id;
                  return (
                    <button key={c.id} disabled={!canVote || hasVoted} onClick={() => canVote && !hasVoted && setChoice(v => ({ ...v, [el.id]: c.id }))} style={{
                      position: 'relative', display: 'flex', alignItems: 'center', gap: 11, padding: 11, textAlign: 'left',
                      borderRadius: 13, cursor: canVote ? 'pointer' : 'default', WebkitTapHighlightColor: 'transparent', overflow: 'hidden',
                      background: T.cardFlat, border: `1px solid ${selected || c.won ? (c.won ? T.green : 'rgba(166,214,185,0.4)') : T.line}`,
                    }}>
                      {showResults && <div style={{ position: 'absolute', inset: 0, width: `${c.pct}%`, background: c.won ? 'rgba(166,214,185,0.16)' : 'rgba(166,214,185,0.08)' }} />}
                      <Avatar color={c.avatar} name={c.name} size={38} resident style={{ position: 'relative' }} />
                      <div style={{ flex: 1, minWidth: 0, position: 'relative' }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                          <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14, color: T.fg }}>{c.name}</span>
                          {c.won && <Icon name="emoji_events" size={15} color={T.green} fill={1} />}
                        </div>
                        <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 1 }}>{c.pitch}</div>
                      </div>
                      <div style={{ position: 'relative', display: 'flex', alignItems: 'center', gap: 8 }}>
                        {showResults && <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 15, color: c.won ? T.green : T.fg2 }}>{c.pct}%</span>}
                        {canVote && <Icon name={selected ? 'radio_button_checked' : 'radio_button_unchecked'} size={22} color={selected ? T.green : T.fg3} fill={selected ? 1 : 0} />}
                      </div>
                    </button>
                  );
                })}
              </div>

              {canVote && (
                <Btn kind="primary" full size="md" icon={hasVoted ? 'check_circle' : 'how_to_vote'} style={{ marginTop: 13, opacity: hasVoted || myVote ? 1 : 0.5 }}
                  onClick={() => { if (hasVoted || !myVote) return; const cand = el.candidates.find(x => x.id === myVote); window.openJourney('vote', { election: el, candidate: cand, onConfirm: () => setVoted(v => ({ ...v, [el.id]: true })) }); }}>{hasVoted ? 'Voto registrado' : myVote ? 'Confirmar voto' : 'Escolha um candidato'}</Btn>
              )}
            </Card>
          );
        })}
      </div>
    </div>
  );
}

// Território management — curador journey
function ManageScreen({ territory }) {
  const [done, setDone] = React.useState({});
  const queue = [
    { id: 'q1', kind: 'Entidade', icon: 'landscape', label: 'Mirante da Pedra', sub: 'Sugerido por Bruno · validar local', color: T.green },
    { id: 'q2', kind: 'Post', icon: 'flag', label: 'Post sinalizado', sub: 'Possível spam · revisar', color: T.alert },
    { id: 'q3', kind: 'Membro', icon: 'how_to_reg', label: 'Confirmar residência', sub: 'Ana Ribeiro · comprovante enviado', color: T.blue },
  ];
  const stats = [
    { label: 'Moradores', val: '1.284', icon: 'groups' },
    { label: 'Na curadoria', val: '3', icon: 'pending_actions' },
    { label: 'Entidades', val: '6', icon: 'location_on' },
  ];
  return (
    <div style={{ padding: '0 18px 16px' }}>
      <Card style={{ display: 'flex', alignItems: 'center', gap: 11, padding: 14, marginBottom: 16, background: T.cardHiGrad }}>
        <div style={{ width: 40, height: 40, borderRadius: 12, background: T.alertDim, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Icon name="shield_person" size={21} color={T.alert} fill={1} />
        </div>
        <div style={{ flex: 1 }}>
          <div style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, color: T.fg }}>Você é curador de {territory.name}</div>
          <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 1 }}>Cuide do feed, do mapa e da comunidade.</div>
        </div>
      </Card>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 10, marginBottom: 20 }}>
        {stats.map(s => (
          <Card key={s.label} style={{ padding: '14px 8px', textAlign: 'center' }}>
            <Icon name={s.icon} size={20} color={T.green} fill={1} />
            <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 19, color: T.fg, marginTop: 6 }}>{s.val}</div>
            <div style={{ fontFamily: T.sans, fontSize: 10.5, color: T.fg3, marginTop: 1 }}>{s.label}</div>
          </Card>
        ))}
      </div>

      <SectionLabel icon="rule">Fila de curadoria</SectionLabel>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10, marginTop: 11, marginBottom: 20 }}>
        {queue.filter(q => !done[q.id]).map(q => (
          <Card key={q.id} style={{ padding: 14 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 11, marginBottom: 12 }}>
              <div style={{ width: 38, height: 38, borderRadius: 11, background: `${q.color}1f`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <Icon name={q.icon} size={20} color={q.color} fill={1} />
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <span style={{ fontFamily: T.sans, fontSize: 10, fontWeight: 600, letterSpacing: 0.6, textTransform: 'uppercase', color: q.color }}>{q.kind}</span>
                </div>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14.5, color: T.fg, marginTop: 1 }}>{q.label}</div>
                <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 1 }}>{q.sub}</div>
              </div>
            </div>
            <div style={{ display: 'flex', gap: 9 }}>
              <Btn kind="primary" size="sm" icon="check" style={{ flex: 1 }} onClick={() => setDone(d => ({ ...d, [q.id]: true }))}>Aprovar</Btn>
              <Btn kind="dark" size="sm" icon="close" style={{ flex: 1, color: T.alert }} onClick={() => setDone(d => ({ ...d, [q.id]: true }))}>Recusar</Btn>
            </div>
          </Card>
        ))}
        {queue.every(q => done[q.id]) && (
          <Card style={{ padding: 22, textAlign: 'center' }}>
            <Icon name="task_alt" size={28} color={T.green} fill={1} />
            <div style={{ fontFamily: T.sans, fontSize: 13.5, color: T.fg2, marginTop: 8 }}>Fila de curadoria zerada. Bom trabalho!</div>
          </Card>
        )}
      </div>

      <SectionLabel icon="how_to_vote">Eleições do território</SectionLabel>
      <Card onClick={() => window.openJourney('createElection')} style={{ padding: 14, marginTop: 11, display: 'flex', alignItems: 'center', gap: 11 }}>
        <div style={{ width: 38, height: 38, borderRadius: 11, background: T.alertDim, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Icon name="add" size={20} color={T.alert} />
        </div>
        <div style={{ flex: 1 }}>
          <div style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, color: T.fg }}>Abrir nova eleição</div>
          <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>Conselho · representantes · consultas</div>
        </div>
        <Icon name="chevron_right" size={20} color={T.fg3} />
      </Card>
    </div>
  );
}

Object.assign(window, { ElectionsScreen, ManageScreen });
