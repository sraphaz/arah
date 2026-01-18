import Image from "next/image";
import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Hero() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="grid gap-12 md:grid-cols-[1.05fr_0.95fr] md:items-center">
            <div className="max-w-2xl space-y-6">
              <h1 className="text-5xl font-bold tracking-tight text-forest-950 md:text-6xl lg:text-7xl">
                ARAPONGA
              </h1>
              <p className="text-xl leading-relaxed text-forest-800 md:text-2xl">
                <strong className="font-semibold text-forest-900">Território-Primeiro &amp; Comunidade-Primeiro</strong>
                <br />
                <span className="text-forest-700">Plataforma orientada ao território para organização comunitária local</span>
              </p>
            </div>
            <div className="flex">
              <Image
                src="/first_rght_side_cover.png"
                alt=""
                width={800}
                height={600}
                className="w-full rounded-3xl object-contain shadow-lg md:max-h-96 transition-transform duration-300 hover:scale-105"
                priority
              />
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
