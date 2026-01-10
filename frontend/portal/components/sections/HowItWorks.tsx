import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import SectionDense from "@/components/ui/SectionDense";

const STEPS = [
  {
    title: "Login e autenticação",
    body: "Usuário faz login e cria identidade básica. Ainda não há vínculo com território específico."
  },
  {
    title: "Escolher um território",
    body: "Usuário seleciona o território de interesse. Cada território possui regras próprias."
  },
  {
    title: "Visitante ou Residente",
    body: "Sistema atribui papel inicial. Residência pode ser solicitada conforme regras locais."
  },
  {
    title: "Feed e mapa filtrados",
    body: "Conteúdo exibido respeita o papel do usuário. Tudo aparece em feed e mapa."
  }
];

export default function HowItWorks() {
  return (
    <SectionDense>
      <RevealOnScroll>
        <GlassCard tone="dense">
          <div className="space-y-8">
            <h2 className="text-2xl font-semibold tracking-tight text-white">Como funciona na prática</h2>
            <div className="grid gap-6 md:grid-cols-2">
              {STEPS.map((c, index) => (
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
