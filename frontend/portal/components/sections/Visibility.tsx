import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import SectionDense from "@/components/ui/SectionDense";

const VISIBILITY = [
  {
    title: "Visitante & Residente",
    items: [
      "Visitante vê o que é público no território.",
      "Residente acessa conteúdos e dinâmicas restritas.",
      "Regras claras reduzem ruído e aumentam confiança."
    ]
  },
  {
    title: "Público & Restrito",
    items: [
      "Algumas postagens são úteis para visitantes (serviços, eventos abertos).",
      "Outras exigem pertencimento (alertas locais, decisões comunitárias).",
      "O modelo prioriza segurança e autonomia territorial."
    ]
  }
];

export default function Visibility() {
  return (
    <SectionDense>
      <RevealOnScroll>
        <GlassCard tone="dense">
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-white">
              Regras de visibilidade: por que isso importa
            </h2>
            <div className="grid gap-10 md:grid-cols-2">
              {VISIBILITY.map((col, index) => (
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
