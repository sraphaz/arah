import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Future() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-6">
            <h2 className="text-2xl font-semibold tracking-tight text-forest-950">O Futuro</h2>
            <div className="space-y-4 text-base leading-relaxed text-forest-800">
              <h3 className="text-lg font-semibold text-forest-900">
                O futuro é ancestral, local, descentralizado e inevitável
              </h3>
              <h3 className="text-lg font-semibold text-forest-900">
                O futuro é ancestral, local, descentralizado e inevitável
              </h3>
              <p>
                A transformação já começou.
                <br />
                Comunidades estão retomando o controle sobre suas formas de organização, comunicação
                e governança.
              </p>
              <p>
                Plataformas centralizadas que ignoram contextos locais estão perdendo relevância
                onde mais importa: <strong>no território</strong>.
              </p>
              <p>
                O Araponga não é apenas uma ferramenta.
                <br />
                É parte de um movimento maior em direção à autonomia comunitária, transparência e
                relevância local.
              </p>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
