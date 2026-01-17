"use client";

import { ReactNode, useState } from "react";

interface AccordionProps {
  title: string;
  children: ReactNode;
  defaultOpen?: boolean;
  icon?: string;
}

export function Accordion({ title, children, defaultOpen = false, icon }: AccordionProps) {
  const [isOpen, setIsOpen] = useState(defaultOpen);

  return (
    <div className="accordion-container border border-forest-200 dark:border-forest-800 rounded-xl overflow-hidden my-4">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="accordion-trigger w-full flex items-center justify-between px-6 py-4 text-left hover:bg-forest-50 dark:hover:bg-forest-900/50 transition-colors duration-200"
        aria-expanded={isOpen}
      >
        <div className="flex items-center gap-3">
          {icon && <span className="text-xl">{icon}</span>}
          <span className="font-semibold text-forest-900 dark:text-forest-50">{title}</span>
        </div>
        <svg
          className={`w-5 h-5 text-forest-600 dark:text-forest-400 transition-transform duration-300 ${isOpen ? "rotate-180" : ""}`}
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
        </svg>
      </button>
      <div
        className={`accordion-content overflow-hidden transition-all duration-300 ease-in-out ${
          isOpen ? "max-h-[2000px] opacity-100" : "max-h-0 opacity-0"
        }`}
      >
        <div className="px-6 py-4 text-forest-700 dark:text-forest-200 border-t border-forest-200 dark:border-forest-800">
          {children}
        </div>
      </div>
    </div>
  );
}
