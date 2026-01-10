import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const ROADMAP = [
  {
    title: "Fase 1: MVP sólido",
    items: ["Cadastro e autenticação", "Seleção de território", "Feed e mapa base", "Papéis visitante/residente"]
  },
  {
    title: "Fase 2: Mídia regenerativa",
    items: ["Alertas comunitários", "Eventos e mutirões", "Ferramentas de moderação", "Camadas territoriais"]
  },
  {
    title: "Fase 3: Propriedades avançadas",
    items: ["Governança ampliada", "Economias locais", "Identidade descentralizada", "Integrações e parceiros"]
  }
];

export default function Roadmap() {
  return (
    <Section id="roadmap">
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-forest-950">
              Roadmap: evolução com três fases
            </h2>
            <div className="grid gap-6 md:grid-cols-3">
              {ROADMAP.map((col, index) => (
                <RevealOnScroll key={col.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/40 bg-white/30 p-5 text-forest-800">
                    <h3 className="text-base font-semibold text-forest-900">{col.title}</h3>
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
