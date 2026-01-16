import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

const IMPROVEMENTS = [
  {
    phase: "Fase 2",
    title: "Qualidade de Código e Confiabilidade",
    status: "✅ Completo",
    items: [
      "Paginação completa em 15 endpoints",
      "Validação robusta com FluentValidation",
      "Cobertura de testes >90%",
      "Testes de segurança e performance",
      "Refatoração completa de services"
    ]
  },
  {
    phase: "Fase 3",
    title: "Performance e Escalabilidade",
    status: "✅ Completo",
    items: [
      "Concorrência otimista com RowVersion",
      "Cache distribuído (Redis)",
      "Processamento assíncrono de eventos",
      "Suporte a read replicas",
      "Deployment multi-instância"
    ]
  },
  {
    phase: "Fase 4",
    title: "Observabilidade e Monitoramento",
    status: "✅ Completo",
    items: [
      "Logs centralizados (Serilog + Seq)",
      "Métricas Prometheus",
      "Distributed tracing (OpenTelemetry)",
      "Dashboards e alertas",
      "Runbook e troubleshooting"
    ]
  }
];

export default function Improvements() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-3">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                Melhorias Técnicas Implementadas
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                O Araponga evolui continuamente com melhorias de qualidade, performance e observabilidade.
                Estas são as principais implementações técnicas concluídas:
              </p>
            </div>
            <div className="grid gap-6 md:grid-cols-3">
              {IMPROVEMENTS.map((improvement, index) => (
                <RevealOnScroll key={improvement.phase} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/60 bg-white/65 p-6 text-forest-800 transition-transform duration-300 hover:scale-105">
                    <div className="mb-4">
                      <span className="text-xs font-semibold text-forest-600 uppercase tracking-wide">{improvement.phase}</span>
                      <h3 className="mt-2 text-base font-semibold text-forest-900">{improvement.title}</h3>
                      <p className="mt-2 text-sm font-semibold text-forest-700">
                        {improvement.status}
                      </p>
                    </div>
                    <ul className="mt-4 space-y-2 text-sm">
                      {improvement.items.map((item) => (
                        <li key={item} className="flex items-start gap-2">
                          <span className="mt-0.5 flex-shrink-0 text-forest-600">✓</span>
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
