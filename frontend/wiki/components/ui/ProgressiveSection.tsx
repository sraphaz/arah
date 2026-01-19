"use client";

import { ReactNode, useState } from "react";

interface ProgressiveSectionProps {
  title: string;
  summary: string;
  children: ReactNode;
  defaultExpanded?: boolean;
  level?: "surface" | "intermediate" | "deep";
}

/**
 * Seção progressiva que revela mais detalhes ao clicar
 * Design limpo sem bordas excessivas
 */
export function ProgressiveSection({
  title,
  summary,
  children,
  defaultExpanded = false,
  level = "surface",
}: ProgressiveSectionProps) {
  const [isExpanded, setIsExpanded] = useState(defaultExpanded);

  const levelStyles = {
    surface: "border-forest-200/40 dark:border-forest-800/40",
    intermediate: "border-forest-300/60 dark:border-forest-700/60 border-l-2",
    deep: "border-forest-400/60 dark:border-forest-600/60 border-l-4",
  };

  return (
    <div
      className={`progressive-section my-6 rounded-lg transition-all duration-300 ${
        levelStyles[level]
      } ${isExpanded ? "border shadow-sm" : "border hover:shadow-sm"}`}
    >
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className={`w-full text-left p-5 transition-colors duration-200 ${
          isExpanded
            ? "bg-forest-50/50 dark:bg-forest-900/30"
            : "hover:bg-forest-50/30 dark:hover:bg-forest-900/20"
        }`}
        aria-expanded={isExpanded}
      >
        <div className="flex items-start justify-between gap-4">
          <div className="flex-1 min-w-0">
            <h3 className="text-lg font-semibold text-forest-900 dark:text-forest-50 mb-2">
              {title}
            </h3>
            <p className="text-sm text-forest-600 dark:text-forest-400 leading-relaxed">
              {summary}
            </p>
          </div>
          <svg
            className={`w-5 h-5 flex-shrink-0 text-forest-500 dark:text-forest-400 transition-transform duration-300 mt-1 ${
              isExpanded ? "rotate-180" : ""
            }`}
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={2}
          >
            <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
          </svg>
        </div>
      </button>
      <div
        className={`progressive-section-content overflow-hidden transition-all duration-300 ease-in-out ${
          isExpanded ? "max-h-[2000px] opacity-100" : "max-h-0 opacity-0"
        }`}
      >
        <div className="px-5 pb-5 pt-0 text-forest-700 dark:text-forest-300 prose prose-sm dark:prose-invert max-w-none">
          {children}
        </div>
      </div>
    </div>
  );
}
