"use client";

import { ReactNode, useState } from "react";

interface ExpandableCardProps {
  title: string;
  summary: string;
  children: ReactNode;
  icon?: string;
  color?: "forest" | "accent" | "link";
}

export function ExpandableCard({ title, summary, children, icon, color = "forest" }: ExpandableCardProps) {
  const [isExpanded, setIsExpanded] = useState(false);

  const colorClasses = {
    forest: "border-forest-300 dark:border-forest-700 bg-forest-50 dark:bg-forest-900/50",
    accent: "border-[#4dd4a8]/30 dark:border-[#4dd4a8]/50 bg-[#4dd4a8]/5 dark:bg-[#4dd4a8]/10",
    link: "border-[#7dd3ff]/30 dark:border-[#7dd3ff]/50 bg-[#7dd3ff]/5 dark:bg-[#7dd3ff]/10",
  };

  const iconColorClasses = {
    forest: "text-forest-600 dark:text-forest-400",
    accent: "text-[#4dd4a8] dark:text-[#5ee5b9]",
    link: "text-[#7dd3ff] dark:text-[#9de3ff]",
  };

  return (
    <div
      className={`expandable-card border-2 rounded-xl overflow-hidden transition-all duration-300 hover:shadow-lg ${colorClasses[color]}`}
    >
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className="w-full flex items-start justify-between p-6 text-left hover:bg-forest-100/50 dark:hover:bg-forest-900/50 transition-colors duration-200"
        aria-expanded={isExpanded}
      >
        <div className="flex-1">
          <div className="flex items-center gap-3 mb-2">
            {icon && <span className={`text-2xl ${iconColorClasses[color]}`}>{icon}</span>}
            <h3 className="text-xl font-bold text-forest-900 dark:text-forest-50">{title}</h3>
          </div>
          <p className="text-forest-700 dark:text-forest-300 text-sm leading-relaxed">{summary}</p>
        </div>
        <svg
          className={`w-6 h-6 ml-4 flex-shrink-0 ${iconColorClasses[color]} transition-transform duration-300 ${
            isExpanded ? "rotate-180" : ""
          }`}
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
        </svg>
      </button>
      <div
        className={`expandable-content overflow-hidden transition-all duration-300 ease-in-out ${
          isExpanded ? "max-h-[2000px] opacity-100" : "max-h-0 opacity-0"
        }`}
      >
        <div className="px-6 pb-6 pt-0 text-forest-700 dark:text-forest-200 border-t border-forest-200/60 dark:border-forest-800/60">
          {children}
        </div>
      </div>
    </div>
  );
}
