import Image from "next/image";
import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import Section from "@/components/ui/Section";

export default function Support() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="grid gap-12 md:grid-cols-[1.1fr_0.9fr] md:items-center">
            <div className="space-y-6">
              <div className="space-y-3">
                <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                  Apoie o desenvolvimento
                </h2>
                <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                  O Araponga é um projeto de código aberto e conta com o apoio da comunidade para
                  continuar crescendo e fortalecendo comunidades locais.
                </p>
              </div>
              <div className="space-y-4">
                <p className="text-base font-semibold text-forest-900">
                  Sua contribuição ajuda a:
                </p>
                <div className="grid gap-4">
                  <div className="flex items-start gap-3 rounded-xl border border-white/60 bg-white/50 p-4">
                    <span className="text-2xl">🛠️</span>
                    <div>
                      <p className="text-sm font-semibold text-forest-900">Manter a infraestrutura</p>
                      <p className="text-xs text-forest-700 mt-1">Servidores, domínios e serviços essenciais</p>
                    </div>
                  </div>
                  <div className="flex items-start gap-3 rounded-xl border border-white/60 bg-white/50 p-4">
                    <span className="text-2xl">✨</span>
                    <div>
                      <p className="text-sm font-semibold text-forest-900">Desenvolver novas funcionalidades</p>
                      <p className="text-xs text-forest-700 mt-1">Recursos que fortalecem comunidades</p>
                    </div>
                  </div>
                  <div className="flex items-start gap-3 rounded-xl border border-white/60 bg-white/50 p-4">
                    <span className="text-2xl">🌱</span>
                    <div>
                      <p className="text-sm font-semibold text-forest-900">Alcançar mais comunidades</p>
                      <p className="text-xs text-forest-700 mt-1">Expansão e impacto territorial</p>
                    </div>
                  </div>
                </div>
              </div>
              <div className="rounded-2xl border border-forest-200/60 bg-forest-50/50 p-5">
                <p className="text-sm font-semibold text-forest-900 mb-4">Contribua via PIX</p>
                <a
                  href="https://geradordepix.com/pagamento/01KESFV2Q5P6V6K2JWWBY340EK"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="inline-flex items-center justify-center gap-2 rounded-lg bg-forest-600 px-6 py-3 text-base font-semibold text-white transition-colors hover:bg-forest-700 focus:outline-none focus:ring-2 focus:ring-forest-500 focus:ring-offset-2"
                >
                  <span>Contribuir via PIX</span>
                  <svg
                    className="h-4 w-4"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    strokeWidth={2}
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"
                    />
                  </svg>
                </a>
              </div>
            </div>
            <div className="flex">
              <Image
                src="/app_banner.png"
                alt=""
                width={800}
                height={600}
                className="w-full rounded-3xl object-contain shadow-lg transition-transform duration-300 hover:scale-105"
              />
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
