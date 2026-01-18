"use client";

import { useEffect, useState } from "react";

export function ThemeToggle() {
  const [theme, setTheme] = useState<"light" | "dark">("dark");
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
    try {
      // Verificar preferência salva ou usar dark como padrão
      const savedTheme = localStorage.getItem("wiki-theme") as "light" | "dark" | null;
      // Padrão: dark mode (se não houver preferência salva)
      const initialTheme = savedTheme || "dark";
      
      setTheme(initialTheme);
      applyTheme(initialTheme);
      
      // Log para debug (apenas em desenvolvimento)
      if (process.env.NODE_ENV === 'development') {
        console.log('[ThemeToggle] Initial theme:', initialTheme, 'Saved:', savedTheme);
      }
    } catch (error) {
      // Fallback: dark mode em caso de erro
      console.error('[ThemeToggle] Error initializing theme:', error);
      setTheme("dark");
      applyTheme("dark");
    }
  }, []);

  const applyTheme = (newTheme: "light" | "dark") => {
    try {
      document.documentElement.classList.toggle("dark", newTheme === "dark");
      localStorage.setItem("wiki-theme", newTheme);
      
      // Log para debug (apenas em desenvolvimento)
      if (process.env.NODE_ENV === 'development') {
        console.log('[ThemeToggle] Applied theme:', newTheme);
      }
    } catch (error) {
      console.error('[ThemeToggle] Error applying theme:', error);
      // Tenta aplicar dark mode como fallback
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

  // Evitar hidratação mismatch
  if (!mounted) {
    return (
      <button
        className="w-10 h-10 rounded-xl bg-forest-100 text-forest-700 hover:bg-forest-200 transition-colors"
        aria-label="Alternar tema"
        disabled
      >
        <span className="text-lg">🌓</span>
      </button>
    );
  }

  return (
    <button
      onClick={toggleTheme}
      className="w-10 h-10 rounded-xl bg-forest-100 dark:bg-forest-900 text-forest-700 dark:text-forest-200 hover:bg-forest-200 dark:hover:bg-forest-800 transition-all duration-300 flex items-center justify-center shadow-sm hover:shadow-md"
      aria-label={theme === "light" ? "Ativar modo escuro" : "Ativar modo claro"}
      title={theme === "light" ? "Modo escuro" : "Modo claro"}
    >
      <span className="text-lg transition-transform duration-300 transform">
        {theme === "light" ? "🌙" : "☀️"}
      </span>
    </button>
  );
}
