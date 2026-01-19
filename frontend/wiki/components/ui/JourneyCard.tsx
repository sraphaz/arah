import Link from 'next/link';
import { ArrowRight } from 'lucide-react';
import { Journey, JourneyStep } from '../../lib/journeys';

interface JourneyCardProps {
  journey: Journey;
}

export function JourneyCard({ journey }: JourneyCardProps) {
  return (
    <div className="glass-card">
      <div className="mb-4">
        <h3 className="text-xl font-semibold text-forest-900 dark:text-forest-50 mb-2">
          {journey.title}
        </h3>
        <p className="text-sm text-forest-600 dark:text-forest-400">
          {journey.description}
        </p>
      </div>
      <ol className="space-y-3">
        {journey.steps.map((step, index) => (
          <li key={step.doc} className="flex items-start gap-3">
            <span className="flex-shrink-0 w-6 h-6 rounded-full bg-forest-200 dark:bg-forest-800 text-forest-700 dark:text-forest-300 text-xs font-medium flex items-center justify-center mt-0.5">
              {index + 1}
            </span>
            <div className="flex-1 min-w-0">
              <Link
                href={`/docs/${step.doc}`}
                className="text-base font-medium text-forest-900 dark:text-forest-50 hover:text-forest-700 dark:hover:text-forest-200 transition-colors block mb-1"
              >
                {step.label}
              </Link>
              {step.description && (
                <p className="text-sm text-forest-600 dark:text-forest-400">
                  {step.description}
                </p>
              )}
            </div>
          </li>
        ))}
      </ol>
    </div>
  );
}
