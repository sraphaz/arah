import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Hero() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="grid gap-10 md:grid-cols-[1.05fr_0.95fr] md:items-center">
            <div className="max-w-xl space-y-4">
              <h1 className="text-4xl font-semibold tracking-tight text-forest-950 md:text-5xl">
                ARAPONGA
              </h1>
              <p className="text-lg leading-relaxed text-forest-800">
                <strong>Território-Primeiro &amp; Comunidade-Primeiro</strong>
                <br />
                Plataforma orientada ao território para organização comunitária local
              </p>
            </div>
            <div className="flex justify-center md:justify-end">
              <img
                src="/bukeh.jpg"
                alt=""
                className="h-56 w-auto rounded-2xl object-cover shadow-lg md:h-72"
              />
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
