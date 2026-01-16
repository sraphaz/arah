import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Inspiration() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-4">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                O Pássaro que nos inspira
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                O nome Araponga carrega significado profundo sobre comunicação, território e
                presença.
              </p>
            </div>
            <div className="grid gap-8 md:grid-cols-2 md:items-center">
              <div className="space-y-4 text-base leading-relaxed text-forest-800">
                <p>
                  O <strong className="font-semibold text-forest-900">Araponga</strong>, também conhecido como{" "}
                  <strong className="font-semibold text-forest-900">"pássaro-ferreiro"</strong>, é famoso por
                  seu canto metálico e ressonante nas florestas brasileiras.
                </p>
                <p>
                  Sua presença vibrante simboliza a <strong className="font-semibold text-forest-900">força e a resiliência</strong> da natureza,
                  comunicando-se de forma clara e autêntica em seu território.
                </p>
                <p>
                  A escolha do Araponga reflete nosso compromisso com a <strong className="font-semibold text-forest-900">autenticidade e a comunicação
                  clara</strong>.
                </p>
                <p>
                  Assim como ele se destaca em seu território, buscamos valorizar as comunidades
                  locais e suas <strong className="font-semibold text-forest-900">vozes singulares</strong>.
                </p>
              </div>
              <div className="flex items-center justify-center">
                <div className="rounded-3xl border border-forest-200/60 bg-forest-50/50 p-8 text-center">
                  <div className="text-6xl mb-4">🐦</div>
                  <p className="text-sm font-semibold text-forest-900 italic">
                    "Araponga canta para avisar, proteger e comunicar."
                  </p>
                  <p className="text-xs text-forest-700 mt-2">
                    Que esta plataforma faça o mesmo.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
