import type { ReactNode } from "react";

interface SectionProps {
  children: ReactNode;
  className?: string;
  id?: string;
}

export default function Section({ children, className, id }: SectionProps) {
  return (
    <section id={id} className={`section ${className ?? ""}`.trim()}>
      <div className="section-inner container-max">{children}</div>
    </section>
  );
}
