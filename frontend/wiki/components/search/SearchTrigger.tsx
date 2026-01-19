'use client';

import { useState, useEffect } from 'react';
import { Search } from 'lucide-react';
import { SearchDialog } from './SearchDialog';

export function SearchTrigger() {
  const [isOpen, setIsOpen] = useState(false);

  // Atalho Cmd/Ctrl + K
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
        e.preventDefault();
        setIsOpen(true);
      }
      if (e.key === 'Escape' && isOpen) {
        setIsOpen(false);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [isOpen]);

  return (
    <>
      <button
        onClick={() => setIsOpen(true)}
        className="flex items-center gap-2 px-4 py-2 rounded-lg bg-forest-100 dark:bg-forest-900 text-forest-700 dark:text-forest-300 hover:bg-forest-200 dark:hover:bg-forest-800 transition-colors text-sm font-medium"
        aria-label="Abrir busca (Cmd/Ctrl + K)"
      >
        <Search className="w-4 h-4" />
        <span className="hidden sm:inline">Buscar</span>
        <kbd className="hidden sm:inline-flex items-center gap-1 px-1.5 py-0.5 text-xs bg-forest-200 dark:bg-forest-800 rounded">
          <span className="text-[10px]">⌘</span>K
        </kbd>
      </button>
      <SearchDialog isOpen={isOpen} onClose={() => setIsOpen(false)} />
    </>
  );
}
