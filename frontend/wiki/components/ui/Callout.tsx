import { AlertCircle, Info, CheckCircle, AlertTriangle, XCircle, Lightbulb } from 'lucide-react';
import { cn } from '../../lib/utils';

export type CalloutType = 'info' | 'success' | 'warning' | 'error' | 'tip' | 'note';

interface CalloutProps {
  type: CalloutType;
  title?: string;
  children: React.ReactNode;
  className?: string;
}

const calloutConfig = {
  info: {
    icon: Info,
    bgColor: 'bg-forest-50 dark:bg-forest-950/50',
    borderColor: 'border-forest-300 dark:border-forest-700',
    iconColor: 'text-forest-600 dark:text-forest-400',
    titleColor: 'text-forest-900 dark:text-forest-50',
    textColor: 'text-forest-700 dark:text-forest-300',
  },
  success: {
    icon: CheckCircle,
    bgColor: 'bg-green-50 dark:bg-green-950/30',
    borderColor: 'border-green-300 dark:border-green-700',
    iconColor: 'text-green-600 dark:text-green-400',
    titleColor: 'text-green-900 dark:text-green-50',
    textColor: 'text-green-700 dark:text-green-300',
  },
  warning: {
    icon: AlertTriangle,
    bgColor: 'bg-yellow-50 dark:bg-yellow-950/30',
    borderColor: 'border-yellow-300 dark:border-yellow-700',
    iconColor: 'text-yellow-600 dark:text-yellow-600',
    titleColor: 'text-yellow-900 dark:text-yellow-100',
    textColor: 'text-yellow-700 dark:text-yellow-200',
  },
  error: {
    icon: XCircle,
    bgColor: 'bg-red-50 dark:bg-red-950/30',
    borderColor: 'border-red-300 dark:border-red-700',
    iconColor: 'text-red-600 dark:text-red-400',
    titleColor: 'text-red-900 dark:text-red-50',
    textColor: 'text-red-700 dark:text-red-300',
  },
  tip: {
    icon: Lightbulb,
    bgColor: 'bg-blue-50 dark:bg-blue-950/30',
    borderColor: 'border-blue-300 dark:border-blue-700',
    iconColor: 'text-blue-600 dark:text-blue-400',
    titleColor: 'text-blue-900 dark:text-blue-50',
    textColor: 'text-blue-700 dark:text-blue-300',
  },
  note: {
    icon: AlertCircle,
    bgColor: 'bg-forest-50 dark:bg-forest-950/50',
    borderColor: 'border-forest-300 dark:border-forest-700',
    iconColor: 'text-forest-600 dark:text-forest-400',
    titleColor: 'text-forest-900 dark:text-forest-50',
    textColor: 'text-forest-700 dark:text-forest-300',
  },
};

/**
 * Componente Callout para destacar informações importantes
 * Usa cores semânticas do design system
 */
export function Callout({ type, title, children, className }: CalloutProps) {
  const config = calloutConfig[type];
  const Icon = config.icon;

  return (
    <div
      className={cn(
        'my-6 rounded-lg border-l-4 p-4',
        config.bgColor,
        config.borderColor,
        className
      )}
      role="alert"
      aria-label={title || `${type} callout`}
    >
      <div className="flex items-start gap-3">
        <Icon className={cn('w-5 h-5 flex-shrink-0 mt-0.5', config.iconColor)} aria-hidden="true" />
        <div className="flex-1 min-w-0">
          {title && (
            <h4 className={cn('font-semibold mb-2', config.titleColor)}>{title}</h4>
          )}
          <div className={cn('text-sm leading-relaxed', config.textColor)}>{children}</div>
        </div>
      </div>
    </div>
  );
}
