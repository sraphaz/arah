/**
 * Adapter de tema do Mermaid — fonte única de cores para diagramas.
 *
 * Lê os design tokens (CSS custom properties de frontend/shared/styles/design-tokens.css
 * e aliases do globals.css) em runtime. Os literais abaixo existem apenas como fallback
 * SSR/teste e espelham os tokens; este arquivo é tratado como definição de tema pelo
 * design-gate-check (LIC-001/LIC-002).
 */

const TOKEN_FALLBACKS: Record<string, string> = {
  "--accent": "#4dd4a8",
  "--link": "#7dd3ff",
  "--color-dark-800": "#1e2830",
  "--color-dark-850": "#141a21",
  "--color-dark-900": "#0f1419",
  "--color-dark-950": "#0a0e12",
  "--color-neutral-0": "#ffffff",
};

function cssToken(name: string): string {
  if (typeof window !== "undefined") {
    const value = getComputedStyle(document.documentElement).getPropertyValue(name).trim();
    if (value) return value;
  }
  return TOKEN_FALLBACKS[name] ?? "";
}

/** themeVariables do Mermaid derivadas dos design tokens Arah. */
export function buildMermaidThemeVariables(): Record<string, string> {
  const accent = cssToken("--accent");
  const link = cssToken("--link");
  const surface = cssToken("--color-dark-900");
  const surfaceElevated = cssToken("--color-dark-850");
  const surfaceMuted = cssToken("--color-dark-800");
  const textOnDark = cssToken("--color-neutral-0");

  return {
    primaryColor: accent,
    primaryTextColor: textOnDark,
    primaryBorderColor: accent,
    lineColor: link,
    secondaryColor: surfaceElevated,
    tertiaryColor: surfaceMuted,
    background: surface,
    mainBkg: surfaceElevated,
    secondBkg: surfaceMuted,
    textColor: textOnDark,
    border1: accent,
    border2: link,
    noteBkgColor: surfaceElevated,
    noteTextColor: textOnDark,
    noteBorderColor: accent,
    actorBorder: accent,
    actorBkg: surfaceElevated,
    actorTextColor: textOnDark,
    actorLineColor: link,
    signalColor: link,
    signalTextColor: textOnDark,
    labelBoxBkgColor: surfaceElevated,
    labelBoxBorderColor: accent,
    labelTextColor: textOnDark,
    loopTextColor: textOnDark,
    activationBorderColor: accent,
    activationBkgColor: surfaceElevated,
    sequenceNumberColor: textOnDark,
  };
}
