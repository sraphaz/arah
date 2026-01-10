import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import SectionDense from "@/components/ui/SectionDense";

const PROBLEMS = [
  {
    title: "Relevância diluída",
    body:
      "Plataformas generalistas priorizam alcance global, tornando difícil encontrar conteúdos realmente relevantes para o entorno imediato."
  },
  {
    title: "Falta de contexto territorial",
    body:
      "Ferramentas existentes não reconhecem as especificidades de cada lugar: regras, dinâmicas de confiança e organização comunitária."
  },
  {
    title: "Dependência de gigantes",
    body: "Comunidades ficam reféns de algoritmos opacos e políticas que não atendem às necessidades do território."
  }
];

export default function Problem() {
  return (
    <SectionDense>
      <RevealOnScroll>
        <GlassCard tone="dense">
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-white">
              O ruído da escala global dificulta o contexto local
            </h2>
            <div className="grid gap-6 md:grid-cols-3">
              {PROBLEMS.map((c, index) => (
                <RevealOnScroll key={c.title} delay={index * 60} className="h-full">
                  <div className="h-full rounded-2xl border border-white/20 bg-white/10 p-5 text-white/85">
                    <h3 className="text-base font-semibold text-white">{c.title}</h3>
                    <p className="mt-3 text-sm leading-relaxed">{c.body}</p>
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
