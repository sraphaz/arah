// screens14.jsx — Journey framework: full-screen multi-step flows + shared field helpers.

// Full-screen journey overlay with progress, header and sticky footer CTA.
function JourneyShell({ title, step, steps, onBack, onClose, footer, children }) {
  const pct = Math.round(((step + 1) / steps) * 100);
  return (
    <div style={{ position: 'absolute', inset: 0, zIndex: 300, background: `radial-gradient(130% 80% at 50% -8%, #15201A 0%, ${T.bg} 58%)`, display: 'flex', flexDirection: 'column', animation: 'sheetUp .32s cubic-bezier(.16,1,.3,1)' }}>
      <div style={{ height: 56, flexShrink: 0 }} />
      {/* header */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '6px 16px 12px' }}>
        <button onClick={step === 0 ? onClose : onBack} style={iconBtn}>
          <Icon name={step === 0 ? 'close' : 'arrow_back'} size={22} color={T.fg} />
        </button>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 17, color: T.fg, letterSpacing: -0.3, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{title}</div>
          <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 1 }}>Passo {step + 1} de {steps}</div>
        </div>
      </div>
      {/* progress */}
      <div style={{ height: 4, background: T.cardHi, margin: '0 16px', borderRadius: 999, overflow: 'hidden', flexShrink: 0 }}>
        <div style={{ width: `${pct}%`, height: '100%', background: T.greenGrad, borderRadius: 999, transition: 'width .35s var(--ease, ease)' }} />
      </div>
      {/* body */}
      <div key={step} className="appscroll screen-fade" style={{ flex: 1, overflowY: 'auto', padding: '20px 18px 16px' }}>{children}</div>
      {/* footer */}
      {footer && <div style={{ padding: '12px 18px 30px', borderTop: `1px solid ${T.line}`, background: T.bg2, flexShrink: 0 }}>{footer}</div>}
    </div>
  );
}

// Success / confirmation state
function SuccessView({ icon = 'check_circle', title, msg, primaryLabel = 'Concluir', onPrimary, secondaryLabel, onSecondary, accent = T.green }) {
  return (
    <div style={{ position: 'absolute', inset: 0, zIndex: 300, background: `radial-gradient(130% 80% at 50% -8%, #15201A 0%, ${T.bg} 58%)`, display: 'flex', flexDirection: 'column', animation: 'fadeIn .3s ease' }}>
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', textAlign: 'center', padding: '0 36px' }}>
        <div style={{ width: 88, height: 88, borderRadius: '50%', background: `${accent}1f`, display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: 22, animation: 'pop .4s cubic-bezier(.16,1.4,.5,1)' }}>
          <Icon name={icon} size={48} color={accent} fill={1} />
        </div>
        <h2 style={{ margin: 0, fontFamily: T.display, fontWeight: 700, fontSize: 24, color: T.fg, letterSpacing: -0.5 }}>{title}</h2>
        <p style={{ margin: '12px 0 0', fontFamily: T.sans, fontSize: 15, lineHeight: 1.55, color: T.fg2, maxWidth: 300 }}>{msg}</p>
      </div>
      <div style={{ padding: '12px 18px 32px', display: 'flex', flexDirection: 'column', gap: 10 }}>
        <Btn kind="primary" full size="lg" icon="check" onClick={onPrimary}>{primaryLabel}</Btn>
        {secondaryLabel && <Btn kind="secondary" full size="md" onClick={onSecondary}>{secondaryLabel}</Btn>}
      </div>
    </div>
  );
}

// ---- shared field helpers ----
function JField({ label, hint, children }) {
  return (
    <div style={{ marginBottom: 18 }}>
      {label && <div style={{ fontFamily: T.sans, fontSize: 12.5, fontWeight: 600, color: T.fg2, marginBottom: 9, letterSpacing: 0.1 }}>{label}</div>}
      {children}
      {hint && <div style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3, marginTop: 7 }}>{hint}</div>}
    </div>
  );
}

const jInput = {
  width: '100%', boxSizing: 'border-box', background: T.cardFlat, color: T.fg,
  border: `1px solid ${T.line}`, borderRadius: 14, padding: '13px 15px',
  fontFamily: T.sans, fontSize: 15, outline: 'none',
};
window.jInput = jInput;

// horizontal selectable pills
function PillSelect({ options, value, onChange, cols }) {
  return (
    <div style={{ display: cols ? 'grid' : 'flex', gridTemplateColumns: cols ? `repeat(${cols},1fr)` : undefined, gap: 8, flexWrap: 'wrap' }}>
      {options.map(o => {
        const v = typeof o === 'string' ? o : o.value;
        const lbl = typeof o === 'string' ? o : o.label;
        const on = value === v;
        return (
          <button key={v} onClick={() => onChange(v)} style={{
            padding: '11px 14px', borderRadius: 13, cursor: 'pointer', WebkitTapHighlightColor: 'transparent',
            background: on ? T.greenDim : T.cardFlat, color: on ? T.green : T.fg2,
            border: `1px solid ${on ? 'transparent' : T.line}`, fontFamily: T.sans, fontSize: 14, fontWeight: 500,
            textAlign: 'center', transition: 'all .15s',
          }}>{lbl}</button>
        );
      })}
    </div>
  );
}

// − N + counter
function Counter({ value, onChange, min = 0, max = 20, suffix = '' }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 16, background: T.cardFlat, border: `1px solid ${T.line}`, borderRadius: 14, padding: '8px 14px', width: 'fit-content' }}>
      <button onClick={() => onChange(Math.max(min, value - 1))} style={counterBtn}><Icon name="remove" size={20} color={T.fg} /></button>
      <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: 17, color: T.fg, minWidth: 28, textAlign: 'center' }}>{value}{suffix}</span>
      <button onClick={() => onChange(Math.min(max, value + 1))} style={counterBtn}><Icon name="add" size={20} color={T.fg} /></button>
    </div>
  );
}
const counterBtn = { width: 34, height: 34, borderRadius: 10, border: `1px solid ${T.line}`, background: T.cardHi, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', WebkitTapHighlightColor: 'transparent' };

// review summary rows
function ReviewRow({ icon, label, value, accent }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '13px 0', borderBottom: `1px solid ${T.line}` }}>
      {icon && <Icon name={icon} size={19} color={T.fg3} />}
      <span style={{ flex: 1, fontFamily: T.sans, fontSize: 14, color: T.fg2 }}>{label}</span>
      <span style={{ fontFamily: T.sans, fontSize: 14, fontWeight: 600, color: accent || T.fg, textAlign: 'right' }}>{value}</span>
    </div>
  );
}

// PIX payment block
function PixPay({ amount }) {
  const [copied, setCopied] = React.useState(false);
  return (
    <div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 10, padding: '13px 15px', borderRadius: 14, background: 'rgba(111,197,214,0.1)', border: '1px solid rgba(111,197,214,0.22)', marginBottom: 16 }}>
        <Icon name="pix" size={22} color={T.water} fill={1} />
        <div style={{ flex: 1 }}>
          <div style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, color: T.fg }}>Pague com PIX</div>
          <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3 }}>Aprovação na hora · sem taxa entre moradores</div>
        </div>
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', padding: '22px', borderRadius: 16, background: '#fff', marginBottom: 14 }}>
        <Icon name="qr_code_2" size={132} color="#0E2117" fill={0} />
      </div>
      <button onClick={() => { setCopied(true); setTimeout(() => setCopied(false), 1600); }} style={{
        width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 8, padding: '13px',
        borderRadius: 14, cursor: 'pointer', background: T.cardFlat, border: `1px solid ${T.line}`,
        color: copied ? T.green : T.fg, fontFamily: T.sans, fontSize: 14, fontWeight: 600, WebkitTapHighlightColor: 'transparent',
      }}>
        <Icon name={copied ? 'check' : 'content_copy'} size={17} /> {copied ? 'Código PIX copiado' : 'Copiar código PIX'}
      </button>
      <div style={{ textAlign: 'center', marginTop: 14, fontFamily: T.display, fontWeight: 700, fontSize: 22, color: T.fg }}>{amount}</div>
    </div>
  );
}

function JStepTitle({ children, sub }) {
  return (
    <div style={{ marginBottom: 18 }}>
      <h2 style={{ margin: 0, fontFamily: T.display, fontWeight: 700, fontSize: 21, color: T.fg, letterSpacing: -0.4 }}>{children}</h2>
      {sub && <p style={{ margin: '7px 0 0', fontFamily: T.sans, fontSize: 14, lineHeight: 1.5, color: T.fg2 }}>{sub}</p>}
    </div>
  );
}

Object.assign(window, { JourneyShell, SuccessView, JField, PillSelect, Counter, ReviewRow, PixPay, JStepTitle });
