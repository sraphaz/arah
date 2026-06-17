import { ReactNode } from "react";

interface InfoBoxProps {
  type?: "info" | "success" | "warning" | "tip";
  title?: string;
  children: ReactNode;
}

export function InfoBox({ type = "info", title, children }: InfoBoxProps) {
  // Harmonizado com paleta Arah (tokens dark-accent / dark-link)
  const config = {
    info: {
      icon: "ℹ️",
      bg: "bg-link/10 dark:bg-dark-link/15",
      border: "border-link/50 dark:border-dark-link/60",
      text: "text-forest-900 dark:text-dark-text",
      titleColor: "text-forest-800 dark:text-dark-link",
    },
    success: {
      icon: "✅",
      bg: "bg-accent/10 dark:bg-dark-accent/15",
      border: "border-accent/50 dark:border-dark-accent/60",
      text: "text-forest-900 dark:text-dark-text",
      titleColor: "text-forest-800 dark:text-dark-accent",
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
