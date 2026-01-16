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
            <div className="space-y-4 text-base leading-relaxed text-forest-800 md:text-lg">
              <p>
                O Araponga organiza-se em domínios funcionais que trabalham de forma integrada.
                <br />
                Cada domínio possui responsabilidades claras e se relaciona com os demais para
                garantir uma experiência completa orientada ao território.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Território</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Define a unidade geográfica e suas regras.
                  <br />
                  Cada território possui limites, governança e políticas próprias de participação.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Vínculo (Membership)</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Gerencia papéis (<strong>visitor / resident</strong>) e permissões.
                  <br />
                  Controla quem pode acessar o quê dentro de cada território.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Feed</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Publicações com múltiplas imagens (até 10), georreferenciamento via GeoAnchors,
                  interações (like, comment, share) e moderação.
                  <br />
                  Feed pessoal e feed do território.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Mapa</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Visualização geográfica de conteúdos com entidades do território (MapEntity) e
                  pins integrados (MapEntity + GeoAnchors de posts e assets).
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Marketplace</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Sistema completo de trocas locais: Stores (lojas/comércios), Items (produtos e
                  serviços com múltiplas imagens), Cart, Checkout, Inquiries e Platform Fees
                  configuráveis por território.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Eventos</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Eventos comunitários por território com georreferenciamento, imagem de capa e
                  imagens adicionais (até 10 no total), participações e gestão completa.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Chat</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Chat territorial com canais (público/moradores) e grupos com aprovação por
                  curadoria. Suporte a envio de imagens (1 imagem por mensagem, máx. 5MB) e
                  governança por território.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Alertas</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Alertas de saúde pública e comunicação emergencial por território para
                  mobilização rápida da comunidade.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Assets</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Recursos compartilhados do território (documentos, mídias) com validação,
                  georreferenciamento e gestão completa.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Mídia</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Sistema completo de mídia com armazenamento local, S3 e Azure Blob, upload,
                  download, cache de URLs (Redis/Memory) e suporte a imagens, vídeos e documentos.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Moderação</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Reports de posts e usuários, bloqueios, sanções territoriais e globais, moderação
                  automática por threshold e auditoria completa.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Notificações</h4>
                <p className="text-sm leading-relaxed text-forest-800">
                  Sistema confiável de notificações in-app com outbox e inbox persistido para
                  entrega garantida de eventos importantes.
                </p>
              </div>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
