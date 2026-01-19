import Link from 'next/link';
import { ArrowRight } from 'lucide-react';

interface NextStep {
  label: string;
  href: string;
  description?: string;
}

interface NextStepsProps {
  steps: NextStep[];
}

/**
 * Componente padronizado de Próximos Passos
 * Usado ao final de documentos para guiar navegação
 */
export function NextSteps({ steps }: NextStepsProps) {
  if (!steps || steps.length === 0) return null;

  return (
    <div className="glass-card mt-12 mb-8">
      <h2 className="text-2xl font-semibold text-forest-900 dark:text-forest-50 mb-4">
        Próximos Passos
      </h2>
      <p className="text-sm text-forest-600 dark:text-forest-400 mb-6">
        Continue explorando a documentação:
      </p>
      <ul className="space-y-3">
        {steps.map((step, index) => (
          <li key={index}>
            <Link
              href={step.href}
              className="flex items-start gap-3 group p-3 rounded-lg hover:bg-forest-50 dark:hover:bg-forest-900/50 transition-colors"
            >
              <span className="flex-shrink-0 w-6 h-6 rounded-full bg-forest-200 dark:bg-forest-800 text-forest-700 dark:text-forest-300 text-xs font-medium flex items-center justify-center mt-0.5">
                {index + 1}
              </span>
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2 mb-1">
                  <span className="text-base font-medium text-forest-900 dark:text-forest-50 group-hover:text-forest-700 dark:group-hover:text-forest-200 transition-colors">
                    {step.label}
                  </span>
                  <ArrowRight className="w-4 h-4 text-forest-400 dark:text-forest-500 opacity-0 group-hover:opacity-100 transition-opacity" />
                </div>
                {step.description && (
                  <p className="text-sm text-forest-600 dark:text-forest-400">
                    {step.description}
                  </p>
                )}
              </div>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
}
