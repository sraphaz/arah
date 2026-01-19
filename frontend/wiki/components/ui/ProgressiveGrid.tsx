"use client";

import { ReactNode, useState } from "react";

interface GridItem {
  id: string;
  title: string;
  summary: string;
  content: ReactNode;
  level?: "surface" | "intermediate" | "deep";
}

interface ProgressiveGridProps {
  items: GridItem[];
  columns?: 1 | 2 | 3 | 4;
  defaultExpanded?: string | null;
}

/**
 * Grid progressivo que revela conhecimento do superficial ao complexo
 * Design limpo sem excesso de bordas
 */
export function ProgressiveGrid({
  items,
  columns = 2,
  defaultExpanded = null,
}: ProgressiveGridProps) {
  const [expandedId, setExpandedId] = useState<string | null>(defaultExpanded);

  const gridCols = {
    1: "grid-cols-1",
    2: "grid-cols-1 md:grid-cols-2",
    3: "grid-cols-1 md:grid-cols-2 lg:grid-cols-3",
    4: "grid-cols-1 md:grid-cols-2 lg:grid-cols-4",
  };

  const levelColors = {
    surface: "bg-forest-50/50 dark:bg-forest-950/50 border-forest-200/60 dark:border-forest-800/60",
    intermediate: "bg-forest-100/50 dark:bg-forest-900/50 border-forest-300/60 dark:border-forest-700/60",
    deep: "bg-forest-100/80 dark:bg-forest-900/80 border-forest-400/60 dark:border-forest-600/60",
  };

  return (
    <div className={`grid ${gridCols[columns]} gap-4 my-8`}>
      {items.map((item) => {
        const isExpanded = expandedId === item.id;
        const levelColor = levelColors[item.level || "surface"];

        return (
          <div
            key={item.id}
            className={`progressive-grid-item rounded-lg transition-all duration-300 ${
              isExpanded
                ? `${levelColor} border-2 shadow-lg`
                : "border border-forest-200/40 dark:border-forest-800/40 hover:border-forest-300 dark:hover:border-forest-700 hover:shadow-md"
            }`}
          >
            <button
              onClick={() => setExpandedId(isExpanded ? null : item.id)}
              className={`w-full text-left p-5 transition-colors duration-200 ${
                isExpanded
                  ? "bg-forest-50/80 dark:bg-forest-900/80"
                  : "hover:bg-forest-50/50 dark:hover:bg-forest-900/30"
              }`}
              aria-expanded={isExpanded}
            >
              <div className="flex items-start justify-between gap-4">
                <div className="flex-1 min-w-0">
                  <h3 className="text-lg font-semibold text-forest-900 dark:text-forest-50 mb-2">
                    {item.title}
                  </h3>
                  <p className="text-sm text-forest-600 dark:text-forest-400 leading-relaxed">
                    {item.summary}
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
              className={`progressive-content overflow-hidden transition-all duration-300 ease-in-out ${
                isExpanded ? "max-h-[2000px] opacity-100" : "max-h-0 opacity-0"
              }`}
            >
              <div className="px-5 pb-5 pt-0 text-forest-700 dark:text-forest-300 prose prose-sm dark:prose-invert max-w-none">
                {typeof item.content === 'string' ? (
                  <div dangerouslySetInnerHTML={{ __html: item.content }} />
                ) : (
                  item.content
                )}
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
}
