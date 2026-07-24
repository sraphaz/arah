import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./app/**/*.{ts,tsx}",
    "./components/**/*.{ts,tsx}"
  ],
  theme: {
    extend: {
      // Paleta em hex (não var(--x)): utilitários com modificador de opacidade
      // (ex.: bg-forest-600/90) exigem valores concretos decomponíveis em canais RGB.
      // forest.50 == --color-forest-50 e forest.200 == --color-forest-200; a escala
      // completa (100/300/400/500/600/700/800/900) é a paleta canônica da UI
      // wiki/portal e deve permanecer idêntica ao tailwind.config.ts da wiki.
      // keep in sync with design-tokens.css
      colors: {
        forest: {
          50: "#F1F8F4",
          100: "#E2F1E8",
          200: "#C6E3D2",
          300: "#9FCEB4",
          400: "#6FB28C",
          500: "#4F956F",
          600: "#377B57",
          700: "#2B6246",
          800: "#214D37",
          900: "#173525"
        }
      }
    }
  },
  plugins: []
};

export default config;
