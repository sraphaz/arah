import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Architecture() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-3">
              <h2 className="text-2xl font-semibold tracking-tight text-forest-950">Arquitetura</h2>
              <h3 className="text-lg font-semibold text-forest-900">
                Domínios principais da plataforma
              </h3>
            </div>
            <div className="space-y-4 text-base leading-relaxed text-forest-800">
              <p>
                O Araponga organiza-se em domínios funcionais que trabalham de forma integrada.
                <br />
                Cada domínio possui responsabilidades claras e se relaciona com os demais para
                garantir uma experiência completa orientada ao território.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-2">
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Território</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Define a unidade geográfica e suas regras.
                  <br />
                  Cada território possui limites, governança e políticas próprias de participação.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Vínculo</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Gerencia papéis (<strong>visitor / resident</strong>) e permissões.
                  <br />
                  Controla quem pode acessar o quê dentro de cada território.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Feed</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Publicações e eventos organizados em timeline.
                  <br />
                  Inclui filtros por tipo, data e relevância territorial.
                  <br />
                  Eventos possuem data e hora associadas.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Mapa</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Visualização geográfica de conteúdos.
                  <br />
                  Permite explorar publicações e eventos espacialmente dentro do território.
                </p>
              </div>
            </div>
            <p className="text-sm leading-relaxed text-forest-800">
              <strong>Planejado:</strong> Media + GeoAnchor — sistema de mídia georreferenciada para
              documentar lugares, memórias e histórias do território.
              <br />
              Ainda em desenvolvimento.
            </p>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
