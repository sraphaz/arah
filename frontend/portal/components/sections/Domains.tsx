import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const DOMAINS = [
  {
    title: "Território",
    body: "Regras, geofencing, identidade local, pontos de interesse."
  },
  {
    title: "Usuários",
    body: "Perfis, verificação de residência, papéis e permissões."
  },
  {
    title: "Feed",
    body: "Publicações, alertas, eventos e moderação local."
  },
  {
    title: "Mapa",
    body: "Camadas territoriais, locais, eventos e visualização contextual."
  }
];

export default function Domains() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-6">
            <div>
              <h2 className="text-2xl font-semibold tracking-tight text-forest-950">
                Domínios principais da plataforma
              </h2>
              <p className="mt-4 max-w-3xl text-sm leading-relaxed text-forest-800">
                O Araponga organiza-se em domínios funcionais integrados, com responsabilidades claras.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-2">
              {DOMAINS.map((c, index) => (
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
