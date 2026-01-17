import type { Config } from "tailwindcss";

const config: Config = {
  darkMode: 'class',
  content: [
    "./app/**/*.{ts,tsx}",
    "./components/**/*.{ts,tsx}"
  ],
  theme: {
    extend: {
      colors: {
        // Identidade visual Araponga - Paleta Forest
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
          900: "#173525",
          950: "#0a0e12" // Dark mode background (devportal)
        },
        // Cores do devportal para dark mode
        dark: {
          bg: "#0a0e12",
          "bg-elevated": "#0f1419",
          "bg-card": "#141a21",
          "bg-muted": "#1a2129",
          border: "#25303a",
          "border-subtle": "#1e2830",
          text: "#e8edf2",
          "text-muted": "#b8c5d2",
          "text-subtle": "#8a97a4",
          accent: "#4dd4a8",
          "accent-hover": "#5ee5b9",
          link: "#7dd3ff",
          "link-hover": "#9de3ff"
        }
      },
      fontFamily: {
        sans: ['var(--font-sora)', 'system-ui', 'sans-serif']
      }
    }
  },
  plugins: [
    require('@tailwindcss/typography')
  ],
  // Configurações de typography refinadas
  typography: {
    DEFAULT: {
      css: {
        maxWidth: 'none',
        color: 'rgb(21, 53, 37)',
        fontSize: '1.125rem',
        lineHeight: '1.8',
        '--tw-prose-headings': 'rgb(23, 53, 37)',
        '--tw-prose-links': 'rgb(55, 123, 87)',
        '--tw-prose-bold': 'rgb(23, 53, 37)',
        '--tw-prose-code': 'rgb(23, 53, 37)',
        '--tw-prose-pre-code': 'rgb(241, 248, 244)',
        '--tw-prose-pre-bg': 'rgb(23, 53, 37)',
        '--tw-prose-quotes': 'rgb(43, 98, 70)',
        '--tw-prose-quote-borders': 'rgb(79, 149, 111)',
      },
    },
  },
};

export default config;
