import { ReactNode } from "react";
import Link from "next/link";

interface FeatureCardProps {
  icon: ReactNode;
  title: string;
  description: string;
  color?: "forest" | "blue" | "purple" | "orange";
  href?: string;
}

export function FeatureCard({ icon, title, description, color = "forest", href }: FeatureCardProps) {
  const colorClasses = {
    forest: "border-forest-400/30 hover:border-forest-400 bg-forest-50/50 dark:bg-forest-900/30 hover:bg-forest-100/70 dark:hover:bg-forest-900/50",
    blue: "border-blue-400/30 hover:border-blue-400 bg-blue-50/50 dark:bg-blue-900/30 hover:bg-blue-100/70 dark:hover:bg-blue-900/50",
    purple: "border-purple-400/30 hover:border-purple-400 bg-purple-50/50 dark:bg-purple-900/30 hover:bg-purple-100/70 dark:hover:bg-purple-900/50",
    orange: "border-orange-400/30 hover:border-orange-400 bg-orange-50/50 dark:bg-orange-900/30 hover:bg-orange-100/70 dark:hover:bg-orange-900/50",
  };

  const iconColorClasses = {
    forest: "text-forest-600 dark:text-forest-400",
    blue: "text-blue-600 dark:text-blue-400",
    purple: "text-purple-600 dark:text-purple-400",
    orange: "text-orange-600 dark:text-orange-400",
  };

  const cardContent = (
    <div className={`glass-card group relative overflow-hidden border-2 ${colorClasses[color]} transition-all duration-500 hover:scale-[1.02] hover:-translate-y-1 hover:shadow-xl`}>
      <div className="glass-card__content relative z-10">
        {/* Decorative background pattern */}
        <div className="absolute inset-0 opacity-5 group-hover:opacity-10 transition-opacity duration-500 pointer-events-none">
          <div className="absolute inset-0" style={{
            backgroundImage: `radial-gradient(circle at 2px 2px, currentColor 1px, transparent 0)`,
            backgroundSize: '24px 24px'
          }}></div>
        </div>

        {/* Icon with animated background */}
        <div className={`relative inline-flex items-center justify-center w-16 h-16 rounded-2xl ${colorClasses[color]} mb-4 group-hover:scale-110 transition-transform duration-500`}>
          <div className={`text-3xl ${iconColorClasses[color]} relative z-10 group-hover:rotate-12 transition-transform duration-500`}>
            {icon}
          </div>
          {/* Pulsing ring effect */}
          <div className={`absolute inset-0 rounded-2xl ${colorClasses[color]} opacity-0 group-hover:opacity-100 group-hover:animate-ping pointer-events-none`}></div>
        </div>

        <h3 className="text-xl md:text-2xl font-bold text-forest-900 dark:text-forest-50 mb-2 group-hover:text-forest-700 dark:group-hover:text-forest-200 transition-colors">
          {title}
        </h3>
        <p className="text-forest-600 dark:text-forest-300 text-sm md:text-base leading-relaxed">
          {description}
        </p>

        {/* Arrow indicator */}
        <div className="mt-4 flex items-center text-sm font-medium opacity-0 group-hover:opacity-100 translate-x-0 group-hover:translate-x-2 transition-all duration-300">
          <span className={iconColorClasses[color]}>Explorar</span>
          <span className={`ml-2 ${iconColorClasses[color]}`}>→</span>
        </div>
      </div>
    </div>
  );

  if (href) {
    return (
      <Link href={href} className="block">
        {cardContent}
      </Link>
    );
  }

  return cardContent;
}
