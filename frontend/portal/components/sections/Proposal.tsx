import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Proposal() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-4">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                A proposta: território primeiro, comunidade primeiro
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                Uma plataforma que coloca o território no centro da experiência digital, respeitando
                a autonomia e as especificidades de cada comunidade.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-3">
              <RevealOnScroll delay={0} className="h-full">
                <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-6 text-forest-800 transition-transform duration-300 hover:scale-105">
                  <div className="mb-3 text-2xl">📍</div>
                  <h3 className="text-base font-semibold text-forest-900 mb-2">Orientado ao território</h3>
                  <p className="text-sm leading-relaxed">
                    Cada instância representa um território específico, com suas próprias regras de
                    visibilidade, governança e organização comunitária.
                  </p>
                </div>
              </RevealOnScroll>
              <RevealOnScroll delay={60} className="h-full">
                <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-6 text-forest-800 transition-transform duration-300 hover:scale-105">
                  <div className="mb-3 text-2xl">🗺️</div>
                  <h3 className="text-base font-semibold text-forest-900 mb-2">
                    Feed local + mapa integrado
                  </h3>
                  <p className="text-sm leading-relaxed">
                    Publicações e eventos aparecem tanto em timeline quanto em visualização
                    geográfica, facilitando a descoberta do que acontece por perto.
                  </p>
                </div>
              </RevealOnScroll>
              <RevealOnScroll delay={120} className="h-full">
                <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-6 text-forest-800 transition-transform duration-300 hover:scale-105">
                  <div className="mb-3 text-2xl">⚖️</div>
                  <h3 className="text-base font-semibold text-forest-900 mb-2">Governança explícita</h3>
                  <p className="text-sm leading-relaxed">
                    Regras claras sobre quem pode ver e participar, com distinção entre visitantes e
                    residentes, respeitando a autonomia comunitária.
                  </p>
                </div>
              </RevealOnScroll>
            </div>
            <div className="space-y-4 text-base leading-relaxed text-forest-800">
              <p>
                O Arah é uma plataforma de <strong>código aberto</strong> projetada para
                fortalecer a organização comunitária a partir do território.
                <br />
                Ela combina feed social, mapeamento colaborativo e regras de participação definidas
                pela própria comunidade.
              </p>
              <p>
                O objetivo é promover relevância local, reduzir ruído informacional e diminuir a
                dependência de plataformas generalistas que não atendem às especificidades de cada
                lugar.
              </p>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
