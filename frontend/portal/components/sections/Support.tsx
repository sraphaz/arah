import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Support() {
  return (
    <Section id="apoie">
      <RevealOnScroll>
        <GlassCard className="support-card">
          <div className="space-y-10">
            <div className="grid gap-10 md:grid-cols-2 md:items-center">
              <div>
                <h3 className="text-xl font-semibold text-forest-950">Apoie o desenvolvimento</h3>
                <p className="mt-4 text-sm leading-relaxed text-forest-800">
                  Se você quer ver o Araponga nascer como infraestrutura territorial, apoie e participe
                  da construção.
                </p>
                <div className="mt-6 flex flex-wrap gap-3">
                  <a
                    href="#"
                    className="rounded-lg bg-forest-700 px-5 py-3 text-sm font-medium text-white hover:bg-forest-800"
                  >
                    Apoiar o Vivo Regenerativa
                  </a>
                  <a
                    href="mailto:contato@araponga.app"
                    className="rounded-lg border border-forest-300 px-5 py-3 text-sm font-medium text-forest-800 hover:bg-forest-50"
                  >
                    contato@araponga.app
                  </a>
                </div>
              </div>
              <div className="rounded-2xl border border-white/40 bg-forest-900/70 p-6 text-white shadow-sm">
                <p className="text-sm font-semibold">EM BREVE</p>
                <p className="mt-2 text-sm text-white/85">Disponível para Android e Apple</p>
                <div className="mt-6 flex flex-wrap gap-3">
                  <div className="rounded-lg bg-white/10 px-4 py-2 text-xs">Google Play</div>
                  <div className="rounded-lg bg-white/10 px-4 py-2 text-xs">App Store</div>
                </div>
              </div>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
