import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const ROADMAP = [
  {
    title: "Fase 1 — MVP sólido",
    status: "✅ Completo",
    items: [
      "✅ Território e vínculos funcionais",
      "✅ Feed de publicações com múltiplas imagens e GeoAnchors",
      "✅ Mapa com entidades e pins integrados",
      "✅ Regras de visibilidade (visitor/resident)",
      "✅ API documentada + portal web",
      "✅ Marketplace completo (Stores, Items, Cart, Checkout)",
      "✅ Eventos comunitários com georreferenciamento",
      "✅ Chat territorial (canais e grupos)",
      "✅ Alertas de saúde pública",
      "✅ Assets e recursos territoriais",
      "✅ Sistema completo de mídia",
      "✅ Moderação e reports",
      "✅ Notificações in-app confiáveis",
      "✅ Segurança e rate limiting",
      "✅ Paginação completa (15 endpoints)",
      "✅ Cache distribuído e otimizações"
    ]
  },
  {
    title: "Fase 2 — Experiências avançadas",
    status: "🚧 Em planejamento",
    items: [
      "Frontend e experiências móveis",
      "Friends (círculo interno) e stories exclusivos",
      "Admin/observabilidade com dashboards avançados",
      "GeoAnchor avançado / memórias / galeria",
      "Integração com dados abertos",
      "Ferramentas de análise comunitária"
    ]
  },
  {
    title: "Fase 3 — Visão de longo prazo",
    status: "💭 Futuro",
    items: [
      "Economias e moedas locais",
      "Trocas de serviços comunitários",
      "Governança distribuída",
      "Indicadores ambientais do território",
      "Integração com iniciativas regenerativas"
    ]
  }
];

export default function Roadmap() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-3">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                Roadmap: evolução em três fases
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                O Araponga evolui de forma incremental, priorizando solidez antes de escala.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-3">
              {ROADMAP.map((col, index) => (
                <RevealOnScroll key={col.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-5 text-forest-800">
                    <h3 className="text-base font-semibold text-forest-900">{col.title}</h3>
                    <p className="mt-2 text-sm font-semibold text-forest-700">
                      <strong>Status:</strong> {col.status}
                    </p>
                    <ul className="mt-4 space-y-2 text-sm">
                      {col.items.map((it) => (
                        <li key={it} className="flex items-start gap-2">
                          <span className="mt-0.5 flex-shrink-0">{it.startsWith("✅") ? "✅" : it.startsWith("🚧") ? "🚧" : it.startsWith("💭") ? "💭" : "•"}</span>
                          <span className={it.startsWith("✅") ? "text-forest-700" : ""}>{it.replace(/^[✅🚧💭] /, "")}</span>
                        </li>
                      ))}
                    </ul>
                  </div>
                </RevealOnScroll>
              ))}
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
