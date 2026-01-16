import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Join() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-4">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                Faça parte dessa transformação
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                O Araponga é um projeto aberto à colaboração. Sua participação fortalece comunidades
                locais e promove autonomia territorial.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-3">
              <RevealOnScroll delay={0} className="h-full">
                <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-6 text-forest-800">
                  <div className="mb-3 text-3xl">📚</div>
                  <h3 className="text-base font-semibold text-forest-900 mb-2">
                    Explore e aprenda
                  </h3>
                  <p className="text-sm leading-relaxed">
                    Explore o código e a documentação completa no GitHub. Entenda a arquitetura,
                    princípios e funcionalidades.
                  </p>
                </div>
              </RevealOnScroll>
              <RevealOnScroll delay={60} className="h-full">
                <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-6 text-forest-800">
                  <div className="mb-3 text-3xl">🧪</div>
                  <h3 className="text-base font-semibold text-forest-900 mb-2">
                    Teste na prática
                  </h3>
                  <p className="text-sm leading-relaxed">
                    Teste a plataforma em sua comunidade. Veja como o Araponga pode fortalecer
                    organizações locais.
                  </p>
                </div>
              </RevealOnScroll>
              <RevealOnScroll delay={120} className="h-full">
                <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-6 text-forest-800">
                  <div className="mb-3 text-3xl">🤝</div>
                  <h3 className="text-base font-semibold text-forest-900 mb-2">
                    Contribua
                  </h3>
                  <p className="text-sm leading-relaxed">
                    Contribua com ideias, código, testes ou feedback. Cada contribuição fortalece o
                    projeto.
                  </p>
                </div>
              </RevealOnScroll>
            </div>
            <div className="rounded-2xl border border-forest-200/60 bg-forest-50/50 p-6 text-center">
              <p className="text-lg font-semibold text-forest-900 leading-relaxed">
                Território primeiro. Comunidade primeiro. Vida primeiro.
              </p>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
