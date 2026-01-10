import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import SectionDense from "@/components/ui/SectionDense";

export default function CTA() {
  return (
    <SectionDense>
      <RevealOnScroll>
        <GlassCard tone="dense">
          <div className="space-y-6">
            <h2 className="text-2xl font-semibold tracking-tight text-white">
              O futuro é Ancestral, Local, Descentralizado e Inevitável
            </h2>
            <p className="max-w-3xl text-sm leading-relaxed text-white/85">
              O Araponga é um instrumento para reconectar comunidade e território com clareza, segurança e
              autonomia. Se isso ressoa com vocês, vamos construir juntos.
            </p>
            <div className="flex flex-wrap gap-3">
              <a
                href="#apoie"
                className="rounded-lg bg-white px-5 py-3 text-sm font-semibold text-forest-800 hover:bg-forest-50"
              >
                Primeiros apoiadores
              </a>
              <a
                href="#apoie"
                className="rounded-lg border border-white/40 px-5 py-3 text-sm font-medium text-white hover:bg-white/10"
              >
                Entrar em contato
              </a>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </SectionDense>
  );
}
