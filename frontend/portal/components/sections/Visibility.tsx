import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Visibility() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-3">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                Regras de visibilidade: por que isso importa
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                O sistema de visibilidade do Araponga garante que comunidades tenham controle sobre
                suas informações, promovendo segurança e confiança.
              </p>
            </div>
            <div className="grid gap-8 md:grid-cols-2">
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h3 className="text-base font-semibold text-forest-900">Visitante &amp; Residente</h3>
                <p className="text-sm leading-relaxed text-forest-800">
                  A distinção entre visitante e residente permite que comunidades controlem o
                  acesso a informações sensíveis ou restritas.
                </p>
                <ul className="list-disc space-y-2 pl-5 text-sm text-forest-800">
                  <li>
                    <strong>Visitante</strong>: acesso apenas a conteúdo marcado como público
                  </li>
                  <li>
                    <strong>Residente</strong>: acesso ampliado, incluindo conteúdos restritos do
                    território
                  </li>
                </ul>
                <p className="text-sm leading-relaxed text-forest-800">
                  Essa separação respeita a autonomia comunitária e permite gradações de confiança
                  baseadas em engajamento real.
                </p>
              </div>
              <div className="space-y-4 rounded-2xl border border-white/60 bg-white/65 p-5">
                <h3 className="text-base font-semibold text-forest-900">Público &amp; Restrito</h3>
                <p className="text-sm leading-relaxed text-forest-800">
                  Cada publicação, evento ou conteúdo pode ser classificado como público ou
                  restrito.
                </p>
                <p className="text-sm leading-relaxed text-forest-800">
                  O sistema garante que apenas usuários com permissão adequada tenham acesso a
                  informações sensíveis.
                  <br />
                  Isso é essencial para:
                </p>
                <ul className="list-disc space-y-2 pl-5 text-sm text-forest-800">
                  <li>proteger discussões internas</li>
                  <li>planejar ações comunitárias sem exposição desnecessária</li>
                  <li>construir ambientes de confiança</li>
                </ul>
                <p className="text-sm leading-relaxed text-forest-800">
                  Essas regras não são arbitrárias.
                  <br />
                  Elas promovem <strong>segurança, relevância e respeito às dinâmicas locais</strong>
                  .
                </p>
              </div>
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
