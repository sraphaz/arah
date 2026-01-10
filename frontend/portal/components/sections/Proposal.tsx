import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

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
              <div className="h-full rounded-2xl border border-white/60 bg-white/60 p-5 text-forest-800">
                <h3 className="text-base font-semibold text-forest-900">Orientado ao território</h3>
                <p className="mt-3 text-sm leading-relaxed">
                  Cada instância representa um território específico, com suas próprias regras de
                  visibilidade, governança e organização comunitária.
                </p>
              </div>
              <div className="h-full rounded-2xl border border-white/60 bg-white/60 p-5 text-forest-800">
                <h3 className="text-base font-semibold text-forest-900">
                  Feed local + mapa integrado
                </h3>
                <p className="mt-3 text-sm leading-relaxed">
                  Publicações e eventos aparecem tanto em timeline quanto em visualização
                  geográfica, facilitando a descoberta do que acontece por perto.
                </p>
              </div>
              <div className="h-full rounded-2xl border border-white/60 bg-white/60 p-5 text-forest-800">
                <h3 className="text-base font-semibold text-forest-900">Governança explícita</h3>
                <p className="mt-3 text-sm leading-relaxed">
                  Regras claras sobre quem pode ver e participar, com distinção entre visitantes e
                  residentes, respeitando a autonomia comunitária.
                </p>
              </div>
            </div>
            <div className="space-y-4 text-base leading-relaxed text-forest-800">
              <p>
                O Araponga é uma plataforma de <strong>código aberto</strong> projetada para
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
