import type { ReactNode } from "react";

interface GlassCardProps {
  children: ReactNode;
  className?: string;
  tone?: "light" | "dense";
}

export default function GlassCard({ children, className, tone = "light" }: GlassCardProps) {
  const toneClass = tone === "dense" ? "glass-card glass-card--dense" : "glass-card";

  return (
    <div className={`${toneClass} ${className ?? ""}`.trim()}>
      <div className="glass-card__content">{children}</div>
    </div>
  );
}
