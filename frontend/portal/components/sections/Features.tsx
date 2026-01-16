import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const FEATURES = [
  {
    category: "Core",
    items: [
      "Backend estruturado com Clean Architecture",
      "Autenticação social com JWT e gestão de usuários",
      "Descoberta e seleção de territórios",
      "Vínculos (morador e visitante) com regras de visibilidade",
      "Feature flags por território"
    ]
  },
  {
    category: "Feed e Social",
    items: [
      "Feed territorial com criação, interações (like, comment, share) e moderação",
      "Feed pessoal e feed do território",
      "Posts com GeoAnchors (georreferenciamento)",
      "Posts com múltiplas imagens (até 10 por post)",
      "Paginação completa em todos os endpoints de listagem"
    ]
  },
  {
    category: "Marketplace",
    items: [
      "Stores (lojas/comércios) por território",
      "Items (produtos e serviços) com busca e filtros",
      "Items com múltiplas imagens (até 10 por item)",
      "Cart e Checkout",
      "Inquiries (consultas de compra)",
      "Platform Fees (taxas configuráveis por território)"
    ]
  },
  {
    category: "Eventos e Comunidade",
    items: [
      "Eventos comunitários por território",
      "Participações em eventos",
      "Eventos com georreferenciamento",
      "Eventos com imagem de capa e imagens adicionais (até 10 no total)"
    ]
  },
  {
    category: "Chat",
    items: [
      "Chat territorial: canais (público/moradores) + grupos com aprovação por curadoria",
      "Chat com suporte a envio de imagens (1 imagem por mensagem, máx. 5MB)"
    ]
  },
  {
    category: "Mapa e Localização",
    items: [
      "Mapa territorial com entidades (MapEntity) e relações",
      "Pins integrados (MapEntity + GeoAnchors de posts e assets)",
      "Visualização de entidades do território no mapa"
    ]
  },
  {
    category: "Alertas e Saúde",
    items: [
      "Alertas de saúde pública (Health Alerts)",
      "Comunicação emergencial por território"
    ]
  },
  {
    category: "Assets e Mídia",
    items: [
      "Recursos compartilhados do território (Territory Assets)",
      "Validação e georreferenciamento de assets",
      "Sistema completo de mídia (armazenamento local, S3, Azure Blob)",
      "Upload, download e gestão de mídias (imagens, vídeos, documentos)",
      "Cache de URLs de mídia com suporte a Redis e Memory Cache"
    ]
  },
  {
    category: "Moderação",
    items: [
      "Reports de posts e usuários",
      "Bloqueios de usuários",
      "Sanções territoriais e globais",
      "Moderação automática por threshold"
    ]
  },
  {
    category: "Notificações",
    items: [
      "Notificações in-app com outbox e inbox persistido",
      "Sistema confiável de entrega de notificações"
    ]
  },
  {
    category: "Segurança e Produção",
    items: [
      "JWT secret via variáveis de ambiente (obrigatório, mínimo 32 caracteres)",
      "HTTPS obrigatório em produção com HSTS",
      "Rate limiting completo (proteção contra DDoS e abuso)",
      "Security headers em todas as respostas",
      "Validação completa de input (14 validators FluentValidation)",
      "Testes de segurança abrangentes",
      "CORS configurado com validação em produção",
      "Health checks completos (liveness e readiness)",
      "Logging estruturado (Serilog)",
      "Connection pooling e retry policies",
      "Índices de banco para performance",
      "Cache invalidation automático em 9 services críticos"
    ]
  },
  {
    category: "Testes",
    items: [
      "Testes automatizados (unidade, integração, E2E)",
      "371/371 testes passando (100%)",
      "Cobertura de testes ~50% (objetivo >90%)",
      "Testes de segurança (14 testes)",
      "Testes de performance (7 testes com SLAs definidos)",
      "CI configurado com builds reprodutíveis"
    ]
  }
];

export default function Features() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-4">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                Funcionalidades Implementadas
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                O Araponga já possui um conjunto robusto de funcionalidades implementadas e testadas,
                cobrindo desde o core da plataforma até recursos avançados de segurança e performance.
                <br />
                <strong className="font-semibold text-forest-900">371/371 testes passando (100%)</strong> com cobertura de ~50% e objetivo de >90%.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
              {FEATURES.map((feature, index) => (
                <RevealOnScroll key={feature.category} delay={index * 40} className="h-full">
                  <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-5">
                    <h3 className="text-base font-semibold text-forest-900 mb-3">
                      {feature.category}
                    </h3>
                    <ul className="space-y-2 text-sm text-forest-800">
                      {feature.items.map((item, itemIndex) => (
                        <li key={itemIndex} className="flex items-start gap-2">
                          <span className="mt-1 flex-shrink-0 text-forest-600">✓</span>
                          <span className="leading-relaxed">{item}</span>
                        </li>
                      ))}
                    </ul>
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
