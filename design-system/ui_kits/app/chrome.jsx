// chrome.jsx — Arah app shell: territory top bar (logo + chat + notifications) + bottom nav.

function TopBar({ territory, onTerritoryTap, onChat, onNotif, unread = 0, chatUnread = 0 }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 10, padding: '6px 16px 14px', position: 'relative', zIndex: 5 }}>
      <button onClick={onTerritoryTap} style={{
        display: 'flex', alignItems: 'center', gap: 10, flex: 1, minWidth: 0,
        background: 'transparent', border: 'none', cursor: 'pointer', padding: 0,
        textAlign: 'left', WebkitTapHighlightColor: 'transparent',
      }}>
        <div style={{
          width: 40, height: 40, borderRadius: 13, flexShrink: 0, overflow: 'hidden',
          background: 'linear-gradient(150deg, #1F2A22, #141A15)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          border: `1px solid ${T.lineHi}`, boxShadow: T.shadowSoft,
        }}>
          <Logo size={28} mark="png" />
        </div>
        <div style={{ minWidth: 0 }}>
          <div style={{
            fontFamily: T.sans, fontSize: 10.5, fontWeight: 600, letterSpacing: 1.2,
            textTransform: 'uppercase', color: T.fg3, marginBottom: 1,
          }}>Território</div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 5 }}>
            <span style={{
              fontFamily: T.display, fontSize: 19, fontWeight: 600, color: T.fg,
              letterSpacing: -0.3, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis',
            }}>{territory.name}</span>
            <Icon name="unfold_more" size={17} color={T.fg3} />
          </div>
        </div>
      </button>
      <button onClick={onChat} style={topIconBtn}>
        <Icon name="forum" size={21} color={T.fg2} />
        {chatUnread > 0 && <Dot />}
      </button>
      <button onClick={onNotif} style={topIconBtn}>
        <Icon name="notifications" size={21} color={T.fg2} />
        {unread > 0 && <Dot />}
      </button>
    </div>
  );
}

function Dot() {
  return <span style={{
    position: 'absolute', top: 7, right: 8, width: 8, height: 8, borderRadius: '50%',
    background: T.alert, border: `1.5px solid ${T.bg2}`,
  }} />;
}

const topIconBtn = {
  width: 40, height: 40, borderRadius: 12, flexShrink: 0, position: 'relative',
  background: 'transparent', border: `1px solid ${T.line}`, cursor: 'pointer',
  display: 'flex', alignItems: 'center', justifyContent: 'center',
  WebkitTapHighlightColor: 'transparent',
};
const iconBtn = topIconBtn;
window.iconBtn = iconBtn;

// Bottom nav — 5 tabs. Center "Publicar" is an elevated gradient action.
function BottomNav({ active, onNav }) {
  const tabs = [
    { id: 'feed', icon: 'eco', label: 'Feed' },
    { id: 'explore', icon: 'explore', label: 'Explorar' },
    { id: 'post', icon: 'add', label: 'Publicar', center: true },
    { id: 'services', icon: 'grid_view', label: 'Serviços' },
    { id: 'profile', icon: 'person', label: 'Perfil' },
  ];
  return (
    <div style={{
      display: 'flex', alignItems: 'flex-end', justifyContent: 'space-around',
      padding: '10px 12px 30px', position: 'relative',
      background: 'linear-gradient(to top, #0B0C0A 64%, rgba(11,12,10,0))',
      borderTop: `1px solid ${T.line}`,
    }}>
      {tabs.map(t => {
        const on = active === t.id;
        if (t.center) {
          return (
            <button key={t.id} onClick={() => onNav(t.id)} style={navBtn}>
              <div style={{
                width: 54, height: 40, borderRadius: 16, background: T.greenGrad,
                display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: T.greenGlow,
              }}>
                <Icon name="add" size={26} color="#0C1B10" weight={500} />
              </div>
              <span style={navLabel(false)}>{t.label}</span>
            </button>
          );
        }
        return (
          <button key={t.id} onClick={() => onNav(t.id)} style={{ ...navBtn, flex: 1 }}>
            <Icon name={t.icon} size={25} color={on ? T.green : T.fg3} fill={on ? 1 : 0} weight={on ? 500 : 400} />
            <span style={navLabel(on)}>{t.label}</span>
          </button>
        );
      })}
    </div>
  );
}
const navBtn = {
  background: 'none', border: 'none', cursor: 'pointer',
  display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 5,
  WebkitTapHighlightColor: 'transparent',
};
const navLabel = (on) => ({
  fontFamily: T.sans, fontSize: 10.5, fontWeight: on ? 600 : 500,
  color: on ? T.green : T.fg3, letterSpacing: 0.1,
});

Object.assign(window, { TopBar, BottomNav });
