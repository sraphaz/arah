import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Support() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="grid gap-10 md:grid-cols-[1.1fr_0.9fr] md:items-center">
            <div className="space-y-5">
              <h2 className="text-2xl font-semibold tracking-tight text-forest-950">
                Apoie o desenvolvimento
              </h2>
              <p className="text-base leading-relaxed text-forest-800">
                O Araponga é um projeto de código aberto e conta com o apoio da comunidade para
                continuar crescendo.
              </p>
              <p className="text-base leading-relaxed text-forest-800">Sua contribuição ajuda a:</p>
              <ul className="list-disc space-y-2 pl-5 text-base leading-relaxed text-forest-800">
                <li>manter a infraestrutura</li>
                <li>desenvolver novas funcionalidades</li>
                <li>alcançar mais comunidades</li>
              </ul>
              <p className="text-base font-semibold text-forest-900">
                <strong>PIX:</strong> rapha.sos@gmail.com
              </p>
            </div>
            <div className="flex justify-center md:justify-end">
              <img
                src="/app_banner.png"
                alt=""
                className="w-full max-w-md rounded-2xl object-cover shadow-lg"
              />
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
