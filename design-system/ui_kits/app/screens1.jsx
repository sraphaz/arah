// screens1.jsx — Feed, PostCard, PostDetail (comments), Explore.

function PostCard({ post, onLike, liked, onOpen }) {
  const isAlert = post.type === 'Alert';
  const accent = isAlert ? T.alert : T.green;
  return (
    <Card glow style={{ padding: 16, marginBottom: 12, border: post.pinned ? `1px solid rgba(232,160,106,0.28)` : `1px solid ${T.line}` }}>
      {post.pinned && (
        <div style={{ display: 'inline-flex', alignItems: 'center', gap: 5, marginBottom: 11,
          color: T.alert, fontFamily: T.sans, fontSize: 11.5, fontWeight: 600, letterSpacing: 0.2 }}>
          <Icon name="push_pin" size={14} fill={1} /> Fixado pelo conselho
        </div>
      )}
      <div style={{ display: 'flex', alignItems: 'center', gap: 11, marginBottom: 13 }}>
        <Avatar color={post.avatar} name={post.author} size={42} resident={post.role === 'morador'} />
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
            <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 15, color: T.fg, letterSpacing: -0.2 }}>{post.author}</span>
            <RoleBadge role={post.role} />
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 6, color: T.fg3, fontFamily: T.sans, fontSize: 12.5, marginTop: 1 }}>
            <span>@{post.handle}</span><span style={{ opacity: 0.5 }}>·</span><span>{post.time}</span>
            {post.visibility === 'ResidentsOnly' && (
              <span style={{ display: 'inline-flex', alignItems: 'center', gap: 3, color: T.green }}>
                <span style={{ opacity: 0.5, color: T.fg3 }}>·</span><Icon name="lock" size={12} /> só moradores
              </span>
            )}
          </div>
        </div>
        {isAlert && (
          <div style={{ display: 'flex', alignItems: 'center', gap: 5, padding: '5px 10px', borderRadius: 999,
            background: T.alertDim, color: T.alert, fontFamily: T.sans, fontSize: 11.5, fontWeight: 600 }}>
            <Icon name="warning" size={14} fill={1} /> Alerta
          </div>
        )}
      </div>
      <div onClick={onOpen} style={{ cursor: 'pointer' }}>
        <h3 style={{ margin: '0 0 6px', fontFamily: T.display, fontWeight: 600, fontSize: 17, color: T.fg, lineHeight: 1.3, letterSpacing: -0.3 }}>{post.title}</h3>
        <p style={{ margin: 0, fontFamily: T.sans, fontSize: 14.5, lineHeight: 1.55, color: T.fg2, textWrap: 'pretty' }}>{post.body}</p>
        {post.photo && (
          <div style={{ marginTop: 13, borderRadius: 14, overflow: 'hidden', border: `1px solid ${T.line}`, position: 'relative' }}>
            <img src={post.photo} alt="" style={{ width: '100%', height: 168, objectFit: 'cover', display: 'block' }} />
            <div style={{ position: 'absolute', inset: 0, background: 'linear-gradient(to top, rgba(11,12,10,0.35), transparent 50%)' }} />
          </div>
        )}
      </div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 22, marginTop: 14 }}>
        <button onClick={onLike} style={postAction(liked ? accent : T.fg3)}>
          <Icon name="favorite" size={19} fill={liked ? 1 : 0} color={liked ? accent : T.fg3} />
          {post.likes + (liked ? 1 : 0)}
        </button>
        <button onClick={onOpen} style={postAction(T.fg3)}><Icon name="mode_comment" size={18} /> {post.comments}</button>
        <button style={postAction(T.fg3)}><Icon name="ios_share" size={18} /></button>
        <div style={{ flex: 1 }} />
        <button style={{ ...postAction(T.fg3), gap: 0 }}><Icon name="bookmark_border" size={19} /></button>
      </div>
    </Card>
  );
}
const postAction = (color) => ({
  display: 'inline-flex', alignItems: 'center', gap: 6, background: 'none', border: 'none',
  cursor: 'pointer', color, fontFamily: T.sans, fontSize: 13.5, fontWeight: 500,
  WebkitTapHighlightColor: 'transparent', padding: 0,
});

function FeedScreen({ territory, content, likes, onLike, onOpen, role }) {
  const [filter, setFilter] = React.useState('Tudo');
  const filters = ['Tudo', 'Avisos', 'Vizinhança'];
  let posts = content.feed;
  if (filter === 'Avisos') posts = posts.filter(p => p.type === 'Alert');
  if (filter === 'Vizinhança') posts = posts.filter(p => p.visibility !== 'ResidentsOnly');
  return (
    <div>
      <div style={{ padding: '2px 18px 12px' }}>
        <div style={{ display: 'flex', gap: 8, overflowX: 'auto', paddingBottom: 2 }} className="noscroll">
          {filters.map(f => <Chip key={f} active={filter === f} onClick={() => setFilter(f)}>{f}</Chip>)}
        </div>
      </div>
      {role === 'visitante' && (
        <div style={{ padding: '0 18px 12px' }}>
          <Card onClick={() => window.openJourney('residencia')} style={{ padding: 14, display: 'flex', alignItems: 'center', gap: 12, background: T.cardHiGrad }}>
            <div style={{ width: 38, height: 38, borderRadius: 11, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <Icon name="cottage" size={20} color={T.green} fill={1} />
            </div>
            <div style={{ flex: 1 }}>
              <div style={{ fontFamily: T.sans, fontSize: 13.5, fontWeight: 600, color: T.fg }}>Você é visitante de {territory.name}</div>
              <div style={{ fontFamily: T.sans, fontSize: 12, color: T.fg3, marginTop: 1 }}>Confirme residência para ver posts e votar.</div>
            </div>
            <Icon name="chevron_right" size={20} color={T.fg3} />
          </Card>
        </div>
      )}
      <div style={{ padding: '0 18px 8px' }}>
        {posts.map(p => <PostCard key={p.id} post={p} liked={!!likes[p.id]} onLike={() => onLike(p.id)} onOpen={() => onOpen(p.id)} />)}
        <div style={{ textAlign: 'center', color: T.fg3, fontFamily: T.sans, fontSize: 12.5, padding: '14px 0 4px', letterSpacing: 0.2 }}>
          Você viu tudo de {territory.name} · {territory.members.toLocaleString('pt-BR')} moradores
        </div>
      </div>
    </div>
  );
}

// Post detail with comments + composer
function PostDetailScreen({ postId, likes, onLike }) {
  const allPosts = Object.values(window.ARAH.content).flatMap(c => c.feed);
  const post = allPosts.find(p => p.id === postId) || window.ARAH.content.t1.feed[0];
  const [comments, setComments] = React.useState(window.ARAH.comments[postId] || []);
  const [draft, setDraft] = React.useState('');
  const send = () => {
    if (!draft.trim()) return;
    setComments(c => [...c, { id: 'cx' + Date.now(), author: 'Ana Ribeiro', avatar: '#4F956F', role: 'visitante', time: 'agora', body: draft.trim() }]);
    setDraft('');
  };
  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <div className="appscroll" style={{ flex: 1, overflowY: 'auto', padding: '0 18px 12px' }}>
        <PostCard post={post} liked={!!likes[postId]} onLike={() => onLike(postId)} onOpen={() => {}} />
        <div style={{ fontFamily: T.sans, fontSize: 12.5, fontWeight: 600, letterSpacing: 0.6, textTransform: 'uppercase', color: T.fg3, margin: '4px 2px 12px' }}>
          {comments.length} comentários
        </div>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
          {comments.map(c => (
            <div key={c.id} style={{ display: 'flex', gap: 11 }}>
              <Avatar color={c.avatar} name={c.author} size={34} resident={c.role === 'morador'} />
              <div style={{ flex: 1 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
                  <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 13.5, color: T.fg }}>{c.author}</span>
                  <span style={{ fontFamily: T.sans, fontSize: 11.5, color: T.fg3 }}>{c.time}</span>
                </div>
                <p style={{ margin: '3px 0 0', fontFamily: T.sans, fontSize: 14, lineHeight: 1.5, color: T.fg2 }}>{c.body}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 9, padding: '12px 16px 16px', borderTop: `1px solid ${T.line}`, background: T.bg2 }}>
        <input value={draft} onChange={e => setDraft(e.target.value)} onKeyDown={e => e.key === 'Enter' && send()}
          placeholder="Comentar como visitante…" style={{
            flex: 1, background: T.cardFlat, color: T.fg, border: `1px solid ${T.line}`, borderRadius: 999,
            padding: '11px 16px', fontFamily: T.sans, fontSize: 14, outline: 'none',
          }} />
        <button onClick={send} style={{
          width: 42, height: 42, borderRadius: '50%', border: 'none', background: T.greenGrad, cursor: 'pointer',
          display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: T.greenGlow, flexShrink: 0,
        }}><Icon name="send" size={19} color="#0C1B10" fill={1} /></button>
      </div>
    </div>
  );
}

function ExploreScreen({ territory, activeId, onEnter, onMap, onTool, role }) {
  const list = window.ARAH.territories;
  const content = window.ARAH.getContent(activeId);
  const tools = [
    { id: 'map', icon: 'map', label: 'Mapa', color: T.green },
    { id: 'events', icon: 'event', label: 'Eventos', color: T.blue },
    { id: 'health', icon: 'eco', label: 'Saúde do território', color: T.green },
    { id: 'elections', icon: 'how_to_vote', label: 'Eleições', color: T.alert, badge: 'Novo' },
    ...(role === 'curador' ? [{ id: 'manage', icon: 'shield_person', label: 'Gestão', color: T.alert, badge: 'Curador' }] : []),
  ];
  return (
    <div style={{ padding: '0 18px 12px' }}>
      <Card onClick={onMap} hi style={{ padding: 0, overflow: 'hidden', marginBottom: 16 }}>
        <div style={{ position: 'relative', height: 116 }}>
          <MapCanvas height={116} mini tile={content.tile} />
          <div style={{ position: 'absolute', inset: 0, display: 'flex', alignItems: 'center', gap: 11, padding: 16,
            background: 'linear-gradient(to right, rgba(11,12,10,0.85), rgba(11,12,10,0.2))' }}>
            <div style={{ width: 42, height: 42, borderRadius: 12, background: T.greenDim, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <Icon name="map" size={22} color={T.green} fill={1} />
            </div>
            <div style={{ flex: 1 }}>
              <div style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg }}>Mapa do território</div>
              <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg2 }}>Lugares, trilhas, nascentes e mirantes</div>
            </div>
            <Icon name="arrow_forward" size={20} color={T.fg2} />
          </div>
        </div>
      </Card>

      {/* Ferramentas do território (movido do Perfil para Explorar) */}
      <SectionLabel icon="apps">Ferramentas de {territory.name}</SectionLabel>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10, marginTop: 11, marginBottom: 20 }}>
        {tools.map(t => (
          <Card key={t.id} onClick={() => onTool(t.id)} style={{ display: 'flex', alignItems: 'center', gap: 11, padding: 14 }}>
            <div style={{ width: 38, height: 38, borderRadius: 11, background: `${t.color}1f`, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
              <Icon name={t.icon} size={20} color={t.color} fill={1} />
            </div>
            <span style={{ flex: 1, fontFamily: T.sans, fontSize: 13.5, fontWeight: 500, color: T.fg, lineHeight: 1.2 }}>{t.label}</span>
            {t.badge && <span style={{ fontFamily: T.sans, fontSize: 9.5, fontWeight: 700, color: t.color, background: `${t.color}1f`, padding: '2px 6px', borderRadius: 999, letterSpacing: 0.3 }}>{t.badge}</span>}
          </Card>
        ))}
      </div>

      <SectionLabel icon="travel_explore">Trocar de território</SectionLabel>
      <p style={{ margin: '11px 0 12px', fontFamily: T.sans, fontSize: 13.5, color: T.fg2 }}>Toque em um território para ver o feed da região ou trocar de comunidade.</p>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {list.map(t => {
          const on = t.id === activeId;
          return (
            <Card key={t.id} onClick={() => onEnter(t.id)} hi={on} style={{ display: 'flex', alignItems: 'center', gap: 13, padding: 14, border: `1px solid ${on ? 'rgba(166,214,185,0.32)' : T.line}` }}>
              <div style={{ width: 46, height: 46, borderRadius: 14, flexShrink: 0, background: on ? T.greenGrad : T.cardHi, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Icon name={on ? 'location_on' : 'forest'} size={24} color={on ? '#0C1B10' : T.green} fill={1} />
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <span style={{ fontFamily: T.display, fontWeight: 600, fontSize: 16, color: T.fg, letterSpacing: -0.2 }}>{t.name}</span>
                  {on && <span style={{ fontFamily: T.sans, fontSize: 10.5, fontWeight: 600, color: T.green, background: 'rgba(166,214,185,0.15)', padding: '2px 8px', borderRadius: 999 }}>ATIVO</span>}
                </div>
                <div style={{ fontFamily: T.sans, fontSize: 12.5, color: T.fg3, marginTop: 2 }}>{t.region} · {t.distance} km</div>
                <div style={{ fontFamily: T.sans, fontSize: 13, color: T.fg2, marginTop: 5 }}>{t.desc}</div>
              </div>
              <Icon name={on ? 'check_circle' : 'chevron_right'} size={on ? 22 : 20} color={on ? T.green : T.fg3} fill={on ? 1 : 0} />
            </Card>
          );
        })}
      </div>
    </div>
  );
}

Object.assign(window, { PostCard, FeedScreen, PostDetailScreen, ExploreScreen });
