"use client";

import { useEffect, useRef, useState, Suspense } from "react";
import { useSearchParams } from "next/navigation";

function MermaidFullscreenContent() {
  const searchParams = useSearchParams();
  const containerRef = useRef<HTMLDivElement>(null);
  const svgRef = useRef<SVGSVGElement | null>(null);
  const [zoom, setZoom] = useState(1);
  const [pan, setPan] = useState({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState({ x: 0, y: 0 });
  const [code, setCode] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  // Carrega código do localStorage ou query params
  useEffect(() => {
    const codeFromParams = searchParams.get('code');
    if (codeFromParams) {
      try {
        setCode(decodeURIComponent(codeFromParams));
      } catch (e) {
        setError('Erro ao decodificar código do diagrama');
      }
    } else {
      // Tenta carregar do localStorage
      const storedCode = localStorage.getItem('mermaid-fullscreen-code');
      if (storedCode) {
        setCode(storedCode);
        // Limpa após usar
        localStorage.removeItem('mermaid-fullscreen-code');
      } else {
        setError('Código do diagrama não encontrado');
      }
    }
  }, [searchParams]);

  // Renderiza o diagrama Mermaid
  useEffect(() => {
    if (!containerRef.current || !code) return;

    const renderMermaid = async () => {
      try {
        // Carrega Mermaid dinamicamente
        const mermaid = (await import("mermaid")).default;
        
        // Configuracao do Mermaid no estilo do devportal
        mermaid.initialize({
          startOnLoad: false,
          theme: "dark",
          themeVariables: {
            primaryColor: "#4dd4a8",
            primaryTextColor: "#ffffff",
            primaryBorderColor: "#4dd4a8",
            lineColor: "#7dd3ff",
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

        const diagramId = `mermaid-fullscreen-${Math.random().toString(36).substr(2, 9)}`;
        const { svg } = await mermaid.render(diagramId, code);
        
        if (containerRef.current) {
          containerRef.current.innerHTML = svg;
          const svgElement = containerRef.current.querySelector('svg');
          if (svgElement) {
            svgRef.current = svgElement;
            svgElement.style.maxWidth = '100%';
            svgElement.style.height = 'auto';
            svgElement.style.transform = `translate(${pan.x}px, ${pan.y}px) scale(${zoom})`;
            svgElement.style.transformOrigin = 'center center';
            svgElement.style.transition = 'transform 0.1s ease-out';
          }
        }
      } catch (error) {
        console.error("Erro ao renderizar diagrama Mermaid:", error);
        setError(`Erro ao renderizar diagrama: ${error}`);
      }
    };

    renderMermaid();
  }, [code]);

  // Atualiza transformações quando zoom ou pan mudam
  useEffect(() => {
    if (svgRef.current) {
      svgRef.current.style.transform = `translate(${pan.x}px, ${pan.y}px) scale(${zoom})`;
    }
  }, [zoom, pan]);

  const handleZoomIn = () => {
    setZoom(prev => Math.min(prev + 0.2, 3));
  };

  const handleZoomOut = () => {
    setZoom(prev => Math.max(prev - 0.2, 0.5));
  };

  const handleResetZoom = () => {
    setZoom(1);
    setPan({ x: 0, y: 0 });
  };

  const handleClose = () => {
    window.close();
  };

  const handleMouseDown = (e: React.MouseEvent) => {
    e.preventDefault();
    if (e.button === 0) {
      setIsDragging(true);
      setDragStart({ x: e.clientX - pan.x, y: e.clientY - pan.y });
    }
  };

  const handleMouseMove = (e: MouseEvent) => {
    if (isDragging) {
      setPan({
        x: e.clientX - dragStart.x,
        y: e.clientY - dragStart.y,
      });
    }
  };

  const handleMouseUp = () => {
    setIsDragging(false);
  };

  const handleWheel = (e: WheelEvent) => {
    e.preventDefault();
    e.stopPropagation();
    const delta = e.deltaY > 0 ? -0.1 : 0.1;
    setZoom(prev => Math.max(0.5, Math.min(3, prev + delta)));
  };

  // Event listeners para drag e zoom
  useEffect(() => {
    const mouseMoveHandler = (e: MouseEvent) => {
      e.preventDefault();
      handleMouseMove(e);
    };
    const mouseUpHandler = () => handleMouseUp();
    const wheelHandler = (e: WheelEvent) => handleWheel(e);
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        handleClose();
      }
    };

    document.addEventListener('mousemove', mouseMoveHandler, { passive: false });
    document.addEventListener('mouseup', mouseUpHandler);
    document.addEventListener('wheel', wheelHandler, { passive: false });
    document.addEventListener('keydown', handleEsc);

    return () => {
      document.removeEventListener('mousemove', mouseMoveHandler);
      document.removeEventListener('mouseup', mouseUpHandler);
      document.removeEventListener('wheel', wheelHandler);
      document.removeEventListener('keydown', handleEsc);
    };
  }, [isDragging, dragStart]);

  if (error) {
    return (
      <div className="min-h-screen bg-forest-950 flex items-center justify-center p-4">
        <div className="bg-forest-900 rounded-lg p-6 border border-forest-800 max-w-md">
          <h1 className="text-xl font-bold text-forest-50 mb-4">Erro</h1>
          <p className="text-forest-300 mb-4">{error}</p>
          <button
            onClick={handleClose}
            className="px-4 py-2 bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded font-semibold"
          >
            Fechar
          </button>
        </div>
      </div>
    );
  }

  if (!code) {
    return (
      <div className="min-h-screen bg-forest-950 flex items-center justify-center">
        <div className="text-forest-300">Carregando diagrama...</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-forest-950 relative overflow-hidden">
      {/* Botão de fechar */}
      <button
        onClick={handleClose}
        className="fixed top-4 right-4 z-[10000] bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded-full w-12 h-12 flex items-center justify-center shadow-lg transition-all hover:scale-110"
        aria-label="Fechar"
      >
        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
          <line x1="18" y1="6" x2="6" y2="18"></line>
          <line x1="6" y1="6" x2="18" y2="18"></line>
        </svg>
      </button>

      {/* Controles de zoom */}
      <div className="fixed top-4 left-4 z-[10000] flex flex-col gap-2 bg-forest-900/90 dark:bg-forest-800/90 rounded-lg p-2 shadow-lg border border-forest-800">
        <button
          onClick={(e) => {
            e.preventDefault();
            e.stopPropagation();
            handleZoomIn();
          }}
          className="px-3 py-2 bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded font-semibold transition-all"
          aria-label="Zoom in"
        >
          +
        </button>
        <button
          onClick={(e) => {
            e.preventDefault();
            e.stopPropagation();
            handleZoomOut();
          }}
          className="px-3 py-2 bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded font-semibold transition-all"
          aria-label="Zoom out"
        >
          −
        </button>
        <button
          onClick={(e) => {
            e.preventDefault();
            e.stopPropagation();
            handleResetZoom();
          }}
          className="px-3 py-2 bg-[#7dd3ff] hover:bg-[#8de3ff] text-forest-950 rounded text-xs font-semibold transition-all"
          aria-label="Resetar zoom"
        >
          Reset
        </button>
        <div className="text-xs text-forest-300 text-center px-2 py-1">
          {Math.round(zoom * 100)}%
        </div>
      </div>

      {/* Instruções */}
      <div className="fixed bottom-4 left-1/2 transform -translate-x-1/2 z-[10000] bg-forest-900/90 dark:bg-forest-800/90 rounded-lg px-4 py-2 text-sm text-forest-300 border border-forest-800">
        <span>Arraste para mover • Roda do mouse para zoom • ESC para fechar</span>
      </div>

      {/* Container do SVG com zoom e pan */}
      <div
        className="w-full h-screen flex items-center justify-center overflow-hidden relative"
        onMouseDown={handleMouseDown}
        style={{ cursor: isDragging ? 'grabbing' : 'grab' }}
      >
        <div
          ref={containerRef}
          className="flex items-center justify-center"
        />
      </div>
    </div>
  );
}

export default function MermaidFullscreenPage() {
  return (
    <Suspense fallback={
      <div className="min-h-screen bg-forest-950 flex items-center justify-center">
        <div className="text-forest-300">Carregando diagrama...</div>
      </div>
    }>
      <MermaidFullscreenContent />
    </Suspense>
  );
}
