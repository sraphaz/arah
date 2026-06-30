// screens4.jsx — Map (geolocation + entities + health), Login, Onboarding, Territory sheet.

// Slippy-tile math: lat/lng → tile x/y at zoom z
function lngLatToTile(lng, lat, z) {
  const n = Math.pow(2, z);
  const x = Math.floor((lng + 180) / 360 * n);
  const latRad = lat * Math.PI / 180;
  const y = Math.floor((1 - Math.log(Math.tan(latRad) + 1 / Math.cos(latRad)) / Math.PI) / 2 * n);
  return { x, y };
}

// Dark-styled OpenStreetMap tile mosaic. coords (geo) override tile; else tile prop; else Camburi.
function MapCanvas({ height = 220, coords = null, tile = null, mini = false, entities = [], onPin }) {
  const z = 13;
  const center = coords ? lngLatToTile(coords.lng, coords.lat, z) : (tile || { x: 3057, y: 4652 });
  const tiles = [];
  for (let dy = -1; dy <= 1; dy++)
    for (let dx = -1; dx <= 1; dx++)
      tiles.push({ url: `https://tile.openstreetmap.org/${z}/${center.x + dx}/${center.y + dy}.png` });
  return (
    <div style={{ position: 'relative', height, overflow: 'hidden', background: '#0a120d' }}>
      <div style={{
        position: 'absolute', width: 768, height: 768, left: '50%', top: '50%', transform: 'translate(-50%, -50%)',
        display: 'grid', gridTemplateColumns: 'repeat(3, 256px)', gridTemplateRows: 'repeat(3, 256px)',
        filter: 'invert(0.91) hue-rotate(165deg) saturate(0.55) brightness(0.92) contrast(0.98)',
      }}>
        {tiles.map((t, i) => <img key={i} src={t.url} width={256} height={256} alt="" loading="lazy" style={{ display: 'block' }} crossOrigin="anonymous" />)}
      </div>
      <div style={{ position: 'absolute', inset: 0, background: 'radial-gradient(120% 100% at 50% 50%, transparent 40%, rgba(11,18,13,0.55))', mixBlendMode: 'multiply' }} />
      {/* territory boundary */}
      <div style={{ position: 'absolute', left: '50%', top: '50%', transform: 'translate(-50%,-50%)',
        width: height * 0.7, height: height * 0.7, borderRadius: '50%',
        border: `2px solid ${T.green}`, background: 'rgba(166,214,185,0.10)' }} />
      {/* user position */}
      <div style={{ position: 'absolute', left: '50%', top: '50%', transform: 'translate(-50%,-50%)', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <div style={{ width: 16, height: 16, borderRadius: '50%', background: T.alert, border: '3px solid #fff', boxShadow: '0 0 0 6px rgba(232,160,106,0.25)' }} />
      </div>
      {/* entity pins */}
      {!mini && entities.map(e => (
        <button key={e.id} onClick={() => onPin && onPin(e)} style={{
          position: 'absolute', left: `${e.x}%`, top: `${e.y}%`, transform: 'translate(-50%,-100%)',
          background: 'none', border: 'none', cursor: 'pointer', padding: 0, WebkitTapHighlightColor: 'transparent',
        }}>
          <div style={{ width: 34, height: 34, borderRadius: '50% 50% 50% 2px', transform: 'rotate(45deg)',
            background: 'rgba(16,20,16,0.85)', backdropFilter: 'blur(4px)', border: `1.5px solid ${e.color}`,
            display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 6px 14px rgba(0,0,0,0.4)' }}>
            <Icon name={e.icon} size={16} color={e.color} fill={1} style={{ transform: 'rotate(-45deg)' }} />
          </div>
        </button>
      ))}
    </div>
  );
}

function MapScreen({ territory, content, focusId }) {
  const ents = content.entities;
  const [coords, setCoords] = React.useState(null);
  const [geoState, setGeoState] = React.useState('idle'); // idle|loading|ok|denied
  const [sel, setSel] = React.useState(focusId ? ents.find(e => e.id === focusId) : null);

  const locate = () => {
    setGeoState('loading');
    if (!navigator.geolocation) { setGeoState('denied'); return; }
    navigator.geolocation.getCurrentPosition(
      pos => { setCoords({ lat: pos.coords.latitude, lng: pos.coords.longitude }); setGeoState('ok'); },
      () => setGeoState('denied'),
      { timeout: 8000 }
    );
  };

  return (
    <div style={{ position: 'relative', height: '100%' }}>
      <MapCanvas height={760} coords={coords} tile={content.tile} entities={ents} onPin={setSel} />

      {/* locate button */}
      <button onClick={locate} style={{
        position: 'absolute', top: 14, right: 16, width: 44, height: 44, borderRadius: 14, cursor: 'pointer',
        background: T.glassGrad, backdropFilter: 'blur(14px)', WebkitBackdropFilter: 'blur(14px)',
        border: `1px solid ${T.lineHi}`, display: 'flex', alignItems: 'center', justifyContent: 'center',
      }}>
        <Icon name={geoState === 'loading' ? 'sync' : 'my_location'} size={22} color={geoState === 'ok' ? T.green : T.fg} fill={geoState === 'ok' ? 1 : 0}
          style={geoState === 'loading' ? { animation: 'spin 1s linear infinite' } : {}} />
      </button>
      {geoState !== 'idle' && (
        <div style={{ position: 'absolute', top: 16, left: 16, padding: '8px 13px', borderRadius: 999,
          background: T.glassGrad, backdropFilter: 'blur(14px)', border: `1px solid ${T.lineHi}`,
          fontFamily: T.sans, fontSize: 12.5, color: T.fg, display: 'flex', alignItems: 'center', gap: 6 }}>
          <Icon name="place" size={15} color={T.alert} fill={1} />
          {geoState === 'loading' ? 'Localizando…' : geoState === 'denied' ? 'Mostrando Camburi' : coords ? `${coords.lat.toFixed(3)}, ${coords.lng.toFixed(3)}` : ''}
        </div>
      )}

      {/* bottom: entity detail OR health summary */}
      {sel ? (
        <div style={{ position: 'absolute', left: 16, right: 16, bottom: 16, padding: 16,
          background: T.glassGrad, backdropFilter: 'blur(16px)', WebkitBackdropFilter: 'blur(16px)',
          borderRadius: 20, border: `1px solid ${T.lineHi}`, boxShadow: '0 12px 30px rgba(0,0,0,0.45)' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            <div style={{ width: 44, height: 44, borderRadius: 13, background: `${sel.color}22`, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <Icon name={sel.icon} size={23} color={sel.color} fill={1} />
            </div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
                <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg }}>{sel.label}</span>
                {sel.confirmed
                  ? <Icon name="verified" size={16} color={T.green} fill={1} />
                  : <span style={{ fontFamily: T.sans, fontSize: 10.5, color: T.alert, background: T.alertDim, padding: '2px 7px', borderRadius: 999, fontWeight: 600 }}>em curadoria</span>}
              </div>
              <div style={{ fontFamily: T.sans, fontSize: 13, color: T.fg2, marginTop: 2 }}>{sel.sub}</div>
            </div>
            <button onClick={() => setSel(null)} style={{ ...iconBtn, width: 34, height: 34 }}><Icon name="close" size={18} color={T.fg2} /></button>
          </div>
          <div style={{ display: 'flex', gap: 9, marginTop: 13 }}>
            <Btn kind="dark" size="sm" icon="directions" style={{ flex: 1 }}>Rotas</Btn>
            <Btn kind="dark" size="sm" icon={sel.confirmed ? 'thumb_up' : 'how_to_reg'} style={{ flex: 1 }}>{sel.confirmed ? 'Confirmar' : 'Validar'}</Btn>
          </div>
        </div>
      ) : (
        <div style={{ position: 'absolute', left: 16, right: 16, bottom: 16, padding: 16,
          background: T.glassGrad, backdropFilter: 'blur(16px)', WebkitBackdropFilter: 'blur(16px)',
          borderRadius: 20, border: `1px solid ${T.lineHi}`, boxShadow: '0 12px 30px rgba(0,0,0,0.45)' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 11 }}>
            <Logo size={18} mark="png" />
            <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{territory.name}</span>
            <span style={{ marginLeft: 'auto', fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>{ents.length} pontos</span>
          </div>
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 12 }}>
            {[['storefront', 'Lugares', T.green], ['water', 'Nascentes', T.water], ['landscape', 'Mirantes', T.green], ['event', 'Eventos', T.blue], ['warning', 'Alertas', T.alert]].map(([ic, lb, c]) => (
              <div key={lb} style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <Icon name={ic} size={16} color={c} fill={1} /><span style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2 }}>{lb}</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

function LoginScreen({ onLogin }) {
  return (
    <div style={{ height: '100%', display: 'flex', flexDirection: 'column', padding: '0 26px' }}>
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
        <div style={{ width: 92, marginBottom: 22 }}><Logo size={92} mark="png" /></div>
        <Eyebrow>Território-primeiro</Eyebrow>
        <h1 style={{ margin: '10px 0 0', fontFamily: T.display, fontWeight: 700, fontSize: 44, lineHeight: 1.02, color: T.fg, letterSpacing: -1.2 }}>Arah</h1>
        <p style={{ margin: '14px 0 0', fontFamily: T.sans, fontSize: 16, lineHeight: 1.5, color: T.fg2, maxWidth: 300 }}>
          A vida da sua comunidade, organizada pelo território. Conecte-se com quem está perto.
        </p>
      </div>
      <div style={{ paddingBottom: 40, display: 'flex', flexDirection: 'column', gap: 11 }}>
        <Btn kind="primary" full size="lg" icon="login" onClick={onLogin}>Entrar com Google</Btn>
        <Btn kind="secondary" full size="lg" icon="mail" onClick={onLogin}>Entrar com e-mail</Btn>
        <div style={{ textAlign: 'center', marginTop: 8, fontFamily: T.sans, fontSize: 13, color: T.fg3 }}>
          Não tem conta? <span style={{ color: T.green, fontWeight: 600 }}>Criar conta</span>
        </div>
      </div>
    </div>
  );
}

function OnboardingScreen({ onContinue }) {
  const [sel, setSel] = React.useState('t1');
  const [coords, setCoords] = React.useState(null);
  const [geo, setGeo] = React.useState('idle');
  const list = window.ARAH.territories;
  const selT = list.find(t => t.id === sel);

  React.useEffect(() => {
    if (!navigator.geolocation) { setGeo('denied'); return; }
    setGeo('loading');
    navigator.geolocation.getCurrentPosition(
      pos => { setCoords({ lat: pos.coords.latitude, lng: pos.coords.longitude }); setGeo('ok'); },
      () => setGeo('denied'), { timeout: 8000 }
    );
  }, []);

  return (
    <div style={{ padding: '0 18px 16px' }}>
      <h1 style={{ margin: '4px 0 6px', fontFamily: T.display, fontWeight: 700, fontSize: 25, color: T.fg, letterSpacing: -0.5 }}>Escolha seu território</h1>
      <p style={{ margin: '0 0 16px', fontFamily: T.sans, fontSize: 14, lineHeight: 1.5, color: T.fg2 }}>Para ver o feed e participar, escolha um território próximo a você.</p>
      <Card style={{ display: 'flex', alignItems: 'center', gap: 11, padding: '12px 14px', marginBottom: 14 }}>
        <Icon name={geo === 'ok' ? 'my_location' : geo === 'loading' ? 'sync' : 'location_off'} size={20} color={geo === 'denied' ? T.fg3 : T.green} fill={geo === 'ok' ? 1 : 0}
          style={geo === 'loading' ? { animation: 'spin 1s linear infinite' } : {}} />
        <div style={{ flex: 1 }}>
          <div style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, color: T.fg }}>
            {geo === 'ok' ? 'Localização ativa' : geo === 'loading' ? 'Buscando localização…' : 'Localização indisponível'}
          </div>
          <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 1 }}>
            {geo === 'ok' && coords ? `${coords.lat.toFixed(3)}, ${coords.lng.toFixed(3)} · privada` : 'Privada — não fica visível para outros.'}
          </div>
        </div>
      </Card>
      <div style={{ borderRadius: 18, overflow: 'hidden', border: `1px solid ${T.line}`, marginBottom: 16 }}>
        <MapCanvas height={180} coords={coords} tile={window.ARAH.content.t1.tile} entities={window.ARAH.content.t1.entities.slice(0, 3)} />
      </div>
      <Eyebrow style={{ marginBottom: 10 }}>Próximos a você</Eyebrow>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 9, marginBottom: 18 }}>
        {list.slice(0, 3).map(t => {
          const on = t.id === sel;
          return (
            <Card key={t.id} onClick={() => setSel(t.id)} hi={on} style={{ display: 'flex', alignItems: 'center', gap: 12, padding: 13, border: `1.5px solid ${on ? T.green : T.line}` }}>
              <Icon name="place" size={22} color={on ? T.green : T.fg3} fill={on ? 1 : 0} />
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{t.name}</div>
                <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 1 }}>{t.distance} km de distância</div>
              </div>
              <Icon name={on ? 'check_circle' : 'radio_button_unchecked'} size={21} color={on ? T.green : T.fg3} fill={on ? 1 : 0} />
            </Card>
          );
        })}
      </div>
      <p style={{ margin: '0 0 12px', fontFamily: T.sans, fontSize: 12, color: T.fg3, lineHeight: 1.5 }}>Ao continuar, você entrará como visitante deste território.</p>
      <Btn kind="primary" full size="lg" icon="arrow_forward" onClick={() => onContinue(sel)}>Continuar com {selT.name}</Btn>
    </div>
  );
}

function TerritorySheet({ activeId, onPick, onClose }) {
  return (
    <Sheet onClose={onClose} title="Trocar de território" subtitle="Você participa como morador ou visitante.">
      <div style={{ display: 'flex', flexDirection: 'column', gap: 9 }}>
        {window.ARAH.territories.map(t => {
          const on = t.id === activeId;
          return (
            <Card key={t.id} onClick={() => onPick(t.id)} hi={on} style={{ display: 'flex', alignItems: 'center', gap: 12, padding: 13, border: `1px solid ${on ? 'rgba(166,214,185,0.35)' : T.line}` }}>
              <div style={{ width: 40, height: 40, borderRadius: 12, flexShrink: 0, background: on ? T.greenGrad : T.cardHi, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Icon name="forest" size={20} color={on ? '#0C1B10' : T.green} fill={1} />
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg }}>{t.name}</div>
                <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>{t.region}</div>
              </div>
              {on && <Icon name="check_circle" size={21} color={T.green} fill={1} />}
            </Card>
          );
        })}
      </div>
    </Sheet>
  );
}

// Reusable bottom sheet
function Sheet({ children, title, subtitle, onClose }) {
  return (
    <div style={{ position: 'absolute', inset: 0, zIndex: 100 }}>
      <div onClick={onClose} style={{ position: 'absolute', inset: 0, background: 'rgba(0,0,0,0.6)', backdropFilter: 'blur(2px)', animation: 'fadeIn .25s ease' }} />
      <div style={{ position: 'absolute', left: 0, right: 0, bottom: 0, background: 'linear-gradient(180deg, #1A1D17, #131510)',
        borderRadius: '26px 26px 0 0', padding: '12px 18px 34px', maxHeight: '80%', overflow: 'auto',
        border: `1px solid ${T.lineHi}`, borderBottom: 'none', animation: 'sheetUp .32s cubic-bezier(.16,1,.3,1)' }}>
        <div style={{ width: 38, height: 4, borderRadius: 999, background: T.lineHi, margin: '0 auto 16px' }} />
        {title && <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 19, color: T.fg, marginBottom: 4, letterSpacing: -0.3 }}>{title}</div>}
        {subtitle && <div style={{ fontFamily: T.sans, fontSize: 13, color: T.fg3, marginBottom: 16 }}>{subtitle}</div>}
        {children}
      </div>
    </div>
  );
}

Object.assign(window, { MapCanvas, MapScreen, LoginScreen, OnboardingScreen, TerritorySheet, Sheet });
