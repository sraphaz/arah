// screens7.jsx — Minha loja: abrir loja, vender produtos, configuração de pagamento. NEW (proposed).

function StoreScreen() {
  const store = window.ARAH.myStore;
  const [open, setOpen] = React.useState(store.open);
  const [products, setProducts] = React.useState(store.products);
  const [pay, setPay] = React.useState(store.payment);

  if (!open) {
    return (
      <div style={{ padding: '0 18px 16px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 16, padding: '9px 12px', borderRadius: 12, background: T.greenDim }}>
          <Icon name="auto_awesome" size={16} color={T.green} fill={1} />
          <span style={{ fontFamily: T.sans, fontSize: 12, color: T.fg2, lineHeight: 1.4 }}>Novo na Arah — venda no mercado do seu território.</span>
        </div>
        <Card glow style={{ padding: 22, textAlign: 'center', marginBottom: 18 }}>
          <div style={{ width: 64, height: 64, borderRadius: 18, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 14px' }}>
            <Icon name="storefront" size={32} color={T.green} fill={1} />
          </div>
          <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 21, color: T.fg, letterSpacing: -0.4 }}>Abra sua loja</div>
          <p style={{ margin: '8px auto 0', fontFamily: T.sans, fontSize: 14, lineHeight: 1.5, color: T.fg2, maxWidth: 290 }}>
            Venda produtos e serviços direto para a sua comunidade. Sem intermediário, com pagamento via PIX.
          </p>
        </Card>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 11, marginBottom: 22 }}>
          {[['sell', 'Anuncie produtos e serviços', 'Comida, artesanato, passeios, aulas.'],
            ['account_balance_wallet', 'Receba via PIX', 'Sem taxa entre moradores do território.'],
            ['verified_user', 'Confiança local', 'Compradores e vendedores do mesmo território.']].map(([ic, t, s]) => (
            <div key={t} style={{ display: 'flex', gap: 13, alignItems: 'center' }}>
              <div style={{ width: 40, height: 40, borderRadius: 12, background: T.cardHi, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <Icon name={ic} size={20} color={T.green} fill={1} />
              </div>
              <div style={{ flex: 1 }}>
                <div style={{ fontFamily: T.sans, fontSize: 14, fontWeight: 600, color: T.fg }}>{t}</div>
                <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3, marginTop: 1 }}>{s}</div>
              </div>
            </div>
          ))}
        </div>
        <Btn kind="primary" full size="lg" icon="add_business" onClick={() => setOpen(true)}>Abrir minha loja</Btn>
      </div>
    );
  }

  return (
    <div style={{ padding: '0 18px 16px' }}>
      {/* store header */}
      <Card glow style={{ padding: 16, marginBottom: 16, background: T.cardHiGrad }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
          <div style={{ width: 52, height: 52, borderRadius: 15, background: T.greenGrad, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
            <Icon name="storefront" size={26} color="#0C1B10" fill={1} />
          </div>
          <div style={{ flex: 1, minWidth: 0 }}>
            <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 17, color: T.fg, letterSpacing: -0.3 }}>{store.name}</div>
            <div style={{ display: 'inline-flex', alignItems: 'center', gap: 4, marginTop: 3, fontFamily: T.sans, fontSize: 12, color: T.green }}>
              <Icon name="circle" size={9} fill={1} /> Loja ativa em Camburi
            </div>
          </div>
        </div>
        <div style={{ display: 'flex', gap: 10, marginTop: 14 }}>
          <div style={{ flex: 1, background: 'rgba(0,0,0,0.18)', borderRadius: 13, padding: '11px 12px' }}>
            <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 17, color: T.fg }}>{store.sales.month}</div>
            <div style={{ fontFamily: T.sans, fontSize: 11, color: T.fg3, marginTop: 1 }}>Vendas no mês</div>
          </div>
          <div style={{ flex: 1, background: 'rgba(0,0,0,0.18)', borderRadius: 13, padding: '11px 12px' }}>
            <div style={{ fontFamily: T.display, fontWeight: 700, fontSize: 17, color: T.fg }}>{store.sales.orders}</div>
            <div style={{ fontFamily: T.sans, fontSize: 11, color: T.fg3, marginTop: 1 }}>Pedidos</div>
          </div>
        </div>
      </Card>

      {/* payment config */}
      <SectionLabel icon="account_balance_wallet">Pagamento</SectionLabel>
      <Card style={{ overflow: 'hidden', margin: '11px 0 22px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 13, padding: '14px 15px', borderBottom: `1px solid ${T.line}` }}>
          <Icon name="pix" size={20} color={T.water} fill={1} />
          <div style={{ flex: 1 }}>
            <div style={{ fontFamily: T.sans, fontSize: 14, color: T.fg }}>Chave PIX</div>
            <div style={{ fontFamily: T.mono || 'monospace', fontSize: 12, color: T.fg3, marginTop: 1 }}>{pay.pix}</div>
          </div>
          <Icon name="edit" size={18} color={T.fg3} />
        </div>
        <div style={{ padding: '14px 15px', borderBottom: `1px solid ${T.line}` }}>
          <div style={{ fontFamily: T.sans, fontSize: 14, color: T.fg, marginBottom: 9 }}>Formas de pagamento</div>
          <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
            {['PIX', 'Cartão', 'Dinheiro'].map(m => {
              const on = pay.methods.includes(m);
              return (
                <button key={m} onClick={() => setPay(p => ({ ...p, methods: on ? p.methods.filter(x => x !== m) : [...p.methods, m] }))} style={{
                  display: 'inline-flex', alignItems: 'center', gap: 6, padding: '7px 13px', borderRadius: 999, cursor: 'pointer',
                  background: on ? T.greenDim : T.cardFlat, color: on ? T.green : T.fg2,
                  border: `1px solid ${on ? 'transparent' : T.line}`, fontFamily: T.sans, fontSize: 13, fontWeight: 500,
                }}>
                  <Icon name={on ? 'check' : 'add'} size={15} /> {m}
                </button>
              );
            })}
          </div>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 13, padding: '14px 15px', borderBottom: `1px solid ${T.line}` }}>
          <Icon name="percent" size={20} color={T.fg2} />
          <span style={{ flex: 1, fontFamily: T.sans, fontSize: 14, color: T.fg }}>Taxa entre moradores</span>
          <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 14, color: T.green }}>{pay.fee}</span>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 13, padding: '14px 15px' }}>
          <Icon name="account_balance" size={20} color={T.fg2} />
          <span style={{ flex: 1, fontFamily: T.sans, fontSize: 14, color: T.fg }}>Conta de repasse</span>
          <span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3 }}>{pay.payout}</span>
        </div>
      </Card>

      {/* products */}
      <div style={{ display: 'flex', alignItems: 'center', marginBottom: 11 }}>
        <SectionLabel icon="inventory_2">Meus produtos</SectionLabel>
        <button onClick={() => window.openJourney('addProduct', { onConfirm: (prod) => { setProducts(p => [prod, ...p]); window.arahMutate(() => { window.ARAH.myStore.products.unshift(prod); }); } })} style={{ marginLeft: 'auto', display: 'inline-flex', alignItems: 'center', gap: 5, background: 'none', border: 'none', cursor: 'pointer', color: T.green, fontFamily: T.sans, fontSize: 13, fontWeight: 600 }}>
          <Icon name="add" size={17} /> Adicionar
        </button>
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {products.map(p => (
          <Card key={p.id} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: 13 }}>
            <div style={{ width: 44, height: 44, borderRadius: 12, background: T.cardHi, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
              <Icon name={p.icon} size={22} color={T.green} fill={0} />
            </div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 14.5, color: T.fg, letterSpacing: -0.2 }}>{p.title}</div>
              <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 1 }}>{p.tag} · {p.price}</div>
            </div>
            <button onClick={() => setProducts(ps => ps.map(x => x.id === p.id ? { ...x, active: !x.active } : x))} style={{
              width: 46, height: 27, borderRadius: 999, border: 'none', cursor: 'pointer', position: 'relative', flexShrink: 0,
              background: p.active ? T.greenSolid : T.cardHi, transition: 'background .2s',
            }}>
              <span style={{ position: 'absolute', top: 3, left: p.active ? 22 : 3, width: 21, height: 21, borderRadius: '50%', background: '#fff', transition: 'left .2s' }} />
            </button>
          </Card>
        ))}
      </div>
    </div>
  );
}

Object.assign(window, { StoreScreen });
