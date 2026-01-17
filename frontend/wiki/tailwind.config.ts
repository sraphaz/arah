import type { Config } from "tailwindcss";

const config: Config = {
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
          900: "#173525"
        }
      },
      fontFamily: {
        sans: ['var(--font-sora)', 'system-ui', 'sans-serif']
      }
    }
  },
  plugins: [
    require('@tailwindcss/typography')
  ]
};

export default config;
