/**
 * Utilitários para manipulação de classes CSS
 * Função cn (className) para combinar classes condicionalmente
 */

import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

/**
 * Combina classes CSS usando clsx e tailwind-merge
 * Útil para combinar classes condicionalmente e resolver conflitos do Tailwind
 */
export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
