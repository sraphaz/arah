import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const ROADMAP = [
  {
    title: "Fase 1 — MVP sólido",
    status: "Em desenvolvimento",
    items: [
      "Território e vínculos funcionais",
      "Feed de publicações e eventos",
      "Mapa básico",
      "Regras de visibilidade",
      "API documentada + portal web"
    ]
  },
  {
    title: "Fase 2 — Mídia georreferenciada",
    status: "Planejado",
    items: [
      "Sistema Media + GeoAnchor",
      "Upload de fotos e vídeos associados a locais",
      "Galeria territorial de memórias",
      "Marcadores visuais no mapa"
    ]
  },
  {
    title: "Fase 3 — Experiências avançadas",
    status: "Visão de longo prazo",
    items: [
      "Plataforma de produtos e serviços territoriais",
      "Integração com dados abertos",
      "Ferramentas de análise comunitária",
      "Indicadores ambientais do território"
    ]
  }
];

export default function Roadmap() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-forest-950">
              Roadmap: evolução em três fases
            </h2>
            <div className="grid gap-6 md:grid-cols-3">
              {ROADMAP.map((col, index) => (
                <RevealOnScroll key={col.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-5 text-forest-800">
                    <h3 className="text-base font-semibold text-forest-900">{col.title}</h3>
                    <p className="mt-2 text-sm font-semibold text-forest-700">
                      <strong>Status:</strong> {col.status}
                    </p>
                    <ul className="mt-4 list-disc space-y-2 pl-5 text-sm">
                      {col.items.map((it) => (
                        <li key={it}>{it}</li>
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
