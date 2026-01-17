"use client";

import { useEffect, useState } from "react";

export function ThemeToggle() {
  const [theme, setTheme] = useState<"light" | "dark">("light");
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
    // Verificar preferência salva ou preferência do sistema
    const savedTheme = localStorage.getItem("wiki-theme") as "light" | "dark" | null;
    const systemPrefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
    const initialTheme = savedTheme || (systemPrefersDark ? "dark" : "light");
    setTheme(initialTheme);
    applyTheme(initialTheme);
  }, []);

  const applyTheme = (newTheme: "light" | "dark") => {
    document.documentElement.classList.toggle("dark", newTheme === "dark");
    localStorage.setItem("wiki-theme", newTheme);
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
