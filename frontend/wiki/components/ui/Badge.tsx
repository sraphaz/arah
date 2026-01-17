import { ReactNode } from "react";

interface BadgeProps {
  children: ReactNode;
  variant?: "default" | "success" | "warning" | "info" | "primary";
  size?: "sm" | "md" | "lg";
  pulse?: boolean;
}

export function Badge({ children, variant = "default", size = "md", pulse = false }: BadgeProps) {
  const variants = {
    default: "bg-forest-100 dark:bg-forest-900 text-forest-700 dark:text-forest-200 border-forest-200 dark:border-forest-800",
    success: "bg-green-100 dark:bg-green-900 text-green-700 dark:text-green-200 border-green-300 dark:border-green-700",
    warning: "bg-orange-100 dark:bg-orange-900 text-orange-700 dark:text-orange-200 border-orange-300 dark:border-orange-700",
    info: "bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-200 border-blue-300 dark:border-blue-700",
    primary: "bg-forest-600 dark:bg-forest-500 text-white dark:text-forest-50 border-forest-700 dark:border-forest-400",
  };

  const sizes = {
    sm: "text-xs px-2.5 py-1",
    md: "text-sm px-3 py-1.5",
    lg: "text-base px-4 py-2",
  };

  return (
    <span
      className={`inline-flex items-center rounded-full border font-medium ${variants[variant]} ${sizes[size]} transition-all duration-300 hover:scale-110 ${
        pulse ? "animate-pulse" : ""
      }`}
    >
      {children}
    </span>
  );
}
