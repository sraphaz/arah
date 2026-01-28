import { SocialIcon } from "./SocialIcon";
import Link from "next/link";

type SocialLinkProps = {
  platform: "github" | "discord";
  href: string;
  label: string;
  external?: boolean;
  iconSize?: number;
  className?: string;
};

/**
 * Componente reutilizável para links de redes sociais com ícone
 * Suporta links internos (Next.js Link) e externos (<a>)
 */
export function SocialLink({
  platform,
  href,
  label,
  external = true,
  iconSize = 20,
  className = "",
}: SocialLinkProps) {
  const baseClasses = "inline-flex items-center gap-2 transition-transform hover:scale-105 text-forest-700 dark:text-forest-300 hover:text-forest-900 dark:hover:text-forest-100";

  const content = (
    <>
      <SocialIcon platform={platform} size={iconSize} />
      <span>{label}</span>
    </>
  );

  if (external) {
    return (
      <a
        href={href}
        target="_blank"
        rel="noopener noreferrer"
        className={`${baseClasses} ${className}`}
      >
        {content}
      </a>
    );
  }

  return (
    <Link href={href} prefetch={false} className={`${baseClasses} ${className}`}>
      {content}
    </Link>
  );
}
