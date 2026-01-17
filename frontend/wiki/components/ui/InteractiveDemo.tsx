"use client";

import { ReactNode, useState } from "react";

interface InteractiveDemoProps {
  title: string;
  description: string;
  children: ReactNode;
  defaultOpen?: boolean;
}

export function InteractiveDemo({ title, description, children, defaultOpen = false }: InteractiveDemoProps) {
  const [isOpen, setIsOpen] = useState(defaultOpen);

  return (
    <div className="interactive-demo my-8 border-2 border-forest-300 dark:border-forest-700 rounded-xl overflow-hidden bg-forest-50/50 dark:bg-forest-900/30">
      <div className="p-6 border-b border-forest-200 dark:border-forest-800">
        <div className="flex items-start justify-between gap-4">
          <div className="flex-1">
            <h4 className="text-lg font-bold text-forest-900 dark:text-forest-50 mb-2">{title}</h4>
            <p className="text-sm text-forest-700 dark:text-forest-300">{description}</p>
          </div>
          <button
            onClick={() => setIsOpen(!isOpen)}
            className="px-4 py-2 text-sm font-medium rounded-lg bg-forest-600 dark:bg-[#4dd4a8] text-white dark:text-forest-950 hover:bg-forest-700 dark:hover:bg-[#5ee5b9] transition-colors duration-200 flex-shrink-0"
          >
            {isOpen ? "Ocultar" : "Explorar"}
          </button>
        </div>
      </div>
      <div
        className={`interactive-content overflow-hidden transition-all duration-500 ease-in-out ${
          isOpen ? "max-h-[2000px] opacity-100" : "max-h-0 opacity-0"
        }`}
      >
        <div className="p-6 bg-white dark:bg-forest-950">{children}</div>
      </div>
    </div>
  );
}
