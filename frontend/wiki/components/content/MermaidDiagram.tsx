"use client";

import { useEffect, useRef } from "react";

interface MermaidDiagramProps {
  code: string;
  id?: string;
}

export function MermaidDiagram({ code, id }: MermaidDiagramProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const renderedRef = useRef<boolean>(false);

  useEffect(() => {
    if (!containerRef.current || renderedRef.current) return;

    const renderMermaid = async () => {
      try {
        // Carrega Mermaid dinamicamente
        const mermaid = (await import("mermaid")).default;
        
        // Configuração do Mermaid no estilo do devportal
        mermaid.initialize({
          startOnLoad: false,
          theme: "dark",
          themeVariables: {
            primaryColor: "#4dd4a8", // Verde água do devportal
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

        // Gera ID único se não fornecido
        const diagramId = id || `mermaid-${Math.random().toString(36).substr(2, 9)}`;
        
        // Renderiza o diagrama usando a API moderna do Mermaid
        const { svg } = await mermaid.render(diagramId, code);
        if (containerRef.current) {
          containerRef.current.innerHTML = svg;
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
    <div 
      ref={containerRef} 
      className="mermaid-diagram-container my-8 p-4 bg-forest-900/50 dark:bg-forest-950 rounded-xl border border-forest-800 dark:border-forest-800 overflow-x-auto"
      style={{ minHeight: "300px" }}
    />
  );
}
