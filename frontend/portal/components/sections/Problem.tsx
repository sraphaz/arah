import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Problem() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-3">
              <h2 className="text-2xl font-semibold tracking-tight text-forest-950">O Problema</h2>
              <p className="text-lg text-forest-800">
                O ruído da escala global dificulta o contexto local
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-3">
              <div className="space-y-3 rounded-2xl border border-white/60 bg-white/60 p-5">
                <h3 className="text-base font-semibold text-forest-900">Relevância diluída</h3>
                <p className="text-sm leading-relaxed text-forest-800">
                  Plataformas generalistas priorizam alcance global, tornando difícil descobrir
                  informações verdadeiramente relevantes para o entorno imediato onde vivemos e
                  atuamos.
                </p>
              </div>
              <div className="space-y-3 rounded-2xl border border-white/60 bg-white/60 p-5">
                <h3 className="text-base font-semibold text-forest-900">
                  Falta de contexto territorial
                </h3>
                <p className="text-sm leading-relaxed text-forest-800">
                  Ferramentas existentes não reconhecem adequadamente as especificidades de cada
                  território — suas regras, cultura, dinâmicas de confiança e formas de organização
                  comunitária.
                </p>
              </div>
              <div className="space-y-3 rounded-2xl border border-white/60 bg-white/60 p-5">
                <h3 className="text-base font-semibold text-forest-900">Dependência de gigantes</h3>
                <p className="text-sm leading-relaxed text-forest-800">
                  Comunidades locais ficam reféns de algoritmos opacos e políticas de plataformas
                  centralizadas que não atendem às necessidades específicas do território.
                </p>
              </div>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
