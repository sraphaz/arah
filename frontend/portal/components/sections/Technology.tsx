import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Technology() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-forest-950">Tecnologia</h2>
            <div className="space-y-4 text-base leading-relaxed text-forest-800">
              <h3 className="text-lg font-semibold text-forest-900">
                Arquitetura técnica: domínio primeiro, evolução incremental
              </h3>
              <p>
                O Araponga é construído com <strong>.NET</strong> e segue princípios de{" "}
                <strong>Domain-Driven Design (DDD)</strong>.
                <br />
                A arquitetura prioriza domínios de negócio bem definidos e evolução incremental,
                permitindo adicionar funcionalidades sem comprometer a base existente.
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-2">
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Componentes principais</h4>
                <ul className="list-disc space-y-2 pl-5 text-sm text-forest-800">
                  <li>
                    <strong>API RESTful</strong>: expõe recursos via HTTP, documentada com Swagger /
                    OpenAPI
                  </li>
                  <li>
                    <strong>Portal de autosserviço</strong>: interface web para interação com feed,
                    mapa e configurações
                  </li>
                  <li>
                    <strong>Persistência</strong>: banco relacional PostgreSQL com suporte a dados
                    geográficos
                  </li>
                  <li>
                    <strong>Autenticação e autorização</strong>: baseada em papéis e políticas
                    territoriais
                  </li>
                  <li>
                    <strong>Cache distribuído</strong>: Redis com fallback automático para cache em
                    memória
                  </li>
                  <li>
                    <strong>Processamento assíncrono</strong>: eventos processados em background
                    com retry e dead letter queue
                  </li>
                </ul>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h4 className="text-base font-semibold text-forest-900">Princípios de design</h4>
                <ul className="list-disc space-y-2 pl-5 text-sm text-forest-800">
                  <li>Domínio primeiro</li>
                  <li>Evolução incremental</li>
                  <li>API-first</li>
                  <li>Código aberto</li>
                </ul>
                <h4 className="mt-4 text-base font-semibold text-forest-900">Performance e escalabilidade</h4>
                <ul className="list-disc space-y-2 pl-5 text-sm text-forest-800">
                  <li>Concorrência otimista com RowVersion</li>
                  <li>Cache distribuído (Redis)</li>
                  <li>Processamento assíncrono de eventos</li>
                  <li>Suporte a read replicas</li>
                  <li>Deployment multi-instância</li>
                </ul>
                <h4 className="mt-4 text-base font-semibold text-forest-900">Observabilidade e monitoramento</h4>
                <ul className="list-disc space-y-2 pl-5 text-sm text-forest-800">
                  <li>Logs centralizados (Serilog + Seq)</li>
                  <li>Métricas Prometheus (HTTP, negócio, sistema)</li>
                  <li>Distributed tracing (OpenTelemetry)</li>
                  <li>Dashboards e alertas configuráveis</li>
                  <li>Runbook e troubleshooting completo</li>
                </ul>
              </div>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
