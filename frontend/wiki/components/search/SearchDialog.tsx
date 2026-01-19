'use client';

import { useState, useEffect, useRef, useCallback } from 'react';
import { Search, FileText, ArrowRight, X } from 'lucide-react';
import Fuse from 'fuse.js';
import { WikiSearchItem } from '../../lib/search-index';

interface SearchDialogProps {
  isOpen: boolean;
  onClose: () => void;
}

export function SearchDialog({ isOpen, onClose }: SearchDialogProps) {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<WikiSearchItem[]>([]);
  const [index, setIndex] = useState<WikiSearchItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const inputRef = useRef<HTMLInputElement>(null);
  const resultsRef = useRef<HTMLDivElement>(null);

  // Carrega índice de busca (asset estático para export estático)
  useEffect(() => {
    if (isOpen && index.length === 0) {
      setLoading(true);
      // Carrega do arquivo JSON estático gerado no build time
      fetch('/wiki/search-index.json')
        .then(res => {
          if (!res.ok) {
            throw new Error(`Failed to load search index: ${res.status}`);
          }
          return res.json();
        })
        .then(data => {
          setIndex(data.index || []);
          setLoading(false);
        })
        .catch(err => {
          console.error('Error loading search index:', err);
          console.warn('Search index not available - search will be empty');
          setIndex([]);
          setLoading(false);
        });
    }
  }, [isOpen, index.length]);

  // Busca com Fuse.js
  useEffect(() => {
    if (!query.trim() || index.length === 0) {
      setResults([]);
      return;
    }

    const fuse = new Fuse(index, {
      keys: [
        { name: 'title', weight: 0.7 },
        { name: 'content', weight: 0.3 },
      ],
      threshold: 0.4,
      distance: 100,
      minMatchCharLength: 2,
      includeScore: true,
    });

    const searchResults = fuse.search(query);
    setResults(searchResults.slice(0, 10).map(result => result.item));
    setSelectedIndex(0);
  }, [query, index]);

  // Foco no input quando abre
  useEffect(() => {
    if (isOpen && inputRef.current) {
      inputRef.current.focus();
    }
  }, [isOpen]);

  // Navegação por teclado
  const handleKeyDown = useCallback((e: React.KeyboardEvent) => {
    if (e.key === 'ArrowDown') {
      e.preventDefault();
      setSelectedIndex(prev => Math.min(prev + 1, results.length - 1));
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      setSelectedIndex(prev => Math.max(prev - 1, 0));
    } else if (e.key === 'Enter' && results[selectedIndex]) {
      e.preventDefault();
      window.location.href = results[selectedIndex].url;
    } else if (e.key === 'Escape') {
      onClose();
    }
  }, [results, selectedIndex, onClose]);

  // Scroll para resultado selecionado
  useEffect(() => {
    if (resultsRef.current && selectedIndex >= 0) {
      const selectedElement = resultsRef.current.children[selectedIndex] as HTMLElement;
      if (selectedElement) {
        selectedElement.scrollIntoView({ block: 'nearest' });
      }
    }
  }, [selectedIndex]);

  if (!isOpen) return null;

  return (
    <div
      className="fixed inset-0 z-50 flex items-start justify-center pt-[10vh] px-4"
      onClick={onClose}
      role="dialog"
      aria-modal="true"
      aria-label="Busca de documentação"
    >
      {/* Overlay */}
      <div className="fixed inset-0 bg-black/50 backdrop-blur-sm" />

      {/* Dialog */}
      <div
        className="relative w-full max-w-2xl glass-card"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Input */}
        <div className="flex items-center gap-3 p-4 border-b border-forest-200 dark:border-forest-800">
          <Search className="w-5 h-5 text-forest-500 dark:text-forest-400 flex-shrink-0" />
          <input
            ref={inputRef}
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Buscar documentação... (Cmd/Ctrl + K)"
            className="flex-1 bg-transparent border-0 outline-0 text-forest-900 dark:text-forest-50 placeholder-forest-500 dark:placeholder-forest-400 text-base"
            aria-label="Campo de busca"
          />
          <button
            onClick={onClose}
            className="p-1 hover:bg-forest-100 dark:hover:bg-forest-900 rounded transition-colors"
            aria-label="Fechar busca"
          >
            <X className="w-5 h-5 text-forest-500 dark:text-forest-400" />
          </button>
        </div>

        {/* Results */}
        <div
          ref={resultsRef}
          className="max-h-[60vh] overflow-y-auto"
          role="listbox"
          aria-label="Resultados da busca"
        >
          {loading ? (
            <div className="p-8 text-center text-forest-600 dark:text-forest-400">
              Carregando índice de busca...
            </div>
          ) : query.trim() && results.length === 0 ? (
            <div className="p-8 text-center text-forest-600 dark:text-forest-400">
              Nenhum resultado encontrado para &quot;{query}&quot;
            </div>
          ) : results.length > 0 ? (
            results.map((result, index) => (
              <a
                key={result.id}
                href={result.url}
                className={`flex items-start gap-4 p-4 hover:bg-forest-50 dark:hover:bg-forest-900/50 transition-colors ${
                  index === selectedIndex ? 'bg-forest-100 dark:bg-forest-900/50' : ''
                }`}
                role="option"
                aria-selected={index === selectedIndex}
              >
                <FileText className="w-5 h-5 text-forest-500 dark:text-forest-400 flex-shrink-0 mt-0.5" />
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-medium text-forest-900 dark:text-forest-50 truncate">
                      {result.title}
                    </h3>
                    {result.category && (
                      <span className="px-2 py-0.5 text-xs font-medium text-forest-600 dark:text-forest-400 bg-forest-100 dark:bg-forest-900 rounded">
                        {result.category}
                      </span>
                    )}
                  </div>
                  {result.description && (
                    <p className="text-sm text-forest-600 dark:text-forest-400 line-clamp-2">
                      {result.description}
                    </p>
                  )}
                </div>
                <ArrowRight className="w-4 h-4 text-forest-400 dark:text-forest-500 flex-shrink-0" />
              </a>
            ))
          ) : (
            <div className="p-8 text-center text-forest-600 dark:text-forest-400">
              Digite para buscar na documentação...
            </div>
          )}
        </div>

        {/* Footer */}
        {results.length > 0 && (
          <div className="flex items-center justify-between gap-4 p-3 border-t border-forest-200 dark:border-forest-800 text-xs text-forest-500 dark:text-forest-400">
            <div className="flex items-center gap-4">
              <span className="flex items-center gap-1">
                <kbd className="px-1.5 py-0.5 bg-forest-100 dark:bg-forest-900 rounded">↑↓</kbd>
                <span>navegar</span>
              </span>
              <span className="flex items-center gap-1">
                <kbd className="px-1.5 py-0.5 bg-forest-100 dark:bg-forest-900 rounded">Enter</kbd>
                <span>abrir</span>
              </span>
            </div>
            <span className="flex items-center gap-1">
              <kbd className="px-1.5 py-0.5 bg-forest-100 dark:bg-forest-900 rounded">Esc</kbd>
              <span>fechar</span>
            </span>
          </div>
        )}
      </div>
    </div>
  );
}
