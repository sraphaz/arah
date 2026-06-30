// components.jsx — Arah shared UI primitives. Premium minimalist, dark-default.
// v2: depth via subtle gradients, real araponga logo, richer surfaces.
// Material Symbols Rounded icons (the app's native icon system is Material Icons).

const T = {
  bg: '#0B0C0A',          // deepest base
  bg2: '#0E100D',         // app surface
  card: '#16181400',      // (placeholder)
  // gradient surfaces (depth)
  cardGrad: 'linear-gradient(160deg, #1C201A 0%, #161814 100%)',
  cardHiGrad: 'linear-gradient(160deg, #242821 0%, #1C201A 100%)',
  cardFlat: '#191B17',
  cardHi: '#20231E',
  glassGrad: 'linear-gradient(160deg, rgba(32,36,30,0.92), rgba(20,23,18,0.88))',
  line: 'rgba(255,255,255,0.07)',
  lineHi: 'rgba(255,255,255,0.13)',
  fg: '#F2F4EE',
  fg2: '#A6AC9E',         // muted
  fg3: '#6B7164',         // subtle
  green: '#A6D6B9',       // canopy accent (light, for dark UI)
  greenSolid: '#81C784',
  greenGrad: 'linear-gradient(150deg, #93D29C 0%, #6FB98C 100%)', // primary button depth
  greenDeep: '#2B6246',
  greenDim: 'rgba(129,199,132,0.13)',
  greenGlow: '0 8px 24px rgba(129,199,132,0.30)',
  // elevated depth shadows (modern, layered)
  shadowSoft: '0 1px 0 rgba(255,255,255,0.05) inset, 0 8px 22px rgba(0,0,0,0.30)',
  shadowCard: '0 1px 0 rgba(255,255,255,0.05) inset, 0 14px 34px rgba(0,0,0,0.40)',
  shadowFloat: '0 18px 44px rgba(0,0,0,0.50)',
  alert: '#E8A06A',       // warm terracotta-amber
  alertDim: 'rgba(232,160,106,0.14)',
  blue: '#86AEEA',
  blueDim: 'rgba(134,174,234,0.14)',
  water: '#6FC5D6',       // território health: água
  display: '"Sora", system-ui, sans-serif',
  sans: '"Geist", system-ui, sans-serif',
};
window.T = T;

// Material Symbols Rounded icon
function Icon({ name, size = 24, color = 'currentColor', fill = 0, weight = 400, grade = 0, style = {} }) {
  return (
    <span className="msr" style={{
      fontSize: size, color, lineHeight: 1, userSelect: 'none',
      fontVariationSettings: `'FILL' ${fill}, 'wght' ${weight}, 'GRAD' ${grade}, 'opsz' 24`,
      display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
      flexShrink: 0, ...style,
    }}>{name}</span>
  );
}

// Premium brand mark — leaf-wing fusion (araponga in flight + território vivo).
// tone: 'canopy' (gradient) | 'ink' | 'white' (flat, for mono use).
let _logoSeq = 0;
function LogoMark({ size = 32, variant = 'wingleaf', tone = 'canopy' }) {
  const uid = React.useMemo(() => 'lg' + (++_logoSeq), []);
  const flat = tone === 'ink' ? '#0E1F12' : tone === 'white' ? '#F2F4EE' : null;
  const light = flat || '#A6D6B9';
  const deep = flat || '#5FB07F';
  const solid = flat || '#81C784';
  const bodyFill = flat || `url(#${uid}b)`;
  const barbCol = tone === 'ink' ? '#A6D6B9' : tone === 'white' ? '#1A2D20' : '#0E2117';
  const paths = {
    // leaf-wing fusion — a tilted leaf whose central vein sprouts feather barbs
    wingleaf: (<>
      {!flat && (
        <defs>
          <linearGradient id={uid + 'b'} x1="0.1" y1="0.05" x2="0.85" y2="0.95">
            <stop offset="0" stopColor="#D2EFDD" />
            <stop offset="0.5" stopColor="#93D2A8" />
            <stop offset="1" stopColor="#56A877" />
          </linearGradient>
        </defs>
      )}
      {/* underwing — depth */}
      <path d="M11.5 40.5 C6.5 26 16 11.5 40.5 7.5 C33 23 26 35.5 11.5 40.5 Z" fill={deep} opacity={flat ? 0.45 : 0.9} />
      {/* main leaf-wing */}
      <path d="M14.5 37.8 C12.4 27 19 16.3 35.4 11.2 C29.6 23 22.6 32.6 14.5 37.8 Z" fill={bodyFill} />
      {/* leading-edge sheen */}
      {!flat && <path d="M35.4 11.2 C24.5 14.5 17.8 21.5 14.9 30.5" stroke="rgba(255,255,255,0.55)" strokeWidth="1.1" strokeLinecap="round" fill="none" opacity="0.7" />}
      {/* feather barbs / veins */}
      <g stroke={barbCol} strokeWidth="1.5" strokeLinecap="round" opacity={flat ? 0.5 : 0.42} fill="none">
        <path d="M15.4 36.6 L33.2 13.4" />
        <path d="M19.4 31.4 L27.6 28.6" />
        <path d="M22.4 26.6 L30.4 23.8" />
        <path d="M25.4 21.6 L32.4 18.8" />
      </g>
    </>),
    // two leaf-wings meeting at the base — reads as flight and canopy growth
    wing: (<>
      <path d="M24 43 C12 35 7 21 13 8 C23 15 28 30 24 43 Z" fill={deep} />
      <path d="M24 43 C36 35 41 21 35 8 C25 15 20 30 24 43 Z" fill={light} />
    </>),
    leaf: (<>
      <path d="M24 5 C37 15 37 33 24 44 C11 33 11 15 24 5 Z" fill={light} />
      <path d="M24 11 L24 39" stroke={tone === 'ink' ? '#0E1F12' : '#13241A'} strokeWidth="2" strokeLinecap="round" opacity="0.5" />
    </>),
    pin: (<>
      <path d="M24 4 C15 4 8 11 8 20 C8 31 24 44 24 44 C24 44 40 31 40 20 C40 11 33 4 24 4 Z" fill={solid} />
      <path d="M24 12 C30 16 30 25 24 30 C18 25 18 16 24 12 Z" fill={tone === 'ink' ? '#A6D6B9' : '#0E1F12'} />
    </>),
  };
  return (
    <svg width={size} height={size} viewBox="0 0 48 48" fill="none" style={{ display: 'block' }} aria-label="Arah">
      {paths[variant] || paths.wing}
    </svg>
  );
}

// Brand logo. variant: 'mark' | 'lockup'. mark: 'wing' | 'leaf' | 'pin' | 'png'.
function Logo({ size = 32, lockup = false, color = T.fg, mark = 'png' }) {
  const glyph = mark === 'png'
    ? <img src="../../assets/arah-logo.png" alt="Arah" style={{ height: size, width: 'auto', display: 'block' }} />
    : <LogoMark size={size} variant={mark} />;
  if (!lockup) return glyph;
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
      {glyph}
      <span style={{ fontFamily: T.display, fontWeight: 700, fontSize: size * 0.7, color, letterSpacing: -0.6 }}>Arah</span>
    </div>
  );
}

// Circular avatar with initials
function Avatar({ color = T.greenDeep, name = '', size = 40, ring = false, resident = false }) {
  const initials = name.split(/[ .]/).filter(Boolean).slice(0, 2).map(w => w[0]?.toUpperCase()).join('');
  return (
    <div style={{ position: 'relative', flexShrink: 0 }}>
      <div style={{
        width: size, height: size, borderRadius: '50%',
        background: `linear-gradient(150deg, ${color}, ${color}bb)`,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        color: '#fff', fontFamily: T.display, fontWeight: 600, fontSize: size * 0.36,
        boxShadow: ring ? `0 0 0 2px ${T.bg2}, 0 0 0 3.5px ${T.green}` : 'inset 0 1px 0 rgba(255,255,255,0.15)',
        letterSpacing: 0.2,
      }}>{initials}</div>
      {resident && (
        <div style={{
          position: 'absolute', bottom: -1, right: -1, width: size * 0.4, height: size * 0.4,
          borderRadius: '50%', background: T.greenSolid, border: `2px solid ${T.bg2}`,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <Icon name="verified" size={size * 0.26} color="#0E1F12" fill={1} />
        </div>
      )}
    </div>
  );
}

// Role pill: morador vs visitante
function RoleBadge({ role, size = 'sm' }) {
  const resident = role === 'morador';
  const fs = size === 'sm' ? 10.5 : 12;
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 4, padding: size === 'sm' ? '2px 7px' : '4px 10px',
      borderRadius: 999, fontFamily: T.sans, fontWeight: 600, fontSize: fs, letterSpacing: 0.2,
      background: resident ? T.greenDim : 'rgba(166,172,158,0.12)',
      color: resident ? T.green : T.fg2,
    }}>
      <Icon name={resident ? 'cottage' : 'explore'} size={fs + 2} fill={resident ? 1 : 0} />
      {resident ? 'Morador' : 'Visitante'}
    </span>
  );
}

// Primary / secondary / ghost button
function Btn({ children, kind = 'primary', icon, iconEnd, onClick, full = false, size = 'md', style = {} }) {
  const pad = size === 'lg' ? '15px 22px' : size === 'sm' ? '8px 14px' : '12px 18px';
  const fs = size === 'lg' ? 16 : size === 'sm' ? 13.5 : 15;
  const kinds = {
    primary: { background: T.greenGrad, color: '#0C1B10', border: 'none', boxShadow: T.greenGlow },
    secondary: { background: 'transparent', color: T.fg, border: `1px solid ${T.lineHi}` },
    ghost: { background: 'transparent', color: T.green, border: 'none' },
    dark: { background: T.cardFlat, color: T.fg, border: `1px solid ${T.line}` },
  };
  return (
    <button onClick={onClick} style={{
      display: 'inline-flex', alignItems: 'center', justifyContent: 'center', gap: 8,
      padding: pad, borderRadius: 14, fontFamily: T.sans, fontWeight: 600, fontSize: fs,
      cursor: 'pointer', width: full ? '100%' : undefined, letterSpacing: -0.1,
      transition: 'transform .12s, filter .18s', WebkitTapHighlightColor: 'transparent',
      ...kinds[kind], ...style,
    }}
    onMouseDown={e => e.currentTarget.style.transform = 'scale(0.975)'}
    onMouseUp={e => e.currentTarget.style.transform = 'scale(1)'}
    onMouseLeave={e => e.currentTarget.style.transform = 'scale(1)'}>
      {icon && <Icon name={icon} size={fs + 4} fill={kind === 'primary' ? 1 : 0} weight={500} />}
      {children}
      {iconEnd && <Icon name={iconEnd} size={fs + 4} fill={kind === 'primary' ? 1 : 0} weight={500} />}
    </button>
  );
}

// Pill chip
function Chip({ children, active = false, onClick, icon, tone = 'green' }) {
  const toneColor = tone === 'alert' ? T.alert : T.green;
  const toneDim = tone === 'alert' ? T.alertDim : T.greenDim;
  return (
    <button onClick={onClick} style={{
      display: 'inline-flex', alignItems: 'center', gap: 6, flexShrink: 0,
      padding: '7px 13px', borderRadius: 999, cursor: 'pointer',
      fontFamily: T.sans, fontWeight: 500, fontSize: 13, letterSpacing: -0.1,
      background: active ? toneDim : 'transparent',
      color: active ? toneColor : T.fg2,
      border: `1px solid ${active ? 'transparent' : T.line}`,
      transition: 'all .15s', WebkitTapHighlightColor: 'transparent',
    }}>
      {icon && <Icon name={icon} size={15} fill={active ? 1 : 0} />}
      {children}
    </button>
  );
}

function Eyebrow({ children, color = T.green, style = {} }) {
  return (
    <div style={{
      fontFamily: T.sans, fontSize: 11, fontWeight: 600, letterSpacing: 1.4,
      textTransform: 'uppercase', color, ...style,
    }}>{children}</div>
  );
}

// Card surface with depth gradient + layered elevation
function Card({ children, style = {}, onClick, hi = false, glow = false }) {
  return (
    <div onClick={onClick} style={{
      background: hi ? T.cardHiGrad : T.cardGrad,
      border: `1px solid ${T.line}`, borderRadius: 20,
      boxShadow: glow ? T.shadowCard : T.shadowSoft,
      cursor: onClick ? 'pointer' : undefined, WebkitTapHighlightColor: 'transparent',
      ...style,
    }}>{children}</div>
  );
}

Object.assign(window, { Icon, Logo, LogoMark, Avatar, RoleBadge, Btn, Chip, Eyebrow, Card });
