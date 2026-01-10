import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Join() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-6">
            <h2 className="text-2xl font-semibold tracking-tight text-forest-950">
              Faça parte dessa transformação
            </h2>
            <ul className="list-disc space-y-2 pl-5 text-base leading-relaxed text-forest-800">
              <li>Explore o código e a documentação no GitHub</li>
              <li>Teste a plataforma em sua comunidade</li>
              <li>Contribua com ideias, código ou feedback</li>
            </ul>
            <p className="text-base font-semibold text-forest-900">
              <strong>Território primeiro. Comunidade primeiro. Vida primeiro.</strong>
            </p>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
