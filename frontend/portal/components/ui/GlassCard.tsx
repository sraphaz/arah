import type { ReactNode } from "react";

interface GlassCardProps {
  children: ReactNode;
  className?: string;
}

export default function GlassCard({ children, className }: GlassCardProps) {
  return (
    <div className={`glass-card ${className ?? ""}`.trim()}>
      <div className="glass-card__content">{children}</div>
    </div>
  );
}
