"use client";

import { useEffect, useRef, useState } from "react";

interface MermaidDiagramProps {
  code: string;
  id?: string;
}

export function MermaidDiagram({ code, id }: MermaidDiagramProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const svgRef = useRef<SVGSVGElement | null>(null);
  const renderedRef = useRef<boolean>(false);

  useEffect(() => {
    if (!containerRef.current || renderedRef.current) return;

    const renderMermaid = async () => {
      try {
        // Carrega Mermaid dinamicamente
        const mermaid = (await import("mermaid")).default;
        
        // Configuracao do Mermaid no estilo do devportal
        mermaid.initialize({
          startOnLoad: false,
          theme: "dark",
          themeVariables: {
            primaryColor: "#4dd4a8", // Verde agua do devportal
            primaryTextColor: "#ffffff",
            primaryBorderColor: "#4dd4a8",
            lineColor: "#7dd3ff", // Azul claro do devportal
            secondaryColor: "#2a2a2a",
            tertiaryColor: "#3a3a3a",
            background: "#1a1a1a",
            mainBkg: "#2a2a2a",
            secondBkg: "#3a3a3a",
            textColor: "#ffffff",
            border1: "#4dd4a8",
            border2: "#7dd3ff",
            noteBkgColor: "#2a2a2a",
            noteTextColor: "#ffffff",
            noteBorderColor: "#4dd4a8",
            actorBorder: "#4dd4a8",
            actorBkg: "#2a2a2a",
            actorTextColor: "#ffffff",
            actorLineColor: "#7dd3ff",
            signalColor: "#7dd3ff",
            signalTextColor: "#ffffff",
            labelBoxBkgColor: "#2a2a2a",
            labelBoxBorderColor: "#4dd4a8",
            labelTextColor: "#ffffff",
            loopTextColor: "#ffffff",
            activationBorderColor: "#4dd4a8",
            activationBkgColor: "#2a2a2a",
            sequenceNumberColor: "#ffffff",
          },
          flowchart: {
            nodeSpacing: 50,
            rankSpacing: 80,
            curve: "basis",
            padding: 20,
          },
          fontFamily: "var(--font-inter), system-ui, sans-serif",
        });

        // Gera ID unico se nao fornecido
        const diagramId = id || `mermaid-${Math.random().toString(36).substr(2, 9)}`;
        
        // Renderiza o diagrama usando a API moderna do Mermaid
        const { svg } = await mermaid.render(diagramId, code);
        if (containerRef.current) {
          containerRef.current.innerHTML = svg;
          const svgElement = containerRef.current.querySelector('svg');
          if (svgElement) {
            svgRef.current = svgElement;
            // Estiliza o SVG
            svgElement.style.maxWidth = '100%';
            svgElement.style.height = 'auto';
          }
          renderedRef.current = true;
        }
      } catch (error) {
        console.error("Erro ao renderizar diagrama Mermaid:", error);
        if (containerRef.current) {
          containerRef.current.innerHTML = `<pre class="text-red-400 p-4 bg-red-900/20 rounded">Erro ao renderizar diagrama Mermaid: ${error}</pre>`;
        }
      }
    };

    renderMermaid();
  }, [code, id]);


  return (
    <>
      <div className="relative my-8 group">
        <div 
          ref={containerRef} 
          className="mermaid-diagram-container p-4 bg-forest-900/50 dark:bg-forest-950 rounded-xl border border-forest-800 dark:border-forest-800 overflow-x-auto"
          style={{ minHeight: "300px" }}
        />
        {/* Botao de tela cheia - abre nova janela */}
        <button
          onClick={(e) => {
            e.preventDefault();
            e.stopPropagation();
            // Salva código no localStorage para passar para a nova janela
            localStorage.setItem('mermaid-fullscreen-code', code);
            // Abre nova janela
            const width = window.screen.width * 0.9;
            const height = window.screen.height * 0.9;
            const left = (window.screen.width - width) / 2;
            const top = (window.screen.height - height) / 2;
            window.open(
              '/wiki/mermaid/fullscreen',
              'mermaid-fullscreen',
              `width=${width},height=${height},left=${left},top=${top},resizable=yes,scrollbars=yes`
            );
          }}
          className="absolute top-2 right-2 bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded-lg px-3 py-2 text-sm font-semibold shadow-lg transition-all opacity-0 group-hover:opacity-100 z-10 flex items-center gap-2"
          aria-label="Abrir em tela cheia em nova janela"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3" />
          </svg>
          Tela Cheia
        </button>
      </div>
    </>
  );
}
