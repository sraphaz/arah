import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Future() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-4">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">O Futuro</h2>
              <h3 className="text-xl font-semibold text-forest-900 md:text-2xl">
                O futuro é ancestral, local, descentralizado e inevitável
              </h3>
            </div>
            <div className="space-y-6 text-base leading-relaxed text-forest-800 md:text-lg">
              <p>
                A transformação já começou.
                <br />
                Comunidades estão retomando o controle sobre suas formas de organização, comunicação
                e governança.
              </p>
              <p>
                Plataformas centralizadas que ignoram contextos locais estão perdendo relevância
                onde mais importa: <strong className="font-semibold text-forest-900">no território</strong>.
              </p>
              <div className="rounded-2xl border border-forest-200/60 bg-forest-50/50 p-6">
                <p className="font-semibold text-forest-900">
                  O Araponga não é apenas uma ferramenta.
                </p>
                <p className="mt-2">
                  É parte de um movimento maior em direção à <strong className="font-semibold text-forest-900">autonomia comunitária</strong>,{" "}
                  <strong className="font-semibold text-forest-900">transparência</strong> e{" "}
                  <strong className="font-semibold text-forest-900">relevância local</strong>.
                </p>
              </div>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
