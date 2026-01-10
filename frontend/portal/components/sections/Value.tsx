import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const VALUES = [
  {
    title: "Relevância local garantida",
    body:
      "Informações filtradas por território eliminam ruído global e priorizam o que realmente importa para quem vive ali."
  },
  {
    title: "Construção de confiança",
    body:
      "Regras explícitas de visibilidade e governança transparente promovem ambientes seguros para organização comunitária."
  },
  {
    title: "Organização facilitada",
    body:
      "Feed + mapa integrados simplificam a coordenação de eventos, mobilizações e iniciativas locais sem depender de plataformas externas."
  },
  {
    title: "Custo reduzido de curadoria",
    body:
      "Filtros territoriais e regras locais diminuem a necessidade de moderação centralizada intensiva, distribuindo responsabilidade."
  }
];

export default function Value() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-forest-950">
              Valor gerado para comunidades e parceiros
            </h2>
            <div className="grid gap-6 md:grid-cols-2">
              {VALUES.map((c, index) => (
                <RevealOnScroll key={c.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-5 text-forest-800">
                    <h3 className="text-sm font-semibold text-forest-900">{c.title}</h3>
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
