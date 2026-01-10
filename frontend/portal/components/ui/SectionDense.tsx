import type { ReactNode } from "react";

interface SectionDenseProps {
  children: ReactNode;
  className?: string;
  id?: string;
}

export default function SectionDense({ children, className, id }: SectionDenseProps) {
  return (
    <section id={id} className={`section section-dense ${className ?? ""}`.trim()}>
      <div className="section-inner container-max">{children}</div>
    </section>
  );
}
