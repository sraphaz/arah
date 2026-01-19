import { ReactNode } from "react";
import Link from "next/link";

interface FeatureCardProps {
  icon: ReactNode;
  title: string;
  description: string;
  color?: "forest" | "accent" | "link";
  href?: string;
}

export function FeatureCard({ icon, title, description, color = "forest", href }: FeatureCardProps) {
  // Harmonizado com a paleta do Dev Portal Araponga - sem bordas excessivas
  const colorClasses = {
    forest: "bg-forest-50/50 dark:bg-forest-900/30 hover:bg-forest-100/70 dark:hover:bg-forest-900/50",
    accent: "bg-[#4dd4a8]/5 dark:bg-[#4dd4a8]/10 hover:bg-[#4dd4a8]/10 dark:hover:bg-[#4dd4a8]/15",
    link: "bg-[#7dd3ff]/5 dark:bg-[#7dd3ff]/10 hover:bg-[#7dd3ff]/10 dark:hover:bg-[#7dd3ff]/15",
  };

  const iconColorClasses = {
    forest: "text-forest-600 dark:text-forest-400",
    accent: "text-[#4dd4a8] dark:text-[#5ee5b9]",
    link: "text-[#7dd3ff] dark:text-[#9de3ff]",
  };

  const cardContent = (
    <div className={`glass-card group relative overflow-hidden ${colorClasses[color]} transition-all duration-500 hover:scale-[1.02] hover:-translate-y-1 hover:shadow-xl`}>
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
