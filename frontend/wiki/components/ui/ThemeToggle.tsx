"use client";

import { useEffect, useState } from "react";

// SVG Icons estáticos para evitar flash de emoji
const MoonIcon = () => (
  <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
  </svg>
);

const SunIcon = () => (
  <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
  </svg>
);

export function ThemeToggle() {
  const [theme, setTheme] = useState<"light" | "dark">("dark");
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
    try {
      const savedTheme = localStorage.getItem("wiki-theme") as "light" | "dark" | null;
      const initialTheme = savedTheme || "dark";
      
      setTheme(initialTheme);
      applyTheme(initialTheme);
    } catch (error) {
      console.error('[ThemeToggle] Error initializing theme:', error);
      setTheme("dark");
      applyTheme("dark");
    }
  }, []);

  const applyTheme = (newTheme: "light" | "dark") => {
    try {
      document.documentElement.classList.toggle("dark", newTheme === "dark");
      localStorage.setItem("wiki-theme", newTheme);
    } catch (error) {
      console.error('[ThemeToggle] Error applying theme:', error);
      try {
        document.documentElement.classList.add("dark");
      } catch (fallbackError) {
        console.error('[ThemeToggle] Fallback also failed:', fallbackError);
      }
    }
  };

  const toggleTheme = () => {
    const newTheme = theme === "light" ? "dark" : "light";
    setTheme(newTheme);
    applyTheme(newTheme);
  };

  // Durante SSR, renderiza com dark mode padrão (sem flash)
  if (!mounted) {
    return (
      <button
        className="w-10 h-10 rounded-xl bg-forest-100 dark:bg-forest-900 text-forest-700 dark:text-forest-200 transition-colors flex items-center justify-center"
        aria-label="Alternar tema"
        disabled
        style={{ opacity: 1 }}
      >
        <SunIcon />
      </button>
    );
  }

  return (
    <button
      onClick={toggleTheme}
      className="w-10 h-10 rounded-xl bg-forest-100 dark:bg-forest-900 text-forest-700 dark:text-forest-200 hover:bg-forest-200 dark:hover:bg-forest-800 transition-all duration-300 flex items-center justify-center shadow-sm hover:shadow-md"
      aria-label={theme === "light" ? "Ativar modo escuro" : "Ativar modo claro"}
      title={theme === "light" ? "Modo escuro" : "Modo claro"}
      style={{ opacity: 1 }}
    >
      {theme === "light" ? <MoonIcon /> : <SunIcon />}
    </button>
  );
}
