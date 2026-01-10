import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import SectionDense from "@/components/ui/SectionDense";

const ARCHITECTURE = [
  {
    title: "Componentes principais",
    items: [
      "API orientada a domínios (Território, Usuários, Feed, Mapa).",
      "Camada de autenticação e papéis (visitante/residente).",
      "Persistência por território e trilhas de auditoria."
    ]
  },
  {
    title: "Princípios de design",
    items: [
      "Domínio primeiro: regras explícitas antes de features.",
      "Evolução incremental: MVP simples, extensões por módulos.",
      "Transparência: governança e visibilidade claras."
    ]
  }
];

export default function Architecture() {
  return (
    <SectionDense>
      <RevealOnScroll>
        <GlassCard tone="dense">
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-white">
              Arquitetura técnica: domínio primeiro, evolução incremental
            </h2>
            <div className="grid gap-8 md:grid-cols-2">
              {ARCHITECTURE.map((col, index) => (
                <RevealOnScroll key={col.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/20 bg-white/10 p-5 text-white/85">
                    <h3 className="text-base font-semibold text-white">{col.title}</h3>
                    <ul className="mt-4 list-disc space-y-2 pl-5 text-sm">
                      {col.items.map((item) => (
                        <li key={item}>{item}</li>
                      ))}
                    </ul>
                  </div>
                </RevealOnScroll>
              ))}
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </SectionDense>
  );
}
