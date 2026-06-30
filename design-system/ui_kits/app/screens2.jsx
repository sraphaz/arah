// screens2.jsx — Create Post (with photo), Notifications.

function CreatePostScreen({ territory, onPublish }) {
  const [type, setType] = React.useState('General');
  const [vis, setVis] = React.useState('Public');
  const [title, setTitle] = React.useState('');
  const [body, setBody] = React.useState('');
  const [photo, setPhoto] = React.useState(null);
  const fileRef = React.useRef(null);

  const pickPhoto = (e) => {
    const f = e.target.files?.[0];
    if (f) setPhoto(URL.createObjectURL(f));
  };

  return (
    <div style={{ padding: '0 18px 16px' }}>
      <Card style={{ display: 'flex', alignItems: 'center', gap: 10, padding: '11px 14px', marginBottom: 16, background: T.cardHiGrad }}>
        <Logo size={22} />
        <span style={{ fontFamily: T.sans, fontSize: 13.5, color: T.fg }}>
          Publicando em <strong style={{ color: T.green, fontWeight: 600 }}>{territory.name}</strong>
        </span>
      </Card>

      <Field label="Título">
        <input value={title} onChange={e => setTitle(e.target.value)} placeholder="Dê um título ao seu post" style={inputStyle} />
      </Field>
      <Field label="Conteúdo">
        <textarea value={body} onChange={e => setBody(e.target.value)} rows={4} placeholder="O que você quer compartilhar com a vila?" style={{ ...inputStyle, resize: 'none', lineHeight: 1.5 }} />
      </Field>

      {/* media */}
      <Field label="Mídia">
        <input ref={fileRef} type="file" accept="image/*" onChange={pickPhoto} style={{ display: 'none' }} />
        {photo ? (
          <div style={{ position: 'relative', borderRadius: 14, overflow: 'hidden', border: `1px solid ${T.line}` }}>
            <img src={photo} alt="" style={{ width: '100%', height: 170, objectFit: 'cover', display: 'block' }} />
            <button onClick={() => setPhoto(null)} style={{
              position: 'absolute', top: 10, right: 10, width: 34, height: 34, borderRadius: '50%', border: 'none',
              background: 'rgba(11,12,10,0.7)', backdropFilter: 'blur(6px)', cursor: 'pointer',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
            }}><Icon name="close" size={19} color="#fff" /></button>
          </div>
        ) : (
          <div style={{ display: 'flex', gap: 9 }}>
            <MediaBtn icon="image" label="Foto" onClick={() => fileRef.current?.click()} />
            <MediaBtn icon="photo_camera" label="Câmera" onClick={() => fileRef.current?.click()} />
            <MediaBtn icon="location_on" label="Local" onClick={() => {}} />
          </div>
        )}
      </Field>

      <Field label="Tipo">
        <div style={{ display: 'flex', gap: 9 }}>
          <SegOption active={type === 'General'} onClick={() => setType('General')} icon="chat_bubble" tone="green">Geral</SegOption>
          <SegOption active={type === 'Alert'} onClick={() => setType('Alert')} icon="warning" tone="alert">Alerta</SegOption>
        </div>
      </Field>
      <Field label="Visibilidade">
        <div style={{ display: 'flex', gap: 9 }}>
          <SegOption active={vis === 'Public'} onClick={() => setVis('Public')} icon="public" tone="green">Público</SegOption>
          <SegOption active={vis === 'ResidentsOnly'} onClick={() => setVis('ResidentsOnly')} icon="lock" tone="green">Só moradores</SegOption>
        </div>
      </Field>

      <div style={{ marginTop: 22 }}>
        <Btn kind="primary" full size="lg" icon="send" onClick={() => onPublish({ type, vis, title, body, photo })}>Publicar no território</Btn>
      </div>
    </div>
  );
}

function MediaBtn({ icon, label, onClick }) {
  return (
    <button onClick={onClick} style={{
      flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 7, padding: '16px 8px',
      borderRadius: 14, cursor: 'pointer', WebkitTapHighlightColor: 'transparent',
      background: T.cardFlat, border: `1px dashed ${T.lineHi}`, color: T.fg2,
      fontFamily: T.sans, fontSize: 12.5, fontWeight: 500,
    }}>
      <Icon name={icon} size={22} color={T.green} /> {label}
    </button>
  );
}

function Field({ label, children }) {
  return (
    <div style={{ marginBottom: 16 }}>
      <label style={{ display: 'block', fontFamily: T.sans, fontSize: 12.5, fontWeight: 600, color: T.fg2, marginBottom: 8, letterSpacing: 0.1 }}>{label}</label>
      {children}
    </div>
  );
}
const inputStyle = {
  width: '100%', boxSizing: 'border-box', background: T.cardFlat, color: T.fg,
  border: `1px solid ${T.line}`, borderRadius: 14, padding: '13px 15px',
  fontFamily: T.sans, fontSize: 15, outline: 'none',
};
function SegOption({ children, active, onClick, icon, tone }) {
  const c = tone === 'alert' ? T.alert : T.green;
  const dim = tone === 'alert' ? T.alertDim : T.greenDim;
  return (
    <button onClick={onClick} style={{
      flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 7, padding: '12px',
      borderRadius: 13, cursor: 'pointer', WebkitTapHighlightColor: 'transparent',
      background: active ? dim : T.cardFlat, color: active ? c : T.fg2,
      border: `1px solid ${active ? 'transparent' : T.line}`,
      fontFamily: T.sans, fontSize: 14, fontWeight: 500, transition: 'all .15s',
    }}>
      <Icon name={icon} size={17} fill={active ? 1 : 0} /> {children}
    </button>
  );
}

function NotificationsScreen({ items, onRead, onOpen }) {
  const kindMeta = {
    alert: { icon: 'warning', color: T.alert, dim: T.alertDim },
    event: { icon: 'event', color: T.blue, dim: T.blueDim },
    map: { icon: 'map', color: T.water, dim: 'rgba(111,197,214,0.14)' },
    connection: { icon: 'group', color: T.green, dim: T.greenDim },
    post: { icon: 'article', color: T.green, dim: T.greenDim },
  };
  return (
    <div style={{ padding: '0 18px 12px' }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        {items.map(n => {
          const m = kindMeta[n.kind] || kindMeta.post;
          return (
            <Card key={n.id} onClick={() => { onRead(n.id); onOpen(n.link); }} style={{
              display: 'flex', gap: 13, padding: 14, position: 'relative',
              background: n.read ? 'transparent' : T.cardGrad,
              border: `1px solid ${n.read ? T.line : 'rgba(166,214,185,0.18)'}`,
            }}>
              <div style={{ width: 42, height: 42, borderRadius: 12, flexShrink: 0, background: m.dim, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Icon name={m.icon} size={21} color={m.color} fill={1} />
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ fontFamily: T.display, fontWeight: n.read ? 500 : 600, fontSize: 14.5, color: T.fg, letterSpacing: -0.2, lineHeight: 1.3 }}>{n.title}</div>
                <div style={{ fontFamily: T.sans, fontSize: 13, color: T.fg2, marginTop: 3, lineHeight: 1.45 }}>{n.body}</div>
                <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 6 }}>{n.time}</div>
              </div>
              {!n.read && <div style={{ width: 8, height: 8, borderRadius: '50%', background: T.green, flexShrink: 0, marginTop: 4 }} />}
            </Card>
          );
        })}
      </div>
    </div>
  );
}

Object.assign(window, { CreatePostScreen, NotificationsScreen });
