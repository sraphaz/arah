import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const PROPOSALS = [
  {
    title: "Orientado ao território",
    body:
      "Cada comunidade tem regras, identidade e contexto próprios. O Araponga parte do território como unidade primária."
  },
  {
    title: "Feed local + mapa integrado",
    body:
      "Postagens, lugares, alertas e eventos aparecem em timeline e mapa, sempre filtrados pelo território."
  },
  {
    title: "Governança explícita",
    body:
      "Regras de visibilidade e participação são claras: visitante vs residente, conteúdos públicos vs restritos."
  }
];

export default function Proposal() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-forest-950">
              A proposta: território primeiro, comunidade primeiro
            </h2>
            <div className="grid gap-6 md:grid-cols-3">
              {PROPOSALS.map((c, index) => (
                <RevealOnScroll key={c.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/40 bg-white/30 p-5 text-forest-800">
                    <h3 className="text-base font-semibold text-forest-900">{c.title}</h3>
                    <p className="mt-3 text-sm leading-relaxed">{c.body}</p>
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
