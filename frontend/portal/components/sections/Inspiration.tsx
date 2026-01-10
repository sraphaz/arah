import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Inspiration() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-6">
            <h2 className="text-2xl font-semibold text-forest-950">O Pássaro que nos inspira</h2>
            <div className="space-y-4 text-base leading-relaxed text-forest-800">
              <p>
                O Araponga, também conhecido como <strong>“pássaro-ferreiro”</strong>, é famoso por
                seu canto metálico e ressonante nas florestas brasileiras.
                <br />
                Sua presença vibrante simboliza a força e a resiliência da natureza.
              </p>
              <p>
                A escolha do Araponga reflete nosso compromisso com a autenticidade e a comunicação
                clara.
                <br />
                Assim como ele se destaca em seu território, buscamos valorizar as comunidades
                locais e suas vozes singulares.
              </p>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
