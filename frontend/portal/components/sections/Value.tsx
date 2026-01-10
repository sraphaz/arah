import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const VALUES = [
  { title: "Relevância local", body: "Menos ruído, mais utilidade no dia a dia." },
  { title: "Construção de confiança", body: "Papéis e regras explícitos fortalecem a comunidade." },
  { title: "Organização facilitada", body: "Eventos, alertas e decisões num só lugar." },
  { title: "Custo reduzido", body: "Infra e governança pensadas para pequenos territórios." }
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
            <div className="grid gap-6 md:grid-cols-4">
              {VALUES.map((c, index) => (
                <RevealOnScroll key={c.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/40 bg-white/30 p-5 text-forest-800">
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
