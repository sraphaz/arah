import { ReactNode } from "react";

interface InfoBoxProps {
  type?: "info" | "success" | "warning" | "tip";
  title?: string;
  children: ReactNode;
}

export function InfoBox({ type = "info", title, children }: InfoBoxProps) {
  const config = {
    info: {
      icon: "ℹ️",
      bg: "bg-blue-50/80 dark:bg-blue-900/20",
      border: "border-blue-400 dark:border-blue-600",
      text: "text-blue-900 dark:text-blue-100",
      titleColor: "text-blue-800 dark:text-blue-200",
    },
    success: {
      icon: "✅",
      bg: "bg-forest-50/80 dark:bg-forest-900/20",
      border: "border-forest-400 dark:border-forest-600",
      text: "text-forest-900 dark:text-forest-100",
      titleColor: "text-forest-800 dark:text-forest-200",
    },
    warning: {
      icon: "⚠️",
      bg: "bg-orange-50/80 dark:bg-orange-900/20",
      border: "border-orange-400 dark:border-orange-600",
      text: "text-orange-900 dark:text-orange-100",
      titleColor: "text-orange-800 dark:text-orange-200",
    },
    tip: {
      icon: "💡",
      bg: "bg-purple-50/80 dark:bg-purple-900/20",
      border: "border-purple-400 dark:border-purple-600",
      text: "text-purple-900 dark:text-purple-100",
      titleColor: "text-purple-800 dark:text-purple-200",
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
