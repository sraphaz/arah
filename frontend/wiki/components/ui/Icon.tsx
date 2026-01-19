import { LucideIcon, Code, Eye, BookOpen, MapPin, Users, Sprout, Rocket, Globe, ArrowRight, X, ChevronDown, ChevronUp, ExternalLink } from "lucide-react";
import { cn } from "../../lib/utils";

export type IconName = 
  | "code" // Desenvolvedores
  | "eye" // Analistas
  | "book-open" // Documentação/Índice
  | "map-pin" // Território/Localização
  | "users" // Pessoas/Comunidade
  | "sprout" // Crescimento/Autonomia
  | "rocket" // Em Breve/Lançamento
  | "globe" // Site/Internet
  | "arrow-right" // Navegação/Explorar
  | "x" // Fechar
  | "chevron-down" // Expandir
  | "chevron-up" // Colapsar
  | "external-link"; // Link externo

const iconMap: Record<IconName, LucideIcon> = {
  "code": Code,
  "eye": Eye,
  "book-open": BookOpen,
  "map-pin": MapPin,
  "users": Users,
  "sprout": Sprout,
  "rocket": Rocket,
  "globe": Globe,
  "arrow-right": ArrowRight,
  "x": X,
  "chevron-down": ChevronDown,
  "chevron-up": ChevronUp,
  "external-link": ExternalLink,
};

interface IconProps {
  name: IconName;
  size?: number;
  className?: string;
  strokeWidth?: number;
}

/**
 * Componente de ícone SVG monocromático usando Lucide Icons
 * Mantém identidade visual consistente - ícones sem cores, apenas formas
 */
export function Icon({ name, size = 24, className = "", strokeWidth = 2 }: IconProps) {
  const IconComponent = iconMap[name];
  
  if (!IconComponent) {
    console.warn(`Icon "${name}" not found in iconMap`);
    return null;
  }

  return (
    <IconComponent
      size={size}
      className={cn("text-current", className)}
      strokeWidth={strokeWidth}
      aria-hidden="true"
    />
  );
}
