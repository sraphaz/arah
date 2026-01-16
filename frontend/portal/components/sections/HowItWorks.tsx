import GlassCard from "@/components/ui/GlassCard";
import RevealOnScroll from "@/components/ui/RevealOnScroll";
import type { ReactNode } from "react";

import Section from "@/components/ui/Section";

const STEPS: { number: string; title: string; body: ReactNode[] }[] = [
  {
    number: "01",
    title: "Login e autenticação",
    body: [
      "Usuário faz login via sistema de autenticação.",
      "A identidade é verificada, mas ainda não há vínculo com um território específico."
    ].map((line) => <p key={line}>{line}</p>)
  },
  {
    number: "02",
    title: "Escolher um território",
    body: [
      "Usuário seleciona o território de interesse na plataforma.",
      "Cada território possui regras próprias de participação e visibilidade."
    ].map((line) => <p key={line}>{line}</p>)
  },
  {
    number: "03",
    title: "Visitante ou Residente",
    body: [
      <p key="intro">O sistema atribui o papel inicial:</p>,
      <p key="visitante" className="ml-4">
        <strong>Visitante</strong>: acesso limitado
      </p>,
      <p key="residente" className="ml-4">
        <strong>Residente</strong>: acesso ampliado
      </p>,
      <p key="residencia">A residência pode ser solicitada conforme regras locais.</p>
    ]
  },
  {
    number: "04",
    title: "Feed e mapa filtrados",
    body: [
      <p key="1">O conteúdo exibido respeita o papel do usuário.</p>,
      <p key="2">Visitantes veem apenas conteúdo público.</p>,
      <p key="3">Residentes acessam conteúdos restritos.</p>,
      <p key="4">
        Tudo aparece tanto no <strong>feed</strong> quanto no <strong>mapa</strong>.
      </p>
    ]
  }
];

export default function HowItWorks() {
  return (
    <Section>
      <RevealOnScroll>
        <GlassCard>
          <div className="space-y-8">
            <div className="space-y-4">
              <h2 className="text-3xl font-semibold tracking-tight text-forest-950 md:text-4xl">
                Como funciona na prática
              </h2>
              <p className="text-base leading-relaxed text-forest-800 md:text-lg">
                O fluxo de uso foi desenhado para ser direto e respeitar as regras de cada
                território desde o primeiro acesso.
              </p>
            </div>
            <div className="relative space-y-8 border-l border-forest-300/60 pl-6">
              {STEPS.map((step, index) => (
                <RevealOnScroll
                  key={step.title}
                  delay={index * 120}
                  className="reveal--timeline"
                >
                  <div className="relative space-y-3 rounded-2xl border border-white/60 bg-white/70 p-5">
                    <span className="absolute -left-9 top-6 flex h-10 w-10 items-center justify-center rounded-full border border-forest-300/70 bg-white text-sm font-semibold text-forest-800">
                      {step.number}
                    </span>
                    <h3 className="text-base font-semibold text-forest-900">{step.title}</h3>
                    <div className="space-y-2 text-sm leading-relaxed text-forest-800">
                      {step.body}
                    </div>
                  </div>
                </RevealOnScroll>
              ))}
            </div>
          </div>
        </GlassCard>
      </RevealOnScroll>
    </Section>
  );
}
