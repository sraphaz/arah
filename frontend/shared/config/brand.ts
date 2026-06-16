/**
 * Configuração central de marca e URLs do ecossistema Arah.
 * Wiki, Portal e DevPortal devem importar daqui (DRY).
 */
export const brand = {
  name: "Arah",
  wikiTitle: "Wiki Arah",
  tagline: "Documentação Completa",
  description:
    "Documentação completa do Arah: visão do produto, arquitetura, desenvolvimento, onboarding e mais.",
  urls: {
    site: "https://arah.app",
    devportal: "https://devportal.arah.app",
    wiki: "https://wiki.arah.app",
    github: "https://github.com/sraphaz/arah",
    discord: "https://discord.gg/auwqN8Yjgw",
  },
} as const;

export type BrandConfig = typeof brand;
