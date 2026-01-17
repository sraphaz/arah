import { ReactNode } from "react";

interface InfoBoxProps {
  type?: "info" | "success" | "warning" | "tip";
  title?: string;
  children: ReactNode;
}

export function InfoBox({ type = "info", title, children }: InfoBoxProps) {
  // Harmonizado com paleta Araponga (Dev Portal)
  const config = {
    info: {
      icon: "ℹ️",
      bg: "bg-[#7dd3ff]/10 dark:bg-[#7dd3ff]/15",
      border: "border-[#7dd3ff]/50 dark:border-[#7dd3ff]/60",
      text: "text-forest-900 dark:text-[#e8edf2]",
      titleColor: "text-forest-800 dark:text-[#7dd3ff]",
    },
    success: {
      icon: "✅",
      bg: "bg-[#4dd4a8]/10 dark:bg-[#4dd4a8]/15",
      border: "border-[#4dd4a8]/50 dark:border-[#4dd4a8]/60",
      text: "text-forest-900 dark:text-[#e8edf2]",
      titleColor: "text-forest-800 dark:text-[#4dd4a8]",
    },
    warning: {
      icon: "⚠️",
      bg: "bg-forest-50/80 dark:bg-forest-900/30",
      border: "border-forest-400 dark:border-forest-600",
      text: "text-forest-900 dark:text-forest-100",
      titleColor: "text-forest-800 dark:text-forest-200",
    },
    tip: {
      icon: "💡",
      bg: "bg-forest-50/80 dark:bg-forest-900/30",
      border: "border-forest-500 dark:border-forest-500",
      text: "text-forest-900 dark:text-forest-100",
      titleColor: "text-forest-800 dark:text-forest-200",
    },
  };

  const style = config[type];

  return (
    <div className={`${style.bg} ${style.border} border-l-4 rounded-r-xl p-6 my-6 shadow-sm hover:shadow-md transition-shadow duration-300`}>
      <div className="flex items-start gap-4">
        <div className="text-2xl flex-shrink-0 animate-bounce-slow">{style.icon}</div>
        <div className="flex-1">
          {title && (
            <h4 className={`${style.titleColor} font-bold text-lg mb-2`}>
              {title}
            </h4>
          )}
          <div className={style.text}>
            {children}
          </div>
        </div>
      </div>
    </div>
  );
}
